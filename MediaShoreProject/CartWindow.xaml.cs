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
            // 1. Ищем текущую корзину пользователя (статус 4)
            currentOrder = App.db.Orders.FirstOrDefault(o => o.userID == user.id && o.statusID == 4);

            if (currentOrder != null)
            {
                // 2. Получаем список всех позиций в этой корзине
                var items = App.db.OrderItem.Where(oi => oi.orderID == currentOrder.id).ToList();
                lbCartItems.ItemsSource = items;

                // 3. Считаем итоговую сумму
                // Используем (int?), чтобы Sum не падал на пустом списке, и ?? 0 для замены null на ноль
                int total = items.Sum(i => (int?)((i.quantity ?? 0) * (i.PriceAtTime ?? 0))) ?? 0;

                txtTotalSum.Text = $"Итого: {total} руб.";

                // 4. Логика кнопки оформления
                if (items.Count == 0)
                {
                    // Если в корзине пусто
                    btnCheckout.IsEnabled = false;
                    btnCheckout.Content = "Корзина пуста";
                }
                else if (user.userBalance < total)
                {
                    // Если денег не хватает
                    btnCheckout.IsEnabled = false;
                    btnCheckout.Content = "Недостаточно средств";
                    txtTotalSum.Foreground = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    // Если всё отлично
                    btnCheckout.IsEnabled = true;
                    btnCheckout.Content = "Оформить заказ";
                    txtTotalSum.Foreground = System.Windows.Media.Brushes.Black;
                }
            }
            else
            {
                // Если заказа со статусом "Корзина" вообще не существует в БД
                lbCartItems.ItemsSource = null;
                txtTotalSum.Text = "Итого: 0 руб.";
                btnCheckout.IsEnabled = false;
                btnCheckout.Content = "Оформить заказ";
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
                // 1. Еще раз считаем итоговую сумму для надежности
                int total = App.db.OrderItem
                    .Where(oi => oi.orderID == currentOrder.id)
                    .Sum(i => (i.quantity ?? 0) * (i.PriceAtTime ?? 0));

                // 2. Финальная проверка на всякий случай
                if (user.userBalance < total)
                {
                    MessageBox.Show($"Ошибка! Тебе не хватает {total - user.userBalance} руб. для совершения покупки.",
                                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 3. Подтверждение покупки
                var result = MessageBox.Show($"С твоего баланса будет списано {total} руб. Продолжить?",
                                             "Подтверждение", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    // 4. Списываем деньги
                    user.userBalance -= total;

                    // 5. Переводим заказ из состояния "Корзина" (4) в "Оплачен" (3)
                    currentOrder.statusID = 3;
                    currentOrder.totalSum = total;
                    currentOrder.orderDateOfPurchase = DateTime.Now;

                    // 6. Сохраняем всё в БД DIPLOMAT_01
                    App.db.SaveChanges();

                    MessageBox.Show("Заказ успешно оплачен! Спасибо за покупку.", "Успех");
                    this.Close(); // Закрываем окно корзины
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при оплате: " + ex.Message);
            }
        }
    }
}
