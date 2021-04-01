-- Инициализировать базовые объекты из файла
init_base_objects = function()
    if base_tech_objects == nil then
        return
    end

    local objects = base_tech_objects()
    for eplanName, value in pairs(objects) do
        -- Данные для минимальной инициализации
        local name = value.name or ""
        local s88Level = value.s88Level or 0
        local basicName = value.basicName or ""
        local bindingName = value.bindingName or ""
        local isPid = value.isPID or false
        local luaModuleName = value.luaModuleName or ""
        local monitorName = value.monitorName or "TankObj"

        -- Добавить базовый объект
        local baseObject = AddBaseObject(name, eplanName, s88Level,
            basicName, bindingName, isPid, luaModuleName, monitorName)

        -- Добавить группы танков
        local objectGroups = value.objectGroups or { }
        init_objectGroups(baseObject, objectGroups)

        -- Добавить базовые операции (параметры, шаги)
        local baseOperations = value.baseOperations or { }
        init_base_operations(baseObject, baseOperations)

        -- Добавить оборудование
        local equipment = value.equipment or { }
        init_equipment(baseObject, equipment)

        -- Добавить параметры объекта как агрегата
        local aggregateParameters = value.aggregateParameters or { }
        init_aggregate_parameters(baseObject, aggregateParameters)

        -- Добавить системные параметры объекта
        local systemParameters = value.systemParams or { }
        init_system_parameters(baseObject, systemParameters)

        -- Добавить параметры объекта
        local parameters = value.parameters or { }
        init_parameters(baseObject, parameters)
    end

    return 0
end

-- Инициализация оборудования объекта
init_equipment = function(object, equipment)
    for luaName, value in pairs(equipment) do
        -- Данные для добавления оборудования
        local name = value.name or ""
        local defaultValue = value.defaultValue or ""

        -- Добавить оборудование
        object:AddEquipment(luaName, name, defaultValue)
    end
end

-- Инициализация параметров объекта как агрегата
init_aggregate_parameters = function(object, aggregateParameters)
    -- Добавить активные параметры агрегата
    local activeAggregateParameters = aggregateParameters.active or { }
    init_active_parameters(object, activeAggregateParameters)
    -- Добавить булевые параметры агрегата
    local boolAggregateParameters = aggregateParameters.bool or { }
    init_active_bool_parameters(object, boolAggregateParameters)
    -- Добавить главный параметр агрегата
    local mainAggregateParameter = aggregateParameters.main or { }
    init_main_aggregate_parameter(object, mainAggregateParameter)
end

-- Инициализация базовых операций объекта
init_base_operations = function(object, operations)
    for luaName, value in pairs(operations) do
        -- Данные для минимальной инициализации операции
        local name = value.name or ""
        local defaultPosition = value.defaultPosition or 0

        -- Добавить базовую операцию
        local operation = object:AddBaseOperation(luaName, name, defaultPosition)

        -- Добавить параметры базовой операции
        local params = value.params or { }
        init_operation_parameters(operation, params)

        -- Добавить состояния и их шаги
        local states = value.state or { }
        init_operation_states(operation, states)
    end
end

-- Инициализация параметров базовой операции
init_operation_parameters = function(operation, params)
    -- Добавить активные параметры операции
    local activeParameters = params.active or { }
    init_active_parameters(operation, activeParameters)

    -- Добавить булевы параметра операции
    local activeBoolParameters = params.bool or { }
    init_active_bool_parameters(operation, activeBoolParameters)
end

-- Инициализация активных параметров
-- object - базовая операция или базовый объект
init_active_parameters = function(object, activeParameters)
	for luaName, value in pairs(activeParameters) do
        -- Данные для добавления параметра
        local name = value.name or ""
        local defaultValue = value.defaultValue or ""
        local displayObjects = value.displayObjects or { }

        -- Добавить активный параметр
        local parameter = object:AddActiveParameter(luaName, name, defaultValue)

        for showProperty, value in pairs(displayObjects) do
            parameter:AddDisplayObject(value)
        end

    end
end

-- Инициализация булевых параметров
-- object - базовая операция или базовый объект
init_active_bool_parameters = function(object, activeBoolParameters)
	for luaName, value in pairs(activeBoolParameters) do
        -- Данные для добавления параметра
        local name = value.name or ""
        local defaultValue = value.defaultValue or ""

        -- Добавить булевый параметр
        object:AddActiveBoolParameter(luaName, name, defaultValue)
    end
end

-- Инициализация главного параметра агрегата
init_main_aggregate_parameter = function(object, tableWithParameter)
    for luaName, value in pairs(tableWithParameter) do
        local name = value.name or ""
        local defaultValue = value.defaultValue or ""
        object:AddMainAggregateParameter(luaName, name, defaultValue)
    end
end

-- Инициализация состояний базовой операции
init_operation_states = function(operation, states)
    for stateLuaName, state in pairs(states) do
        -- Базовые шаги состояния
        local steps = state.steps or { }

        -- Инициализировать базовые шаги состояния
        init_operation_steps(operation, steps, stateLuaName)
    end
end

-- Инициализация шагов базовой операции в состоянии
init_operation_steps = function(operation, steps, stateLuaName)
    for luaName, value in pairs(steps) do
        -- Данные для добавления базового шага
        local name = value.name or ""
        local defaultPosition = value.defaultPosition or 0

        -- Добавить базовый шаг для операции
        operation:AddStep(stateLuaName, luaName, name, defaultPosition)
    end
end

-- Инициализация групп танков базового объекта
init_objectGroups = function(object, objectGroups)
    for luaName, value in pairs(objectGroups) do
        local name = value.name or ""
        local allowedObjects = value.allowedObjects or ""
        
        object:AddObjectGroup(luaName, name, allowedObjects)
    end
end

-- Инициализация системных параметров базового объекта
init_system_parameters = function(object, systemParameters)
    for index, parameter in ipairs(systemParameters) do
        local name = parameter.name or ""
        local defaultValue = parameter.defaultValue or 0
        local meter = parameter.meter or ""
        local nameLua = parameter.nameLua or ""

        object:AddSystemParameter(nameLua, name, defaultValue, meter)
    end
end

-- Инициализация параметров базового объекта
init_parameters = function(object, parameters)
    for key, parameter in ipairs(parameters) do
        local name = parameter.name or ""
        local luaName = parameter.luaName or ""
        local meter = parameter.meter or ""
        local value = parameter.value or 0

        object:AddParameter(luaName, name, value, meter)
    end
end
