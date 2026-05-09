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

namespace MediaShoreProject
{
    /// <summary>
    /// Interaction logic for AdminEditWindow.xaml
    /// </summary>
    public partial class AdminEditWindow : Window
    {
        public string ResultText { get; set; }
        public AdminEditWindow(string title, string oldText = "")
        {
            InitializeComponent();
            lblTitle.Text = title;
            tbInput.Text = oldText;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbInput.Text))
            {
                ResultText = tbInput.Text;
                this.DialogResult = true;
            }
        }

    }
}
