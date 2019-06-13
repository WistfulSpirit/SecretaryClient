using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretaryClient
{
    public class Message
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public System.DateTime Registry_Date { get; set; }
        public Person Adressee { get; set; }
        public Person Sender { get; set; }
        public string Content { get; set; }
        public IList<Tag> Tags { get; set; }
    }
}
