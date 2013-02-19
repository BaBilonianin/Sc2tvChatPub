using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    /// Interaction logic for ChatsControl.xaml
    /// </summary>
    public partial class ChatsControl : UserControl {
        public ChatsControl() {
            InitializeComponent();
            this.DataContextChanged += ChatsControl_DataContextChanged;
        }

        INotifyCollectionChanged Collection;

        void ChatsControl_DataContextChanged( object sender, DependencyPropertyChangedEventArgs e ) {
            if (Collection != null) {
                Collection.CollectionChanged -= DataContextCollectionChanged;
                root.Children.Clear();
            }

            Collection = e.NewValue as INotifyCollectionChanged;

            if (Collection != null) {
                Collection.CollectionChanged += DataContextCollectionChanged;

                ICollection col = Collection as ICollection;
                SetRows(col.Count);
                UpdateOnReset(Collection as ICollection);
                AddSplitters();
            }
        }

        private void UpdateOnReset( ICollection collection ) {
            foreach (FrameworkElement fe in collection)
                if (fe != null)
                    root.Children.Add(fe);
        }

        void SetRows( int Count ) {
            if (root.RowDefinitions.Count > Count) {
                while (root.RowDefinitions.Count > Count)
                    root.RowDefinitions.RemoveAt(0);
            } else
                if (root.RowDefinitions.Count < Count) {
                    for (int j = 0; j < (Count - root.RowDefinitions.Count); ++j)
                        root.RowDefinitions.Add(new RowDefinition() {
                            Height = new GridLength(1, GridUnitType.Star),
                            MinHeight = 20.0
                        });
                }

            int i = 0;
            while (i < root.Children.Count)
                if (root.Children[i] is GridSplitter)
                    root.Children.RemoveAt(i);
                else
                    i++;

        }

        private void AddSplitters() {
            for (int j = 1; j < root.RowDefinitions.Count; ++j) {
                GridSplitter gs = new GridSplitter();
                root.Children.Add(gs);
                gs.SetResourceReference(GridSplitter.StyleProperty, "ChatSplitter");
                Grid.SetRow(gs, j);
            }
        }

        private void OrderItems( IList list ) {
            for (int j = 0; j < list.Count; ++j) {
                FrameworkElement fe = list[j] as FrameworkElement;
                Grid.SetRow(fe, j);
            }
        }

        void DataContextCollectionChanged( object sender, NotifyCollectionChangedEventArgs e ) {
            ICollection col = sender as ICollection;
            SetRows(col.Count);

            switch (e.Action) {
                case NotifyCollectionChangedAction.Replace:
                
                case NotifyCollectionChangedAction.Reset:
                    root.Children.Clear();
                    UpdateOnReset(Collection as ICollection);
                    AddSplitters();
                    return;

                case NotifyCollectionChangedAction.Add:
                    for (int j = 0; j < e.NewItems.Count; ++j) {
                        FrameworkElement fe = e.NewItems[j] as FrameworkElement;
                        if (fe != null)
                            root.Children.Add(fe);
                    }
                    break;

                case NotifyCollectionChangedAction.Move: // Тупо реордер
                    break;

                case NotifyCollectionChangedAction.Remove:
                    for (int j = 0; j < e.OldItems.Count; ++j) {
                        FrameworkElement fe = e.OldItems[j] as FrameworkElement;
                        if (fe != null)
                            root.Children.Remove(fe);
                    }
                    break;

            }
            
            IList list = sender as IList;
            OrderItems(list);
            AddSplitters();
        }

        public void SetChatHeightByIndex( int Index, double Height ) {
            if (Height < 20.0) Height = 20.0;

            if( root.RowDefinitions.Count > Index )
                root.RowDefinitions[Index].Height = new GridLength(Height, GridUnitType.Star);
        }

        public double GetChatHeightByIndex( int Index ) {
            return root.RowDefinitions[Index].Height.Value;
        }
      
    }
}
