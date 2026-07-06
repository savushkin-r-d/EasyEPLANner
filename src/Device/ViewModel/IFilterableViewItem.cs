namespace EasyEPlanner.Devices.ViewModel
{
    public interface IFilterableViewItem
    {
        bool Filter(string searchString, bool hideEmptyItems);

        void ResetFilter();

        bool Contains(string value);
    }
}
