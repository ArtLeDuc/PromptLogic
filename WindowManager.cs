using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Teleprompter
{
    public class PopupInfo
    {
        public Form Popup { get; }
        public int Column { get; set; }
        public int Row { get; set; }

        public PopupInfo(Form popup)
        {
            Popup = popup ?? throw new ArgumentNullException(nameof(popup));
        }
    }   
    public class WindowManager
    {
        private readonly List<PopupInfo> _openPopups = new List<PopupInfo>();
        private Form _mainForm;

        // Layout constants (tune these)
        private const int HorizontalGap = 10;
        private const int VerticalGap = 1;
        private const int TopMargin = 0;
        private const int ColumnWidth = 260; // or width of your widest popup

        public WindowManager(Form mainForm)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));

            _mainForm.Move += (_, __) => Reflow();
            _mainForm.Resize += (_, __) => Reflow();
        }

        public void RegisterMainForm(Form mainForm)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
        }

        public void ShowPopup(Form popup)
        {
            if (popup == null) throw new ArgumentNullException(nameof(popup));

            // If already tracked, just bring to front
            var existing = _openPopups.FirstOrDefault(p => p.Popup == popup);
            if (existing != null)
            {
                popup.BringToFront();
                return;
            }

            // Track closing so we can remove + reflow
            popup.FormClosed += Popup_FormClosed;
            
            bool topMost = ((MainForm)_mainForm).IsAppTopMost();
            NativeMethods.SetWindowPos(popup.Handle,
                topMost ? NativeMethods.HWND_TOPMOST : NativeMethods.HWND_NOTOPMOST,
                0, 0, 0, 0,
                NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE);

            var info = new PopupInfo(popup);
            _openPopups.Add(info);


            popup.Show();
            Reflow(); // let Reflow assign column/row and positions

            popup.BringToFront();
        }

        public void ClosePopup(Form popup)
        {
            if (popup == null) return;

            var info = _openPopups.FirstOrDefault(p => p.Popup == popup);
            if (info == null) return;

            popup.FormClosed -= Popup_FormClosed;
            _openPopups.Remove(info);

            if (!popup.IsDisposed)
                popup.Close();

            Reflow();
        }

        private void Popup_FormClosed(object sender, FormClosedEventArgs e)
        {
            var popup = sender as Form;
            if (popup == null) return;

            var info = _openPopups.FirstOrDefault(p => p.Popup == popup);
            if (info != null)
                _openPopups.Remove(info);

            Reflow();
        }

        public void Reflow()
        {
            if (_mainForm == null || _openPopups.Count == 0)
                return;

            // Screen bounds of the main form (you could choose a specific screen instead)
            var screen = Screen.FromControl(_mainForm).WorkingArea;
            int screenBottom = screen.Bottom;

            int column = 0;
            int row = 0;
            int totalHeight = 0;
            // Reflow in the order opened
            foreach (var info in _openPopups)
            {
                var popup = info.Popup;
                if (popup.IsDisposed) continue;

                // Base X for this column
                int baseX = _mainForm.Right + HorizontalGap + (column * ColumnWidth);

                // Base Y for this row within the column
                int baseY = _mainForm.Top + TopMargin;

                // Compute the Y for this row
                //                int y = baseY + row * (popup.Height + VerticalGap);
                int y = baseY + (VerticalGap*row) + totalHeight;

                // If it would go off-screen, start a new column
                if (y + popup.Height > screenBottom)
                {
                    column++;
                    row = 0;

                    baseX = _mainForm.Right + HorizontalGap + (column * ColumnWidth);
                    baseY = _mainForm.Top + TopMargin;
                    y = baseY; // first row in new column
                }

                totalHeight += popup.Height;
                int x = baseX;

                info.Column = column;
                info.Row = row;

                popup.Location = new Point(x, y);

                row++;
            }
        }
    }

}
