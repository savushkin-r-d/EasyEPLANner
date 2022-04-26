using System.Collections.Generic;
using Editor;
using NUnit.Framework;

namespace Tests.Editor
{
    class ObjectPropertyTest
    {
        [TestCase(ImageIndexEnum.NONE)]
        public void ImageIndex_EmptyProperty_ReturnsDefaultValue(
            ImageIndexEnum expectedValue)
        {
            Assert.AreEqual(expectedValue, GetEmptyProperty().ImageIndex);
        }

        [Test]
        public void Clone_EmptyProperty_MemberwiseClone()
        {
            ObjectProperty emptyProperty = GetEmptyProperty();
            ObjectProperty clonedProperty = emptyProperty.Clone();

            Assert.AreEqual(emptyProperty.Name, clonedProperty.Name);
            Assert.AreEqual(emptyProperty.Value, clonedProperty.Value);
            Assert.AreEqual(
                emptyProperty.DefaultValue, clonedProperty.DefaultValue);
            Assert.AreEqual(emptyProperty.ImageName, "Свойство");
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase("")]
        [TestCase("1")]
        [TestCase("0")]
        [TestCase("-1")]
        [TestCase("ABCDE")]
        [TestCase("АБВГДЕ")]
        [TestCase(-5.0)]
        [TestCase(5.0)]
        [TestCase(0.0)]
        [TestCase(-5.3)]
        [TestCase(5.3)]
        [TestCase(0.3)]
        public void SetValue_EmptyProperty_ReturnsNewValueAsString(
            object expectedValue)
        {
            var property = new ObjectProperty(string.Empty, string.Empty);
            property.SetValue(expectedValue);

            Assert.AreEqual(expectedValue.ToString(), property.Value);
        }

        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase(1, "1")]
        [TestCase(0, "0")]
        [TestCase(-1, "-1")]
        [TestCase("ABC", "ABC")]
        [TestCase("АБВ", "АБВ")]
        [TestCase(5.0, "5")]
        [TestCase(-5.0, "-5")]
        [TestCase(0.0, "0")]
        [TestCase(5.3, "5,3")]
        [TestCase(-5.3, "-5,3")]
        [TestCase(0.3, "0,3")]
        public void DefaultValue_NewProperty_ReturnsDefaultValueOrEmptyAsString(
            object defaultValue,string expectedValue)
        {
            var property = new ObjectProperty(string.Empty, string.Empty,
                defaultValue);

            Assert.AreEqual(expectedValue, property.DefaultValue);
        }

        [Test]
        public void Parent_NewProperty_TheSameValue()
        {
            ObjectProperty parentProperty = GetEmptyProperty();
            var checkingProperty =
                new ObjectProperty(string.Empty, string.Empty);

            checkingProperty.Parent = parentProperty;

            Assert.AreEqual(parentProperty.GetHashCode(),
                checkingProperty.Parent.GetHashCode());
        }

        [TestCase("", "", "",
            new string[] { "", StaticHelper.CommonConst.StubForCells })]
        [TestCase("Name", "", null,
            new string[] { "Name", StaticHelper.CommonConst.StubForCells })]
        [TestCase("Name", "Value", "", new string[] { "Name", "Value" })]
        [TestCase("Name", 123, 0, new string[] { "Name", "123" })]
        public void DisplayText_NewObject_ReturnsDisplayTextInStringArr(
            string name, object value, object defaultValue, string[] expected)
        {
            var objectProperty = new ObjectProperty(name, value, defaultValue);

            Assert.AreEqual(expected, objectProperty.DisplayText);
        }

        [TestCase(0, new string[] { "", "0" })]
        [TestCase(1, new string[] { "", "1" })]
        [TestCase(-1, new string[] { "", "-1" })]
        [TestCase("ABCD", new string[] { "", "ABCD" })]
        [TestCase("АБВГ", new string[] { "", "АБВГ" })]
        [TestCase(5.3, new string[] { "", "5.3" })]
        [TestCase(-5.3, new string[] { "", "-5.3" })]
        [TestCase(0.3, new string[] { "", "0.3" })]
        [TestCase(5.0, new string[] { "", "5" })]
        [TestCase(-5.0, new string[] { "", "-5" })]
        [TestCase(0.0, new string[] { "", "0" })]
        public void EditText_NewObject_ReturnsStringArrToEdit(object value,
            string[] expected)
        {
            var objectProperty = new ObjectProperty("Name", value);

            Assert.AreEqual(expected, objectProperty.EditText);
        }
        
