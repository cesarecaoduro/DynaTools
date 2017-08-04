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

        /// <summary>
        /// This node allows to collect elements of category froma specified document
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="category">Category</param>
        /// <param name="refresh"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "Elements" })]
        public static Dictionary<string, object> ElementByCategoryFromDocument(Document doc, Revit.Elements.Category category, Boolean refresh = false)
        {
            List<Revit.Elements.Element> elList = new List<Revit.Elements.Element>();
            if (refresh)
            {

                BuiltInCategory myCatEnum = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), category.Id.ToString());

                ElementCategoryFilter filter = new ElementCategoryFilter(myCatEnum);

                //Document doc = DocumentManager.Instance.CurrentDBDocument;
                //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
                //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
                //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

                FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(filter).WhereElementIsNotElementType();
                foreach (Autodesk.Revit.DB.Element e in collector)
                {
                    DynaFunctions f = new DynaFunctions();
                    elList.Add(doc.GetElement(e.Id).ToDSType(true));
                }
            }

            return new Dictionary<string, object>
            {
                { "Elements", elList},

            };

        }

        /// <summary>
        /// This node allows to collect elements from a document and, at the same time,
        /// collect a list of parameters value
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="category">Category</param>
        /// <param name="parameters">List of Parameters</param>
        /// <param name="refresh"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "Elements", "Values", "Result" })]
        public static Dictionary<string, object> ElementParametersByCategoryFromDocument(Document doc, Revit.Elements.Category category, List<string> parameters = null, Boolean refresh = false)
        {
            string executed = "";
            List<Revit.Elements.Element> elList = new List<Revit.Elements.Element>();
            List<List<object>> values = new List<List<object>>();
            if (refresh)
            {

                BuiltInCategory myCatEnum = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), category.Id.ToString());
                ElementCategoryFilter filter = new ElementCategoryFilter(myCatEnum);

                //Document doc = DocumentManager.Instance.CurrentDBDocument;
                //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
                //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
                //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

                FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(filter).WhereElementIsNotElementType();
                List<Definition> param = new List<Definition>();
                Autodesk.Revit.DB.Element e = collector.FirstElement();
                ParameterSet paramSet = e.Parameters;

                foreach (string s in parameters)
                {
                    foreach (Autodesk.Revit.DB.Parameter p in paramSet)
                    {
                        if (p.Definition.Name == s)
                        {
                            param.Add(p.Definition);
                        }
                    }
                }

                if (param.Count != parameters.Count)
                {
                    executed = "Check parameters name";
                }
                else
                {
                    executed = "Executed";

                    foreach (Autodesk.Revit.DB.Element el in collector)
                    {
                        List<object> elParams = new List<object>();
                        DynaFunctions f = new DynaFunctions();
                        elList.Add(doc.GetElement(el.Id).ToDSType(true));
                        foreach (Definition p in param)
                        {
                            switch (el.get_Parameter(p).StorageType)
                            {
                                case StorageType.Double: elParams.Add(el.get_Parameter(p).AsDouble()); break;
                                case StorageType.Integer: elParams.Add(el.get_Parameter(p).AsInteger()); break;
                                case StorageType.String: elParams.Add(el.get_Parameter(p).AsString()); break;
                                case StorageType.ElementId: elParams.Add(el.get_Parameter(p).AsValueString()); break;
                            }
                        }
                        values.Add(elParams);
                    }
                }
            }

            return new Dictionary<string, object>
            {
                { "Elements", elList},
                { "Values", values},
                { "Result", executed}

            };

        }
    }

    public static class Parameters
    {
        [MultiReturn(new[] { "found", "notFound"})]
        public static Dictionary<string, object> CheckIfParameterExist(List<String> parameters)
        {
            Document doc = DocumentManager.Instance.CurrentDBDocument;
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(ParameterElement));
            List<string> found = new List<string>();
            List<string> notFound = new List<string>();

            foreach (string s in parameters)
            {
                int c = 0;
                foreach (ParameterElement p in collector)
                {
                    if (p.Name == s)
                    {
                        c += 1;
                    }
                }
                if (c > 0)
                {
                    found.Add(s);
                }
                else
                {
                    notFound.Add(s);
                }
            }

            return new Dictionary<string, object>
            {
                { "found", found},
                { "notFound", notFound}
            };
        }

        //[MultiReturn(new[] { "filled", "empty" })]
        //public static Dictionary<string, object> CheckIfParameterIsEmpty(List<Revit.Elements.Element> elements, List<String> parameters)
        //{
        //    Document doc = DocumentManager.Instance.CurrentDBDocument;
        //    List<List<Revit.Elements.Element>> filled = new List<List<Revit.Elements.Element>>();
        //    List<List<Revit.Elements.Element>> empty = new List<List<Revit.Elements.Element>>();

            

        //    foreach (string s in parameters)
        //    {
        //        List<Revit.Elements.Element> fi = new List<Revit.Elements.Element>();
        //        List<Revit.Elements.Element> em = new List<Revit.Elements.Element>();
        //        foreach (Revit.Elements.Element e in elements)
        //        {
        //            if (e.GetParameterValueByName(s) != null)
        //            {
        //                fi.Add(e);
        //            }
        //            else
        //            {
        //                em.Add(e);
        //            }
        //        }
        //        empty.Add(em);
        //        filled.Add(fi);
        //    }

        //    return new Dictionary<string, object>
        //    {
        //        { "filled", filled},
        //        { "empty", empty}
        //    };
        //}

    };

    /// <summary>
    /// 
    /// </summary>
    public static class Tools
    {
        [MultiReturn(new[] { "elements"})]
        public static Dictionary<string, object> ChangeFittingsLevel(List<Revit.Elements.Element> elements, Revit.Elements.Level endLevel)
        {
            Document doc = DocumentManager.Instance.CurrentDBDocument;
            string result = "";
            Autodesk.Revit.DB.Element ll = doc.GetElement(endLevel.UniqueId.ToString());
            double ofEndLevel = ll.get_Parameter(BuiltInParameter.LEVEL_ELEV).AsDouble();
            ElementId endLeveliD = ll.Id;
            
            try
            {
                foreach (Revit.Elements.Element e in elements)
                {
                    Autodesk.Revit.DB.Element el = doc.GetElement(e.UniqueId.ToString());
                    double elOffset = el.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM).AsDouble();
                    ElementId startLevel = el.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId();
                    double ofStartLevel = doc.GetElement(startLevel).get_Parameter(BuiltInParameter.LEVEL_ELEV).AsDouble();
                    double newOffset = -ofEndLevel + elOffset + ofStartLevel;
                    el.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM).Set(newOffset);
                    el.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).Set(endLeveliD);
                }

                result = "Executed";
            }
            catch (Exception ex)
            {
                result = "Not executed: " + ex.Message;
            }
  
            return new Dictionary<string, object>
            {
                {"elements", elements},
                { "result", result}
            };
        }

       

    };


}
