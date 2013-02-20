using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RatChat.Core;
using System.Globalization;
using System.Windows.Controls;
using System.Windows;

namespace RatChat {
    public class ChatSourceManager {
        public Dictionary<string, Type> Sources { get; private set; }
        public ObservableCollection<FrameworkElement> Chats { get; private set; }
        public RatChat.Core.ConfigStorage ChatConfigStorage { get; private set; }
        public SmilesDataDase SmilesDataDase { get; private set; }
        public Achievment Achievment { get; private set; }

        public ChatSourceManager() {
            SmilesDataDase = new Core.SmilesDataDase();
            Achievment = new RatChat.Achievment();
            Sources = new Dictionary<string, Type>();
            Chats = new ObservableCollection<FrameworkElement>();
            ChatConfigStorage = new Core.ConfigStorage();
        }

        public void Initialize( Controls.ChatsControl ChatsControl, string LayoutStore ) {
            ChatConfigStorage.Load(LayoutStore);

            foreach (string file in Directory.GetFiles(App.RootFolder + "/Chats/", "*.dll")) {
                try {
                    Assembly a = Assembly.LoadFile(file);
                    foreach (Type t in a.GetTypes()) {
                        Type iface = t.GetInterface("RatChat.Core.IChatSource");
                        if (iface != null) {
                            var v = RatChat.Core.ChatNameAttribute.GetAttribute(t);
                            if (v != null) {
                                Sources[v.Name] = t;
                            } else {
                                //Sources[Path.GetFileNameWithoutExtension(file)] = t;
                            }
                        }
                    }
                } catch {
                    //////
                }
            }

            RestoreChats(ChatsControl);
        }

        public void CreateChat( string ConfigPrefix, string SourceChatId ) {
            RatChat.Core.IChatSource ichat = Activator.CreateInstance(Sources[SourceChatId]) as RatChat.Core.IChatSource;
            ichat.ConfigPrefix = ConfigPrefix;
            ichat.OnLoad(ChatConfigStorage);
            ichat.OnNewMessagesArrived += ichat_OnNewMessagesArrived;

           

            foreach (var smile in ichat.SmilesUri)
                SmilesDataDase.AddSmileTuple(smile.Key, smile.Value);

            UserControl customView = ichat.CreateCustomView();

            if (customView == null) {
                RatChat.Controls.VisualChatCtrl vchat = new Controls.VisualChatCtrl();
                vchat.Manager = this;
                vchat.Tag = new Tuple<RatChat.Core.IChatSource, string>(ichat, SourceChatId);
                Chats.Add(vchat);
                vchat.ConnectToChatSource(ichat);
            } else {
                RatChat.Controls.CustomControlContainer vchat = new Controls.CustomControlContainer();
                vchat.Manager = this;
                vchat.Tag = new Tuple<RatChat.Core.IChatSource, string>(ichat, SourceChatId);
                Chats.Add(vchat);
                vchat.ConnectToChatSource(customView, ichat);
            }


            RatChat.Core.IChatListener iListener = ichat as RatChat.Core.IChatListener;
            if (iListener != null) {
                // При добавлении Listener, ищу ВСЕ Source и подписываюсь
                foreach (FrameworkElement fe in Chats) {
                    RatChat.Core.IChatSource i = ((Tuple<RatChat.Core.IChatSource, string>)fe.Tag).Item1;
                    i.OnNewMessagesArrived += iListener.OnNewMessageReceived;
                }
            } else {
                // При добавлении Source, ищу ВСЕ Listeners и подписываю
                foreach (FrameworkElement fe in Chats) {
                    RatChat.Core.IChatSource i = ((Tuple<RatChat.Core.IChatSource, string>)fe.Tag).Item1;
                    RatChat.Core.IChatListener il= i as RatChat.Core.IChatListener;
                    if (il != null)
                        ichat.OnNewMessagesArrived += il.OnNewMessageReceived;
                    //i.OnNewMessagesArrived += iListener.OnNewMessageReceived;
                }
            }
        }

        void ichat_OnNewMessagesArrived( List<ChatMessage> NewMessages ) {
            foreach (ChatMessage cm in NewMessages)
                Achievment.OnChatMessate(cm);
        }

        public void OnChatClosed( FrameworkElement vChat ) {
            var data = vChat.Tag as Tuple<RatChat.Core.IChatSource, string>;
            data.Item1.OnNewMessagesArrived -= ichat_OnNewMessagesArrived;
            data.Item1.EndWork();


            RatChat.Core.IChatListener iListener = data.Item1 as RatChat.Core.IChatListener;
            if (iListener == null) {
                foreach (FrameworkElement fe in Chats) {
                    RatChat.Core.IChatSource i = ((Tuple<RatChat.Core.IChatSource, string>)fe.Tag).Item1;
                    RatChat.Core.IChatListener il = i as RatChat.Core.IChatListener;
                    if (il != null)
                        data.Item1.OnNewMessagesArrived -= il.OnNewMessageReceived;
                    //i.OnNewMessagesArrived += iListener.OnNewMessageReceived;
                }
            } else {
                foreach (FrameworkElement fe in Chats) {
                    RatChat.Core.IChatSource i = ((Tuple<RatChat.Core.IChatSource, string>)fe.Tag).Item1;
                    i.OnNewMessagesArrived -= iListener.OnNewMessageReceived;
                } 
            }

            Chats.Remove(vChat);

            ChatConfigStorage.RemoveWithPrefix(data.Item1.ConfigPrefix);
        }

        public void RestoreChats( Controls.ChatsControl ChatsControl ) {
            string Settings = ChatConfigStorage.GetDefault("ChatManager.UberChatList", "");
            if (string.IsNullOrEmpty(Settings))
                return;

            string[] chats = Settings.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

            for (int j = 0; j < chats.Length; ++j) {
                string[] vals = chats[j].Split('=');
                CreateChat(vals[1], vals[0]);

                if( vals.Length > 2 )
                    ChatsControl.SetChatHeightByIndex(j, double.Parse(vals[2], CultureInfo.InvariantCulture));
            }
        }


        public void StoreChats( Controls.ChatsControl ChatsControl ) {
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < Chats.Count; ++j) {

                var data = Chats[j].Tag as Tuple<RatChat.Core.IChatSource, string>;
            
                sb.AppendFormat( 
                    CultureInfo.InvariantCulture,
                    "|{0}={1}={2}", 
                    data.Item2, 
                    data.Item1.ConfigPrefix,
                    ChatsControl.GetChatHeightByIndex(j) );
            }
            ChatConfigStorage["ChatManager.UberChatList"] = sb.ToString();
        }
    }
}
