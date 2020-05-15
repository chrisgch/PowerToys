using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ColorPickerAlpha
{
    public partial class MagnificationWindow : Window
    {
        public MagnificationWindow()
        {
            InitializeComponent();

            Activated += delegate { Topmost = true; };
            Deactivated += delegate { Topmost = true; };
        }

        public void UpdateVisuals(int cursorX, int cursorY)
        {
            //var relativePosition = e.GetPosition(this);
            //var pnt = PointToScreen(relativePosition);
            System.Drawing.Size a = new System.Drawing.Size();
            a.Width = 3;
            a.Height = 3;
      
            System.Drawing.Image b;
            using (b = new Bitmap(a.Width, a.Height))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.CopyFromScreen(new System.Drawing.Point(cursorX, cursorY), new System.Drawing.Point(0, 0), a);
                }

                Bitmap resized = new Bitmap(b, new System.Drawing.Size(b.Width * 25, b.Height * 25));
                b = resized;
                System.Drawing.Image x = new Bitmap(b);

                ImageBrush myBrush = new ImageBrush();
                x.Save("sam.jpeg", ImageFormat.Jpeg);

                myBrush.ImageSource = new BitmapImage(new Uri("sam.jpeg", UriKind.Relative));
                Background = myBrush;
            }
        }
    }
}
