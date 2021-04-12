using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailApp
{
    public class Mail
    {
        public string from { get; set; }
        public string to { get; set; }
        public string subject { get; set; }
        public string body { get; set;  }

        public Mail(string from, string to, string subject, string body)
        {
            this.from = from;
            this.to = to;
            this.subject = subject;
            this.body = body;
        }
    }
}
