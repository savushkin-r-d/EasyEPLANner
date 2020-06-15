-- Шаблон файла, описывающего базовую функциональность.
-- В этом файле описан один пустой базовый объект, для примера.

-- Базовый объект (название таблицы - eplanName т.е LUA-имя объекта, пишется в любых регистрах):
-- 1. s88Level - уровень объекта по ISA-88 (1 - для юнитов, аппаратов. 2 - для агрегатов).
-- 2. name - русскоязычное название объекта.
-- 3. baseOperations - описание базовых операций объекта.
-- 4. basicName - англоязычное имя объекта, которое фигурирует в функциональности (add_functionality).
-- Это имя должно содержать имя без приписки "basic_"
-- 5. equipment - оборудование базового объекта.
-- 6. aggregateParameters - параметры объекта, который является агрегатом (которые будут добавлены
-- в аппарат, при привязке агрегата к аппарату).
-- 7. bindingName - имя агрегата, используемое при привязке его к аппарату (для аппаратов не обязательно).

-- Базовые операции (название таблицы - это Lua-имя операции, пишется в верхнем регистре):
-- 1. name - русскоязычное название операции.
-- 2. params - параметры операции, могут быть активными, пассивными, булевыми.
-- 3. steps - базовые шаги операции.

-- Активные параметры (active) - параметры, которые отображаются и сохраняются, имеют общую обработку,
-- которая характерна для всех таких параметров (название таблицы - это Lua-имя параметра,
-- пишется в верхнем регистре):
-- 1. name - русскоязычное имя параметра.
-- 2. defaultValue - значение по-умолчанию (опционально).

-- Булевые параметры (bool) - аналог активных параметров, только имеют два значения - да или нет
-- (название таблицы - это Lua-имя параметра, пишется в верхнем регистре):
-- 1. name - русскоязычное имя параметра.
-- 2. defaultValue - значение по-умолчанию.

-- Главный параметр агрегата (main) - аналог булевого параметра. Является главным управляющим
-- параметром агрегата (управляет доступностью параметров). Имеет только два значения - да или нет
-- (название таблицы - это Lua-имя параметра, пишется в верхнем регистре):
-- 1. name - русскоязычное имя параметра.
-- 2. defaultValue - значение по-умолчанию.

-- Базовые шаги - по аналогии с активными параметрами.
-- Оборудование - по аналогии с активными параметрами.
-- Параметры объекта, как агрегата - по аналогии с активными и булевскими параметрами и
-- главным параметром агрегата, он является обязательным для агрегата. main-параметр задается только один!

