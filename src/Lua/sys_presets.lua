init_presets = function()
    if Recipes == nil then
        return
    end

    for _, recipe in ipairs(Recipes) do
        local preset = ADD_PRESET(recipe.name)

        for tech_object_index, preset_tech_object in pairs(recipe.tech_objects or {}) do
            for param_index, value in pairs(preset_tech_object.params or {}) do
                preset:AddParam(tech_object_index, param_index, value)
            end
        end

        local devices = recipe.devices
        if devices ~= nil then
            for _, dev_name in pairs (devices.opened_devices or {}) do
                preset:AddDev('opened_devices', dev_name)
            end
            for _, dev_name in pairs (devices.closed_devices or {}) do
                preset:AddDev('closed_devices', dev_name)
            end
        end
    end
end