        [TestCase(0, 20, 0)]
        [TestCase("", "", "")]
        [TestCase("", 20, 20)]
        [TestCase(0.1, 20.1, 0.1)]
        public void Delete_EmptyObject_SetDefaultValue(object defaultValue,
            object value, object expectedValue)
        {
            var property = new ObjectProperty("Name", value, defaultValue);

            property.Delete(null); // Argument never used

            Assert.AreEqual(expectedValue.ToString(), property.Value);
        } 
        
        [Test]
        public void IsDeleteable_EmptyObject_ReturnsTrue()
        {
            Assert.IsTrue(GetEmptyProperty().IsDeletable);
        }

        [Test]
        public void IsCopyable_EmptyObject_ReturnsFalse()
        {
            Assert.IsFalse(GetEmptyProperty().IsCopyable);
        }

        [Test]
        public void Copy_EmptyObject_ReturnsNull()
        {
            Assert.IsNull(GetEmptyProperty().Copy());
        }

        [Test]
        public void IsMoveable_EmptyObject_ReturnsFalse()
        {
            Assert.IsFalse(GetEmptyProperty().IsMoveable);
        }

        [Test]
        public void MoveDown_EmptyObject_ReturnsNull()
        {
            Assert.IsNull(GetEmptyProperty().MoveDown(null));
        }

        [Test]
        public void MoveUp_EmptyObject_ReturnsNull()
        {
            Assert.IsNull(GetEmptyProperty().MoveUp(null));
        }

        [Test]
        public void IsInsertable_EmptyObject_ReturnsFalse()
        {
            Assert.IsFalse(GetEmptyProperty().IsInsertable);
        }

        [Test]
        public void IsInsertableCopy_EmptyObject_ReturnsFalse()
        {
            Assert.IsFalse(GetEmptyProperty().IsInsertableCopy);
        }
        
        [Test]
        public void InsertCopy_EmptyObject_ReturnsNull()
        {
            Assert.IsNull(GetEmptyProperty().InsertCopy(null));
        }

        [Test]
        public void Replace_EmptyObject_ReturnsNull()
        {
            Assert.IsNull(GetEmptyProperty().Replace(null, null));
        }

        [Test]
        public void Items_EmptyObject_ReturnsNull()
        {
            Assert.IsNull(GetEmptyProperty().Items);
        }

        [TestCase(new int[] { -1, 1 })]
        public void EditablePart_EmptyObject_ReturnsExpectedValue(
            int[] expectedValue)
        {
            Assert.AreEqual(expectedValue, GetEmptyProperty().EditablePart);
        }

