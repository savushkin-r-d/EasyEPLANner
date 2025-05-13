local modbus =
{
    modbusstep = 0,
}

--- Инициализация Modbus-клиента
function modbus:Init()
    if G_PAC_INFO():is_emulator() then
        self.mc = modbus_client( 151, '127.0.0.1', self.port, 300 )
    else
        self.mc = modbus_client( 151, self.ip, self.port, 300 )
    end
    self:InitExchangeFunctions()
end

--- Вычисление
function modbus:Evaluate()
    if not G_PAC_INFO():is_emulator() then
        self:Exchange()
    end
end

--- Инициализация функций чтения/записи для сигналов
function modbus:InitExchangeFunctions()
    for _, group in ipairs(self.read) do
        for signal_index, signal in ipairs(group) do
            local dev_name, type, word, bit = signal[2], signal[3], signal[4], signal[5]

            if dev_name == '' then
                signal.read = function( ) end -- do nothing
            else
                local function_body
                if type == 'Int' then
                    function_body = '__'..dev_name..':set_state( self.mc:get_int2( '..word..' ) )'
                elseif type == 'Real' then
                    function_body = '__'..dev_name..':set_value( self.mc:get_float( '..word..' ) / 65536 )'
                elseif type == 'Float' then
                    function_body = '__'..dev_name..':set_value( self.mc:get_float( '..word..' ) )'
                elseif type == 'Bool' then
                    function_body = '__'..dev_name..':set_state( self.mc:reg_get_bit( '..word..', '..bit..' ) )'
                else
                    function_body = ''
                end
                signal.read = StrToFunction( 'function( self ) '..function_body..' end' )
            end

            group[ signal_index ] = setmetatable( signal, { __index = self } )
        end
    end

    for _, group in ipairs(self.write) do
        for signal_index, signal in ipairs(group) do
            local dev_name, type, word, bit = signal[2], signal[3], signal[4], signal[5]

            if dev_name == '' then
                signal.write = function( ) end -- do nothing
            else
                local function_body
                if type == 'Int' then
                    function_body = 'self.mc:set_int2( '..word..', '..dev_name..':get_state() )'
                elseif type == 'Real' then
                    function_body = 'self.mc:set_float( '..word..', '..dev_name..':get_value() / 65536 )'
                elseif type == 'Float' then
                    function_body = 'self.mc:set_float( '..word..', '..dev_name..':get_value() )'
                elseif type == 'Bool' then
                    function_body = 'self.mc:reg_set_bit( '..word..', '..bit..', '..dev_name..':get_state() )'
                else
                    function_body = ''
                end
                signal.write = StrToFunction( 'function( self ) '..function_body..' end' )
            end

            group[ signal_index ] = setmetatable( signal, { __index = self } )
        end
    end
end

---Загрузить функцию из строки
---@param str any строка с функцией
---@return function function функция
function StrToFunction( str )
    str = 'return '..str

    local success, f = pcall( loadstring, str )
    if success == false then
        _, f = pcall( load, str )
    end
    _, result = pcall( f )
    return result
end

--- Обмен
function modbus:Exchange()
    -- Начальный шаг
    if self.modbusstep == 0 then
        self.mc:set_station( 1 )
        self.modbusstep = 1
        return
    end

    if self.modbusstep <= #self.read then
        local group = self.read[ self.modbusstep ]
        if 1 == self.mc:async_read_holding_registers( self.read.offset + group.offset, Area(group) ) then
            for _, signal in ipairs( group ) do
                signal:read()
            end

            self.modbusstep = self.modbusstep + 1
            self.mc:zero_output_buff()
        end
    elseif self.modbusstep - #self.read <= #self.write then
        local group = self.write[ self.modbusstep - #self.read ]
        for _, signal in ipairs( group ) do
            signal:write()
        end
        if 1 == self.mc:async_write_multiply_registers( self.write.offset + group.offset, Area(group) ) then

            self.modbusstep = self.modbusstep + 1
            self.mc:zero_output_buff()
        end
    end

    if self.modbusstep > #self.read + #self.write then
        self.modbusstep = 1
    end
end

---Область для чтения/записи данных
---@param g table таблица со списком сигналов
---@return integer area размер области для чтения/записи данных 
function Area(g)
    local lastSignal = g[ #g ]
    local data_type, word = lastSignal[ 3 ], lastSignal[ 4 ]
    return word + (data_type == 'Real' or data_type == 'Float') and 2 or 1
end

return { __index = modbus }