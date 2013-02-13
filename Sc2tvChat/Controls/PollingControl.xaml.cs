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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RatChat.Controls {
    /// <summary>
    /// Interaction logic for PollingControl.xaml
    /// </summary>
    public partial class PollingControl : UserControl {
        /// <summary>
        /// Этот конструктор только для дизайнера
        /// </summary>
        public PollingControl() {
            InitializeComponent();
        }


        //bool AllowRegisterVote = false;
        ///// <summary>
        ///// Выборы чатлан.
        ///// В ключе - кто выбрал, в значении - что выбрал. 
        ///// </summary>
        //Dictionary<string, int> Selections = new Dictionary<string, int>();
        ///// <summary>
        ///// Варианты для вывода на экран.
        ///// </summary>
        //List<string> Variants;
        ///// <summary>
        ///// Нормализованные в периоде [0.0 - 1.0] варианты ответов.
        ///// </summary>
        //List<Double> Graphs;

        ///// <summary>
        ///// TODO: Варианты для текста чатлан (на текущий момент, 1., 2., 3., 4. )
        ///// </summary>
        ////List<string> Checks;

        ///// <summary>
        ///// Выбрать текущий вариант набравший максимальное кол-во голосов.
        ///// (да мне пох на LINQ)
        ///// </summary>
        //public string Win {
        //    get {
        //        double max = -1000;
        //        int i = -1;
        //        for (int j = 0; j < Graphs.Count; ++j)
        //            if (max < Graphs[j]) {
        //                max = Graphs[j];
        //                i = j;
        //            }

        //        return Variants[i];
        //    }
        //}

        ///// <summary>
        ///// Зарегестрировать голос чатланина.
        ///// TODO: Связку с Checks сделать надо.
        ///// </summary>
        ///// <param name="Data"></param>
        ////public void RegisterVote( Message Data ) {
        ////    if (AllowRegisterVote) {
        ////        int vote = 0;
        ////        if (Data.Text.Contains("1."))
        ////            vote = 1;
        ////        else
        ////            if (Data.Text.Contains("2."))
        ////                vote = 2;
        ////            else
        ////                if (Data.Text.Contains("3."))
        ////                    vote = 3;
        ////                else
        ////                    if (Data.Text.Contains("4."))
        ////                        vote = 4;

        ////        if (vote > Variants.Count || vote == 0)
        ////            return;

        ////        Selections[Data.Name] = vote - 1;

        ////        UpdateGraphs();
        ////    }
        ////}


        ///// <summary>
        ///// Начать голосование
        ///// TODO: Сделать выбор чатланика не банальный (1.) и т.п., а например :peka: для первого варианта, и т.п.
        ///// </summary>
        ///// <param name="Variants">Варианты выбора</param>
        //public void StartPoll( IEnumerable<string> Variants ) {


        //    Selections.Clear();

        //    this.Variants = new List<string>();
        //    this.Variants.AddRange(Variants);
        //    this.Graphs = new List<double>();
        //    for (int j = 0; j < this.Variants.Count; ++j)
        //        this.Graphs.Add(0.0);


        //    PollGrid.Children.Clear();
        //    PollGrid.RowDefinitions.Clear();
        //    //    // Add header
        //    PollGrid.RowDefinitions.Add(new RowDefinition());
        //    TextBlock header = new TextBlock() { 
        //        Text = "Идет голосование!", 
        //        Style = (Style)App.Current.Resources["PollHeaderStyle"] };
        //    PollGrid.Children.Add(header);
        //    Grid.SetColumnSpan(header, 2);

        //    List<Border> Graphs = new List<Border>();

        //    for (int j = 0; j < this.Variants.Count; ++j) {
        //        PollGrid.RowDefinitions.Add(new RowDefinition());

        //        TextBlock line = new TextBlock() { 
        //            Text = (j + 1) + ". " + this.Variants[j], 
        //            Style = (Style)App.Current.Resources["PollVariantStyle"] };
        //        PollGrid.Children.Add(line);
        //        Grid.SetRow(line, j + 1);

        //        Border graph = new Border() { Style = (Style)this.Resources["PollGraphStyle"] };
        //        PollGrid.Children.Add(graph);
        //        Grid.SetRow(graph, j + 1);
        //        Grid.SetColumn(graph, 1);
        //        Graphs.Add(graph);
        //    }

        //    AllowRegisterVote = true;
        //}

        //public void CancelPoll() {
        //    AllowRegisterVote = false;
        //}

        //public string FinishPoll() {
        //    AllowRegisterVote = false;
        //    return "fuck";
        //}

        ////private void UpdateVisualGraphs( Polling CurrentPoling ) {
        ////    if (PollGrid.ColumnDefinitions[1].ActualWidth > 0.0) {
        ////        for (int j = 0; j < CurrentPoling.Variants.Count; ++j) {
        ////            CurrentPoling.VisualGraphs[j].Width =
        ////                CurrentPoling.Graphs[j] * PollGrid.ColumnDefinitions[1].ActualWidth;
        ////        }
        ////    }
        ////}

          
        ///// <summary>
        ///// Хочу чтобы при доп. голосе была понтовая анимация.
        ///// </summary>
        //private void UpdateGraphs() {
        //    List<Double> sels = new List<double>();
        //    foreach (var v in Variants)
        //        sels.Add(0.0);

        //    if (Selections.Count == 0)
        //        return;

        //    foreach (var v in Selections)
        //        sels[v.Value] = sels[v.Value] + 1;

        //    double max = 0.0;

        //    for (int j = 0; j < sels.Count; ++j)
        //        if (max < sels[j])
        //            max = sels[j];

        //    for (int j = 0; j < sels.Count; ++j)
        //        Graphs[j] = sels[j] / max;

        //    // Проверить поток, и обновить график
        //}

      
    }
}
