using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

namespace EplanDevice
{
    public partial class IODevice
    {
        public static class UnitFormat
        {
            public const string Empty = "{0}";
            public const string Boolean = "{0:Да;-;Нет}";
            public const string Seconds = "{0} c";
            public const string Milliseconds = "{0} мс";
            public const string Meters = "{0} м";
            public const string Kilograms = "{0} кг";
            public const string Bars = "{0} бар";
            public const string RKP = "{0} мВ/В";
            public const string Percentages = "{0} %";
            public const string DegreesCelsius = "{0} °C";
            public const string CubicMeterPerHour = "{0} м3/ч";
        }


        /// <summary>
        /// Описание параметров устройства.
        /// </summary>
        public class Parameter : ITag
        {
            /// <summary> Номинальная нагрузка в кг. </summary>
            public static readonly Parameter P_NOMINAL_W = 
                new(nameof(P_NOMINAL_W), "Номинальная нагрузка",
                    UnitFormat.Kilograms);

            /// <summary> Рабочий коэффициент передачи </summary>
            public static readonly Parameter P_RKP = 
                new(nameof(P_RKP), "Рабочий коэффициент передачи",
                    UnitFormat.RKP);

            /// <summary> Сдвиг нуля. </summary>
            public static readonly Parameter P_C0 = 
                new(nameof(P_C0), "Сдвиг нуля");

            /// <summary> Время порогового фильтра. </summary>
            public static readonly Parameter P_DT = 
                new(nameof(P_DT), "Время порогового фильтра",
                    UnitFormat.Milliseconds);

            /// <summary> Время включения. </summary>
            public static readonly Parameter P_ON_TIME = 
                new(nameof(P_ON_TIME), "Время включения", 
                    UnitFormat.Milliseconds);

            public static readonly Parameter P_OFF_TIME = 
                new(nameof(P_OFF_TIME), "Время выключения",
                    UnitFormat.Milliseconds);

            /// <summary> Обратная связь, 1/0 (Да/Нет) </summary>
            public static readonly Parameter P_FB = 
                new(nameof(P_FB), "Обратная связь", 
                    UnitFormat.Boolean);

            /// <summary> Аварийное значение. </summary>
            public static readonly Parameter P_ERR = 
                new(nameof(P_ERR), "Аварийное значение");

            /// <summary> Минимальное значение. </summary>
            public static readonly Parameter P_MIN_V = 
                new(nameof(P_MIN_V), "Мин. значение");

            /// <summary> Максимальное значение. </summary>
            public static readonly Parameter P_MAX_V = 
                new(nameof(P_MAX_V), "Мак. значение");

            /// <summary> Давление, на которое настроен датчик. </summary>
            public static readonly Parameter P_MAX_P = 
                new(nameof(P_MAX_P), "Давление датчика",
                    UnitFormat.Bars);

            /// <summary> Радиус танка. </summary>
            public static readonly Parameter P_R = 
                new(nameof(P_R), "Радиус танка",
                    UnitFormat.Meters);

            /// <summary> Высота конической части танка. </summary>
            public static readonly Parameter P_H_CONE = 
                new(nameof(P_H_CONE), "Высота конической части танка",
                    UnitFormat.Meters);

            /// <summary> Высота усеченной части танка. </summary>
            public static readonly Parameter P_H_TRUNC = 
                new(nameof(P_H_TRUNC), "Высота усеченной части танка",
                    UnitFormat.Meters);

            /// <summary> Минимальное значение для потока. </summary>
            public static readonly Parameter P_MIN_F = 
                new(nameof(P_MIN_F), "Мин. значение для потока");

            /// <summary> Максимальное значение для потока. </summary>
            public static readonly Parameter P_MAX_F = 
                new(nameof(P_MAX_F), "Макс. значение для потока");

            /// <summary> Коэффициент усиления. </summary>
            public static readonly Parameter P_k = 
                new(nameof(P_k), "Коэффициент усиления");

            /// <summary> Время интегрирования. </summary>
            public static readonly Parameter P_Ti = 
                new(nameof(P_Ti), "Время интегрирования");

            /// <summary> Время дифференцирования. </summary>
            public static readonly Parameter P_Td = 
                new(nameof(P_Td), "Время дифференцирования");

            /// <summary> Интервал расчёта. </summary>
            public static readonly Parameter P_dt = 
                new(nameof(P_dt), "Интервал расчета", 
                    UnitFormat.Milliseconds);

            /// <summary> Максимальное значение входной величины. </summary>
            public static readonly Parameter P_max = 
                new(nameof(P_max), "Макс. входное значение");

            /// <summary> Минимальное значение входной величины. </summary>
            public static readonly Parameter P_min = 
                new(nameof(P_min), "Мин. входное значение");

            /// <summary> Время выхода на режим регулирования. </summary>
            public static readonly Parameter P_acceleration_time = 
                new(nameof(P_acceleration_time), "Время выхода",
                    UnitFormat.Seconds);

            /// <summary> Ручной режим, 0 - авто, 1 - ручной. </summary>
            public static readonly Parameter P_is_manual_mode = 
                new(nameof(P_is_manual_mode), "Ручной режим",
                    UnitFormat.Boolean);

            /// <summary> Заданное ручное значение выходного сигнала. </summary>
            public static readonly Parameter P_U_manual = 
                new(nameof(P_U_manual), "Заданное ручное значение",
                    UnitFormat.Percentages);

            /// <summary> Коэффициент усиления 2. </summary>
            public static readonly Parameter P_k2 = 
                new(nameof(P_k2), "Коэффициент усиления 2");

            /// <summary> Время интегрирования 2. </summary>
            public static readonly Parameter P_Ti2 = 
                new(nameof(P_Ti2), "Время интегрирования 2");

