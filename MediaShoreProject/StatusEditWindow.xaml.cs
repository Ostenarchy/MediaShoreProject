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
    /// Interaction logic for StatusEditWindow.xaml
    /// </summary>
    public partial class StatusEditWindow : Window
    {
        public Statuses SelectedStatus { get; set; }
        public StatusEditWindow(Statuses currentStatus)
        {
            InitializeComponent();
            // Загружаем все доступные статусы из базы
            cbStatus.ItemsSource = App.db.Statuses.ToList();
            // Устанавливаем текущий статус заказа как выбранный по умолчанию
            cbStatus.SelectedItem = App.db.Statuses.FirstOrDefault(s => s.id == currentStatus.id);
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cbStatus.SelectedItem is Statuses status)
            {
                SelectedStatus = status;
                this.DialogResult = true;
            }
        }
    }
}
