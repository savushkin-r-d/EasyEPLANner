using System.Collections.Generic;
using System.Drawing;

namespace IO
{
    public interface IIONode
    {
        /// <summary>
        /// Модули ввода-вывода узла.
        /// </summary>
        List<IIOModule> IOModules { get; }

        /// <summary>
        /// Тип узла.
        /// </summary>
        IONode.TYPES Type { get; }

        /// <summary>
        /// Название узла ввода-вывода.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Полный номер узла.
        /// </summary>
        int NodeNumber { get; }

        /// <summary>
        /// IP-адрес.
        /// </summary>
        string IP { get; }

        /// <summary>
        /// Является ли узел каплером.
        /// </summary>
        bool IsCoupler { get; }

        /// <summary>
        /// Номер.
        /// </summary>
        int N { get; }

        /// <summary>
        /// Тип узла (строка).
        /// </summary>
        string TypeStr { get; }

        /// <summary>
        /// Количество дискретных входов.
        /// </summary>
        int DI_count { get; set; }

        /// <summary>
        /// Количество дискретных выходов.
        /// </summary>
        int DO_count { get; set; }

        /// <summary>
        /// Количество аналоговых входов.
        /// </summary>
        int AI_count { get; set; }

        /// <summary>
        /// Количество аналоговых выходов.
        /// </summary>
        int AO_count { get; set; }

        /// <summary>
        /// Расположение узла.
        /// </summary>
        string Location { get; }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        string SaveAsLuaTable(string prefix);
        void SaveAsConnectionArray(ref object[,] res, ref int idx, Dictionary<string, int> modulesCount,
            Dictionary<string, Color> modulesColor, int nodeIdx, Dictionary<string, object[,]> asInterfaceConnection);

        /// <summary>
        /// Добавление модуля в узел в заданную позицию.
        /// </summary>
        /// <param name="iOModule">Вставляемый модуль.</param>
        /// <param name="position">Позиция модуля, начиная с 1.</param>
        void SetModule(IIOModule iOModule, int position);

        /// <summary>
        /// Получение модуля.
        /// </summary>
        /// <param name="idx">Индекс модуля.</param>
        /// <returns>Модуль с заданным индексом.</returns>
        IIOModule this[int idx] { get; }
    }
}
