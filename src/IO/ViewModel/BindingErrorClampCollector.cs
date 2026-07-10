using System.Collections.Generic;
using System.Linq;

namespace IO.ViewModel
{
    /// <summary>
    /// Собирает клеммы с ошибкой привязки из выбранных элементов дерева ПЛК.
    /// </summary>
    public static class BindingErrorClampCollector
    {
        public static IEnumerable<IClamp> Collect(IEnumerable<object> selectedItems)
        {
            var clamps = new HashSet<IClamp>();

            foreach (var item in selectedItems ?? Enumerable.Empty<object>())
            {
                foreach (var clamp in CollectFromItem(item))
                {
                    clamps.Add(clamp);
                }
            }

            return clamps;
        }

        private static IEnumerable<IClamp> CollectFromItem(object item)
        {
            if (item is IClamp clamp)
            {
                if (clamp.HasBindingError)
                {
                    yield return clamp;
                }

                yield break;
            }

            if (item is IExpandable expandable)
            {
                foreach (var child in expandable.Items)
                {
                    foreach (var childClamp in CollectFromItem(child))
                    {
                        yield return childClamp;
                    }
                }
            }
        }
    }
}
