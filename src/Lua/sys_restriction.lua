init_restrictions = function()
    if restrictions == nil then
        return
    end

    for keyObj, valObj in pairs( restrictions ) do
        local object_n            = keyObj
        local obj = Get_TECH_OBJECT( object_n )

        init_restriction(valObj, object_n, obj)
    end

    return 0
end

init_generic_restrictions = function ()
    if generic_restrictions == nil then
        return
    end

    for keyObj, valObj in pairs( generic_restrictions ) do
        local object_n  = keyObj
        local obj = Get_GENERIC_OBJECT( object_n )
        
        init_restriction(valObj, object_n, obj)
    end

    return 0
end

init_restriction = function(valObj, object_n, obj)
    if obj ~= nil then
        for keyM, valM in pairs( valObj ) do
            local mode_n = keyM - 1
            local mode   = obj:GetMode( mode_n )

            if mode ~= nil then
                for keyRM, valRM in pairs ( valM ) do
                    for keyR, valR in pairs(valRM) do
                        if valR == 1 then
                            if keyRM == 1 then
                                mode:AddRestriction("LocalRestriction", object_n, keyR)
                            elseif keyRM == 3 then
                                mode:AddRestriction("NextModeRestriction", object_n, keyR)
                            end
                        else
                            for keyRT, valRT in pairs(valR) do
                                mode:AddRestriction("TotalRestriction", keyR, keyRT)
                            end
                        end
                    end
                end
                mode:SortRestriction()
            end
        end
    end
end