using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.DataModel.EObjects;
using Eplan.EplApi.DataModel.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner
{
    public class ImportICPProjectsAction : IEplAction
    {
        ~ImportICPProjectsAction() { }

        public bool Execute(ActionCallingContext oActionCallingContext)
        {
            try
            {
                Project currentProject = EProjectManager.GetInstance()
                    .GetCurrentPrj();
                if (currentProject == null)
                {
                    MessageBox.Show("Нет открытого проекта!", "EPlaner",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return true;
                }

                //create new Schematic page in current project
                PagePropertyList oPagePropList = new PagePropertyList();
                //set Plant
                oPagePropList[Eplan.EplApi.DataModel.Properties.Page.DESIGNATION_PLANT] = "P1";
                oPagePropList[Eplan.EplApi.DataModel.Properties.Page.PAGE_NAME] = "Устройства";
                //set Location
                //oPagePropList[Eplan.EplApi.DataModel.Properties.Page.DESIGNATION_FUNCTIONALASSIGNMENT] = "L1";
                Page oNewPage = new Page();

                SymbolLibrary SPECIAL = new SymbolLibrary(currentProject, "SPECIAL");
                SymbolLibrary BMK = new SymbolLibrary(currentProject, "Bmk");

                //var symbol = new Symbol(SPECIAL, "SC");
                //var symbolV = new SymbolVariant(symbol, 1);
                

                //var shield = new Shield();
                //var plc = new PLC();


                // Создание и блокировка страницы
                oNewPage.Create(currentProject, DocumentTypeManager.DocumentType.ProcessAndInstrumentationDiagram, oPagePropList);
                oNewPage.LockObject();


                var dev = new Symbol(BMK, "xE4");
                var devV = new SymbolVariant(dev, 0);
                var func = new Function();

                func.Create(oNewPage, devV);
                
                //func.Name = "-V1";
                func.VisibleName = "-V1";
                func.Location = new Eplan.EplApi.Base.PointD(100, 100);


                var obj = new Symbol(SPECIAL, "SC");
                var objV = new SymbolVariant(obj, 0);
                var obj_func = new Eplan.EplApi.DataModel.LocationBox();
                obj_func.Create(oNewPage, objV);

                obj_func.LockObject();

                obj_func.Location = new Eplan.EplApi.Base.PointD(80, 80);
                obj_func.SetLogicalArea(new Eplan.EplApi.Base.PointD(80, 80), new Eplan.EplApi.Base.PointD(120, 120));
                obj_func.VisibleName = "+A1";


                var PLC_page = new Page();
                oPagePropList[Eplan.EplApi.DataModel.Properties.Page.PAGE_NOMINATIOMN] = "ПЛК";

                PLC_page.Create(currentProject, DocumentTypeManager.DocumentType.Overview, oPagePropList);
                PLC_page.LockObject();

                var plc = new Symbol(SPECIAL, "PLCC");
                var plcV = new SymbolVariant(plc, 0);
                var plc_func = new Function();

                plc_func.Create(PLC_page, plcV);
                //plc_func.Location = new Eplan.EplApi.Base.PointD(100, 500);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return true;
        }

        public bool OnRegister(ref string Name, ref int Ordinal)
        {
            Name = "ImportICPProjects";
            Ordinal = 30;

            return true;
        }

        public void GetActionProperties(ref ActionProperties actionProperties)
        {
        }
    }
}
