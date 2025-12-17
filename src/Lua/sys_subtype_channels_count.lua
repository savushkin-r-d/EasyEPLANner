-- Список количества логических каналов устройств для каждого подтипа

-- key - название подтипа
-- value (table):
--      DI - количество каналов DI,
--      DO - количество каналов DO,
--      AI - количество каналов AI,
--      AO - количество каналов AO,

local subtype_channels =
{
    -- V
    V_DO1 = { DO = 1, },
    V_DO2 = { DO = 2, },
    V_DO1_DI1_FB_OFF = { DI = 1, DO = 1, },
    V_DO1_DI1_FB_ON = { DI = 1, DO = 1, },
    V_DO1_DI2 = { DI = 2, DO = 1, },
    V_DO2_DI2 = { DI = 2, DO = 2, },
    V_MIXPROOF = { DI = 2, DO = 3, },
    V_AS_MIXPROOF = { DI = 2, DO = 3, },
    V_BOTTOM_MIXPROOF = { DI = 2, DO = 3, },
    V_AS_DO1_DI2 = { DI = 2, DO = 1, },
    V_DO2_DI2_BISTABLE = { DI = 2, DO = 2, },
    V_IOLINK_VTUG_DO1 = { DO = 1, },
    V_IOLINK_VTUG_DO1_FB_OFF = { DI = 1, DO = 1, },
    V_IOLINK_VTUG_DO1_FB_ON = { DI = 1, DO = 1, },
    V_IOLINK_MIXPROOF = { DI = 4, DO = 4, AI = 2, },
    V_IOLINK_DO1_DI2 = { DI = 2, DO = 2, AI = 2, },
    V_IOLINK_VTUG_DO1_DI2 = { DI = 2, DO = 1, },
    V_MINI_FLUSHING = { DI = 2, DO = 2, },
    V_IOL_TERMINAL_MIXPROOF_DO3 = { DO = 3, },
    V_VIRT = { DO =1 },

    -- VC
    VC = { AO = 1, },
    VC_IOLINK = { DI = 1, AI = 2, AO = 1, },
    VC_EY = { AO = 1, },
    VC_VIRT = { AO = 1 },

    -- M
    M = { DI = 1, DO = 1, },
    M_FREQ = { DI = 1, DO = 1, AO = 1, },
    M_REV = { DI = 1, DO = 2, },
    M_REV_FREQ = { DI = 1, DO = 2, AO = 1, },
    M_REV_2 = { DI = 1, DO = 2, },
    M_REV_FREQ_2 = { DI = 1, DO = 2, AO = 1, },
    M_REV_2_ERROR = { DI = 2, DO = 2, },
    M_REV_FREQ_2_ERROR = { DI = 2, DO = 2, AO = 1, },
    M_ATV = { DI = 1, DO = 1, AI = 1, AO = 1, },
    M_ATV_LINEAR = { DI = 1, DO = 1, AI = 1, AO = 1, },
    M_VIRT = { DI = 1, DO = 1 },

    -- LS
    LS_MIN = { DI = 1, },
    LS_MAX = { DI = 1, },
    LS_IOLINK_MIN = { DI = 1, AI = 1, },
    LS_IOLINK_MAX = { DI = 1, AI = 1, },
    LS_VIRT = { DI = 1 },

    -- TE
    TE = { AI = 1, },
    TE_IOLINK = { AI = 2, },
    TE_ANALOG = { AI = 1, },
    TE_VIRT = { AI = 1 },

    -- FS
    FS = { DI = 1, },
    FS_VIRT = { DI = 1, },

    -- GS
    GS = { DI = 1, },
    GS_INVERSE = { DI = 1, },
    GS_VIRT = { DI = 1 },

    -- FQT
    FQT = { AI = 1, },
    FQT_F = { AI = 2, },
    FQT_IOLINK = { AI = 3, },
    FQT_VIRT = { AI = 1 },

    -- LT
    LT = { AI = 1, },
    LT_CYL = { AI = 1, },
    LT_CONE = { AI = 1, },
    LT_TRUNC = { AI = 1, },
    LT_IOLINK = { AI = 2, },
    LT_VIRT = { AI = 1 },

    -- QT
    QT = { AI = 1, },
    QT_OK = { DI = 1, AI = 1, },
    QT_IOLINK = { AI = 3, },
    QT_VIRT = { AI = 1 },

    -- HA
    HA = { DO = 1, },
    HA_VIRT = { DO = 1 },

    -- HL
    HL = { DO = 1, },
    HL_VIRT = { DO = 1 },

    -- SB
    SB = { DI = 1, },
    SB_VIRT = { DI = 1 },

    -- DI
    DI = { DI = 1, },
    DI_VIRT = { DI = 1 },

    -- DO
    DO = { DO = 1, },
    DO_VIRT = { DO = 1 },

    -- AI
    AI = { AI = 1, },
    AI_VIRT = { AI = 1 },

    -- AO
    AO = { AO = 1, },
    AO_EY = { AO = 1, },
    AO_VIRT = { AO = 1 },

    -- WT
    WT = { AI = 1, },
    WT_RS232 = { AI = 1, },
    WT_ETH = { AI = 1, },
    WT_PXC_AXL = { AI = 1, },
    WT_VIRT = { AI = 1 },

    -- PT
    PT = { AI = 1, },
    PT_IOLINK = { AI = 2, },
    PT_VIRT = { AI = 1 },

    -- F
    F = { AI = 4, AO = 4, },
    F_VIRT = { AI = 4, AO = 4 },

    -- HLA
    HLA_IOLINK = { AO = 1, },

    -- PDS
    PDS = { DI = 1, },
    PFS_VIRT = { DI = 1 },

    -- TS
    TS = { DI = 1, },
    TS_VIRT = { DI = 1 },

    -- G
    G_IOL_4 = { AI = 4, AO = 4, },
    G_IOL_8 = { AI = 8, AO = 8, },

    -- WATCHDOG
    WATCHDOG = { AI = 1, AO = 1, },

    -- EY
    DEV_CONV_AO2 = { AI = 1, AO = 2, },

    -- Y
    DEV_VTUG_8 = { DO = 8, },
    DEV_VTUG_16 = { DO = 16, },
    DEV_VTUG_24 = { DO = 24, },
}

for subtype, channels in pairs(subtype_channels) do
    ADD_CHANNELS_COUNT(subtype, channels["DI"] or 0, channels["DO"] or 0, channels["AI"] or 0, channels["AO"] or 0 )
end