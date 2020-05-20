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

base_tech_objects = function()
return
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
                            { luaName = "ACTIVE_WORKING", name = "Активная работа", defaultValue = "false" },
                        },
                    },
                    steps =
                    {
                        { luaName = "WAITING_HI_LS", name = "Ожидание появления ВУ" },
                        { luaName = "WAITING_LOW_LS", name = "Ожидание пропадания НУ" },
                    },
                },
            },
            basicName = "ice_water_pump_tank",
            equipment =
            {
                { luaName = "M1", name = "Насос", defaultValue = "M1" },
                { luaName = "LS_up", name = "Датчик верхнего уровня", defaultValue = "LS2" },
                { luaName = "LS_down", name = "Датчик нижнего уровня", defaultValue = "LS1" },
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
                            { luaName = "ACTIVE_WORKING", name = "Активная работа", defaultValue = "false" },
                        },
                    },
                    steps =
                    {
                        { luaName = "WAITING_HI_LS", name = "Ожидание появления ВУ" },
                        { luaName = "WAITING_LOW_LS", name = "Ожидание пропадания НУ" },
                    },
                },
            },
            basicName = "ice_water_pump_tank_PID",
            equipment =
            {
                { luaName = "M1", name = "Насос", "M1" },
                { luaName = "LS_up", name = "Датчик верхнего уровня", defaultValue = "LS2" },
                { luaName = "LS_down", name = "Датчик нижнего уровня", defaultValue = "LS1" },
                { luaName = "LT", name = "Датчик текущего уровня", defaultValue = "LT1" },
                { luaName = "SET_VALUE", name = "Задание", defaultValue = "" },
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
                        { luaName = "WAITING_LOW_LS", name = "Ожидание пропадания нижнего уровня" },
                        { luaName = "WATER_2_LOW_LS", name = "Наполнение до нижнего уровня" },
                        { luaName = "WATER_2_HI_LS", name = "Наполнение до верхнего уровня" },
                    },
                },
            },
            basicName = "boiler",
            equipment = { },
            aggregateParameters =
            {
                { luaName = "BOILER", name = "Использовать бойлер", defaultValue = "false" },
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
                            { luaName = "CIP_WASH_END", name = "Мойка завершена" },
                            { luaName = "CIP_WAS_REQUEST", name = "Автоматическое включение мойки" },
                        },
                    },
                    steps =
                    {
                        { luaName = "DRAINAGE", name = "Дренаж" },
                    },
                },
                { -- Наполнение
                    name = "Наполнение",
                    luaName = "FILL",
                    params = { },
                    steps =
                    {
                        { luaName = "IN_DRAINAGE", name = "В дренаж" },
                        { luaName = "IN_TANK", name = "В приемник" },
                        { luaName = "WAITING_KEY", name = "Ожидание" },
                    },
                },
                { -- Выдача
                    name = "Выдача",
                    luaName = "OUT",
                    params = { },
                    steps =
                    {
                        { luaName = "OUT_WATER", name = "Проталкивание" },
                        { luaName = "OUT_TANK", name = "Из источника" },
                        { luaName = "WAITING_KEY", name = "Ожидание" },
                    },
                },
                { -- Работа
                    name = "Работа",
                    luaName = "WORK",
                    params = { },
                    steps =
                    {
                        { luaName = "WAITING", name = "Ожидание" },
                        { luaName = "OUT_WATER", name = "Проталкивание" },
                        { luaName = "OUT_TANK", name = "Из источника" },
                        { luaName = "IN_TANK", name = "В приемник" },
                        { luaName = "IN_DRAINAGE", name = "В дренаж" },
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
                            { luaName = "CIP_WASH_END", name = "Мойка завершена" },
                            { luaName = "CIP_WAS_REQUEST", name = "Автоматическое включение мойки" },
                        },
                    },
                    steps =
                    {
                        { luaName = "DRAINAGE", name = "Дренаж" },
                    },
                },
                { -- Наполнение
                    name = "Наполнение",
                    luaName = "FILL",
                    params = { },
                    steps =
                    {
                        { luaName = "IN_DRAINAGE", name = "В дренаж" },
                        { luaName = "IN_TANK", name = "В приемник" },
                        { luaName = "WAITING_KEY", name = "Ожидание" },
                    },
                },
                { -- Работа
                    name = "Работа",
                    luaName = "WORK",
                    params = { },
                    steps =
                    {
                        { luaName = "WAITING", name = "Ожидание" },
                        { luaName = "OUT_WATER", name = "Проталкивание" },
                        { luaName = "OUT_TANK", name = "Из источника" },
                        { luaName = "IN_TANK", name = "В приемник" },
                        { luaName = "IN_DRAINAGE", name = "В дренаж" },
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
                            { luaName = "CIP_WASH_END", name = "Мойка завершена" },
                            { luaName = "CIP_WAS_REQUEST", name = "Автоматическое включение мойки" },
                        },
                    },
                    steps =
                    {
                        { luaName = "DRAINAGE", name = "Дренаж" },
                    },
                },
                { -- Выдача
                    name = "Выдача",
                    luaName = "OUT",
                    params = { },
                    steps =
                    {
                        { luaName = "OUT_WATER", name = "Проталкивание" },
                        { luaName = "OUT_TANK", name = "Из источника" },
                        { luaName = "WAITING_KEY", name = "Ожидание" },
                    },
                },
                { -- Работа
                    name = "Работа",
                    luaName = "WORK",
                    params = { },
                    steps =
                    {
                        { luaName = "WAITING", name = "Ожидание" },
                        { luaName = "OUT_WATER", name = "Проталкивание" },
                        { luaName = "OUT_TANK", name = "Из источника" },
                        { luaName = "IN_TANK", name = "В приемник" },
                        { luaName = "IN_DRAINAGE", name = "В дренаж" },
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
                            { luaName = "CIP_WASH_END", name = "Мойка завершена" },
                            { luaName = "CIP_WAS_REQUEST", name = "Автоматическое включение мойки" },
                        },
                        passive =
                        {
                            { luaName = "DRAINAGE", name = "Номер шага дренаж" },
                        },
                    },
                    steps =
                    {
                        { luaName = "DRAINAGE", name = "Дренаж" },
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
                            { luaName = "CIP_WASH_END", name = "Мойка завершена" },
                            { luaName = "DI_CIP_FREE", name = "МСА свободна" },
                            { luaName = "CIP_WAS_REQUEST", name = "Автоматическое включение мойки" },
                        },
                        passive =
                        {
                            { luaName = "DRAINAGE", name = "Номер шага дренаж" },
                        },
                    },
                    steps =
                    {
                        { luaName = "DRAINAGE", name = "Дренаж" },
                    },
                },
                { -- Наполнение
                    name = "Наполнение",
                    luaName = "FILL",
                    params =
                    {
                        active =
                        {
                            { luaName = "OPERATION_AFTER_FILL", name = "Номер операции после наполнения" },
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
                            { luaName = "NEED_STORING_AFTER", name = "Включить хранение после выдачи", defaultValue = "true" },
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
                        { luaName = "TO_START_TEMPERATURE", name = "Нагрев до заданной температуры" },
                        { luaName = "SLOW_HEAT", name = "Нагрев заданное время" },
                    },
                },
                { -- работа
                    name = "Работа",
                    luaName = "WORK",
                    params = { },
                    steps =
                    {
                        { luaName = "WAIT", name = "Ожидание" },
                        { luaName = "IN_TANK", name = "В танк" },
                        { luaName = "OUT_TANK", name = "Из танка" },
                    },
                },
            },
            basicName = "tank",
            equipment =
            {
                { luaName = "hatch", name = "Датчик крышки люка", defaultValue = "GS1" },
                { luaName = "LS_up", name = "Датчик верхнего уровня", defaultValue = "LS2" },
                { luaName = "LS_down", name = "Датчик нижнего уровня", defaultValue = "LS1" },
                { luaName = "LT", name = "Датчик текущего уровня", defaultValue = "LT1" },
                { luaName = "TE", name = "Датчик температуры", defaultValue = "TE1" },
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
                { luaName = "M1", name = "Мотор", defaultValue = "M1" },
                { luaName = "PT", name = "Датчик давления", defaultValue = "PT1" },
                { luaName = "SET_VALUE", name = "Задание", defaultValue = "" },
            },
            aggregateParameters =
            {
                { luaName = "NEED_PRESSURE_CONTROL", name = "Использовать узел давления", defaultValue = "false" },
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
                        { luaName = "WORKING", name = "Работа" },
                        { luaName = "WAITING ", name = "Ожидание" },
                    },
                },
            },
            basicName = "heater_node",
            equipment = { },
            aggregateParameters =
            {
                { luaName = "HEATER_NODE", name = "Использовать узел подогрева", defaultValue = "false" },
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
                        { luaName = "WORKING", name = "Работа" },
                        { luaName = "WAITING ", name = "Ожидание" },
                    },
                },
            },
            basicName = "heater_node_PID",
            equipment =
            {
                { luaName = "TE", name = "Датчик температуры", defaultValue = "TE1" },
                { luaName = "VC", name = "Регулирующий клапан", defaultValue = "VC1" },
                { luaName = "SET_VALUE", name = "Задание", defaultValue = "" },
            },
            aggregateParameters =
            {
                { luaName = "HEATER_NODE", name = "Использовать узел подогрева", defaultValue = "false" },
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
                { luaName = "FQT1", name = "Счетчик", defaultValue = "FQT1" },
                { luaName = "M1", name = "Насос", defaultValue = "M1" },
                { luaName = "SET_VALUE", name = "Задание", defaultValue = "" },
            },
            aggregateParameters =
            {
                { luaName = "NEED_FLOW_CONTROL", name = "Использовать узел расхода", defaultValue = "false" },
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
                        { luaName = "WORKING", name = "Работа" },
                        { luaName = "WAITING ", name = "Ожидание" },
                    },
                },
            },
            basicName = "cooler_node",
            equipment =
            {
                { luaName = "TE", name = "Датчик температуры", defaultValue = "TE1" },
            },
            aggregateParameters =
            {
                { luaName = "NEED_COOLING", name = "Использовать узел охлаждения", defaultValue = "false" },
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
                        { luaName = "WORKING", name = "Работа" },
                        { luaName = "WAITING ", name = "Ожидание" },
                    },
                },
            },
            basicName = "cooler_node_PID",
            equipment =
            {
                { luaName = "TE", name = "Датчик температуры", defaultValue = "TE1" },
                { luaName = "VC", name = "Регулирующий клапан", defaultValue = "VC1" },
                { luaName = "SET_VALUE", name = "Задание", defaultValue = "" },
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
                { luaName = "mix", name = "Мешалка", defaultValue = "M1" },
                { luaName = "bar", name = "Датчик решетки люка", defaultValue = "GS2" },
                { luaName = "hatch", name = "Датчик крышки люка", defaultValue = "GS1" },
                { luaName = "LT", name = "Датчик текущего уровня", defaultValue = "LT1" },
            },
            aggregateParameters = { },
        },
    }
end