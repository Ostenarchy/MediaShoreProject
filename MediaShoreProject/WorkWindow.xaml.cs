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
        private void UpdateCartCounter()
        {
            var cartOrder = App.db.Orders.FirstOrDefault(o => o.userID == currentUser.id && o.statusID == 4);
            int count = cartOrder != null ? App.db.OrderItem.Where(oi => oi.orderID == cartOrder.id).Sum(oi => (int?)oi.quantity) ?? 0 : 0;
            btnCart.Content = $"Корзина ({count})";
        }
        private void btnCart_Click(object sender, RoutedEventArgs e)
        {
            CartWindow cw = new CartWindow(currentUser);
            cw.ShowDialog();
            // 3. Этот код сработает СРАЗУ ПОСЛЕ закрытия CartWindow
            LoadDiscs();         // Перезагрузит список дисков из БД (обновит "Осталось")
            UpdateCartCounter(); // Обновит текст на самой кнопке "Корзина (n)"
        }

        private Orders GetOrCreateCart()
        {
            var order = App.db.Orders.FirstOrDefault(o => o.userID == currentUser.id && o.statusID == 4);
            if (order == null)
            {
                order = new Orders
                {
                    userID = currentUser.id,
                    statusID = 4,
                    orderDateOfPurchase = DateTime.Now,
                    totalSum = 0
                };
                App.db.Orders.Add(order);
                App.db.SaveChanges();
            }
            return order;
        }

        private void Button_Click(object sender, RoutedEventArgs e) // -
        {
            // 1. Получаем объект диска, на который нажали
            var selectedDisc = (sender as Button)?.DataContext as Discs;
            if (selectedDisc == null || currentUser == null) return;

            // 2. Находим активную корзину пользователя (статус 4)
            var order = App.db.Orders.FirstOrDefault(o => o.userID == currentUser.id && o.statusID == 4);
            if (order == null) return;

            // 3. Ищем, есть ли этот диск в корзине
            var orderItem = App.db.OrderItem.FirstOrDefault(oi => oi.orderID == order.id && oi.discID == selectedDisc.id);

            // 4. Если товар найден в корзине — возвращаем его на склад
            if (orderItem != null)
            {
                if (orderItem.quantity > 1)
                {
                    // Уменьшаем количество в корзине
                    orderItem.quantity--;
                }
                else
                {
                    // Если была последняя единица — удаляем запись из корзины совсем
                    App.db.OrderItem.Remove(orderItem);
                }

                // ВАЖНО: Возвращаем единицу товара на склад
                selectedDisc.discQuantityInStock++;

                // 5. Сохраняем изменения в базе DIPLOMAT_01
                App.db.SaveChanges();

                // 6. Обновляем интерфейс
                UpdateCartCounter(); // Текст на кнопке "Корзина (n)"
                lbDisc.Items.Refresh(); // Обновляем текст "Осталось: n" в списке
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) // + 
        {
            var selectedDisc = (sender as Button)?.DataContext as Discs;
            if (selectedDisc == null || currentUser == null) return;

            // ПРОВЕРКА: Если на складе 0, не даем добавить
            if (selectedDisc.discQuantityInStock <= 0)
            {
                MessageBox.Show("Товар закончился на складе!");
                return;
            }

            try
            {
                var order = GetOrCreateCart();
                var orderItem = App.db.OrderItem.FirstOrDefault(oi => oi.orderID == order.id && oi.discID == selectedDisc.id);

                if (orderItem != null)
                {
                    orderItem.quantity++;
                }
                else
                {
                    App.db.OrderItem.Add(new OrderItem
                    {
                        orderID = order.id,
                        discID = selectedDisc.id,
                        quantity = 1,
                        PriceAtTime = selectedDisc.discPrice
                    });
                }

                // УМЕНЬШАЕМ количество на складе
                selectedDisc.discQuantityInStock--;

                App.db.SaveChanges();

                // ОБНОВЛЯЕМ интерфейс
                UpdateCartCounter();
                lbDisc.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
