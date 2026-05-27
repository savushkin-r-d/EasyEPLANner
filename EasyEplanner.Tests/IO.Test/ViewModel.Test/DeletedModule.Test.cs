using IO;
using IO.ViewModel;
using Moq;
using NUnit.Framework;
using StaticHelper;
using Color = System.Drawing.Color;

namespace IOTests
{
    public class DeletedModuleTest
    {
        [Test]
        public void Getters_ReturnModuleData()
        {
            var function = Mock.Of<IEplanFunction>(f =>
                f.VisibleName == "=PLC-DEL338");
            var moduleInfo = Mock.Of<IIOModuleInfo>(info =>
                info.Name == "750-430" &&
                info.Description == "Digital input" &&
                info.TypeName == "DI" &&
                info.ModuleColor == Color.Blue);
            var module = Mock.Of<IIOModule>(m =>
                m.Name == "DEL338" &&
                m.Info == moduleInfo &&
                m.Function == function);

            var deletedModule = new DeletedModule(module);

            Assert.Multiple(() =>
            {
                Assert.AreEqual("DEL338", deletedModule.Name);
                Assert.AreEqual("Digital input", deletedModule.Description);
                Assert.AreSame(module, deletedModule.IOModule);
                Assert.AreEqual("=PLC-DEL338",
                    (deletedModule as IToolTip).Name);
                Assert.AreEqual(
                    "Артикул: 750-430\nОписание: Digital input\nDI",
                    (deletedModule as IToolTip).Description);
                Assert.AreEqual(IO.ViewModel.Icon.BlueModule,
                    (deletedModule as IHasIcon).Icon);
            });
        }
    }
}