        [Test]
        public void IsEditable_EmptyObject_ReturnsTrue()
        {
            Assert.IsTrue(GetEmptyProperty().IsEditable);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void SetNewValueWithExtraValue_EmptyObject_ReturnsFalse(
            bool isExtraValue)
        {
            Assert.AreEqual(false,
                GetEmptyProperty().SetNewValue(null, isExtraValue));
        }

        [Test]
        public void SetNewValueWithDictionaty_EmptyObject_ReturnsFalse()
        {
            Assert.IsFalse(GetEmptyProperty()
                .SetNewValue(new Dictionary<int, List<int>>()));
        }

        [Test]
        public void Insert_EmptyObject_ReturnsNull()
        {
            Assert.IsNull(GetEmptyProperty().Insert());
        }

        [Test]
        public void IsUseDevList_EmptyObject_ReturnsFalse()
        {
            Assert.IsFalse(GetEmptyProperty().IsUseDevList);
        }

        [Test]
        public void IsUseRestriction_EmptyObject_ReturnsFalse()
        {
            Assert.IsFalse(GetEmptyProperty().IsUseRestriction);
        }

        [Test]
        public void IsLocalRestrictionUse_EmptyObject_ReturnsFalse()
        {
            Assert.IsFalse(GetEmptyProperty().IsLocalRestrictionUse);
        }

        [Test]
        public void IsDrawOnEplanPage_EmptyObject_ReturnsFalse()
        {
            Assert.IsFalse(GetEmptyProperty().IsDrawOnEplanPage);
        }

        [Test]
        public void GetObjectToDrawOnEplanPage_EmptyObject_ReturnsNull()
        {
            Assert.IsNull(GetEmptyProperty().GetObjectToDrawOnEplanPage());
        }

        [Test]
        public void NeedRebuildParent_EmptyObject_ReturnsFalse()
        {
            Assert.IsFalse(GetEmptyProperty().NeedRebuildParent);
        }

        [Test]
        public void IsBoolParameter_EmptyObject_ReturnsFalse()
        {
            Assert.IsFalse(GetEmptyProperty().IsBoolParameter);
        }

        [Test]
        public void IsMainObject_EmptyObject_ReturnsFalse()
        {
            Assert.IsFalse(GetEmptyProperty().IsMainObject);
        }

        [Test]
        public void IsMode_EmptyObject_ReturnsFalse()
        {
            Assert.IsFalse(GetEmptyProperty().IsMode);
        }

        [Test]
        public void ShowWarningBeforeDelete_EmptyObject_ReturnsFalse()
        {
            Assert.IsFalse(GetEmptyProperty().ShowWarningBeforeDelete);
        }

        [Test]
        public void IsFilled_EmptyObject_ReturnsTrue()
        {
            Assert.IsTrue(GetEmptyProperty().IsFilled);
        }

        [Test]
        public void ContainsBaseObject_EmptyObject_ReturnsFalse()
        {
            Assert.IsFalse(GetEmptyProperty().ContainsBaseObject);
        }

        [Test]
        public void BaseObjectsList_EmptyObject_ReturnsEmptyList()
        {
            Assert.AreEqual(new List<string>(),
                GetEmptyProperty().BaseObjectsList);
        }

        [Test]
        public void Cut_EmptyObject_ReturnsNull()
        {
            Assert.IsNull(GetEmptyProperty().Cut(null));
        }

        [Test]
        public void IsCuttable_EmptyObject_ReturnsFalse()
        {
            Assert.IsFalse(GetEmptyProperty().IsCuttable);
        }

        [Test]
        public void GetLinkToHelpPage_EmptyObject_ReturnsNull()
        {
            Assert.IsNull(GetEmptyProperty().GetLinkToHelpPage());
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void Disabled_EmptyObject_ReturnsTheSameValue(bool actualValue,
            bool expectedValue)
        {
            ObjectProperty property = GetEmptyProperty();
            
            property.Disabled = actualValue;

            Assert.AreEqual(expectedValue, property.Disabled);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void MarkToCut_EmptyObject_ReturnsTheSameValue(bool actualValue,
            bool expectedValue)
        {
            ObjectProperty property = GetEmptyProperty();

            property.MarkToCut = actualValue;

            Assert.AreEqual(expectedValue, property.MarkToCut);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void NeedDisable_EmptyObject_ReturnsTheSameValue(
            bool actualValue, bool expectedValue)
        {
            ObjectProperty property = GetEmptyProperty();

            property.NeedDisable = actualValue;

            Assert.AreEqual(expectedValue, property.NeedDisable);
        }

        public void GetDisplayObjects_EmptyObject_ReturnsNullAndFalse()
        {
            ObjectProperty property = GetEmptyProperty();
            property.GetDisplayObjects(out EplanDevice.DeviceType[] devTypes,
                out EplanDevice.DeviceSubType[] devSubTypes,
                out bool displayParameters);

            Assert.IsNull(devTypes);
            Assert.IsNull(devSubTypes);
            Assert.IsFalse(displayParameters);
        }

        [Test]
        public void AddParent_EmptyObject_AddParentObjectToChild()
        {
            ObjectProperty child = GetEmptyProperty();
            var parent = new ObjectProperty("Parent", "");
            child.AddParent(parent);

            Assert.AreEqual(child.Parent.GetHashCode(), parent.GetHashCode());
        }

        private ObjectProperty GetEmptyProperty()
        {
            return new ObjectProperty(string.Empty, string.Empty);
        }
    }
}
