using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EplanDevice
{
    public interface ISetupTerminal
    {
        /// <summary>
        /// Установить свойства из привязки к терминалу
        /// </summary>
        /// <param name="terminal">Название</param>
        /// <param name="action">Название канала</param>
        /// <param name="clamp">Канал терминала</param>
        void SetupTerminal(string terminal, string action, int clamp);
    }
}
