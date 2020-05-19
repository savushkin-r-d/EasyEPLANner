-- Шаблон файла, описывающего базовую функциональность.
-- В этом файле описан один пустой базовый объект, для примера.

-- Базовый объект:
-- 1. s88Level - уровень объекта по ISA-88 (1 - для юнитов, аппаратов. 2 - для агрегатов).
-- 2. name - русскоязычное название объекта.
-- 3. eplanName - LUA имя объекта.
-- 4. baseOperations - описание базовых операций объекта.
-- 5. basicName - англоязычное имя объекта, которое фигурирует в функциональности (add_functionality).
-- Это имя должно содержать имя без приписки "basic_"
-- 6. equipment - оборудование базового объекта.
-- 7. aggregateParameters - параметры объекта, который является агрегатом (которые будут добавлены
-- в аппарат, при привязке агрегата к аппарату).

-- Базовые операции:
-- 1. name - русскоязычное название операции.
-- 2. luaName - LUA имя операции.
-- 3. params - параметры операции, могут быть активными, пассивными, булевыми.
-- 4. steps - базовые шаги операции.

-- Активные параметры (active) - параметры, которые отображаются и сохраняются, имеют общую обработку,
-- которая характерна для всех таких параметров:
-- 1. luaName - LUA имя параметра.
-- 2. name - русскоязычное имя параметра.
-- 3. defaultValue - значение по-умолчанию (опционально).

-- Пассивные параметры (passive) - параметры, которые не отображаются и не сохраняются, имеют уникальную
-- обработку, вшитую внутри кода C#, из-за того, что эти параметры введены с целью автоматизации
-- и они не обрабатываются пользователем, только системой в автоматическом режиме:
-- 1. luaName - LUA имя параметра.
-- 2. name - русскоязычное имя параметра.

-- Булевые параметры (bool) - аналог активных параметров, только имеют два значения - да или нет:
-- 1. luaName - LUA имя параметра.
-- 2. name - русскоязычное имя параметра.
-- 3. defaultValue - значение по-умолчанию.

-- Базовые шаги - по аналогии с активными параметрами.
-- Оборудование - по аналогии с активными параметрами.
-- Параметры объекта, как агрегата - по аналогии с булевыми параметрами.