            /// <summary> Время дифференцирования 2 </summary>
            public static readonly Parameter P_Td2 = 
                new(nameof(P_Td2), "Время дифференцирования 2");

            /// <summary> Максимальное значение выходной величины. </summary>
            public static readonly Parameter P_out_max = 
                new(nameof(P_out_max), "Макс. выходное значение");

            /// <summary> Минимальное значение выходной величины. </summary>
            public static readonly Parameter P_out_min = 
                new(nameof(P_out_min), "Мин. выходное значение");

            /// <summary> Обратного (реверсивного) действия, 0 - false, 1 - true. </summary>
            public static readonly Parameter P_is_reverse = 
                new(nameof(P_is_reverse), "Выход обратного действия 100-0", 
                    UnitFormat.Boolean);

            /// <summary> Нулевое стартовое значение, 0 - false, 1 - true. </summary>
            public static readonly Parameter P_is_zero_start = 
                new(nameof(P_is_zero_start), "Нулевое стартовое значение",
                    UnitFormat.Boolean);

            /// <summary> Диаметр вала, м. </summary>
            public static readonly Parameter P_SHAFT_DIAMETER = 
                new(nameof(P_SHAFT_DIAMETER), "Диаметр вала",
                    UnitFormat.Meters);

            /// <summary> Передаточное число </summary>
            public static readonly Parameter P_TRANSFER_RATIO = 
                new(nameof(P_TRANSFER_RATIO), "Передаточное число");

            /// <summary> Предельное время отсутствия готовности к работе, секунд. </summary>
            public static readonly Parameter P_READY_TIME = 
                new(nameof(P_READY_TIME), "Предельное время отсутствия готовности к работе",
                    UnitFormat.Seconds);

            /// <summary> Параметр для обработки ошибки счета импульсов. </summary>
            public static readonly Parameter P_ERR_MIN_FLOW = 
                new(nameof(P_ERR_MIN_FLOW), "Ошибка счета импульсов");

            /// <summary> Дельта значение </summary>
            public static readonly Parameter P_delta = 
                new(nameof(P_delta), "Дельта срабатывания");

            public static readonly Parameter P_T_GEN =
                new(nameof(P_T_GEN), "Интервал генерации выходных импульсов", 
                    UnitFormat.Milliseconds);

            public static readonly Parameter P_T_ERR =
                new(nameof(P_T_ERR), "Интервал ожидания изменения входа",
                    UnitFormat.Milliseconds);

            /// <summary> Коэффициент пропорциональности </summary>
            public static readonly Parameter P_K =
                new(nameof(P_K), "Коэффициент пропорциональности",
                    UnitFormat.Milliseconds);

            protected static readonly Lazy<Dictionary<string, Parameter>> AllParameters = InitParameters();

            private static Lazy<Dictionary<string, Parameter>> InitParameters()
            {
                return new Lazy<Dictionary<string, Parameter>>(() =>
                {
                    var parameters = typeof(Parameter)
                        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                        .Where(x => x.FieldType == typeof(Parameter))
                        .Select(x => x.GetValue(null))
                        .Cast<Parameter>()
                        .ToDictionary(x => x.name, x => x);
                    return parameters;
                });
            }

            /// <summary>
            /// Конструктор параметра
            /// </summary>
            /// <param name="name">Название параметра (CAD-name)</param>
            /// <param name="description">Описание параметра</param>
            /// <param name="">Формат string.() (единицы измерения) </param>
            private Parameter(string name, string description = "", string format = UnitFormat.Empty)
            {
                this.name = name;
                this.description = description;
                this.format = format;
            }

            /// <summary>
            /// Неявное преобразование названия в параметр по его названию
            /// Если параметр не найден, возвращается новый параметр с неверным названием
            /// </summary>
            /// <param name="parameterName">Название параметра</param>
            public static implicit operator Parameter(string parameterName)
            {
                if (AllParameters.Value.TryGetValue(parameterName, out var parameter))
                {
                    return parameter;
                }
                else
                {
                    return new Parameter(parameterName, string.Empty, string.Empty);
                }
            }

            /// <summary>
            /// Неявное преобразование параметра в строку с названием
            /// </summary>
            /// <param name="parameterType">Параметр</param>
            public static implicit operator string(Parameter parameterType)
            {
                return parameterType.name;
            }

            public static string GetFormatValue(Parameter parameter, object value, IODevice device = null)
            {
                if (parameter.description == string.Empty && parameter.format == string.Empty) 
                    return value.ToString();

                string displayedValue = value.ToString();

                if (!double.TryParse(value.ToString(), out double editedValue))
                    displayedValue = "-";

                // Формат параметров в зависимости от типа устройства
                if (device.DeviceType is DeviceType.WT && new[] { P_C0, P_DT }.Contains(parameter))
                    return string.Format(UnitFormat.Kilograms, displayedValue);

                if (device.DeviceType is DeviceType.C &&
                    new [] { P_max, P_min, P_delta }.Contains(parameter))
                {
                    var signalDevice = DeviceManager.GetInstance().Devices
                        .Find(dev => dev.Name == device.Properties[Property.IN_VALUE]?.ToString());

                    if (signalDevice != null)
                    {
                        return signalDevice.DeviceType is DeviceType.C ?
                            GetFormatValue(parameter, value, signalDevice)
                            : string.Format(signalDevice.PIDUnitFormat, displayedValue);
                    }
                }

                if (parameter.format == UnitFormat.Boolean)
                    return string.Format(parameter.format, editedValue);

                return string.Format(parameter.format, displayedValue);
            }

            public override string ToString() => name;

            public string Name { get => name; }
            public string Description { get => description; }
            public string Format { get => format; }

            private readonly string name;
            private readonly string description;
            private readonly string format;
        }
    }


}