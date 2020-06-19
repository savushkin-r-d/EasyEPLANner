-- Чтение описания main.io.lua
init_io_file = function()
    if (devices == nil or nodes == nil or PAC_name == "") then
        return
    end

    local model = CreateModel()
    add_nodes(model)
    add_devices(model)
end

-- Добавление информации об узле
add_nodes = function(model)
    for key, value in pairs(nodes) do
        if (value.ntype == 201 or value.ntype == 2) then
            local IP = value.IP or ""
            model:AddPLCData(IP, PAC_name)
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