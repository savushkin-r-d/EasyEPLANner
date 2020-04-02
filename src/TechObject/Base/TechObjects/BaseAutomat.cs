﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Базовый автомат
    /// </summary>
    public class BaseAutomat : BaseTechObject
    {
        public BaseAutomat() : base()
        {
            S88Level = 2;
            Name = "Автомат";
            EplanName = "automat";
            BaseOperations = DataBase.Imitation.BaseEmptyOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "automat";
            Equipment = DataBase.Imitation.EmptyEquipment();
            AggregateProperties = DataBase.Imitation.EmptyAggregateProperties();
        }
    }
}
