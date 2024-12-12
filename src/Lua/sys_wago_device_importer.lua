function Import(importer)
    Progress(0);
    CheckTanks(importer)
    ImportDevices(importer)
    Progress(100);
    return 0;
end


--- Импорт танков проекта. Используется для идентификации устройств по танкам
---@param importer any
function CheckTanks (importer)
    if init_tech_objects_modes == nil then
        return
    end

    local objects = init_tech_objects_modes();

    if objects == nil then
        return
    end

    for id, object in pairs(objects) do
       if object.base_tech_object == 'tank' then
        importer:AddTank(object.n)
       end 
    end
end

--- Функция обработчик для импорта устройств проекта
---@param importer any
function ImportDevices(importer)
    if devices == nil then
        return
    end

    for _, device in ipairs(devices) do
        local importDevice = importer:ImportDevice(
            DeviceTypes[device.dtype],
            WagoDeviceTypes[device.dtype],
            device.number,
            ValveSubTypes[device.subtype] or DeviceTypes[device.dtype],
            device.descr)

        local commentAO = {}
        local commentAI = {}
        local commentDO = {}
        local commentDI = {}

        local typeComment = ChannelComments[device.type]
        if ( typeComment ~= nil ) then
            local subtypeComment = typeComment[device.subtype]
            if ( subtypeComment ~= nil ) then
                commentAO = subtypeComment['AO'] or {}
                commentAI = subtypeComment['AI'] or {}
                commentDO = subtypeComment['DO'] or {}
                commentDI = subtypeComment['DI'] or {}
            end
        end


        -- Сигналы AO
        for channel_index, channel in ipairs(device.AO or {}) do
            importDevice:AddChannel("AO", channel.node, channel.offset, commentAO[channel_index] or '')
        end

        -- Сигналы AO
        for channel_index, channel in ipairs(device.AI or {}) do
            importDevice:AddChannel("AI", channel.node, channel.offset, commentAI[channel_index] or '')
        end

        -- Сигналы AO
        for channel_index, channel in ipairs(device.DO or {}) do
            importDevice:AddChannel("DO", channel.node, channel.offset, commentDO[channel_index] or '')
        end

        -- Сигналы AO
        for channel_index, channel in ipairs(device.DI or {}) do
            importDevice:AddChannel("DI", channel.node, channel.offset, commentDI[channel_index] or '')
        end
    end

    -- Запуск генерации страниц EPLAN на основе импортированных устройств
    importer:GenerateDevicesPages();
end


--- Типы устройств.
---  - [index] - тип в старом проекте
---  - = "TYPE" - текущее название типа
DeviceTypes = {
    [0] = "V",      -- Клапан.                      <=  DT_V,    - Клапан.   
    [1] = "M",      -- Мотор.                       <=  DT_N,    - Насос.
    [2] = 'M',      -- Мотор.                       <=  DT_M,    - Мешалка.
    [3] = 'LS',     -- Уровень (есть/нет).          <=  DT_LS,   - Уровень (есть/нет).
    [4] = 'TE',     -- Температура.                 <=  DT_TE,   - Температура.
    [5] = 'FE',     --    ??????????                <=  DT_FE,   - Расход (значение).
    [6] = 'FS',     -- Расход (есть/нет).           <=  DT_FS,   - Расход (есть/нет).
    [7] = 'FQT',    -- Счетчик.                     <=  DT_CTR,  - Счетчик.
    [8] = 'AO',     -- Аналоговый выходной сигнал.  <=  DT_AO,   - Аналоговый выход.
    [9] = 'LT',     -- Уровень (значение).          <=  DT_LE,   - Уровень (значение).
    [10] = 'DI',    -- Дискретный входной сигнал.   <=  DT_FB,   - Обратная связь.
    [11] = 'DO',    -- Дискретный выходной сигнал.  <=  DT_UPR,  - Канал управления.
    [12] = 'QT',    -- Концентрация.                <=  DT_QE,   - Концентрация.
    [13] = 'AI'     -- Аналоговый входной сигнал.   <=  DT_AI    - Аналоговый вход.
}

