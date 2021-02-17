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
-- 9. objectGroups - группы объектов (танки, линии и др).
-- 10. system_params - системные параметры объекта.
-- 11. parameters - указываются параметры объекта. Можно указать в виде ссылки на другую переменную,
-- которая представляет параметр, или же описать самому в виде lua-таблицы. При описании в виде
-- ссылки на переменную, параметры будут идти по порядку (по индексам), а если описывать через
-- Lua-таблицы, то по ключу Lua-таблицы. Предпочтительный первый вариант, он описан ниже по файлу,
-- а именно, там где определена переменная object_parameters.

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

-- Группы объектов (objectGroups) - группы объектов (в основном для линий). Внутри этой переменной
-- указываются группы объектов, которые нужны в объекте.
-- Название таблицы - Lua-имя группы, пишется в любом регистре:
-- 1. name - русскоязычное название группы.
-- 2. allowedObjects - указываются через запятую доступные объекты (units, aggregates, all).
-- Стандартное значение - aggregates, оно будет использоваться, если не указать это поле.

-- Оборудование - параметр оборудования.
-- Параметры объекта, как агрегата - по аналогии с активными и булевскими параметрами и
-- главным параметром агрегата, он является обязательным для агрегата. main-параметр задается только один!

-- Системные параметры объекта (systemParams) - системные параметры объекта, которые используются
-- в контроллере. Доступны только для чтения, можно менять только значение.
-- Индекс таблицы - указывает его номер в списке параметров.
-- 1. name - русскоязычное название параметра
-- 2. meter - единица измерения параметра
-- 3. defaultValue - стандартное значение параметра
-- 4. nameLua - LUA-имя параметра

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
            isPID = true,
            systemParams =
            {
                [ 1 ] = {
                    name = "Параметр k",
                    defaultValue = 1,
                    nameLua = "P_k",
                },
                [ 2 ] = {
                    name = "Параметр Ti",
                    defaultValue = 15,
                    nameLua = "P_Ti",
                },
                [ 3 ] = {
                    name = "Параметр Td",
                    defaultValue = 0.01,
                    nameLua = "P_Td",
                },
                [ 4 ] = {
                    name = "Интервал расчёта",
                    meter = "мсек",
                    defaultValue = 1000,
                    nameLua = "P_dt",
                },
                [ 5 ] = {
                    name = "Максимальное значение входной величины",
                    meter = "%",
                    defaultValue = 100,
                    nameLua = "P_max",
                },
                [ 6 ] = {
                    name = "Минимальное значение входной величины",
                    meter = "%",
                    defaultValue = 0,
                    nameLua = "P_min",
                },
                [ 7 ] = {
                    name = "Время выхода на режим регулирования",
                    meter = "сек",
                    defaultValue = 30,
                    nameLua = "P_acceleration_time",
                },
                [ 8 ] = {
                    name = "Ручной режим",
                    meter = "№ режима",
                    defaultValue = 0, -- Ручной режим, 0 - авто, 1 - ручной
                    nameLua = "P_is_manual_mode",
                },
                [ 9 ] = {
                    name = "Заданное ручное значение выходного сигнала",
                    meter = "%",
                    defaultValue = 65,
                    nameLua = "P_U_manual",
                },
                [ 10 ] = {
                    name = "Параметр k2",
                    defaultValue = 0,
                    nameLua = "P_k2",
                },
                [ 11 ] = {
                    name = "Параметр Ti2",
                    defaultValue = 0,
                    nameLua = "P_Ti2",
                },
                [ 12 ] = {
                    name = "Параметр Td2",
                    defaultValue = 0,
                    nameLua = "P_Td2",
                },
                [ 13 ] = {
                    name = "Максимальное значение выходной величины",
                    meter = "%",
                    defaultValue = 100,
                    nameLua = "P_out_max",
                },
                [ 14 ] = {
                    name = "Минимальное значение выходной величины",
                    meter = "%",
                    defaultValue = 0,
                    nameLua = "P_out_min",
                },
            },
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
            bindingName = "master",
            systemParams =
            {
                [ 1 ] = {
                    name = "Интервал промывки седел клапанов",
                    meter = "сек",
                    defaultValue = 180,
                    nameLua = "P_MIX_FLIP_PERIOD",
                },
                [ 2 ] = {
                    name = "Время промывки верхних седел клапанов",
                    meter = "мсек",
                    defaultValue = 2000,
                    nameLua = "P_MIX_FLIP_UPPER_TIME",
                },
                [ 3 ] = {
                    name = "Время промывки нижних седел клапанов",
                    meter = "мсек",
                    defaultValue = 1000,
                    nameLua = "P_MIX_FLIP_LOWER_TIME",
                },
                [ 4 ] = {
                    name = "Время задержки закрытия клапанов",
                    meter = "мсек",
                    defaultValue = 1000,
                    nameLua = "P_V_OFF_DELAY_TIME",
                },
                [ 5 ] = {
                    name = "Время задержки закрытия донных клапанов",
                    meter = "мсек",
                    defaultValue = 1200,
                    nameLua = "P_V_BOTTOM_OFF_DELAY_TIME",
                },
                [ 6 ] = {
                    name = "Среднее время задержки получения ответа от узла I/O",
                    meter = "мсек",
                    defaultValue = 50,
                    nameLua = "P_WAGO_TCP_NODE_WARN_ANSWER_AVG_TIME",
                },
                [ 7 ] = {
                    name = "Среднее время цикла программы",
                    meter = "мсек",
                    defaultValue = 300,
                    nameLua = "P_MAIN_CYCLE_WARN_ANSWER_AVG_TIME",
                },
                [ 8 ] = {
                    name = "Работа модуля ограничений",
                    meter = "№ режима",
                    defaultValue = 0,
                    -- 0 - авто, 1 - ручной, 2 - полуавтоматический (через
                    -- время @P_RESTRICTIONS_MANUAL_TIME вернется в
                    -- автоматический режим).
                    nameLua = "P_RESTRICTIONS_MODE",
                },
                [ 9 ] = {
                    name = "Работа модуля ограничений в ручном режиме заданное время",
                    meter = "мсек",
                    defaultValue = 120000, -- 2 * 60 * 1000 мсек
                    nameLua = "P_RESTRICTIONS_MANUAL_TIME",
                },
                [ 10 ] = {
                    name = "Переход на паузу операции при ошибке устройств",
                    meter = "№ режима",
                    defaultValue = 0, -- 0 - авто (есть), 1 - ручной (нет).
                    nameLua = "P_AUTO_PAUSE_OPER_ON_DEV_ERR",
                },
            }
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
                    params =
                    {
                        active =
                        {
                            TOTAL_VOLUME_AI =
                            {
                                name = "Объем линии",
                                displayObjects = { "signals" },
                            }
                        },
                        bool =
                        {
                            IS_END_ON_FULL_TANK =
                            {
                                name = "Автоматическое выключение по наполнению крайнего танка",
                                defaultValue = "false",
                            },
                        },
                    },
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
                        PRODUCT_PUSH = {
                            name = "Проталкивание продукта",
                        }
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
                QT = { name = "Концентратомер", defaultValue = "QT1" },
            },
            aggregateParameters = { },
            bindingName = "line",
            objectGroups =
            {
                tanks = { name = "Группа танков", allowedObjects = "units" },
            }
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
                    params =
                    {
                        active =
                        {
                            CONCENTRATION =
                            {
                                name = "Концентрация продукта",
                                displayObjects = { "parameters" },
                            },
                            DELTA_CONCENTRATION =
                            {
                                name = "Дельта концентрации продукта",
                                displayObjects = { "parameters" },
                            },
                            TOTAL_VOLUME_AI =
                            {
                                name = "Объем линии",
                                displayObjects = { "signals" },
                            }
                        },
                        bool =
                        {
                            IS_END_ON_FULL_TANK =
                            {
                                name = "Автоматическое выключение по наполнению крайнего танка",
                                defaultValue = "false",
                            },
                        },
                    },
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
                        PRODUCT_PUSH = {
                            name = "Проталкивание продукта",
                        }
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
                QT = { name = "Концентратомер", defaultValue = "QT1" },
            },
            aggregateParameters = { },
            bindingName = "line_in",
            objectGroups =
            {
                tanks = { name = "Группа танков", allowedObjects = "units" },
            }
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
                        },
                        active =
                        {
                            product_request =
                            {
                                name = "Запрос продукта от приемника",
                                displayObjects = { "signals" },
                            },
                            water_request =
                            {
                                name = "Запрос воды от приемника",
                                displayObjects = { "signals" },
                            }
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
                        IN_DRAINAGE = {
                            name = "В дренаж",
                        },
                        IN_DESTINATION = {
                            name = "В приемник",
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
            },
            aggregateParameters = { },
            bindingName = "line_out",
            objectGroups =
            {
                tanks = { name = "Группа танков", allowedObjects = "units" },
            }
        },
        line_pumping = {
            name = "Линия перекачки",
            s88Level = 2,
            baseOperations =
            {
                WASHING_CIP =
                {
                    name = "Мойка",
                    steps =
                    {
                        DRAINAGE = {
                            name = "Дренаж",
                            defaultPosition = 6
                        },
                    },
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
                    defaultPosition = 1,
                },
                PUMPING =
                {
                    name = "Перекачивание",
                    steps =
                    {
                        WAITING = {
                            name = "Ожидание",
                            defaultPosition = 1,
                        },
                        OUT_WATER = {
                            name = "Проталкивание",
                            defaultPosition = 2,
                        },
                        OUT_TANK = {
                            name = "Из источника",
                            defaultPosition = 3,
                        },
                        IN_DRAINAGE = {
                            name = "В дренаж",
                            defaultPosition = 4,
                        },
                        IN_TANK = {
                            name = "В приемник",
                            defaultPosition = 5,
                        },
                    },
                    params =
                    {
                        active =
                        {
                            SRC =
                            {
                                name = "Параметр источника",
                                displayObjects = { "parameters" },
                            },
                            DST =
                            {
                                name = "Параметр приемника",
                                displayObjects = { "parameters" },
                            },
                        },
                    },
                    defaultPosition = 2,
                },
            },
            basicName = "line",
            equipment =
            {
                product_CTR = { name = "Счетчик", defaultValue = "FQT1" },
            },
            bindingName = "line_pumping",
            objectGroups =
            {
                src_tanks = { name = "Источники", allowedObjects = "units" },
                dst_tanks = { name = "Приемники", allowedObjects = "units" },
            },
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
                WORK = {
                    name = "Работа",
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
                FILL_2 = {
                    name = "Наполнение 2",
                    params =
                    {
                        active =
                        {
                            OPERATION_AFTER_FILL = { name = "Номер последующей операции" },
                        },
                    },
                    steps = { },
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
                            LEAVENING_TIME =
                            {
                                name = "Время заквашивания (мин)",
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
                    params =
                    {
                        active =
                        {
                            AUTO_DI =
                            {
                                name = "Сигнал автоматического включения/выключения",
                                displayObjects = { "signals" },
                            },
                        }
                    },
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
                HEATING = {
                    name = "Подогрев основы",
                    steps =
                    {
                        HEATING = { name = "Нагрев" },
                        CHECKING_TEMPERATURE = { name = "Проверка температуры" },
                    },
                    params =
                    {
                        active =
                        {
                            MAX_TIME =
                            {
                                name = "Максимальное время операции",
                                displayObjects = { "parameters" },
                            },
                            TEMPERATURE_VERIFICATION_TIME =
                            {
                                name = "Время стабилизации температуры",
                                displayObjects = { "parameters" },
                            },
                        },
                    },
                },
                WHEY_SEPARATION_TURNOVER = {
                    name = "Отделение сыворотки по оборотам",
                    params =
                    {
                        active =
                        {
                            MAX_TIME =
                            {
                                name = "Максимальное время операции",
                                displayObjects = { "parameters" },
                            },
                        },
                    },
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
            isPID = true,
            systemParams =
            {
                [ 1 ] = {
                    name = "Параметр k",
                    defaultValue = 1,
                    nameLua = "P_k",
                },
                [ 2 ] = {
                    name = "Параметр Ti",
                    defaultValue = 15,
                    nameLua = "P_Ti",
                },
                [ 3 ] = {
                    name = "Параметр Td",
                    defaultValue = 0.01,
                    nameLua = "P_Td",
                },
                [ 4 ] = {
                    name = "Интервал расчёта",
                    meter = "мсек",
                    defaultValue = 1000,
                    nameLua = "P_dt",
                },
                [ 5 ] = {
                    name = "Максимальное значение входной величины",
                    meter = "бар",
                    defaultValue = 100,
                    nameLua = "P_max",
                },
                [ 6 ] = {
                    name = "Минимальное значение входной величины",
                    meter = "бар",
                    defaultValue = 0,
                    nameLua = "P_min",
                },
                [ 7 ] = {
                    name = "Время выхода на режим регулирования",
                    meter = "сек",
                    defaultValue = 30,
                    nameLua = "P_acceleration_time",
                },
                [ 8 ] = {
                    name = "Ручной режим",
                    meter = "№ режима",
                    defaultValue = 0, -- Ручной режим, 0 - авто, 1 - ручной
                    nameLua = "P_is_manual_mode",
                },
                [ 9 ] = {
                    name = "Заданное ручное значение выходного сигнала",
                    meter = "%",
                    defaultValue = 65,
                    nameLua = "P_U_manual",
                },
                [ 10 ] = {
                    name = "Параметр k2",
                    defaultValue = 0,
                    nameLua = "P_k2",
                },
                [ 11 ] = {
                    name = "Параметр Ti2",
                    defaultValue = 0,
                    nameLua = "P_Ti2",
                },
                [ 12 ] = {
                    name = "Параметр Td2",
                    defaultValue = 0,
                    nameLua = "P_Td2",
                },
                [ 13 ] = {
                    name = "Максимальное значение выходной величины",
                    meter = "%",
                    defaultValue = 100,
                    nameLua = "P_out_max",
                },
                [ 14 ] = {
                    name = "Минимальное значение выходной величины",
                    meter = "%",
                    defaultValue = 0,
                    nameLua = "P_out_min",
                },
            },
        },
        heater_node = {
            name = "Узел подогрева",
            s88Level = 2,
            baseOperations =
            {
                HEATING = {
                    name = "Нагрев",
                    params =
                    {
                        active =
                        {
                            WORK_REQUEST =
                            {
                                name = "Сигнал включения шага \"Работа\"",
                                displayObjects = { "signals" }
                            },
                        },
                    },
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
                        WAITING_TO_WORK_REQUEST = {
                            name = "Ожидание запроса работы"
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
                    params =
                    {
                        active =
                        {
                            WORK_REQUEST =
                            {
                                name = "Сигнал включения шага \"Работа\"",
                                displayObjects = { "signals" }
                            },
                        },
                    },
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
                        WAITING_TO_WORK_REQUEST = {
                            name = "Ожидание запроса работы"
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
            isPID = true,
            systemParams =
            {
                [ 1 ] = {
                    name = "Параметр k",
                    defaultValue = 1,
                    nameLua = "P_k",
                },
                [ 2 ] = {
                    name = "Параметр Ti",
                    defaultValue = 15,
                    nameLua = "P_Ti",
                },
                [ 3 ] = {
                    name = "Параметр Td",
                    defaultValue = 0.01,
                    nameLua = "P_Td",
                },
                [ 4 ] = {
                    name = "Интервал расчёта",
                    meter = "мсек",
                    defaultValue = 1000,
                    nameLua = "P_dt",
                },
                [ 5 ] = {
                    name = "Максимальное значение входной величины",
                    meter = "°C",
                    defaultValue = 100,
                    nameLua = "P_max",
                },
                [ 6 ] = {
                    name = "Минимальное значение входной величины",
                    meter = "°C",
                    defaultValue = 0,
                    nameLua = "P_min",
                },
                [ 7 ] = {
                    name = "Время выхода на режим регулирования",
                    meter = "сек",
                    defaultValue = 30,
                    nameLua = "P_acceleration_time",
                },
                [ 8 ] = {
                    name = "Ручной режим",
                    meter = "№ режима",
                    defaultValue = 0, -- Ручной режим, 0 - авто, 1 - ручной
                    nameLua = "P_is_manual_mode",
                },
                [ 9 ] = {
                    name = "Заданное ручное значение выходного сигнала",
                    meter = "%",
                    defaultValue = 65,
                    nameLua = "P_U_manual",
                },
                [ 10 ] = {
                    name = "Параметр k2",
                    defaultValue = 0,
                    nameLua = "P_k2",
                },
                [ 11 ] = {
                    name = "Параметр Ti2",
                    defaultValue = 0,
                    nameLua = "P_Ti2",
                },
                [ 12 ] = {
                    name = "Параметр Td2",
                    defaultValue = 0,
                    nameLua = "P_Td2",
                },
                [ 13 ] = {
                    name = "Максимальное значение выходной величины",
                    meter = "%",
                    defaultValue = 100,
                    nameLua = "P_out_max",
                },
                [ 14 ] = {
                    name = "Минимальное значение выходной величины",
                    meter = "%",
                    defaultValue = 0,
                    nameLua = "P_out_min",
                },
            },
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
            isPID = true,
            systemParams =
            {
                [ 1 ] = {
                    name = "Параметр k",
                    defaultValue = 1,
                    nameLua = "P_k",
                },
                [ 2 ] = {
                    name = "Параметр Ti",
                    defaultValue = 15,
                    nameLua = "P_Ti",
                },
                [ 3 ] = {
                    name = "Параметр Td",
                    defaultValue = 0.01,
                    nameLua = "P_Td",
                },
                [ 4 ] = {
                    name = "Интервал расчёта",
                    meter = "мсек",
                    defaultValue = 1000,
                    nameLua = "P_dt",
                },
                [ 5 ] = {
                    name = "Максимальное значение входной величины",
                    meter = "м3/ч",
                    defaultValue = 100,
                    nameLua = "P_max",
                },
                [ 6 ] = {
                    name = "Минимальное значение входной величины",
                    meter = "м3/ч",
                    defaultValue = 0,
                    nameLua = "P_min",
                },
                [ 7 ] = {
                    name = "Время выхода на режим регулирования",
                    meter = "сек",
                    defaultValue = 30,
                    nameLua = "P_acceleration_time",
                },
                [ 8 ] = {
                    name = "Ручной режим",
                    meter = "№ режима",
                    defaultValue = 0, -- Ручной режим, 0 - авто, 1 - ручной
                    nameLua = "P_is_manual_mode",
                },
                [ 9 ] = {
                    name = "Заданное ручное значение выходного сигнала",
                    meter = "%",
                    defaultValue = 65,
                    nameLua = "P_U_manual",
                },
                [ 10 ] = {
                    name = "Параметр k2",
                    defaultValue = 0,
                    nameLua = "P_k2",
                },
                [ 11 ] = {
                    name = "Параметр Ti2",
                    defaultValue = 0,
                    nameLua = "P_Ti2",
                },
                [ 12 ] = {
                    name = "Параметр Td2",
                    defaultValue = 0,
                    nameLua = "P_Td2",
                },
                [ 13 ] = {
                    name = "Максимальное значение выходной величины",
                    meter = "%",
                    defaultValue = 100,
                    nameLua = "P_out_max",
                },
                [ 14 ] = {
                    name = "Минимальное значение выходной величины",
                    meter = "%",
                    defaultValue = 0,
                    nameLua = "P_out_min",
                },
            },
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
                            WORK_REQUEST =
                            {
                                name = "Сигнал включения шага \"Работа\"",
                                displayObjects = { "signals" }
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
                        WAITING_TO_WORK_REQUEST = {
                            name = "Ожидание запроса работы"
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
                    params =
                    {
                        active =
                        {
                            COOL_TEMPERATURE =
                            {
                                name = "Температура охлаждения",
                                displayObjects = { "parameters" },
                            },
                            COOL_TEMPERATURE_DELTA =
                            {
                                name = "Дельта температуры охлаждения",
                                displayObjects = { "parameters" },
                            },
                            WORK_REQUEST =
                            {
                                name = "Сигнал включения шага \"Работа\"",
                                displayObjects = { "signals" }
                            },
                        },
                    },
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
                        WAITING_TO_WORK_REQUEST = {
                            name = "Ожидание запроса работы"
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
                    NEED_COOLING = { name = "Использовать узел охлаждения ПИД", defaultValue = "false" },
                },
            },
            bindingName = "cooler_node",
            isPID = true,
            systemParams =
            {
                [ 1 ] = {
                    name = "Параметр k",
                    defaultValue = 1,
                    nameLua = "P_k",
                },
                [ 2 ] = {
                    name = "Параметр Ti",
                    defaultValue = 15,
                    nameLua = "P_Ti",
                },
                [ 3 ] = {
                    name = "Параметр Td",
                    defaultValue = 0.01,
                    nameLua = "P_Td",
                },
                [ 4 ] = {
                    name = "Интервал расчёта",
                    meter = "мсек",
                    defaultValue = 1000,
                    nameLua = "P_dt",
                },
                [ 5 ] = {
                    name = "Максимальное значение входной величины",
                    meter = "°C",
                    defaultValue = 100,
                    nameLua = "P_max",
                },
                [ 6 ] = {
                    name = "Минимальное значение входной величины",
                    meter = "°C",
                    defaultValue = 0,
                    nameLua = "P_min",
                },
                [ 7 ] = {
                    name = "Время выхода на режим регулирования",
                    meter = "сек",
                    defaultValue = 30,
                    nameLua = "P_acceleration_time",
                },
                [ 8 ] = {
                    name = "Ручной режим",
                    meter = "№ режима",
                    defaultValue = 0, -- Ручной режим, 0 - авто, 1 - ручной
                    nameLua = "P_is_manual_mode",
                },
                [ 9 ] = {
                    name = "Заданное ручное значение выходного сигнала",
                    meter = "%",
                    defaultValue = 65,
                    nameLua = "P_U_manual",
                },
                [ 10 ] = {
                    name = "Параметр k2",
                    defaultValue = 0,
                    nameLua = "P_k2",
                },
                [ 11 ] = {
                    name = "Параметр Ti2",
                    defaultValue = 0,
                    nameLua = "P_Ti2",
                },
                [ 12 ] = {
                    name = "Параметр Td2",
                    defaultValue = 0,
                    nameLua = "P_Td2",
                },
                [ 13 ] = {
                    name = "Максимальное значение выходной величины",
                    meter = "%",
                    defaultValue = 100,
                    nameLua = "P_out_max",
                },
                [ 14 ] = {
                    name = "Минимальное значение выходной величины",
                    meter = "%",
                    defaultValue = 0,
                    nameLua = "P_out_min",
                },
            },
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
                POSITION_MIX = {
                    name = "Позиционирование мешалки",
                    params =
                    {
                        active =
                        {
                            MAX_TIME =
                            {
                                name = "Максимальное время позиционирования (мин)",
                                displayObjects = { "parameters" },
                            },
                            MIX_SPEED_RPM =
                            {
                                name = "Скорость позиционирования (об/мин)",
                                displayObjects = { "parameters" },
                            },
                        }
                    },
                },
                MIXING_REVOL = {
                    name = "Перемешивание по оборотам",
                    steps =
                    {
                        INIT = {
                            name = "Определение положения",
                        },
                        FROM_SENSOR_TO_LEFT = {
                            name = "Соскальзывание влево с датчика верхнего положения",
                        },
                        TO_SENSOR_TO_LEFT = {
                            name = "Движение влево до верхнего положения",
                        },
                        FROM_SENSOR_TO_RIGHT = {
                            name = "Соскальзывание вправо с датчика верхнего положения",
                        },
                        TO_SENSOR_TO_RIGHT = {
                            name = "Движение вправо до верхнего положения",
                        },
                    },
                    params =
                    {   active =
                        {
                            DIRECTION =
                            {
                                name = "Направление вращения (влево/вправо)",
                                displayObjects = { "parameters" },
                            },
                            MIX_SPEED_RPM =
                            {
                                name = "Заданная скорость (об/мин)",
                                displayObjects = { "parameters" },
                            },
                            MAX_STEP_TIME =
                            {
                                name = "Максимальное время работы мешалки",
                                displayObjects = { "parameters" },
                            },
                            SET_CNT =
                            {
                                name = "Задано оборотов",
                                displayObjects = { "parameters" },
                            },
                            DONE_CNT =
                            {
                                name = "Сделано оборотов",
                                displayObjects = { "parameters" },
                            },
                        },
                    },
                },
                MIXING_AUTO = {
                    name = "Перемешивание авто",
                    steps =
                    {
                        INIT = { name = "Определение положения" },
                        FROM_LEFT_SENSOR = { name = "Движение от левого положения" },
                        TO_RIGHT_SENSOR = { name = "Движение к правому положению" },
                        FROM_RIGHT_SENSOR = { name = "Движение от правого положения" },
                        TO_LEFT_SENSOR = { name = "Движение к левому положению" },
                    },
                    params =
                    {
                        active =
                        {
                            MIX_SPEED_RPM = {
                                name = "Заданная скорость (об/мин)",
                                displayObjects = { "parameters" }
                            },
                            MAX_TIME = {
                                name = "Максимальное время перемешивания (мин)",
                                displayObjects = { "parameters" },
                            },
                        },
                    },
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
                high_mix_lvl = { name = "Датчик верхнего положения мешалки" },
                left_mix_lvl = { name = "Датчик левого положения мешалки" },
                right_mix_lvl = { name = "Датчик правого положения мешалки" },
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
            aggregateParameters =
            {
                main =
                {
                    NEED_STERILE_AIR_NODE =
                    {
                        name = "Использовать узел стерильного воздуха",
                        defaultValue = "false"
                    },
                },
            },
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
            aggregateParameters =
            {
                main =
                {
                    NEED_STEAM_BLAST_NODE =
                    {
                        name = "Использовать узел продувания",
                        defaultValue = "false"
                    },
                },
            },
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
            isPID = true,
            systemParams =
            {
                [ 1 ] = {
                    name = "Параметр k",
                    defaultValue = 1,
                    nameLua = "P_k",
                },
                [ 2 ] = {
                    name = "Параметр Ti",
                    defaultValue = 15,
                    nameLua = "P_Ti",
                },
                [ 3 ] = {
                    name = "Параметр Td",
                    defaultValue = 0.01,
                    nameLua = "P_Td",
                },
                [ 4 ] = {
                    name = "Интервал расчёта",
                    meter = "мсек",
                    defaultValue = 1000,
                    nameLua = "P_dt",
                },
                [ 5 ] = {
                    name = "Максимальное значение входной величины",
                    meter = "%",
                    defaultValue = 100,
                    nameLua = "P_max",
                },
                [ 6 ] = {
                    name = "Минимальное значение входной величины",
                    meter = "%",
                    defaultValue = 0,
                    nameLua = "P_min",
                },
                [ 7 ] = {
                    name = "Время выхода на режим регулирования",
                    meter = "сек",
                    defaultValue = 30,
                    nameLua = "P_acceleration_time",
                },
                [ 8 ] = {
                    name = "Ручной режим",
                    meter = "№ режима",
                    defaultValue = 0, -- Ручной режим, 0 - авто, 1 - ручной
                    nameLua = "P_is_manual_mode",
                },
                [ 9 ] = {
                    name = "Заданное ручное значение выходного сигнала",
                    meter = "%",
                    defaultValue = 65,
                    nameLua = "P_U_manual",
                },
                [ 10 ] = {
                    name = "Параметр k2",
                    defaultValue = 0,
                    nameLua = "P_k2",
                },
                [ 11 ] = {
                    name = "Параметр Ti2",
                    defaultValue = 0,
                    nameLua = "P_Ti2",
                },
                [ 12 ] = {
                    name = "Параметр Td2",
                    defaultValue = 0,
                    nameLua = "P_Td2",
                },
                [ 13 ] = {
                    name = "Максимальное значение выходной величины",
                    meter = "%",
                    defaultValue = 100,
                    nameLua = "P_out_max",
                },
                [ 14 ] = {
                    name = "Минимальное значение выходной величины",
                    meter = "%",
                    defaultValue = 0,
                    nameLua = "P_out_min",
                },
            },
            aggregateParameters =
            {
                main =
                {
                    NEED_TANK_LEVEL_NODE_PID =
                    {
                        name = "Использовать узел текущего уровня ПИД",
                        defaultValue = "false"
                    },
                },
            },
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
            bindingName = "tank_level_node",
            aggregateParameters =
            {
                main =
                {
                    NEED_TANK_LEVEL_NODE =
                    {
                        name = "Использовать узел текущего уровня",
                        defaultValue = "false"
                    },
                },
            },
        },
        cip_module = {
            name = "Модуль мойки",
            s88Level = 2,
            baseOperations = { },
            basicName = "cip_module",
            equipment = { },
            aggregateParameters = { },
            bindingName = "cip_module",
        },
        user_object = {
            name = "Пользовательский объект",
            s88Level = 3, -- Не относится к s88Level.
            basicName = "user_object",
            bindingName = "user_object",
        },
        CIP_node = {
            name = "Линия мойки",
            s88Level = 2,
            basicName = "line",
            bindingName = "CIP_line",
            baseOperations =
            {
                WASHING_CIP =
                {
                    name = "Мойка",
                    defaultPosition = 1,
                    params =
                    {
                        active =
                        {
                            CIP_WASH_REQUEST =
                            {
                                name = "Автоматическое включение мойки",
                                displayObjects = { "signals" },
                            },
                        },
                    },
                }
            },
            objectGroups =
            {
                CIP_objects = { name = "Моющиеся объекты", allowedObjects = "all" },
            }
        }
    }
end

-- Здесь содержится общий пул параметров, для базовых объектов. Параметры
-- настраиваются следующим образом:
-- 1. luaName - указывается строковое Lua-имя параметра. Это Lua-имя дублируется
-- в названии переменной (таблицы) параметра.
-- 2. name - указывается строковое отображаемое название.
-- 3. value - указывается начальное значение параметра.
-- (целое число или число с плавающей запятой).
-- 4. meter - указывается единица измерения в виде строки.
object_parameters =
{
    -- Пример правильного заполнения, удалить при заполнении
    -- удалить при заполнении реальными данными.
    ParLuaName1 = {
        luaName = "ParLuaName1",
        name = "Название параметра",
        value = 20,
        meter = "сек",
    },
}