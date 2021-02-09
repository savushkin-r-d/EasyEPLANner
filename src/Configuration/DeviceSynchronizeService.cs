using System.Collections.Generic;

namespace EasyEPlanner
{
    /// <summary>
    /// Интерфейс сервиса синхронизации устройств
    /// </summary>
    public interface IDeviceSynchronizeService
    {
        /// <summary>
        /// Синхронизировать устройства
        /// </summary>
        /// <param name="array">Массив изменений в индексах устройств</param>
        /// <param name="devicesIndexes">Массив индексов устройств в элементе
        /// </param>
        void SynchronizeDevices(int[] array, ref List<int> devicesIndexes);
    }

    /// <summary>
    /// Сервис синхронизации устройств
    /// </summary>
    public class DeviceSynchronizeService : IDeviceSynchronizeService
    {
        public void SynchronizeDevices(int[] array,
            ref List<int> devicesIndexes)
        {
            bool noDevices = devicesIndexes.Count <= 0;
            if (noDevices)
            {
                return;
            }

            var del = new List<int>();
            for (int j = 0; j < devicesIndexes.Count; j++)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (devicesIndexes[j] == i)
                    {
                        // Что бы не учитывало "-2" из array
                        if (array[i] == -1)
                        {
                            del.Add(j);
                            break;
                        }
                        if (array[i] >= 0)
                        {
                            devicesIndexes[j] = array[i];
                            break;
                        }
                    }
                }
            }

            int dx = 0;
            foreach (int index in del)
            {
                devicesIndexes.RemoveAt(index - dx++);
            }
        }
    }
}
