using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DynaToolsFunctions;
using Revit.Elements;
using RevitServices.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CollectorTools
{
    /// <summary>
    /// Collection of tools to get quantities of elements and useful parameters values
    /// </summary>
    public static class Collectors
    {

        /// <summary>
        /// Collect all the cable trays in the active document and push some useful data
        /// </summary>
        /// <param name="refresh">Refresh the output</param>
        /// <returns></returns>
        [MultiReturn(new[] { "Elements", "Id", "Name", "Length", "Offset", "Size", "Level" })]
        public static Dictionary<string, object> CableTray(Boolean refresh = false)
        {

            List<Revit.Elements.Element> elList = new List<Revit.Elements.Element>();
            List<ElementId> elIdList = new List<ElementId>();
            List<string> elNameList = new List<string>();
            List<double> elLengthList = new List<double>();
            List<double> elOffsetList = new List<double>();
            List<string> elCalcSizeList = new List<string>();
            List<string> elLevelList = new List<string>();

            if (refresh)
            {
                
                Document doc = DocumentManager.Instance.CurrentDBDocument;
                //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
                //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
                //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

                FilteredElementCollector collector = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_CableTray);

                foreach (Autodesk.Revit.DB.Element e in collector)
                {
                    DynaFunctions f = new DynaFunctions();

                    elIdList.Add(e.Id);
                    elNameList.Add(e.Name);
                    elLengthList.Add(f.feetToMillimeter(f.paramAsDouble(e, BuiltInParameter.CURVE_ELEM_LENGTH)));
                    elList.Add(doc.GetElement(e.Id).ToDSType(true));
                    elOffsetList.Add(f.feetToMillimeter(f.paramAsDouble(e, BuiltInParameter.RBS_OFFSET_PARAM)));
                    elCalcSizeList.Add(f.paramAsString(e, BuiltInParameter.RBS_CALCULATED_SIZE));
                    elLevelList.Add(f.paramAsValueString(e, BuiltInParameter.RBS_START_LEVEL_PARAM));

                }
            }

            return new Dictionary<string, object>
            {
                { "Elements", elList},
                { "Id", elIdList},
                { "Name", elNameList },
                { "Length", elLengthList },
                { "Offset", elOffsetList },
                { "Size", elCalcSizeList },
                { "Level", elLevelList },
            };

        }

        /// <summary>
        /// Collect all the ducts in the active document and push some useful data
        /// </summary>
        /// <param name="refresh">Refresh the output</param>
        /// <returns></returns>
        [MultiReturn(new[] { "Elements", "Id", "Name", "Length", "Area", "Offset", "Size", "Level"})]
        public static Dictionary<string, object> Duct(Boolean refresh = false)
        {

            List<Revit.Elements.Element> elList = new List<Revit.Elements.Element>();
            List<ElementId> elIdList = new List<ElementId>();
            List<string> elNameList = new List<string>();
            List<double> elLengthList = new List<double>();
            List<double> elOffsetList = new List<double>();
            List<string> elCalcSizeList = new List<string>();
            List<string> elLevelList = new List<string>();
            List<double> elArea = new List<double>();

            if (refresh)
            {

                Document doc = DocumentManager.Instance.CurrentDBDocument;
                //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
                //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
                //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

                FilteredElementCollector collector = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_DuctCurves);

                foreach (Autodesk.Revit.DB.Element e in collector)
                {
                    DynaFunctions f = new DynaFunctions();

                    elIdList.Add(e.Id);
                    elNameList.Add(e.Name);
                    elLengthList.Add(f.feetToMillimeter(f.paramAsDouble(e, BuiltInParameter.CURVE_ELEM_LENGTH)));
                    elList.Add(doc.GetElement(e.Id).ToDSType(true));
                    elOffsetList.Add(f.feetToMillimeter(f.paramAsDouble(e, BuiltInParameter.RBS_OFFSET_PARAM)));
                    elCalcSizeList.Add(f.paramAsString(e, BuiltInParameter.RBS_CALCULATED_SIZE));
                    elLevelList.Add(f.paramAsValueString(e, BuiltInParameter.RBS_START_LEVEL_PARAM));
                    elArea.Add(f.sqfToSqm(f.paramAsDouble(e,BuiltInParameter.RBS_CURVE_SURFACE_AREA)));

                }
            }

            return new Dictionary<string, object>
            {
                { "Elements", elList},
                { "Id", elIdList},
                { "Name", elNameList },
                { "Length", elLengthList },
                { "Area", elArea },
                { "Offset", elOffsetList },
                { "Size", elCalcSizeList },
                { "Level", elLevelList },
            };

        }

        /// <summary>
        /// Collect all the pipes in the active document and push some useful data
        /// </summary>
        /// <param name="refresh">Refresh the output</param>
        /// <returns></returns>
        [MultiReturn(new[] { "Elements", "Id", "Name", "Length", "Offset", "Size", "Level" })]
        public static Dictionary<string, object> Pipe(Boolean refresh = false)
        {

            List<Revit.Elements.Element> elList = new List<Revit.Elements.Element>();
            List<ElementId> elIdList = new List<ElementId>();
            List<string> elNameList = new List<string>();
            List<double> elLengthList = new List<double>();
            List<double> elOffsetList = new List<double>();
            List<string> elCalcSizeList = new List<string>();
            List<string> elLevelList = new List<string>();

            if (refresh)
            {

                Document doc = DocumentManager.Instance.CurrentDBDocument;
                //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
                //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
                //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

                FilteredElementCollector collector = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_PipeCurves);

                foreach (Autodesk.Revit.DB.Element e in collector)
                {
                    DynaFunctions f = new DynaFunctions();

                    elIdList.Add(e.Id);
                    elNameList.Add(e.Name);
                    elLengthList.Add(f.feetToMillimeter(f.paramAsDouble(e, BuiltInParameter.CURVE_ELEM_LENGTH)));
                    elList.Add(doc.GetElement(e.Id).ToDSType(true));
                    elOffsetList.Add(f.feetToMillimeter(f.paramAsDouble(e, BuiltInParameter.RBS_OFFSET_PARAM)));
                    elCalcSizeList.Add(f.paramAsString(e, BuiltInParameter.RBS_CALCULATED_SIZE));
                    elLevelList.Add(f.paramAsValueString(e, BuiltInParameter.RBS_START_LEVEL_PARAM));
                }
            }

            return new Dictionary<string, object>
            {
                { "Elements", elList},
                { "Id", elIdList},
                { "Name", elNameList },
                { "Length", elLengthList },
                { "Offset", elOffsetList },
                { "Size", elCalcSizeList },
                { "Level", elLevelList },
            };

        }

        /// <summary>
        /// This node allows to collect elements of category froma specified document
        /// </summary>
        /// <param name="doc">Document</param>
        /// <param name="category">Category</param>
        /// <param name="refresh"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "Elements"})]
        public static Dictionary<string, object> ElementByCategoryFromDocument(Document doc, Revit.Elements.Category category, Boolean refresh = false)
        {
            List<Revit.Elements.Element> elList = new List<Revit.Elements.Element>();
            if (refresh)
            {

                BuiltInCategory myCatEnum = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory),category.Id.ToString());

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
        [MultiReturn(new[] { "Elements" , "Values", "Result"})]
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

   

   

}
