using Eplan.EplApi.DataModel.EObjects;
using IO;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyEPlanner.ProjectImportICP
{
    /// <summary>
    /// Описание импортированного модуля
    /// </summary>
    public interface IImportModule
    {
        /// <summary>
        /// Адресное пространство по типу
        /// </summary>
        /// <param name="channelType">Тип канала</param>
        int AddressSpace(string channelType);

        /// <summary>
        /// Описание модуля
        /// </summary>
        IOModuleInfo ModuleInfo { get; }
    }

    /// <summary>
    /// Описание импортированного модуля (EPLAN-функции модуля и клемм, а также их адреса)
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ImportModule : IImportModule
    {
        public ImportModule(List<Terminal> clamps, IOModuleInfo moduleInfo, PLC function)
        {
            foreach (var clamp in clamps)
            {
                string clampStr = clamp.Properties.FUNC_ADDITIONALIDENTIFYINGNAMEPART.ToString();

                if (int.TryParse(clampStr, out int clampNumber))
                {
                    int clampIndex = -1;


                    if (clampNumber < moduleInfo.ChannelAddressesIn.Count())
                    {
                        clampIndex = moduleInfo.ChannelAddressesIn[clampNumber];
                    }
                    
                    if (clampNumber < moduleInfo.ChannelAddressesOut.Count())
                    {
                        clampIndex = moduleInfo.ChannelAddressesOut[clampNumber];
                    }

                    if (clampIndex >= 0)
                        this.clamps[clampIndex] = clamp;
                }
            }

            Function = function;
            ModuleInfo = moduleInfo;

            addressSpace["AO"] = ModuleInfo.AOCount;
            addressSpace["AI"] = ModuleInfo.AICount;
            addressSpace["DO"] = ModuleInfo.DOCount;
            addressSpace["DI"] = ModuleInfo.DICount;
        }

        private readonly Dictionary<string, int> addressSpace = new Dictionary<string, int>();

        private readonly Dictionary<int, Terminal> clamps = new Dictionary<int, Terminal>();

        public Dictionary<int, Terminal> Clamps => clamps;

        public IOModuleInfo ModuleInfo { get; private set; }

        public PLC Function { get; private set; }

        public int AddressSpace(string channelType)
            => addressSpace[channelType];
    }
}
