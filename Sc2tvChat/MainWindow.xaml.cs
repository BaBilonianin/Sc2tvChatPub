using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Threading;

namespace Sc2tvChat {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            PollBorder.Visibility = System.Windows.Visibility.Hidden;
        }

        int PekaCount = 0;
        double TempCount = 0.0;

        DispatcherTimer render, next;

        private void Window_Loaded_1( object sender, RoutedEventArgs e ) {
          //  ;
            //<img src="/img/a.png?1" title=":happy:" width="30" height="30" class="chat-smile" alt=":happy:">
            SmilesUri[":happy: "] = new Uri("http://chat.sc2tv.ru/img/a.png?1");
            SmilesUri[":aws: "] = new Uri("http://chat.sc2tv.ru/img/awesome.png?1");
            SmilesUri[":nc: "] = new Uri("http://chat.sc2tv.ru/img/nocomments.png?1");
            SmilesUri[":manul: "] = new Uri("http://chat.sc2tv.ru/img/manul.png?1");
            SmilesUri[":crazy: "] = new Uri("http://chat.sc2tv.ru/img/crazy.png?1");
            SmilesUri[":cry: "] = new Uri("http://chat.sc2tv.ru/img/cry.png?1");
            SmilesUri[":glory: "] = new Uri("http://chat.sc2tv.ru/img/glory.png?1");
            SmilesUri[":kawai: "] = new Uri("http://chat.sc2tv.ru/img/kawai.png?1");
            SmilesUri[":mee: "] = new Uri("http://chat.sc2tv.ru/img/mee.png?1");
            SmilesUri[":omg: "] = new Uri("http://chat.sc2tv.ru/img/omg.png?1");
            SmilesUri[":whut: "] = new Uri("http://chat.sc2tv.ru/img/mhu.png?1");
            SmilesUri[":sad: "] = new Uri("http://chat.sc2tv.ru/img/sad.png?1");
            SmilesUri[":spk: "] = new Uri("http://chat.sc2tv.ru/img/slowpoke.png?1");
            SmilesUri[":hmhm: "] = new Uri("http://chat.sc2tv.ru/img/2.png?1");
            SmilesUri[":mad: "] = new Uri("http://chat.sc2tv.ru/img/mad.png?1");
            SmilesUri[":angry: "] = new Uri("http://chat.sc2tv.ru/img/aangry.png?1");
            SmilesUri[":xd: "] = new Uri("http://chat.sc2tv.ru/img/ii.png?1");
            SmilesUri[":huh: "] = new Uri("http://chat.sc2tv.ru/img/huh.png?1");
            SmilesUri[":tears: "] = new Uri("http://chat.sc2tv.ru/img/happycry.png?1");
            SmilesUri[":notch: "] = new Uri("http://chat.sc2tv.ru/img/notch.png?1");
            SmilesUri[":vaga: "] = new Uri("http://chat.sc2tv.ru/img/vaganych.png?1");
            SmilesUri[":ra: "] = new Uri("http://chat.sc2tv.ru/img/ra.png?1");
            SmilesUri[":fp: "] = new Uri("http://chat.sc2tv.ru/img/facepalm.png?1");
            SmilesUri[":neo: "] = new Uri("http://chat.sc2tv.ru/img/smith.png?1");
            SmilesUri[":peka: "] = new Uri("http://chat.sc2tv.ru/img/mini-happy.png?3");
            SmilesUri[":trf: "] = new Uri("http://chat.sc2tv.ru/img/trollface.png?2");
            SmilesUri[":fu: "] = new Uri("http://chat.sc2tv.ru/img/fuuuu.png?3");
            SmilesUri[":why: "] = new Uri("http://chat.sc2tv.ru/img/why.png?1");
            SmilesUri[":yao: "] = new Uri("http://chat.sc2tv.ru/img/yao.png?1");
            SmilesUri[":fyeah: "] = new Uri("http://chat.sc2tv.ru/img/fyeah.png?1");
            SmilesUri[":lucky: "] = new Uri("http://chat.sc2tv.ru/img/lol.png?3");
            SmilesUri[":okay: "] = new Uri("http://chat.sc2tv.ru/img/okay.png?2");
            SmilesUri[":alone: "] = new Uri("http://chat.sc2tv.ru/img/alone.png?2");
            SmilesUri[":joyful: "] = new Uri("http://chat.sc2tv.ru/img/ewbte.png?3");
            SmilesUri[":wtf: "] = new Uri("http://chat.sc2tv.ru/img/wtf.png?1");
            SmilesUri[":danu: "] = new Uri("http://chat.sc2tv.ru/img/daladno.png?1");
            SmilesUri[":gusta: "] = new Uri("http://chat.sc2tv.ru/img/megusta.png?1");
            SmilesUri[":bm: "] = new Uri("http://chat.sc2tv.ru/img/bm.png?4");
            SmilesUri[":lol: "] = new Uri("http://chat.sc2tv.ru/img/loool.png?1");
            SmilesUri[":notbad: "] = new Uri("http://chat.sc2tv.ru/img/notbad.png?1");
            SmilesUri[":rly: "] = new Uri("http://chat.sc2tv.ru/img/really.png?1");
            SmilesUri[":ban: "] = new Uri("http://chat.sc2tv.ru/img/banan.png?1");
            SmilesUri[":cap: "] = new Uri("http://chat.sc2tv.ru/img/cap.png?1");
            SmilesUri[":br: "] = new Uri("http://chat.sc2tv.ru/img/br.png?1");
            SmilesUri[":fpl: "] = new Uri("http://chat.sc2tv.ru/img/leefacepalm.png?1");
            SmilesUri[":ht: "] = new Uri("http://chat.sc2tv.ru/img/heart.png?1");
            SmilesUri[":adolf: "] = new Uri("http://chat.sc2tv.ru/img/adolf.png?2");
            SmilesUri[":bratok: "] = new Uri("http://chat.sc2tv.ru/img/bratok.png?1");
            SmilesUri[":strelok: "] = new Uri("http://chat.sc2tv.ru/img/strelok.png?1");
            SmilesUri[":white-ra: "] = new Uri("http://chat.sc2tv.ru/img/white-ra.png?1");
            SmilesUri[":dimaga: "] = new Uri("http://chat.sc2tv.ru/img/dimaga.png?1");
            SmilesUri[":bruce: "] = new Uri("http://chat.sc2tv.ru/img/bruce.png?1");
            SmilesUri[":jae: "] = new Uri("http://chat.sc2tv.ru/img/jaedong.png?1");
            SmilesUri[":flash: "] = new Uri("http://chat.sc2tv.ru/img/flash1.png?1");
            SmilesUri[":bisu: "] = new Uri("http://chat.sc2tv.ru/img/bisu.png?1");
            SmilesUri[":jangbi: "] = new Uri("http://chat.sc2tv.ru/img/jangbi.png?1");
            SmilesUri[":idra: "] = new Uri("http://chat.sc2tv.ru/img/idra.png?1");
            SmilesUri[":vdv: "] = new Uri("http://chat.sc2tv.ru/img/vitya.png?1");
            SmilesUri[":imba: "] = new Uri("http://chat.sc2tv.ru/img/djigurda.png?1");
            SmilesUri[":chuck: "] = new Uri("http://chat.sc2tv.ru/img/chan.png?1");
            SmilesUri[":tgirl: "] = new Uri("http://chat.sc2tv.ru/img/brucelove.png?1");
            SmilesUri[":top1sng: "] = new Uri("http://chat.sc2tv.ru/img/happy.png?1");
            SmilesUri[":slavik: "] = new Uri("http://chat.sc2tv.ru/img/slavik.png?1");
            SmilesUri[":olsilove: "] = new Uri("http://chat.sc2tv.ru/img/olsilove.png?1");
            SmilesUri[":kas: "] = new Uri("http://chat.sc2tv.ru/img/kas.png?1");
            SmilesUri[":pool: "] = new Uri("http://chat.sc2tv.ru/img/pool.png?1");
            SmilesUri[":ej: "] = new Uri("http://chat.sc2tv.ru/img/ejik.png?1");
            SmilesUri[":mario: "] = new Uri("http://chat.sc2tv.ru/img/mario.png?1");
            SmilesUri[":tort: "] = new Uri("http://chat.sc2tv.ru/img/tort.png?1");
            SmilesUri[":arni: "] = new Uri("http://chat.sc2tv.ru/img/terminator.png?1");
            SmilesUri[":crab: "] = new Uri("http://chat.sc2tv.ru/img/crab.png?1");
            SmilesUri[":hero: "] = new Uri("http://chat.sc2tv.ru/img/heroes3.png?1");
            SmilesUri[":mc: "] = new Uri("http://chat.sc2tv.ru/img/mine.png?1");
            SmilesUri[":osu: "] = new Uri("http://chat.sc2tv.ru/img/osu.png?1");
            SmilesUri[":q3: "] = new Uri("http://chat.sc2tv.ru/img/q3.png?1");
            SmilesUri[":tigra: "] = new Uri("http://chat.sc2tv.ru/img/tigrica.png?1");
            SmilesUri[":volck: "] = new Uri("http://chat.sc2tv.ru/img/voOlchik1.png?1");
            SmilesUri[":hpeka: "] = new Uri("http://chat.sc2tv.ru/img/harupeka.png?1");
            SmilesUri[":slow: "] = new Uri("http://chat.sc2tv.ru/img/spok.png?1");
            SmilesUri[":alex: "] = new Uri("http://chat.sc2tv.ru/img/alfi.png?1");
            SmilesUri[":panda: "] = new Uri("http://chat.sc2tv.ru/img/panda.png?1");
            SmilesUri[":sun: "] = new Uri("http://chat.sc2tv.ru/img/sunl.png?1");
            SmilesUri[":cou: "] = new Uri("http://chat.sc2tv.ru/img/cougar.png?2");
            SmilesUri[":wb: "] = new Uri("http://chat.sc2tv.ru/img/wormban.png?1");
            SmilesUri[":dobro: "] = new Uri("http://chat.sc2tv.ru/img/dobre.png?1");
            SmilesUri[":theweedle: "] = new Uri("http://chat.sc2tv.ru/img/weedle.png?1");
            SmilesUri[":apc: "] = new Uri("http://chat.sc2tv.ru/img/apochai.png?1");
            SmilesUri[":globus: "] = new Uri("http://chat.sc2tv.ru/img/globus.png?1");
            SmilesUri[":cow: "] = new Uri("http://chat.sc2tv.ru/img/cow.png?1");
            SmilesUri[":nook: "] = new Uri("http://chat.sc2tv.ru/img/no-okay.png?1");
            SmilesUri[":noj: "] = new Uri("http://chat.sc2tv.ru/img/knife.png?1");
            SmilesUri[":fpd: "] = new Uri("http://chat.sc2tv.ru/img/fp.png?1");
            SmilesUri[":hg: "] = new Uri("http://chat.sc2tv.ru/img/hg.png?1");
            SmilesUri[":yoko: "] = new Uri("http://chat.sc2tv.ru/img/yoko.png?1");
            SmilesUri[":miku: "] = new Uri("http://chat.sc2tv.ru/img/miku.png?1");
            SmilesUri[":winry: "] = new Uri("http://chat.sc2tv.ru/img/winry.png?1");
            SmilesUri[":asuka: "] = new Uri("http://chat.sc2tv.ru/img/asuka.png?1");
            SmilesUri[":konata: "] = new Uri("http://chat.sc2tv.ru/img/konata.png?1");
            SmilesUri[":reimu: "] = new Uri("http://chat.sc2tv.ru/img/reimu.png?1");
            SmilesUri[":sex: "] = new Uri("http://chat.sc2tv.ru/img/sex.png?1");
            SmilesUri[":mimo: "] = new Uri("http://chat.sc2tv.ru/img/mimo.png?1");
            SmilesUri[":fire: "] = new Uri("http://chat.sc2tv.ru/img/fire.png?1");
            SmilesUri[":mandarin: "] = new Uri("http://chat.sc2tv.ru/img/mandarin.png?1");


            //string s = "";
            //string ssss = File.ReadAllText( "x:\\smiles.txt" );
            //Regex rx = new Regex("\\<img.*?src.*?\\\"(.*?)\\\".*?title.*?\\\"(.*?)\\\".*?\\>");
            //foreach (Match m in rx.Matches(ssss)) {
            //    s += string.Format("SmilesUri[\"{1} \"] = new Uri(\"http://chat.sc2tv.ru{0}\");\r\n", m.Groups[1].Value, m.Groups[2].Value);
            //}

            RenderMessages = new List<RenderMessage>();
            render = new DispatcherTimer();
            render.Interval = TimeSpan.FromMilliseconds(50);
            render.Tick += render_Tick;
            render.Start();

            next = new DispatcherTimer();
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

            ClockTB.Text = string.Format("Time: {0:HH:mm:ss}, Peka's: {1}, Temp: {2:0.0}°C", DateTime.Now, PekaCount, TempCount);

            if (TempCount > 50)
                TempCount -= 0.01;
            else
                TempCount -= 0.001;

            double y = AnimCanvas.ActualHeight-5;
          //  bool needScroll = false;
            for (int j = RenderMessages.Count - 1; j >= (RenderMessages.Count-15); --j) {

                if( j>=0 )
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
                        //    needScroll = true;
                        break;
                    case 2:
                        //RenderMessages[j].DestHeight = -RenderMessages[j].Height - 20.0;
                        //if (Math.Abs(Canvas.GetTop(RenderMessages[j].Text) - RenderMessages[j].DestHeight) < 0.1) {
                        if( RenderMessages[j].Name != null )
                            AnimCanvas.Children.Remove(RenderMessages[j].Name);
                        if (RenderMessages[j].Text != null)
                            AnimCanvas.Children.Remove(RenderMessages[j].Text);
                            RenderMessages[j].State = 3;
                        //}
                        break;

                    case 3:
                        RenderMessages[j].Live--;
                        break;
                }
            }

            //if (needScroll && RenderMessages.Count > 0) {
            //    for (int j = 0; j < RenderMessages.Count; ++j)
            //        if (RenderMessages[j].State == 1) {
            //            RenderMessages[j].State = 2;
            //            break;
            //        }
            //}

            int i = 0;
            while (i < RenderMessages.Count) {
                if (RenderMessages[i].Live < 0) {
                    RenderMessages.RemoveAt(i);
                } else {
                    i++;
                }
            }


            for (int j = 0; j < RenderMessages.Count; ++j) {
                if (RenderMessages[j].State < 3) {
                    if (RenderMessages[j].Text == null) {
                        RenderMessages[j].State = 3;
                    } else {
                        double oy = Canvas.GetTop(RenderMessages[j].Text);

                        double dy = (RenderMessages[j].DestHeight - oy) / 10.0;

                        if (Convert.ToInt32(dy) != 0) {

                            Canvas.SetTop(RenderMessages[j].Text, oy + dy);
                            Canvas.SetTop(RenderMessages[j].Name, oy + dy);
                        }
                    }
                }
            }


        }

        Regex UriDetector = new Regex(@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)" );

        private void CreateVisual( RenderMessage renderMessage ) {
            const double NameWidth = 115.0;
            const string LinkReplacer = "%LINKLINK%";

            renderMessage.Name = new TextBlock() { Text = renderMessage.Owner.Name };
            renderMessage.Name.Style = (Style)this.Resources["NameStyle"];
            renderMessage.Name.Measure(new Size(200.0, double.PositiveInfinity));
            renderMessage.Name.Arrange(new Rect(0, 0, renderMessage.Name.DesiredSize.Width, renderMessage.Name.DesiredSize.Height));

            Canvas.SetLeft(renderMessage.Name, NameWidth - renderMessage.Name.ActualWidth);
            Canvas.SetTop(renderMessage.Name, 200.0);


            Style textStyle = (Style)this.Resources["TextStyle"];
            Style nameTextStyle = (Style)this.Resources["NameTextStyle"];
            
            // parse text
            WrapPanel wp = new WrapPanel() { 
                MaxWidth = this.ActualWidth - NameWidth - 10, 
                Orientation = System.Windows.Controls.Orientation.Horizontal };
            List<string> ttt = new List<string>();
            renderMessage.Owner.Text =
                renderMessage.Owner.Text.Replace("&quot;", "\"").Replace(":s:", " :s:").Replace("  ", " ");

            List<Uri> Urls = new List<Uri>();
         

            renderMessage.Owner.Text = UriDetector.Replace(renderMessage.Owner.Text, new MatchEvaluator(( m ) => {
                Urls.Add(new Uri(m.Value, UriKind.RelativeOrAbsolute));
                return LinkReplacer;
            }));

            if (CurrentPoling != null) {
                CurrentPoling.RegisterVote(renderMessage.Owner.Name, renderMessage.Owner.Text);
                UpdateVisualGraphs(CurrentPoling);
            }

            int nxd = renderMessage.Owner.Text.IndexOf("<b>");
            if (nxd >= 0) {
                int nxd2 = renderMessage.Owner.Text.IndexOf("</b>");
                renderMessage.TalkTo = renderMessage.Owner.Text.Substring(nxd + 3, nxd2 - nxd - 3);
                renderMessage.Owner.Text = renderMessage.Owner.Text.Substring(nxd2 + 6);
               
                if (renderMessage.TalkTo== Properties.Settings.Default.streamerNick) {
                    wp.Style = (Style)this.Resources["StreamerText"];
                }

                renderMessage.TalkTo = renderMessage.TalkTo + ", ";
                ttt.Add(renderMessage.TalkTo);

            } else {
                renderMessage.TalkTo = "";
            }
                        
            ttt.AddRange(from b in renderMessage.Owner.Text.Split(' ')
                         select b + " ");

            // check smiles inside
            int linkIndex = 0;

            for (int j = 0; j < ttt.Count; ++j) {
                Style s;
                if (j == 0 && !string.IsNullOrEmpty(renderMessage.TalkTo))
                    s = nameTextStyle;
                else
                    s = textStyle;

                if (ttt[j] == LinkReplacer+" ") {
                    TextBlock link = new TextBlock() { 
                        Text = "link ", 
                        Cursor = Cursors.Hand,
                        Style = (Style)this.Resources["LinkStyle"], 
                        ToolTip = Urls[linkIndex] ,
                        Tag = Urls[linkIndex]
                    };
                    link.MouseLeftButtonUp += link_MouseLeftButtonUp;
                    wp.Children.Add(link);
                    linkIndex++;
                }else
                if (ttt[j].StartsWith(":s:")) {
                    CreateSmile(renderMessage.Owner.Name, ttt[j], wp);
                } else {
                    wp.Children.Add(new TextBlock() { Text = ttt[j], Style = s });
                }
            }

            //string t  = ;

            renderMessage.Text = wp;// new ;
           // renderMessage.Text.Style = ;
            renderMessage.Text.Measure(new Size((this.ActualWidth-NameWidth-5), double.PositiveInfinity));
            renderMessage.Text.Arrange(new Rect(0, 0, renderMessage.Text.DesiredSize.Width, renderMessage.Text.DesiredSize.Height));
            Canvas.SetLeft(renderMessage.Text, NameWidth+10);
            Canvas.SetTop(renderMessage.Text, 200.0);

            renderMessage.Height = renderMessage.Text.ActualHeight + 2.0;
            renderMessage.State = 1;

            renderMessage.DestHeight = 200.0;

            AnimCanvas.Children.Add(renderMessage.Name);
            AnimCanvas.Children.Add(renderMessage.Text);
        }

        void link_MouseLeftButtonUp( object sender, MouseButtonEventArgs e ) {
            Uri u = ((TextBlock)sender).Tag as Uri;
            System.Diagnostics.Process.Start(u.ToString());
        }

        private void UpdateVisualGraphs( Polling CurrentPoling ) {
            if (PollGrid.ColumnDefinitions[1].ActualWidth > 0.0) {
                for (int j = 0; j < CurrentPoling.Variants.Count; ++j) {
                    CurrentPoling.VisualGraphs[j].Width =
                        CurrentPoling.Graphs[j] * PollGrid.ColumnDefinitions[1].ActualWidth;
                }
            }
        }

        Dictionary<string, Uri> SmilesUri = new Dictionary<string, Uri>();
        Dictionary<string, BitmapImage> SmilesBmp = new Dictionary<string, BitmapImage>();

        private void CreateSmile( string UserName, string SmileId, WrapPanel TextPanel ) {
            Image img = new Image() { Height = 24.0 };
            BitmapImage bi;

            int nn = SmileId.LastIndexOf(':');
            SmileId = SmileId.Substring(2, nn + 1 - 2) + " ";

            
            if (SmilesBmp.TryGetValue(SmileId, out bi)) {
                img.Source = bi;
            } else {
                bi = new BitmapImage(SmilesUri[SmileId]);
                img.Source = bi;
                SmilesBmp[SmileId] = bi;
            }

           
            UpdateAchivments(UserName, SmileId);

            TextPanel.Children.Add(img);
        }

        private void UpdateAchivments( string UserName, string SmileId ) {
            Achivment a;
            if (Achivments.TryGetValue(UserName, out a)) {
            } else {
                a = new Achivment();
                Achivments[UserName] = a;
            }

            if (SmileId == ":peka: ")
                PekaCount++;

            if (SmileId == ":fire: ")
                TempCount+= 5.0;

            a.Update(SmileId);
        }

        public class Message {
            [JsonProperty(PropertyName = "channelId")]
            public int ChannelId { get; set; }

            [JsonProperty(PropertyName = "date")]
            public DateTime Date { get; set; }

            [JsonProperty(PropertyName = "id")]
            public int Id { get; set; }

            [JsonProperty(PropertyName = "uid")]
            public int Uid { get; set; }

            [JsonProperty(PropertyName = "rid")]
            public int Rid { get; set; }

            [JsonProperty(PropertyName = "message")]
            public string Text { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }
        }

        public class Messages {
            [JsonProperty(PropertyName = "messages")]
            public Message[] Content { get; set; }
        }

        public class RenderMessage {
            public Message Owner { get; set; }

            public FrameworkElement Name { get; set; }
            public FrameworkElement Text { get; set; }

            public double Height { get; set; }
            public int State { get; set; }

            public double DestHeight { get; set; }

            public int Live { get; set; }

            public string TalkTo { get; set; }
        }

        public class Achivment {
            public int PekaCount { get; set; }
            public void Update( string SmileId ) {
                if( SmileId == ":peka:" )
                    PekaCount++;
            }
        }

        Dictionary<string, Achivment> Achivments = new Dictionary<string, Achivment>();
        List<RenderMessage> RenderMessages;

        public RenderMessage GetVisual( Message newOwner ) {
            for (int j = 0; j < RenderMessages.Count; ++j)
                if (RenderMessages[j].Owner.Id == newOwner.Id)
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
            //wc.Headers.Add("Cookie", "chat_channel_id=50463; chat-img=1;");
            wc.DownloadStringAsync(new Uri("http://chat.sc2tv.ru/memfs/channel-" + ChannelId + ".json"));
        }

        private void UpdateMessages( Messages messages ) {

            var msgs = (from b in messages.Content
                        orderby b.Date
                        select b).ToArray();

            for (int j = 0; j < msgs.Length; ++j) {
                RenderMessage rm = GetVisual(msgs[j]);
                if (rm == null) {
                    rm = new RenderMessage() { Owner = msgs[j], State = 0 };
                    RenderMessages.Add(rm);
                }
                rm.Live = 100;
            }
        }

        private void Thumb_DragDelta_1( object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e ) {
            this.Left += e.HorizontalChange;
            this.Top += e.VerticalChange;
        }

        private void Button_Click_1( object sender, RoutedEventArgs e ) {
            this.Topmost = false;
            FindIDForm fid = new FindIDForm();
            var r = fid.ShowDialog();
            this.Topmost = true;
            if (r.HasValue && r.Value) {
                PekaCount = 0;
                TempCount = 0;
                RenderMessages.Clear();
                Achivments.Clear();
                AnimCanvas.Children.Clear();

                CurrentPoling = null;
                PollGrid.Children.Clear();
                PollGrid.RowDefinitions.Clear();
                PollBorder.Visibility = System.Windows.Visibility.Hidden;

                Properties.Settings.Default.Save();
            }
        }

        private void Button_Click_2( object sender, RoutedEventArgs e ) {
            Properties.Settings.Default.Save();
            Close();
        }

        private void Thumb_MouseEnter_1( object sender, MouseEventArgs e ) {
            b0.Visibility = b1.Visibility = b2.Visibility = System.Windows.Visibility.Visible;
        }

        private void Thumb_MouseLeave_1( object sender, MouseEventArgs e ) {
            b0.Visibility = b1.Visibility = b2.Visibility = System.Windows.Visibility.Hidden;
        }

        private void b0_Click_1( object sender, RoutedEventArgs e ) {
            if (CurrentPoling != null) {
                PollGrid.Children.Clear();
                PollGrid.RowDefinitions.Clear();

                TextBlock header = new TextBlock() {
                    Text = "Выиграл вариант: " + CurrentPoling.Win, 
                    Style = (Style)this.Resources["PollResultStyle"] };
                PollGrid.Children.Add(header);
                Grid.SetColumnSpan(header, 2);
                CurrentPoling = null;
                return;
            }

            if (PollBorder.Visibility == System.Windows.Visibility.Visible) {
                PollBorder.Visibility = System.Windows.Visibility.Hidden;
                return;
            }
            

            PollingForm pf = new PollingForm();
            var r = pf.ShowDialog();

            if (r.HasValue && r.Value) {
                PollBorder.Visibility = System.Windows.Visibility.Visible;
                PollGrid.Children.Clear();
                PollGrid.RowDefinitions.Clear();
                // Add header
                PollGrid.RowDefinitions.Add(new RowDefinition());
                TextBlock header = new TextBlock() { Text = "Идет голосование!", Style = (Style)this.Resources["PollHeaderStyle"] };
                PollGrid.Children.Add(header);
                Grid.SetColumnSpan(header, 2);

                List<Border> Graphs = new List<Border>();

                for (int j = 0; j < pf.Variants.Count; ++j) {
                    PollGrid.RowDefinitions.Add(new RowDefinition());

                    TextBlock line = new TextBlock() { Text = (j+1) + ". " + pf.Variants[j], Style = (Style)this.Resources["PollVariantStyle"] };
                    PollGrid.Children.Add(line);
                    Grid.SetRow(line, j + 1);

                    Border graph = new Border() { Style = (Style)this.Resources["PollGraphStyle"] };
                    PollGrid.Children.Add(graph);
                    Grid.SetRow(graph, j + 1);
                    Grid.SetColumn(graph, 1);
                    Graphs.Add(graph);
                }

                CurrentPoling = new Polling(pf, Graphs);
            } else {
                CurrentPoling = null;
                PollBorder.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        Polling CurrentPoling;

        class Polling {
            Dictionary<string, int> Selections = new Dictionary<string, int>();

            public List<string> Variants { get; set; }
            public List<Double> Graphs { get; set; }
            public List<Border> VisualGraphs { get; set; }

            public string Win {
                get {
                    double max = -1000;
                    int i = -1;
                    for (int j = 0; j < Graphs.Count; ++j)
                        if (max < Graphs[j]) {
                            max = Graphs[j];
                            i = j;
                        }

                    return Variants[i];
                }
            }

            public Polling( PollingForm pf, List<Border> Graphs ) {
                Variants = pf.Variants;
                this.Graphs = new List<double>();
                foreach (var v in Variants)
                    this.Graphs.Add(0.0);
                VisualGraphs = Graphs;
            }

            public void RegisterVote( string User, string Text ) {
                int vote = 0;
                if (Text.Contains("1."))
                    vote = 1;
                else
                if (Text.Contains("2."))
                    vote = 2;
                else
                if (Text.Contains("3."))
                    vote = 3;
                else
                if (Text.Contains("4."))
                    vote = 4;

                if (vote > Variants.Count || vote == 0)
                    return;

                Selections[User] = vote-1;

                UpdateGraphs();
            }

            private void UpdateGraphs() {
                List<Double> sels = new List<double>();
                foreach( var v in Variants )
                    sels.Add( 0.0 );

                if (Selections.Count == 0)
                    return;

                foreach (var v in Selections)
                    sels[v.Value] = sels[v.Value] + 1;

                double max = 0.0;

                for (int j = 0; j < sels.Count; ++j)
                    if (max < sels[j])
                        max = sels[j];

                for (int j = 0; j < sels.Count; ++j)
                    Graphs[j] = sels[j] / max;
            }
        }

    }
}
