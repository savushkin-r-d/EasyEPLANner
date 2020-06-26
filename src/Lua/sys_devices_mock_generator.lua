-- Генератор заглушек main.devices.lua
devs = {}
devsidx = 0

function V(dev)
	devs[devsidx] = {dev, 'V', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function WT(dev)
	devs[devsidx] = {dev, 'WT', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function VC(dev)
	devs[devsidx] = {dev, 'VC', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function M(dev)
	devs[devsidx] = {dev, 'M', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function LS(dev)
	devs[devsidx] = {dev, 'LS', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function TE(dev)
	devs[devsidx] = {dev, 'TE', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function GS(dev)
	devs[devsidx] = {dev, 'GS', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function FQT(dev)
	devs[devsidx] = {dev, 'FQT', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function QT(dev)
	devs[devsidx] = {dev, 'QT', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function PT(dev)
	devs[devsidx] = {dev, 'PT', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function FS(dev)
	devs[devsidx] = {dev, 'FS', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function LT(dev)
	devs[devsidx] = {dev, 'LT', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function HA(dev)
	devs[devsidx] = {dev, 'HA', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function HL(dev)
	devs[devsidx] = {dev, 'HL', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function SB(dev)
	devs[devsidx] = {dev, 'SB', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function DI(dev)
	devs[devsidx] = {dev, 'DI', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function DO(dev)
	devs[devsidx] = {dev, 'DO', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function AO(dev)
	devs[devsidx] = {dev, 'AO', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function AI(dev)
	devs[devsidx] = {dev, 'AI', devices[devsidx + 1]}
	devsidx = devsidx + 1
	return dev
end

function DEVICE(dev)
	return devs[dev][1]
end