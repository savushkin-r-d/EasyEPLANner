# Руководство начинающего разработчика для EasyEplanner #

## Содержание ##
1-Технологический-стек-и-проекты

## 1. Технологический стек и проекты ##

Решение EasyEplanner.sln для Visual Studio  содержит в себе 5 проектов:
1. Aga.Controls - проект UI элемента управления.
2. EasyEPlanner - основной проект дополнения для Eplan.
3. EasyEplanner.Tests - проект с юнит-тестами для дополнения.
4. EplanIdleTimeModule - модуль простоя Eplan. Используется как отдельно, так и как часть EasyEPlanner.
5. ObjectListView2012 - проект UI элемента управления для редактора технологических объектов.

Все проекты написаны на языке C# с применением технологии .Net Framework версии 4.5 (Aga.Controls, ObjectListView2012), версии 4.7.2 (EasyEPlanner) и версии .Net Core 3.1 (EasyEplanner.Tests). Дополнительно в проекте EasyEPlanner используется скриптовый язык Lua.

Проекты 1, 4 и 5 используется в EasyEPlanner как подмодули (submodules).

### 1.1 Aga.Controls ###

### 1.2 EasyEPlanner ###

### 1.3 EasyEplanner.Tests ###

### 1.4 EplanIdleTimeModule ####

### 1.5 ObjectListView2012 ###

## 2. Файловая структура ##

Проекты Aga.Controls и ObjectListView2012

## 3. Сборка проекта и подписывание библиотек ##

### 3.1 Сборка через терминал ###

### 3.2 Сборка через Visual Studio ###

## Часто задаваемые вопросы ##