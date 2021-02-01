using Models.Enums;

namespace Models.Requests
{
    public class CurseRequest
    {
        public CurrenciesEnum From { get; set; }
        
        public CurrenciesEnum To { get; set; }
    }
}