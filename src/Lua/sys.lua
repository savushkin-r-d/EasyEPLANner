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

            --Оборудование
            if value.equipment ~= nil then
                for field, value in pairs( value.equipment ) do
                    obj:AddEquipment(field, value)
                end
            end

            --Параметры
            proc_params( value.par_float, "par_float", obj )
            proc_params( value.par_uint, "par_uint", obj )
            proc_params( value.rt_par_float, "rt_par_float", obj )
            proc_params( value.rt_par_uint, "rt_par_uint", obj)

            local params_float = value.par_float
            
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


proc = function( mode, state_n, devices, step_n, action_name )
    if devices ~= nil then
        for field, value in pairs ( devices ) do
            mode[ state_n ][ step_n ]:AddDev( action_name, value )
        end
    end
end

proc_groups = function( mode, state_n, step_n, groups, n )
    --Группа устройств
    if groups ~= nil then
        local group_n = 0
        for field, group in pairs( groups ) do
            for field, v in pairs( group ) do
                mode[ state_n ][ step_n ]:AddDev( n, v, group_n )
            end
            group_n = group_n + 1
        end
    end
end

proc_wash_data = function( mode, state_n, step_n, value)
    --Группа устройств, управляемых по ОС с выдачей сигнала
    if value.wash_data ~= nil then
        --DI
        if value.wash_data.DI ~= nil then
            mode[ state_n ][ step_n ]:AddDev( "wash_data", 
                value.wash_data.DI[ 1 ], 0 )
        end

        --Control signal DO
        if value.wash_data.DO ~= nil then
            for field, value in pairs( value.wash_data.DO ) do
                mode[ state_n ][ step_n ]:AddDev( "wash_data", value, 1 )
            end
        end

        --On devices.
        if value.wash_data.devices ~= nil then
            for field, value in pairs( value.wash_data.devices ) do
                mode[ state_n ][ step_n ]:AddDev( "wash_data", value, 2 )
            end
        end

        --On reverse devices.
        if value.wash_data.rev_devices ~= nil then
            for field, value in pairs( value.wash_data.rev_devices ) do
                mode[ state_n ][ step_n ]:AddDev( "wash_data", value, 3 )
            end
        end

        --Frequency parameter.
        if value.wash_data.pump_freq ~= nil then
           mode[ state_n ][ step_n ]:AddParam( "wash_data", 1,
               value.wash_data.pump_freq )
        end
    end
end
