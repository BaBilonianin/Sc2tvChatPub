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
    public class Smile {
        public BitmapImage Image { get; set; }
        public string Id { get; set; }
        public Uri Uri { get; set; }
    }

    public class SmilesDataDase {
     
        Dictionary<string, Uri> SmilesUri = new Dictionary<string, Uri>();
        Dictionary<string, Smile> SmilesBmp = new Dictionary<string, Smile>();

        public SmilesDataDase() {
            SmilesUri.Add("Oxlamon ", new Uri("pack://application:,,,/RatChat;component/oxlamon.png", UriKind.RelativeOrAbsolute));
            SmilesUri.Add("oxlamon ", new Uri("pack://application:,,,/RatChat;component/oxlamon.png", UriKind.RelativeOrAbsolute));
            SmilesUri.Add("Oxlamon, ", new Uri("pack://application:,,,/RatChat;component/oxlamon.png", UriKind.RelativeOrAbsolute));
            SmilesUri.Add("пони ", new Uri("pack://application:,,,/RatChat;component/chan.png", UriKind.RelativeOrAbsolute));
        }

        public void SetCustoms( string Smiles ) {
            string[] smiles = Smiles.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string smile in smiles) {
                string[] d = smile.Split('=');
                try {
                    SmilesUri[d[0].Trim() + " "] = new Uri(d[1], UriKind.RelativeOrAbsolute);
                } catch {
                }
            }
        }

        public void AddSmileTuple( string Id, string Uri ) {
            SmilesUri[Id] = new Uri(Uri, UriKind.RelativeOrAbsolute);
        }

        public FrameworkElement GetSmile( string id ) {
            ContentPresenter cp = new ContentPresenter();
            Smile bi = null;

            if (!SmilesBmp.TryGetValue(id, out bi)) {
                if (SmilesUri.ContainsKey(id)) {
                    bi = new Smile() {
                        Image = new BitmapImage(SmilesUri[id]),
                        Uri = SmilesUri[id],
                        Id = id
                    };
                    //bi.ta
                    SmilesBmp[id] = bi;
                } else
                    return null;
            }

            cp.Content = bi;
            cp.SetResourceReference(ContentPresenter.ContentTemplateProperty, "SmileStyle2");           
            return cp;



            //Image img = new Image();// { Height = 24.0, IsHitTestVisible = false };
            //BitmapImage bi;

            //if (SmilesBmp.TryGetValue(id, out bi)) {
            //    img.Source = bi;
            //} else
            //    if (SmilesUri.ContainsKey(id)) {
            //        bi = new BitmapImage(SmilesUri[id]);
            //        img.Source = bi;

            //        SmilesBmp[id] = bi;
            //    } else
            //        return null;

            //img.SetResourceReference(Image.StyleProperty, "SmileStyle");
            //return img;
        }
    }
}
