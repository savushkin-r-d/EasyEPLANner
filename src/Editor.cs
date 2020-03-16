using System.Collections.Generic;
///@file Editor.cs
///@brief Классы, реализующие минимальную функциональность, необходимую для 
///редактирования описания операций технологических объектов.
///
/// @author  Иванюк Дмитрий Сергеевич.
///
/// @par Текущая версия:
/// @$Rev: --- $.\n
/// @$Author: sedr $.\n
/// @$Date:: 2019-10-21#$.
/// 

/// Описание механизма редактирования технологических объектов.
/// В целях упрощения данного механизма используется представление данных в 
/// виде дерева. Каждый узел дерева связан с объектом, реализующий интерфейс 
/// ITreeViewItem, в котором определены методы (свойства), реализующие 
/// отображение, редактирование, удаление, добавление, копирование, вставку.
/// Класс Editor реализует данную функциональность 
/// (Editor.Editor.Edit(ITreeViewItem корень_дерева)). 
/// 
/// 1.Отображение - строковое представление в виде узла дерева. Используется
/// свойство ITreeViewItem.Name.
///
/// 2.Редактирование (двойной клик левой кнопкой мышки) - редактирование 
/// строкового значения узла дерева. Редактируются определенные узлы  
/// (ITreeViewItem.Editable). При редактировании отображается другой текст 
/// (ITreeViewItem.EditText). Отредактированный текст обрабатывается объектом 
/// (ITreeViewItem.SetNewValue( string )) и обновляется отображение текста 
/// узла (ITreeViewItem.Name).
///
/// 3.Удаление (клавиша Del на удаляемом элементе). Действует для
/// определенных узлов (ITreeViewItem.Deletable). Реализуется методом
/// bool ITreeViewItem.Delete( object child ). Вызывается для родительского 
/// узла. Если он возвращает true, данный узел удаляется. Потом обновляется
/// отображение всех дочерних узлов (ITreeViewItem.Name).
///
/// 4.Добавление (клавиша Ins на узле, куда хотим вставить элемент). Действует 
/// для определенных узлов (ITreeViewItem.Insertable). Реализуется методом
/// ITreeViewItem ITreeViewItem.Insert(). Потом добавляется новый узел 
/// дерева для вставленного элемента.
/// 
/// 5. Копирование (Ctrl + C). Действует для определенных узлов 
/// (ITreeViewItem.Copyable). Реализуется методом
/// ITreeViewItem ITreeViewItem.Copy(). Копируемый узел сохраняется в буферной
/// переменной.
/// 
/// 6. Вставка (Ctrl + V). Действует для определенных узлов 
/// (ITreeViewItem.InsertableCopy). Реализуется методом
/// ITreeViewItem ITreeViewItem.InsertCopy( object obj ). Узел из буферной
/// переменной вставляется в дерево.
/// 
/// 7. Отображение на странице Eplan'a - подсветка устройств.
/// Действует для определенных узлов. Используется свойство 
/// ITreeViewItem.IsDrawOnEplanPage.
/// 
/// 8. Получение списка устройств для отображения на странице Eplan'a - 
/// подсветка устройств (открываемые устройства - зеленый прямоугольник на
/// заднем фоне, закрываемые - красный, нижние седла - нижняя половина зеленого
/// прямоугольника, верхние седла - верхняя половина зеленого прямоугольника).
/// Действует для определенных узлов. Используется свойство 
/// ITreeViewItem.GetObjectToDrawOnEplanPage.
/// <summary>    
/// Пространство имен редактора технологических объектов.
/// </summary>
namespace Editor
{
    public struct DrawInfo
    {
        public DrawInfo(Style style, Device.Device dev)
        {
            this.style = style;
            this.dev = dev;
        }

        public void SetStyle(Style newStyle)
        {
            style = newStyle;
        }

        public enum Style
        {
            NO_DRAW,

            RED_BOX,
            GREEN_BOX,

