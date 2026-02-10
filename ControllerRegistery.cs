using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptLogic.Controllers
{
    public class ControllerDescriptor
    {
        public required string Id { get; set; }              // "ppt", "obs", "pdf"
        public required string DisplayName { get; set; }     // "PowerPoint", "OBS", "PDF"
        public required string[] FileExtensions { get; set; } // [".pptx", ".ppt"] or empty
        public bool CanOpenFiles { get; set; }      // OBS = false, PPT = true
        public bool ActivatedByScript { get; set; } // OBS = true, PPT = false
        public required Type ControllerType { get; set; }

    }

    public class ControllerRegistery
    {
        public static readonly List<ControllerDescriptor> Controllers =
            new()
            {
            new ControllerDescriptor
            {
                Id = "ppt",
                DisplayName = "PowerPoint",
                FileExtensions = new[] { ".pptx", ".ppt", ".ppsx" },
                CanOpenFiles = true,
                ActivatedByScript = false,
                ControllerType = typeof(PptController)
            },

            new ControllerDescriptor
            {
                Id = "txt",
                DisplayName = "Text",
                FileExtensions = new[] { ".txt" },
                CanOpenFiles = true,
                ActivatedByScript = false,
                ControllerType = typeof(TxtController)
            },

/*
            new ControllerDescriptor
            {
                Id = "pdf",
                DisplayName = "PDF",
                FileExtensions = new[] { ".pdf" },
                CanOpenFiles = true,
                ActivatedByScript = false
                ControllerType = typeof(PdfController)
            },
*/
            new ControllerDescriptor
            {
                Id = "obs",
                DisplayName = "OBS",
                FileExtensions = Array.Empty<string>(),
                CanOpenFiles = false,
                ActivatedByScript = true,
                ControllerType = typeof(ObsController)
            },
/*
            new ControllerDescriptor
            {
                Id = "slidelist",
                DisplayName = "Slide List",
                FileExtensions = new[] { ".txt", ".script" },
                CanOpenFiles = true,
                ActivatedByScript = true
                ControllerType = typeof(ScrController)
            }
*/
            };
        public string BuildOpenFileDialogFilter()
        {
            var parts = new List<string>();

            foreach (var c in Controllers.Where(c => c.CanOpenFiles))
            {
                if (c.FileExtensions.Length == 0)
                    continue;

                // "*.pptx;*.ppt"
                var pattern = string.Join(";", c.FileExtensions.Select(ext => "*" + ext));

                // "PowerPoint (*.pptx;*.ppt)|*.pptx;*.ppt"
                parts.Add($"{c.DisplayName} ({pattern})|{pattern}");
            }

            // Add "All Files"
            parts.Add("All Files (*.*)|*.*");

            return string.Join("|", parts);
        }

        public ControllerDescriptor? FindByExtension(string ext)
        {
            return Controllers
                .Where(c => c.CanOpenFiles)
                .FirstOrDefault(c =>
                    c.FileExtensions.Any(e =>
                        string.Equals(e, ext, StringComparison.OrdinalIgnoreCase)));
        }

    }
}
