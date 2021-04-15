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
            BrowserView.NavigateToStream(html_stream);
        }
    }
}
