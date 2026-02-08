using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptLogic.Controllers
{
    public class ControllerManager : IDisposable
    {
        private readonly Dictionary<string, IController> _controllers;
        private ControllerRegistery _controllerRegistry;

        public ControllerManager()
        {
            _controllerRegistry = new ControllerRegistery();

            _controllers = new Dictionary<string, IController>(
                StringComparer.OrdinalIgnoreCase);
        }

        // Register a controller under a prefix (e.g., "obs", "ppt")
        public void Register(string prefix, IController controller)
        {
            if (prefix == null) throw new ArgumentNullException("prefix");
            if (controller == null) throw new ArgumentNullException("controller");

            _controllers[prefix] = controller;
        }

        // Check if a controller exists and is enabled
        public bool IsEnabled(string prefix)
        {
            IController? controller;
            if (_controllers.TryGetValue(prefix, out controller))
                return controller.IsEnabled;

            return false;
        }

        // Route a command to the appropriate controller
        public void Dispatch(string command, string[] args)
        {
            if (string.IsNullOrEmpty(command))
                return;

            // Extract prefix: "obs_scene" → "obs"
            int underscore = command.IndexOf('_');
            if (underscore < 0)
                return; // Not a controller command

            string prefix = command.Substring(0, underscore);

            IController? controller;
            if (_controllers.TryGetValue(prefix, out controller) && controller.IsEnabled)
            {
                controller.ExecuteCommandAsync(command, args);
            }
            else
            {
                // Optional: log unknown or disabled controller
            }
        }

        // Dispose all controllers
        public void Dispose()
        {
            foreach (var kv in _controllers)
            {
                try
                {
                    kv.Value.Dispose();
                }
                catch
                {
                    // swallow or log
                }
            }

            _controllers.Clear();
        }

        public IController Get(string prefix)
        {
            if (prefix == null)
                return null;

            IController? controller;
            if (_controllers.TryGetValue(prefix, out controller))
                return controller;

            return null;
        }
        public string BuildOpenFileDialogFilter() => _controllerRegistry.BuildOpenFileDialogFilter();
        public ControllerDescriptor? FindByExtension(string ext) => _controllerRegistry.FindByExtension(ext);

        public IController GetOrCreateController(string id)
        {
            if (_controllers.TryGetValue(id, out var existing))
                return existing;

            var descriptor = ControllerRegistery.Controllers.FirstOrDefault(c => c.Id == id);

            if (descriptor == null)
                throw new InvalidOperationException($"Unknown controller id: {id}");

            var controller = (IController)Activator.CreateInstance(descriptor.ControllerType)!;
            _controllers[id] = controller;
            return controller;
        }
        public void Remove(string prefix)
        {
            IController? controller;
            if (_controllers.TryGetValue(prefix, out controller))
            {
                try { controller.Dispose(); }
                catch { }

                _controllers.Remove(prefix);
            }
        }

    }
}
