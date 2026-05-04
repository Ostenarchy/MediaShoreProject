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

namespace MediaShoreProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            
        }

        private void btnGuest_Click(object sender, RoutedEventArgs e)
        {
            Users guestUser = new Users();
            guestUser.userBalance = 0;
            guestUser.roleID = 3; // typical user lol
            guestUser.userFirstName = "Гость";
            guestUser.userLastName = "";
            WorkWindow ww = new WorkWindow(guestUser);
            ww.Show();
            this.Close();
        }

        private void btnReg_Click(object sender, RoutedEventArgs e)
        {
            NewUserWindow nuw = new NewUserWindow();
            nuw.ShowDialog();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if(tbLogin.Text.Length == 0 || pbPassword.Password.Length == 0)
            {
                MessageBox.Show("Пожалуйста заполните все поля!");
                return;
            }
            Users user = App.db.Users.FirstOrDefault(u => u.userName == tbLogin.Text && u.userPassword == pbPassword.Password);
            if(user != null)
            {
                WorkWindow ww = new WorkWindow(user);
                ww.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!");
            }
        }
    }
}
