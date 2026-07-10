using Editor;
using NUnit.Framework;
using System.Collections.Generic;
using TechObject;

namespace Tests.TechObject
{
    class GroupableActionTest
    {
        class GroupableActionTestImplementation : GroupableAction
        {
            public GroupableActionTestImplementation(string name,
                Step owner, string luaName) : base(name, owner, luaName) { }
        }

        [Test]
        public void HasSubActions_NewAction_ReturnsTrue()
        {
            var action = new GroupableActionTestImplementation(string.Empty,
                null, string.Empty);

            bool hasSubActions = action.HasSubActions;

            Assert.IsTrue(hasSubActions);
        }

        [Test]
        public void Constructor_NewAction_ReturnsZeroSubActionsCount()
        {
            var action = new GroupableActionTestImplementation(string.Empty,
                null, string.Empty);

            int count = action.SubActions.Count;

            Assert.Zero(count);
        }

        [Test]
        public void IsDeleteable_NewAction_ReturnsTrue()
        {
            var action = new GroupableActionTestImplementation(string.Empty,
                null, string.Empty);

            bool isDeleteable = action.IsDeletable;

            Assert.IsTrue(isDeleteable);
        }

        [Test]
        public void IsUseDevList_NewAction_ReturnsFalse()
        {
            var action = new GroupableActionTestImplementation(string.Empty,
                null, string.Empty);

            bool isUseDevList = action.IsUseDevList;

            Assert.IsFalse(isUseDevList);
        }

        [Test]
        public void IsEditable_NewAction_ReturnsFalse()
        {
            var action = new GroupableActionTestImplementation(string.Empty,
                null, string.Empty);

            bool isEditable = action.IsEditable;

            Assert.IsFalse(isEditable);
        }

        [Test]
        public void Items_NewAction_ReturnsArrayOfSubActions()
        {
            var action = new GroupableActionTestImplementation(string.Empty,
                null, string.Empty);
            string testSubActionName = "TestSubActionName";
            string testSubActionLuaName = "LuaSubAction";
            var testSubAction = new Action(testSubActionName, null,
                testSubActionLuaName);
            action.SubActions.Add(testSubAction);

            ITreeViewItem[] items = action.Items;

            var subAction = (IAction)items[0];
            Assert.Multiple(() =>
            {
                Assert.AreEqual(action.SubActions.Count, items.Length);
                Assert.AreEqual(testSubActionName, subAction.Name);
                Assert.AreEqual(testSubActionLuaName, subAction.LuaName);
            });
        }

        [Test]
        public void DrawStyle_NewAction_ReturnsCorrectDrawStyleForSubAction()
        {
            var action = new GroupableActionTestImplementation(string.Empty,
                null, string.Empty);
            var subAction = new Action(string.Empty, null, string.Empty);
            action.SubActions.Add(subAction);

            DrawInfo.Style defaultSubActionDrawStyle = subAction.DrawStyle;
            DrawInfo.Style defaultActionDrawStyle = action.DrawStyle;

            DrawInfo.Style expectedDrawStyle = DrawInfo.Style.GRAY_BOX;
            action.DrawStyle = expectedDrawStyle;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedDrawStyle, subAction.DrawStyle);
                Assert.AreEqual(expectedDrawStyle, action.DrawStyle);
                Assert.AreNotEqual(defaultSubActionDrawStyle,
                    subAction.DrawStyle);
                Assert.AreNotEqual(defaultActionDrawStyle, action.DrawStyle);
            });

        }
    }
}
