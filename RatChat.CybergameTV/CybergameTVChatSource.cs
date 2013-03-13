using Newtonsoft.Json;
using RatChat.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Threading;

namespace RatChat.CybergameTV {
    [ChatName("Чат для http://cybergame.tv")]
    [ConfigValue(".CYBERGAMETVCHAT.StreamerURI", "", "Адрес страницы стримера cybergame.tv:", false)]
    public class CybergameTVChatSource : RatChat.Core.IChatSource, INotifyPropertyChanged {
        public const string UpdateSmilesUri = "http://chat.sc2tv.ru/js/smiles.js";

        DispatcherTimer next;
        string _ChannelUri = "";
        List<Message> LoadedMessages;
        SmilesDataDase Smiles;
        //Regex ExtractSmile = new Regex("\\:s\\:\\w\\w.*?\\:");

        public CybergameTVChatSource() {
            LoadedMessages = new List<Message>();
            Smiles = new SmilesDataDase();
            SmilesUri = new Dictionary<string, string>();

            //WebClient smile = new WebClient();
            //smile.DownloadString();
        }

        void next_Tick( object sender, EventArgs e ) {
            if ( !string.IsNullOrEmpty(StreamerNick) ) {
                next.Stop();


                ThreadPool.QueueUserWorkItem(( a ) => {
                    LoadChat();
                    if (next != null)
                        next.Start();
                });
            }
        }

        void LoadChat() {

            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(@"http://cybergame.tv/wp-admin/admin-ajax.php");

            UTF8Encoding encoding = new UTF8Encoding();
            string postData = string.Format( "action=quick-chat-ajax-update-messages&quick_chat_last_timestamp={0}&quick_chat_rooms[]={1}&quick_chat_update_messages_nonce={2}",
                  cctv_timestamp,
                cctv_streamer,
                cctv_nonce );
            cctv_timestamp += 5;
       //   cctv_timestamp = unixTimestamp();

            byte[] data = encoding.GetBytes(postData);

            httpWReq.Method = "POST";
            httpWReq.ContentType = "application/x-www-form-urlencoded";
            httpWReq.ContentLength = data.Length;

            using (Stream stream = httpWReq.GetRequestStream()) {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse resp = (HttpWebResponse)httpWReq.GetResponse();
            MemoryStream ms = new MemoryStream();
            using (Stream s = resp.GetResponseStream()) {
                byte[] b = new byte[2500];
                int l = 0;
                while ((l = s.Read(b, 0, b.Length)) > 0) {
                    ms.Write(b, 0, l);
                }

                string ret = encoding.GetString(ms.ToArray());

                Messages messages = JsonConvert.DeserializeObject<Messages>(ret);
                if (messages != null)
                    if (messages.Content != null) {
                        UpdateMessages(messages);
                        
                    }
            }
        }

        long cctv_timestamp;
        string cctv_streamer;
        string cctv_nonce;

        //private long unixTimestamp() {
        //    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        //    TimeSpan diff = DateTime.Now - origin;
        //    return (long)Math.Floor(diff.TotalSeconds);
        //}

        public static DateTime UnixTimeStampToDateTime( long unixTimeStamp ) {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
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

                    //Match mmm = ExtractSmile.Match(msg[j].Text);
                    //if (mmm.Success) {
                    //}

                    msg[j].Text = HttpUtility.HtmlDecode(msg[j].Text);

                    NewMessage.Add(msg[j]);
                    msg[j].NeedToDelete = 60 * 5;
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
                                          orderby b.Date
                                          select new ChatMessage() {
                                              Date = UnixTimeStampToDateTime(b.Date),
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
            get { return "Чаты с http://cybergame.tv"; }
        }

        string _Header = "http://cybergame.tv";
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
            //StreamerNick = Config.GetDefault(ConfigPrefix + ".CYBERGAMETVCHAT.StreamerNick", "");
            //_StreamerID = (int)Config.GetDefault(ConfigPrefix + ".CYBERGAMETVCHAT.StreamerID", 0);
            //_ChannelUri = Config.GetDefault(ConfigPrefix + ".CYBERGAMETVCHAT.StreamerURI", "");
            //LoadedMessages.Clear();
            //UpdateHeader();
            //UpdateSmiles();
            OnConfigApply(Config);
        }

        private void UpdateSmiles() {
            try {
                WebClient wc = new WebClient();
                string js = wc.DownloadString(UpdateSmilesUri);

                SmilesUri.Clear();
                Regex smiles = new Regex("\\'(.*?)\\'.*?\\'(.*?)\\',.*?\\}", RegexOptions.Multiline);

                foreach (Match m in smiles.Matches(js))
                    SmilesUri[":s" + m.Groups[1].Value + " "] = "http://chat.sc2tv.ru/img/" + m.Groups[2].Value;
            } catch {
            }
        }

        private void UpdateHeader() {
            if (string.IsNullOrEmpty(StreamerNick)) {
                Header = "http://cybergame.tv, Чат не подключен.";
            } else {
                Header = "http://cybergame.tv, " + StreamerNick;
            }
        }

        public void OnConfigApply( ConfigStorage Config ) {

            cctv_nonce = "";
            cctv_streamer = "";
            cctv_timestamp = 88;

            _ChannelUri = (string)Config.GetDefault(ConfigPrefix + ".CYBERGAMETVCHAT.StreamerURI", "");

            ManualResetEvent mre = new ManualResetEvent(false);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            try {
                //     //"quick_chat_room_name":"soboleff"

                //"quick_chat_username_check_nonce":"c94805e03f",
                //"quick_chat_delete_nonce":"7227a8f20c",
                //"quick_chat_ban_nonce":"a8615484a3",
                //"quick_chat_ban_msg_nonce":"ab5e9ae451",
                //"quick_chat_new_message_nonce":"78ae6168c1",
                //"quick_chat_update_messages_nonce":"e299c4e6c6",
                //"quick_chat_update_users_nonce":"c97c5cae9a",
                //"quick_chat_last_timestamp":1362321788

                //    //"));


                string Result = wc.DownloadString(new Uri(_ChannelUri, UriKind.RelativeOrAbsolute));

                Regex nonceGet = new Regex("quick_chat_update_messages_nonce.*?\\:\\\"(.*?)\\\"");
                Match m = nonceGet.Match(Result);
                if (m.Success) {
                    cctv_nonce = m.Groups[1].Value;
                }

                //Regex nonceGet = new Regex("quick_chat_js_vars.*?{(.*?)}", RegexOptions.Multiline);
                
                //Match m = nonceGet.Match(Result);
                //if (m.Success) {
                //    string[] vals = m.Groups[1].Value.Split(',');
                //}

                nonceGet = new Regex("quick_chat_room_name.*?\\:\\\"(.*?)\\\"");
                m = nonceGet.Match(Result);
                if (m.Success) {
                    StreamerNick = cctv_streamer = m.Groups[1].Value;
                }

                nonceGet = new Regex("quick_chat_last_timestamp.*?\\:(.*?)\\,");
                m = nonceGet.Match(Result);
                if (m.Success) {
                    cctv_timestamp = long.Parse(m.Groups[1].Value);                    
                }

            } catch {
                MessageBox.Show("Ошибка сети или стример не найден.");
            }

            LoadedMessages.Clear();
            UpdateHeader();
            UpdateSmiles();
        }

        public void BeginWork() {
            if (next == null) {
                next = new DispatcherTimer();
                next.Interval = TimeSpan.FromSeconds(5);
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

