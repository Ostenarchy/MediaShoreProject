using System;
using System.Linq;
using System.Windows;

namespace MediaShoreProject
{
    public partial class OrdersWindow : Window
    {
        public OrdersWindow()
        {
            InitializeComponent();
            RefreshData();
        }

        private void RefreshData()
        {
            // Обновляем списки из базы DIPLOMAT_01
            lbAuthors.ItemsSource = App.db.Authors.ToList();
            lbGenres.ItemsSource = App.db.DiscGenres.ToList();
            RefreshOrders(); // Вызываем специфичный метод с сортировкой
        }

        // --- ЛОГИКА АВТОРОВ ---

        private void btnAddAuthor_Click(object sender, RoutedEventArgs e)
        {
            AdminEditWindow w = new AdminEditWindow("Добавить нового автора");

            if (w.ShowDialog() == true)
            {
                // Разделяем введенный текст по пробелу. 
                // Параметр '2' гарантирует, что если введено "Имя Фамилия Отчество", 
                // то "Имя" уйдет в первую переменную, а всё остальное — во вторую.
                var parts = w.ResultText.Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 0)
                {
                    Authors newAuthor = new Authors();

                    // Записываем данные в реальные поля БД
                    newAuthor.authorFirstName = parts[0];
                    newAuthor.authorLastName = parts.Length > 1 ? parts[1] : "";

                    App.db.Authors.Add(newAuthor);
                    App.db.SaveChanges();
                    RefreshData();
                }
                else
                {
                    MessageBox.Show("Поле не может быть пустым!");
                }
            }
        }

        private void btnEditAuthor_Click(object sender, RoutedEventArgs e)
        {
            var selected = lbAuthors.SelectedItem as Authors;
            if (selected == null) return;

            AdminEditWindow w = new AdminEditWindow("Редактировать автора", selected.authorFullName);
            if (w.ShowDialog() == true)
            {
                // Делим строку по пробелу
                var parts = w.ResultText.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 0)
                {
                    selected.authorFirstName = parts[0]; // Первое слово в имя
                    selected.authorLastName = parts.Length > 1 ? parts[1] : ""; // Остальное в фамилию

                    App.db.SaveChanges();
                    RefreshData();
                }
            }
        }

        private void btnDeleteAuthor_Click(object sender, RoutedEventArgs e)
        {
            var selected = lbAuthors.SelectedItem as Authors;
            if (selected == null) return;

            if (App.db.Discs.Any(d => d.discAuthorID == selected.id))
            {
                MessageBox.Show("Нельзя удалить автора, так как за ним закреплены диски!");
                return;
            }

            App.db.Authors.Remove(selected);
            App.db.SaveChanges();
            RefreshData();
        }

        // --- ЛОГИКА ЖАНРОВ ---

        private void btnAddGenre_Click(object sender, RoutedEventArgs e)
        {
            AdminEditWindow w = new AdminEditWindow("Добавить жанр");
            if (w.ShowDialog() == true)
            {
                DiscGenres newGenre = new DiscGenres { genreName = w.ResultText };
                App.db.DiscGenres.Add(newGenre);
                App.db.SaveChanges();
                RefreshData();
            }
        }

        private void btnEditGenre_Click(object sender, RoutedEventArgs e)
        {
            var selected = lbGenres.SelectedItem as DiscGenres;
            if (selected == null) return;

            AdminEditWindow w = new AdminEditWindow("Изменить жанр", selected.genreName);
            if (w.ShowDialog() == true)
            {
                selected.genreName = w.ResultText;
                App.db.SaveChanges();
                RefreshData();
            }
        }

        private void btnDeleteGenre_Click(object sender, RoutedEventArgs e)
        {
            var selected = lbGenres.SelectedItem as DiscGenres;
            if (selected == null) return;

            if (App.db.Discs.Any(d => d.genreID == selected.id))
            {
                MessageBox.Show("Этот жанр используется в товарах. Удаление невозможно.");
                return;
            }

            App.db.DiscGenres.Remove(selected);
            App.db.SaveChanges();
            RefreshData();
        }

        // --- ЛОГИКА ЗАКАЗОВ ---

        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var selected = lbOrders.SelectedItem as Orders;
            if (selected == null) return;

            var res = MessageBox.Show("Удалить заказ №" + selected.id + "?", "Подтверждение", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                // Сначала удаляем состав заказа, чтобы не нарушить связи
                var items = App.db.OrderItem.Where(oi => oi.orderID == selected.id).ToList();
                App.db.OrderItem.RemoveRange(items);

                App.db.Orders.Remove(selected);
                App.db.SaveChanges();
                RefreshData();
            }
        }

        private void btnEditStatus_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = lbOrders.SelectedItem as Orders;
            if (selectedOrder == null)
            {
                MessageBox.Show("Сначала выберите заказ из списка!");
                return;
            }

            // Открываем окно и передаем текущий статус
            StatusEditWindow w = new StatusEditWindow(selectedOrder.Statuses);
            w.Owner = this; // Чтобы окно открылось по центру родителя

            if (w.ShowDialog() == true)
            {
                // Обновляем статус в объекте заказа
                selectedOrder.statusID = w.SelectedStatus.id;

                App.db.SaveChanges(); // Сохраняем изменения в БД
                RefreshData(); // Обновляем ListBox
            }

        }

        private void rbSortOrder_Checked(object sender, RoutedEventArgs e)
        {
            RefreshOrders();
        }

        // Отдельный метод для обновления только заказов
        private void RefreshOrders()
        {
            if (lbOrders == null) return;

            var query = App.db.Orders.AsQueryable();

            // Применяем сортировку
            if (rbNew.IsChecked == true)
            {
                query = query.OrderByDescending(o => o.orderDateOfPurchase); // Сначала новые (по убыванию даты)
            }
            else if (rbOld.IsChecked == true)
            {
                query = query.OrderBy(o => o.orderDateOfPurchase); // Сначала старые (по возрастанию даты)
            }

            lbOrders.ItemsSource = query.ToList();
        }
    }
}