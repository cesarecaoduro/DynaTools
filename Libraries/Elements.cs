using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.DesignScript.Runtime;
using Dynamo.Graph.Nodes;
using Autodesk.Revit.Attributes;
using RevitServices.Persistence;
using System.Windows.Controls;
using Autodesk.DesignScript.Geometry;
using Revit.Elements;
using System.Xml;
using System.Data.SqlClient;
using CoreNodeModels;
using CoreNodeModelsWpf.Nodes;
using RevitServices.EventHandler;
using Dynamo.Wpf;
using Dynamo.Controls;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using Dynamo.Configuration;
using System.Windows;
using DynaToolsFunctions;

namespace ElementTools
{
    /// <summary>
    /// 
    /// </summary>
    public static class Elements
    {
        /// <summary>
        /// This node get all paramaters (Type and Instance) for a list of elements.
        /// To be used carefully due to the high amount of information retrieved.
        /// </summary>
        /// <param name="elements">List of elements</param>
        /// <returns></returns>
        [MultiReturn(new[] { "parameterInstanceNames", "parameterTypeNames", "parameterInstanceList", "parameterTypeList" })]
        public static Dictionary<string, object> getAllElementsParameters(List<Revit.Elements.Element> elements)
        {
            Document doc = DocumentManager.Instance.CurrentDBDocument;
            List<object> listType = new List<object>();
            List<object> listInstance = new List<object>();
            Autodesk.Revit.DB.Element el = doc.GetElement(elements[0].UniqueId.ToString());
            Autodesk.Revit.DB.Element elType = doc.GetElement(el.GetTypeId());
            ParameterSet paramSet = el.Parameters;
            ParameterSet paramSetType = elType.Parameters;
            List<string> paramInstanceNames = new List<string>();
            List<string> paramTypeNames = new List<string>();

            foreach (Revit.Elements.Element e in elements)
            {
                List<object> itemInstanceParams = new List<object>();
                el = doc.GetElement(e.UniqueId.ToString());
                foreach (Autodesk.Revit.DB.Parameter p in paramSet)
                {
                    paramInstanceNames.Add(p.Definition.Name);
                    if (el.get_Parameter(p.Definition) != null)
                    {
                        switch (el.get_Parameter(p.Definition).StorageType)
                        {
                            case StorageType.Double: itemInstanceParams.Add(el.get_Parameter(p.Definition).AsValueString()); break;
                            case StorageType.Integer: itemInstanceParams.Add(el.get_Parameter(p.Definition).AsInteger()); break;
                            case StorageType.String: itemInstanceParams.Add(el.get_Parameter(p.Definition).AsString()); break;
                            case StorageType.ElementId: itemInstanceParams.Add(el.get_Parameter(p.Definition).AsValueString()); break;
                        }
                    }
                    else
                    {
                        itemInstanceParams.Add("");
                    }

                }

                listInstance.Add(itemInstanceParams);
            }

            foreach (Revit.Elements.Element e in elements)
            {
                List<object> itemTypeParams = new List<object>();
                el = doc.GetElement(e.UniqueId.ToString());
                Autodesk.Revit.DB.Element eType = doc.GetElement(el.GetTypeId());
                foreach (Autodesk.Revit.DB.Parameter p in paramSetType)
                {
                    paramTypeNames.Add(p.Definition.Name);
                    if (eType.get_Parameter(p.Definition) != null)
                        {
                            switch (eType.get_Parameter(p.Definition).StorageType)
                            {
                                case StorageType.Double: itemTypeParams.Add(eType.get_Parameter(p.Definition).AsValueString()); break;
                                case StorageType.Integer: itemTypeParams.Add(eType.get_Parameter(p.Definition).AsInteger()); break;
                                case StorageType.String: itemTypeParams.Add(eType.get_Parameter(p.Definition).AsString()); break;
                                case StorageType.ElementId: itemTypeParams.Add(eType.get_Parameter(p.Definition).AsValueString()); break;
                            }
                        }
                        else
                        {
                            itemTypeParams.Add("");
                        }

                }

                listType.Add(itemTypeParams);
            }

            return new Dictionary<string, object>
            {
                { "parameterInstanceNames", paramInstanceNames},
                { "parameterTypeNames", paramTypeNames},
                { "parameterInstanceList", listInstance}, 
                { "parameterTypeList", listType},
            };
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements">List of elements</param>
        /// <param name="parameterName">Copy to parameter</param>
        [IsDesignScriptCompatible]
        [IsVisibleInDynamoLibrary(false)]
        public static string copyIDToParameter(List<Revit.Elements.Element> elements, string parameterName = "")
        {
            Document doc = DocumentManager.Instance.CurrentDBDocument;
            string executed = "";

            if (parameterName != "")
            {
                foreach (Revit.Elements.Element el in elements)
                {

                    using (Transaction t = new Transaction(doc,"copyID"))
                    {
                        t.Start("CopyID");
                        el.SetParameterByName(parameterName, el.Id.ToString());
                        t.Commit();
                        executed = "Executed";
                    }
                        
                }


            }
            else
            {
                executed = "Set a parameter name";
            }

            return executed;
        }
    }
}