            GREEN_UPPER_BOX,
            GREEN_LOWER_BOX,

            GREEN_RED_BOX,
        }

        public Device.Device dev;
        public Style style;
    };

    /// <summary>    
    /// Интерфейс редактируемого объекта.
    /// </summary>
    public interface ITreeViewItem
    {
        /// <summary>
        /// Получаем индекс той части или колонки, которую можем редактировать.
        /// </summary>
        int[] EditablePart
        {
            get;
        }

        /// <summary>    
        /// Получение имени, которое отображается на дереве.
        /// </summary>
        string[] DisplayText
        {
            get;
        }

        /// <summary>    
        /// Получение текста, который отображается на дереве при редактирования.
        /// </summary>
        string[] EditText
        {
            get;
        }

        /// <summary>    
        /// Признак возможности удаления.
        /// </summary>
        bool IsDeletable
        {
            get;
        }

        /// <summary>    
        /// Удаление узла.
        /// </summary>
        /// <param name="child">Удаляемый объект.</param>
        /// <returns>Удалять ли объект из дерева.</returns>
        bool Delete(object child);

        /// <summary>
        /// Признак возможности перемещения.
        /// </summary>
        bool IsMoveable
        {
            get;
        }

        /// <summary>    
        /// Перемещение узла вниз.
        /// </summary>
        /// <param name="child">Перемещаемый объект.</param>
        /// <returns>Перемещенный вниз объект.</returns>
        ITreeViewItem MoveDown(object child);

        /// <summary>    
        /// Перемещение узла вверх.
        /// </summary>
        /// <param name="child">Перемещаемый объект.</param>
        /// <returns>Перемещенный вверх объект.</returns>
        ITreeViewItem MoveUp(object child);

        /// <summary>    
        /// Признак возможности замены.
        /// </summary>
        bool IsReplaceable
        {
            get;
        }

        /// <summary>    
        /// Замена активного узла скопированным.
        /// </summary>
        /// <param name="child">Заменяемый объект.</param>
        /// <param name="copyObject">Ранее скопированный объект.</param>
        /// <returns>Новый вставленный объект.</returns> 
        ITreeViewItem Replace(object child, object copyObject);

        /// <summary>    
        /// Признак возможности копирования.
        /// </summary>
        bool IsCopyable
        {
            get;
        }

        /// <summary>    
        /// Копирование объекта.
        /// </summary>
        /// <returns>Копия объекта для последующей вставки.</returns> 
        object Copy();

        /// <summary>    
        /// Признак возможности вставки (ранее скопированного объекта).
        /// </summary>
        bool IsInsertableCopy
        {
            get;
        }

        /// <summary>    
        /// Вставка ранее скопированного объекта.
        /// </summary>
        ITreeViewItem InsertCopy(object obj);

        /// <summary>    
        /// Получение составляющих объектов.
        /// </summary>
        ITreeViewItem[] Items
        {
            get;
        }

        /// <summary>    
        /// Признак возможности редактирования.
        /// </summary>
        bool IsEditable
        {
            get;
        }

        /// <summary>    
        /// Установка нового значения после редактирования.
        /// </summary>
        ///  <param name="newValue">Новое значение после редактирования.</param>
        bool SetNewValue(string newValue);

        bool SetNewValue(SortedDictionary<int, List<int>> newDict);

        bool SetNewValue(string newValue, bool isExtraValue);

        /// <summary>    
        /// Признак возможности добавления.
        /// </summary>
        bool IsInsertable
        {
            get;
        }

        /// <summary>    
        /// Добавление элемента.
        /// </summary>
        /// <returns>Добавленный элемент. Может быть null, тогда ничего не 
        /// вставляется.</returns>
        ITreeViewItem Insert();

        /// <summary>    
        /// Признак возможности добавления устройств через список устройств.
        /// </summary>
        bool IsUseDevList
        {
            get;
        }

        /// <summary>    
        /// Признак возможности добавления ограничений.
        /// </summary>
        bool IsUseRestriction
        {
            get;
        }

