using System.ComponentModel;

namespace Models.Enums
{
    public enum PeriodEnum
    {
        [Description("5 минут")]
        T5 = 5, 
        
        [Description("15 минут")]
        T15 = 15,
        
        [Description("30 минут")]
        T30 = 30,
        
        [Description("1 час")]
        T60 = 60
    }
}