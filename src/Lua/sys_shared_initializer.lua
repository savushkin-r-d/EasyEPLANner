-- Функция чтения shared.lua
init_shared_lua_file = function()
    if (remote_gateways) then
        init_remote_gateways()
    end

    if (shared_devices) then
        init_shared_devices()
    end
end

-- Инициализация удаленных узлов
init_remote_gatewasys = function()
    for projectName, table in pairs(remote_gateways) do
        init_project_as_receiver(projectName, table)
    end
end

-- Инициализация проекта как получателя сигналов
init_project_as_receiver = function(projectName, table)
    local model = GetModel(projectName)
    if (model == nil) then
        model = CreateModel()
    end

    init_PLC_Data(model, table)


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

-- Инициализация передаваемых устройств
init_shared_devices = function()
    for stationNum, table in pairs(shared_devices) do
        init_project_as_source(stationNum, table)
    end
end