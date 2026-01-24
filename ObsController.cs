using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PromptLogic.Controllers
{
    public class ObsController : IController, IDisposable
    {
        private class ObsCommand
        {
            public string CommandName { get; set; }
            public string[] Args { get; set; }

            public ObsCommand(string name, params string[] args)
            {
                CommandName = name;
                Args = args;
            }

            public string ToJson()
            {
                // TODO: Convert to OBS WebSocket JSON format
                return "{}";
            }
        }

        // ---------------------------------------------------------
        // Fields
        // ---------------------------------------------------------

        private Thread _workerThread;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly ConcurrentQueue<ObsCommand> _commandQueue = new ConcurrentQueue<ObsCommand>();
        private ClientWebSocket _webSocket;
        private bool _isConnected = false;
        private bool _socketInitialized = false;
        private string _sceneCollection = "";
        public string Password = "uLolcGDhzMkYVzro";
        public bool IsEnabled => _isConnected;

        private readonly object _connectionLock = new object();
        public event EventHandler<ControllerEventArgs> ControllerEvent;
        private readonly Dictionary<string, Func<string[], Task>> _commandMap;

        // ---------------------------------------------------------
        // Constructor
        // ---------------------------------------------------------

        public ObsController(string sceneCollection = null)
        {
            _sceneCollection = sceneCollection;
            // Start background worker
            _workerThread = new Thread(WorkerLoop)
            {
                IsBackground = true,
                Name = "OBS Worker Thread"
            };
            _workerThread.Start();

            _commandMap = new Dictionary<string, Func<string[], Task>>(StringComparer.OrdinalIgnoreCase)
            {
                { "obs_mute", args =>
                    {
                        SendRequest("ToggleInputMute", new JObject { ["inputName"] = args[0] });
                        return Task.CompletedTask;
                    }
                },

                { "obs_unmute", args =>
                    {
                        SendRequest("ToggleInputMute", new JObject { ["inputName"] = args[0] });
                        return Task.CompletedTask;
                    }
                },

                { "obs_scene", args =>
                    {
                        SendRequest("SetCurrentProgramScene", new JObject { ["sceneName"] = args[0] });
                        return Task.CompletedTask;
                    }
                },

                { "obs_record_start", args =>
                    {
                        SendRequest("StartRecord");
                        return Task.CompletedTask;
                    }
                },

                { "obs_record_stop", args =>
                    {
                        SendRequest("StopRecord");
                        return Task.CompletedTask;
                    }
                },

                { "obs_source_show", args =>
                    {
                        SendRequest("SetSceneItemEnabled", new JObject {
                                                                ["sceneName"] = args[1],
                                                                ["sceneItemId"] = 0,
                                                                ["sceneItemEnabled"] = true
                                                                });
                        return Task.CompletedTask;
                    }
                },

                { "obs_source_hide", args =>
                    {
                        SendRequest("SetSceneItemEnabled", new JObject {
                                                                ["sceneName"] = args[1],
                                                                ["sceneItemId"] = 0,
                                                                ["sceneItemEnabled"] = false });
                        return Task.CompletedTask;
                    }
                },

                { "obs_tranition", args =>
                    {
                        SendRequest("SetCurrentSceneTransition", new JObject { ["transitionName"] = args[0] }); 
                        return Task.CompletedTask;
                    }
                }
            };

        }

        public Task ExecuteCommandAsync(string command, string[] args)
        {
            if (_commandMap.TryGetValue(command, out var handler))
                return handler(args);

            return Task.CompletedTask;
        }

        // ---------------------------------------------------------
        // Public API
        // ---------------------------------------------------------
        public void Enable()
        {
            Task.Run(async () =>
            {
                await AttemptConnectionAsync(true);
            });
        }

        public async Task AttemptConnectionAsync(bool retryIfObsNotRunning)
        {
            try
            {
                _webSocket = new ClientWebSocket();
                _socketInitialized = true;
                await _webSocket.ConnectAsync(new Uri("ws://127.0.0.1:4455"), CancellationToken.None);

                // Step 1: Receive Hello packet
                var helloJson = await ReceiveMessageAsync();
                var hello = JObject.Parse(helloJson);

                var challenge = (string)hello["d"]["authentication"]["challenge"];
                var salt = (string)hello["d"]["authentication"]["salt"];

                // Step 2: Compute authentication response
                string auth = ComputeAuthResponse(Password, salt, challenge);

                // Step 3: Send Identify packet
                var identify = new JObject
                {
                    ["op"] = 1, // Identify
                    ["d"] = new JObject
                    {
                        ["rpcVersion"] = 1,
                        ["authentication"] = auth
                    }
                };

                await SendMessageAsync(identify.ToString());

                // Step 4: Receive Identified packet
                var identifiedJson = await ReceiveMessageAsync();
                var identified = JObject.Parse(identifiedJson);

                if ((int)identified["op"] == 2) // Identified
                {
                    _isConnected = true;
                    
                    if (!string.IsNullOrEmpty(_sceneCollection))
                        SetCurrentSceneCollection(_sceneCollection);

                    OnControllerEvent("obs", "Connected", ControllerEventType.Info); 
                }
            }
            catch (Exception ex)
            {
                SafeDispose();

                if (!retryIfObsNotRunning)
                {
                    OnControllerEvent("obs", "Connection failed: " + ex.Message, ControllerEventType.Error);
                    return;
                }

                // Retry logic begins here
                if (!IsObsRunning())
                {
                    if (TryLaunchObs())
                    {
                        // Give OBS time to initialize WebSocket
                        await Task.Delay(4000);

                        // Retry once, no further retries allowed
                        await AttemptConnectionAsync(retryIfObsNotRunning: false);
                        return;
                    }
                }

                //                    Debug.WriteLine("OBS connection/auth failed: " + ex.Message);
                OnControllerEvent("obs", "Connection failed: " + ex.Message, ControllerEventType.Error);
                _isConnected = false;
            }
        }
        private void SafeDispose()
        {
            try
            {
                if (_webSocket != null)
                {
                    if (_socketInitialized &&
                        (_webSocket.State == WebSocketState.Open ||
                         _webSocket.State == WebSocketState.CloseReceived ||
                         _webSocket.State == WebSocketState.CloseSent))
                    {
                        _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None)
                           .Wait();
                    }

                    _webSocket.Dispose();
                }
            }
            catch
            {
                // swallow
            }
            finally
            {
                _webSocket = null;
                _socketInitialized = false;
            }
        }

        private void ToggleMute(string sourceName)
        {
            var data = new JObject
            {
                ["source"] = sourceName
            };

            SendRequest("ToggleInputMute", data);
        }

        private void SetCurrentProgramScene(string sceneName)
        {
            var data = new JObject
            {
                ["sceneName"] = sceneName
            };

            SendRequest("SetCurrentProgramScene", data);
        }

        private void SetCurrentSceneCollection(string name)
        {
            var data = new JObject
            {
                ["sceneCollectionName"] = name
            };

            SendRequest("SetCurrentSceneCollection", data);
        }

        private void SendRequest(string requestType, JObject requestData = null)
        {
            var request = new JObject
            {
                ["op"] = 6, // Request
                ["d"] = new JObject
                {
                    ["requestType"] = requestType,
                    ["requestId"] = Guid.NewGuid().ToString(),
                    ["requestData"] = requestData ?? new JObject()
                }
            };

            Task.Run(async () =>
            {
                await SendMessageAsync(request.ToString());
            });
        }

        private static string ComputeAuthResponse(string password, string salt, string challenge)
        {
            // secret = Base64(SHA256(password + salt))
            var secretBytes = SHA256.Create().ComputeHash(
                Encoding.UTF8.GetBytes(password + salt)
            );
            string secret = Convert.ToBase64String(secretBytes);

            // auth = Base64(SHA256(secret + challenge))
            var authBytes = SHA256.Create().ComputeHash(
                Encoding.UTF8.GetBytes(secret + challenge)
            );
            return Convert.ToBase64String(authBytes);
        }

        private async Task<string> ReceiveMessageAsync()
        {
            var buffer = new ArraySegment<byte>(new byte[4096]);
            var result = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);
            return Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
        }

        private async Task SendMessageAsync(string json)
        {
            if (_webSocket == null || _webSocket.State != WebSocketState.Open)
                return;

            var bytes = Encoding.UTF8.GetBytes(json);
            var buffer = new ArraySegment<byte>(bytes);

            try
            {
                await _webSocket.SendAsync(
                    buffer,
                    WebSocketMessageType.Text,
                    endOfMessage: true,
                    cancellationToken: CancellationToken.None
                );
            }
            catch
            {
                // Optional: log or trigger reconnect
            }


/*
            var bytes = Encoding.UTF8.GetBytes(json);
            var buffer = new ArraySegment<byte>(bytes);
            await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
*/
        }


        public void EnqueueCommand(string command, params string[] args)
        {
            var obsCmd = new ObsCommand(command, args);
            _commandQueue.Enqueue(obsCmd);
        }

        public bool IsConnected => _isConnected;


        // ---------------------------------------------------------
        // Worker Thread Loop
        // ---------------------------------------------------------

        private void WorkerLoop()
        {
            while (!_cts.IsCancellationRequested)
            {
                if (!_isConnected)
                {
                    Thread.Sleep(500);
                    continue;
                }

                if (_commandQueue.TryDequeue(out var cmd))
                {
                    SendCommand(cmd);
                }

                Thread.Sleep(10);
            }
        }


        // ---------------------------------------------------------
        // Sending Commands
        // ---------------------------------------------------------

        private void SendCommand(ObsCommand cmd)
        {
            if (!_isConnected)
                return;

            try
            {
                string json = cmd.ToJson();
                var buffer = Encoding.UTF8.GetBytes(json);

                _webSocket.SendAsync(
                    new ArraySegment<byte>(buffer),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None
                ).Wait();
            }
            catch
            {
                _isConnected = false;
                // TODO: Log error, trigger reconnect
            }
        }


        // ---------------------------------------------------------
        // Cleanup
        // ---------------------------------------------------------

        public void Dispose()
        {
            _cts.Cancel();

            try { _workerThread.Join(500); } catch { }

            SafeDispose();

        }

        protected virtual void OnControllerEvent(string prefix, string message, ControllerEventType type)
        {
            var handler = ControllerEvent;
            if (handler != null)
            {
                handler(this, new ControllerEventArgs
                {
                    Prefix = prefix,
                    Message = message,
                    Type = type
                });
            }
        }

        private bool IsObsRunning()
        {
            return Process.GetProcessesByName("obs64").Any();
        }

        private string FindObsExecutable()
        {
            // Standard install
            var standard = @"C:\Program Files\obs-studio\bin\64bit\obs64.exe";
            if (File.Exists(standard))
                return standard;

            // Steam install
            var steam = @"C:\Program Files (x86)\Steam\steamapps\common\OBS Studio\bin\64bit\obs64.exe";
            if (File.Exists(steam))
                return steam;

            return null; // Could add registry lookup later
        }

        private bool TryLaunchObs()
        {
            var obsExePath = FindObsExecutable();
            if (obsExePath == null)
                return false;

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = obsExePath, // e.g., C:\Program Files\obs-studio\bin\64bit\obs64.exe
                    WorkingDirectory = Path.GetDirectoryName(obsExePath),
                    UseShellExecute = false
                };
/*
                if (!string.IsNullOrEmpty(_sceneCollection))
                {
                    startInfo.Arguments = $"--scene-collection \"{_sceneCollection}\"";
                }
*/
                Process.Start(startInfo);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
