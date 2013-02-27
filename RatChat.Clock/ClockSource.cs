using RatChat.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatChat.Clock {
    [ChatName("Часы")]
    [ConfigValue(".CLOCK.ShowClock", "да", "Показывать часы (да/нет):", false)]
    [ConfigValue(".CLOCK.Timer1", "00:05:00", "Таймер 1:", false)]
    [ConfigValue(".CLOCK.Timer1.Text", "Вернусь через {0:HH:mm:ss}", "Текст для таймера 1:", false)]
    [ConfigValue(".CLOCK.Timer2", "00:10:00", "Таймер 2:", false)]
    [ConfigValue(".CLOCK.Timer2.Text", "Обед. Вернусь через {0:HH:mm:ss}", "Текст для таймера 2:", false)]
    [ConfigValue(".CLOCK.Timer3", "00:15:00", "Таймер 3:", false)]
    [ConfigValue(".CLOCK.Timer3.Text", "Перекус. Вернусь через {0:HH:mm:ss}", "Текст для таймера 3:", false)]
    public class ClockSource : IChatSource, IChatListener, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Copyright {
            get { return "Oxlamon (c) 2013"; }
        }

        public string Description {
            get { return "Часы"; }
        }

        public string StreamerNick { get; set; }

        public void BeginWork() {
        }

        public void EndWork() {
        }

        public event OnNewMessagesArrivedDelegate OnNewMessagesArrived;


        string _Header = "Часы";
        public string Header {
            get { return _Header; }
            set {
                if (string.Compare(_Header, value) != 0) {
                    _Header = value;
                    FireChange("Header");
                }
            }
        }

        protected void FireChange( string PropertyName ) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public Dictionary<string, string> SmilesUri { get; set; }

        public void OnLoad( ConfigStorage Config ) {
        }

        public void OnConfigApply( ConfigStorage Config ) {
        }

        public System.Windows.Controls.UserControl CreateCustomView() {
            return null;
        }

        public string ConfigPrefix { get; set; }

        public void OnNewMessageReceived( List<ChatMessage> NewMessages ) {
        }
    }
}
