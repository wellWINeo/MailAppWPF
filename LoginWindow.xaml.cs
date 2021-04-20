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
using System.IO;


namespace MailApp
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public string Login, Passwd;
        public string LoginFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Login.txt");
        public bool IsLoginFileExist = false;
        public string SourceUri { get; }

        public LoginWindow()
        {
            InitializeComponent();

            if (File.Exists(LoginFilePath))
            {
                LoginBox.Visibility = Visibility.Collapsed;
                LoginLabel.Visibility = Visibility.Collapsed;
                SaveLogin.Visibility = Visibility.Collapsed;
                IsLoginFileExist = true;
            }
        }

        /// <summary>
        /// Login click handler
        /// </summary>
        public void LoginClick(object button, RoutedEventArgs e)
        {
            this.Login = LoginBox.Text;
            this.Passwd = PasswdBox.Password;

            if (IsLoginFileExist)
            {
                DecryptFile(this.Passwd);
            }
            else if (SaveLogin.IsChecked == true)
            {
                UnlockWindow unlocker = new UnlockWindow();
                unlocker.ShowDialog();
                EncryptFile(unlocker.local_passwd);
            }

            this.Close();
        }

        /// <summary>
        /// Metho to encrypt login/passowrd and
        /// write it to .txt. file
        /// </summary>
        private void EncryptFile(string local_passwd)
        {            
            using (FileStream fs = new FileStream(LoginFilePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine(StringCipher.Encrypt(this.Login, local_passwd));
                    writer.WriteLine(StringCipher.Encrypt(this.Passwd, local_passwd));
                }
            }
        }


        /// <summary>
        /// Method to decrypt login & password, 
        /// written in plaint .txt file
        /// </summary>
        private void DecryptFile(string local_passwd)
        {

            using (FileStream fs = new FileStream(LoginFilePath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    this.Login = StringCipher.Decrypt(reader.ReadLine(), local_passwd);
                    this.Passwd = StringCipher.Decrypt(reader.ReadLine(), local_passwd);
                }
            }
        }
    }
}
