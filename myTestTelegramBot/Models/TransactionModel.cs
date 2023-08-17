namespace myTestTelegramBot.Models
{
    public class TransactionModel
    {
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public TransactionCategory Category
        {
            get => Category;
            set
            {
                Category = value;
                CategoryStr = TransactionCategories.GetValueOrDefault(value);
            }
        }
        public string? CategoryStr { get; set; }
        public DateTime Date { get; set; }




        public enum TransactionType : byte
        {
            Income,
            Outcome
        }

        public enum TransactionCategory
        {
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
            { TransactionCategory.DigitalPurchases , "Цифровые покупки" }, 
        };

    }
}
