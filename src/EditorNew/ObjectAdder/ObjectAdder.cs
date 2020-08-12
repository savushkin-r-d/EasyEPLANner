using System;
using System.Data;
using System.Linq;
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
            LastSelectedType = null;
            LastSelectedSubType = null;
        }

        /// <summary>
        /// Конструктор, если известен выбранный тип объекта.
        /// </summary>
        /// <param name="typeName">тип объекта, имя</param>
        public ObjectsAdder(string typeName)
        {
            InitializeComponent();
            baseTechObjectsManager = NewTechObject.BaseTechObjectManager
                .GetInstance();
            LastSelectedType = typeName;
            LastSelectedSubType = null;

            SetUpObjectTypesListView();
        }

        /// <summary>
        /// Настройка дерева с типами объектов.
        /// </summary>
        private void SetUpObjectTypesListView()
        {
            int index = objectTypes.FindString(LastSelectedType);
            objectTypes.SetSelected(index, true);
            objectTypes.Enabled = false;
        }

        private void acceptButton_Click(object sender, EventArgs e)
        {
            bool incorrectInput =
                (objectSubTypes.Enabled && objectSubTypes.SelectedIndex == -1) ||
                (objectTypes.Enabled && objectTypes.SelectedIndex == -1);
            if (incorrectInput)
            {
                string message = "Ошибка. Выберите значения в списке(-ах)";
                MessageBox.Show(message, "Внимание", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            else
            {
                LastSelectedSubType = objectSubTypes.SelectedItem.ToString();
                LastSelectedType = objectTypes.SelectedItem.ToString();
                Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            LastSelectedType = null;
            LastSelectedSubType = null;
            Close();
        }

        private void ObjectTypes_SelectedValueChanged(object sender, 
            EventArgs e)
        {
            objectSubTypes.ClearSelected();
            objectSubTypes.Items.Clear();

            if(objectTypes.SelectedItem == null)
            {
                return;
            }
            else
            {
                string selectedItem = objectTypes.SelectedItem.ToString();
                switch (selectedItem)
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
        }

        /// <summary>
        /// Сброс выбранных типов и подтипов.
        /// </summary>
        public static void Reset()
        {
            LastSelectedSubType = null;
            LastSelectedType = null;
        }

        /// <summary>
        /// Установленное значение типа (Мастер, Аппарат, Агрегат)
        /// </summary>
        public static string LastSelectedType { get; set; }

        /// <summary>
        /// Установленное значение подтипа (Танк и др.)
        /// </summary>
        public static string LastSelectedSubType { get; set; }

        NewTechObject.BaseTechObjectManager baseTechObjectsManager;
    }
}
