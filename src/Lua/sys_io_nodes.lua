-- �������� ����� �����-������

-- constructor description:
-- name - �������� ���� �����-������ (eg. 750-863, AXL F BK ETH, AXC F 1152, etc.).
-- isCoupler - �������� �� ���� �������� (default value - false).
-- type - ��� ���������� (������� �� type enumeration).

-- type enumeration:
-- -1 - �� ���������.
-- 0 - ������ � ����������� 750-863.
-- 2 - ������ � ����������� PFC200.
-- 100 - ��������� Ethernet ����.
-- 200 - ������ � ����������� Phoenix Contact.
-- 201 - ���������� Phoenix Contact

local io_nodes =
{
    [ 1 ] =
    {
        name = "750-863",
        isCoupler = false,
        type = 0
    },
    [ 2 ] =
    {
        name = "750-341",
        isCoupler = true,
        type = 100
    },
    [ 3 ] =
    {
        name = "750-352",
        isCoupler = true,
        type = 100
    },
    [ 4 ] =
    {
        name = "750-841",
        isCoupler = false,
        type = 100
    },
    [ 5 ] =
    {
        name = "750-8202",
        isCoupler = false,
        type = 2
    },
    [ 6 ] =
    {
        name = "750-8203",
        isCoupler = false,
        type = 2
    },
    [ 7 ] =
    {
        name = "750-8204",
        isCoupler = false,
        type = 2
    },
    [ 8 ] =
    {
        name = "750-8206",
        isCoupler = false,
        type = 2
    },
    [ 9 ] =
    {
        name = "AXL F BK ETH",
        isCoupler = true,
        type = 200
    },
    [ 10 ] =
    {
        name = "AXL F BK ETH NET2",
        isCoupler = true,
        type = 200
    },
    [ 11 ] =
    {
        name = "AXC F 1152",
        isCoupler = false,
        type = 201
    },
    [ 12 ] =
    {
        name = "AXC F 2152",
        isCoupler = false,
        type = 201
    },
    [ 13 ] =
    {
        name = "AXC F 3152",
        isCoupler = false,
        type = 201
    }
}

return io_nodes