using System.ComponentModel;

namespace Models.Enums
{
    public enum CurrenciesEnum
    {
        [Description("RUB")]
        Rub = 1,
        
        [Description("EUR")]
        Eur = 2,
        
        [Description("USD")]
        Usd = 3
    }
}