local base_tech_objects =
{
    { -- Автомат
        name = "Автомат",
        eplanName = "automat",
        s88Level = 2,
        baseOperations = { },
        basicName = "automat",
        equipment = { },
        aggregateParameters = { },
    }, 
    { -- Бачок откачки лёдводы
        name = "Бачок откачки лёдводы",
        eplanName = "_tank",
        s88Level = 2,
        baseOperations =
        {
            { -- Охлаждение
                name = "Охлаждение",
                luaName = "COOLING",
                params =
                {
                    bool =
                    {
                        { "ACTIVE_WORKING", "Активная работа", "false" },
                    },
                },
                steps =
                {
                    { "WAITING_HI_LS", "Ожидание появления ВУ" },
                    { "WAITING_LOW_LS", "Ожидание пропадания НУ" },
                },
            },
        },
        basicName = "ice_water_pump_tank",
        equipment =
        {
            { "M1", "Насос", "M1" },
            { "LS_up", "Датчик верхнего уровня", "LS2" },
            { "LS_down", "Датчик нижнего уровня", "LS1" },
        },
        aggregateParameters = { },
    },
    { -- Бачок откачки лёдводы ПИД
        name = "Бачок откачки лёдводы ПИД",
        eplanName = "_tank_PID",
        s88Level = 2,
        baseOperations =
        {
            { -- Охлаждение
                name = "Охлаждение",
                luaName = "COOLING",
                params =
                {
                    bool =
                    {
                        { "ACTIVE_WORKING", "Активная работа", "false" },
                    },
                },
                steps =
                {
                    { "WAITING_HI_LS", "Ожидание появления ВУ" },
                    { "WAITING_LOW_LS", "Ожидание пропадания НУ" },
                },
            },
        },
        basicName = "ice_water_pump_tank_PID",
        equipment =
        {
            { "M1", "Насос", "M1" },
            { "LS_up", "Датчик верхнего уровня", "LS2" },
            { "LS_down", "Датчик нижнего уровня", "LS1" },
            { "LT", "Датчик текущего уровня", "LT1" },
            { "SET_VALUE", "Задание", "" },
        },
        aggregateParameters = { },
    },
    { -- Бойлер
        name = "Бойлер",
        eplanName = "boil",
        s88Level = 2,
        baseOperations =
        {
            { -- Нагрев
                name = "Нагрев",
                luaName = "HEATING",
                params = { },
                steps =
                {
                    { "WAITING_LOW_LS", "Ожидание пропадания нижнего уровня" },
                    { "WATER_2_LOW_LS", "Наполнение до нижнего уровня" },
                    { "WATER_2_HI_LS", "Наполнение до верхнего уровня" },
                },
            },
        },
        basicName = "boiler",
        equipment = { },
        aggregateParameters =
        {
            { "BOILER", "Использовать бойлер", "false" },
        },
    },
    { -- Мастер
        name = "Мастер",
        eplanName = "master",
        s88Level = 1,
        baseOperations = { },
        basicName = "master",
        equipment = { },
        aggregateParameters = { },
    },
    { -- Линия
        name = "Линия",
        eplanName = "line",
        s88Level = 2,
        baseOperations =
        {
            { -- Мойка
                name = "Мойка",
                luaName = "WASHING_CIP",
                params =
                {
                    active =
                    {
                        { "CIP_WASH_END", "Мойка завершена" },
                        { "CIP_WAS_REQUEST", "Автоматическое включение мойки" },
                    },
                },
                steps =
                {
                    { "DRAINAGE", "Дренаж" },
                },
            },
            { -- Наполнение
                name = "Наполнение",
                luaName = "FILL",
                params = { },
                steps =
                {
                    { "IN_DRAINAGE", "В дренаж" },
                    { "IN_TANK", "В приемник" },
                    { "WAITING_KEY", "Ожидание" },
                },
            },
            { -- Выдача
                name = "Выдача",
                luaName = "OUT",
                params = { },
                steps =
                {
                    { "OUT_WATER", "Проталкивание" },
                    { "OUT_TANK", "Из источника" },
                    { "WAITING_KEY", "Ожидание" },
                },
            },
            { -- Работа
                name = "Работа",
                luaName = "WORK",
                params = { },
                steps =
                {
                    { "WAITING", "Ожидание" },
                    { "OUT_WATER", "Проталкивание" },
                    { "OUT_TANK", "Из источника" },
                    { "IN_TANK", "В приемник" },
                    { "IN_DRAINAGE", "В дренаж" },
                },
            },
        },
        basicName = "line",
        equipment = { },
        aggregateParameters = { },
    },
    { -- Линия приемки
        name = "Линия приемки",
        eplanName = "line_in",
        s88Level = 2,
        baseOperations =
        {
            { -- Мойка
                name = "Мойка",
                luaName = "WASHING_CIP",
                params =
                {
                    active =
                    {
                        { "CIP_WASH_END", "Мойка завершена" },
                        { "CIP_WAS_REQUEST", "Автоматическое включение мойки" },
                    },
                },
                steps =
                {
                    { "DRAINAGE", "Дренаж" },
                },
            },
            { -- Наполнение
                name = "Наполнение",
                luaName = "FILL",
                params = { },
                steps =
                {
                    { "IN_DRAINAGE", "В дренаж" },
                    { "IN_TANK", "В приемник" },
                    { "WAITING_KEY", "Ожидание" },
                },
            },
            { -- Работа
                name = "Работа",
                luaName = "WORK",
                params = { },
                steps =
                {
                    { "WAITING", "Ожидание" },
                    { "OUT_WATER", "Проталкивание" },
                    { "OUT_TANK", "Из источника" },
                    { "IN_TANK", "В приемник" },
                    { "IN_DRAINAGE", "В дренаж" },
                },
            },
        },
        basicName = "line",
        equipment = { },
        aggregateParameters = { },
    },
    { -- Линия выдачи
        name = "Линия выдачи",
        eplanName = "line_out",
        s88Level = 2,
        baseOperations =
        {
            { -- Мойка
                name = "Мойка",
                luaName = "WASHING_CIP",
                params =
                {
                    active =
                    {
                        { "CIP_WASH_END", "Мойка завершена" },
                        { "CIP_WAS_REQUEST", "Автоматическое включение мойки" },
                    },
                },
                steps =
                {
                    { "DRAINAGE", "Дренаж" },
                },
            },
            { -- Выдача
                name = "Выдача",
                luaName = "OUT",
                params = { },
                steps =
                {
                    { "OUT_WATER", "Проталкивание" },
                    { "OUT_TANK", "Из источника" },
                    { "WAITING_KEY", "Ожидание" },
                },
            },
            { -- Работа
                name = "Работа",
                luaName = "WORK",
                params = { },
                steps =
                {
                    { "WAITING", "Ожидание" },
                    { "OUT_WATER", "Проталкивание" },
                    { "OUT_TANK", "Из источника" },
                    { "IN_TANK", "В приемник" },
                    { "IN_DRAINAGE", "В дренаж" },
                },
            },
        },
        basicName = "line",
        equipment = { },
        aggregateParameters = { },
    },
    { -- Пастеризатор
        name = "Пастеризатор",
        eplanName = "pasteurizator",
        s88Level = 2,
        baseOperations =
        {
            { -- Мойка
                name = "Мойка",
                luaName = "WASHING_CIP",
                params =
                {
                    active =
                    {
                        { "CIP_WASH_END", "Мойка завершена" },
                        { "CIP_WAS_REQUEST", "Автоматическое включение мойки" },
                    },
                    passive =
                    {
                        { "DRAINAGE", "Номер шага дренаж" },
                    },
                },
                steps =
                {
                    { "DRAINAGE", "Дренаж" },
                },
            },
        },
        basicName = "pasteurizator",
        equipment = { },
        aggregateParameters = { },
    },
    { -- Пост
        name = "Пост",
        eplanName = "post",
        s88Level = 2,
        baseOperations = { },
        basicName = "post",
        equipment = { },
        aggregateParameters = { },
    },
    { -- Танк
        name = "Танк",
        eplanName = "tank",
        s88Level = 1,
        baseOperations =
        {
            { -- Мойка
                name = "Мойка",
                luaName = "WASHING_CIP",
                params =
                {
                    active =
                    {
                        { "CIP_WASH_END", "Мойка завершена" },
                        { "DI_CIP_FREE", "МСА свободна" },
                        { "CIP_WAS_REQUEST", "Автоматическое включение мойки" },
                    },
                    passive =
                    {
                        { "DRAINAGE", "Номер шага дренаж" },
                    },
                },
                steps =
                {
                    { "DRAINAGE", "Дренаж" },
                },
            },
            { -- Наполнение
                name = "Наполнение",
                luaName = "FILL",
                params =
                {
                    active =
                    {
                        { "OPERATION_AFTER_FILL", "Номер операции после наполнения" },
                    },
                },
                steps = { },
            },
            { -- Хранение
                name = "Хранение",
                luaName = "STORING",
                params = { },
                steps = { },
            },
            { -- Выдача
                name = "Выдача",
                luaName = "OUT",
                params =
                {
                    bool =
                    {
                        { "NEED_STORING_AFTER", "Включить хранение после выдачи", "true" },
                    },
                },
                steps = { },
            },
            { -- Томление
                name = "Томление",
                luaName = "SLOW_HEAT",
                params = { },
                steps =
                {
                    { "TO_START_TEMPERATURE", "Нагрев до заданной температуры" },
                    { "SLOW_HEAT", "Нагрев заданное время" },
                },
            },
            { -- работа
                name = "Работа",
                luaName = "WORK",
                params = { },
                steps =
                {
                    { "WAIT", "Ожидание" },
                    { "IN_TANK", "В танк" },
                    { "OUT_TANK", "Из танка" },
                },
            },
        },
        basicName = "tank",
        equipment =
        {
            { "hatch", "Датчик крышки люка", "GS1" },
            { "LS_up", "Датчик верхнего уровня", "LS2" },
            { "LS_down", "Датчик нижнего уровня", "LS1" },
            { "LT", "Датчик текущего уровня", "LT1" },
            { "TE","Датчик температуры", "TE1" },
        },
        aggregateParameters = { },
    },
    { -- Узел давления ПИД
        name = "Узел давления ПИД",
        eplanName = "pressure_node_PID",
        s88Level = 2,
        baseOperations =
        {
            { -- Работа
                name = "Работа",
                luaName = "WORK",
                params = { },
                steps = { },
            },
        },
        basicName = "pressure_node_PID",
        equipment =
        {
            { "M1", "Мотор", "M1" },
            { "PT", "Датчик давления", "PT1" },
            { "SET_VALUE", "Задание", "" },
        },
        aggregateParameters =
        {
            { "NEED_PRESSURE_CONTROL", "Использовать узел давления", "false" },
        },
    },
    { -- Узел подогрева
        name = "Узел подогрева",
        eplanName = "heater_node",
        s88Level = 2,
        baseOperations =
        {
            { -- Нагрев
                name = "Нагрев",
                luaName = "HEATING",
                params = { },
                steps =
                {
                    { "WORKING", "Работа" },
                    { "WAITING ", "Ожидание" },
                },
            },
        },
        basicName = "heater_node",
        equipment = { },
        aggregateParameters =
        {
            { "HEATER_NODE", "Использовать узел подогрева", "false" },
        },
    },
    { -- Узел подогрева ПИД
        name = "Узел подогрева ПИД",
        eplanName = "heater_node_PID",
        s88Level = 2,
        baseOperations =
        {
            { -- Нагрев
                name = "Нагрев",
                luaName = "HEATING",
                params = { },
                steps =
                {
                    { "WORKING", "Работа" },
                    { "WAITING ", "Ожидание" },
                },
            },
        },
        basicName = "heater_node_PID",
        equipment =
        {
            { "TE", "Датчик температуры", "TE1" },
            { "VC", "Регулирующий клапан", "VC1" },
            { "SET_VALUE", "Задание", "" },
        },
        aggregateParameters =
        {
            { "HEATER_NODE", "Использовать узел подогрева", "false" },
        },
    },
    { -- Узел расхода ПИД
        name = "Узел расхода ПИД",
        eplanName = "flow_node_PID",
        s88Level = 2,
        baseOperations =
        {
            { -- Работа
                name = "Работа",
                luaName = "WORKING",
                params = { },
                steps = { },
            },
        },
        basicName = "flow_node_PID",
        equipment =
        {
            { "FQT1", "Счетчик", "FQT1" },
            { "M1", "Насос", "M1" },
            { "SET_VALUE", "Задание", "" },
        },
        aggregateParameters =
        {
            { "NEED_FLOW_CONTROL", "Использовать узел расхода", "false" },
        },
    },
    { -- Узел охлаждения
        name = "Узел охлаждения",
        eplanName = "cooler_node",
        s88Level = 2,
        baseOperations =
        {
            { -- Охлаждение
                name = "Охлаждение",
                luaName = "COOLING",
                params = { },
                steps =
                {
                    { "WORKING", "Работа" },
                    { "WAITING ", "Ожидание" },
                },
            },
        },
        basicName = "cooler_node",
        equipment =
        {
            { "TE", "Датчик температуры", "TE1" },
        },
        aggregateParameters =
        {
            { "NEED_COOLING", "Использовать узел охлаждения", "false" },
        },
    },
    { -- Узел охлаждения ПИД
        name = "Узел охлаждения ПИД",
        eplanName = "cooler_node_PID",
        s88Level = 2,
        baseOperations =
        {
            { -- Охлаждение
                name = "Охлаждение",
                luaName = "COOLING",
                params = { },
                steps =
                {
                    { "WORKING", "Работа" },
                    { "WAITING ", "Ожидание" },
                },
            },
        },
        basicName = "cooler_node_PID",
        equipment =
        {
            { "TE", "Датчик температуры", "TE1" },
            { "VC", "Регулирующий клапан", "VC1" },
            { "SET_VALUE", "Задание", "" },
        },
        aggregateParameters = { },
    },
    { -- Узел перемешивания
        name = "Узел перемешивания",
        eplanName = "mix_node",
        s88Level = 2,
        baseOperations = { },
        basicName = "mix_node",
        equipment =
        {
            { "mix", "Мешалка", "M1" },
            { "bar", "Датчик решетки люка", "GS2" },
            { "hatch", "Датчик крышки люка", "GS1" },
            { "LT", "Датчик текущего уровня", "LT1" },
        },
        aggregateParameters = { },
    },
}

return base_tech_objects