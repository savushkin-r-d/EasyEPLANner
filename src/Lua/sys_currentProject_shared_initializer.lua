-- Функция чтения shared.lua для текущего проекта
init_shared_lua_file = function()
    if (remote_gateways) then
        init_remote_gateways()
    end

    if (shared_devices) then
        init_shared_devices()
    end
end

-- Инициализация удаленных узлов текущего
init_remote_gatewasys = function()
    for projectName, table in pairs(remote_gateways) do
        init_project_as_receiver(projectName, table)
    end
end

-- Инициализация текущего проекта как получателя сигналов
init_project_as_receiver = function(projectName, table)
    local model = GetModel(projectName)
    if (model == nil) then
        model = CreateModel()
    end

    init_PLC_Data(model, table)
    init_devices(model, table, false)
end

-- Инициализация устройств модели
-- receiveMode - true or false
init_devices = function(model, table, receiveMode)
    if (table.AI) then
        local type = "AI"
        for _, signal in pairs(table.AI) do
            model:AddSignal(signal, type, receiveMode)
        end
    end

    if (table.AO) then
        local type = "AO"
        for _, signal in pairs(table.AO) do
            model:AddSignal(signal, type, receiveMode)
        end
    end

    if (table.DI) then
        local type = "DI"
        for _, signal in pairs(table.DI) do
            model:AddSignal(signal, type, receiveMode)
        end
    end

    if (table.DO) then
        local type = "DO"
        for _, signal in pairs(table.DO) do
            model:AddSignal(signal, type, receiveMode)
        end
    end
end

-- Инициализация данных о ПЛК, IP и ProjectName берутся из main.io.lua
init_PLC_Data = function(model, table)
    -- Заполняем информацию о ПЛК (после or дефолтные данные)
    local emulationEnabled = table.emulation or false
    local cycleTime = table.cycletime or 200
    local timeout = table.timeout or 300
    local port = table.port or 10502
    local enabledGate = table.enabled or true
    local station = table.station or 0
    model:AddPLCData(emulationEnabled, cycleTime, timeout,
               port, enabledGate, station)
end

-- Инициализация передаваемых проектом устройств
init_shared_devices = function()
    for stationNum, table in pairs(shared_devices) do
        init_project_as_source(stationNum, table)
    end
end

-- Инициализация текущего проекта как источника сигналов
init_project_as_source = function(stationNum, table)
    local projectName = table.projectName or nil
    if (projectName == nil or projectName == "") then
        return
    end

    local model = GetModel(projectName)
    if (model == nil) then
        model = CreateModel()
    end

    model:AddPLCData(projectName)
    init_devices(model, table, true)
end