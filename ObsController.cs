#nullable disable
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
        public string Name => "obs";

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private ClientWebSocket _webSocket;
        private bool _isConnected = false;
        private bool _socketInitialized = false;
        private string _sceneCollection = "";
        public string Password = "uLolcGDhzMkYVzro";
        public bool IsEnabled => _isConnected;

        private readonly object _connectionLock = new object();
        public event EventHandler<ControllerEventArgs> ControllerEvent;
        private readonly Dictionary<string, Func<string[], Task>> _commandMap;
        private readonly ConcurrentDictionary<SceneSourceKey, int> _cache = new ConcurrentDictionary<SceneSourceKey, int>();
        
        private readonly Dictionary<string, int> _sceneItemIdCache = new Dictionary<string, int>();
        private TaskCompletionSource<JObject> _pendingResponse;
        private string _pendingRequestId;
        private readonly TimeSpan _requestTimeout = TimeSpan.FromSeconds(5);

        public void OpenFile(string path)
        {
            throw new NotSupportedException("OBS does not open files directly.");
        }

        public readonly struct SceneSourceKey : IEquatable<SceneSourceKey>
        {
            public string Scene { get; }
            public string Source { get; }

            public SceneSourceKey(string scene, string source)
            {
                Scene = scene;
                Source = source;
            }

            public bool Equals(SceneSourceKey other) =>
                string.Equals(Scene, other.Scene, StringComparison.Ordinal) &&
                string.Equals(Source, other.Source, StringComparison.Ordinal);

            public override bool Equals(object obj) =>
                obj is SceneSourceKey other && Equals(other);

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + (Scene?.GetHashCode() ?? 0);
                    hash = hash * 23 + (Source?.GetHashCode() ?? 0);
                    return hash;
                }
            }
        }

        // ---------------------------------------------------------
        // Constructor
        // ---------------------------------------------------------
        public ObsController(string sceneCollection = null)
        {
            _sceneCollection = sceneCollection;
            // Start background worker

            _commandMap = new Dictionary<string, Func<string[], Task>>(StringComparer.OrdinalIgnoreCase)
            {
                { "obs_mute", args =>
                  {
                      SendRequest("SetInputMute", new JObject {
                          ["inputName"] = args[0],
                          ["inputMuted"] = true
                      });
                      return Task.CompletedTask;
                  }
                },
                { "obs_unmute", args =>
                  {
                      SendRequest("SetInputMute", new JObject {
                          ["inputName"] = args[0],
                          ["inputMuted"] = false
                      });
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
                        _ = HandleSourceVisibility(args[0], args[1], true);
                        return Task.CompletedTask;
                    }
                },

                { "obs_source_hide", args =>
                    {
                        _ = HandleSourceVisibility(args[0], args[1], false);
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

                    OnControllerEvent(Name, "Connected", ControllerEventType.Info);

                    StartReceiveLoop();
                }
            }
            catch (Exception ex)
            {
                SafeDispose();

                if (!retryIfObsNotRunning)
                {
                    OnControllerEvent(Name, "Connection failed: " + ex.Message, ControllerEventType.Error);
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
                OnControllerEvent(Name, "Connection failed: " + ex.Message, ControllerEventType.Error);
                _isConnected = false;
            }
        }
        private void SafeDispose()
        {
            var ws = Interlocked.Exchange(ref _webSocket, null);

            if (ws != null)
            {
                try
                {
                    if (ws != null)
                    {
                        if (_socketInitialized &&
                            (ws.State == WebSocketState.Open ||
                             ws.State == WebSocketState.CloseReceived ||
                             ws.State == WebSocketState.CloseSent))
                        {
                            ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None)
                               .Wait();
                        }

                        ws.Dispose();
                    }
                }
                catch
                {
                    // swallow
                }
            }
            _socketInitialized = false;
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

        private async Task<int> ResolveSceneItemId(string scene, string source)
        {
            string key = $"{scene}::{source}";

            // 1. Cache check
            if (_sceneItemIdCache.TryGetValue(key, out int cachedId))
                return cachedId;

            try
            {
                // 2. Build request payload
                var requestData = new JObject
                {
                    ["sceneName"] = scene,
                    ["sourceName"] = source
                };

                // 3. Ask OBS for the ID
                JObject response = await SendRequestAsync("GetSceneItemId", requestData);

                // 4. Extract ID safely
                int id = response["responseData"]?["sceneItemId"]?.Value<int>() ?? 0;

                if (id <= 0)
                    throw new Exception($"OBS returned invalid sceneItemId for {scene}/{source}");

                // 5. Cache it
                _sceneItemIdCache[key] = id;

                return id;
            }
            catch (TimeoutException)
            {
                throw new Exception($"Timed out resolving sceneItemId for {scene}/{source}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to resolve sceneItemId for {scene}/{source}: {ex.Message}");
            }
        }
        private async Task<JObject> SendRequestAsync(string requestType, JObject requestData = null)
        {
            // 1. Create a new requestId
            string requestId = Guid.NewGuid().ToString();

            // 2. Build the request envelope
            var request = new JObject
            {
                ["op"] = 6, // Request
                ["d"] = new JObject
                {
                    ["requestType"] = requestType,
                    ["requestId"] = requestId,
                    ["requestData"] = requestData ?? new JObject()
                }
            };

            // 3. Prepare to wait for the response
            _pendingRequestId = requestId;
            _pendingResponse = new TaskCompletionSource<JObject>(TaskCreationOptions.RunContinuationsAsynchronously);

            // 4. Send the request
            await SendMessageAsync(request.ToString());

            // 5. Wait for the response OR timeout
            using (var cts = new CancellationTokenSource(_requestTimeout))
            {
                var completed = await Task.WhenAny(_pendingResponse.Task, Task.Delay(Timeout.Infinite, cts.Token));

                if (completed != _pendingResponse.Task)
                {
                    throw new TimeoutException($"OBS request '{requestType}' with id '{requestId}' timed out.");
                }
            }

            JObject response = await _pendingResponse.Task;

            // 6. Optional: check for OBS-side errors
            var status = response["requestStatus"];
            if (status != null)
            {
                bool ok = status["result"]?.Value<bool>() ?? true;
                if (!ok)
                {
                    string code = status["code"]?.ToString() ?? "unknown";
                    string comment = status["comment"]?.ToString() ?? "no details";
                    throw new Exception($"OBS request '{requestType}' failed: {code} - {comment}");
                }
            }

            return response;
        }

        private async Task<int> GetSceneItemId(string sceneName, string sourceName)
        {
            // Build the request
            var request = new JObject
            {
                ["sceneName"] = sceneName,
                ["sourceName"] = sourceName
            };

            // Send the request and wait for the response
            JObject response = await SendRequestAsync("GetSceneItemId", request);

            // Extract the ID
            int id = response["sceneItemId"].Value<int>();

            return id;
        }

        private async Task HandleSourceVisibility(string scene, string source, bool enabled)
        {
            try
            {
                int id = await ResolveSceneItemId(scene, source);

                SendRequest("SetSceneItemEnabled", new JObject
                {
                    ["sceneName"] = scene,
                    ["sceneItemId"] = id,
                    ["sceneItemEnabled"] = enabled
                });
            }
            catch (Exception ex)
            {
                // Optional: log or surface this
                OnControllerEvent(Name, $"Error setting visibility: {ex.Message}", ControllerEventType.Error);
            }
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

        }

        public bool IsConnected => _isConnected;

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
                Process.Start(startInfo);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void HandleIncomingMessage(string json)
        {
            var msg = JObject.Parse(json);

            int op = msg["op"].Value<int>();
            var d = (JObject)msg["d"];

            // Handle request responses
            if (op == 7) // RequestResponse
            {
                string requestId = d["requestId"].Value<string>();

                if (requestId == _pendingRequestId)
                {
                    _pendingResponse?.TrySetResult(d);
                    return;
                }
            }

            // Later: handle events (op == 5), scene changes, etc.
        }

        private async void StartReceiveLoop()
        {
            try
            {
                while (_webSocket != null && _webSocket.State == WebSocketState.Open)
                {
                    string json = await ReceiveMessageAsync();
                    HandleIncomingMessage(json);
                }
            }
            catch
            {
                // Optional: log or reconnect
                SafeDispose();
            }
        }

    }
}
