function Import(importer)
    import_nodes(importer)

    Progress(100);
    return 0;
end


function import_nodes(importer)
    if nodes == nil then
        return
    end

    for node_index, node in ipairs(nodes) do
        importer:ImportNode (node.ntype, node.address, node.IP or '')
        for module_index, module in ipairs(node.modules or {}) do
            importer:ImportModule(module[1], node_index, module_index)
            Progress(100 / #nodes * (node_index - 1) + 100 / #nodes / #node.modules * (module_index - 1))
        end
    end
end
