using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject;

/// <summary>
/// Идентификатор тех. обхекта
/// </summary>
public interface ITechObjectIdentifyData
{
    /// <summary>
    /// Номер объекта
    /// </summary>
    int TechNumber { get; }

    /// <summary>
    /// Тип объекта
    /// </summary>
    int TechType { get; }

    /// <summary>
    /// ОУ
    /// </summary>
    string NameEplan { get; }

    /// <summary>
    /// Название в Monitor
    /// </summary>
    string NameBC { get; }

    /// <summary>
    /// Установить номер объекта для для идентификатора
    /// </summary>
    /// <param name="techNumber"></param>
    /// <returns></returns>
    ITechObjectIdentifyData WithTechNumber(int techNumber);
}


public class TechObjectIdentifyData : ITechObjectIdentifyData
{
    public TechObjectIdentifyData(int techNumber, int techType, string nameEplan, string nameBC)
    {
        TechNumber = techNumber;
        TechType = techType;
        NameEplan = nameEplan;
        NameBC = nameBC;
    }


    public int TechNumber { get; private set; }

    public int TechType { get; private set; }

    public string NameEplan { get; private set; }

    public string NameBC { get; private set; }

    public ITechObjectIdentifyData WithTechNumber(int techNumber)
    {
        TechNumber = techNumber;
        return this;
    }
}