base_tech_objects = function()
return
    {
        automat = {
            name = "Автомат",
            s88Level = 2,
            baseOperations = { },
            basicName = "automat",
            equipment = { },
            aggregateParameters = { },
        },
        _tank = {
            name = "Бачок откачки лёдводы",
            s88Level = 2,
            baseOperations =
            {
                COOLING = {
                    name = "Охлаждение",
                    params =
                    {
                        bool =
                        {
                            ACTIVE_WORKING = { name = "Активная работа", defaultValue = "false" },
                        },
                    },
                    steps =
                    {
                        WAITING_HI_LS = { name = "Ожидание появления ВУ" },
                        WAITING_LOW_LS = { name = "Ожидание пропадания НУ" },
                    },
                },
            },
            basicName = "ice_water_pump_tank",
            equipment =
            {
                M1 = { name = "Насос", defaultValue = "M1" },
                LS_up = { name = "Датчик верхнего уровня", defaultValue = "LS2" },
                LS_down = { name = "Датчик нижнего уровня", defaultValue = "LS1" },
            },
            aggregateParameters = { },
            bindingName = "ice_water_pump_tank"
        },
        _tank_PID = {
            name = "Бачок откачки лёдводы ПИД",
            s88Level = 2,
            baseOperations =
            {
                COOLING = {
                    name = "Охлаждение",
                    params =
                    {
                        bool =
                        {
                            ACTIVE_WORKING = { name = "Активная работа", defaultValue = "false" },
                        },
                    },
                    steps =
                    {
                        WAITING_HI_LS = { name = "Ожидание появления ВУ" },
                        WAITING_LOW_LS = { name = "Ожидание пропадания НУ" },
                    },
                },
            },
            basicName = "ice_water_pump_tank_PID",
            equipment =
            {
                M1 = { name = "Насос", "M1" },
                LS_up = { name = "Датчик верхнего уровня", defaultValue = "LS2" },
                LS_down = { name = "Датчик нижнего уровня", defaultValue = "LS1" },
                LT = { name = "Датчик текущего уровня", defaultValue = "LT1" },
                SET_VALUE = { name = "Задание"},
            },
            aggregateParameters = { },
            bindingName = "ice_water_pump_tank"
        },
        boil = {
            name = "Бойлер",
            s88Level = 2,
            baseOperations =
            {
                HEATING = {
                    name = "Нагрев",
                    params = { },
                    steps =
                    {
                        WAITING_LOW_LS = { name = "Ожидание пропадания нижнего уровня" },
                        WATER_2_LOW_LS = { name = "Наполнение до нижнего уровня" },
                        WATER_2_HI_LS = { name = "Наполнение до верхнего уровня" },
                    },
                },
            },
            basicName = "boiler",
            equipment = { },
            aggregateParameters =
            {
                main =
                {
                    NEED_BOILER = { name = "Использовать бойлер", defaultValue = "false" },
                },
            },
            bindingName = "boiler"
        },
        master = {
            name = "Мастер",
            s88Level = 1,
            baseOperations = { },
            basicName = "master",
            equipment = { },
            aggregateParameters = { },
        },
        line = {
            name = "Линия",
            s88Level = 2,
            baseOperations =
            {
                WASHING_CIP = {
                    name = "Мойка",
                    params =
                    {
                        active =
                        {
                            CIP_WASH_END = { name = "Мойка завершена" },
                            CIP_WASH_REQUEST = { name = "Автоматическое включение мойки" },
                        },
                    },
                    steps =
                    {
                        DRAINAGE = { name = "Дренаж" },
                    },
                },
                FILL = {
                    name = "Наполнение",
                    params = { },
                    steps =
                    {
                        IN_DRAINAGE = { name = "В дренаж" },
                        IN_TANK = { name = "В приемник" },
                        WAITING_KEY = { name = "Ожидание" },
                    },
                },
                OUT = {
                    name = "Выдача",
                    params = { },
                    steps =
                    {
                        OUT_WATER = { name = "Проталкивание" },
                        OUT_TANK = { name = "Из источника" },
                        WAITING_KEY = { name = "Ожидание" },
                    },
                },
                WORK = {
                    name = "Работа",
                    params = { },
                    steps =
                    {
                        WAITING = { name = "Ожидание" },
                        OUT_WATER = { name = "Проталкивание" },
                        OUT_TANK = { name = "Из источника" },
                        IN_TANK = { name = "В приемник" },
                        IN_DRAINAGE = { name = "В дренаж" },
                    },
                },
            },
            basicName = "line",
            equipment = { },
            aggregateParameters = { },
        },
        line_in = {
            name = "Линия приемки",
            s88Level = 2,
            baseOperations =
            {
                WASHING_CIP = {
                    name = "Мойка",
                    params =
                    {
                        active =
                        {
                            CIP_WASH_END = { name = "Мойка завершена" },
                            CIP_WASH_REQUEST = { name = "Автоматическое включение мойки" },
                        },
                    },
                    steps =
                    {
                        DRAINAGE = { name = "Дренаж" },
                    },
                },
                FILL = {
                    name = "Наполнение",
                    params = { },
                    steps =
                    {
                        IN_DRAINAGE = { name = "В дренаж" },
                        IN_TANK = { name = "В приемник" },
                        WAITING_KEY = { name = "Ожидание" },
                    },
                },
                WORK = {
                    name = "Работа",
                    params = { },
                    steps =
                    {
                        WAITING = { name = "Ожидание" },
                        OUT_WATER = { name = "Проталкивание" },
                        OUT_TANK = { name = "Из источника" },
                        IN_TANK = { name = "В приемник" },
                        IN_DRAINAGE = { name = "В дренаж" },
                    },
                },
            },
            basicName = "line",
            equipment = { },
            aggregateParameters = { },
        },
        line_out = {
            name = "Линия выдачи",
            s88Level = 2,
            baseOperations =
            {
                WASHING_CIP = {
                    name = "Мойка",
                    params =
                    {
                        active =
                        {
                            CIP_WASH_END = { name = "Мойка завершена" },
                            CIP_WASH_REQUEST = { name = "Автоматическое включение мойки" },
                        },
                    },
                    steps =
                    {
                        DRAINAGE = { name = "Дренаж" },
                    },
                },
                OUT = {
                    name = "Выдача",
                    params = { },
                    steps =
                    {
                        OUT_WATER = { name = "Проталкивание" },
                        OUT_TANK = { name = "Из источника" },
                        WAITING_KEY = { name = "Ожидание" },
                    },
                },
                WORK = {
                    name = "Работа",
                    params = { },
                    steps =
                    {
                        WAITING = { name = "Ожидание" },
                        OUT_WATER = { name = "Проталкивание" },
                        OUT_TANK = { name = "Из источника" },
                        IN_TANK = { name = "В приемник" },
                        IN_DRAINAGE = { name = "В дренаж" },
                    },
                },
            },
            basicName = "line",
            equipment = { },
            aggregateParameters = { },
        },
        pasteurizator = {
            name = "Пастеризатор",
            s88Level = 2,
            baseOperations =
            {
                WASHING_CIP = {
                    name = "Мойка",
                    params =
                    {
                        active =
                        {
                            CIP_WASH_END = { name = "Мойка завершена" },
                            CIP_WASH_REQUEST = { name = "Автоматическое включение мойки" },
                        },
                    },
                    steps =
                    {
                        DRAINAGE = { name = "Дренаж" },
                    },
                },
            },
            basicName = "pasteurizator",
            equipment = { },
            aggregateParameters = { },
        },
        post = {
            name = "Пост",
            s88Level = 2,
            baseOperations = { },
            basicName = "post",
            equipment = { },
            aggregateParameters = { },
        },
        tank = {
            name = "Танк",
            s88Level = 1,
            baseOperations =
            {
                WASHING_CIP = {
                    name = "Мойка",
                    params =
                    {
                        active =
                        {
                            CIP_WASH_END = { name = "Мойка завершена" },
                            DI_CIP_FREE = { name = "МСА свободна" },
                            CIP_WASH_REQUEST = { name = "Автоматическое включение мойки" },
                            MEDIUM_CHANGE_REQUEST = { name = "Запрос смены среды" },
                        },
                    },
                    steps =
                    {
                        DRAINAGE = { name = "Дренаж" },
                        MEDIUM_CHANGE = { name = "Смена среды" },
                    },
                },
                EMPTY_TANK_HEATING = {
                    name = "Нагрев пустого танка",
                },
                FILL = {
                    name = "Наполнение",
                    params =
                    {
                        active =
                        {
                            OPERATION_AFTER_FILL = { name = "Номер операции после наполнения" },
                        },
                    },
                    steps = { },
                },
                ADDITION_OF_STARTER = {
                    name = "Внесение закваски",
                },
                LEAVENING = {
                    name = "Заквашивание",
                },
                SOURING = {
                    name = "Сквашивание",
                },
                WHEY_SEPARATION_PRE = {
                    name = "Отделение сыворотки (нагрев)",
                },
                STORING = {
                    name = "Хранение",
                    params = { },
                    steps = { },
                },
                OUT = {
                    name = "Выдача",
                    params =
                    {
                        bool =
                        {
                            NEED_STORING_AFTER = { name = "Включить хранение после выдачи", defaultValue = "true" },
                        },
                    },
                    steps = { },
                },
                SLOW_HEATING = {
                    name = "Томление",
                    params = { },
                    steps =
                    {
                        TO_START_TEMPERATURE = { name = "Нагрев до заданной температуры" },
                        SLOW_HEATING = { name = "Нагрев заданное время" },
                    },
                },
                COOLING_BEFORE_LEAVENING = {
                    name = "Охлаждение перед заквашиванием",
                    params = { },
                    steps =
                    {
                        HOT_WATER_PUSHING = { name = "Вытеснение горячей воды" },
                        COOLING = { name = "Охлаждение" },
                        CHECKING_TEMPERATURE = { name = "Проверка заданной температуры" },
                    },
                },
                WORK = {
                    name = "Работа",
                    params = { },
                    steps =
                    {
                        WAIT = { name = "Ожидание" },
                        IN_TANK = { name = "В танк" },
                        OUT_TANK = { name = "Из танка" },
                    },
                },
            },
            basicName = "tank",
            equipment =
            {
                hatch = { name = "Датчик крышки люка", defaultValue = "GS1" },
                LS_up = { name = "Датчик верхнего уровня", defaultValue = "LS2" },
                LS_down = { name = "Датчик нижнего уровня", defaultValue = "LS1" },
                LT = { name = "Датчик текущего уровня", defaultValue = "LT1" },
                TE = { name = "Датчик температуры", defaultValue = "TE1" },
                -- out_pump defaultValue пустое т.к по другому происходит
                -- обработка ОУ. Обрабатывается не объект, а устройство.
                out_pump = { name = "Откачивающий насос" },
                lamp = { name = "Лампа освещения", defaultValue = "HL1" },
            },
            aggregateParameters = { },
        },
        pressure_node_PID = {
            name = "Узел давления ПИД",
            s88Level = 2,
            baseOperations =
            {
                WORKING = {
                    name = "Работа",
                    params =
                    {
                        active =
                        {
                            product_request = { name = "Запрос продукта" },
                        }
                    },
                    steps = { },
                },
            },
            basicName = "pressure_node_PID",
            equipment =
            {
                M1 = { name = "Мотор", defaultValue = "M1" },
                PT = { name = "Датчик давления", defaultValue = "PT1" },
                SET_VALUE = { name = "Задание", defaultValue = "" },
            },
            aggregateParameters =
            {
                main =
                {
                    NEED_PRESSURE_CONTROL = { name = "Использовать узел давления", defaultValue = "false" },
                },
            },
            bindingName = "pressure_node"
        },
        heater_node = {
            name = "Узел подогрева",
            s88Level = 2,
            baseOperations =
            {
                HEATING = {
                    name = "Нагрев",
                    params = { },
                    steps =
                    {
                        WORKING = { name = "Работа" },
                        WAITING = { name = "Ожидание" },
                    },
                },
            },
            basicName = "heater_node",
            equipment =
            {
                TE = { name = "Датчик температуры", defaultValue = "TE1" },
            },
            aggregateParameters =
            {
                main =
                {
                    NEED_HEATER_NODE = { name = "Использовать узел подогрева", defaultValue = "false" },
                },
            },
            bindingName = "heater_node"
        },
        heater_node_PID = {
            name = "Узел подогрева ПИД",
            s88Level = 2,
            baseOperations =
            {
                HEATING = {
                    name = "Нагрев",
                    luaName = "",
                    params = { },
                    steps =
                    {
                        WORKING = { name = "Работа" },
                        WAITING = { name = "Ожидание" },
                    },
                },
            },
            basicName = "heater_node_PID",
            equipment =
            {
                TE = { name = "Датчик температуры", defaultValue = "TE1" },
                VC = { name = "Регулирующий клапан", defaultValue = "VC1" },
                SET_VALUE = { name = "Задание"},
            },
            aggregateParameters =
            {
                main =
                {
                    NEED_HEATER_NODE = { name = "Использовать узел подогрева", defaultValue = "false" },
                },
            },
            bindingName = "heater_node"
        },
        flow_node_PID = {
            name = "Узел расхода ПИД",
            s88Level = 2,
            baseOperations =
            {
                WORKING = {
                    name = "Работа",
                    params = { },
                    steps = { },
                },
            },
            basicName = "flow_node_PID",
            equipment =
            {
                FQT1 = { name = "Счетчик", defaultValue = "FQT1" },
                M1 = { name = "Насос", defaultValue = "M1" },
                SET_VALUE = { name = "Задание"},
            },
            aggregateParameters =
            {
                main =
                {
                    NEED_FLOW_CONTROL = { name = "Использовать узел расхода", defaultValue = "false" },
                },
            },
            bindingName = "flow_node"
        },
        cooler_node = {
            name = "Узел охлаждения",
            s88Level = 2,
            baseOperations =
            {
                COOLING = {
                    name = "Охлаждение",
                    params = { },
                    steps =
                    {
                        WORKING = { name = "Работа" },
                        WAITING = { name = "Ожидание" },
                    },
                },
            },
            basicName = "cooler_node",
            equipment =
            {
                TE = { name = "Датчик температуры", defaultValue = "TE1" },
            },
            aggregateParameters =
            {
                main =
                {
                    NEED_COOLING = { name = "Использовать узел охлаждения", defaultValue = "false" },
                },
            },
            bindingName = "cooler_node"
        },
        cooler_node_PID = {
            name = "Узел охлаждения ПИД",
            s88Level = 2,
            baseOperations =
            {
                COOLING = {
                    name = "Охлаждение",
                    params = { },
                    steps =
                    {
                        WORKING = { name = "Работа" },
                        WAITING = { name = "Ожидание" },
                    },
                },
            },
            basicName = "cooler_node_PID",
            equipment =
            {
                TE = { name = "Датчик температуры", defaultValue = "TE1" },
                VC = { name = "Регулирующий клапан", defaultValue = "VC1" },
                SET_VALUE = { name = "Задание"},
            },
            aggregateParameters = { },
            bindingName = "cooler_node"
        },
        mix_node = {
            name = "Узел перемешивания",
            s88Level = 2,
            baseOperations =
            {
                MIXING = {
                    name = "Перемешивание",
                },
                MIXING_LEFT = {
                    name = "Перемешивание влево",
                },
                MIXING_RIGHT = {
                    name = "Перемешивание вправо",
                },
            },
            basicName = "mix_node",
            equipment =
            {
                mix = { name = "Мешалка", defaultValue = "M1" },
                bar = { name = "Датчик решетки люка", defaultValue = "GS2" },
                hatch = { name = "Датчик крышки люка", defaultValue = "GS1" },
                LT = { name = "Датчик текущего уровня", defaultValue = "LT1" },
            },
            aggregateParameters =
            {
                active =
                {
                    MIX_NODE_MIX_OPERATION = { name = "Используемая операция узла перемешивания", defaultValue = 1 },
                },
                main =
                {
                    NEED_MIXING = { name = "Использовать узел перемешивания", defaultValue = "true" },
                },
            },
            bindingName = "mix_node"
        },
        sterile_air_node = {
            name = "Узел стерильного воздуха",
            s88Level = 2,
            baseOperationg = { },
            basicName = "sterile_air_node",
            equipment = { },
            aggregateParameters = { },
            bindingName = "sterile_air_node",
        },
        steam_blast_node = {
            name = "Узел продувания",
            s88Level = 2,
            baseOperationg = { },
            basicName = "steam_blast_node",
            equipment = { },
            aggregateParameters = { },
            bindingName = "steam_blast_node",
        },
        in_tank_level_node = {
            name = "Узел текущего уровня (наполнение)",
            s88Level = 2,
            baseOperations =
            {
                WORKING = {
                    name = "Работа",
                },
            },
            basicName = "in_tank_level_node",
            equipment =
            {
                LT = { name = "Датчик текущего уровня", defaultValue = "LT1" },
                M1 = { name = "Насос (AO)", "M1" },
                SET_VALUE = { name = "Задание"},
            },
            bindingName = "in_tank_level_node"
        },
    }
end
