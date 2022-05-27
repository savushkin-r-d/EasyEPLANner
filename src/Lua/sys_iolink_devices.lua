-- Здесь хранятся изделия для устройств.

-- Описание вводимых полей:
-- articleName - название изделия устройства.
-- sizeIn - размер входной области в словах  (WORD).
-- sizeOut - размер выходной области в словах (WORD).

local iolink_devices =
{
    -- V
    { articleName = "AL.9615-4003-06", sizeIn = 2, sizeOut = 0.5 },
    { articleName = "AL.9615-4003-08", sizeIn = 2, sizeOut = 0.5 },
    { articleName = "AL.9615-4004-20", sizeIn = 2, sizeOut = 0.5 },
    { articleName = "DEF.SORIO-1SV", sizeIn = 2, sizeOut = 0.5 },
    -- FQT
    { articleName = "SMR12GGXFRKG/US-100", sizeIn = 4, sizeOut = 0 },
    -- QT
    { articleName = "IFM.LDL100", sizeIn = 6, sizeOut = 0 },
    { articleName = "IFM.LDL200", sizeIn = 6, sizeOut = 0 },
    -- PT
    { articleName = "IFM.PI2715", sizeIn = 1, sizeOut = 0 },
    { articleName = "IFM.PI2794", sizeIn = 1, sizeOut = 0 },
    { articleName = "IFM.PI2797", sizeIn = 1, sizeOut = 0 },
    { articleName = "IFM.PM1704", sizeIn = 2, sizeOut = 0 },
    { articleName = "IFM.PM1705", sizeIn = 2, sizeOut = 0 }, -- PT&LT
    { articleName = "IFM.PM1707", sizeIn = 2, sizeOut = 0 },
    { articleName = "IFM.PM1708", sizeIn = 2, sizeOut = 0 }, -- PT&LT
    { articleName = "IFM.PM1709", sizeIn = 2, sizeOut = 0 },
    { articleName = "IFM.PM1715", sizeIn = 2, sizeOut = 0 },
    { articleName = "FES.8001446", sizeIn = 1, sizeOut = 0 },
    -- LT
    { articleName = "IFM.LR2750", sizeIn = 1, sizeOut = 0 },
    -- TE
    { articleName = "IFM.TA2502", sizeIn = 1, sizeOut = 0 },
    { articleName = "IFM.TA2532", sizeIn = 1, sizeOut = 0 },
    { articleName = "IFM.TA2535", sizeIn = 1, sizeOut = 0 },
    { articleName = "IFM.TA2435", sizeIn = 1, sizeOut = 0 },
    { articleName = "E&H.TM311-AAC0BH2BBB2A1", sizeIn = 2, sizeOut = 0 },
    -- LS
    { articleName = "IFM.LMT100", sizeIn = 1, sizeOut = 0 },
    { articleName = "IFM.LMT102", sizeIn = 1, sizeOut = 0 },
    { articleName = "IFM.LMT104", sizeIn = 1, sizeOut = 0 },
    { articleName = "IFM.LMT105", sizeIn = 1, sizeOut = 0 },
    { articleName = "E&H.FTL33-GR7N2ABW5J", sizeIn = 1, sizeOut = 0 },
    -- DEV_VTUG
    { articleName = "FES.VTUG-10-MSDR-B1Y-25V20-G18FD-DTFD-M7SFD-12K+M1SCVA", sizeIn = 0, sizeOut = 2 },
    { articleName = "FES.VTUG-10-VRLK-B1Y-G18FD-DTFD-M7SFD-12K+SCVA", sizeIn = 0, sizeOut = 2 },
    { articleName = "FES.VTUG-10-VRLK-B1Y-G18FD-DTFD-M7SFD-16K+SCVA", sizeIn = 0, sizeOut = 2 },
    { articleName = "FES.VTUG-10-VRLK-B1Y-G18FD-DTFD-M7SFD-8K+SCVA", sizeIn = 0, sizeOut = 1 },
    { articleName = "FES.VTUG-10-VRLK-B1Y-G18FDL-DTFDL-M7SFD-4K+SCVA", sizeIn = 0, sizeOut = 1 },
    -- F
    { articleName = "PXC.2910411", sizeIn = 4, sizeOut = 1.5 },
    -- VC
    { articleName = "BURKERT.8694", sizeIn = 5, sizeOut = 2.5},
    -- HLA
    { articleName = "PXC.1191993", sizeIn = 0, sizeOut = 1 },
}

return iolink_devices