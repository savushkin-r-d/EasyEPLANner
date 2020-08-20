﻿using System.Collections.Generic;

namespace Editor
{
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

        /// <summary>
        /// Установка нового значения после редактирования
        /// </summary>
        /// <param name="newValue">Новое значение</param>
        /// <param name="isExtraValue">Является ли свойство расширенным</param>
        /// <returns></returns>
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
        /// <param name="devTypes">Типы устройств, допустимые для 
        /// редактирования.</param>
        /// <param name="devSubTypes">Подтипы устройств, допустимые для 
        /// редактирования.</param>
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

        /// <summary>
        /// Получение списка базовых объектов для тех объектов, 
        /// которые имеют такой функционал внутри себя.
        /// </summary>
        List<string> BaseObjectsList
        {
            get;
        }

        /// <summary>
        /// Имеет ли объект базовые объект.
        /// </summary>
        bool ContainsBaseObject
        {
            get;
        }

        /// <summary>
        /// Является ли элемент булевым параметром.
        /// </summary>
        bool IsBoolParameter
        {
            get;
        }

        /// <summary>
        /// Является ли этот объект главным (начальным).
        /// </summary>
        bool IsMainObject
        {
            get;
        }

        /// <summary>
        /// Необходимость обновить главный объект дерева.
        /// </summary>
        bool NeedRebuildMainObject
        {
            get;
        }

        /// <summary>
        /// Отображать предупреждение перед удалением
        /// </summary>
        bool ShowWarningBeforeDelete
        {
            get;
        }

        /// <summary>
        /// Установка родителя для элемента
        /// </summary>
        /// <param name="parent">Родительский элемент</param>
        void AddParent(ITreeViewItem parent);

        /// <summary>
        /// Нужно ли отключить элемент
        /// </summary>
        /// <returns></returns>
        bool NeedDisable { get; set; }

        /// <summary>
        /// Заполнен или нет элемент дерева
        /// True - отображать
        /// False - скрывать
        /// </summary>
        bool IsFilled { get; }

        /// <summary>
        /// Отключено или нет свойство
        /// </summary>
        bool Disabled { get; set; }
    }
}