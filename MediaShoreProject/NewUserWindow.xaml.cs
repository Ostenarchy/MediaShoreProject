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
    /// Interaction logic for NewUserWindow.xaml
    /// </summary>
    public partial class NewUserWindow : Window
    {
        public NewUserWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Users user = new Users();
            DateTime today = DateTime.Today;
            if (tbLogin.Text.Length > 0 && tbEmail.Text.Length > 0 && pbPassword.Password.Length > 0 && pbPasswordConfirmed.Password.Length > 0)
            {
                user.userName = tbLogin.Text; // *
                user.userEmail = tbEmail.Text; // *
                if (pbPassword.Password == pbPasswordConfirmed.Password)
                {
                    user.userPassword = pbPassword.Password; // *
                }
                else
                {
                    MessageBox.Show("Пароль и подтверждение должны совпадать!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Заполните обязательные (*) поля!");
                return;
            }
            user.userRegDate = today; // -
            user.roleID = 3; // -
            user.userBalance = 0; // -

            if(tbFirstName.Text.Trim().Length == 0)
            {
                user.userFirstName = "Anon";
            }
            else
            {
                user.userFirstName = tbFirstName.Text; // def
            }
            if (tbLastName.Text.Trim().Length == 0)
            {
                user.userLastName = "Person";
            }
            else
            {
                user.userLastName = tbLastName.Text; // def
            }
            user.id = 1; // ?
            try
            {
                App.db.Users.Add(user);
                App.db.SaveChanges();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"ERROR WITH CREATING NEW USER: {ex.Message}");
            }
            MessageBox.Show("Успешная регистрация!");
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        { 
            this.Close();
        }
    }
}
