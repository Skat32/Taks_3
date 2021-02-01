using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Requests;
using Models.Responses;

namespace Logic.Interfaces
{
    /// <summary>
    /// Интерфейс для апи
    /// </summary>
    public interface ICurrencyService
    {
        /// <summary>
        /// Получаем список значений на вывод для выбраного курса
        /// </summary>
        /// <param name="request"> валюты </param>
        /// <returns></returns>
        Task<IEnumerable<CurseResponse>> GetCursesAsync(CurseRequest request);
    }
}