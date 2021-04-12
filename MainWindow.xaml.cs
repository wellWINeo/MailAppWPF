﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Mail;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using OpenPop.Pop3;
using OpenPop.Mime;

namespace MailApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<MailMessage> MailsArr = new List<MailMessage>();
        public string login, passwd;
        public MainWindow()
        {
            LoginWindow loginWindow = new LoginWindow();

            InitializeComponent();

            for (int i = 0; i < 10; i++)
            {
                MailsArr.Add(new MailMessage());
                // IncomingMails.Items.Add("fdghsdf");
            }

            IncomingMails.ItemsSource = MailsArr;

            loginWindow.ShowDialog();

            this.login = loginWindow.Login;
            this.passwd = loginWindow.Passwd;

            //this.LoadMails();

            //MessageBox.Show(login + " - " + passwd);
        }

        public async void LoadMails()
        {
            Pop3Client client = new Pop3Client();
            client.Connect("pop.mail.ru", 995, true);
            client.Authenticate(this.login, this.passwd);

            int count = client.GetMessageCount();
            for (int i = count; i > count - 10; i--)
            {
                this.MailsArr.Add(client.GetMessage(i).ToMailMessage());
            }
            //Message msg = client.GetMessage(count);
            //MailMessage mail_msg = msg.ToMailMessage();
        }

        public void NewMailClick(object sender, RoutedEventArgs e)
        {
            EditorWindow NewMailWindow = new EditorWindow(this.login, this.passwd);
            NewMailWindow.ShowDialog();
            //MessageBox.Show()
        }

    }
}
