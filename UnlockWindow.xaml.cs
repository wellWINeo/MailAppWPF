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
    /// Логика взаимодействия для UnlockWindow.xaml
    /// </summary>
    public partial class UnlockWindow : Window
    {
        public string local_passwd;
        public UnlockWindow()
        {
            InitializeComponent();
        }

        private void UnlockClick(object sender, RoutedEventArgs e)
        {
            this.local_passwd = LocalLoginPasswdBox.Password;
            this.Close();
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
