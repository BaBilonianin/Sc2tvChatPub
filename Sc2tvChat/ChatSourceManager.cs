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

namespace RatChat {
    public class ChatSourceManager {
        public Dictionary<string, Type> Sources { get; private set; }
        public ObservableCollection<RatChat.Controls.VisualChatCtrl> Chats { get; private set; }
        public RatChat.Core.ConfigStorage ChatConfigStorage { get; private set; }

        public ChatSourceManager() {
            Sources = new Dictionary<string, Type>();
            Chats = new ObservableCollection<Controls.VisualChatCtrl>();
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
                } catch (Exception e) {
                    //////
                }
            }

            RestoreChats(ChatsControl);
        }

        public RatChat.Controls.VisualChatCtrl CreateChat( string VisualChatId, string SourceChatId ) {
            RatChat.Controls.VisualChatCtrl vchat = new Controls.VisualChatCtrl();

            vchat.VisualId = VisualChatId;
            vchat.Manager = this;
            vchat.SourceChatId = SourceChatId;

            RatChat.Core.IChatSource ichat = Activator.CreateInstance(Sources[SourceChatId]) as RatChat.Core.IChatSource;
            ichat.OnLoad(VisualChatId, ChatConfigStorage);

            vchat.ConnectToChatSource(ichat);
            Chats.Add(vchat);
            return vchat;
        }

        public void OnChatClosed( RatChat.Controls.VisualChatCtrl vChat ) {
            Chats.Remove(vChat);
            // remove options
            ChatConfigStorage.RemoveWithPrefix(vChat.VisualId);
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
                sb.AppendFormat( 
                    CultureInfo.InvariantCulture,
                    "|{0}={1}={2}", 
                    Chats[j].SourceChatId, 
                    Chats[j].VisualId,
                    ChatsControl.GetChatHeightByIndex(j) );
            }
            ChatConfigStorage["ChatManager.UberChatList"] = sb.ToString();
        }
    }
}
