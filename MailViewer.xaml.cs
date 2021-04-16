using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
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


// cryptography
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace MailApp
{
    /// <summary>
    /// Логика взаимодействия для MailViewer.xaml
    /// </summary>
    public partial class MailViewer : Window
    {
        public MailMessage mail;
        public MemoryStream html_stream;
        public MailViewer(MailMessage mail)
        {
            this.mail = mail;
            InitializeComponent();
            html_stream = new MemoryStream(Encoding.UTF8.GetBytes(this.mail.Body));
            
            // fill info about mail
            FromLabel.Content = this.mail.From;
            ToLabel.Content = this.mail.To;
            SubjectLabel.Content = this.mail.Subject;

            // open body in browser
           
            BrowserView.NavigateToStream(html_stream);

           
        }
    }
}
