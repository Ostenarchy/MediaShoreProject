using System;
using System.Collections.Generic;
using System.Data;
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
                textBlockBalance.Text = $"({currentUser.userBalance} руб.)";
            }
            switch (currentUser.roleID)
            {
                case 1:
                    btnAdd.Visibility = Visibility.Visible;
                    btnDelete.Visibility = Visibility.Visible;
                    btnEdit.Visibility = Visibility.Visible;
                    break;
                case 2:
                    btnEdit.Visibility = Visibility.Visible;
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnAdd.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    btnEdit.Visibility = Visibility.Collapsed;
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnAdd.Visibility = Visibility.Collapsed;
                    break;
                default:
                    btnEdit.Visibility = Visibility.Collapsed;
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnAdd.Visibility = Visibility.Collapsed;
                    break;
            }
            
        }

        void LoadDiscs() // дефолтный загрузчик данных
        {
            List<Discs> disc = App.db.Discs.ToList();
            lbDisc.ItemsSource = disc;

            
                // загрузка типов дисков для комбобокса
                List<DiscTypes> ds = App.db.DiscTypes.ToList();
                ds.Insert(0, new DiscTypes { id = 0, typeName = "Все типы" });

                cbDiscType.ItemsSource = ds;
                cbDiscType.DisplayMemberPath = "typeName"; // Что видит пользователь
                cbDiscType.SelectedValuePath = "id";      // Что получаем при выборе
                cbDiscType.SelectedIndex = 0;
            
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
            textBlockBalance.Text = $"({currentUser.userBalance} руб.)";
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

            try
            {
                // 1. Проверка на наличие на складе
                if (selectedDisc.discQuantityInStock <= 0)
                {
                    MessageBox.Show("Этого товара больше нет в наличии!", "Склад");
                    return;
                }

                // 2. Проверка на баланс
                // Считаем общую сумму того, что УЖЕ в корзине
                var cartOrder = App.db.Orders.FirstOrDefault(o => o.userID == currentUser.id && o.statusID == 4);
                int currentCartSum = 0;
                if (cartOrder != null)
                {
                    currentCartSum = App.db.OrderItem
                        .Where(oi => oi.orderID == cartOrder.id)
                        .Sum(oi => (int?)(oi.quantity * oi.PriceAtTime)) ?? 0;
                }

                // Проверяем: хватит ли денег на текущую корзину + новый выбранный диск
                if (currentUser.userBalance < (currentCartSum + selectedDisc.discPrice))
                {
                    MessageBox.Show($"Недостаточно средств на балансе для добавления \"{selectedDisc.discName}\"!", "Баланс");
                    return;
                }

                // 3. Если проверки пройдены — добавляем в корзину
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

                // 4. Обновляем склад и БД
                selectedDisc.discQuantityInStock--;
                App.db.SaveChanges();

                // 5. Обновляем UI
                UpdateCartCounter();
                lbDisc.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void ApplyFilters()
        {
            // ПРОВЕРКА: Если ListBox еще не инициализирован, выходим из метода
            if (lbDisc == null) return;
            // Начинаем с полного списка дисков
            var query = App.db.Discs.AsQueryable();

            // 1. Фильтрация по типу диска (через DiscGenres.typeID)
            if (cbDiscType.SelectedValue != null && (int)cbDiscType.SelectedValue != 0)
            {
                int selectedTypeId = (int)cbDiscType.SelectedValue;
                query = query.Where(d => d.DiscGenres.typeID == selectedTypeId);
            }

            // 2. Поиск по названию
            string searchText = tbSearch.Text.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(d => d.discName.ToLower().Contains(searchText));
            }

            // 3. Сортировка
            if (rbAB.IsChecked == true)
            {
                query = query.OrderBy(d => d.discName);
            }
            else if (rbBA.IsChecked == true)
            {
                query = query.OrderByDescending(d => d.discName);
            }
            // Если выбран rbDEF (IsChecked == true), OrderBy просто не применяется, 
            // и данные идут в порядке их ID из базы.

            // Выводим результат в ListBox
            lbDisc.ItemsSource = query.ToList();
        }

        private void cbDiscType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void rbAB_Checked(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void rbBA_Checked(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void rbDEF_Checked(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddNewOne addWindow = new AddNewOne();
            // Если окно закрылось с результатом true (сохранение прошло успешно)
            if (addWindow.ShowDialog() == true)
            {
                ApplyFilters(); // Твой метод обновления списка ListBox
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // 1. Получаем выбранный объект из ListBox
            var selectedDisc = lbDisc.SelectedItem as Discs;

            if (selectedDisc == null)
            {
                MessageBox.Show("Сначала выбери диск из списка для удаления!", "Внимание");
                return;
            }

            // 2. Подтверждение удаления (чтобы не удалить случайно)
            var result = MessageBox.Show($"Ты уверен, что хочешь безвозвратно удалить диск \"{selectedDisc.discName}\"?",
                                         "Подтверждение удаления",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // 3. Проверка на наличие диска в заказах (чтобы база не выдала ошибку FK)
                    bool hasOrders = App.db.OrderItem.Any(oi => oi.discID == selectedDisc.id);
                    if (hasOrders)
                    {
                        MessageBox.Show("Нельзя удалить этот диск, так как он уже фигурирует в заказах пользователей. " +
                                        "Вместо удаления лучше установи количество на складе в 0.", "Ошибка связи");
                        return;
                    }

                    // 4. Удаляем из контекста и сохраняем
                    App.db.Discs.Remove(selectedDisc);
                    App.db.SaveChanges();

                    MessageBox.Show("Диск успешно удален из базы данных.");

                    // 5. Обновляем список, чтобы диск исчез с экрана
                    ApplyFilters();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла системная ошибка при удалении: " + ex.Message);
                }
            }
        }
        public void Edit(Users user)
        {
            if (user.roleID == 1 || user.roleID == 2)
            {
                // 1. Получаем выбранный диск
                var selectedDisc = lbDisc.SelectedItem as Discs;

                if (selectedDisc == null)
                {
                    MessageBox.Show("Выбери диск из списка для редактирования!", "Внимание");
                    return;
                }

                // 2. Открываем окно и передаем туда наш объект
                AddNewOne editWindow = new AddNewOne(selectedDisc);

                // 3. Если окно закрылось с результатом True (сохранение прошло)
                if (editWindow.ShowDialog() == true)
                {
                    ApplyFilters(); // Обновляем список, чтобы увидеть изменения
                }
            }
            else
            {
                MessageBox.Show("Недостаточно прав для редактирования товара");
            }
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Edit(currentUser);
        }

        private void lbDisc_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Edit(currentUser);
        }


    }
}
