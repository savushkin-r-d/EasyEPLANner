-- ������� ������ shared.lua ��� �������� �������
init_current_project_shared_lua = function()
    if (remote_gateways) then
        init_remote_gateways()
    end

    if (shared_devices) then
        init_shared_devices()
    end
end

-- ������������� ��������� ����� ��������
init_remote_gateways = function()
    for projectName, table in pairs(remote_gateways) do
        init_project_as_receiver(projectName, table)
    end
end

-- ������������� �������� ������� ��� ���������� ��������
init_project_as_receiver = function(projectName, table)
    local model = GetModel(projectName)
    if (model == nil) then
        model = CreateModel()
    end

    init_PLC_Data(model, table, projectName)
    init_currProj_devices(model, table, false)
end

-- ������������� ��������� ������ ��� �������� �������
-- receiveMode - true or false
init_currProj_devices = function(model, table, receiveMode)
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

-- ������������� ������ � ���, IP � ProjectName ������� �� main.io.lua
init_PLC_Data = function(model, table, projectName)
    -- ��������� ���������� � ��� (����� or ��������� ������)
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

-- ������������� ������������ �������� ���������
init_shared_devices = function()
    for _, table in pairs(shared_devices) do
        init_project_as_source(table)
    end
end

-- ������������� �������� ������� ��� ��������� ��������
init_project_as_source = function(table)
    local projectName = table.projectName or nil
    if (projectName == nil or projectName == "") then
        return
    end

    local model = GetModel(projectName)
    if (model == nil) then
        model = CreateModel()
    end

    model:AddPLCData(projectName)
    init_currProj_devices(model, table, true)
end

--------

-- ������� ������ shared.lua ��� ��������������� �������
init_advanced_project_shared_lua = function()
    local mainProjectName = GetMainProjectName()
    if (remote_gateways) then
        init_advProj_remote_gateways(mainProjectName)
    end

    if (shared_devices) then
        init_advProj_shared_devices(mainProjectName)
    end
end

-- ������������� ���������� ���� ������� ��� ��������������� �������
init_advProj_remote_gateways = function(mainProjectName)
    for projectName, table in pairs(remote_gateways) do
        if (projectName == mainProjectName) then
            local model = GetModel(projectName)
            init_PLC_Data(model, table, projectName)
            init_advProj_devices(model, table, true)
        end
    end
end

-- ������������� ��������� ��� ��������������� �������
init_advProj_devices = function(model, table, receiveMode)
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

-- ������������� ������������ ��������� ��� ��������������� �������
init_advProj_shared_devices = function(mainProjectName)
    for _, table in pairs(shared_devices) do
        local projectName = table.projectName or nil
        if (projectName == nil) then
            return
        end

        if (projectName == mainProjectName) then
            local model = GetModel(projectName)
            init_advProj_devices(model, table, false)
        end
    end
end