-- Функция чтения shared.lua для альтернативного проекта
init_advanced_project_shared_lua = function()
    local mainProjectName = GetMainProjectName()
    if (remote_gateways) then
        init_remote_gateways(mainProjectName)
    end

    if (shared_devices) then
        init_shared_devices(mainProjectName)
    end
end

-- Инициализация удаленного узла проекта для альтернативного проекта
init_remote_gateways = function(mainProjectName)
    for projectName, table in pairs(remote_gateways) do
        if (projectName == mainProjectName) then
            local model = GetModel(projectName)
            init_PLC_data(model, table)
            init_devices(model, table, true)
        end
    end
end

-- Инициализация данных о ПЛК для альтернативного проекта
init_PLC_data = function(model, table)
    local IP = table.IP or ""
    local emulationEnabled = table.emulation or false
    local cycleTime = table.cycletime or 200
    local timeout = table.timeout or 300
    local port = table.port or 10502
    local enabledGate = table.enabled or true
    local station = table.station or 0
    model:AddPLCData(IP, projectName)
    model:AddPLCData(emulationEnabled, cycleTime, timeout,
       port, enabledGate, station)
end

-- Инициализация устройств для альтернативного проекта
init_devices = function(model, table, receiveMode)
    if (table.AI) then
        local type = "AO"
        for _, signal in pairs(table.AI) do
            model:AddSignal(signal, type, receiveMode)
        end
    end

    if (table.AO) then
        local type = "AI"
        for _, signal in pairs(table.AO) do
            model:AddSignal(signal, type, receiveMode)
        end
    end

    if (table.DI) then
        local type = "DO"
        for _, signal in pairs(table.DI) do
            model:AddSignal(signal, type, receiveMode)
        end
    end

    if (table.DO) then
        local type = "DI"
        for _, signal in pairs(table.DO) do
            model:AddSignal(signal, type, receiveMode)
        end
    end
end

-- Инициализация передаваемых устройств для альтернативного проекта
init_shared_devices = function(mainProjectName)
    for _, table in pairs(shared_devices) do
        local projectName = table.projectName or nil
        if (projectName == nil) then
            return
        end

        if (projectName == mainProjectName) then
            local model = GetModel(projectName)
            init_devices(model, table, false)
        end
    end
end