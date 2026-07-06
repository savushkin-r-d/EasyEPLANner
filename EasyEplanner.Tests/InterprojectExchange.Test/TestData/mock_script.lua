init_io_file = function(projectName)
    if (devices == nil or nodes == nil or PAC_name == "") then
        return
    end

    local model = CreateMainModel()
    model.ProjectName = PAC_name

    for key, value in pairs(devices) do
        local name = value.name or ""
        local descr = value.descr or ""
        local dtype = value.dtype or -1
        local subtype = value.subtype or 0
        model:AddDeviceData(name, descr, dtype, subtype)
    end
    model:SortDeviceData()
end
