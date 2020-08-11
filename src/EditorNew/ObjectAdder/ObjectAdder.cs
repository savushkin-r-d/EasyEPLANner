using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewEditor
{
    public partial class ObjectsAdder : Form
    {
        public ObjectsAdder()
        {
            InitializeComponent();
            baseTechObjectsManager = NewTechObject.BaseTechObjectManager
                .GetInstance();
            SelectedType = null;
            SelectedSubType = null;
        }

        private void acceptButton_Click(object sender, EventArgs e)
        {
            SelectedSubType = objectSubTypes.SelectedValue.ToString();
            SelectedType = objectTypes.SelectedValue.ToString();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            SelectedType = null;
            SelectedSubType = null;
            Close();
        }

        private void ObjectTypes_SelectedValueChanged(object sender, 
            EventArgs e)
        {
            objectSubTypes.ClearSelected();
            objectSubTypes.Items.Clear();
            switch (objectTypes.SelectedItem.ToString())
            {
                case "Мастер":
                    var master = baseTechObjectsManager.Master
                        .Name;
                    objectSubTypes.Items.Add(master);
                    break;

                case "Аппарат":
                    var units = baseTechObjectsManager.Units
                        .Select(x => x.Name).ToArray();
                    objectSubTypes.Items.AddRange(units);
                    break;

                case "Агрегат":
                    var aggregates = baseTechObjectsManager.Aggregates
                        .Select(x => x.Name).ToArray();
                    objectSubTypes.Items.AddRange(aggregates);
                    break;
            }
        }

        /// <summary>
        /// Установленное значение типа (Мастер, Аппарат, Агрегат)
        /// </summary>
        public static string SelectedType { get; set; }

        /// <summary>
        /// Установленное значение подтипа (Танк и др.)
        /// </summary>
        public static string SelectedSubType { get; set; }

        NewTechObject.BaseTechObjectManager baseTechObjectsManager;
    }
}
