using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechObject
{
    /// <summary>
    /// Класс реализующий базовый аппарат для технологического объекта
    /// </summary>
    public class BaseTechObject
    {
        public BaseTechObject()
        {
            Name = "";
            EplanName = "";
            S88Level = 0;
            BaseOperations = new BaseOperation[0];
            BaseProperties = new BaseProperty[0];
            BasicName = "";
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string EplanName
        {
            get
            {
                return eplanName;
            }

            set
            {
                eplanName = value;
            }
        }

        public int S88Level
        {
            get
            {
                return s88Level;
            }

            set
            {
                s88Level = value;
            }
        }

        public BaseOperation[] BaseOperations
        {
            get 
            { 
                return objectOperations; 
            }

            set
            {
                objectOperations = value;
            }
        }

        public string BasicName
        {
            get 
            {
                return basicName;
            }

            set
            {
                basicName = value;
            }
        }

        public BaseProperty[] BaseProperties
        {
            get
            {
                return objectProperties;
            }

            set
            {
                objectProperties = value;
            }
        }

        private string name; // Отображаемое имя технологического объекта.
        private string eplanName; // ОУ объекта в Eplan.
        private int s88Level; // Уровень объекта по S88.
        private string basicName; // Базовое имя объекта (для функциональности)

        private BaseOperation[] objectOperations;  // Базовые операции объекта.
        private BaseProperty[] objectProperties; // Свойства объекта
    }

    public class BaseAutomat : BaseTechObject
    {
        public BaseAutomat() : base()
        {
            S88Level = 2;
            Name = "Автомат";
            EplanName = "automat";
            BaseOperations = DataBase.Imitation.BaseEmptyOperations();
            BaseProperties = DataBase.Imitation.AutomatProperties();
            BasicName = "automat";
        }
    }

    public class BaseMaster : BaseTechObject
    {
        public BaseMaster() : base()
        {
            S88Level = 1;
            Name = "Мастер";
            EplanName = "master";
            BaseOperations = DataBase.Imitation.BaseEmptyOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "master";
        }
    }

	//Бачок
    public class BaseWaterTank : BaseTechObject
    {
        public BaseWaterTank() : base()
        {
            S88Level = 2;
            Name = "Бачок";
            EplanName = "_tank";
            BaseOperations = DataBase.Imitation.BaseEmptyOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "cooler";
        }
    }

    public class BaseBoiler : BaseTechObject
    {
        public BaseBoiler() : base()
        {
            S88Level = 2;
            Name = "Бойлер";
            EplanName = "boil";
            BaseOperations = DataBase.Imitation.BaseEmptyOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "boiler";
        }
    }

    public class BaseLine : BaseTechObject
    {
        public BaseLine() : base()
        {
            S88Level = 2;
            Name = "Линия";
            EplanName = "line";
            BaseOperations = DataBase.Imitation.BaseLineOperations();
            BaseProperties = DataBase.Imitation.LineProperties();
            BasicName = "line";
        }
    }

    public class BaseLineIn : BaseTechObject
    {
        public BaseLineIn() : base()
        {
            S88Level = 2;
            Name = "Линия приемки";
            EplanName = "line";
            BaseOperations = DataBase.Imitation.BaseLineOperations();
            BaseProperties = DataBase.Imitation.LineProperties();
            BasicName = "line";
        }
    }

    public class BaseLineOut : BaseTechObject
    {
        public BaseLineOut() : base()
        {
            S88Level = 2;
            Name = "Линия выдачи";
            EplanName = "line";
            BaseOperations = DataBase.Imitation.BaseLineOperations();
            BaseProperties = DataBase.Imitation.LineProperties();
            BasicName = "line";
        }
    }

    public class BasePOU : BaseTechObject
    {
        public BasePOU() : base()
        {
            S88Level = 2;
            Name = "Пастеризатор";
            EplanName = "pasteurizator";
            BaseOperations = DataBase.Imitation.BaseEmptyOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "pasteurizator";
        }
    }

    public class BasePost : BaseTechObject
    {
        public BasePost() : base()
        {
            S88Level = 2;
            Name = "Пост";
            EplanName = "post";
            BaseOperations = DataBase.Imitation.BaseEmptyOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "post";
        }
    }

    public class BaseTank : BaseTechObject
    {
        public BaseTank() : base()
        {
            S88Level = 1;
            Name = "Танк";
            EplanName = "tank";
            BaseOperations = DataBase.Imitation.BaseTankOperations();
            BaseProperties = DataBase.Imitation.TankProperties();
            BasicName = "tank";
        }
    }

    public class BaseHeater : BaseTechObject
    {
        public BaseHeater() : base()
        {
            S88Level = 2;
            Name = "Узел подогрева";
            EplanName = "heater_node";
            BaseOperations = DataBase.Imitation.BaseEmptyOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "heater_node";
        }
    }

    public class BaseCooler : BaseTechObject
    {
        public BaseCooler() : base()
        {
            S88Level = 2;
            Name = "Узел охлаждения";
            EplanName = "cooler_node";
            BaseOperations = DataBase.Imitation.BaseEmptyOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "cooler_node";
        }
    }

    public class BaseMixer : BaseTechObject
    {
        public BaseMixer() : base()
        {
            S88Level = 2;
            Name = "Узел перемешивания";
            EplanName = "mix_node";
            BaseOperations = DataBase.Imitation.BaseEmptyOperations();
            BaseProperties = DataBase.Imitation.EmptyProperties();
            BasicName = "mix_node";
        }
    }
}
