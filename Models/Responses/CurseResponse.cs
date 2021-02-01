using Models.Enums;

namespace Models.Responses
{
    public class CurseResponse
    {
        /// <summary>
        /// Ключ валют
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Период агрегации
        /// </summary>
        public PeriodEnum PeriodEnum { get; set; }

        /// <summary>
        /// Максимальное значение
        /// </summary>
        public decimal MaxValue { get; set; }

        /// <summary>
        /// Минимальное значение
        /// </summary>
        public decimal MinValue { get; set; }

        /// <summary>
        /// Первое значение
        /// </summary>
        public decimal FirstValue { get; set; }

        /// <summary>
        /// Последнее значение
        /// </summary>
        public decimal LastValue { get; set; }
    }
}