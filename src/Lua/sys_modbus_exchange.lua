function init ( model )
    if gate == nil then
        return
    end

    model.IP = gate.ip or ''
    model.Port = gate.port or 0

    init_main_group( model.Read, gate.read or { } )
    init_main_group( model.Write, gate.write or { } )
end

function init_main_group( model_group, group )
    model_group.offset = group.offset or 0
    for _, subgroup in ipairs( group ) do
        local name = subgroup.name or ''
        local g = GetGroup( model_group, name )
        g.offset = subgroup.offset or 0
        for _, signal in ipairs( subgroup ) do
            local word, bit, data_type, device_name, description =
                signal[1], signal[2], signal[3], signal[4], signal[5]

            AddSignal( g, description, device_name, data_type, word, bit )
        end
    end
end