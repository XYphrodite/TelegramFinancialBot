using System.ComponentModel.DataAnnotations;

namespace myTestTelegramBot.Models
{
    public class UserModel
    {
        [Key]
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public ICollection<TransactionModel> Transactions { get; set; } = new List<TransactionModel>();

        public UserModel(long userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }
    }
}
