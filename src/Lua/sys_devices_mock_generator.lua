-- Генератор заглушек main.devices.lua
devs = {}
devsidx = 0

function DEVICE(dev)
	return devs[dev][1] or { }
end

function V(dev)
	devs[devsidx] = {dev, 'V'}
	devsidx = devsidx + 1
	return dev
end

function WT(dev)
	devs[devsidx] = {dev, 'WT'}
	devsidx = devsidx + 1
	return dev
end

function VC(dev)
	devs[devsidx] = {dev, 'VC'}
	devsidx = devsidx + 1
	return dev
end

function M(dev)
	devs[devsidx] = {dev, 'M'}
	devsidx = devsidx + 1
	return dev
end

function LS(dev)
	devs[devsidx] = {dev, 'LS'}
	devsidx = devsidx + 1
	return dev
end

function TE(dev)
	devs[devsidx] = {dev, 'TE'}
	devsidx = devsidx + 1
	return dev
end

function GS(dev)
	devs[devsidx] = {dev, 'GS'}
	devsidx = devsidx + 1
	return dev
end

function FQT(dev)
	devs[devsidx] = {dev, 'FQT'}
	devsidx = devsidx + 1
	return dev
end

function QT(dev)
	devs[devsidx] = {dev, 'QT'}
	devsidx = devsidx + 1
	return dev
end

function PT(dev)
	devs[devsidx] = {dev, 'PT'}
	devsidx = devsidx + 1
	return dev
end

function FS(dev)
	devs[devsidx] = {dev, 'FS'}
	devsidx = devsidx + 1
	return dev
end

function LT(dev)
	devs[devsidx] = {dev, 'LT'}
	devsidx = devsidx + 1
	return dev
end

function HA(dev)
	devs[devsidx] = {dev, 'HA'}
	devsidx = devsidx + 1
	return dev
end

function HL(dev)
	devs[devsidx] = {dev, 'HL'}
	devsidx = devsidx + 1
	return dev
end

function SB(dev)
	devs[devsidx] = {dev, 'SB'}
	devsidx = devsidx + 1
	return dev
end

function DI(dev)
	devs[devsidx] = {dev, 'DI'}
	devsidx = devsidx + 1
	return dev
end

function DO(dev)
	devs[devsidx] = {dev, 'DO'}
	devsidx = devsidx + 1
	return dev
end

function AO(dev)
	devs[devsidx] = {dev, 'AO'}
	devsidx = devsidx + 1
	return dev
end

function AI(dev)
	devs[devsidx] = {dev, 'AI'}
	devsidx = devsidx + 1
	return dev
end

function C(dev)
	devs[devsidx] = {dev, "C"}
	devsidx = devsidx + 1
	return dev
end

function F(dev)
	devs[devsidx] = {dev, "F"}
	devsidx = devsidx + 1
	return dev
end

function HLA(dev)
	devs[devsidx] = {dev, "HLA"}
	devsidx = devsidx + 1
	return dev
end

function CAM(dev)
	devs[devsidx] = {dev, "CAM"}
	devsidx = devsidx + 1
	return dev
end

function PDS(dev)
	devs[devsidx] = {dev, "PDS"}
	devsidx = devsidx + 1
	return dev
end
