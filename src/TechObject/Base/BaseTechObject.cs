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
        /// <summary>
        /// Базовая инициализация
        /// </summary>
        public BaseTechObject()
        {
            Name = "";
            EplanName = "";
            S88Level = 0;
            BaseOperations = new BaseOperation[0];
            BaseProperties = new BaseProperty[0];
            BasicName = "";
        }

        /// <summary>
        /// Имя базового объекта.
        /// </summary>
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

        /// <summary>
        /// ОУ базового объекта
        /// </summary>
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

        /// <summary>
        /// Уровень по S88 иерархии
        /// </summary>
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

        /// <summary>
        /// Базовые операции объекта
        /// </summary>
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

        /// <summary>
        /// Имя объекта для базовой функциональности
        /// </summary>
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

        /// <summary>
        /// Свойства базового объекта
        /// </summary>
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

        /// <summary>
        /// Получить базовую операцию по имени.
        /// </summary>
        /// <param name="name">Имя</param>
        /// <returns></returns>
        public BaseOperation GetBaseOperationByName(string name)
        {
            var operation = BaseOperations.Where(x => x.Name == name)
                .FirstOrDefault();
            return operation;
        }

        /// <summary>
        /// Список операций базового объекта
        /// </summary>
        /// <returns></returns>
        public List<string> BaseOperationsList
        {
            get
            {
                return BaseOperations.Select(x => x.Name).ToList();
            }
        }

        /// <summary>
        /// Копия объекта
        /// </summary>
        /// <returns></returns>
        public virtual BaseTechObject Clone()
        {
            return new BaseTechObject();
        }

        private string name;
        private string eplanName;
        private int s88Level;
        private string basicName;

        private BaseOperation[] objectOperations;
        private BaseProperty[] objectProperties;
    }

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
            BaseProperties = DataBase.Imitation.AutomatProperties();
            BasicName = "automat";
        }

        public override BaseTechObject Clone()
        {
            return new BaseAutomat();
        }
    }

    /// <summary>
    /// Базовый мастер
    /// </summary>
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

        public override BaseTechObject Clone()
        {
            return new BaseMaster();
        }
    }

	/// <summary>
    /// Базовый бачок
    /// </summary>
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

        public override BaseTechObject Clone()
        {
            return new BaseWaterTank();
        }
    }

    /// <summary>
    /// Базовый бойлер
    /// </summary>
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

        public override BaseTechObject Clone()
        {
            return new BaseBoiler();
        }
    }

    /// <summary>
    /// Базовая линия
    /// </summary>
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

        public override BaseTechObject Clone()
        {
            return new BaseLine();
        }
    }

    /// <summary>
    /// Базовая линия приемки
    /// </summary>
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

        public override BaseTechObject Clone()
        {
            return new BaseLineIn();
        }
    }

    /// <summary>
    /// Базовая линия выдачи
    /// </summary>
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

        public override BaseTechObject Clone()
        {
            return new BaseLineOut();
        }
    }

    /// <summary>
    /// Базовый пастеризатор
    /// </summary>
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

        public override BaseTechObject Clone()
        {
            return new BasePOU();
        }
    }

    /// <summary>
    /// Базовый пост
    /// </summary>
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

        public override BaseTechObject Clone()
        {
            return new BasePost();
        }
    }

    /// <summary>
    /// Базовый танк
    /// </summary>
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

        public override BaseTechObject Clone()
        {
            return new BaseTank();
        }
    }

    /// <summary>
    /// Базовый узел подогрева
    /// </summary>
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

        public override BaseTechObject Clone()
        {
            return new BaseHeater();
        }
    }

    /// <summary>
    /// Базовый узел охлаждения
    /// </summary>
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

        public override BaseTechObject Clone()
        {
            return new BaseCooler();
        }
    }

    /// <summary>
    /// Базовый узел перемешивания
    /// </summary>
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

        public override BaseTechObject Clone()
        {
            return new BaseMixer();
        }
    }
}
