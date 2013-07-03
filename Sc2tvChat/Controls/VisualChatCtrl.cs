using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RatChat.Controls {
    [TemplatePart(Name = "PART_Messages", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_OptionsButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Header", Type = typeof(Label))]
    public class VisualChatCtrl : UserControl, INotifyPropertyChanged {
        static VisualChatCtrl() {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(VisualChatCtrl),
                new FrameworkPropertyMetadata(typeof(VisualChatCtrl)));
        }

        public VisualChatCtrl()
            : base() {
            this.SetResourceReference(VisualChatCtrl.StyleProperty, "VisualChatStyle");
            ChatDataSource = new ObservableCollection<VisualMessage>();
        }

        ListBox PART_Messages;
        Button PART_OptionsButton, PART_CloseButton;
        Label PART_Header;


        protected void FireChange( string PropertyName ) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        ObservableCollection<VisualMessage> ChatDataSource;
        RatChat.Core.IChatSource _Source;
        public RatChat.Core.IChatSource Source {
            get { return _Source; }
            private set {
                _Source = value;
              //  this.DataContext = _Source;
                FireChange("Source");
            }
        }
        public ChatSourceManager Manager { get; set; }

        //public string VisualId { get; set; }
        //public string SourceChatId { get; set; }

        public void ConnectToChatSource( RatChat.Core.IChatSource Source ) {
            this.Source = Source;
            this.Source.OnNewMessagesArrived += Source_OnNewMessagesArrived;
            this.Source.BeginWork();
        }

        void Source_OnNewMessagesArrived( List<Core.ChatMessage> NewMessages ) {

            if (this.Dispatcher.CheckAccess()) {
                Safe_Source_OnNewMessagesArrived(NewMessages);
            } else {
                this.Dispatcher.BeginInvoke(new Action(() => {
                    Safe_Source_OnNewMessagesArrived(NewMessages);
                }));
            }
        }

        void Safe_Source_OnNewMessagesArrived( List<Core.ChatMessage> NewMessages ) {
            for (int j = 0; j < NewMessages.Count; ++j) {
                VisualMessage vm = new VisualMessage(Source, Manager.SmilesDataDase, NewMessages[j]);
                
                if (ChatDataSource.Count > 0) {
                    int prevId = ChatDataSource.Count - 1;
                    vm.DoubleName = vm.Data.Name == ChatDataSource[prevId].Data.Name;
                    if (vm.DoubleName) {
                        if( ChatDataSource[prevId].Sequence == 0 )
                            ChatDataSource[prevId].Sequence = 1;
                        else
                            ChatDataSource[prevId].Sequence = 2;
                        vm.Sequence = 3;
                    } else {
                        vm.Sequence = 0;
                    }
                }

                ChatDataSource.Add(vm);
            }



            if (ChatDataSource.Count > 100) {
                while (ChatDataSource.Count > 40)
                    ChatDataSource.RemoveAt(0);
            }

            //if (ChatDataSource.Count > 0) {
            //    for (int j = 1; j < ChatDataSource.Count; ++j) {
            //        if( ChatDataSource[j-1].Data.Name == ChatDataSource[j-1].Data.Name
            //    }
            //}
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            this.PART_Messages = this.GetTemplateChild("PART_Messages") as ListBox;
            this.PART_CloseButton = this.GetTemplateChild("PART_CloseButton") as Button;
            this.PART_OptionsButton = this.GetTemplateChild("PART_OptionsButton") as Button;
            this.PART_Header = this.GetTemplateChild("PART_Header") as Label;

            this.PART_CloseButton.Click += PART_CloseButton_Click;
            this.PART_OptionsButton.Click += PART_OptionsButton_Click;

            this.PART_Messages.DataContext = ChatDataSource;
            this.PART_Header.DataContext = Source;

            ContextMenu fuckUserContextMenu = new ContextMenu();
            userMenu = new MenuItem() {
                Header = "Fuck the user"
            };
            userMenu.Click += mi_Click;
            fuckUserContextMenu.Items.Add(userMenu);
            fuckUserContextMenu.Items.Add(new MenuItem() {
                Header = "Cancel"
            });

            this.PART_Messages.PreviewMouseRightButtonUp += PART_Messages_MouseDown;
            this.PART_Messages.ContextMenu = fuckUserContextMenu;
        }

        void mi_Click( object sender, RoutedEventArgs e ) {
            if (currentMessage != null) {
                int j = 0;
                do {

                    if (ChatDataSource[j].Data.Name == currentMessage.Data.Name) {
                        ChatDataSource.RemoveAt(j);
                    } else
                        j++;

                } while (j < ChatDataSource.Count);
            }
        }

        MenuItem userMenu;
        VisualMessage currentMessage;

        void PART_Messages_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e ) {
            //var sex = PART_Messages.InputHitTest(e.GetPosition(PART_Messages));


            var hitTestResult = VisualTreeHelper.HitTest(this.PART_Messages, e.GetPosition(PART_Messages));
            var selectedItem = hitTestResult.VisualHit;
            while (selectedItem != null) {
                if (selectedItem is ListBoxItem) {
                    break;
                }
                selectedItem = VisualTreeHelper.GetParent(selectedItem);
            }

            ListBoxItem item = selectedItem != null ? ((ListBoxItem)selectedItem) : null;
            if (item != null) {
                var sex = item.Content as VisualMessage;
                if (sex != null) {
                    //fuckUserContextMenu.Tag = sex;
                    currentMessage = sex;
                    userMenu.Header = "Fuck the " + sex.Data.Name;
                    userMenu.IsEnabled = true;
                }
            } else {
                currentMessage = null;
                userMenu.Header = "Nothing to fuck";
                userMenu.IsEnabled = false;
            }
        }

        void PART_OptionsButton_Click( object sender, RoutedEventArgs e ) {
            if (ChatOptionsWindow.ShowOptionsWindow(this, Manager.ChatConfigStorage)) {
                Source.OnConfigApply(Manager.ChatConfigStorage);
                ChatDataSource.Clear();
            }
        }

        void PART_CloseButton_Click( object sender, RoutedEventArgs e ) {
            this.Manager.OnChatClosed(this);
        }

    }
}
