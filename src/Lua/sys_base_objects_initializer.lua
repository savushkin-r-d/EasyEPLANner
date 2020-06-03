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

        -- Добавить базовый объект
        local baseObject = AddBaseObject(name, eplanName, s88Level,
            basicName, bindingName)

        -- Добавить базовые операции (параметры, шаги)
        local baseOperations = value.baseOperations or { }
        init_base_operations(baseObject, baseOperations)

        -- Добавить оборудование
        local equipment = value.equipment or { }
        init_equipment(baseObject, equipment)

        -- Добавить параметры объекта как агрегата
        local aggregateParameters = value.aggregateParameters or { }
        init_aggregate_parameters(baseObject, aggregateParameters)
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
end

-- Инициализация базовых операций объекта
init_base_operations = function(object, operations)
    for luaName, value in pairs(operations) do
        -- Данные для минимальной инициализации операции
        local name = value.name or ""

        -- Добавить базовую операцию
        local operation = object:AddBaseOperation(luaName, name)

        -- Добавить параметры базовой операции
        local params = value.params or { }
        init_operation_parameters(operation, params)

        -- Добавить шаги базовой операции
        local steps = value.steps or { }
        init_operation_steps(operation, steps)
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

        -- Добавить активный параметр
        object:AddActiveParameter(luaName, name, defaultValue)
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

-- Инициализация шагов базовой операции
init_operation_steps = function(operation, steps)
    for luaName, value in pairs(steps) do
        -- Данные для добавления базового шага
        local name = value.name or ""

        -- Добавить базовый шаг для операции
        operation:AddStep(luaName, name)
    end
end