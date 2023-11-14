
namespace API.Entities
{
    public class Message
    {
        //one to many relationship between sender and message
        //one to many relationship between recipient and message
        public int Id { get; set; }   
        public int SenderId { get; set; }
        public int SenderUsername { get; set; }
        public AppUser Sender { get; set; }

        public int RecipientId { get; set; }
        public int RecipientUsername { get; set; }
        public AppUser Recipient { get; set; }
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.UtcNow;

        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
    }
}