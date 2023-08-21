using System.ComponentModel.DataAnnotations;

namespace myTestTelegramBot.Models
{
    public class UserModel
    {
        [Key]
        public  int Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }

        public UserModel(long userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }
    }
}
