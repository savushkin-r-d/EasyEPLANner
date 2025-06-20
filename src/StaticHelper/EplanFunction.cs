using EasyEPlanner.Extensions;
using Eplan.EplApi.DataModel;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace StaticHelper
{
    /// <summary>
    /// <inheritdoc cref="IEplanFunction"/>
    /// </summary>
    /// <param name="function"></param>
    [ExcludeFromCodeCoverage]
    public class EplanFunction(Function function) : IEplanFunction
    {
        public Function Function => function;

        public string IP 
        {
            get => function.Properties.FUNC_PLCGROUP_STARTADDRESS.GetString();
            set => function.Properties.FUNC_PLCGROUP_STARTADDRESS = value;
        }

        public string SubnetMask 
        { 
            get => function.Properties.FUNC_PLC_SUBNETMASK.GetString(); 
            set => function.Properties.FUNC_PLC_SUBNETMASK = value;
        }

        public string Gateway 
        { 
            get => GetSupplemenataryField(15); 
            set => SetSupplementaryField(15, value); 
        }

        public int ClampNumber => int.TryParse(
            function.Properties.FUNC_ADDITIONALIDENTIFYINGNAMEPART.GetString(),
            out int clamp) ? clamp : -1;

        public string VisibleName => function.VisibleName;

        public IEnumerable<IEplanFunction> SubFunctions => function.SubFunctions.Select(f => new EplanFunction(f));

        public string FunctionalText 
        { 
            get => function.GetFunctionalText();
            set => function.Properties.FUNC_TEXT = value;
        }

        public string Name => function.Name;

        public bool PlacedOnCircuit => function.Page.PageType is DocumentTypeManager.DocumentType.Circuit;

        public string GetSupplemenataryField(int propertyIndex)
            => function.Properties.FUNC_SUPPLEMENTARYFIELD[propertyIndex].GetString();

        public void SetSupplementaryField(int propertyIndex, string value)
        {
            try
            {
                function.LockObject();
            }
            catch 
            {
                //do nothing
            }
            
            function.Properties.FUNC_SUPPLEMENTARYFIELD[propertyIndex] = value;
        }

        public override bool Equals(object obj) 
            => function == (obj as EplanFunction)?.Function;

        public override int GetHashCode() 
            => function?.GetHashCode() ?? 0;

        public void Lock()
        {
            try
            {
                function.LockObject();
            }
            catch
            {
                //do nothing
            }
        }

        public bool IsMainFunction => function.IsMainFunction;

        public bool Expanded 
        {
            get => bool.TryParse(GetSupplemenataryField(13), out var res) && res;
            set => SetSupplementaryField(13, value.ToString());
        }


        public bool Off
        {
            get => GetSupplemenataryField(2).Trim() == "1";
            set => SetSupplementaryField(2, value ? "1" : "");
        }

        public string SubType
        {
            get => GetSupplemenataryField(2);
            set => SetSupplementaryField(2, value);
        }

        public string Parameters
        {
            get => GetSupplemenataryField(3);
            set => SetSupplementaryField(3, value);
        }

        public string Properties
        {
            get => GetSupplemenataryField(4);
            set => SetSupplementaryField(4, value);
        }

        public string RuntimeParameters
        {
            get => GetSupplemenataryField(5);
            set => SetSupplementaryField(5, value);
        }

        public string OldDeviceName
        {
            get => GetSupplemenataryField(10);
            set => SetSupplementaryField(10, value);
        }

        public string Article
        {
            get
            {
                var articleName = string.Empty;
                if (function == null) return articleName;

                var articlesRefs = function.ArticleReferences;
                if (articlesRefs.Length > 0 &&
                    !string.IsNullOrEmpty(function.ArticleReferences[0].PartNr))
                {
                    articleName = function.ArticleReferences[0].PartNr;
                }

                return articleName;
            }
        }

        public bool IsValid => function.IsValid;

        public string Description
        {
            get => function.Properties.FUNC_COMMENT.GetString();
            set => function.Properties.FUNC_COMMENT = value;
        }
    }
}
