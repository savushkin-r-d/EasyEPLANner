init = function()
    if init_tech_objects_modes == nil then
        return
    end

    local objects = init_tech_objects_modes()
    local initialized_objects = { }
	for number, value in ipairs( objects ) do
		-- Проверка пустоты таблицы для импортируемых объектов.
        -- Экспорт выполняется так, что бы пустые элементы таблицы имели длину 
        -- 1,а при импорте это игнорируется. Сохранение таблиц (файлов проекта)
        -- задает длину всех таблиц равную 0.
        if(#value == 0) then
            -- Глобальный номер объекта
            local global_number       = number -- Для импорта в редактор
		    local object_n            = value.n or 1
		    local object_name         = value.name or "Object"
		    local object_tech_type    = value.tech_type or 1
		    local object_name_eplan   = value.name_eplan or "TANK"
		    local object_name_BC      = value.name_BC or "TankObj"
		    local cooper_param_number = value.cooper_param_number or -1
		    local base_tech_object	  = value.base_tech_object or "" 
		    local attached_objects	  = value.attached_objects or ""

		    local obj = ADD_TECH_OBJECT(global_number, object_n, object_name,
			    object_tech_type, object_name_eplan, cooper_param_number,
			    object_name_BC, base_tech_object, attached_objects)
		    initialized_objects[number] = obj
        end
	end

    for fields, value in ipairs( objects ) do
        -- Аналогично с предыдущим циклом.
        if (#value == 0) then
            local obj = initialized_objects[fields]

            --Параметры
            proc_params( value.par_float, "par_float", obj )
            proc_params( value.par_uint, "par_uint", obj )
            proc_params( value.rt_par_float, "rt_par_float", obj )
            proc_params( value.rt_par_uint, "rt_par_uint", obj)

            -- Системные параметры
            proc_system_params( value.system_parameters, obj)

            local params_float = value.par_float

            --Оборудование (всегда после параметров)
            if value.equipment ~= nil then
                for field, value in pairs( value.equipment ) do
                    obj:AddEquipment(field, value)
                end
            end

            if value.tank_groups ~= nil then
                for luaName, value in pairs(value.tank_groups) do
                    obj:AddGroupObjects(luaName, value)
                end
            end

            for fields, value in ipairs( value.modes ) do
                local mode_name = value.name or "Операция ??"
			    local mode_base_operation = value.base_operation or ""

			    -- Доп. свойства по операции
			    local mode_base_operation_props = {}
			    if value.props ~= nil then
				    for fields, value in pairs(value.props) do
					    mode_base_operation_props[fields] = value 
				    end
			    end

                local mode = obj:AddMode( mode_name, mode_base_operation, 
                    mode_base_operation_props)

                local idx = fields
                proc_oper_params( params_float, mode, idx, obj )

                if value.states ~= nil then
                    for fields, value in pairs( value.states ) do
                        local state_n = fields - 1
                        proc_operation( value, mode, state_n )
                    end
                end
            end
        end
    end

    return 0
end

--Обработка сохраненного описания операции.
proc_operation = function( value, mode, state_n )
    proc( mode, state_n, value.opened_devices, -1, "opened_devices" )
    proc( mode, state_n, value.opened_reverse_devices, -1, 
        "opened_reverse_devices" )
    proc( mode, state_n, value.closed_devices, -1, "closed_devices" )

    proc_groups( mode, state_n, -1, value.opened_upper_seat_v, 
        "opened_upper_seat_v" )
    proc_groups( mode, state_n, -1, value.opened_lower_seat_v, 
        "opened_lower_seat_v" )

    proc( mode, state_n, value.required_FB,    -1, "required_FB" )

    proc_groups(mode, state_n, -1, value.DI_DO, "DI_DO")
    proc_groups(mode, state_n, -1, value.AI_AO, "AI_AO")

    proc_wash_data(mode, state_n, -1, value)

    if value.steps ~= nil then
        for fields, value in ipairs( value.steps ) do
            mode:AddStep( state_n, value.name or "Шаг ??", value.baseStep or "" )
            local step_n = fields - 1

            proc( mode, state_n, value.opened_devices, step_n,
                "opened_devices" )
            proc( mode, state_n, value.opened_reverse_devices, step_n, 
                "opened_reverse_devices" )
            proc( mode, state_n, value.closed_devices, step_n, 
                "closed_devices" )
            proc_groups( mode, state_n, step_n, value.opened_upper_seat_v,
                "opened_upper_seat_v" )
            proc_groups( mode, state_n, step_n, value.opened_lower_seat_v,
                "opened_lower_seat_v" )
            proc( mode, state_n, value.required_FB, step_n, "required_FB" )

            proc_groups(mode, state_n, step_n, value.DI_DO, "DI_DO")
            proc_groups(mode, state_n, step_n, value.AI_AO, "AI_AO")

            proc_wash_data(mode, state_n, step_n, value)

            local time_param_n = value.time_param_n or 0
            local next_step_n = value.next_step_n or 0

            if time_param_n > 0 then
                mode[ state_n ][ step_n ]:SetPar( time_param_n, next_step_n )
            end
        end
    end
end

proc_params = function( par, par_name, obj )
    if type( par ) == "table" then
        for fields, value in ipairs( par ) do
            local param = obj:GetParamsManager():AddParam(par_name,
            value.name or "Параметр", value.value or 0, value.meter or "шт.",
            value.nameLua or "" )
            if value.oper ~= nil then param:SetOperationN( value.oper ) end
        end
    end
end

proc_system_params = function( par, obj )
    if type ( par ) == "table" then
        for luaName, param in pairs( par ) do
            local parValue = param.value
            obj:AddSystemParameters(luaName, parValue)
        end
    end
end

proc_oper_params = function( par, operation, idx, obj )
    if type( par ) == "table" then
        for fields, value in ipairs( par ) do
            local pr = obj:GetParamsManager():GetParam( value.nameLua or "" )
            if pr ~= nil then
                if type( value.oper ) == "table" then
                    for field, operationNumber in ipairs(value.oper) do
                        if operationNumber == idx then
                            operation:GetOperationParams():AddParam( pr )
                        end
                    end
                elseif type (value.oper) == "number" then
                    if value.oper == idx then
                        operation:GetOperationParams():AddParam( pr )
                    end
                end
            end
        end
    end
end

proc = function( mode, state_n, devices, step_n, action_name, inner_action_index,
    wash_group_index )
    if devices ~= nil then
        if inner_action_index == nil then
            inner_action_index = 0 -- default value in simple action.
        end
        if wash_group_index == nil then
            wash_group_index = 0 -- default value in simple action.
        end

        for _, value in pairs ( devices ) do
            mode[ state_n ][ step_n ]:AddDev( action_name, value,
            inner_action_index, wash_group_index )
        end
    end
end

proc_groups = function( mode, state_n, step_n, groups, action_name )
    --Группа устройств
    if groups ~= nil then
        local group_n = 0
        local wash_group_index = 0

        for _, group in pairs( groups ) do
            for _, v in pairs( group ) do
                mode[ state_n ][ step_n ]:AddDev( action_name, v, group_n,
                    wash_group_index )
            end
            group_n = group_n + 1
        end
    end
end

proc_wash_data = function( mode, state_n, step_n, value )
    local devices_data = value.devices_data -- Новая функциональность.
    local wash_data = value.wash_data -- Старая функциональность.

    if devices_data ~= nil then
        wash_data = devices_data
    end

    --Группа устройств, управляемых по ОС с выдачей сигнала
    if wash_data ~= nil then
        if ( wash_data.DI ~= nil or
             wash_data.DO ~= nil or
             wash_data.devices ~= nil or
             wash_data.rev_devices ~= nil or
             wash_data.pump_freq ~= nil ) then

            local wash_group_index = 0
            proc_wash_group_data( mode, state_n, step_n, wash_data, wash_group_index )
        else -- Process new version
            local wash_group_index = 0
            for _, wash_group in pairs( wash_data ) do
                proc_wash_group_data( mode, state_n, step_n, wash_group, wash_group_index )
                wash_group_index = wash_group_index + 1
            end
        end
    end
end

proc_wash_group_data = function ( mode, state_n, step_n, wash_data, wash_group_index )
    local parent_action = "wash_data"

    --DI
    if wash_data.DI ~= nil then
        local DI_action_index = 0
        proc( mode, state_n, wash_data.DI, step_n, parent_action,
            DI_action_index, wash_group_index  )
    end

    --Control signal DO
    if wash_data.DO ~= nil then
        local DO_action_index = 1
        proc( mode, state_n, wash_data.DO, step_n, parent_action,
            DO_action_index, wash_group_index )
    end

    --On devices.
    if wash_data.devices ~= nil then
        local devices_action_index = 2
        proc( mode, state_n, wash_data.devices, step_n, parent_action,
            devices_action_index, wash_group_index )
    end

    --On reverse devices.
    if wash_data.rev_devices ~= nil then
        local rev_devices_action_index = 3
        proc( mode, state_n, wash_data.rev_devices, step_n, parent_action,
         rev_devices_action_index, wash_group_index )
    end

    --Frequency parameter.
    if wash_data.pump_freq ~= nil then
        mode[ state_n ][ step_n ]:AddParam( parent_action, wash_data.pump_freq,
            wash_group_index )
    end
end