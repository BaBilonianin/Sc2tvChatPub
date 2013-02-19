using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RatChat.Core {
    public class SmilesDataDase {
     
        Dictionary<string, Uri> SmilesUri = new Dictionary<string, Uri>();
        Dictionary<string, BitmapImage> SmilesBmp = new Dictionary<string, BitmapImage>();

        public SmilesDataDase() {
        }

        public void AddSmileTuple( string Id, string Uri ) {
            SmilesUri[Id] = new Uri(Uri, UriKind.RelativeOrAbsolute);
        }

        public FrameworkElement GetSmile( string id ) {
            Image img = new Image();// { Height = 24.0, IsHitTestVisible = false };
            img.SetResourceReference(Image.StyleProperty, "SmileStyle");
            BitmapImage bi;

            if (SmilesBmp.TryGetValue(id, out bi)) {
                img.Source = bi;
            } else
                if (SmilesUri.ContainsKey(id)) {
                    bi = new BitmapImage(SmilesUri[id]);
                    img.Source = bi;

                    SmilesBmp[id] = bi;
                } else
                    return null;

            return img;
        }
    }
}
