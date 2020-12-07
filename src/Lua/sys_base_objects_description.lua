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
-- 8. isPID - является ли объект ПИД-регулятором.
-- 9. useGroups - используются ли группы.

-- Базовые операции (baseOperations) (название таблицы - это Lua-имя операции, пишется в верхнем регистре):
-- 1. name - русскоязычное название операции.
-- 2. params - параметры операции, могут быть активными, пассивными, булевыми.
-- 3. steps - базовые шаги операции.
-- 4. defaultPosition - позиция операции при автоматической настройке, когда осуществляется вставка
-- объекта. Если поле отсутствует - операция не будет настраиваться автоматически.
-- Значения этого поля должны быть от 1 и не пересекаться (опционально).

-- Базовые шаги - шаг базовой операции (steps). Название таблицы - LUA-имя базового шага, в верхнем регистре.
-- Внутри таблицы указываются следующие поля:
-- 1. name = строковое обозначение имени шага.
-- 2. defaultPosition - по аналогии с defaultPosition в базовой операции, только относится
-- к базовому шагу (опционально).

-- Активные параметры (active) - параметры, которые отображаются и сохраняются, имеют общую обработку,
-- которая характерна для всех таких параметров (название таблицы - это Lua-имя параметра,
-- пишется в верхнем регистре):
-- 1. name - русскоязычное имя параметра.
-- 2. defaultValue - значение по-умолчанию (опционально).
-- 3. displayObjects - список параметров, указывающий, что будет отображаться в окне
-- графической настройки параметра (окно "Устройства"), при настройке параметра.
-- Таблица displayObjects может отсутствовать.
-- Допустимые свойства: signals - будут добавлены сигналы AO, AI, DO, DI;
-- parameters - будут добавлены все параметры объекта.
-- Пример: displayObjects = { "signals", "parameters" } - будут добавлены сигналы и параметры.

-- Булевые параметры (bool) - аналог активных параметров, только имеют два значения - да или нет
-- (название таблицы - это Lua-имя параметра, пишется в верхнем регистре):
-- 1. name - русскоязычное имя параметра.
-- 2. defaultValue - значение по-умолчанию.

-- Главный параметр агрегата (main) - аналог булевого параметра. Является главным управляющим
-- параметром агрегата (управляет доступностью параметров). Имеет только два значения - да или нет
-- (название таблицы - это Lua-имя параметра, пишется в верхнем регистре):
-- 1. name - русскоязычное имя параметра.
-- 2. defaultValue - значение по-умолчанию.

-- Параметр оборудования (equipment) - параметр для оборудования. Обработка схожая с действиями
-- в шагах операции. Этот тип параметра создается, когда заполняется equipment в объекте.
-- Создается таблице equipment. Имеет общую обработку,которая характерна для
-- всех таких параметров (название таблицы - это Lua-имя параметра,
-- пишется в верхнем регистре):
-- 1. name - русскоязычное имя параметра.
-- 2. defaultValue - значение по-умолчанию (опционально).

