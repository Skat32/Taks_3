using Models.Enums;

namespace Models.Entities
{
    public class Curse : Base
    {
        public CurrenciesEnum CurrenciesFrom { get; set; }
        
        public CurrenciesEnum CurrenciesTo { get; set; }
        
        /// <summary>
        /// Цена за за валюту
        /// </summary>
        public decimal Value { get; set; }
    }
}