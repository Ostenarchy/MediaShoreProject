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
    /// Interaction logic for CartWindow.xaml
    /// </summary>
    /// 

    public partial class CartWindow : Window
    {
        private Users user;
        private Orders currentOrder;
        public CartWindow(Users currentUser)
        {
            InitializeComponent();
            user = currentUser;
            LoadCart();
        }
        private void LoadCart()
        {
            currentOrder = App.db.Orders.FirstOrDefault(o => o.userID == user.id && o.statusID == 4);
            if (currentOrder != null)
            {
                var items = App.db.OrderItem.Where(oi => oi.orderID == currentOrder.id).ToList();
                lbCartItems.ItemsSource = items;

                // Считаем общую сумму
                int total = items.Sum(i => (i.quantity ?? 0) * (i.PriceAtTime ?? 0));
                txtTotalSum.Text = $"Итого: {total} руб.";

                // Проверка баланса
                if (user.userBalance < total)
                {
                    btnCheckout.IsEnabled = false;
                    btnCheckout.Content = "Недостаточно средств";
                    btnCheckout.Background = Brushes.Gray;
                }
                else
                {
                    btnCheckout.IsEnabled = true;
                    btnCheckout.Content = "Оформить заказ";
                }
            }
        }
        private void btnPlus_Click(object sender, RoutedEventArgs e)
        {
            // 1. Получаем элемент корзины (OrderItem)
            var item = (sender as Button)?.DataContext as OrderItem;
            if (item == null) return;

            // 2. Находим соответствующий диск на складе, чтобы проверить остаток
            var discInStock = App.db.Discs.FirstOrDefault(d => d.id == item.discID);

            if (discInStock != null && discInStock.discQuantityInStock > 0)
            {
                // Увеличиваем в корзине, уменьшаем на складе
                item.quantity++;
                discInStock.discQuantityInStock--;

                App.db.SaveChanges();
                LoadCart(); // Метод для обновления списка и пересчета итоговой суммы
            }
            else
            {
                MessageBox.Show("На складе больше нет этого товара!");
            }
        }

        private void btnMinus_Click(object sender, RoutedEventArgs e)
        {
            // 1. Получаем элемент корзины
            var item = (sender as Button)?.DataContext as OrderItem;
            if (item == null) return;

            // 2. Находим диск на складе для возврата
            var discInStock = App.db.Discs.FirstOrDefault(d => d.id == item.discID);

            if (item.quantity > 1)
            {
                item.quantity--;
                if (discInStock != null) discInStock.discQuantityInStock++;
            }
            else
            {
                // Если был 1 и нажали минус — удаляем из корзины совсем
                if (discInStock != null) discInStock.discQuantityInStock++;
                App.db.OrderItem.Remove(item);
            }

            App.db.SaveChanges();
            LoadCart();
        }

        private void btnCheckout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int total = App.db.OrderItem.Where(oi => oi.orderID == currentOrder.id)
                                            .Sum(i => (i.quantity ?? 0) * (i.PriceAtTime ?? 0));

                // Списываем баланс и меняем статус
                user.userBalance -= total;
                currentOrder.statusID = 3; // Оплачен
                currentOrder.orderDateOfPurchase = DateTime.Now;
                currentOrder.totalSum = total;

                App.db.SaveChanges();
                MessageBox.Show("Заказ успешно оформлен!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
