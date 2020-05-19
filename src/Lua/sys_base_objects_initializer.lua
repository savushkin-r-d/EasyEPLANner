init_base_objects = function()
    if base_tech_objects == nil then
        return
    end

    local objects = base_tech_objects
    for field, value in ipairs(objects) do
        local name = value.name or ""
        local eplanName = value.eplanName or ""
        local s88Level = value.s88Level or 0
        local baseOperations = value.baseOperation or { }
        local basicName = value.basicName or ""
        local equipment = value.equipment or { }
        local aggregateParameters = value.aggregateParameters or { }

    end
    --TODO read base tech objects and initialize in C#

    return 0
end