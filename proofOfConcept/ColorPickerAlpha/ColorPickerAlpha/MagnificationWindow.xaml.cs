using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace ColorPickerAlpha
{
    public partial class MagnificationWindow : Window
    {
        float magnificationFactor = 1f;
        int width;
        int height;
        RECT magWindowRect;
        IntPtr hwndMag;
        private IntPtr hwnd;
        private bool initialized;


        public MagnificationWindow()
        {
            InitializeComponent();
            AllowsTransparency = true;
            Opacity = 255;
            hwnd = new WindowInteropHelper(this).EnsureHandle();

            Loaded += delegate
            {
                Activated += delegate { Topmost = true; };
                Deactivated += delegate { Topmost = true; };

                magnificationFactor = 2f;

                if (NativeMethods.MagInitialize())
                {
                    SetUpMagnWindow();
                }
            };

            //TODO: if doesnt work catch
            Closing += delegate { NativeMethods.MagUninitialize(); };
        }


        private void SetUpMagnWindow()
        {
            width = (int)(Width / magnificationFactor);
            height = (int)(Height / magnificationFactor);
            magWindowRect = new RECT();

            IntPtr hInst = NativeMethods.GetModuleHandle(null);

            NativeMethods.GetClientRect(hwnd, ref magWindowRect);
            hwndMag = NativeMethods.CreateWindow((int)ExtendedWindowStyles.WS_EX_CLIENTEDGE, NativeMethods.WC_MAGNIFIER,
                "MagnifierWindow", (int)WindowStyles.WS_CHILD | (int)MagnifierStyle.MS_SHOWMAGNIFIEDCURSOR |
                (int)WindowStyles.WS_VISIBLE,
                magWindowRect.left, magWindowRect.top, magWindowRect.right, magWindowRect.bottom,
                IntPtr.Zero, hwnd, hInst, IntPtr.Zero);

            if (hwndMag == IntPtr.Zero)
            {
                return;
            }

            // Set the magnification factor.
            Transformation matrix = new Transformation(magnificationFactor);
            NativeMethods.MagSetWindowTransform(hwndMag, ref matrix);
        }

        public virtual void UpdateMaginifier(int cursorX, int cursorY)
        {
            if ((!initialized) || (hwndMag == IntPtr.Zero))
                return;

            RECT sourceRect = new RECT();

            int width = (int)((magWindowRect.right - magWindowRect.left) / magnificationFactor);
            int height = (int)((magWindowRect.bottom - magWindowRect.top) / magnificationFactor);

            sourceRect.left = Math.Max(cursorX - width / 2, 0);
            sourceRect.top = Math.Max(cursorY - height / 2, 0);

            // Don't scroll outside desktop area.
            if (sourceRect.left > NativeMethods.GetSystemMetrics(NativeMethods.SM_CXSCREEN) - width)
            {
                sourceRect.left = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXSCREEN) - width;
            }
            sourceRect.right = sourceRect.left + width;

            if (sourceRect.top > NativeMethods.GetSystemMetrics(NativeMethods.SM_CYSCREEN) - height)
            {
                sourceRect.top = NativeMethods.GetSystemMetrics(NativeMethods.SM_CYSCREEN) - height;
            }
            sourceRect.bottom = sourceRect.top + height;

            // Set the source rectangle for the magnifier control.
            NativeMethods.MagSetWindowSource(hwndMag, sourceRect);

            // Reclaim topmost status, to prevent unmagnified menus from remaining in view. 
            NativeMethods.SetWindowPos(hwnd, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0,
                (int)SetWindowPosFlags.SWP_NOACTIVATE | (int)SetWindowPosFlags.SWP_NOMOVE | (int)SetWindowPosFlags.SWP_NOSIZE);

            // Force redraw.
            NativeMethods.InvalidateRect(hwndMag, IntPtr.Zero, true);

            ResizeMagnifier();
        }

        protected virtual void ResizeMagnifier()
        {
            if (initialized && (hwndMag != IntPtr.Zero))
            {
                NativeMethods.GetClientRect(hwnd, ref magWindowRect);
                // Resize the control to fill the window.
                NativeMethods.SetWindowPos(hwndMag, IntPtr.Zero,
                    magWindowRect.left, magWindowRect.top, magWindowRect.right, magWindowRect.bottom, 0);
            }
        }
    }
}
