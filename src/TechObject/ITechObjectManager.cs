﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TechObject
{
    /// <summary>
    /// Интерфейс класса TechObjectManager
    /// </summary>
    public interface ITechObjectManager
    {
        /// <summary>
        /// Загрузка описания проекта из строки Lua
        /// </summary>
        /// <param name="LuaStr">Строка с описанием</param>
        /// <param name="projectName">Имя проекта</param>
        void LoadDescription(string LuaStr, string projectName);

        /// <summary>
        /// Загрузка ограничений проекта
        /// </summary>
        /// <param name="LuaStr">Описание ограничений</param>
        void LoadRestriction(string LuaStr);

        /// <summary>
        /// Сохранить описание проекта
        /// </summary>
        /// <param name="prefixStr">Отступ</param>
        /// <returns></returns>
        string SaveAsLuaTable(string prefixStr);

        /// <summary>
        /// Сохранить ограничения проекта
        /// </summary>
        /// <param name="prefixStr">Отступ</param>
        /// <returns></returns>
        string SaveRestrictionAsLua(string prefixStr);

        /// <summary>
        /// Глобальный список объектов
        /// </summary>
        List<TechObject> TechObjects { get; }

        /// <summary>
        /// Получить объект по его глобальному номеру
        /// </summary>
        /// <param name="globalNum">Глобальный номер</param>
        /// <returns></returns>
        TechObject GetTObject(int globalNum);

        /// <summary>
        /// Получить глобальный номер объекта
        /// </summary>
        /// <param name="techObject">Объект</param>
        /// <returns></returns>
        int GetTechObjectN(object techObject);

        /// <summary>
        /// Импорт объекта в проект.
        /// </summary>
        /// <param name="importingObject">Объект для импорта</param>
        void ImportObject(TechObject importingObject);

        /// <summary>
        /// Получить описание объектов для базы каналов
        /// </summary>
        /// <param name="rootNode">Дерево</param>
        /// <param name="cdbxTagView">Группировать тэги в один подтип</param>
        /// <param name="cdbxNewNames">Использовать имена объектов вместо OBJECT
        /// </param>
        void GetObjectForXML(TreeNode rootNode, bool cdbxTagView,
            bool cdbxNewNames);

        /// <summary>
        /// Вставить базовый объект в редактор
        /// </summary>
        /// <param name="luaName">Lua-имя объекта</param>
        void InsertBaseObject(string luaName);

        /// <summary>
        /// Количество аппаратов в проекте
        /// </summary>
        int UnitsCount { get; }

        /// <summary>
        /// Количество агрегатов в проекте
        /// </summary>
        int EquipmentModulesCount { get; }

        /// <summary>
        /// Проверка объектов
        /// </summary>
        /// <returns></returns>
        string Check();

        /// <summary>
        /// Синхронизация устройств в объектах;
        /// </summary>
        /// <param name="indexArray">Индексная таблица</param>
        void Synch(int[] indexArray);

        /// <summary>
        /// Проверка и исправление ограничений при удалении/перемещении объекта
        /// </summary>
        void CheckRestriction(int oldNum, int newNum);

        /// <summary>
        /// Изменение привязки объектов при удалении объекта из дерева
        /// </summary>
        /// <param name="deletedObjectNum">Номер удаленного объекта</param>
        void ChangeAttachedObjectsAfterDelete(int deletedObjectNum);

        /// <summary>
        /// Изменение привязки объектов при перемещении объекта по дереву
        /// </summary>
        /// <param name="newNum">Новый глобальный номер объекта</param>
        /// <param name="oldNum">Старый глобальный номер объекта</param>
        void ChangeAttachedObjectsAfterMove(int oldNum, int newNum);

        /// <summary>
        /// Удалить этот объект из привязки объектов
        /// </summary>
        /// <param name="techObject">Объект</param>
        void RemoveAttachingToObjects(TechObject techObject);

        /// <summary>
        /// Изменить базовый объект (первая реализация - сбросить)
        /// </summary>
        /// <param name="techObject">Базовый объект который надо изменить
        /// </param>
        /// <returns>Результат изменения, true - успех, false - нет</returns>
        bool ChangeBaseObject(TechObject techObject);
    }
}
