namespace EplanDevice
{
    /// <summary>
    /// Список каналов подтипа
    /// </summary>
    /// <param name="subType">Подтип</param>
    /// <param name="DI">Количество каналов DI</param>
    /// <param name="DO">Количество каналов DO</param>
    /// <param name="AI">Количество каналов AI</param>
    /// <param name="AO">Количество каналов AO</param>
    public class SubtypeControlChannels(DeviceSubType subType, int DI, int DO, int AI, int AO) : ISubtypeControlChannels
    {
        public DeviceSubType SubType { get; private set; } = subType;

        public int DI { get; private set; } = DI;

        public int DO { get; private set; } = DO;

        public int AI { get; private set; } = AI;

        public int AO { get; private set; } = AO;

        public int[] ChannelsCount => [DI, DO, AI, AO];
    }
}
