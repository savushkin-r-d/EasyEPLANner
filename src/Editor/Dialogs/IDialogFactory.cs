using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TechObject;

namespace Editor
{
    public interface IDialogFactory
    {
        /// <summary>
        /// Получить диалог создания элемента для отоброжения
        /// </summary>
        /// <typeparam name="TRslt">Результат диалого</typeparam>
        /// <typeparam name="TBase">Базовый тип для получения диалога</typeparam>
        /// <typeparam name="TArg">Тип аргумента передаваемый в диалог</typeparam>
        IInsertDialog<TRslt, TArg> GetInsertDialog<TRslt, TBase, TArg>();
    }

    public class DialogFactory(Control parent) : IDialogFactory
    {
        public IInsertDialog<TRslt, TArg> GetInsertDialog<TRslt, TBase, TArg>() => typeof(TBase) switch
        {
            var T when T == typeof(Mode) => new StatesCreatorDialog(parent) as IInsertDialog<TRslt, TArg>,
            _ => null,
        };
    }
}
