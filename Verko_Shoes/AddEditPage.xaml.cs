using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Verko_Shoes
{
    public partial class AddEditPage : Page
    {
        private Products _currentProduct = new Products();
        private Users _currentUser;
        private bool _isEditMode;

        public AddEditPage(Products selectedProduct, Users user)
        {
            InitializeComponent();

            _currentUser = user;

            if (selectedProduct != null)
            {
                _currentProduct = selectedProduct;
                _isEditMode = true;
                Title = "Редактирование товара";
            }
            else
            {
                _isEditMode = false;
                Title = "Добавление товара";
            }

            CategoryComboBox.ItemsSource = Verko_ShoesEntities.GetContext().ProductCategory.ToList();
            DataContext = _currentProduct;
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedItem != null)
            {
                var selectedCategory = CategoryComboBox.SelectedItem as ProductCategory;
                _currentProduct.ProductCategoryID = selectedCategory.ProductCategoryID;
                _currentProduct.ProductCategory = selectedCategory;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentProduct.ProductName))
                errors.AppendLine("Укажите название товара");
            else if (_currentProduct.ProductName.Length > 50)
                errors.AppendLine("Название товара слишком длинное (максимум 50 символов)");

            if (string.IsNullOrWhiteSpace(_currentProduct.ProductArticle))
                errors.AppendLine("Укажите артикул товара");
            else if (_currentProduct.ProductArticle.Length > 6)
                errors.AppendLine("Артикул товара слишком длинный (максимум 6 символов)");

            if (string.IsNullOrWhiteSpace(_currentProduct.ProductUnitOfMeasurment))
                errors.AppendLine("Укажите единицу измерения");

            if (string.IsNullOrWhiteSpace(_currentProduct.ProductSupplier))
                errors.AppendLine("Укажите поставщика");

            if (string.IsNullOrWhiteSpace(_currentProduct.ProductManufacturer))
                errors.AppendLine("Укажите производителя");

            if (string.IsNullOrWhiteSpace(_currentProduct.ProductDescription))
                errors.AppendLine("Укажите описание товара");

            if (CategoryComboBox.SelectedItem == null)
                errors.AppendLine("Укажите категорию");

            if (_currentProduct.ProductPrice <= 0)
                errors.AppendLine("Цена должна быть положительным числом");

            if (_currentProduct.ProductCurrentDiscount < 0 || _currentProduct.ProductCurrentDiscount > 100)
                errors.AppendLine("Скидка должна быть в диапазоне от 0 до 100%");

            if (_currentProduct.ProductCountInStock < 0)
                errors.AppendLine("Количество на складе не может быть отрицательным");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                if (!_isEditMode)
                {
                    Verko_ShoesEntities.GetContext().Products.Add(_currentProduct);
                }

                Verko_ShoesEntities.GetContext().SaveChanges();
                MessageBox.Show("Данные успешно сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                Manager.MainFrame.Navigate(new ProductPage(_currentUser));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PhotoChangeButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Выберите изображение товара"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string appFolder = AppDomain.CurrentDomain.BaseDirectory;
                    string picturesFolder = Path.Combine(appFolder, "pic");

                    if (!Directory.Exists(picturesFolder))
                        Directory.CreateDirectory(picturesFolder);

                    string fileName = Path.GetFileName(dialog.FileName);
                    string destinationPath = Path.Combine(picturesFolder, fileName);

                    File.Copy(dialog.FileName, destinationPath, true);

                    _currentProduct.ProductPhoto = Path.Combine("pic", fileName).Replace("\\", "/");

                    PhotoImage.Source = new BitmapImage(new Uri(destinationPath));

                    MessageBox.Show($"Фото успешно сохранено: {fileName}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке фото: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}