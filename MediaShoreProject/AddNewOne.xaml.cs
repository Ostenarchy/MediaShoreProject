using System;
using System.Linq;
using System.Windows;

namespace MediaShoreProject
{
    public partial class AddNewOne : Window

    {
        private Discs _currentDisc; // Храним ссылку на редактируемый диск
        public AddNewOne()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            // Заполняем комбобоксы из БД
            cbAuthor.ItemsSource = App.db.Authors.ToList();
            cbAuthor.DisplayMemberPath = "authorLastName"; // Или другое поле с ФИО
            cbAuthor.SelectedValuePath = "id";

            cbGenre.ItemsSource = App.db.DiscGenres.ToList();
            cbGenre.DisplayMemberPath = "genreName";
            cbGenre.SelectedValuePath = "id";
        }

        public AddNewOne(Discs selectedDisc)
        {
            InitializeComponent();
            LoadData();
            _currentDisc = selectedDisc;

            // Заполняем поля данными выбранного диска
            tbName.Text = _currentDisc.discName;
            cbAuthor.SelectedValue = _currentDisc.discAuthorID;
            cbGenre.SelectedValue = _currentDisc.genreID;
            tbPrice.Text = _currentDisc.discPrice.ToString();
            tbQuantity.Text = _currentDisc.discQuantityInStock.ToString();
            dpReleaseDate.SelectedDate = _currentDisc.discReleaseDate;

            btnSave.Content = "Обновить данные"; // Меняем текст кнопки
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Если мы редактируем, работаем с _currentDisc, если добавляем — создаем новый
                bool isNew = (_currentDisc == null);
                if (isNew) _currentDisc = new Discs();

                _currentDisc.discName = tbName.Text.Trim();
                _currentDisc.discAuthorID = (int)cbAuthor.SelectedValue;
                _currentDisc.genreID = (int)cbGenre.SelectedValue;
                _currentDisc.discPrice = int.Parse(tbPrice.Text);
                _currentDisc.discQuantityInStock = int.Parse(tbQuantity.Text);
                _currentDisc.discReleaseDate = dpReleaseDate.SelectedDate ?? DateTime.Now;
                _currentDisc.discCoverPath = "default.jpg";

                if (isNew) App.db.Discs.Add(_currentDisc);

                App.db.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}