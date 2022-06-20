using Hafta._4.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hafta._4.Domain.Entities
{
    public class Message
    {
        [Key]
        public int MessageID { get; set; }
        public AppUser Sender { get; set; }
        public string Subject { get; set; }
        public string MessageContent { get; set; }
        public AppUser Receiver { get; set; }
        public string Additional { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string MessageType { get; set; }
    }

}
