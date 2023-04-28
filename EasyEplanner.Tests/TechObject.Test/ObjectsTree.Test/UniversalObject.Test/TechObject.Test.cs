using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace EasyEplannerTests.TechObjectTest.ObjectsTreeTest.UniversalObjecsTest
{
    public class TechObjectTest
    {
        [Test]
        public void SaveAsLuaTable_CheckResultString()
        {
            string name = "name_to";
            int globalNum = 1;
            TechObject.GetN getN = (x) => globalNum;
            int techNumber = 2;
            int techType = 3;
            string eplanName = "eplanName_to";
            int cooperParamNumber = 4;
            string BCName = "BC_name_to";
            string prefix = "\t\t";

            string modeName = "name_mode";
            string baseOperationName = "baseOperationName";

            var techObject = new TechObject.TechObject(name, getN, techNumber,
                techType, eplanName, cooperParamNumber, BCName, string.Empty, null);
            techObject.ModesManager.AddMode(modeName, baseOperationName);

            var expected = new StringBuilder()
                .Append($"\t[ {globalNum} ] =\n")
                .Append(prefix).Append("{\n")
                .Append(prefix).Append($"n          = {techNumber},\n")
                .Append(prefix).Append($"tech_type  = {techType},\n")
                .Append(prefix).Append($"name       = '{name}',\n")
                .Append(prefix).Append($"name_eplan = '{eplanName}',\n")
                .Append(prefix).Append($"name_BC    = '{BCName}',\n")
                .Append(prefix).Append($"cooper_param_number = {cooperParamNumber},\n\n")
                .Append(prefix).Append($"modes =\n")
                .Append(prefix).Append($"\t{{\n")
                .Append(prefix).Append($"\t[ 1 ] =\n")
                .Append(prefix).Append($"\t\t{{\n")
                .Append(prefix).Append($"\t\tname = '{modeName}',\n")
                .Append(prefix).Append($"\t\t}},\n")
                .Append(prefix).Append($"\t}},\n")
                .Append(prefix).Append($"}},\n");

            Assert.AreEqual(expected.ToString(), techObject.SaveAsLuaTable(prefix, globalNum));
        }
    }
}
