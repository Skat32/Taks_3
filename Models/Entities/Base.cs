using System;

namespace Models.Entities
{
    public class Base
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Дата создания записи
        /// </summary>
        public DateTime Created { get; set; }
        
        /// <summary>
        /// Дата изменения записи
        /// </summary>
        public DateTime Modified { get; set; }
        
        /// <summary>
        /// Признак удалена ли запись в базе 
        /// </summary>
        public bool IsDeleted { get; set; }

        public void Delete()
        {
            IsDeleted = true;
        }
    }
}