        /// <summary>    
        /// Признак возможности добавления ограничений в текущем объекте.
        /// </summary>
        bool IsLocalRestrictionUse
        {
            get;
        }

        /// <summary>    
        /// Отображение на странице Eplan'a.
        /// </summary>
        bool IsDrawOnEplanPage
        {
            get;
        }

        /// <summary>    
        /// Получение списка устройств для отображения на странице Eplan'a.
        /// </summary>
        List<DrawInfo> GetObjectToDrawOnEplanPage();

        /// <summary>    
        /// Получение типов и подтипов устройства, которые должны отображаться 
        /// в списке устройств при редактировании.
        /// </summary>
        /// <param name="devTypes">Типы устройств, допустимые для редактирования.</param>
        /// <param name="devSubTypes">Подтипы устройств, допустимые для редактирования.</param>
        void GetDevTypes(out Device.DeviceType[] devTypes,
            out Device.DeviceSubType[] devSubTypes);

        /// <summary>    
        /// Признак необходимости обновления отображения родительского узла.
        /// </summary>
        bool NeedRebuildParent
        {
            get;
        }

        /// <summary>
        /// Получение и установка родительского элемента.
        /// </summary>
        ITreeViewItem Parent
        {
            get;
            set;
        }

    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>    
    /// Простейший редактируемый объект - свойство.
    /// </summary>
    public class ObjectProperty : ITreeViewItem
    {
        /// <summary>    
        /// Получение индекса иконки объекта по названию. 
        /// </summary>
        ///  <param name="obj">Объект.</param>
        static public int GetImageIndex(object obj)
        {
            int res = 100;
            switch (obj.GetType().Name)
            {
                case "TechObjectManager":
                    res = 0;
                    break;

                case "TechObject":
                    res = 1;
                    break;

                case "ModesManager":
                    res = 2;
                    break;

                case "Mode":
                    res = 3;
                    break;

                case "Step":
                    res = 4;
                    break;

                case "Action":
                    TechObject.Action action = obj as TechObject.Action;
                    switch (action.stepName)
                    {
                        case "Включать":
                            res = 5;
                            break;

                        case "Выключать":
                            res = 6;
                            break;

                        case "Сигналы для включения":
                            res = 7;
                            break;

                        case "Мойка ( DI, DO, устройства)":
                            res = 10;
                            break;

                        default:
                            break;
                    }
                    break;

                case "Action_WashSeats":
                    TechObject.Action_WashSeats actionWash =
                        obj as TechObject.Action_WashSeats;
                    switch (actionWash.stepName)
                    {
                        case "Верхние седла":
                            res = 8;
                            break;

                        case "Нижние седла":
                            res = 9;
                            break;

                        default:
                            break;
                    }
                    break;

                case "ActionWash":
                    res = 11;
                    break;

                case "Action_DI_DO":
                    res = 10;
                    break;

                case "ParamsManager":
                    res = 12;
                    break;

                case "TimersManager":
                    res = 13;
                    break;

                default:
                    break;
            }

            return res;
        }

        /// <param name="name">Имя свойства.</param>
        /// <param name="value">Значение свойства.</param>
        /// <param name="level">Уровень вложенности (для отображения в дереве).</param>        
        public ObjectProperty(string name, object value)
        {
            this.name = name;
            this.value = value;
        }

        public ObjectProperty Clone()
        {
            return (ObjectProperty)MemberwiseClone();
        }

        public void SetValue(object val)
        {
            value = val;
        }

