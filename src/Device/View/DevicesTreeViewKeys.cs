using EasyEPlanner.Devices.ViewModel;

namespace EasyEPlanner.Devices.View
{
    /// <summary>
    /// Стабильные ключи узлов дерева устройств для сохранения развёртки.
    /// </summary>
    public static class DevicesTreeViewKeys
    {
        public static string GetViewItemKey(object obj) => obj switch
        {
            DevicesRoot => "root",
            DevicesTypeGroupNode typeGroup => $"type:{typeGroup.TypeKey}",
            DevicesObjectGroupNode objectGroup => $"object:{objectGroup.ObjectKey}",
            DevicesDeviceNode deviceNode => $"device:{deviceNode.Device.EplanName}",
            DevicesGroupNode groupNode => GetGroupNodeKey(groupNode),
            _ => null,
        };

        private static string GetGroupNodeKey(DevicesGroupNode groupNode)
        {
            var deviceKey = GetAncestorDeviceKey(groupNode);
            return deviceKey is null ? null : $"group:{deviceKey}:{groupNode.Name}";
        }

        private static string GetAncestorDeviceKey(FilterableViewItemBase item)
        {
            while (item is not null)
            {
                if (item is DevicesDeviceNode deviceNode)
                    return deviceNode.Device.EplanName;

                item = item.ParentItem;
            }

            return null;
        }
    }
}
