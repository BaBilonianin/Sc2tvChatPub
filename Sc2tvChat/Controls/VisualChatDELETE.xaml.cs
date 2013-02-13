using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RatChat.Controls {
    /// <summary>
    /// Interaction logic for VisualChat.xaml
    /// </summary>
 
    public partial class VisualChatDeleted : UserControl {


        public VisualChatDeleted() {
            InitializeComponent();
            ChatDataSource = new ObservableCollection<VisualMessage>();
            Chat.DataContext = ChatDataSource;
        }

        ObservableCollection<VisualMessage> ChatDataSource;
        public RatChat.Core.IChatSource Source { get; private set; }
        public ChatSourceManager Manager { get; set; }

        public string VisualId { get; set; }
        public string SourceChatId { get; set; }

        public void ConnectToChatSource( RatChat.Core.IChatSource Source ) {
            this.Source = Source;
            this.ChatHeader.SetResourceReference(ContentPresenter.ContentTemplateProperty, Source.HeaderDataSkin);
            this.Source.OnNewMessagesArrived += Source_OnNewMessagesArrived;
            this.Source.BeginWork();
        }

        void Source_OnNewMessagesArrived( List<Core.ChatMessage> NewMessages ) {
            this.ChatHeader.DataContext = Source.HeaderData;

            for (int j = 0; j < NewMessages.Count; ++j)
                ChatDataSource.Add(new VisualMessage(Source, NewMessages[j]));

            while (ChatDataSource.Count > 40)
                ChatDataSource.RemoveAt(0);
        }

        private void options_Click_1( object sender, RoutedEventArgs e ) {
           
        }

        private void close_Click_1( object sender, RoutedEventArgs e ) {
           
        }

       
    }
}
