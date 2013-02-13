using RatChat.Controls;
using RatChat.Core;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace RatChat {
    /// <summary>
    /// Interaction logic for ChatOptionsWindow.xaml
    /// </summary>
    public partial class ChatOptionsWindow : Window {
        public ChatOptionsWindow() {
            InitializeComponent();
        }

       public static void ShowOptionsWindow( VisualChatCtrl ChatControl, RatChat.Core.ConfigStorage ChatConfigStorage ) {
            ChatOptionsWindow cow = new ChatOptionsWindow();

            var configs = ConfigValueAttribute.GetAttribute(ChatControl.Source.GetType());

            for (int j=0; j<configs.Length; ++j ) {
                cow.OptionsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30.0) });

                // add text,
                TextBlock text = new TextBlock() { Text = configs[j].Caption };
                text.SetResourceReference(TextBlock.StyleProperty, "ConfigText");

                cow.OptionsGrid.Children.Add(text);
                Grid.SetRow(text, j);

                // add textbox

                TextBox val = new TextBox() { Tag = ChatControl.VisualId + configs[j].Name, Margin = new Thickness(2) };
                val.Text = (string)ChatConfigStorage.GetDefault(ChatControl.VisualId + configs[j].Name, configs[j].DefaultValue);

                cow.OptionsGrid.Children.Add(val);
                Grid.SetRow(val, j);
                Grid.SetColumn(val, 1);
            }

            bool? ret = cow.ShowDialog();
            if (ret.HasValue && ret.Value) {
                // save
                for (int j = 0; j < cow.OptionsGrid.Children.Count; ++j) {
                    TextBox val = cow.OptionsGrid.Children[j] as TextBox;
                    if (val != null) {
                        string name = val.Tag as string;
                        ChatConfigStorage[name] = val.Text;
                    }
                }

            }
        }

        private void Cansel_Click_1( object sender, RoutedEventArgs e ) {
            this.DialogResult = false;
        }

        private void Commit_Click_1( object sender, RoutedEventArgs e ) {
            this.DialogResult = true;
        }
    }
}