        public string Value
        {
            get
            {
                return value.ToString();
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        #region Реализация ITreeViewItem

        public ITreeViewItem Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

        public virtual string[] DisplayText
        {
            get
            {
                return new string[] { name, value.ToString() };
            }
        }

        public string ImageName
        {
            get
            {
                return "свойство";
            }
        }

        public virtual string[] EditText
        {
            get
            {
                if (value is System.Double)
                {
                    System.Globalization.NumberFormatInfo provider =
                        new System.Globalization.NumberFormatInfo();
                    provider.NumberDecimalSeparator = ".";

                    double v = (double)value;
                    return new string[] { "",
                        System.String.Format( provider, "{0:0.##}", v ) };
                }

                return new string[] { "", value.ToString() };
            }
        }

        public bool IsDeletable
        {
            get
            {
                return false;
            }
        }

        public bool Delete(object child)
        {
            return false;
        }

        public bool IsCopyable
        {
            get
            {
                return false;
            }
        }

        public object Copy()
        {
            return null;
        }

        public bool IsMoveable
        {
            get
            {
                return false;
            }
        }

        public ITreeViewItem MoveDown(object child)
        {
            return null;
        }

        public ITreeViewItem MoveUp(object child)
        {
            return null;
        }

        public bool IsInsertableCopy
        {
            get
            {
                return false;
            }
        }

        public ITreeViewItem InsertCopy(object obj)
        {
            return null;
        }

        public virtual bool IsReplaceable
        {
            get
            {
                return false;
            }
        }

        public virtual ITreeViewItem Replace(object child, object copyObject)
        {
            return null;
        }

        public ITreeViewItem[] Items
        {
            get
            {
                return null;
            }
        }

        public int[] EditablePart
        {
            get
            {
                //Можем редактировать содержимое второй колонки.
                return new int[] { -1, 1 };
            }
        }

        public virtual bool IsEditable
        {
            get
            {
                return true;
            }
        }

        public virtual bool SetNewValue(string newValue)
        {           
            bool res = false;

            switch (value.GetType().Name)
            {
                case "String":
                    value = newValue;
                    res = true;
                    break;

                case "Int32":
                case "Int16":
                    try
                    {
                        value = System.Convert.ToInt16(newValue);
                        res = true;
                    }
                    catch (System.Exception)
                    {
                    }
                    break;

                case "Double":
                    try
                    {
                        value = System.Convert.ToDouble(newValue);
                        res = true;
                    }
                    catch (System.Exception)
                    {
                    }
                    break;
            }

            return res;
        }

        public virtual bool SetNewValue(SortedDictionary<int, List<int>> newDict)
        {
            bool res = false;
            return res;
        }

        public virtual bool SetNewValue(string newValue, bool isExtraValue)
        {
            bool res = false;
            return res;
        }

        public bool IsInsertable
        {
            get
            {
                return false;
            }
        }

        public ITreeViewItem Insert()
        {
            return null;
        }

        public virtual bool IsUseDevList
        {
            get
            {
                return false;
            }
        }

        public bool IsUseRestriction
        {
            get
            {
                return false;
            }
        }

        public bool IsLocalRestrictionUse
        {
            get
            {
                return false;
            }
        }

        public bool IsDrawOnEplanPage
        {
            get { return false; }
        }

        public List<DrawInfo> GetObjectToDrawOnEplanPage()
        {
            return null;
        }

        public void GetDevTypes(out Device.DeviceType[] devTypes,
            out Device.DeviceSubType[] devSubTypes)
        {
            devTypes = null;
            devSubTypes = null;
            return;
        }

        public virtual bool NeedRebuildParent
        {
            get { return false; }
        }
        #endregion

        ITreeViewItem parent;
        private string name;  ///Имя свойства.
        private object value; ///Значение свойства.
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    public abstract class TreeViewItem : ITreeViewItem
    {
        private ITreeViewItem parent;

        public ITreeViewItem Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

        public int GetObjType()
        {
            if (this.GetType().FullName == "TechObject.TechObject")
            {
                return (this as TechObject.TechObject).TechType;
            }
            else
            {
                return -1;
            }
        }

        #region Реализация ITreeViewItem
        virtual public string[] DisplayText
        {
            get
            {
                return new string[] { "", "" };
            }
        }

        virtual public ITreeViewItem[] Items
        {
            get
            {
                return null;
            }
        }

        virtual public bool SetNewValue(string newName)
        {
            return false;
        }

        virtual public bool SetNewValue(SortedDictionary<int, List<int>> newDict)
        {
            return false;
        }

        virtual public bool SetNewValue(string newVal, bool isExtraValue)
        {
            return false;
        }

        virtual public bool IsEditable
        {
            get
            {
                return false;
            }
        }

        virtual public int[] EditablePart
        {
            get
            {
                return new int[] { -1, -1 };
            }
        }

        virtual public string[] EditText
        {
            get
            {
                return new string[] { "", "" };
            }
        }

        virtual public bool IsDeletable
        {
            get
            {
                return false;
            }
        }

        virtual public bool IsCopyable
        {
            get
            {
                return false;
            }
        }

        virtual public object Copy()
        {
            return null;
        }

        virtual public bool IsMoveable
        {
            get
            {
                return false;
            }
        }

        virtual public bool IsReplaceable
        {
            get
            {
                return false;
            }
        }

        virtual public ITreeViewItem MoveDown(object child)
        {
            return null;
        }

        virtual public ITreeViewItem MoveUp(object child)
        {
            return null;
        }

        virtual public bool IsInsertableCopy
        {
            get
            {
                return false;
            }
        }

        virtual public ITreeViewItem InsertCopy(object obj)
        {
            return null;
        }

        virtual public ITreeViewItem Replace(object child, object copyObject)
        {
            return null;
        }

        virtual public bool Delete(object child)
        {
            return false;
        }

        virtual public bool IsInsertable
        {
            get
            {
                return false;
            }
        }

        virtual public ITreeViewItem Insert()
        {
            return null;
        }

        virtual public bool IsUseDevList
        {
            get
            {
                return false;
            }
        }

        virtual public bool IsUseRestriction
        {
            get
            {
                return false;
            }
        }

        virtual public bool IsLocalRestrictionUse
        {
            get
            {
                return false;
            }
        }

        virtual public bool IsDrawOnEplanPage
        {
            get { return false; }
        }

        virtual public List<DrawInfo> GetObjectToDrawOnEplanPage()
        {
            return null;
        }

        virtual public void GetDevTypes(out Device.DeviceType[] devTypes,
            out Device.DeviceSubType[] devSubTypes)
        {
            devTypes = null;
            devSubTypes = null;
        }

        public bool NeedRebuildParent
        {
            get { return false; }
        }
        #endregion
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>    
    /// Передача данных после завершения редактирования.
    /// </summary>
    /// <param name="scriptLua"> Данные после редактирования.</param>
    public delegate void SaveDescr(string scriptLua);
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>    
    /// Интерфейс редактора.
    /// </summary>
    public interface IEditor
    {
        string Edit(ITreeViewItem data);
        bool IsShown();
        void CloseEditor();
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    /// <summary>    
    /// Класс, реализующий редактор.
    /// </summary>
    public class Editor : IEditor, System.IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                frm.Dispose();
            }
        }

        private Editor()
        {
        }

        /// <summary>    
        /// Вызов окна редактирования технологических объектов.
        /// </summary>        
        public string Edit(ITreeViewItem data)
        {
            if (frm.wasInit == false)
            {
                frm.Init(data);
                frm.wasInit = true;
            }

            frm.ShowDlg();

            return "";
        }

        public bool IsShown()
        {
            return frm.IsShown;
        }

        public void CloseEditor()
        {
            frm.CloseEditor();
        }

        /// <summary>
        /// Получение экземпляра класса.
        /// </summary>
        /// <returns>Единственный экземпляр класса.</returns>
        public static Editor GetInstance()
        {
            if (null == instance)
            {
                instance = new Editor();
                instance.frm = new EditorCtrl();
            }
            return instance;
        }

        public EditorCtrl EForm
        {
            get
            {
                return frm;
            }
        }

        private EditorCtrl frm;      ///Окно редактора.
        private static Editor instance; ///Экземпляр класса.
    }
}
