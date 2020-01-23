namespace EasyEPlanner
{
    /// <summary>
    /// Класс, проверяющий текущую конфигурацию проекта.
    /// </summary>
    public class ConfigurationChecker
    {
        public ConfigurationChecker()
        {
            this.deviceManager = Device.DeviceManager.GetInstance();
            this.IOManager = IO.IOManager.GetInstance();
            this.techObjectManager = TechObject.TechObjectManager.GetInstance();
        }

        public void Check() 
        {
            errors = "";
            errors = deviceManager.Check();
            errors += IOManager.Check();
            errors += techObjectManager.Check();
        }

        public string Errors 
        { 
            get
            {
                return errors;
            } 
        }

        string errors;

        Device.DeviceManager deviceManager;
        IO.IOManager IOManager;
        TechObject.TechObjectManager techObjectManager;
    }
}
