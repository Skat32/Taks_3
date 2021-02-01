using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Requests;

namespace Logic.Interfaces
{
    /// <summary>
    /// Интерфейс для воркера
    /// </summary>
    public interface ICurrencyConverterService
    {
        /// <summary>
        /// Получение курса от выбранной валюты к другой валюте
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task SaveCurseAsync(CurseRequest request);

        /// <summary>
        /// Получение курсов по выбранным валютам
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        Task SaveCursesAsync(IEnumerable<CurseRequest> requests);
    }
}