-- ������� ������ shared.lua ��� �������� �������
init_current_project_shared_lua = function()
    local mainModel = GetModel(GetMainProjectName())

    if (remote_gateways) then
        init_remote_gateways(mainModel)
    end

    if (shared_devices) then
        init_shared_devices(mainModel)
    end
end

-- ������������� ��������� ����� ��������
init_remote_gateways = function(mainModel)
    for projectName, table in pairs(remote_gateways) do
        local advancedProjectModel = GetModel(projectName)
        if (advancedProjectModel == nil) then
            advancedProjectModel = CreateModel()
        end

        init_PLC_Data(mainModel, table, projectName)
        advancedProjectModel:AddPLCData(projectName)
        init_currProj_devices(mainModel, table, true, projectName)
    end
end

-- ������������� ������������ �������� ���������
init_shared_devices = function(mainModel)
    for _, table in pairs(shared_devices) do
        local projectName = table.projectName or nil
        if (projectName == nil or projectName == "") then
            return
        end

        local model = GetModel(projectName)
        if (model == nil) then
            model = CreateModel()
        end

        model:AddPLCData(projectName)
        init_currProj_devices(mainModel, table, false, projectName)
    end
end

-- ������������� ��������� ������ ��� �������� �������
-- receiveMode - true or false
init_currProj_devices = function(model, table, receiveMode, projectName)
    if (table.AI) then
        local type = "AI"
        for _, signal in pairs(table.AI) do
            model:AddSignal(signal, type, receiveMode, projectName)
        end
    end

    if (table.AO) then
        local type = "AO"
        for _, signal in pairs(table.AO) do
            model:AddSignal(signal, type, receiveMode, projectName)
        end
    end

    if (table.DI) then
        local type = "DI"
        for _, signal in pairs(table.DI) do
            model:AddSignal(signal, type, receiveMode, projectName)
        end
    end

    if (table.DO) then
        local type = "DO"
        for _, signal in pairs(table.DO) do
            model:AddSignal(signal, type, receiveMode, projectName)
        end
    end
end

-- ������������� ������ � ���, IP � ProjectName ������� �� main.io.lua
init_PLC_Data = function(model, table, projectName)
    -- ��������� ���������� � ��� (����� or ��������� ������)
    local emulationEnabled = table.emulation or false
    local cycleTime = table.cycletime or 200
    local timeout = table.timeout or 300
    local port = table.port or 10502
    local enabledGate = table.enabled or true
    local station = table.station or 0
    model:AddPLCData(emulationEnabled, cycleTime, timeout, port, enabledGate, 
                station, projectName)
end

-------- Init advanced project --------

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
        if(projectName == mainProjectName) then
            local selectedModel = GetSelectedModel()
            init_PLC_Data(selectedModel, table, selectedModel.ProjectName)
            init_advProj_devices(selectedModel, table, true)
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
            local selectedModel = GetSelectedModel()
            init_advProj_devices(selectedModel, table, false)
        end
    end
end

-- ������������� ��������� ��� ��������������� �������
init_advProj_devices = function(model, table, receiveMode)
    if (table.AI) then
        local type = "AO"
        for _, signal in pairs(table.AI) do
            model:AddSignal(signal, type, receiveMode, "")
        end
    end

    if (table.AO) then
        local type = "AI"
        for _, signal in pairs(table.AO) do
            model:AddSignal(signal, type, receiveMode, "")
        end
    end

    if (table.DI) then
        local type = "DO"
        for _, signal in pairs(table.DI) do
            model:AddSignal(signal, type, receiveMode, "")
        end
    end

    if (table.DO) then
        local type = "DI"
        for _, signal in pairs(table.DO) do
            model:AddSignal(signal, type, receiveMode, "")
        end
    end
end