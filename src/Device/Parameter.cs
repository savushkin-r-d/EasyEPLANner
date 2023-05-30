﻿using System.Collections.Generic;
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
        public class Parameter
        {
            /// <summary> Номинальная нагрузка в кг. </summary>
            public static readonly Parameter P_NOMINAL_W = new Parameter("P_NOMINAL_W", "Номинальная нагрузка", UnitFormat.Kilograms);

            /// <summary> Рабочий коэффициент передачи </summary>
            public static readonly Parameter P_RKP = new Parameter("P_RKP", "Рабочий коэффициент передачи", UnitFormat.RKP);

            /// <summary> Сдвиг нуля. </summary>
            public static readonly Parameter P_C0 = new Parameter("P_C0", "Сдивг нуля");

            /// <summary> Время порогового фильтра. </summary>
            public static readonly Parameter P_DT = new Parameter("P_DT", "Время порогового фильтра", UnitFormat.Milliseconds);

            /// <summary> Время включения. </summary>
            public static readonly Parameter P_ON_TIME = new Parameter("P_ON_TIME", "Время включения", UnitFormat.Milliseconds);

            public static readonly Parameter P_OFF_TIME = new Parameter(nameof(P_OFF_TIME), "Время выключения", UnitFormat.Milliseconds);

            /// <summary> Обратная связь, 1/0 (Да/Нет) </summary>
            public static readonly Parameter P_FB = new Parameter("P_FB", "Обратная связь", UnitFormat.Boolean);

            /// <summary> Аварийное значение. </summary>
            public static readonly Parameter P_ERR = new Parameter("P_ERR", "Аварийное значение");

            /// <summary> Минимальное значение. </summary>
            public static readonly Parameter P_MIN_V = new Parameter("P_MIN_V", "Мин. значение");

            /// <summary> Максимальное значение. </summary>
            public static readonly Parameter P_MAX_V = new Parameter("P_MAX_V", "Мак. значение");

            /// <summary> Давление, на которое настроен датчик. </summary>
            public static readonly Parameter P_MAX_P = new Parameter("P_MAX_P", "Давление датчика", UnitFormat.Bars);

            /// <summary> Радиус танка. </summary>
            public static readonly Parameter P_R = new Parameter("P_R", "Радиус танка", UnitFormat.Meters);

            /// <summary> Высота конической части танка. </summary>
            public static readonly Parameter P_H_CONE = new Parameter("P_H_CONE", "Высота конической части танка", UnitFormat.Meters);

            /// <summary> Высота усеченной части танка. </summary>
            public static readonly Parameter P_H_TRUNC = new Parameter("P_H_TRUNC", "Высота усеченной части танка", UnitFormat.Meters);

            /// <summary> Минимальное значение для потока. </summary>
            public static readonly Parameter P_MIN_F = new Parameter("P_MIN_F", "Мин. значение для потока");

            /// <summary> Максимальное значение для потока. </summary>
            public static readonly Parameter P_MAX_F = new Parameter("P_MAX_F", "Макс. значение для потока");

            /// <summary> Коэффициент усиления. </summary>
            public static readonly Parameter P_k = new Parameter("P_k", "Коэффициент усиления");

            /// <summary> Время интегрирования. </summary>
            public static readonly Parameter P_Ti = new Parameter("P_Ti", "Время интегрирования");

            /// <summary> Время дифференцирования. </summary>
            public static readonly Parameter P_Td = new Parameter("P_Td", "Время дифференцирования");

            /// <summary> Интервал расчёта. </summary>
            public static readonly Parameter P_dt = new Parameter("P_dt", "Интервал расчета", UnitFormat.Milliseconds);

            /// <summary> Максимальное значение входной величины. </summary>
            public static readonly Parameter P_max = new Parameter("P_max", "Макс. входное значение");

            /// <summary> Минимальное значение входной величины. </summary>
            public static readonly Parameter P_min = new Parameter("P_min", "Мин. входное значение");

            /// <summary> Время выхода на режим регулирования. </summary>
            public static readonly Parameter P_acceleration_time = new Parameter("P_acceleration_time", "Время выхода", UnitFormat.Seconds);

            /// <summary> Ручной режим, 0 - авто, 1 - ручной. </summary>
            public static readonly Parameter P_is_manual_mode = new Parameter("P_is_manual_mode", "Ручной режим", UnitFormat.Boolean);

            /// <summary> Заданное ручное значение выходного сигнала. </summary>
            public static readonly Parameter P_U_manual = new Parameter("P_U_manual", "Заданное ручное значение", UnitFormat.Percentages);

            /// <summary> Коэффициент усиления 2. </summary>
            public static readonly Parameter P_k2 = new Parameter("P_k2", "Коэффициент усиления 2");

            /// <summary> Время интегрирования 2. </summary>
            public static readonly Parameter P_Ti2 = new Parameter("P_Ti2", "Время интегрирования 2");

            /// <summary> Время дифференцирования 2 </summary>
            public static readonly Parameter P_Td2 = new Parameter("P_Td2", "Время дифференцирования 2");

            /// <summary> Максимальное значение выходной величины. </summary>
            public static readonly Parameter P_out_max = new Parameter("P_out_max", "Макс. выходное значение");

            /// <summary> Минимальное значение выходной величины. </summary>
            public static readonly Parameter P_out_min = new Parameter("P_out_min", "Мин. выходное значение");

            /// <summary> Обратного (реверсивного) действия, 0 - false, 1 - true. </summary>
            public static readonly Parameter P_is_reverse = new Parameter("P_is_reverse", "Выход обратного действия 100-0", UnitFormat.Boolean);

            /// <summary> Нулевое стартовое значение, 0 - false, 1 - true. </summary>
            public static readonly Parameter P_is_zero_start = new Parameter("P_is_zero_start", "Выход прямого действия 0-100", UnitFormat.Boolean);

            /// <summary> Диаметр вала, м. </summary>
            public static readonly Parameter P_SHAFT_DIAMETER = new Parameter("P_SHAFT_DIAMETER", "Диаметр вала", UnitFormat.Meters);

            /// <summary> Передаточное число </summary>
            public static readonly Parameter P_TRANSFER_RATIO = new Parameter("P_TRANSFER_RATIO", "Передаточное число");

            /// <summary> Предельное время отсутствия готовности к работе, секунд. </summary>
            public static readonly Parameter P_READY_TIME = new Parameter("P_READY_TIME", "Предельное время отсутсвя готовности к работе", UnitFormat.Seconds);

            /// <summary> Параметр для обработки ошибки счета импульсов. </summary>
            public static readonly Parameter P_ERR_MIN_FLOW = new Parameter("P_ERR_MIN_FLOW", "Ошибка счета импульсов");

            /// <summary> Дельта значение </summary>
            public static readonly Parameter P_DELTA = new Parameter(nameof(P_DELTA), "Дельта срабатывания");

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
            public Parameter(string name, string description = "", string format = UnitFormat.Empty)
            {
                this.name = name;
                this.description = description;
                this.format = format;
            }

            /// <summary>
            /// Неявное преобразование названия в параметр по его названию
            /// Если парметр не найден, возвращатся новый параметр с неверным названиемю
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
                if (parameter.description == string.Empty && parameter.format == string.Empty) return value.ToString();

                string displayedValue = value.ToString();
                double editedValue = 0;

                if (!double.TryParse(value.ToString(), out editedValue))
                    displayedValue = "-";

                // Формат параметров в зависимости от типа устройства
                switch (device.DeviceType)
                {
                    case DeviceType.WT:
                        if (parameter == P_C0 || parameter == P_DT)
                            return string.Format(UnitFormat.Kilograms, displayedValue);
                        break;
                    case DeviceType.C:
                        if (parameter == P_max || parameter == P_min || parameter == P_DELTA)
                        {
                            var inValue = device.Properties[Property.IN_VALUE];

                            if (inValue == null) break;
                            var signalDevice = EplanDevice.DeviceManager
                                .GetInstance().Devices
                                .Find(dev => dev.Name == inValue.ToString());
                            if (signalDevice == null) break;
                            
                            if (signalDevice.DeviceType == DeviceType.C)
                            {
                                return GetFormatValue(parameter, value,
                                        signalDevice);
                            }

                            return string.Format(signalDevice.PIDUnitFormat, displayedValue);
                        }
                        break;
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