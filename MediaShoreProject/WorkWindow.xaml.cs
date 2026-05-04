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
    /// Interaction logic for WorkWindow.xaml
    /// </summary>
    public partial class WorkWindow : Window
    {
        private Users currentUser;
        void RoleDetector(Users user) // определить верно айдишник чтобы не нулл лол эксепшены рот шатал
        {
            currentUser = user;
            textBlockUserFIO.Text = $"{currentUser.userFirstName} {currentUser.userLastName}";
            int? rId = currentUser.roleID; // Берем ID роли из текущего пользователя
            var currentRole = App.db.Roles.FirstOrDefault(r => r.id == rId);

            if (currentRole != null)
            {
                textBlockUserRole.Text = $"{currentRole.roleName}: ";
            }
        }

        void LoadDiscs() // дефолтный загрузчик данных
        {
            List<Discs> disc = App.db.Discs.ToList();
            lbDisc.ItemsSource = disc;
        }
        public WorkWindow(Users user)
        {
            InitializeComponent();
            RoleDetector(user);
            LoadDiscs();
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }
        private void btnAddToCart_Click(object sender, RoutedEventArgs e) // корзинка плюс
        {
            var button = sender as Button;
            var selectedDisc = button?.DataContext as Discs;


        }
    }
}
