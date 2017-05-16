using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using DynaToolsFunctions;
using Revit.Elements;
using RevitServices.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RebarTools
{
    /// <summary>
    /// 
    /// </summary>
    public static class Query
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements">List of rebars elements</param>
        /// <returns></returns>
        [MultiReturn(new[] { "Elements"})]
        public static Dictionary<string, object> getHostElement(List<Revit.Elements.Element> elements)
        {
          
            Document doc = DocumentManager.Instance.CurrentDBDocument;
            List<Revit.Elements.Element> elIdList = new List<Revit.Elements.Element>();
            //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
            //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

            foreach (Revit.Elements.Element e in elements)
            {
                Autodesk.Revit.DB.Element el = doc.GetElement(e.UniqueId.ToString());
                Rebar r = el as Rebar;
                ElementId elId = r.GetHostId();
                elIdList.Add(doc.GetElement(elId).ToDSType(true));
            }

            return new Dictionary<string, object>
            {
                { "hostElements", elIdList},
            };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements">List of elements that can host rebars</param>
        /// <returns></returns>
        [MultiReturn(new[] { "Covers"})]
        public static Dictionary<string, object> getRebarCover(List<Revit.Elements.Element> elements)
        {
            string message = "";
            Document doc = DocumentManager.Instance.CurrentDBDocument;
            List<Revit.Elements.Element> elIdList = new List<Revit.Elements.Element>();
            DynaFunctions f = new DynaFunctions();
            //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
            //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;
            List<double> topExterior = new List<double>();
            List<double> bottomInterior = new List<double>();
            List<double> others = new List<double>();
            Autodesk.Revit.DB.Element el;
            List<BuiltInParameter> covers = new List<BuiltInParameter>
            {
                BuiltInParameter.CLEAR_COVER_EXTERIOR,
                BuiltInParameter.CLEAR_COVER_TOP,
                BuiltInParameter.CLEAR_COVER_INTERIOR,
                BuiltInParameter.CLEAR_COVER_BOTTOM,
                BuiltInParameter.CLEAR_COVER_OTHER,
                BuiltInParameter.CLEAR_COVER
            };
            List<List<double>> coverValues = new List<List<double>>();

            foreach (Revit.Elements.Element e in elements)
            {
                try
                {
                    el = doc.GetElement(e.UniqueId.ToString());
                    List<double> coverValue = new List<double>();
                    foreach (BuiltInParameter c in covers)
                    {
                        ElementId rctId = el.get_Parameter(c).AsElementId();
                        RebarCoverType rct = doc.GetElement(rctId) as RebarCoverType;
                        double cv = f.feetToMillimeter(rct.get_Parameter(BuiltInParameter.COVER_TYPE_LENGTH).AsDouble());
                        if (cv >= 0)
                        {
                            coverValue.Add(cv);
                        }
                    }
                    coverValues.Add(coverValue);
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }

                
            }

            return new Dictionary<string, object>
            { 
                { "Covers", coverValues},
                { "Message", message },
            };

        }

    }





}
