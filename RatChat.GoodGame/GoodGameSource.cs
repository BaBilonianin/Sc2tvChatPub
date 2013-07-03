using RatChat.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RatChat.GoodGame {
    [ChatName("Чат для http://goodgame.ru")]
    [ConfigValue(".GOODGAME.StreamerURI", "http://goodgame.ru/channel/Miker/87/", "Адрес страницы стримера goodgame.ru:", false)]
    public class GoodGameSource : RatChat.Core.IChatSource, INotifyPropertyChanged {
        #region Server protocol
        public void setAuth( object arg0, object arg1 ) {
           // Console.WriteLine("setAuth");
        }

        // Server system message
        public void msgFromSrvr( FluorineFx.ASObject msg ) {
            //string ChatText = msg["text"].ToString();
            //DateTime time = DateTime.Parse(msg["time"].ToString());
            ////var Name = msg["sender"] as dotFlex.ASObject;
            //string Sender = "SYSTEM";// Name["name"].ToString();
            //Console.WriteLine(Sender + "  [" + time.ToString() + "] - " + ChatText);
        }
        #endregion
        
        
        const string ChatUri = "rtmp://www.goodgame.ru:443/chat";
        
        public event PropertyChangedEventHandler PropertyChanged;

        public string Copyright {
            get { return "Oxlamon © 2013"; }
        }

        public string Description {
            get { return "Чаты с http://sc2tv.ru"; }
        }

        string _Header = "...";
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


        public string StreamerNick { get; set; }
        public void BeginWork() {
            if (_ChatID != 0) {

                Header = "Подключение к: " + StreamerNick;

                _NetConnection = new FluorineFx.Net.NetConnection() {
                    ObjectEncoding = FluorineFx.ObjectEncoding.AMF0
                };

                _NetConnection.Client = this;
                _NetConnection.OnConnect += _NetConnection_OnConnect;
                _NetConnection.OnDisconnect += _NetConnection_OnDisconnect;
                _NetConnection.NetStatus += _NetConnection_NetStatus;

                _NetConnection.Connect(ChatUri,
                    _UserId, _UserToken, _ChatID);
            }
        }

        void _NetConnection_NetStatus( object sender, FluorineFx.Net.NetStatusEventArgs e ) {
            //Header = "status " + e.Info[
        }

        void _NetConnection_OnDisconnect( object sender, EventArgs e ) {
            Header = "Отключены...";

        }


        private void Reconnect() {
            if (_NetConnection != null) {
                _NetConnection.Close();
                _NetConnection = null;
            }

            BeginWork();
        }

        public void addHistory( FluorineFx.ASObject[] History ) {
            foreach (var msg in History) {
                NewMessage(msg);
            }
        }

        void _NetConnection_OnConnect( object sender, EventArgs e ) {

            _NetConnection.Call("getHistory", new FluorineFx.Net.Responder<FluorineFx.ASObject[]>(addHistory), 0);

            _Chat = (Chat)FluorineFx.Net.RemoteSharedObject.GetRemote(
                typeof(Chat), "chat" + _ChatID, ChatUri, true);
            _Chat.ObjectEncoding = FluorineFx.ObjectEncoding.AMF0;
            _Chat._Source = this;
            _Chat.Connect(_NetConnection);

            // Получение истории добавить
            Header = "http://goodgame.ru, " + StreamerNick;
        }

        public void EndWork() {
        }

        public event OnNewMessagesArrivedDelegate OnNewMessagesArrived;

        public Dictionary<string, string> SmilesUri { get; set; }


        public class Chat : FluorineFx.Net.RemoteSharedObject {
            public GoodGameSource _Source;

            public void msgFromSrvr( FluorineFx.ASObject msg ) {
                _Source.NewMessage(msg);
            }
        }

        Chat _Chat;
        FluorineFx.Net.NetConnection _NetConnection;
        int _ChatID = 0;
        int _UserId = 0;
        string _UserToken = "";

        public void OnLoad( ConfigStorage Config ) {
            StreamerNick = Config.GetDefault(ConfigPrefix + ".GOODGAME.StreamerNick", "");
            _ChatID = Config.GetDefault(ConfigPrefix + ".GOODGAME.ChatId", 0);
            _UserToken = Config.GetDefault(ConfigPrefix + ".GOODGAME.UserToken", "");
            _UserId = Config.GetDefault(ConfigPrefix + ".GOODGAME.UserId", 0);
        }

        public void OnConfigApply( ConfigStorage Config ) {
            string ChannelUri = Config.GetDefault(ConfigPrefix + ".GOODGAME.StreamerURI", "http://goodgame.ru/channel/Miker/87/");

            ManualResetEvent mre = new ManualResetEvent(false);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            try {
                string Result = wc.DownloadString(new Uri(ChannelUri, UriKind.RelativeOrAbsolute));

                // userid=149347&token=b0052d9789be51c36637df97121b407c&chatroom=1089&chatname=Нами&key=Nami4ka
                //var flashvars = {"userid":0,"token":"d41d8cd98f00b204e9800998ecf8427e","chatroom":"5","chatname":"HotS \u0441 \u041c\u0430\u0439\u043a\u0435\u0440\u043e\u043c","key":"Miker"};


                Regex rx = new Regex("userid\\\"\\:(.*?),\\\"token\\\"\\:\\\"(.*?)\\\",\\\"chatroom\\\"\\:\\\"(.*?)\\\",\\\"chatname\\\"\\:\\\"(.*?)\\\",\\\"key\\\":\\\"(.*?)\\\"");
                Match m = rx.Match(Result);
                if (m.Success) {
                    Config[ConfigPrefix + ".GOODGAME.UserId"] = _UserId = int.Parse(m.Groups[1].Value);
                    Config[ConfigPrefix + ".GOODGAME.UserToken"] = _UserToken = m.Groups[2].Value;
                    Config[ConfigPrefix + ".GOODGAME.ChatId"] = _ChatID = int.Parse(m.Groups[3].Value);
                    Config[ConfigPrefix + ".GOODGAME.ChatName"] = m.Groups[4].Value;
                    Config[ConfigPrefix + ".GOODGAME.StreamerNick"] = StreamerNick = m.Groups[5].Value;
                } else {
                    _ChatID = 1717;
                    _UserId = 0;
                    _UserToken = "d41d8cd98f00b204e9800998ecf8427e";
                }
            } catch {
                _ChatID = 0;
                MessageBox.Show("Ошибка сети или стример не найден.");
            }

            Reconnect();
        }

        public string ConfigPrefix { get; set; }

        public System.Windows.Controls.UserControl CreateCustomView() {
            return null;
        }

        internal void NewMessage( FluorineFx.ASObject msg ) {
            string ChatText = msg["text"].ToString();
            DateTime time = DateTime.Parse(msg["time"].ToString());
            var Name = msg["sender"] as FluorineFx.ASObject;
            string Sender = Name["name"].ToString();
            Console.WriteLine(Sender + "  [" + time.ToString() + "] - " + ChatText);

            ChatMessage chatMessage = new ChatMessage() {
                Date = time,
                Name = Sender,
                Text = ChatText
            };

            if (OnNewMessagesArrived != null) {
                List<ChatMessage> l = new List<ChatMessage>();
                l.Add(chatMessage);
                OnNewMessagesArrived(l);
            }
        }
    }
}
