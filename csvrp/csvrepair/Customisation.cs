using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls.Ribbon;
using System.Windows.Media;
using System.Windows.Controls;

namespace sqx
{
    public class Customisation
    {
        public static void Icon(RibbonButton Gerard, string Way) 
        {
            Gerard.Background = new ImageBrush(new BitmapImage(new Uri(Way)));
            Gerard.FocusedBackground = new ImageBrush(new BitmapImage(new Uri(Way)));
            Gerard.MouseOverBackground = new ImageBrush(new BitmapImage(new Uri(Way)));
        }
    }
}
