using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Verko_Shoes
{
    public partial class ProductPage : Page
    {
        private Users currentUsers;
        private string FIOO;

        public bool IsUsersAuthorized => currentUsers != null;

        public ProductPage(Users users)
        {
            var currentProducts = Verko_ShoesEntities.GetContext().Products.ToList();

            InitializeComponent();
            currentUsers = users;
            InitializeUserInterface();

            ComboType.SelectedIndex = 0;

            ProductList.ItemsSource = currentProducts;

            UpdateProducts();
        }

        private void InitializeUserInterface()
        {
            if (currentUsers == null)
            {
                FOITB.Text = "Гость";
                RoleTB.Text = "Посетитель";
                FIOO = "Гость";
                AddButton.Visibility = Visibility.Hidden;
            }
            else
            {
                FOITB.Text = $"{currentUsers.UserSurname} {currentUsers.UserName} {currentUsers.UserPatronymic}";
                switch (currentUsers.RoleID)
                {
                    case 1:
                        RoleTB.Text = "Администратор";
                        break;
                    case 2:
                        RoleTB.Text = "Менеджер";
                        AddButton.Visibility = Visibility.Hidden;
                        break;
                    case 3:
                        RoleTB.Text = "Авторизованный клиент";
                        AddButton.Visibility = Visibility.Hidden;
                        break;
                }
                FIOO = FOITB.Text;
            }
        }

        private void UpdateProducts()
        {
            var currentProducts = Verko_ShoesEntities.GetContext().Products.ToList();

            if (ComboType.SelectedIndex == 1)
            {
                currentProducts = currentProducts.Where(p => p.ProductSupplier == "Kari").ToList();
            }
            else if (ComboType.SelectedIndex == 2)
            {
                currentProducts = currentProducts.Where(p => p.ProductSupplier == "Обувь для вас").ToList();
            }

            currentProducts = currentProducts.Where(p =>
                p.ProductName.ToLower().Contains(TBSearch.Text.ToLower()) ||
                p.ProductSupplier.ToLower().Contains(TBSearch.Text.ToLower()) ||
                p.ProductUnitOfMeasurment.ToLower().Contains(TBSearch.Text.ToLower()) ||
                p.ProductDescription.ToLower().Contains(TBSearch.Text.ToLower()) ||
                p.ProductManufacturer.ToLower().Contains(TBSearch.Text.ToLower())).ToList();

            if (RButtonDown.IsChecked == true)
            {
                currentProducts = currentProducts.OrderByDescending(p => p.ProductCountInStock).ToList();
            }
            else if (RButtonUp.IsChecked == true)
            {
                currentProducts = currentProducts.OrderBy(p => p.ProductCountInStock).ToList();
            }

            ProductList.ItemsSource = currentProducts;
        }

        private void TBSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null, currentUsers));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button).DataContext as Products;
            Manager.MainFrame.Navigate(new AddEditPage(product, currentUsers));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var productToDelete = (sender as Button).DataContext as Products;

            if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Verko_ShoesEntities.GetContext().Products.Remove(productToDelete);
                    Verko_ShoesEntities.GetContext().SaveChanges();
                    UpdateProducts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
    }
}