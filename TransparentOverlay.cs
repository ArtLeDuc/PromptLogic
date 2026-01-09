using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private const int RESIZE_HANDLE = 8;

        public enum ResizeEdge
        {
            None,
            Left, Right, Top, Bottom,
            TopLeft, TopRight, BottomLeft, BottomRight
        }

        private ResizeEdge activeEdge = ResizeEdge.None;
        private Point dragStartScreen;
        private Rectangle startBounds;

        public Form TargetForm { get; set; }   // set this from MainForm

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

        private ResizeEdge HitTest(Point p)
        {
            ResizeEdge result;

            bool left = p.X <= RESIZE_HANDLE;
            bool right = p.X >= Width - RESIZE_HANDLE;
            bool top = p.Y <= RESIZE_HANDLE;
            bool bottom = p.Y >= Height - RESIZE_HANDLE;

            if (left && top) result = ResizeEdge.TopLeft;
            else if (right && top) result = ResizeEdge.TopRight;
            else if (left && bottom) result = ResizeEdge.BottomLeft;
            else if (right && bottom) result = ResizeEdge.BottomRight;
            else if (left) result = ResizeEdge.Left;
            else if (right) result = ResizeEdge.Right;
            else if (top) result = ResizeEdge.Top;
            else if (bottom) result = ResizeEdge.Bottom;
            else result = ResizeEdge.None;

            return result;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // If currently resizing, perform resize
            if (activeEdge != ResizeEdge.None)
            {
                Point current = PointToScreen(e.Location);
                int dx = current.X - dragStartScreen.X;
                int dy = current.Y - dragStartScreen.Y;

                Rectangle newBounds = startBounds;

                if (!Capture)
                    Capture = true;

                switch (activeEdge)
                {
                    case ResizeEdge.Left:
                        newBounds.X += dx;
                        newBounds.Width -= dx;
                        break;

                    case ResizeEdge.Right:
                        newBounds.Width += dx;
                        break;

                    case ResizeEdge.Top:
                        newBounds.Y += dy;
                        newBounds.Height -= dy;
                        break;

                    case ResizeEdge.Bottom:
                        newBounds.Height += dy;
                        break;

                    case ResizeEdge.TopLeft:
                        newBounds.X += dx;
                        newBounds.Width -= dx;
                        newBounds.Y += dy;
                        newBounds.Height -= dy;
                        break;

                    case ResizeEdge.TopRight:
                        newBounds.Width += dx;
                        newBounds.Y += dy;
                        newBounds.Height -= dy;
                        break;

                    case ResizeEdge.BottomLeft:
                        newBounds.X += dx;
                        newBounds.Width -= dx;
                        newBounds.Height += dy;
                        break;

                    case ResizeEdge.BottomRight:
                        newBounds.Width += dx;
                        newBounds.Height += dy;
                        break;
                }

                if (newBounds.Width >= TargetForm.MinimumSize.Width &&
                    newBounds.Height >= TargetForm.MinimumSize.Height)
                {
                    TargetForm.Bounds = newBounds;
                }

                return; // IMPORTANT — prevents cursor logic from running during resize
            }

            // Not resizing — update cursor using HitTest
            switch (HitTest(e.Location))
            {
                case ResizeEdge.Left:
                case ResizeEdge.Right:
                    Cursor = Cursors.SizeWE;
                    break;

                case ResizeEdge.Top:
                case ResizeEdge.Bottom:
                    Cursor = Cursors.SizeNS;
                    break;

                case ResizeEdge.TopLeft:
                case ResizeEdge.BottomRight:
                    Cursor = Cursors.SizeNWSE;
                    break;

                case ResizeEdge.TopRight:
                case ResizeEdge.BottomLeft:
                    Cursor = Cursors.SizeNESW;
                    break;

                default:
                    Cursor = Cursors.Default;
                    break;
            }
        }

        public ResizeEdge GetResizeEdge(Point p)
        {
            return HitTest(p);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // First: check if we're on a resize edge
            activeEdge = HitTest(e.Location);
            if (activeEdge != ResizeEdge.None)
            {
                // Start resize mode
                dragStartScreen = PointToScreen(e.Location);
                startBounds = TargetForm.Bounds;
                Capture = true;
                return;   // IMPORTANT — do NOT run drag logic
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            activeEdge = ResizeEdge.None;
            Capture = false;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_RBUTTONUP)
            {
                var pos = Cursor.Position;
                RightClickRequested?.Invoke(pos);
                return;

            }

            base.WndProc(ref m);
        }
    }
}
