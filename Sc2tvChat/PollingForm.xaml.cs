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

namespace Sc2tvChat {
    /// <summary>
    /// Interaction logic for PollingForm.xaml
    /// </summary>
    public partial class PollingForm : Window {
        public PollingForm() {
            InitializeComponent();
            Variants = new List<string>();;
        }

        public List<string> Variants { get; set; }

        private void Button_Click_1( object sender, RoutedEventArgs e ) {
            Variants.Clear();

            if (!string.IsNullOrEmpty(var1.Text.Trim()))
                Variants.Add(var1.Text.Trim());
            if (!string.IsNullOrEmpty(var2.Text.Trim()))
                Variants.Add(var2.Text.Trim());
            if (!string.IsNullOrEmpty(var3.Text.Trim()))
                Variants.Add(var3.Text.Trim());
            if (!string.IsNullOrEmpty(var4.Text.Trim()))
                Variants.Add(var4.Text.Trim());

            if (Variants.Count >= 2) {
                this.DialogResult = true;
            } else {
                MessageBox.Show("Мало вариантов!");
            }
        }
    }
}
