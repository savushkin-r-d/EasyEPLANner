function init()
    if init_tech_objects_modes == nil then
        return
    end

    local objects = init_tech_objects_modes()
    local initialized_objects = { }
	for number, value in ipairs(objects) do
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

    for fields, value in ipairs(objects) do
        -- Аналогично с предыдущим циклом.
        if (#value == 0) then
            local obj = initialized_objects[fields]

            --Параметры
            proc_params(value.par_float, "par_float", obj)
            proc_params(value.par_uint, "par_uint", obj)
            proc_params(value.rt_par_float, "rt_par_float", obj)
            proc_params(value.rt_par_uint, "rt_par_uint", obj)

            -- Системные параметры
            proc_system_params(value.system_parameters, obj)

            -- Свойства объекта
            proc_object_properties(value.properties, obj)

            local params_float = value.par_float

            --Оборудование (всегда после параметров)
            if value.equipment then
                for field, value in pairs(value.equipment) do
                    obj:SetEquipment(field, value)
                end
            end

            if value.tank_groups then
                for luaName, value in pairs(value.tank_groups) do
                    obj:SetGroupObject(luaName, value)
                end
            end

            for fields, value in ipairs(value.modes) do
                local mode_name = value.name or "Операция ??"
			    local mode_base_operation = value.base_operation or ""

			    -- Доп. свойства по операции
			    local mode_base_operation_props = {}
			    if value.props then
				    for fields, value in pairs(value.props) do
					    mode_base_operation_props[fields] = value 
				    end
			    end

                local mode = obj:AddMode(mode_name, mode_base_operation, 
                    mode_base_operation_props)

                local idx = fields
                proc_oper_params(params_float, mode, idx, obj)

                if value.states then
                    for fields, value in pairs(value.states) do
                        local state_n = fields - 1
                        proc_operation_devices(value, mode, state_n)
                    end
                end
            end
        end
    end

    return 0
end

function proc_params(par, par_name, obj)
    if type(par) == "table" then
        for fields, value in ipairs(par) do
            local param = obj:GetParamsManager():AddParam(par_name,
                value.name or "Параметр", value.value or 0,
                value.meter or "шт.", value.nameLua or "")
            if value.oper then param:SetOperationN(value.oper) end
        end
    end
end

function proc_system_params(par, obj)
    if type (par) == "table" then
        for luaName, param in pairs(par) do
            local parValue = param.value
            obj:SetSystemParameter(luaName, parValue)
        end
    end
end

function proc_object_properties(par, obj)
    if type (par) == "table" then
        for luaName, value in pairs(par) do
            obj:SetBaseProperty(luaName, value)
        end
    end
end

--Обработка сохраненного описания операции
function proc_oper_params(par, operation, idx, obj)
    if type(par) == "table" then
        for fields, value in ipairs(par) do
            local pr = obj:GetParamsManager():GetParam(value.nameLua or "")
            if pr then
                if type(value.oper) == "table" then
                    for field, operationNumber in ipairs(value.oper) do
                        if operationNumber == idx then
                            operation:GetOperationParams():AddParam(pr)
                        end
                    end
                elseif type (value.oper) == "number" then
                    if value.oper == idx then
                        operation:GetOperationParams():AddParam(pr)
                    end
                end
            end
        end
    end
end

function proc_operation_devices(value, mode, state_n)
    proc_actions(mode, state_n, -1, value)
    if value.steps then
        for fields, value in ipairs(value.steps) do
            mode:AddStep(state_n, value.name or "Шаг ??", value.baseStep or "")
            local step_n = fields - 1

            proc_actions(mode, state_n, step_n, value)

            local next_step_n = value.next_step_n or 0
            local time_param_n = value.time_param_n or 0
            if time_param_n > 0 then
                mode[state_n][step_n]:SetPar(time_param_n, next_step_n)
            end
        end
    end
end

function proc_actions( mode, state_n, step_n, value )
    proc(mode, state_n, step_n, value, "checked_devices")
    proc(mode, state_n, step_n, value, "opened_devices")
    proc(mode, state_n, step_n, value, "opened_reverse_devices")
    proc(mode, state_n, step_n, value, "closed_devices")
    proc_groups(mode, state_n, step_n, value, "opened_upper_seat_v")
    proc_groups(mode, state_n, step_n, value, "opened_lower_seat_v")
    proc(mode, state_n, step_n, value, "required_FB")
    proc_groups(mode, state_n, step_n, value, "DI_DO")
    proc_groups(mode, state_n, step_n, value, "AI_AO")
    proc_wash_data(mode, state_n, step_n, value)

    local notRuntimeStep = step_n >= 0
    if (notRuntimeStep) then
        proc_to_step_by_condition(mode, state_n, step_n, value)
    end
end

function proc(mode, state_n, step_n, actions, action_name, group_number, sub_action_name)
    if (group_number == nil) then
        group_number = 0
    end

    if (sub_action_name == nil) then
        sub_action_name = ""
    end

    if (actions == nil) then
        return
    end

    for current_action_name, devices in pairs (actions) do
        if (current_action_name == action_name and devices ~= nil) then
            for _, dev_name in pairs (devices) do
                add_dev(mode, state_n, step_n, action_name, dev_name,
                    group_number, sub_action_name)
            end  
        end
    end
end

function proc_groups(mode, state_n, step_n, actions, action_name)
    if (actions == nil) then
        return
    end

    for current_action_name, groups in pairs (actions) do
        if (current_action_name == action_name and groups ~= nil) then
            local group_n = 0
            for _, group in pairs(groups) do
                for _, dev_name in pairs(group) do
                    add_dev(mode, state_n, step_n, action_name, dev_name,
                        group_n, "")
                end
                group_n = group_n + 1
            end 
        end
    end  
end

function proc_wash_data(mode, state_n, step_n, value)
    local devices_data = value.devices_data -- Новая функциональность.
    local wash_data = value.wash_data -- Старая функциональность.

    if devices_data then
        wash_data = devices_data
    end

    --Группа устройств, управляемых по ОС с выдачей сигнала
    if wash_data then
        if (wash_data.DI or
            wash_data.DO or
            wash_data.devices or
            wash_data.rev_devices or
            wash_data.pump_freq) then

            local group_number = 0
            proc_wash_group_data(mode, state_n, step_n, wash_data, group_number)
        else -- Process new version
            local group_number = 0
            for _, wash_group in pairs(wash_data) do
                proc_wash_group_data(mode, state_n, step_n, wash_group, group_number)
                group_number = group_number + 1
            end
        end
    end
end

function proc_wash_group_data(mode, state_n, step_n, wash_data, group_number)
    local action_name = "devices_data"
    for sub_action_name, devices in pairs(wash_data) do
        if (type(devices) == "table") then
            for _, dev_name in pairs(devices) do
                add_dev(mode, state_n, step_n, action_name, dev_name, group_number,
                    sub_action_name)
            end
        end
    end

    --Frequency parameter.
    if wash_data.pump_freq then
        mode[state_n][step_n]:AddParam(action_name, wash_data.pump_freq,
            group_number)
    end
end

function proc_to_step_by_condition(mode, state_n, step_n, value)
    local actions = value.to_step_if_devices_in_specific_state
    local action_name = "to_step_if_devices_in_specific_state"

    if actions then
        for sub_action_name, devices in pairs(action_name) do
            for _, dev_name in pairs(devices) do
                add_dev(mode, state_n, step_n, action_name, dev_name, 0, sub_action_name)
            end
        end
    end
end

function add_dev(mode, state_n, step_n, action_name, dev_name, group_number, sub_action_name)
	mode[state_n][step_n]:AddDev(action_name, dev_name, group_number,
        sub_action_name)
end