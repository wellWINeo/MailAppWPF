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

namespace MailApp
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public string Login, Passwd;

        public LoginWindow()
        {
            InitializeComponent();
        }

        public void LoginClick(object button, RoutedEventArgs e)
        {
            this.Login = LoginBox.Text;
            this.Passwd = PasswdBox.Password;
            this.Close();
        }
    }
}
