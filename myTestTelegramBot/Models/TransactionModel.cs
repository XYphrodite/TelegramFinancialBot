using System.Text.Json.Serialization;

namespace myTestTelegramBot.Models
{
    public class TransactionModel
    {
        [JsonIgnore]
        public TransactionType Type { get; set; }
        [JsonIgnore]
        public decimal Amount { get; set; }
        private TransactionCategory _category = TransactionCategory.Unknown;
        [JsonIgnore]
        public TransactionCategory Category
        {
            get => _category;
            set
            {
                _category = value;
                CategoryStr = TransactionCategories.GetValueOrDefault(value);
            }
        }
        [JsonIgnore]
        public string? CategoryStr { get; set; }
        public DateTime Date { get; set; }
        public string? Text { get; set; }


        public TransactionModel(string text)
        {
            Text = text;
            Date = DateTime.Now;
            Category = TransactionCategory.Unknown;
            Type = TransactionType.Unknown;
        }



        public enum TransactionType : byte
        {
            Unknown,
            Income,
            Outcome
        }
        public enum TransactionCategory
        {
            Unknown,
            Transport,
            HealthAndBeauty,
            Education,
            Entertainment,
            TourismAndTravel,
            FoodsAndHouseholdGoods,
            UtilityBills,
            InternetAndCommunications,
            Rent,
            UnforeseenAndRepair,
            ClothingAndGoods,
            DigitalPurchases
        }
        static readonly Dictionary<TransactionCategory, string> TransactionCategories = new Dictionary<TransactionCategory, string>
        {
            { TransactionCategory.Unknown , "Неизвестно" },
            { TransactionCategory.Transport , "Транспорт" },
            { TransactionCategory.HealthAndBeauty , "Здоровье и красота" },
            { TransactionCategory.Education , "Образование" },
            { TransactionCategory.Entertainment , "Развлечения" },
            { TransactionCategory.TourismAndTravel , "Туризм, путешествия" },
            { TransactionCategory.FoodsAndHouseholdGoods , "Продукты и хозтовары" },
            { TransactionCategory.UtilityBills , "Квартплата" },
            { TransactionCategory.InternetAndCommunications , "Интернет и связь" },
            { TransactionCategory.Rent , "Аренда жилья" },
            { TransactionCategory.UnforeseenAndRepair , "Непредвиденное, ремонт" },
            { TransactionCategory.ClothingAndGoods , "Одежда, товары" },
            { TransactionCategory.DigitalPurchases , "Цифровые покупки" }
        };
    }
}
