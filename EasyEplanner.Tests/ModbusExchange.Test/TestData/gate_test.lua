-- test version

local gate =
{
    ip = '127.0.0.1',
    port = 500,
    read =
    {
        offset = 0,
        {
            name = '',
            offset = 0,
            { 0, 0, 'Word', '', 'Сигнал 1' },
        },
        {
            name = 'Группа 1',
            offset = 1,
            { 0, 0, 'Word', '', 'Сигнал 2' },
        },
    },
    write =
    {
        offset = 0,
        {
            name = '',
            offset = 100,
            { 0, 0, 'Word', '', 'Сигнал 3' },
        },
        {
            name = 'Группа 1',
            offset = 1,
            { 0, 0, 'Word', '', 'Сигнал 4' },
        },
    },
}

local s, mt = pcall( require, 'modbusexchange' )
mt = s and mt or { }
return setmetatable( gate, mt )