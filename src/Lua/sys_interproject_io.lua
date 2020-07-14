-- Чтение описания main.io.lua для межпроектного обмена
init_io_file = function()
    if (devices == nil or nodes == nil or PAC_name == "") then
        return
    end

    local isMainModel = false
    local model = GetModel(PAC_name)
    if (model == nil) then
        if(GetMainProjectName() == PAC_name) then
            model = CreateMainModel()
            isMainModel = true
        else
            model = CreateModel()
            isMainModel = false
        end
    end

    add_nodes(model, isMainModel)
    add_devices(model)
end

-- Добавление информации об узле
add_nodes = function(model, isMainModel)
    for key, value in pairs(nodes) do
        -- Контроллер PXC и WAGO
        if (value.ntype == 201 or value.ntype == 2) then
            local IP = value.IP or ""

            if(isMainModel == true) then
                model:AddPLCData(IP, PAC_name)
            else
                local mainModel = GetModel(GetMainProjectName())
                mainModel:AddPLCData(IP,PAC_name)
                model:AddPLCData(PAC_name)
            end
        end
    end
end

-- Добавление информации об устройствах
add_devices = function(model)
    for key, value in pairs(devices) do
        local name = value.name or ""
        local descr = value.descr or ""
        model:AddDeviceData(name, descr)
    end
end 