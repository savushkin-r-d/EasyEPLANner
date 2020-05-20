-- Инициализировать базовые объекты из файла
init_base_objects = function()
    if base_tech_objects == nil then
        return
    end

    local objects = base_tech_objects()
    for field, value in ipairs(objects) do
        -- Данные для минимальной инициализации
        local name = value.name or ""
        local eplanName = value.eplanName or ""
        local s88Level = value.s88Level or 0
        local basicName = value.basicName or ""

        -- Добавить базовый объект
        local baseObject = AddBaseObject(name, eplanName, s88Level, basicName)
        
        -- Добавить базовые операции (параметры, шаги)
        local baseOperations = value.baseOperation or { }
        init_base_operations(object, baseOperations)

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
    for field, value in ipairs(equipment) do
        -- Данные для добавления оборудования
        local luaName = value.luaName or ""
        local name = value.name or ""
        local defaultValue = value.defaultValue or ""

        -- Добавить оборудование
        object:AddEquipment(luaName, name, defaultValue)
    end
end

-- Инициализация параметров объекта как агрегата
init_aggregate_parameters = function(object, aggregateParameters)
    for field, value in ipairs(aggregateParameters) do
        -- Данные для добавления параметра
        local luaName = value.luaName or ""
        local name = value.name or ""
        local defaultValue = value.defaultValue or ""

        -- Добавить параметр
        object:AddAggregateParameter(luaName, name, defaultValue)
    end
end

-- Инициализация базовых операций объекта
init_base_operations = function(object, operations)
    for field, value in ipairs(operations) do
        -- Данные для минимальной инициализации операции
        local luaName = value.luaName or ""
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

    -- Добавить пассивные параметры операции
    local passiveParameters = params.passive or { }
    init_passive_parameters(operation, passiveParameters)
end

-- Инициализация активных параметров базовой операции
init_active_parameters = function(operation, activeParameters)
	for field, value in ipairs(activeParameters) do
        -- Данные для добавления параметра
        local luaName = value.luaName or ""
        local name = value.name or ""
        local defaultValue = value.defaultValue or ""

        -- Добавить активный параметр
        operation:AddActiveParameter(luaName, name, defaultValue)
    end
end

-- Инициализация булевых параметров базовой операции
init_active_bool_parameters = function(operation, activeBoolParameters)
	for field, value in ipairs(activeBoolParameters) do
        -- Данные для добавления параметра
        local luaName = value.luaName or ""
        local name = value.name or ""
        local defaultValue = value.defaultValue or ""

        -- Добавить булевый параметр
        operation:AddActiveBoolParameter(luaName, name, defaultValue)
    end
end

-- Инициализация пассивных параметров базовой операции
init_passive_parameters = function(operation, passiveParameters)
	for field, value in ipairs(passiveParameters) do
        -- Данные для добавления параметра
        local luaName = value.luaName or ""
        local name = value.name or ""

        -- Добавить пассивный параметр
        operation:AddPassiveParameter(luaName, name)
    end
end

-- Инициализация шагов базовой операции
init_operation_steps = function(operation, steps)
    for field, value in ipairs(steps) do
        -- Данные для добавления базового шага
        local luaName = value.luaName or ""
        local name = value.name or ""

        -- Добавить базовый шаг для операции
        local step = operation:AddStep(luaName, name)
    end
end