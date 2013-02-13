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

namespace RatChat.Sc2tv {
    [ChatName("Чат для http://sc2tv.ru")]
    [ConfigValue(".SC2TVCHAT.StreamerURI", "http://sc2tv.ru/content/oxlamonschannel", "Адрес страницы стримера sc2tv.ru:")]
    public class Sc2tvChatSource: RatChat.Core.IChatSource {
        DispatcherTimer next;
        string _ChannelUri = "";
        int _StreamerID = 0;
        List<Message> LoadedMessages;
        SmilesDataDase Smiles;
        Achievment CasterAchivment;

        public INotifyPropertyChanged HeaderData { get { return CasterAchivment; } }

        public Sc2tvChatSource() {
            LoadedMessages = new List<Message>();
            Smiles = new SmilesDataDase();
            CasterAchivment = new Achievment();
        }
        
        void next_Tick( object sender, EventArgs e ) {
            if (_StreamerID != 0) {
                next.Stop();
                LoadChat(_StreamerID);
            }
        }

        void LoadChat( int ChannelId ) {
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(( a, b ) => {
                if (b.Error == null) {
                    Messages messages = JsonConvert.DeserializeObject<Messages>(b.Result);
                    if (messages != null)
                        UpdateMessages(messages);
                } else {
                    // Кстати, тут можно нарисовать ошибку сети.
                }
                if( next != null )
                    next.Start();
            });
            wc.DownloadStringAsync(new Uri("http://chat.sc2tv.ru/memfs/channel-" + ChannelId + ".json"));
        }

        Message GetById( int id ) {
            for (int j = 0; j < LoadedMessages.Count; ++j)
                if (LoadedMessages[j].Id == id)
                    return LoadedMessages[j];
            return null;
        }

        private void UpdateMessages( Messages msgs ) {
            for (int j = 0; j < LoadedMessages.Count; ++j)
                LoadedMessages[j].NeedToDelete--;

            var msg = (from b in msgs.Content
                       orderby b.Date
                       select b).ToArray();

            List<Message> NewMessage = new List<Message>();
            //////////
            for (int j = 0; j < msg.Length; ++j) {
                Message m = GetById(msg[j].Id);
                if (m == null) {
                    LoadedMessages.Add(msg[j]);
                    NewMessage.Add(msg[j]);
                    msg[j].NeedToDelete = 60 * 5;
                } else {
                    m.NeedToDelete = 60 * 5; 
                }
            }
            ///////////

            int i = 0;
            while (i < LoadedMessages.Count)
                if (LoadedMessages[i].NeedToDelete<0)
                    LoadedMessages.RemoveAt(i);
                else
                    ++i;

            ////////////
            if (NewMessage.Count > 0) {
                CasterAchivment.Temperature -= NewMessage.Count * 0.01;

                foreach( var m in NewMessage )
                    OnNewMessage(m);

                if (OnNewMessagesArrived != null) {
                    OnNewMessagesArrived((from b in NewMessage
                                          orderby b.Date
                                          select new ChatMessage() {
                                              Date = b.Date,
                                              Name = b.Name,
                                              Text = b.Text
                                          }).ToList());
                }
            }
        }

        private void OnNewMessage( Message NewMessage ) {
            CasterAchivment.PekaCount += NewMessage.Text.CountSubstring(":s:peka:");

            CasterAchivment.Temperature += NewMessage.Text.CountSubstring(":s:fire:") * 5;

            CasterAchivment.Depth = CasterAchivment.Depth
                - NewMessage.Text.CountSubstring(":s:fp:") * 4
                - NewMessage.Text.CountSubstring(":s:crab:") * 5
                - NewMessage.Text.CountSubstring(":s:fpl:") * 2
                - NewMessage.Text.CountSubstring(":s:mimo:")

                + NewMessage.Text.CountSubstring(":s:fyeah:") * 5
                + NewMessage.Text.CountSubstring(":s:notbad:") * 3
                + NewMessage.Text.CountSubstring(":s:bm:") * 2;
        }

        public string Copyright {
            get { return "Oxlamon © 2013"; }
        }

        public string Description {
            get { return "Чаты с http://sc2tv.ru"; }
        }

        public string HeaderDataSkin {
            get { return "SC2TvHeader"; }
        }

        public string StreamerNick { get; set; }
        
        public void OnLoad( string ConfigPrefix, ConfigStorage Config ) {
            StreamerNick = Config.GetDefault(ConfigPrefix + ".SC2TVCHAT.StreamerNick", "");
            _StreamerID = (int)Config.GetDefault(ConfigPrefix + ".SC2TVCHAT.StreamerID", 0);
            _ChannelUri = Config.GetDefault(ConfigPrefix + ".SC2TVCHAT.StreamerURI", "http://sc2tv.ru/content/oxlamonschannel");
            LoadedMessages.Clear();
            CasterAchivment.Clear();
        }

        public void OnConfigApply( string ConfigPrefix, ConfigStorage Config ) {
            _ChannelUri = (string)Config.GetDefault(ConfigPrefix + ".SC2TVCHAT.StreamerURI", "http://sc2tv.ru/content/oxlamonschannel");

            ManualResetEvent mre = new ManualResetEvent(false);
            WebClient wc = new WebClient();
            try {
                string Result = wc.DownloadString(new Uri(_ChannelUri, UriKind.RelativeOrAbsolute));
                Regex rx = new Regex("\\<link.*?\\\"canonical\\\".*?href=\\\"http://sc2tv.ru/node/(.*?)\\\"");
                Match m = rx.Match(Result);
                if (m.Success) {
                    rx = new Regex(".*?author\\\".*?title.*?\\>(.*?)\\<");

                    Config[ConfigPrefix + ".SC2TVCHAT.StreamerID"] = _StreamerID = int.Parse(m.Groups[1].Value);

                    m = rx.Match(Result);
                    Config[ConfigPrefix + ".SC2TVCHAT.StreamerNick"] = StreamerNick = m.Groups[1].Value;
                } else {
                    Config[ConfigPrefix + ".SC2TVCHAT.StreamerID"] = _StreamerID = 0;
                    Config[ConfigPrefix + ".SC2TVCHAT.StreamerNick"] = "";
                }
            } catch {
                Config[ConfigPrefix + ".SC2TVCHAT.StreamerID"] = _StreamerID = 0;
                Config[ConfigPrefix + ".SC2TVCHAT.StreamerNick"] = "";
                MessageBox.Show("Ошибка сети или стример не найден.");
            }

            LoadedMessages.Clear();
            CasterAchivment.Clear();
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


    }
}
