using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using RatChat.Core;
using System.Text.RegularExpressions;
using System.Threading;
using System.ComponentModel;
using System.Windows;
using System.Web;

namespace RatChat.EmpireTv {
    [ChatName("Чат для http://www.empiretv.org")]
    [ConfigValue(".EMPIRETVCHAT.StreamerURI", "http://www.empiretv.org/user/415", "Адрес страницы стримера empire TV:", false)]
    public class EmpireTvChatSource : RatChat.Core.IChatSource, INotifyPropertyChanged {

        DispatcherTimer next;
        string _ChannelUri = "";
        int _StreamerID = 0;
        List<Message> LoadedMessages;
        SmilesDataDase Smiles;
        Regex ExtractSmile = new Regex("\\:s\\:\\w\\w.*?\\:");

        public EmpireTvChatSource() {
            LoadedMessages = new List<Message>();
            Smiles = new SmilesDataDase();
            SmilesUri = new Dictionary<string, string>();

            //WebClient smile = new WebClient();
            //smile.DownloadString();
            //_StreamerID = 9909;
        }

        void next_Tick( object sender, EventArgs e ) {
            if (_StreamerID != 0) {
                next.Stop();
                LoadChat(_StreamerID);
            }
        }

        // From codex
        private long unixTimestamp() {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = DateTime.Now - origin;
            return (long)Math.Floor(diff.TotalSeconds);
        }

        public static DateTime UnixTimeStampToDateTime( long unixTimeStamp ) {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        void LoadChat( int ChannelId ) {

            WebClient wc = new WebClient();
            wc.Headers.Add("user-agent", "RatChat");
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(( a, b ) => {
                if (b.Error == null) {
                    var messages = JsonConvert.DeserializeObject<Message[]>(b.Result);
                    if (messages != null)
                        UpdateMessages(messages);
                } else {
                    // Кстати, тут можно нарисовать ошибку сети.
                }
                if (next != null)
                    next.Start();
            });
            wc.DownloadStringAsync(new Uri(
                string.Format("http://www.empiretv.org/sites/default/files/fastchat/chat{0}.json?{1}",
                ChannelId, unixTimestamp())));
        }

        Message GetById( string id ) {
            for (int j = 0; j < LoadedMessages.Count; ++j)
                if (LoadedMessages[j].Id == id)
                    return LoadedMessages[j];
            return null;
        }

        private void UpdateMessages( Message[] msgs ) {
            for (int j = 0; j < LoadedMessages.Count; ++j)
                LoadedMessages[j].NeedToDelete--;

          

            List<Message> NewMessage = new List<Message>();
            //////////
            for (int j = 0; j < msgs.Length; ++j) {
                Message m = GetById(msgs[j].Id);
                if (m == null) {
                    LoadedMessages.Add(msgs[j]);

                    //Match mmm = ExtractSmile.Match(msg[j].Text);
                    //if (mmm.Success) {
                    //}

                    msgs[j].Text = HttpUtility.HtmlDecode(msgs[j].Text);

                    NewMessage.Add(msgs[j]);
                    msgs[j].NeedToDelete = 60 * 5;
                } else {
                    m.NeedToDelete = 60 * 5;
                }
            }
            ///////////

            int i = 0;
            while (i < LoadedMessages.Count)
                if (LoadedMessages[i].NeedToDelete < 0)
                    LoadedMessages.RemoveAt(i);
                else
                    ++i;

            ////////////
            if (NewMessage.Count > 0) {
                //CasterAchivment.Temperature -= NewMessage.Count * 0.01;

                //foreach (var m in NewMessage)
                //    OnNewMessage(m);

                if (OnNewMessagesArrived != null) {
                    OnNewMessagesArrived((from b in NewMessage
                                          //orderby b.Date
                                          select new ChatMessage() {
                                              Date = UnixTimeStampToDateTime( b.UnixDate ),
                                              Name = b.Name,
                                              Text = b.Text
                                          }).ToList());
                }
            }
        }

        public string Copyright {
            get { return "Oxlamon © 2013"; }
        }

        public string Description {
            get { return "Чаты с http://sc2tv.ru"; }
        }

        string _Header = "http://sc2tv.ru";
        public string Header {
            get { return _Header; }
            set {
                if (string.Compare(_Header, value) != 0) {
                    _Header = value;
                    FireChange("Header");
                }
            }
        }

        public string StreamerNick { get; set; }

        public void OnLoad( ConfigStorage Config ) {
            StreamerNick = Config.GetDefault(ConfigPrefix + ".EMPIRETVCHAT.StreamerNick", "");
            _StreamerID = (int)Config.GetDefault(ConfigPrefix + ".EMPIRETVCHAT.StreamerID", 0);
            _ChannelUri = Config.GetDefault(ConfigPrefix + ".EMPIRETVCHAT.StreamerURI", "http://sc2tv.ru/content/oxlamonschannel");
            LoadedMessages.Clear();
            UpdateHeader();
        }

       
        private void UpdateHeader(){
            if (string.IsNullOrEmpty(StreamerNick)) {
                Header = "http://empiretv.org, Чат не подключен.";
            } else {
                Header = "http://empiretv.org, " + StreamerNick;
            }
        }

        public void OnConfigApply( ConfigStorage Config ) {
            _ChannelUri = (string)Config.GetDefault(ConfigPrefix + ".EMPIRETVCHAT.StreamerURI", "");

            ManualResetEvent mre = new ManualResetEvent(false);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            try {
                string Result = wc.DownloadString(new Uri(_ChannelUri, UriKind.RelativeOrAbsolute));
                Regex rx = new Regex("title\\>(.*?)\\|");
                Match m = rx.Match(Result);
                if (m.Success) {
                    Config[ConfigPrefix + ".EMPIRETVCHAT.StreamerID"] = _StreamerID = int.Parse(_ChannelUri.Split('/')[4]);
                    Config[ConfigPrefix + ".EMPIRETVCHAT.StreamerNick"] = StreamerNick = m.Groups[1].Value.Trim();
                } else {
                    Config[ConfigPrefix + ".EMPIRETVCHAT.StreamerID"] = _StreamerID = 0;
                    Config[ConfigPrefix + ".EMPIRETVCHAT.StreamerNick"] = "";
                }
            } catch {
                Config[ConfigPrefix + ".EMPIRETVCHAT.StreamerID"] = _StreamerID = 0;
                Config[ConfigPrefix + ".EMPIRETVCHAT.StreamerNick"] = "";
                MessageBox.Show("Ошибка сети или стример не найден.");
            }

            LoadedMessages.Clear();
            UpdateHeader();
        }

        public void BeginWork() {
            if (next == null) {
                next = new DispatcherTimer();
                next.Interval = TimeSpan.FromSeconds(1);
                next.Tick += next_Tick;
                next.Start();
            }
        }

        public void EndWork() {
            if (next != null) {
                next.Stop();
                next = null;
            }
        }

        public event Core.OnNewMessagesArrivedDelegate OnNewMessagesArrived;

        public System.Windows.FrameworkElement CreateSmile( string id ) {
            return Smiles.GetSmile(id);
        }

        protected void FireChange( string PropertyName ) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public Dictionary<string, string> SmilesUri { get; private set; }

        public System.Windows.Controls.UserControl CreateCustomView() {
            return null;
        }

        public string ConfigPrefix { get; set; }

     
    }
}
