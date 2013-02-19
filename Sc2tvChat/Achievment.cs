using RatChat.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatChat {
    public class Achievment : INotifyPropertyChanged {
        public Achievment() {
            Clear();
        }

        #region Peka счетчик
        int _pekaCount = 0;
        public int PekaCount {
            get { return _pekaCount; }
            set {
                if (_pekaCount != value) {
                    _pekaCount = value;
                    FireChange("PekaCount");
                }
            }
        }
        #endregion

        #region Температура
        double _Temperature = 0.0;
        public double Temperature {
            get { return _Temperature; }
            set {
                if (_Temperature != value) {
                    _Temperature = value;
                    if (_Temperature < -273.15)
                        _Temperature = -273.15;
                    FireChange("Temperature");
                }
            }
        }
        #endregion

        #region Глубина падения стримера
        double _Depth = 0.0;
        public double Depth {
            get { return _Depth; }
            set {
                if (_Depth != value) {
                    _Depth = value;
                    if (_Depth < -10994)
                        _Depth = -10994;

                    FireChange("Depth");
                }
            }
        }
        #endregion

        protected void FireChange( string PropertyName ) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Clear() {
            PekaCount = 0;
            Temperature = 0.0;
            Depth = 0.0;
        }

        public void OnChatMessate( ChatMessage Message ) {
            PekaCount += Message.Text.CountSubstring(":s:peka:");

            Temperature -= 0.01;
            Temperature = Temperature
             + Message.Text.CountSubstring(":s:fire:") * 10;

            if (Message.Name == "Nalan")
                Temperature = -1000;
            
            Depth = Depth
                - Message.Text.CountSubstring(":s:crab:") * 5
                - Message.Text.CountSubstring(":s:fp:") * 3
                - Message.Text.CountSubstring(":s:fpl:") * 10
                - Message.Text.CountSubstring(":s:mimo:") 

                + Message.Text.CountSubstring(":s:fyeah:") * 10
                + Message.Text.CountSubstring(":s:bm:") * 5
                + Message.Text.CountSubstring(":s:notbad:") * 2;
        }
    }
}
