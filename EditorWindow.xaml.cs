using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Net.Mail;
using System.Net;

using RtfPipe;
using System.Collections.ObjectModel;

namespace MailApp
{
	/// <summary>
	/// Логика взаимодействия для EditorWindow.xaml
	/// </summary>
	public partial class EditorWindow : Window
	{
		public string Login;
		private string Passwd;
		public int[] fontSizeArr = new int[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
		public ObservableCollection<String> KnownUsers = new ObservableCollection<String>();
		string pathToUsers = Path.Combine(Directory.GetCurrentDirectory(), "Known.users");
		public Collection<Attachment> AttachArr = new Collection<Attachment>();
		public MailMessage msg;

		public EditorWindow(string login, string passwd)
        {
			this.Login = login;
			this.Passwd = passwd;

            InitializeComponent();

			AttachCounter.Content = 0;
			FetchKnownUsers();
			ToCombBox.ItemsSource = KnownUsers;
			FromBox.Text = this.Login;
			cmbFontSize.ItemsSource = fontSizeArr;
        }

		private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
		{
			object temp = rtbEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
			btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
			temp = rtbEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
			btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
			temp = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
			btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

			temp = rtbEditor.Selection.GetPropertyValue(Inline.FontFamilyProperty);
			//cmbFontFamily.SelectedItem = temp;
			temp = rtbEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
			cmbFontSize.Text = temp.ToString();
		}

		public void MailSend(bool IsDraft)
        {
			// Update known users
			UpdateKnownUsers();

			// Creating mail
			var from = new MailAddress(this.Login);
			var to = new MailAddress(ToCombBox.Text);
			msg = new MailMessage(from, to);
			
			// Add attachements
			foreach (var attach in AttachArr)
            {
				msg.Attachments.Add(attach);
            }

			// subject & body
			msg.Subject = SubjectBox.Text;
			msg.SubjectEncoding = Encoding.UTF8;

			msg.BodyEncoding = Encoding.UTF8;
			msg.IsBodyHtml = true;
			
			// mail sending
			SmtpClient client;
			if (IsDraft)
            {
				client = new SmtpClient("mysmtpclient");
				client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
				string pick_dir = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Drafts");
				if (!Directory.Exists(pick_dir)) { Directory.CreateDirectory(pick_dir); }
				client.PickupDirectoryLocation = pick_dir;
			} 
			else
            {
				string mail_host = "smtp." + this.Login.Split('@')[1];
				client = new SmtpClient(mail_host, 587);
				client.EnableSsl = true;
				client.UseDefaultCredentials = false;
				client.Credentials = new NetworkCredential(this.Login, this.Passwd);
            }

			// converting rtf -> html
			string rtf = string.Empty;

			using (MemoryStream stream = new MemoryStream())
			{
				TextRange range = new TextRange(rtbEditor.Document.ContentStart,
											rtbEditor.Document.ContentEnd);
				range.Save(stream, System.Windows.DataFormats.Rtf);
				stream.Seek(0, SeekOrigin.Begin);


				using (StreamReader reader = new StreamReader(stream))
				{
					rtf = reader.ReadToEnd();
				}
			}

			string html = Rtf.ToHtml(rtf);
			msg.Body = html;

			try
			{
				client.Send(msg);
			} 
			catch (SmtpException e)
            {
				MessageBox.Show("Error in mail sending!");
            }
			catch (Exception e)
            {
				MessageBox.Show("Unknown error!\n" + e.Message);
            }
            finally
            {
				msg = null;
            }
		}
		
		public async void UpdateKnownUsers()
        {
			bool isNew = true;
			foreach(string item in KnownUsers)
            {
				if (item == ToCombBox.Text)
                {
					isNew = false;
					break;
                }
            }
			if (isNew)
			{
				using (FileStream fs = new FileStream(pathToUsers, FileMode.OpenOrCreate))
				{
					using (StreamWriter writer = new StreamWriter(fs))
					{
						await writer.WriteLineAsync(ToCombBox.Text);
					}
				}

				KnownUsers.Add(ToCombBox.Text);
			}
        }

		public async void FetchKnownUsers()
        {
			

			using (FileStream fs = new FileStream(pathToUsers, FileMode.OpenOrCreate))
            {
				using (StreamReader reader = new StreamReader(fs))
                {
					while (!reader.EndOfStream)
						this.KnownUsers.Add(await reader.ReadLineAsync());
                }
            }
        }

		private void SendMailClick(object sender, RoutedEventArgs e) { this.MailSend(false); }
		private void SaveAsDraftClick(object sender, RoutedEventArgs e) { this.MailSend(true); }

		private void Open_Executed(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
			if (dlg.ShowDialog() == true)
			{
				FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
				TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
				range.Load(fileStream, System.Windows.DataFormats.Rtf);
			}
		}

		private void Save_Executed(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
			dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
			if (dlg.ShowDialog() == true)
			{
				FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
				TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
				range.Save(fileStream, System.Windows.DataFormats.Rtf);
			}
		}


		private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
		{
			rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
		}

		private void LineSpacing(object sender, RoutedEventArgs e)
		{
			rtbEditor.Document.LineHeight = Convert.ToDouble((sender as System.Windows.Controls.MenuItem).Header);
		}

        private void rtbEditor_Drop(object sender, System.Windows.DragEventArgs e)
        {
			if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
				string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

				foreach (string file in files)
				{
					MessageBox.Show(file);
					AttachArr.Add(new Attachment(file));
					AttachCounter.Content = (int)AttachCounter.Content + 1;
				}
            }
        }

        private void rtbEditor_PreviewDragOver(object sender, System.Windows.DragEventArgs e)
        {
			e.Handled = true;
		}

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
			Color? color = ColorPickerElement.SelectedColor;

			var converter = new BrushConverter();
			var brush = (Brush)converter.ConvertFromString(color.ToString());

			TextSelection text = rtbEditor.Selection;
			text.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
		}
    }
}