-- Оборудование - параметр оборудования.
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
            bindingName = "automat",
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
                        WAITING_HI_LS = {
                            name = "Ожидание появления ВУ",
                            defaultPosition = 1,
                        },
                        WAITING_LOW_LS = {
                            name = "Ожидание пропадания НУ",
                            defaultPosition = 2,
                        },
                    },
                    defaultPosition = 1,
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
                        WAITING_HI_LS = {
                            name = "Ожидание появления ВУ",
                            defaultPosition = 1,
                        },
                        WAITING_LOW_LS = {
                            name = "Ожидание пропадания НУ",
                            defaultPosition = 2,
                        },
                    },
                    defaultPosition = 1,
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
            bindingName = "ice_water_pump_tank",
            isPID = true
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
                        WAITING_LOW_LS = {
                            name = "Ожидание пропадания нижнего уровня",
                            defaultPosition = 1,
                        },
                        WATER_2_LOW_LS = {
                            name = "Наполнение до нижнего уровня",
                            defaultPosition = 2,
                        },
                        WATER_2_HI_LS = {
                            name = "Наполнение до верхнего уровня",
                            defaultPosition = 3,
                        },
                    },
                    defaultPosition = 1,
                },
            },
            basicName = "boiler",
            equipment = { },
            aggregateParameters =
            {
                active =
                {
                    HEATING_WATER_TEMPERATURE =
                    {
                        name = "Температура подогрева воды",
                        displayObjects = { "parameters" },
                    },
                },
                main =
                {
                    NEED_BOILER = { name = "Использовать бойлер", defaultValue = "false" },
                },
            },
            bindingName = "boiler"
        },
        master = {
            name = "Ячейка процесса",
            s88Level = 0, -- Не относится к s88Level, находится выше 1 уровня.
            baseOperations = { },
            basicName = "master",
            equipment = { },
            aggregateParameters = { },
            bindingName = "master"
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
                            CIP_WASH_END =
                            {
                                name = "Мойка завершена",
                                displayObjects = { "signals" },
                            },
                            CIP_WASH_REQUEST =
                            {
                                name = "Автоматическое включение мойки",
                                displayObjects = { "signals" },
                            },
                        },
                        bool =
                            {
                            IGNORE_WATER_FLAG = { name = "Игнорировать наличие продукта", defaultValue = "false" },
                            },
                    },
                    steps =
                    {
                        DRAINAGE = {
                            name = "Дренаж",
                            defaultPosition = 2,
                        },
                    },
                    defaultPosition = 1,
                },
                FILL = {
                    name = "Наполнение",
                    params = { },
                    steps =
                    {
                        IN_DRAINAGE = {
                            name = "В дренаж",
                            defaultPosition = 2,
                        },
                        IN_TANK = {
                            name = "В приемник",
                            defaultPosition = 3,
                        },
                        WAITING_KEY = {
                            name = "Ожидание",
                            defaultPosition = 1,
                        },
                    },
                    defaultPosition = 2,
                },
                OUT = {
                    name = "Выдача",
                    params = { },
                    steps =
                    {
                        OUT_WATER = {
                            name = "Проталкивание",
                            defaultPosition = 2,
                        },
                        OUT_TANK = {
                            name = "Из источника",
                            defaultPosition = 3,
                        },
                        WAITING_KEY = {
                            name = "Ожидание",
                            defaultPosition = 1,
                        },
                    },
                    defaultPosition = 3,
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
            equipment =
            {
                product_CTR = { name = "Счетчик", defaultValue = "FQT1" },
            },
            aggregateParameters = { },
            bindingName = "line",
            useGroups = true
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
                            CIP_WASH_END =
                            {
                                name = "Мойка завершена",
                                displayObjects = { "signals" },
                            },
                            CIP_WASH_REQUEST =
                            {
                                name = "Автоматическое включение мойки",
                                displayObjects = { "signals" },
                            },
                        },
                        bool =
                        {
                            IGNORE_WATER_FLAG = { name = "Игнорировать наличие продукта", defaultValue = "false" },
                        },
                    },
                    steps =
                    {
                        DRAINAGE =
                        {
                            name = "Дренаж",
                            defaultPosition = 2,
                        },
                    },
                    defaultPosition = 1,
                },
                FILL = {
                    name = "Наполнение",
                    params = { },
                    steps =
                    {
                        IN_DRAINAGE = {
                            name = "В дренаж",
                            defaultPosition = 2,
                        },
                        IN_TANK = {
                            name = "В приемник",
                            defaultPosition = 3,
                        },
                        WAITING_KEY = {
                            name = "Ожидание",
                            defaultPosition = 1,
                        },
                    },
                    defaultPosition = 2,
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
            equipment =
            {
                product_CTR = { name = "Счетчик", defaultValue = "FQT1" },
            },
            aggregateParameters = { },
            bindingName = "line_in",
            useGroups = true
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
                            CIP_WASH_END =
                            {
                                name = "Мойка завершена",
                                displayObjects = { "signals" },
                            },
                            CIP_WASH_REQUEST =
                            {
                                name = "Автоматическое включение мойки",
                                displayObjects = { "signals" },
                            },
                        },
                        bool =
                        {
                            IGNORE_WATER_FLAG = 
                            { 
                                name = "Игнорировать наличие продукта",
                                defaultValue = "false" 
                            },
                        },
                    },
                    steps =
                    {
                        DRAINAGE = {
                            name = "Дренаж",
                            defaultPosition = 2,
                        },
                    },
                    defaultPosition = 1,
                },
                OUT = {
                    name = "Выдача",
                    params = 
                    {
                        bool =
                        {
                            USE_VOLUME =
                            {
                                name = "Выдача линией заданного объема",
                                defaultValue = "false",
                            },
                            NEED_PAUSE_AFTER_EMPTY_LAST_TANK =
                            {
                                name = "Пауза после опустошения крайнего танка в очереди",
                                defaultValue = "false",
                            },
                        }
                    },
                    steps =
                    {
                        OUT_WATER = {
                            name = "Проталкивание",
                            defaultPosition = 2,
                        },
                        OUT_TANK = {
                            name = "Из источника",
                            defaultPosition = 3,
                        },
                        WAITING_KEY = {
                            name = "Ожидание",
                            defaultPosition = 1,
                        },
                    },
                    defaultPosition = 2,
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
            equipment =
            {
                product_CTR = { name = "Счетчик", defaultValue = "FQT1" },
                water_v = { name = "Клапан(-а) подачи воды" },
                QT = { name = "Концентратомер", defaultValue = "QT1" },
            },
            aggregateParameters = { },
            bindingName = "line_out",
            useGroups = true,
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
                            CIP_WASH_END =
                            {
                                name = "Мойка завершена",
                                displayObjects = { "signals" },
                            },
                            CIP_WASH_REQUEST =
                            {
                                name = "Автоматическое включение мойки",
                                displayObjects = { "signals" },
                            },
                        },
                    },
                    steps =
                    {
                        DRAINAGE = {
                            name = "Дренаж",
                            defaultPosition = 1,
                        },
                    },
                    defaultPosition = 1,
                },
            },
            basicName = "pasteurizator",
            equipment = { },
            aggregateParameters = { },
            bindingName = "pasteurizator"
        },
        post = {
            name = "Пост",
            s88Level = 2,
            baseOperations = { },
            basicName = "post",
            equipment = { },
            aggregateParameters = { },
            bindingName = "post"
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
                            CIP_WASH_END =
                            {
                                name = "Мойка завершена",
                                displayObjects = { "signals" },
                            },
                            DI_CIP_FREE =
                            {
                                name = "МСА свободна",
                                displayObjects = { "signals" },
                            },
                            CIP_WASH_REQUEST =
                            {
                                name = "Автоматическое включение мойки",
                                displayObjects = { "signals" },
                            },
                            MEDIUM_CHANGE_REQUEST =
                            {
                                name = "Запрос смены среды",
                                displayObjects = { "signals" },
                            },
                        },
                    },
                    steps =
                    {
                        DRAINAGE = {
                            name = "Дренаж",
                            defaultPosition = 3,
                        },
                        MEDIUM_CHANGE = { name = "Смена среды" },
                    },
                    defaultPosition = 1,
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
                            OPERATION_AFTER_FILL = { name = "Номер последующей операции" },
                        },
                    },
                    steps = { },
                    defaultPosition = 2,
                },
                ADDITION_OF_STARTER = {
                    name = "Внесение закваски",
                },
                LEAVENING = {
                    name = "Заквашивание",
                    params =
                    {
                        active =
                        {
                            MIXING_TIME =
                            {
                                name = "Время заквашивания",
                                displayObjects = { "parameters" },
                            },
                            OPERATION_AFTER = { name = "Номер последующей операции" },
                        },
                    },
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
                    defaultPosition = 3,
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
                    defaultPosition = 4,
                },
                SLOW_HEATING = {
                    name = "Томление",
                    params =
                    {
                        bool =
                        {
                            AUTO_COOLING_BEFORE_LEAVENING =
                            {
                                name = "Автоматический переход к охлаждению для заквашивания",
                                defaultValue = "true",
                            },
                        },
                        active =
                        {
                            BAKE_TIME =
                            {
                                name = "Время нагрева (2-го шага)",
                                displayObjects = { "parameters" },
                            },
                        },
                    },
                    steps =
                    {
                        TO_START_TEMPERATURE = { name = "Нагрев до заданной температуры" },
                        SLOW_HEATING = { name = "Нагрев заданное время" },
                    },
                },
                COOLING_BEFORE_LEAVENING = {
                    name = "Охлаждение перед заквашиванием",
                    params =
                        {
                            active =
                            {
                                MIXING_CHECK_TIME =
                                {
                                    name = "Время проверки температуры",
                                    displayObjects = { "parameters" },
                                },
                                OPERATION_AFTER = { name = "Номер следующей операции" },
                            },
                        },
                    steps =
                    {
                        COOLING = { name = "Охлаждение" },
                        CHECKING_TEMPERATURE = { name = "Проверка заданной температуры" },
                    },
                },
                COOLING_AFTER_SOURING = {
                    name = "Охлаждение после сквашивания",
                    params =
                        {
                            active =
                            {
                                COOLING_TIME =
                                {
                                    name = "Время охлаждения (1-го шага)",
                                    displayObjects = { "parameters" },
                                },
                                MIXING_CHECK_TIME =
                                {
                                    name = "Время проверки температуры (3-го шага)",
                                    displayObjects = { "parameters" },
                                },
                            },
                        },
                    steps =
                    {
                        COOLING = { name = "Охлаждение" },
                        MIXING = { name = "Охлаждение и перемешивание" },
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
                MIXING_AFTER_SOURING = {
                    name = "Перемешивание после сквашивания",
                    params = 
                    {
                        active =
                        {
                            OPERATION_AFTER =
                            {
                                name = "Номер следующей операции",
                            },
                            MIXING_AFTER_SOURING_TIME =
                            {
                                name = "Время операции",
                                displayObjects = { "parameters" },
                            }
                        }
                    }
                },
            },
            basicName = "tank",
            equipment =
            {
                hatch = { name = "Датчик крышки люка", defaultValue = "GS1" },
                hatch2 = { name = "Датчик крышки люка 2" },
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
                            product_request =
                            {
                                name = "Запрос продукта",
                                displayObjects = { "signals" },
                            }
                        },
                        bool =
                        {
                            is_reverse = { name = "Обратного (реверсивного) действия", defaultValue = "false" },
                            is_zero_start = { name = "Нулевое стартовое значение", defaultValue = "true" }
                        },
                    },
                    steps = { },
                    defaultPosition = 1,
                },
            },
            basicName = "pressure_node_PID",
            equipment =
            {
                M1 = { name = "Мотор", defaultValue = "M1" },
                PT = { name = "Датчик давления", defaultValue = "PT1" },
                PT2 = { name = "Датчик давления 2 (разностное задание)" },
                SET_VALUE = { name = "Задание", defaultValue = "" },
            },
            aggregateParameters =
            {
                main =
                {
                    NEED_PRESSURE_CONTROL = { name = "Использовать узел давления", defaultValue = "false" },
                },
            },
            bindingName = "pressure_node",
            isPID = true
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
                        WORKING = {
                            name = "Работа",
                            defaultPosition = 1,
                        },
                        WAITING = {
                            name = "Ожидание",
                            defaultPosition = 2,
                        },
                    },
                    defaultPosition = 1,
                },
            },
            basicName = "heater_node",
            equipment =
            {
                TE = { name = "Датчик температуры", defaultValue = "TE1" },
            },
            aggregateParameters =
            {
                active =
                {
                    HEATING_TEMPERATURE =
                    {
                        name = "Температура подогрева",
                        displayObjects = { "parameters" },
                    },
                    HEATING_TEMPERATURE_DELTA =
                    {
                        name = "Дельта температуры подогрева",
                        displayObjects = { "parameters" },
                    },
                },
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
                        WORKING = {
                            name = "Работа",
                            defaultPosition = 1,
                        },
                        WAITING = {
                            name = "Ожидание",
                            defaultPosition = 2,
                        },
                    },
                    defaultPosition = 1,
                },
            },
            basicName = "heater_node_PID",
            equipment =
            {
                TE = { name = "Датчик температуры", defaultValue = "TE1" },
                VC = { name = "Регулирующий клапан", defaultValue = "VC1" },
                FQT1 = { name = "Расходомер", defaultValue = "FQT1" },
                SET_VALUE = { name = "Задание"},
            },
            aggregateParameters =
            {
                main =
                {
                    NEED_HEATER_NODE = { name = "Использовать узел подогрева", defaultValue = "false" },
                },
            },
            bindingName = "heater_node",
            isPID = true
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
                    defaultPosition = 1,
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
            bindingName = "flow_node",
            isPID = true
        },
        cooler_node = {
            name = "Узел охлаждения",
            s88Level = 2,
            baseOperations =
            {
                COOLING = {
                    name = "Охлаждение",
                    params =
                    {
                        active =
                        {
                            FINISH_COLD_WATER_PUSHING_TEMPERATURE =
                            {
                                name = "Температура завершения вытеснения горячей воды",
                                displayObjects = { "parameters" },
                            },
                            COOL_TEMPERATURE =
                            {
                                name = "Температура охлаждения",
                                displayObjects = { "parameters" },
                            },
                            COOL_DELTA_TEMPERATURE =
                            {
                                name = "Дельта температуры охлаждения",
                                displayObjects = { "parameters" },
                            },
                        },
                    },
                    steps =
                    {
                        HOT_WATER_PUSHING = {
                            name = "Вытеснение горячей воды",
                            defaultPosition = 1,
                        },
                        WORKING = {
                            name = "Работа",
                            defaultPosition = 2,
                        },
                        WAITING = {
                            name = "Ожидание",
                            defaultPosition = 3,
                        },
                    },
                    defaultPosition = 1,
                },
            },
            basicName = "cooler_node",
            equipment =
            {
                TE = { name = "Датчик температуры", defaultValue = "TE1" },
                TE2 = { name = "Датчик температуры рубашки", defaultValue = "TE2" },
            },
            aggregateParameters =
            {
                active =
                {
                    COOLING_TEMPERATURE =
                    {
                        name = "Температура охлаждения",
                        displayObjects = { "parameters" },
                    },
                    COOLING_TEMPERATURE_DELTA =
                    {
                        name = "Дельта температуры охлаждения",
                        displayObjects = { "parameters" },
                    },
                },
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
                        WORKING = {
                            name = "Работа",
                            defaultPosition = 1,
                        },
                        WAITING = {
                            name = "Ожидание",
                            defaultPosition = 2,
                        },
                    },
                    defaultPosition = 1,
                },
            },
            basicName = "cooler_node_PID",
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
                    NEED_COOLING = { name = "Использовать узел охлаждения ПИД", defaultValue = "false" },
                },
            },
            bindingName = "cooler_node",
            isPID = true
        },
        mix_node = {
            name = "Узел перемешивания",
            s88Level = 2,
            baseOperations =
            {
                MIXING = {
                    name = "Перемешивание",
                    defaultPosition = 1,
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
                hatch2 = { name = "Датчик крышки люка 2" },
                LT = { name = "Датчик текущего уровня", defaultValue = "LT1" },
            },
            aggregateParameters =
            {
                active =
                {
                    MIX_NODE_MIX_OPERATION =
                    {
                        name = "Используемая операция узла перемешивания",
                        defaultValue = 1
                    },
                    MIX_NODE_MIX_ON_TIME =
                    {
                        name = "Время работы",
                        displayObjects = { "parameters" },
                    },
                    MIX_NODE_MIX_OFF_TIME =
                    {
                        name = "Время простоя",
                        displayObjects = { "parameters" },
                    },
                    MIX_NODE_MIX_SPEED =
                    {
                        name = "Скорость",
                        displayObjects = { "parameters" },
                    },
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
            baseOperations =
            {
                WORKING =
                {
                    name = "Работа",
                    steps =
                    {
                        WORKING = {
                            name = "Работа",
                            defaultPosition = 1,
                        },
                        WAITING = {
                            name = "Ожидание",
                            defaultPosition = 2,
                        },
                    },
                    defaultPosition = 1,
                },
                STERILIZATION =
                {
                    name = "Стерилизация",
                    steps =
                    {
                        HEATING = {
                            name = "Нагрев",
                            defaultPosition = 1,
                        },
                        STERILIZATION = {
                            name = "Стерилизация",
                            defaultPosition = 2,
                        },
                        COOLING = {
                            name = "Охлаждение",
                            defaultPosition = 3,
                        },
                    },
                    params =
                    {
                        active =
                        {
                            STERILIZATION_TEMPERATURE =
                            {
                                name = "Температура стерилизации",
                                displayObjects = { "parameters" },
                            },
                            MIN_STERILIZATION_TEMPERATURE =
                            {
                                name = "Минимальная температура стерилизации",
                                displayObjects = { "parameters" },
                            },
                            COOL_TEMPERATURE =
                            {
                                name = "Температура охлаждения",
                                displayObjects = { "parameters" },
                            },
                            MAX_OPERATION_TIME =
                            {
                                name = "Максимальное время операции",
                                displayObjects = { "parameters" },
                            },
                        },
                    },
                    defaultPosition = 2,
                },
            },
            basicName = "sterile_air_node",
            equipment =
            {
                TE1 = { name = "Датчик температуры", defaultValue = "TE1" },
            },
            aggregateParameters = { },
            bindingName = "sterile_air_node",
        },
        steam_blast_node = {
            name = "Узел продувания",
            s88Level = 2,
            baseOperations =
            {
                WORKING = {
                    name = "Работа",
                    defaultPosition = 1,
                },
            },
            basicName = "steam_blast_node",
            equipment =
            {
                GS = { name = "Датчик(и) люка", defaultValue = "GS1" },
            },
            aggregateParameters = { },
            bindingName = "steam_blast_node",
        },
        tank_level_node_PID = {
            name = "Узел текущего уровня ПИД",
            s88Level = 2,
            baseOperations =
            {
                WORKING = {
                    name = "Работа",
                    defaultPosition = 1,
                },
            },
            basicName = "tank_level_node_PID",
            equipment =
            {
                LT = { name = "Датчик текущего уровня", defaultValue = "LT1" },
                M1 = { name = "Насос (AO)", defaultValue = "M1" },
                SET_VALUE = { name = "Задание" },
            },
            bindingName = "tank_level_node_PID",
            isPID = true
        },
        tank_level_node = {
            name = "Узел текущего уровня",
            s88Level = 2,
            baseOperations =
            {
                WORKING = {
                    name = "Работа",
                    steps =
                    {
                        WAITING_LOW_LS = {
                            name = "Ожидание пропадания нижнего уровня",
                            defaultPosition = 1,
                        },
                        FEEDING_HI_LS = {
                            name = "Пополнение до появления верхнего уровня",
                            defaultPosition = 2
                        },
                    },
                    defaultPosition = 1,
                },
            },
            basicName = "tank_level_node",
            equipment =
            {
                LS_up = { name = "Датчик верхнего уровня", defaultValue = "LS2" },
                LS_down = { name = "Датчик нижнего уровня", defaultValue = "LS1" },
                LT = { name = "Датчик текущего уровня", defaultValue = "LT1" },
            },
            bindingName = "tank_level_node"
        },
        cip_module = {
            name = "Модуль мойки",
            s88Level = 2,
            baseOperations = { },
            basicName = "cip_module",
            equipment = { },
            aggregateParameters = { },
            bindingName = "cip_module"
        },
        user_object = {
            name = "Пользовательский объект",
            s88Level = 3, -- Не относится к s88Level.
            basicName = "user_object",
            bindingName = "user_object",
        }
    }
end
