using System.Collections.Generic;
using Editor;
using Moq;
using NUnit.Framework;

namespace Tests.Editor
{
    class TreeViewItemTest
    {
        public class InheritedTreeViewItem : TreeViewItem
        {
            public InheritedTreeViewItem() : base() { }

            public override string[] EditText => new string[] { "", Value };

            public override ITreeViewItem[] Items => Childs;

            public string Value { get; set; } = string.Empty;

            public ITreeViewItem[] Childs { get; set; } = null;
        }

        [TestCase(ImageIndexEnum.NONE)]
        public void ImageIndex_EmptyProperty_ReturnsDefaultValue(
            ImageIndexEnum expectedValue)
        {
            Assert.AreEqual(expectedValue, GetNewTreeViewItem().ImageIndex);
        }

        [Test]
        public void Parent_NewProperty_TheSameValue()
        {
            InheritedTreeViewItem parentProperty = GetNewTreeViewItem();
            InheritedTreeViewItem checkingProperty = GetNewTreeViewItem();

            checkingProperty.Parent = parentProperty;

            Assert.AreEqual(parentProperty.GetHashCode(),
                checkingProperty.Parent.GetHashCode());
        }

        [Test]
        public void DisplayText_NewObject_ReturnsEmtpyStringsInStringArr()
        {
            InheritedTreeViewItem InheritedTreeViewItem = GetNewTreeViewItem();

            var expected = new string[] { "", "" };
            Assert.AreEqual(expected, InheritedTreeViewItem.DisplayText);
        }

