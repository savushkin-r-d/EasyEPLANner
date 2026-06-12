using BrightIdeasSoftware;
using EasyEPlanner.Devices.ViewModel;
using IO.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace EasyEPlanner.Devices.View
{
    public partial class DevicesViewControl
    {
        private sealed class DevicesTreeViewState
        {
            public int TopItemIndex { get; set; } = -1;

            public Point ScrollPosition { get; set; }

            public HashSet<string> ExpandedKeys { get; set; }

            public string SelectedKey { get; set; }
        }

        private void ApplyTreeViewStateAfterUpdate(Action updateAction)
        {
            var state = SaveTreeViewState();
            updateAction();
            RestoreTreeViewState(state);
        }

        private DevicesTreeViewState SaveTreeViewState()
        {
            var expandedKeys = new HashSet<string>(StringComparer.Ordinal);
            foreach (var expandedObject in devicesTree.ExpandedObjects)
            {
                var key = DevicesTreeViewKeys.GetViewItemKey(expandedObject);
                if (key is not null)
                    expandedKeys.Add(key);
            }

            return new DevicesTreeViewState
            {
                TopItemIndex = devicesTree.TopItemIndex,
                ScrollPosition = devicesTree.LowLevelScrollPosition,
                ExpandedKeys = expandedKeys,
                SelectedKey = DevicesTreeViewKeys.GetViewItemKey(devicesTree.SelectedObject),
            };
        }

        private void RestoreTreeViewState(DevicesTreeViewState state)
        {
            if (state is null)
                return;

            devicesTree.BeginUpdate();
            try
            {
                RestoreExpandedByKeys(DataContext.Roots.Cast<IExpandable>(), state.ExpandedKeys);

                if (!string.IsNullOrEmpty(state.SelectedKey))
                {
                    var selected = FindViewItemByKey(state.SelectedKey);
                    if (selected is not null)
                        devicesTree.SelectedObject = selected;
                }
            }
            finally
            {
                devicesTree.EndUpdate();
            }

            if (state.TopItemIndex >= 0 && state.TopItemIndex < devicesTree.GetItemCount())
                devicesTree.TopItemIndex = state.TopItemIndex;
            else
                devicesTree.LowLevelScroll(state.ScrollPosition.X, state.ScrollPosition.Y);
        }

        private void RestoreExpandedByKeys(
            IEnumerable<IExpandable> items,
            HashSet<string> expandedKeys)
        {
            foreach (var item in items)
            {
                if (item is object obj)
                {
                    var key = DevicesTreeViewKeys.GetViewItemKey(obj);
                    if (key is not null && expandedKeys.Contains(key) &&
                        devicesTree.CanExpand(item))
                    {
                        devicesTree.Expand(item);
                        item.Expanded = true;
                    }
                }

                if (item.Items is not null)
                    RestoreExpandedByKeys(item.Items.OfType<IExpandable>(), expandedKeys);
            }
        }

        private object FindViewItemByKey(string key)
        {
            foreach (var root in DataContext.Roots.OfType<IExpandable>())
            {
                var found = FindViewItemByKey(root, key);
                if (found is not null)
                    return found;
            }

            return null;
        }

        private object FindViewItemByKey(IExpandable item, string key)
        {
            if (item is object obj &&
                string.Equals(DevicesTreeViewKeys.GetViewItemKey(obj), key, StringComparison.Ordinal))
            {
                return obj;
            }

            if (item.Items is null)
                return null;

            foreach (var child in item.Items.OfType<IExpandable>())
            {
                var found = FindViewItemByKey(child, key);
                if (found is not null)
                    return found;
            }

            return null;
        }

        private static IEnumerable<DevicesChannelItem> CollectChannelItems(
            IEnumerable<IExpandable> items)
        {
            foreach (var item in items)
            {
                if (item is DevicesChannelItem channelItem)
                    yield return channelItem;

                if (item.Items is not null)
                {
                    foreach (var child in CollectChannelItems(item.Items.OfType<IExpandable>()))
                        yield return child;
                }
            }
        }
    }
}
