using System.Collections.Generic;

namespace InterprojectExchange
{
    /// <summary>
    /// Модель межконтроллерного обмена текущего проекта.
    /// </summary>
    public interface ICurrentProjectModel : IProjectModel
    {
        /// <summary>
        /// Альтернативный проект, который выбран для обмена с текущим
        /// </summary>
        string SelectedAdvancedProject { get; set; }
    }
}