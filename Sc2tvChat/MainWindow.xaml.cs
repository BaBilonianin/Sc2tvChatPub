using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Sc2tvChat {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            headerButtons.Visibility = System.Windows.Visibility.Hidden;

            ReloadChatStyles(); // На данном этапе сохраненка уже загружена, и глупо менят скин в реалтайме,

            RenderMessages = new List<RenderMessage>();
            render = new DispatcherTimer();
            next = new DispatcherTimer();

            ClassicView = Properties.Settings.Default.classicView;
            HideSmiles = Properties.Settings.Default.hideSmiles;

            pollCtrl.Visibility = System.Windows.Visibility.Hidden;

            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;

            ClockTB.Text = "SC2TV.ru chat"; // Заголовок уродский, некрасивый, мало анимаций. фигня в общем.
        }

        /// Отказ от смены скина в реальном времени, лень апдатить чата размеры.
        /// 
        //string _currentSkin = "DefaultSkin";
        //public string CurrentSkin {
        //    get { return _currentSkin; }
        //    set {
        //        if (_currentSkin != value) {
        //            ResourceDictionary skin = new ResourceDictionary();
        //            skin.Source = new Uri("Skins/" + _currentSkin + ".xaml", UriKind.Relative);
        //            Application.Current.Resources.MergedDictionaries.Remove(skin);

        //            _currentSkin = value;

        /// ------------------------------------------------------------------ >8
        //            skin.Source = new Uri("Skins/" + _currentSkin + ".xaml", UriKind.Relative);
        //            Application.Current.Resources.MergedDictionaries.Add(skin);
        /// ------------------------------------------------------------------ >8

        //            // Skin есть, обновить часть волшебного мира счастья и улыбок (упоролся?)
        //            ReloadChatStyles(); 
        //        }
        //    }
        //}

        private void ReloadChatStyles() {
            NameStyle = (Style)App.Current.Resources["NameStyle"];
            TextStyle = (Style)App.Current.Resources["TextStyle"];
            LinkStyle = (Style)App.Current.Resources["LinkStyle"];
            NameTextStyle = (Style)App.Current.Resources["NameTextStyle"];
            StreamerContainer = (Style)App.Current.Resources["StreamerContainer"];
            NormalTextContainer = (Style)App.Current.Resources["NormalTextContainer"];
            TextSmileStyle = (Style)App.Current.Resources["TextSmileStyle"];
        }



        void Default_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e ) {
            if (e.PropertyName == "streamerID") {
                PekaCount = 0;
                RenderMessages.Clear();
                AnimCanvas.Children.Clear();

                pollCtrl.CancelPoll();
                pollCtrl.Visibility = System.Windows.Visibility.Hidden;

            }
        }
        #region Dirty кусок для ТОЛЬКО загрузки
        bool ClassicView; // Так сделано потому, что новые коменты не должны добавляться в измененном виде
        bool HideSmiles;  // Так сделано потому, что новые коменты не должны добавляться со смайлами
        #endregion

        const double NameWidth = 95.0; // TODO: Бред, надо сделать зависимость от скина. :D
        const string LinkReplacer = "%LINKLINK%";

        int PekaCount = 0;
        Regex UriDetector = new Regex(@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)");
        DispatcherTimer render, next;
        List<RenderMessage> RenderMessages;
        SmilesDataDase SmilesDataDase;


        Style NameStyle, TextStyle, LinkStyle, TextSmileStyle, NameTextStyle, StreamerContainer, NormalTextContainer; 


        private void Window_Loaded_1( object sender, RoutedEventArgs e ) {
            SmilesDataDase = new SmilesDataDase();
         
            //string s = "";
            //string ssss = File.ReadAllText( "x:\\smiles.txt" );
            //Regex rx = new Regex("\\<img.*?src.*?\\\"(.*?)\\\".*?title.*?\\\"(.*?)\\\".*?\\>");
            //foreach (Match m in rx.Matches(ssss)) {
            //    s += string.Format("SmilesUri[\"{1} \"] = new Uri(\"http://chat.sc2tv.ru{0}\");\r\n", m.Groups[1].Value, m.Groups[2].Value);
            //}

            if (ClassicView) {
                divider.Visibility = System.Windows.Visibility.Hidden;
            }
            
            render.Interval = TimeSpan.FromMilliseconds(50);
            render.Tick += render_Tick;
            render.Start();

            
            next.Interval = TimeSpan.FromSeconds(1);
            next.Tick += next_Tick;
            next.Start();

            if (Properties.Settings.Default.streamerID == 0) {
                Button_Click_1(null, null);
            }
        }

        void next_Tick( object sender, EventArgs e ) {
            if (Properties.Settings.Default.streamerID != 0) {
                next.Stop();
                LoadChat(Properties.Settings.Default.streamerID);
            }
        }

        void render_Tick( object sender, EventArgs e ) {

            ClockTB.Text = string.Format("Peka's count: {0}", PekaCount);


            double y = AnimCanvas.ActualHeight;
            
            for (int j = RenderMessages.Count - 1; j >= 0; --j) {
                if (j >= 0)
                    switch (RenderMessages[j].State) {
                        case 0:
                            if (y >= 0) {
                                CreateVisual(RenderMessages[j]);
                            } else {
                                RenderMessages[j].State = 2;
                            }
                            break;
                        case 1:
                            y -= RenderMessages[j].Height;
                            RenderMessages[j].DestHeight = y;
                            if (y < -200)
                                RenderMessages[j].State = 2;
                            break;
                        case 2:
                            if (RenderMessages[j].Name != null) {
                                AnimCanvas.Children.Remove(RenderMessages[j].Name);
                                RenderMessages[j].Name = null;
                            }
                            if (RenderMessages[j].Text != null) {
                                AnimCanvas.Children.Remove(RenderMessages[j].Text);
                                RenderMessages[j].Text = null;
                            }
                            RenderMessages[j].State = 3;
                            break;

                        case 3:
                            RenderMessages[j].Live--;
                            break;
                    }
            }

            int i = 0;
            while (i < RenderMessages.Count) {
                if (RenderMessages[i].Live < 0) {
                    RenderMessages.RemoveAt(i);
                } else {
                    i++;
                }
            }

            /// Анимация.
            for (int j = 0; j < RenderMessages.Count; ++j) {
                if (RenderMessages[j].State < 3) {
                    if (RenderMessages[j].Text == null) {
                        RenderMessages[j].State = 3;
                    } else {
                        double oy = Canvas.GetTop(RenderMessages[j].Text);

                        double dy = (RenderMessages[j].DestHeight - oy) / 5.0;

                        //if (Convert.ToInt32(dy) != 0) {

                            Canvas.SetTop(RenderMessages[j].Text, oy + dy);

                        if( RenderMessages[j].Name != null )
                            Canvas.SetTop(RenderMessages[j].Name, oy + dy);
                       // }
                    }
                }
            }
        }

        private double MaximumMessageWidth {
            get { 
                if( ClassicView )
                    return AnimCanvas.ActualWidth - 10.0;
                return AnimCanvas.ActualWidth - NameWidth - 10.0;
            }
        }

        // Функция более 20 строк, АААААААААААААААААААААААААААААААААААААААААААА, да пох

        private void CreateVisual( RenderMessage renderMessage ) {
            List<Uri> Urls = new List<Uri>();

            string UserText = HttpUtility.HtmlDecode(renderMessage.Data.Text.Replace(":s:", " :s:").Replace("  ", " "));
            
            UserText = UriDetector.Replace(

                UserText, 
              
                // Урлы, выделяем, чистим, заменяем на прелести.
                new MatchEvaluator(( m ) => {
                    Urls.Add(new Uri(m.Value, UriKind.RelativeOrAbsolute));
                    return LinkReplacer + " ";
                })
            );


            if (ClassicView) {
                // Отдельного имени нету в Classic View
            } else {
                // Oxlamon View mode
                renderMessage.Name = new TextBlock() { Text = renderMessage.Data.Name };
                renderMessage.Name.Style = NameStyle;
                renderMessage.Name.Measure(new Size(NameWidth, double.PositiveInfinity));
                renderMessage.Name.Arrange(new Rect(0, 0, renderMessage.Name.DesiredSize.Width, renderMessage.Name.DesiredSize.Height));
                Canvas.SetLeft(renderMessage.Name, NameWidth - renderMessage.Name.ActualWidth);
                Canvas.SetTop(renderMessage.Name, AnimCanvas.ActualHeight);
            }

            // parse text
            WrapPanel wp = new WrapPanel() {
                MaxWidth = MaximumMessageWidth,
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                Style = NormalTextContainer
            };
            List<string> ttt = new List<string>();

            pollCtrl.RegisterVote(renderMessage.Data);


            // Тоже странный кусок:
            int nxd = UserText.IndexOf("<b>");
            if (nxd >= 0) {
                int nxd2 = UserText.IndexOf("</b>");
                renderMessage.TalkTo = UserText.Substring(nxd + 3, nxd2 - nxd - 3);
                UserText = UserText.Substring(nxd2 + 6);

                if (renderMessage.TalkTo == Properties.Settings.Default.streamerNick) {
                    wp.Style = StreamerContainer;
                }

                renderMessage.TalkTo = renderMessage.TalkTo + ", ";
                ttt.Add(renderMessage.TalkTo);

            } else {
                renderMessage.TalkTo = "";
            }

            ttt.AddRange(UserText.Split(' '));

            if (ClassicView) {
                TextBlock name = new TextBlock() { Text = renderMessage.Data.Name + ": " };
                name.Style = NameStyle;
                wp.Children.Add(name);
            }

            // check smiles inside
            int linkIndex = 0;

            for (int j = 0; j < ttt.Count; ++j) {
                Style s;
                if (j == 0 && !string.IsNullOrEmpty(renderMessage.TalkTo))
                    s = NameTextStyle;
                else
                    s = TextStyle;

                if (ttt[j] == LinkReplacer) {
                    TextBlock link = new TextBlock() {
                        Text = "link ",
                        Cursor = Cursors.Hand,
                        Style = LinkStyle,
                        ToolTip = Urls[linkIndex],
                        Tag = Urls[linkIndex]
                    };
                    link.MouseLeftButtonUp += ( sender, b ) => {
                        Uri u = ((TextBlock)sender).Tag as Uri;
                        System.Diagnostics.Process.Start(u.ToString());
                    };
                    wp.Children.Add(link);
                    linkIndex++;
                } else {
                    if (j != (ttt.Count - 1))
                        ttt[j] += ' ';


                    if (ttt[j].StartsWith(":s:")) {
                        CreateSmile(renderMessage.Data.Name, ttt[j], wp);
                    } else {
                        wp.Children.Add(new TextBlock() { Text = ttt[j], Style = s });
                    }
                }
            }

            renderMessage.Text = wp;
            renderMessage.Text.Measure(new Size(wp.MaxWidth, double.PositiveInfinity));
            renderMessage.Text.Arrange(new Rect(0, 0, renderMessage.Text.DesiredSize.Width, renderMessage.Text.DesiredSize.Height));

            if (ClassicView) {
                Canvas.SetLeft(renderMessage.Text, 5);
            } else {
                Canvas.SetLeft(renderMessage.Text, NameWidth + 10);
            }
            Canvas.SetTop(renderMessage.Text, AnimCanvas.ActualHeight);

            renderMessage.Height = renderMessage.Text.ActualHeight;
            renderMessage.State = 1;

            renderMessage.DestHeight = AnimCanvas.ActualHeight;

            if( renderMessage.Name != null )
                AnimCanvas.Children.Add(renderMessage.Name);

            AnimCanvas.Children.Add(renderMessage.Text);
        }

        private void CreateSmile( string UserName, string SmileId, WrapPanel TextPanel ) {
            int nn = SmileId.LastIndexOf(':');
            SmileId = SmileId.Substring(2, nn + 1 - 2) + " ";


            if (SmileId == ":peka: ") // use SPACE at last LUKE!
                PekaCount++;





            if (!HideSmiles) {
                TextPanel.Children.Add(SmilesDataDase.GetSmile(SmileId));
            } else {
                TextBlock smile = new TextBlock() { Text = SmileId.Substring(0, SmileId.Length-1), Style = TextSmileStyle };
                TextPanel.Children.Add(smile);
            }
        }
    
        public class RenderMessage {
            public Message Data { get; set; }

            public FrameworkElement Name { get; set; }
            public FrameworkElement Text { get; set; }



            double _calcedH;
            public double Height 
            {
                get {
                    if (Text == null)
                        return _calcedH;
                    if( double.IsNaN( Text.ActualHeight ) || double.IsInfinity( Text.ActualHeight ) )
                        return _calcedH;
                    return Text.ActualHeight;
                }
                set { _calcedH = value; }
            }

            public int State { get; set; }

            public double DestHeight { get; set; }

            public int Live { get; set; }

            public string TalkTo { get; set; }
        }

        public RenderMessage GetVisual( Message newOwner ) {
            for (int j = 0; j < RenderMessages.Count; ++j)
                if (RenderMessages[j].Data.Id == newOwner.Id)
                    return RenderMessages[j];
            return null;
        }

        void LoadChat( int ChannelId ) {
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(( a, b ) => {
                if (b.Error == null) {
                    Messages messages = JsonConvert.DeserializeObject<Messages>(b.Result);
                    if( messages != null )
                        UpdateMessages(messages);
                }

                next.Start();
            });
            wc.DownloadStringAsync(new Uri("http://chat.sc2tv.ru/memfs/channel-" + ChannelId + ".json"));
        }

        private void UpdateMessages( Messages msgs ) {
            for (int j = 0; j < msgs.Content.Length; ++j) {
                RenderMessage rm = GetVisual(msgs.Content[j]);
                if (rm == null) {
                    rm = new RenderMessage() { Data = msgs.Content[j], State = 0 };
                    RenderMessages.Add(rm);
                }
                rm.Live = 100; // Фантазии извращенца.
            }
        }

        private void Thumb_DragDelta_1( object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e ) {
            this.Left += e.HorizontalChange;
            this.Top += e.VerticalChange;
        }

        private void Button_Click_1( object sender, RoutedEventArgs e ) {
            OptionsForm fid = new OptionsForm();
            fid.ShowDialog();
        }

        private void Button_Click_2( object sender, RoutedEventArgs e ) {
            Close();
        }

        private void Thumb_MouseEnter_1( object sender, MouseEventArgs e ) {
            headerButtons.Visibility = System.Windows.Visibility.Visible;
        }

        private void Thumb_MouseLeave_1( object sender, MouseEventArgs e ) {
            headerButtons.Visibility = System.Windows.Visibility.Hidden;
        }

        private void b0_Click_1( object sender, RoutedEventArgs e ) {
            //if (CurrentPoling != null) {
            //    PollGrid.Children.Clear();
            //    PollGrid.RowDefinitions.Clear();

            //    TextBlock header = new TextBlock() {
            //        Text = "Выиграл вариант: " + CurrentPoling.Win, 
            //        Style = (Style)this.Resources["PollResultStyle"] };
            //    PollGrid.Children.Add(header);
            //    Grid.SetColumnSpan(header, 2);
            //    CurrentPoling = null;
            //    return;
            //}

            //if (PollBorder.Visibility == System.Windows.Visibility.Visible) {
            //    PollBorder.Visibility = System.Windows.Visibility.Hidden;
            //    return;
            //}
            

            //PollingForm pf = new PollingForm();
            //var r = pf.ShowDialog();

            //if (r.HasValue && r.Value) {
            //    PollBorder.Visibility = System.Windows.Visibility.Visible;
 

            //    CurrentPoling = new Polling(pf, Graphs);
            //} else {
            //    CurrentPoling = null;
            //    PollBorder.Visibility = System.Windows.Visibility.Hidden;
            //}
        }

        private void Window_SizeChanged_1( object sender, SizeChangedEventArgs e ) {
          

           // if (e.WidthChanged) {
                

                for (int j = 0; j < AnimCanvas.Children.Count; ++j) {
                    WrapPanel wp = AnimCanvas.Children[j] as WrapPanel;
                    if (wp != null) {
                        wp.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        wp.Arrange(new Rect(0, 0, wp.DesiredSize.Width, wp.DesiredSize.Height));

                        wp.MaxWidth = MaximumMessageWidth;
                    }
                }
            //}


            for (int j = 0; j < RenderMessages.Count; ++j) {
                if (RenderMessages[j].Text != null) {
                    RenderMessages[j].State = 1;
                    RenderMessages[j].Height = RenderMessages[j].Text.ActualHeight;

                } else {
                    RenderMessages[j].State = 0;
                }
            }
        }
    }
}
