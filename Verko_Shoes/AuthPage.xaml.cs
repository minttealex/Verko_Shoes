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

namespace Verko_Shoes
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void BtnEnterGuest_Click(object sender, RoutedEventArgs e)
        {
            string login = TBLogin.Text;
            string password = TBPassword.Text;
            Users Users = Verko_ShoesEntities.GetContext().Users.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);

            Manager.MainFrame.Navigate(new ProductPage(Users = null));
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = TBLogin.Text;
            string password = TBPassword.Text;


            if (login == "" || password == "")
            {
                MessageBox.Show("Заполните все поля");
                return;
            }
            Users Users = Verko_ShoesEntities.GetContext().Users.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            if (Users != null)
            {
                Manager.MainFrame.Navigate(new ProductPage(Users));
                TBLogin.Text = "";
                TBPassword.Text = "";


            }
            else
            {
                MessageBox.Show("Ошибка авторизации");


            }
        }
    }
}