WagoDeviceTypes = {
    [0] = "V",    -- Клапан.     
    [1] = "N",    -- Насос.  
    [2] = 'M',    -- Мешалка.  
    [3] = 'LS',   -- Уровень (есть/нет).  
    [4] = 'TE',   -- Температура.  
    [5] = 'FE',   -- Расход (значение).  
    [6] = 'FS',   -- Расход (есть/нет).  
    [7] = 'CTR',  -- Счетчик.  
    [8] = 'AO',   -- Аналоговый выход.  
    [9] = 'LE',   -- Уровень (значение).  
    [10] = 'FB',  -- Обратная связь.  
    [11] = 'UPR', -- Канал управления.   
    [12] = 'QE',  -- Концентрация.  
    [13] = 'AI'   -- Аналоговый вход.  
}

--- Подтипы клапанов. Некоторых подтипов в текущем проекте нет.
ValveSubTypes = {
    [0] = nil,

    [1] = 'V_DO1',           -- DST_V_1DO,           1       -- Клапан с одним каналом управления.
    [2] = 'V_DO2',           -- DST_V_2DO,           2       -- Клапан с двумя каналами управления.
    [3] = 'V_DO1_DI1_FB_OFF',-- DST_V_1DO_1DI,       3       -- Клапан с одним каналом управления и одной обратной связью.
    [4] = 'V_DO1_DI2',       -- DST_V_1DO_2DI,       5       -- Клапан с одним каналом управления и двумя обратными связями.
    [5] = 'V_DO2_DI2',       -- DST_V_2DO_2DI,       6       -- Клапан с двумя каналами управления и двумя обратными связями.
    [6] = 'V_MIXPROOF',      -- DST_V_MIX,           7       -- Клапан микспруф.
    [7] = 'V_DO1_DI3',       -- DST_V_1DO_3DI,       ??      -- Клапан с одним каналом управления и тремя обратными связями.
    [8] = 'V_DO1_DI2_S',     -- DST_V_1DO_2DI_S,     ??      -- Клапан с одним каналом управления и двумя обратными связями на одно из состояний.
    [9] = 'V_AS_MIXPROOF',   -- DST_V_AS_MIX         8       -- Клапан с двумя каналами управления и двумя обратными связями с AS интерфейсом (микспруф).
}


--- Описание комментариев каналов ввода/вывода.
ChannelComments = {
    -- V
    [ 0 ] = {
        -- V_DO2
        [ 2 ] = {
            [ 'DO' ] = { 'Закрыть', 'Открыть' }
        },
        -- V_DO1_DI2 
        [ 4 ] = {
            [ 'DI' ] = { 'Закрыт', 'Открыт' }
        },
        -- V_DO2_DI2
        [ 5 ] = {
            [ 'DO' ] = { 'Закрыть', 'Открыть' },
            [ 'DI' ] = { 'Закрыт', 'Открыт' }
        },
        -- V_MIXPROOF
        [ 6 ] = {
            [ 'DO' ] = { 'Открыть', 'Открыть НС', 'Открыть ВС' },
            [ 'DI' ] = { 'Закрыт', 'Открыт' }
        },
    },
    -- M
    [ 1 ] = {
        [ 0 ] = {
            [ 'DO' ] = { 'Пуск' },
            [ 'DI' ] = { 'Обратная связь' }
        }
    },
    -- M
    [ 2 ] = {
        [ 0 ] = {
            [ 'DO' ] = { 'Пуск' },
            [ 'DI' ] = { 'Обратная связь' }
        }
    },
    -- FQT
    [ 7 ] = {
        [ 0 ] = {
            [ 'AI' ] = { 'Объем' }
        }
    },
}