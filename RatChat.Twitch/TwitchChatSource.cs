using dotIRC;
using RatChat.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RatChat.Twitch {
    [ChatName("Чат для http://twitch.tv")]
   // [ConfigValue(".TWITCHTVCHAT.StreamerPassword", "", "Пароль для twitch:", true)]
    [ConfigValue(".TWITCHTVCHAT.StreamerNick", "", "Ваш ник на twitch:", false)]
    public class TwitchChatSource : RatChat.Core.IChatSource, INotifyPropertyChanged {
        Dispatcher Dispatcher;

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

        public System.Windows.FrameworkElement CreateSmile( string id ) {
            return null;
        }

        public void OnLoad( string ConfigPrefix, ConfigStorage Config ) {
            OnConfigApply(ConfigPrefix, Config);
        }

        public void OnConfigApply( string ConfigPrefix, ConfigStorage Config ) {
            StreamerNick = Config.GetDefault(ConfigPrefix + ".TWITCHTVCHAT.StreamerNick", "");

            if (string.IsNullOrEmpty(StreamerNick)) {
                Header = "http://twitch.tv, Нет подключения";
                return;
            }

            Reconnect();
        }

        void IrcClient_ConnectFailed( object sender, IrcErrorEventArgs e ) {
            Reconnect();
        }

        private void Reconnect() {
            if (IrcClient != null) {
                IrcClient.Disconnect();
                IrcClient = null;
            }

            IrcClient = new IrcClient();
            IrcClient.Connected += IrcClient_Connected;
            IrcClient.ProtocolError += IrcClient_ProtocolError;
            IrcClient.Error += IrcClient_Error;
            IrcClient.Disconnected += IrcClient_Disconnected;
            IrcClient.RawMessageReceived += IrcClient_RawMessageReceived;
            IrcClient.ConnectFailed += IrcClient_ConnectFailed;

            Random rnd = new Random();
            string s = "justinfan" + rnd.Next(100000000);
            regInfo = new IrcUserRegistrationInfo() {
                NickName = s,
                UserName = s,
                Password = "blah",
                RealName = s
            };

            Header = "http://twitch.tv, Подключаемся к " + StreamerNick;
            IrcClient.Connect(StreamerNick + ".jtvirc.com", 6667, false, regInfo);
        }


        void IrcClient_Error( object sender, IrcErrorEventArgs e ) {
            Reconnect();
        }

        void IrcClient_ProtocolError( object sender, IrcProtocolErrorEventArgs e ) {
            Reconnect();
        }
      
        IrcClient IrcClient;
        IrcRegistrationInfo regInfo;

        public void BeginWork() { }

        bool ParseIrcMessageWithRegex( string message, out string prefix, out string command, out string[] parameters ) {
            string trailing = null;
            prefix = command = String.Empty;
            parameters = new string[] { };

            Regex parsingRegex = new Regex(@"^(:(?<prefix>\S+) )?(?<command>\S+)( (?!:)(?<params>.+?))?( :(?<trail>.+))?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
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
                        }
                        break;

                    default:
                        //msgs.Add(new ChatMessage() {
                        //    Date = DateTime.Now,
                        //    Name = prefix,
                        //    Text = e.RawContent
                        //});
                        break;
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
        }

        public void EndWork() { }

        public event OnNewMessagesArrivedDelegate OnNewMessagesArrived;


        public Dictionary<string, string> SmilesUri { get; private set; }
    }
}
