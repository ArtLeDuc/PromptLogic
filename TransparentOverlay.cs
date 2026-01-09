using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace Teleprompter
{
    public class TransparentOverlay : Control
    {
        public event MouseEventHandler OverlayMouseWheel;
        public event Action<Point> RightClickRequested;


        public TransparentOverlay()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            // Do nothing — fully transparent
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            OverlayMouseWheel?.Invoke(this, e);
        }

        private const int HTNOWHERE = 0;
        private const int HTCLIENT = 1;
        private const int HTCAPTION = 2;

        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;
        private const int WM_RBUTTONUP = 0x0205;

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;

            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);

                var pos = PointToClient(new Point(m.LParam.ToInt32()));
                int grip = 8;

                bool left = pos.X <= grip;
                bool right = pos.X >= Width - grip;
                bool top = pos.Y <= grip;
                bool bottom = pos.Y >= Height - grip;

                if (left && top) { m.Result = (IntPtr)HTTOPLEFT; return; }
                if (right && top) { m.Result = (IntPtr)HTTOPRIGHT; return; }
                if (left && bottom) { m.Result = (IntPtr)HTBOTTOMLEFT; return; }
                if (right && bottom) { m.Result = (IntPtr)HTBOTTOMRIGHT; return; }
                if (left) { m.Result = (IntPtr)HTLEFT; return; }
                if (right) { m.Result = (IntPtr)HTRIGHT; return; }
                if (top) { m.Result = (IntPtr)HTTOP; return; }
                if (bottom) { m.Result = (IntPtr)HTBOTTOM; return; }

                return;
            }
            else if (m.Msg == WM_RBUTTONUP)
            {
                var pos = Cursor.Position;
                RightClickRequested?.Invoke(pos);
                return;

            }

            base.WndProc(ref m);
        }
    }
}
