using Telegram.Bot.Types;

namespace myTestTelegramBot.Models
{
    public class TransactionModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserModel User { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }


        public TransactionModel(Message message)
        {
            Text = !string.IsNullOrEmpty(message.Text) ? message.Text : "—";
            Date = DateTime.Now;
            User = new UserModel(message.From.Id, message.From.Username);
            Type = "";
            Category = "";
        }
        public TransactionModel()
        {
        }
        //public enum TransactionType : byte
        //{
        //    Unknown,
        //    Income,
        //    Outcome
        //}
        //public enum TransactionCategory
        //{
        //    Unknown,
        //    Transport,
        //    HealthAndBeauty,
        //    Education,
        //    Entertainment,
        //    TourismAndTravel,
        //    FoodsAndHouseholdGoods,
        //    UtilityBills,
        //    InternetAndCommunications,
        //    Rent,
        //    UnforeseenAndRepair,
        //    ClothingAndGoods,
        //    DigitalPurchases
        //}
        //static readonly Dictionary<TransactionCategory, string> TransactionCategories = new Dictionary<TransactionCategory, string>
        //{
        //    { TransactionCategory.Unknown , "Неизвестно" },
        //    { TransactionCategory.Transport , "Транспорт" },
        //    { TransactionCategory.HealthAndBeauty , "Здоровье и красота" },
        //    { TransactionCategory.Education , "Образование" },
        //    { TransactionCategory.Entertainment , "Развлечения" },
        //    { TransactionCategory.TourismAndTravel , "Туризм, путешествия" },
        //    { TransactionCategory.FoodsAndHouseholdGoods , "Продукты и хозтовары" },
        //    { TransactionCategory.UtilityBills , "Квартплата" },
        //    { TransactionCategory.InternetAndCommunications , "Интернет и связь" },
        //    { TransactionCategory.Rent , "Аренда жилья" },
        //    { TransactionCategory.UnforeseenAndRepair , "Непредвиденное, ремонт" },
        //    { TransactionCategory.ClothingAndGoods , "Одежда, товары" },
        //    { TransactionCategory.DigitalPurchases , "Цифровые покупки" }
        //};
    }
}
