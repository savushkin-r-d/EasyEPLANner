using NUnit.Framework;
using LuaInterface;
using System.Linq;


namespace StaticHelper.Tests
{
    class LuaHelperTest
    {
        [TestCase("return { par1 = '1', par2 = 2 }", "1 2")]
        [TestCase("return { { par1 = 1 } }", "table")]
        [TestCase("return { par1 = 1, par2 = 'par2' }", "1 par2")]
        public void ConvertLuaTableToString_ReturnsLuaString(string luaTableString, string expected)
        {
            Lua lua = new Lua();
            LuaTable luaTable = (LuaTable) lua.DoString(luaTableString)[0];

            string resultString = LuaHelper.ConvertLuaTableToString(luaTable);
            Assert.AreEqual(expected, resultString);
        }


        [TestCase("return { par1 = '1', par2 = '2' }", "par1 = 1, par2 = 2")]
        [TestCase("return { par1 = '1', par2 = 'par2' }", "par1 = 1, par2 = par2")]
        public void ConvertLuaTableToCArray_ReturnsObjectProperty(string luaTableString, string expected)
        {
            Lua lua = new Lua();
            LuaTable luaTable = (LuaTable)lua.DoString(luaTableString)[0];

            Editor.ObjectProperty[] objectProperties = LuaHelper.ConvertLuaTableToCArray(luaTable);
            var properties = objectProperties.OfType<Editor.ObjectProperty>()
                .Select(op => op.DisplayText[0].ToString() + " = "
                + op.DisplayText[1].ToString()).ToArray();
            var resultString = string.Join(", ", properties);
            Assert.AreEqual(expected, resultString);
        }
    }
}
