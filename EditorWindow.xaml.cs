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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
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
		public ObservableCollection<String> KnownUsers = new ObservableCollection<String>();
		string pathToUsers = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Known.users");

		public EditorWindow(string login, string passwd)
        {
			this.Login = login;
			this.Passwd = passwd;
            InitializeComponent();
			FetchKnownUsers();
			FromBox.Text = this.Login;
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

			var from = new MailAddress(this.Login);
			var to = new MailAddress(ToCombBox.Text);
			MailMessage msg = new MailMessage(from, to);
			msg.Subject = SubjectBox.Text;
			msg.SubjectEncoding = Encoding.UTF8;

			msg.BodyEncoding = Encoding.UTF8;
			msg.IsBodyHtml = true;

			SmtpClient client;
			if (IsDraft)
            {
				client = new SmtpClient("mysmtpclient");
				client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
				string pick_dir = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Drafts");
				if (!Directory.Exists(pick_dir)) { Directory.CreateDirectory(pick_dir); }
				client.PickupDirectoryLocation = pick_dir;
				System.Windows.MessageBox.Show(pick_dir);
			} 
			else
            {
				string mail_host = "smtp." + this.Login.Split('@')[1];
				client = new SmtpClient(mail_host, 587);
				client.EnableSsl = true;
				client.UseDefaultCredentials = false;
				client.Credentials = new NetworkCredential(this.Login, this.Passwd);
            }

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

            //RtfSource source = new RtfSource(new StreamReader(stream));
            System.Windows.MessageBox.Show(rtf);
			string html = Rtf.ToHtml(rtf);
			System.Windows.MessageBox.Show(html);

			msg.Body = html;

			try
			{
				client.Send(msg);
			} 
			catch (SmtpException e)
            {
				System.Windows.Forms.MessageBox.Show("Error in mail sending!");
            }
			catch (Exception e)
            {
				System.Windows.Forms.MessageBox.Show("Unknown error!\n" + e.Message);
            }
		}
		
		public async void UpdateKnownUsers()
        {
			bool isNew = false;
			foreach(string item in KnownUsers)
            {
				if (item == ToCombBox.Text)
					isNew = true;
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

		private void Quit_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Application.Current.Shutdown();
		}

		private void Save_Quit_Click(object sender, RoutedEventArgs e)
		{
			Save_Executed(sender, e);
			Quit_Click(sender, e);
		}

		private void ChangeColor(object sender, RoutedEventArgs e)
		{
			// rtbEditor.SelectionBrush = Brushes.Green;
			TextSelection text = rtbEditor.Selection;

			ColorDialog colorDialog = new ColorDialog();
			if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				BrushConverter bc = new BrushConverter();
				Brush brush = null;
				try
				{
					brush = (Brush)bc.ConvertFromString(colorDialog.Color.Name);
				}
				catch
				{
					System.Windows.MessageBox.Show("Invalid color");
				}
				text.ApplyPropertyValue(TextElement.ForegroundProperty, brush);

			}
			// rtbEditor.IsInactiveSelectionHighlightEnabled = true;
		}

		private void LineSpacing(object sender, RoutedEventArgs e)
		{
			rtbEditor.Document.LineHeight = Convert.ToDouble((sender as System.Windows.Controls.MenuItem).Header);
		}

	}
}