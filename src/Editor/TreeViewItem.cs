﻿using System.Collections.Generic;

namespace Editor
{
    /// <summary>
    /// Редактируемый объект - дерево элемента.
    /// </summary>
    public abstract class TreeViewItem : ITreeViewItem, IHelperItem
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

        virtual public bool SetNewValue(SortedDictionary<int, 
            List<int>> newDict)
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

        public virtual bool NeedRebuildParent
        {
            get { return false; }
        }

        public virtual List<string> BaseObjectsList
        {
            get
            {
                return new List<string>();
            }
        }

        public virtual bool ContainsBaseObject
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsBoolParameter
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsMainObject
        {
            get
            {
                return false;
            }
        }

        public virtual bool NeedRebuildMainObject
        {
            get
            {
                return false;
            }
        }

        public virtual bool ShowWarningBeforeDelete
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
            this.Parent = parent;
            if (this.Items != null)
            {
                foreach (ITreeViewItem item in this.Items)
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

        private bool needDisable = false;

        /// <summary>
        /// Заполнен или нет элемент дерева
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

        /// <summary>
        /// Отключено или нет свойство
        /// </summary>
        public bool Disabled { get; set; }
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
    }
}
