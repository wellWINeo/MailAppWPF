using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;
using System.Net.Mail;

using MsgReader;

namespace MailApp
{
    /// <summary>
    /// Логика взаимодействия для DraftWindow.xaml
    /// </summary>
    public partial class DraftWindow : Window
    {
        public string DraftsPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Drafts");
        public List<MailMessage> DraftsArr = new List<MailMessage>();
        public Dictionary<int, string> MailPathDict = new Dictionary<int, string>();
        
        public DraftWindow()
        {
            InitializeComponent();
            new Thread(LoadDrafts).Start();
            this.DraftMails.ItemsSource = DraftsArr;
        }

        /// <summary>
        /// Method delete Draft
        /// </summary>
        private void DeleteDraftClick(object sender, RoutedEventArgs e)
        {
            if (DraftMails.SelectedItems.Count != 0)
            {
                foreach (MailMessage select in DraftMails.SelectedItems)
                {
                    File.Delete(this.MailPathDict[select.GetHashCode()]);
                    MailPathDict.Remove(select.GetHashCode());
                    DraftsArr.Remove(select);
                }

                this.DraftMails.ItemsSource = this.DraftsArr;
            }
        }

        private void SendDraftClick(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Method to Load drafts from .eml files
        /// </summary>
        public void LoadDrafts()
        {
            DirectoryInfo dir = new DirectoryInfo(this.DraftsPath);
            FileInfo[] files = dir.GetFiles("*.eml");
            foreach(var file in files)
            {
                MsgReader.Mime.Message eml = MsgReader.Mime.Message.Load(file);

                MailMessage msg = new MailMessage();

                if (eml.Headers != null){
                    
                    // parsing headers
                    if (eml.Headers.From != null)
                    {
                        msg.From = new MailAddress(eml.Headers.From.Address);
                    }

                    if (eml.Headers.To != null)
                    {
                        foreach (var receiver in eml.Headers.To)
                        {
                            msg.To.Add(receiver.Address);
                        }
                    }

                    // subject
                    msg.Subject = eml.Headers.Subject;

                    // parsing body content
                    if (eml.HtmlBody != null)
                    {
                        msg.IsBodyHtml = true;
                        msg.Body = Encoding.UTF8.GetString(eml.HtmlBody.Body);
                    }
                    else if (eml.TextBody != null)
                    {
                        msg.Body = Encoding.UTF8.GetString(eml.TextBody.Body);
                    }

                    // adding attachements
                    foreach (var attach in eml.Attachments)
                    {
                        var tmp_attach = new Attachment(new MemoryStream(attach.Body), attach.FileName);
                        msg.Attachments.Add(tmp_attach);
                    }
                }

                this.MailPathDict.Add(msg.GetHashCode(), file.ToString());
                this.DraftsArr.Add(msg);
            }
        }
    }
}
