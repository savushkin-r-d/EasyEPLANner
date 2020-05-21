-- ���������������� ������� ������� �� �����
init_base_objects = function()
    if base_tech_objects == nil then
        return
    end

    local objects = base_tech_objects()
    for eplanName, value in ipairs(objects) do
        -- ������ ��� ����������� �������������
        local name = value.name or ""
        local s88Level = value.s88Level or 0
        local basicName = value.basicName or ""
        local bindingName = value.bindingName or ""

        -- �������� ������� ������
        local baseObject = AddBaseObject(name, eplanName, s88Level, basicName, bindingName)

        -- �������� ������� �������� (���������, ����)
        local baseOperations = value.baseOperations or { }
        init_base_operations(baseObject, baseOperations)

        -- �������� ������������
        local equipment = value.equipment or { }
        init_equipment(baseObject, equipment)

        -- �������� ��������� ������� ��� ��������
        local aggregateParameters = value.aggregateParameters or { }
        init_aggregate_parameters(baseObject, aggregateParameters)
    end

    return 0
end

-- ������������� ������������ �������
init_equipment = function(object, equipment)
    for luaName, value in ipairs(equipment) do
        -- ������ ��� ���������� ������������
        local name = value.name or ""
        local defaultValue = value.defaultValue or ""

        -- �������� ������������
        object:AddEquipment(luaName, name, defaultValue)
    end
end

-- ������������� ���������� ������� ��� ��������
init_aggregate_parameters = function(object, aggregateParameters)
    for luaName, value in ipairs(aggregateParameters) do
        -- ������ ��� ���������� ���������
        local name = value.name or ""
        local defaultValue = value.defaultValue or ""

        -- �������� ��������
        object:AddAggregateParameter(luaName, name, defaultValue)
    end
end

-- ������������� ������� �������� �������
init_base_operations = function(object, operations)
    for luaName, value in ipairs(operations) do
        -- ������ ��� ����������� ������������� ��������
        local name = value.name or ""

        -- �������� ������� ��������
        local operation = object:AddBaseOperation(luaName, name)

        -- �������� ��������� ������� ��������
        local params = value.params or { }
        init_operation_parameters(operation, params)

        -- �������� ���� ������� ��������
        local steps = value.steps or { }
        init_operation_steps(operation, steps)
    end
end

-- ������������� ���������� ������� ��������
init_operation_parameters = function(operation, params)
    -- �������� �������� ��������� ��������
    local activeParameters = params.active or { }
    init_active_parameters(operation, activeParameters)

    -- �������� ������ ��������� ��������
    local activeBoolParameters = params.bool or { }
    init_active_bool_parameters(operation, activeBoolParameters)

    -- �������� ��������� ��������� ��������
    local passiveParameters = params.passive or { }
    init_passive_parameters(operation, passiveParameters)
end

-- ������������� �������� ���������� ������� ��������
init_active_parameters = function(operation, activeParameters)
	for luaName, value in ipairs(activeParameters) do
        -- ������ ��� ���������� ���������
        local name = value.name or ""
        local defaultValue = value.defaultValue or ""

        -- �������� �������� ��������
        operation:AddActiveParameter(luaName, name, defaultValue)
    end
end

-- ������������� ������� ���������� ������� ��������
init_active_bool_parameters = function(operation, activeBoolParameters)
	for luaName, value in ipairs(activeBoolParameters) do
        -- ������ ��� ���������� ���������
        local name = value.name or ""
        local defaultValue = value.defaultValue or ""

        -- �������� ������� ��������
        operation:AddActiveBoolParameter(luaName, name, defaultValue)
    end
end

-- ������������� ��������� ���������� ������� ��������
init_passive_parameters = function(operation, passiveParameters)
	for luaName, value in ipairs(passiveParameters) do
        -- ������ ��� ���������� ���������
        local name = value.name or ""

        -- �������� ��������� ��������
        operation:AddPassiveParameter(luaName, name)
    end
end

-- ������������� ����� ������� ��������
init_operation_steps = function(operation, steps)
    for luaName, value in ipairs(steps) do
        -- ������ ��� ���������� �������� ����
        local name = value.name or ""

        -- �������� ������� ��� ��� ��������
        operation:AddStep(luaName, name)
    end
end