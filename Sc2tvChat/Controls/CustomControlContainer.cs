using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RatChat.Controls {
    [TemplatePart(Name = "PART_Content", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_OptionsButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
   // [TemplatePart(Name = "PART_Header", Type = typeof(TextBlock))]
    public class CustomControlContainer : UserControl, INotifyPropertyChanged {
         static CustomControlContainer() {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(CustomControlContainer),
                new FrameworkPropertyMetadata(typeof(CustomControlContainer)));
        }

         protected void FireChange( string PropertyName ) {
             if (PropertyChanged != null) {
                 PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
             }
         }

         public event PropertyChangedEventHandler PropertyChanged;

         public CustomControlContainer()
            : base() {
            this.SetResourceReference(VisualChatCtrl.StyleProperty, "CustomControlContainerStyle");
            this.DataContext = this;
        }

        ContentPresenter PART_Content;
        Button PART_OptionsButton, PART_CloseButton;
     //   TextBlock PART_Header;

        RatChat.Core.IChatSource _Source;
        public RatChat.Core.IChatSource Source {
            get { return _Source; }
            private set {
                _Source = value;
                FireChange("Source");
            }
        }
        public ChatSourceManager Manager { get; set; }

        public UserControl CustomContent {
            get { return GetValue(CustomContentProperty) as UserControl; }
            set { SetValue(CustomContentProperty, value); }
        }

        public static readonly DependencyProperty CustomContentProperty = DependencyProperty.Register(
            "CustomContent", typeof(UserControl), typeof(CustomControlContainer), new PropertyMetadata(null));

        public void ConnectToChatSource(UserControl CustomView,  RatChat.Core.IChatSource Source ) {
            this.Source = Source;
            this.CustomContent = CustomView;
            this.Source.BeginWork();
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            this.PART_Content = this.GetTemplateChild("PART_Content") as ContentPresenter;
            this.PART_CloseButton = this.GetTemplateChild("PART_CloseButton") as Button;
            this.PART_OptionsButton = this.GetTemplateChild("PART_OptionsButton") as Button;
            //this.PART_Header = this.GetTemplateChild("PART_Header") as TextBlock;

            this.PART_CloseButton.Click += PART_CloseButton_Click;
            this.PART_OptionsButton.Click += PART_OptionsButton_Click;

            //this.PART_Content.DataContext = this;
           // this.PART_Header.DataContext = Source;
        }

        void PART_OptionsButton_Click( object sender, RoutedEventArgs e ) {
            if( ChatOptionsWindow.ShowOptionsWindow(this, Manager.ChatConfigStorage) )
                Source.OnConfigApply(Manager.ChatConfigStorage);
        }

        void PART_CloseButton_Click( object sender, RoutedEventArgs e ) {
            this.Manager.OnChatClosed(this);
        }

    }
}
