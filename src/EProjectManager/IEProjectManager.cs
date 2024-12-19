namespace EasyEPlanner
{
    /// <summary>
    /// Менеджер проекта в CAD Eplan.
    /// </summary>
    public interface IEProjectManager
    {
        /// <summary>
        /// Имя проекта EPLAN
        /// </summary>
        string GetCurrentProjectName();

        /// <summary>
        /// Модифицированное имя проекта (пробелы заменены на минусы)
        /// </summary>
        string GetModifyingCurrentProjectName();
    }
}