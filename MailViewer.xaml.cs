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

using System.Diagnostics;

namespace MailApp
{
    /// <summary>
    /// Логика взаимодействия для MailViewer.xaml
    /// </summary>
    public partial class MailViewer : Window
    {
        public MailMessage mail { get; set; }
        public MemoryStream html_stream;
        public MailViewer(MailMessage mail)
        {
            this.mail = mail;
            InitializeComponent();

            AttachCount.Content = this.mail.Attachments.Count();

            html_stream = new MemoryStream(Encoding.UTF8.GetBytes(this.mail.Body));
            
            // fill info about mail
            FromLabel.Content = this.mail.From;
            ToListBox.ItemsSource = this.mail.To;
            SubjectLabel.Content = this.mail.Subject;

            // open body in browser
            BrowserView.NavigateToStream(html_stream);           
        }

        private async void ViewButtonClick(object sender, RoutedEventArgs e)
        {
            string downloadPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Downloads");
            if (!Directory.Exists(downloadPath))
                Directory.CreateDirectory(downloadPath);

            foreach (Attachment attach in mail.Attachments)
            {
                string filePath = System.IO.Path.Combine(downloadPath, attach.Name);
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.CreateNew))
                    {
                        attach.ContentStream.Position = 0;
                        await attach.ContentStream.CopyToAsync(fs);
                    }
                } catch (IOException) { }
            }

            Process.Start(downloadPath);
        }
    }
}
