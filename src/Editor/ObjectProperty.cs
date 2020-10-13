using System.Collections.Generic;

namespace Editor
{
    /// <summary>    
    /// Простейший редактируемый объект - свойство.
    /// </summary>
    public class ObjectProperty : ITreeViewItem, IHelperItem
    {
        public virtual ImageIndexEnum ImageIndex
        {
            get
            {
                return ImageIndexEnum.NONE;
            }
        }

        /// <param name="name">Имя свойства.</param>
        /// <param name="value">Значение свойства.</param>
        /// <param name="defaultValue">стандартное значение</param>
        public ObjectProperty(string name, object value,
            object defaultValue = null)
        {
            this.name = name;
            this.value = value;

            if (defaultValue == null)
            {
                this.defaultValue = "";
            }
            else
            {
                this.defaultValue = defaultValue;
            }

            needDisable = false;
        }

        /// <summary>
        /// Копия объекта
        /// </summary>
        /// <returns></returns>
        public ObjectProperty Clone()
        {
            return (ObjectProperty)MemberwiseClone();
        }

        /// <summary>
        /// Установить значение свойства
        /// </summary>
        /// <param name="val">Значение</param>
        public void SetValue(object val)
        {
            value = val;
        }

        /// <summary>
        /// Значение свойства
        /// </summary>
        public string Value
        {
            get
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// Имя свойства
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Имя картинки
        /// </summary>
        public string ImageName
        {
            get
            {
                return "Свойство";
            }
        }

        public string DefaultValue
        {
            get
            {
                return defaultValue.ToString();
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
                if (value.ToString() == defaultValue.ToString())
                {
                    return new string[] { name, 
                        StaticHelper.CommonConst.StubForParameters };
                }
                else
                {
                    return new string[] { name, value.ToString() };
                }
            }
        }

        public virtual string[] EditText
        {
            get
            {
                if (value is double)
                {
                    var provider = new System.Globalization.NumberFormatInfo();
                    provider.NumberDecimalSeparator = ".";

                    double v = (double)value;
                    return new string[] { "", 
                        string.Format( provider, "{0:0.##}", v ) };
                }

                return new string[] { "", value.ToString() };
            }
        }

        public virtual bool IsDeletable
        {
            get
            {
                return true;
            }
        }

        public virtual bool Delete(object child)
        {
            if (value.GetType() == defaultValue.GetType())
            {
                return SetNewValue(DefaultValue);
            }

            return false;
        }

        public virtual bool IsCopyable
        {
            get
            {
                return false;
            }
        }

        public virtual object Copy()
        {
            if(IsCopyable)
            {
                return this;
            }
            else
            {
                return null;
            }
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
                        value = System.Convert.ToInt32(newValue);
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

        public virtual bool SetNewValue(SortedDictionary<int, 
            List<int>> newDict)
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

        public virtual bool IsBoolParameter
        {
            get
            {
                return false;
            }
        }

        public bool IsMainObject
        {
            get
            {
                return false;
            }
        }

        public bool IsMode
        {
            get
            {
                return false;
            }
        }

        public bool ShowWarningBeforeDelete
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Установка родителя для элемента
        /// </summary>
        /// <param name="parent">Родительский элемент</param>
        public void AddParent(ITreeViewItem parent)
        {
            Parent = parent;
            if (Items != null)
            {
                foreach (ITreeViewItem item in Items)
                {
                    item.AddParent(this);
                }
            }
        }

        /// <summary>
        /// Нужно ли отключить элемент
        /// </summary>
        /// <returns></returns>
        public virtual bool NeedDisable
        {
            get
            {
                return needDisable;
            }
            set
            {
                needDisable = value;
            }
        }

        /// <summary>
        /// Заполнен или нет элемент дерева.
        /// True - отображать
        /// False - скрывать
        /// </summary>
        public virtual bool IsFilled
        {
            get
            {
                return true;
            }
        }

        public virtual bool ContainsBaseObject
        {
            get
            {
                return false;
            }
        }

        public virtual List<string> BaseObjectsList
        {
            get
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Отключено или нет свойство
        /// </summary>
        public bool Disabled { get; set; }

        public bool MarkToCut { get; set; }

        public ITreeViewItem Cut(ITreeViewItem item)
        {
            return null;
        }

        public  bool IsCuttable { get; } = false;
        #endregion

        #region реализация IHelperItem
        /// <summary>
        /// Получить ссылку на страницу с справкой
        /// </summary>
        /// <returns></returns>
        public virtual string GetLinkToHelpPage()
        {
            return null;
        }
        #endregion

        ITreeViewItem parent;
        private string name;  ///Имя свойства.
        private object value; ///Значение свойства.
        private object defaultValue; ///Стандартное значение

        private bool needDisable;
    }
}
