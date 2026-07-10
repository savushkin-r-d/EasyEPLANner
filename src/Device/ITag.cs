using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EplanDevice
{
    /// <summary>
    /// Тег: тег, параметр, свойство...
    /// </summary>
    public interface ITag
    {
        /// <summary>
        /// Название
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Описание
        /// </summary>
        string Description { get; }
    }
}
