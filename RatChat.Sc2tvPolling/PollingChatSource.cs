using RatChat.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RatChat.Polling {
    [ChatName("Голосовалка")]
    [ConfigValue(".POLLING.Select4", "", "Вариант 4:", false)]
    [ConfigValue(".POLLING.Select3", "", "Вариант 3:", false)]
    [ConfigValue(".POLLING.Select2", "", "Вариант 2:", false)]
    [ConfigValue(".POLLING.Select1", "", "Вариант 1:", false)]
    [ConfigValue(".POLLING.Variant4", "4.", "Выбор чата 4:", false)]
    [ConfigValue(".POLLING.Variant3", "3.", "Выбор чата  3:", false)]
    [ConfigValue(".POLLING.Variant2", "2.", "Выбор чата  2:", false)]
    [ConfigValue(".POLLING.Variant1", "1.", "Выбор чата  1:", false)]
    [ConfigValue(".POLLING.Title", "", " Заголовок голосования:", false)]
    [ConfigValue(".POLLING.Minutes", "", "Время голосования (минуты или 0):", false)]
    public class PollingChatSource : RatChat.Core.IChatSource, INotifyPropertyChanged, IChatListener {
        public PollingChatSource() {
            SmilesUri = new Dictionary<string, string>();
            Result = new Dictionary<string, int>();
            Variants = new ObservableCollection<Variant>();
            PollTimer = new DispatcherTimer();
            PollTimer.Tick += PollTimer_Tick;
            PollTimer.Interval = TimeSpan.FromSeconds(1);
        }

        bool IsVoteEnabled;

        void PollTimer_Tick( object sender, EventArgs e ) {
            if( IsVoteEnabled )
            if (PollExpire.HasValue) {
                PollExpire = PollExpire.Value - TimeSpan.FromSeconds(1);

                Header = string.Format( "Идет голосование: {0}", PollExpire.Value );

                if (PollExpire.Value.TotalSeconds <= 1) {
                    PollExpire = null;
                    Header = "Голосование закончено. Победил вариант: " + GetWinner();
                    IsVoteEnabled = false;
                    // END
                }
            } else {
            }
        }

        DispatcherTimer PollTimer;

        #region Common
        public string Copyright {
            get { return "Oxlamon © 2013"; }
        }

        public string Description {
            get { return "Голосовалка"; }
        }

        public string ConfigPrefix { get; set; }

        public string StreamerNick { get; set; }

        public Dictionary<string, string> SmilesUri { get; private set; }

        public event Core.OnNewMessagesArrivedDelegate OnNewMessagesArrived;
        #endregion

        #region INotifyPropertyChanged
        protected void FireChange( string PropertyName ) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        string _Header = "";
        public string Header {
            get { return _Header; }
            set {
                if (string.Compare(_Header, value) != 0) {
                    _Header = Title + value;
                    FireChange("Header");
                }
            }
        }

        public TimeSpan? PollExpire { get; set; }

        public class Variant : INotifyPropertyChanged {
            public Variant() {
              //  History = new ObservableCollection<int>();
            }

            public string Select { get; set; }

            public string Text { get; set; }

            int _Votes = 0;
            public int Votes {
                get { return _Votes; }
                set {
                    if (_Votes != value) {
                        _Votes = value;
                        FireChange("Votes");
                    }
                }
            }

           // public ObservableCollection<int> History { get; private set; }

          //  public int MiddleVotes { get; set; }

            protected void FireChange( string PropertyName ) {
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        ObservableCollection<Variant> Variants;
        Dictionary<string, int> Result;

        string GetWinner() {
            return (from b in Variants
                    orderby b.Votes descending
                    select b.Text).ToArray()[0];
        }

        void RegisterVote( ChatMessage Message ) {
            if (Result.ContainsKey(Message.Name))
                return;

            int vote = -1;
            for (int j = 0; j < Variants.Count; ++j)
                if (Message.Text.Contains(Variants[j].Select)) {
                    vote = j + 1;
                    Variants[j].Votes++;
                    Result[Message.Name] = vote;
                    break;
                }

            //if (vote != -1)
            //    for (int j = 0; j < Variants.Count; ++j)
            //        Variants[j].History.Add(Variants[j].Votes);
        }

        public void BeginWork() {
            
        }

        public void EndWork() {
            Result.Clear();
            Variants.Clear();
        }

        public void OnLoad( Core.ConfigStorage Config ) {
            OnConfigApply(Config);
            IsVoteEnabled = false;
            Header = "Голосование не начато.";
        }

        string Title = "";

        public void OnConfigApply( Core.ConfigStorage Config ) {

            Result.Clear();
            Variants.Clear();
            PollExpire = null;
            IsVoteEnabled = true;

            Title = Config.GetDefault(ConfigPrefix + ".POLLING.Title", "").Trim();
            if (!string.IsNullOrEmpty(Title))
                Title += ". ";

            for (int j = 0; j < 4; ++j) {
                string txt = Config.GetDefault(ConfigPrefix + ".POLLING.Select" + (j + 1), "").Trim();
                string var = Config.GetDefault(ConfigPrefix + ".POLLING.Variant" + (j + 1), "").Trim();

                if (!string.IsNullOrEmpty(txt) && !string.IsNullOrEmpty(var)) {
                    Variants.Add(new Variant() {
                        Select = var,
                        Text = "(" + var + ") " + txt
                    });
                }
            }


            string min = Config.GetDefault(ConfigPrefix + ".POLLING.Minutes", "").Trim();
            int minutes;
            if (int.TryParse(min, out minutes)) {
                if (minutes == 0)
                    PollExpire = null;
                else
                    PollExpire = TimeSpan.FromMinutes(minutes);
            } else {
                PollExpire = null;
            }

            if (PollExpire == null) {
                Header = "Идет голосование";
            }

            PollTimer.Start();
            /// Автоматом, мы должны обновить вид.
        }

        public System.Windows.Controls.UserControl CreateCustomView() {
            PollControl pc = new PollControl();
            pc.DataContext = Variants;
            return pc;
        }

        public void OnNewMessageReceived( List<ChatMessage> NewMessages ) {
            if( IsVoteEnabled )
                foreach (ChatMessage cm in NewMessages)
                    RegisterVote(cm);
        }
    }
}