        [Test()]
        public void EditText_NewObject_ReturnsStringArrToEdit()
        {
            InheritedTreeViewItem InheritedTreeViewItem = GetNewTreeViewItem();

            var expected = new string[] { "", "" };
            Assert.AreEqual(expected, InheritedTreeViewItem.EditText);
        }
        
       
        [Test]
        public void IsDeleteable_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem().IsDeletable);
        }

        [Test]
        public void Delete_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem().Delete(null));
        }

        [Test]
        public void IsCopyable_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem().IsCopyable);
        }

        [Test]
        public void Copy_NewObject_ReturnsNull()
        {
            Assert.IsNull(GetNewTreeViewItem().Copy());
        }

        [Test]
        public void IsMoveable_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem().IsMoveable);
        }

        [Test]
        public void MoveDown_NewObject_ReturnsNull()
        {
            Assert.IsNull(GetNewTreeViewItem().MoveDown(null));
        }

        [Test]
        public void MoveUp_NewObject_ReturnsNull()
        {
            Assert.IsNull(GetNewTreeViewItem().MoveUp(null));
        }

        [Test]
        public void IsInsertable_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem().IsInsertable);
        }

        [Test]
        public void IsInsertableCopy_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem().IsInsertableCopy);
        }
        
        [Test]
        public void InsertCopy_NewObject_ReturnsNull()
        {
            Assert.IsNull(GetNewTreeViewItem().InsertCopy(null));
        }

        [Test]
        public void Replace_NewObject_ReturnsNull()
        {
            Assert.IsNull(GetNewTreeViewItem().Replace(null, null));
        }

        [Test]
        public void Items_NewObject_ReturnsNull()
        {
            Assert.IsNull(GetNewTreeViewItem().Items);
        }

        [TestCase(new int[] { -1, -1 })]
        public void EditablePart_NewObject_ReturnsExpectedValue(
            int[] expectedValue)
        {
            Assert.AreEqual(expectedValue, GetNewTreeViewItem().EditablePart);
        }

        [Test]
        public void IsEditable_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem().IsEditable);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void SetNewValueWithExtraValue_NewObject_ReturnsFalse(
            bool isExtraValue)
        {
            Assert.AreEqual(false,
                GetNewTreeViewItem().SetNewValue(null, isExtraValue));
        }

        [Test]
        public void SetNewValueWithDictionaty_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem()
                .SetNewValue(new Dictionary<int, List<int>>()));
        }

        public void SetNewvalue_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem()
                .SetNewValue(string.Empty));
        }

        [Test]
        public void Insert_NewObject_ReturnsNull()
        {
            Assert.IsNull(GetNewTreeViewItem().Insert());
        }

        [Test]
        public void IsUseDevList_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem().IsUseDevList);
        }

        [Test]
        public void IsLocalRestrictionUse_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem().IsLocalRestrictionUse);
        }

        [Test]
        public void IsDrawOnEplanPage_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem().IsDrawOnEplanPage);
        }

        [Test]
        public void GetObjectToDrawOnEplanPage_NewObject_ReturnsNull()
        {
            Assert.IsNull(GetNewTreeViewItem().GetObjectToDrawOnEplanPage());
        }

        [Test]
        public void NeedRebuildParent_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem().NeedRebuildParent);
        }

        [Test]
        public void IsBoolParameter_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem().IsBoolParameter);
        }

        [Test]
        public void IsMainObject_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem().IsMainObject);
        }

        [Test]
        public void IsMode_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem().IsMode);
        }

        [Test]
        public void IsReplaceable_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem().IsReplaceable);
        }

        [Test]
        public void ShowWarningBeforeDelete_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem().ShowWarningBeforeDelete);
        }

        [Test]
        public void IsFilled_NewObject_ReturnsTrue()
        {
            Assert.IsFalse(GetNewTreeViewItem().IsFilled);
        }

        [Test]
        public void ContainsBaseObject_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem().ContainsBaseObject);
        }

        [Test]
        public void BaseObjectsList_NewObject_ReturnsEmptyList()
        {
            Assert.AreEqual(new List<string>(),
                GetNewTreeViewItem().BaseObjectsList);
        }

        [Test]
        public void Cut_NewObject_ReturnsNull()
        {
            Assert.IsNull(GetNewTreeViewItem().Cut(null));
        }

        [Test]
        public void IsCuttable_NewObject_ReturnsFalse()
        {
            Assert.IsFalse(GetNewTreeViewItem().IsCuttable);
        }

        [Test]
        public void GetLinkToHelpPage_NewObject_ReturnsNull()
        {
            Assert.AreEqual(string.Empty, GetNewTreeViewItem().GetLinkToHelpPage());
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void Disabled_NewObject_ReturnsTheSameValue(bool actualValue,
            bool expectedValue)
        {
            InheritedTreeViewItem property = GetNewTreeViewItem();
            
            property.Disabled = actualValue;

            Assert.AreEqual(expectedValue, property.Disabled);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void MarkToCut_NewObject_ReturnsTheSameValue(bool actualValue,
            bool expectedValue)
        {
            InheritedTreeViewItem property = GetNewTreeViewItem();

            property.MarkToCut = actualValue;

            Assert.AreEqual(expectedValue, property.MarkToCut);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void NeedDisable_NewObject_ReturnsTheSameValue(
            bool actualValue, bool expectedValue)
        {
            InheritedTreeViewItem property = GetNewTreeViewItem();

            property.NeedDisable = actualValue;

            Assert.AreEqual(expectedValue, property.NeedDisable);
        }

        [Test]
        public void GetDisplayObjects_NewObject_ReturnsNullAndFalse()
        {
            InheritedTreeViewItem property = GetNewTreeViewItem();
            property.GetDisplayObjects(out EplanDevice.DeviceType[] devTypes,
                out EplanDevice.DeviceSubType[] devSubTypes,
                out bool displayParameters);

            Assert.IsNull(devTypes);
            Assert.IsNull(devSubTypes);
            Assert.IsFalse(displayParameters);
        }

        [Test]
        public void AddParent_NewObject_AddParentObjectToChild()
        {
            InheritedTreeViewItem child = GetNewTreeViewItem();
            InheritedTreeViewItem parent = GetNewTreeViewItem();
            child.AddParent(parent);

            Assert.AreEqual(child.Parent.GetHashCode(), parent.GetHashCode());
        }

        [Test]
        public void Filter_FoundInChild()
        {
            var child = new ObjectProperty("par", "qwerty");
            InheritedTreeViewItem parent = GetNewTreeViewItem();

            child.AddParent(parent);
            parent.Childs = new ITreeViewItem[] { child };

            var result = parent.Filter("qwer", false);

            Assert.IsTrue(result);
        }

        [Test]
        public void Filter_FoundInParent()
        {
            var child = new ObjectProperty("par", "");
            InheritedTreeViewItem parent = GetNewTreeViewItem();

            child.AddParent(parent);
            parent.Childs = new ITreeViewItem[] { child };

            parent.Value = "qwerty";

            var result = parent.Filter("qwer", false);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result);
                Assert.IsTrue(parent.ThisOrParentsContains);
            });
        }

        [Test]
        public void Filter_FoundInParent_HideEmpty()
        {
            var child = new ObjectProperty("par", "");
            InheritedTreeViewItem parent = GetNewTreeViewItem();

            child.AddParent(parent);
            parent.Childs = new ITreeViewItem[] { child };

            parent.Value = "qwerty";

            var result = parent.Filter("qwer", true);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result);
                Assert.IsTrue(parent.ThisOrParentsContains);
            });
        }

        [Test]
        public void Filter_HideEmpty()
        {
            var child = new ObjectProperty("par", "");
            InheritedTreeViewItem parent = GetNewTreeViewItem();

            child.AddParent(parent);
            parent.Childs = new ITreeViewItem[] { child };

            parent.Value = "";

            var result = parent.Filter("", true) || child.Filter("", true);

            Assert.Multiple(() =>
            {
                Assert.IsFalse(result);
                Assert.IsFalse(parent.ThisOrParentsContains);
            });
        }

        [Test]
        public void Filter_ReapeatFilterAndReset()
        {
            var child = new ObjectProperty("par", "qwerty");
            InheritedTreeViewItem parent = GetNewTreeViewItem();

            child.AddParent(parent);
            parent.Childs = new ITreeViewItem[] { child };

            parent.Value = "qwerty";

            Assert.Multiple(() =>
            {
                var result = parent.Filter("qwer", false);
                
                Assert.IsTrue(result);
                Assert.IsTrue(parent.Filtred);
                Assert.IsTrue(parent.ThisOrParentsContains);

                result = parent.Filter("qwer", false);

                Assert.IsTrue(result);
                Assert.IsTrue(parent.Filtred);
                Assert.IsTrue(parent.ThisOrParentsContains);

                result = child.Filter("qwer", false);
                Assert.IsTrue(result);

                parent.ResetFilter();

                Assert.IsNull(parent.Filtred);
                Assert.IsNull(child.Filtred);
                Assert.IsFalse(parent.ThisOrParentsContains);
            });
        }

        [Test]
        public void QuickMultiSelect()
        {
            var item1 = GetNewTreeViewItem();
            var item2 = GetNewTreeViewItem();
            var item3 = GetNewTreeViewItem();
            var parent = GetNewTreeViewItem();

            item1.Parent = parent;
            item2.Parent = parent;
            item3.Parent = parent;

            parent.Childs = new ITreeViewItem[] { item1, item2, item3 };

            var multiselect = item2.QuickMultiSelect();

            CollectionAssert.AreEqual(new List<ITreeViewItem>() { item1, item2, item3 }, multiselect);
        }

        [Test]
        public void CanMove()
        {
            var itemMock1 = new Mock<ITreeViewItem>();
            itemMock1.Setup(i => i.IsMoveable).Returns(true);

            var itemMock2 = new Mock<ITreeViewItem>();
            itemMock2.Setup(i => i.IsMoveable).Returns(true);

            var parent = GetNewTreeViewItem();
            parent.Childs = new ITreeViewItem[] { itemMock1.Object, itemMock2.Object };

            Assert.Multiple(() =>
            {
                Assert.IsTrue(parent.CanMoveDown(itemMock1.Object));
                Assert.IsFalse(parent.CanMoveDown(itemMock2.Object));
                Assert.IsTrue(parent.CanMoveUp(itemMock2.Object));
                Assert.IsFalse(parent.CanMoveUp(itemMock1.Object));
            });
        }

        public InheritedTreeViewItem GetNewTreeViewItem()
        {
            return new InheritedTreeViewItem();
        }
    }
}
