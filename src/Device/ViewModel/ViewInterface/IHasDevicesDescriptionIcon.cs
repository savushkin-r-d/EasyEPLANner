namespace EasyEPlanner.Devices.ViewModel.ViewInterface
{
    /// <summary>
    /// Иконка во втором столбце дерева (значение), по аналогии с <see cref="IO.ViewModel.IHasDescriptionIcon"/>.
    /// </summary>
    public interface IHasDevicesDescriptionIcon
    {
        DevicesIcon DescriptionIcon { get; }
    }
}
