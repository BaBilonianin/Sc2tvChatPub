using dotIRC;
using Newtonsoft.Json;
using RatChat.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace RatChat.Twitch {
    [ChatName("Чат для http://twitch.tv")]
   // [ConfigValue(".TWITCHTVCHAT.StreamerPassword", "", "Пароль для twitch:", true)]
    [ConfigValue(".TWITCHTVCHAT.StreamerNick", "", "Ваш ник на twitch:", false)]
    [ConfigValue(".TWITCHTVCHAT.DirectConnect", "199.9.250.229:6667", "x.x.x.x:y для коннекта:", false)]
    public class TwitchChatSource : RatChat.Core.IChatSource, INotifyPropertyChanged, RatChat.Core.ISmileCreator {
        Dispatcher Dispatcher;
        public const string UpdateSmilesUri = "https://api.twitch.tv/kraken/chat/emoticons";


        public TwitchChatSource() {
            Dispatcher = Dispatcher.CurrentDispatcher;
            SmilesUri = new Dictionary<string, string>();
        }

        protected void FireChange( string PropertyName ) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Copyright {
            get { return "Oxlamon © 2013"; }
        }

        public string Description {
            get { return "Чаты с http://twitch.tv"; }
        }

        string _Header = "http://twitch.tv";
        public string Header {
            get { return _Header; }
            set {
                if (string.Compare(_Header, value) != 0) {
                    _Header = value;
                    if (Dispatcher.CheckAccess()) {
                        FireChange("Header");
                    } else {
                        Dispatcher.Invoke(new Action(() => {
                            FireChange("Header");
                        }));                        
                    }
                }
            }
        }

        public string StreamerNick { get; set; }

        string _directAdress = null;

        public System.Windows.FrameworkElement CreateSmile( string id ) {
            return null;
        }

        public void OnLoad( ConfigStorage Config ) {
            OnConfigApply(Config);
        }

        public void OnConfigApply( ConfigStorage Config ) {
            StreamerNick = Config.GetDefault(ConfigPrefix + ".TWITCHTVCHAT.StreamerNick", "");
            _directAdress = Config.GetDefault(ConfigPrefix + ".TWITCHTVCHAT.DirectConnect", "");

            if (string.IsNullOrEmpty(StreamerNick) && string.IsNullOrEmpty(_directAdress)) {
                Header = "http://twitch.tv, Нет подключения";
                return;
            }
        
            UpdateSmiles();
            Reconnect();
        }

        internal class EmoticonImage {
            [JsonProperty(PropertyName = "emoticon_set")]
            public int? Set { get; set; }
            [JsonProperty(PropertyName = "url")]
            public string Uri { get; set; }
            [JsonProperty(PropertyName = "height")]
            public int Height { get; set; }
            [JsonProperty(PropertyName = "width")]
            public int Width { get; set; }
        }

        internal class Emoticon {
            [JsonProperty(PropertyName = "images")]
            public EmoticonImage[] Images { get; set; }
            [JsonProperty(PropertyName = "regex")]
            public string Regex { get; set; }

            public bool AllowToAdd() {
                for (int j = 0; j < Images.Length; ++j)
                    if (!Images[j].Set.HasValue)
                        return true;
                return false;
            }

            public string GetUri() {
                for (int j = 0; j < Images.Length; ++j)
                    if (!Images[j].Set.HasValue)
                        return Images[j].Uri;
                return null;
            }
        }

        internal class Emoticons {
            [JsonProperty(PropertyName = "emoticons")]
            public Emoticon[] EmoticonsArray { get; set; }
        }

        internal class TwitchSmile {
            public readonly Regex Regex;
            public readonly Uri Uri;

            public TwitchSmile( Emoticon emoticon ) {
                this.Regex = new Regex(emoticon.Regex);
                this.Uri = new Uri(emoticon.GetUri(), UriKind.RelativeOrAbsolute);
                //this.Icon = new BitmapImage(this.Uri);
            }
        }

        TwitchSmile[] Smiles;

        private void UpdateSmiles() {
            try {
                WebClient wc = new WebClient();
                string js = wc.DownloadString(UpdateSmilesUri);

                Emoticons asex = JsonConvert.DeserializeObject<Emoticons>(js);

                Smiles = (from porno in asex.EmoticonsArray
                          where porno.AllowToAdd()
                          select new TwitchSmile(porno)).ToArray();


            } catch (Exception e ) {
                MessageBox.Show("Фак смайлы: " + e.Message);
            }
        }

        // 

        void IrcClient_ConnectFailed( object sender, IrcErrorEventArgs e ) {
            Reconnect();
        }

        private void Reconnect() {
            if (IrcClient != null) {
                IrcClient.Disconnect();
                IrcClient = null;
            }

            IrcClient = new IrcClient();

            IrcClient.ClientId = "TWITCHCLIENT 2";

            IrcClient.Connected += IrcClient_Connected;
            IrcClient.ProtocolError += IrcClient_ProtocolError;
            IrcClient.Error += IrcClient_Error;
            IrcClient.Disconnected += IrcClient_Disconnected;
            IrcClient.RawMessageReceived += IrcClient_RawMessageReceived;
            IrcClient.ConnectFailed += IrcClient_ConnectFailed;

            IrcClient.MotdReceived += IrcClient_MotdReceived;

            Random rnd = new Random();
            string s = "justinfan" + rnd.Next(100000000);
            regInfo = new IrcUserRegistrationInfo() {
                NickName = s,
                UserName = s,
                Password = "blah",
                RealName = s
            };

            Header = "http://twitch.tv, Подключаемся к " + StreamerNick;

            if (!string.IsNullOrEmpty(_directAdress)) {
                string[] dat = _directAdress.Split(':');
                try {
                    int port = int.Parse(dat[1]);
                    IrcClient.Connect(dat[0], port, false, regInfo);
                } catch {
                    Header = "http://twitch.tv, Ошибка " + StreamerNick;
                }
            } else {
                IrcClient.Connect(StreamerNick + ".jtvirc.com", 6667, false, regInfo);
            }
        }

        void IrcClient_MotdReceived( object sender, EventArgs e ) {
        }

        void IrcClient_ClientInfoReceived( object sender, EventArgs e ) {
        }

        void IrcClient_NetworkInformationReceived( object sender, EventArgs e ) {
        }


        void IrcClient_Error( object sender, IrcErrorEventArgs e ) {
            Reconnect();
        }

        void IrcClient_ProtocolError( object sender, IrcProtocolErrorEventArgs e ) {
            Reconnect();
        }
      
        IrcClient IrcClient;
        IrcRegistrationInfo regInfo;
        Regex parsingRegex = new Regex(@"^(:(?<prefix>\S+) )?(?<command>\S+)( (?!:)(?<params>.+?))?( :(?<trail>.+))?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        public void BeginWork() { }

        bool ParseIrcMessageWithRegex( string message, out string prefix, out string command, out string[] parameters ) {
            string trailing = null;
            prefix = command = String.Empty;
            parameters = new string[] { };

            Match messageMatch = parsingRegex.Match(message);

            if (messageMatch.Success) {
                prefix = messageMatch.Groups["prefix"].Value;
                command = messageMatch.Groups["command"].Value;
                parameters = messageMatch.Groups["params"].Value.Split(' ');
                trailing = messageMatch.Groups["trail"].Value;

                if (!String.IsNullOrEmpty(trailing))
                    parameters = parameters.Concat(new string[] { trailing }).ToArray();
                return true;
            }
            return false;

        }

        string getUserName( string prefix ) {
            string[] nm = prefix.Split('!');
            if (nm.Length == 0 || nm[0].Length<2)
                return "";
            return
                nm[0].Substring(0, 1).ToUpper() +
                nm[0].Substring(1);
        }

        string dots = "";

        void IrcClient_RawMessageReceived( object sender, IrcRawMessageEventArgs e ) {
            if (OnNewMessagesArrived != null) {
                string prefix, command;
                string[] parameters;
                ParseIrcMessageWithRegex(e.RawContent, out prefix, out command, out parameters);
                List<ChatMessage> msgs = new List<ChatMessage>();
                string username = getUserName(prefix);

                dots += ".";
                if (dots.Length > 3)
                    dots = "";

                 //msgs.Add(new ChatMessage() {
                 //               Date = DateTime.Now,
                 //               Name = "RAW",
                 //               Text = e.RawContent
                 //});

               // System.IO.File.AppendAllText("x:\\twitch.log", e.RawContent + "\r\n");


                switch (command) {
                    case "PART":
                    case "JOIN":
                        Header = "http://twitch.tv, " + StreamerNick + dots;
                        break;

                    case "PRIVMSG":
                        Header = "http://twitch.tv, " + StreamerNick;
                        if (username != "Jtv") {
                            msgs.Add(new ChatMessage() {
                                Date = DateTime.Now,
                                Name = username,
                                Text = parameters[1]
                            });
                        } else {
                            //msgs.Add(new ChatMessage() {
                            //    Date = DateTime.Now,
                            //    Name = "SYSTEM",
                            //    Text = parameters[1]
                            //});
                        }
                        break;

                    //case "+":
                    //    msgs.Add(new ChatMessage() {
                    //        Date = DateTime.Now,
                    //        Name = prefix,
                    //        Text = "[УДАЛЕНИЕ] " + e.RawContent
                    //    });
                    //    break;

                    //default:
                    //    msgs.Add(new ChatMessage() {
                    //        Date = DateTime.Now,
                    //        Name = prefix,
                    //        Text = "["+command + "] " + e.RawContent
                    //    });
                    //    break;
                }
               
                if( msgs.Count > 0 )
                    OnNewMessagesArrived(msgs);
            }
        }

        void IrcClient_Disconnected( object sender, EventArgs e ) {
        }
        
        void IrcClient_Connected( object sender, EventArgs e ) {
            Header = "http://twitch.tv, " + StreamerNick;
         
            IrcClient.Channels.Join("#" + StreamerNick.ToLowerInvariant());


            //IrcClient.SendRawMessage("TWITCHCLIENT 2");

            //try {
            //    
            //} catch (Exception ee ) {
            //}
        }

        public void EndWork() {
            if (IrcClient != null) {
                IrcClient.Disconnect();
                IrcClient = null;
                Header = "http://twitch.tv, Не подключен";
            }
        }

        public event OnNewMessagesArrivedDelegate OnNewMessagesArrived;

        public Dictionary<string, string> SmilesUri { get; private set; }

        public System.Windows.Controls.UserControl CreateCustomView() {
            return null;
        }

        public string ConfigPrefix { get; set; }

        public bool CreateSmile( string SmileId, System.Windows.Controls.WrapPanel TextPanel ) {
            System.Windows.FrameworkElement s = GetSmile(SmileId);
            if (s != null) {
                TextPanel.Children.Add(s);
                return true;
            }

            return false; // Смайл нормуль, и фалсе если не удалось
        }

        public FrameworkElement GetSmile( string id ) {
            ContentPresenter cp = new ContentPresenter();
            Smile bi = null;

            for (int j = 0; j < Smiles.Length; ++j) {
                if (Smiles[j].Regex.IsMatch(id)) {
                    bi = new Smile() {
                        Image = new BitmapImage( Smiles[j].Uri ),
                        Uri = Smiles[j].Uri,
                        Id = id
                    };

                    cp.Content = bi;
                    cp.SetResourceReference(ContentPresenter.ContentTemplateProperty, "SmileStyle2");
                    return cp;
                }
            }

            return null;
        }
    }
}
