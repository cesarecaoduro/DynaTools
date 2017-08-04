using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Dynamo.Graph.Nodes;
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
        public static Dictionary<string, object> CableTrayCollector(Boolean refresh = false)
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
        public static Dictionary<string, object> DuctCollector(Boolean refresh = false)
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
        public static Dictionary<string, object> PipeCollector(Boolean refresh = false)
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

       
    }

   

   

}
