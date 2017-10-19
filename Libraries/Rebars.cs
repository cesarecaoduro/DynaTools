using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using DynaToolsFunctions;
using Revit.Elements;
using RevitServices.Persistence;
using System;
using System.Collections.Generic;
using Autodesk.Revit.DB.Structure;

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
        [MultiReturn(new[] { "hostElements" })]
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
        [MultiReturn(new[] { "topExterior", "bottomInterior", "others" })]
        public static Dictionary<string, object> getRebarCover(List<Revit.Elements.Element> elements)
        {
            string message = "";
            Document doc = DocumentManager.Instance.CurrentDBDocument;
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
            foreach (Revit.Elements.Element e in elements)
            {
                el = doc.GetElement(e.UniqueId.ToString());
                List<double> coverValue = new List<double>();
                foreach (BuiltInParameter c in covers)
                {
                    try
                    {
                        ElementId rctId = el.get_Parameter(c).AsElementId();
                        RebarCoverType rct = doc.GetElement(rctId) as RebarCoverType;
                        double cv = f.feetToMillimeter(rct.get_Parameter(BuiltInParameter.COVER_TYPE_LENGTH).AsDouble());
                        coverValue.Add(cv);
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                    }

                }
                topExterior.Add(coverValue[0]);
                bottomInterior.Add(coverValue[1]);
                others.Add(coverValue[2]);
            }

            return new Dictionary<string, object>
            {
                { "topExterior", topExterior},
                { "bottomInterior", bottomInterior},
                { "others", others},
                //{ "Message", message },
            };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rebars"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "rebarStyle", "rebarBarType", "hookStartType", "hookEndType", "hookStartOrientation", "hookEndOrientation" })]
        public static Dictionary<string, object> getRebarProperties(List<Revit.Elements.Element> rebars)
        {
            string message = "";
            Document doc = DocumentManager.Instance.CurrentDBDocument;
            DynaFunctions f = new DynaFunctions();
            //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
            //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;
            List<string> rStyle = new List<string>();
            List<Revit.Elements.Element> rType = new List<Revit.Elements.Element>();
            List<Revit.Elements.Element> hStartType = new List<Revit.Elements.Element>();
            List<Revit.Elements.Element> hEndType = new List<Revit.Elements.Element>();
            List<string> hStartOrient = new List<string>();
            List<string> hEndOrient = new List<string>();

            foreach (Revit.Elements.Element r in rebars)
            {
                Autodesk.Revit.DB.Element el = doc.GetElement(r.UniqueId.ToString());
                rStyle.Add(el.get_Parameter(BuiltInParameter.REBAR_ELEM_HOOK_STYLE).AsValueString());
                ElementId eId = el.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsElementId();
                rType.Add(doc.GetElement(eId).ToDSType(true));
                try
                {
                    eId = el.get_Parameter(BuiltInParameter.REBAR_ELEM_HOOK_START_TYPE).AsElementId();
                    hStartType.Add(doc.GetElement(eId).ToDSType(true));

                }
                catch { hStartType.Add(null); }
                try
                {
                    eId = el.get_Parameter(BuiltInParameter.REBAR_ELEM_HOOK_END_TYPE).AsElementId();
                    hEndType.Add(doc.GetElement(eId).ToDSType(true));
                }
                catch { hEndType.Add(null); }
                hEndOrient.Add(el.get_Parameter(BuiltInParameter.REBAR_ELEM_HOOK_END_ORIENT).AsValueString());
                hStartOrient.Add(el.get_Parameter(BuiltInParameter.REBAR_ELEM_HOOK_START_ORIENT).AsValueString());
            }

            return new Dictionary<string, object>
            {
                { "rebarStyle", rStyle},
                { "rebarBarType", rType},
                { "hookStartType", hStartType},
                { "hookEndType", hEndType},
                { "hookStartOrientation", hStartOrient},
                { "hookEndOrientation", hEndOrient},
                //{ "Message", message },
            };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rebarBarType"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "rebarDiameter" })]
        public static Dictionary<string, object> getRebarDiameter(List<Revit.Elements.Element> rebarBarType)
        {
            string message = "";
            Document doc = DocumentManager.Instance.CurrentDBDocument;
            DynaFunctions f = new DynaFunctions();
            //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
            //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;
            List<double> rDiameters = new List<double>();

            foreach (Revit.Elements.Element rt in rebarBarType)
            {
                Autodesk.Revit.DB.Element el = doc.GetElement(rt.UniqueId.ToString());
                rDiameters.Add(f.feetToMillimeter(el.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble()));
            }

            return new Dictionary<string, object>
            {
                { "rebarDiameter", rDiameters},
                //{ "Message", message },
            };

        }

        [MultiReturn(new[] { "polyCurves" })]
        public static Dictionary<string, object> getRebarCenterLineCurve(
            List<Revit.Elements.Element> rebar,
            bool adjustForSelfIntersection = false,
            bool suppressHooks = true,
            bool suppressBendRadius = true,
            bool multiplanarOption = true
            )
        {
            string message = "";
            Document doc = DocumentManager.Instance.CurrentDBDocument;
            DynaFunctions f = new DynaFunctions();
            //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
            //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;
            List<Autodesk.DesignScript.Geometry.PolyCurve> curves = new List<Autodesk.DesignScript.Geometry.PolyCurve>();
            MultiplanarOption mp = MultiplanarOption.IncludeOnlyPlanarCurves;

            foreach (Revit.Elements.Element r in rebar)
            {
                switch (multiplanarOption)
                {
                    case true: mp = MultiplanarOption.IncludeOnlyPlanarCurves; break;
                    case false: mp = MultiplanarOption.IncludeAllMultiplanarCurves; break;

                }
                Autodesk.Revit.DB.Element el = doc.GetElement(r.UniqueId.ToString());
                Rebar reb = el as Rebar;
                IList<Curve> sketch = reb.GetCenterlineCurves(adjustForSelfIntersection, suppressHooks, suppressBendRadius, mp, 0);
                List<Autodesk.DesignScript.Geometry.Curve> crv = new List<Autodesk.DesignScript.Geometry.Curve>();
                foreach (Curve s in sketch)
                {
                    Autodesk.DesignScript.Geometry.Curve c = Revit.GeometryConversion.RevitToProtoCurve.ToProtoType(s, true);
                    crv.Add(c);
                }
                Autodesk.DesignScript.Geometry.PolyCurve pc = Autodesk.DesignScript.Geometry.PolyCurve.ByJoinedCurves(crv);
                curves.Add(pc);

            }

            return new Dictionary<string, object>
            {
                { "polyCurves", curves},
                //{ "Message", message },
            };

        }



    }

    public static class Layout
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rebars">List of rebars element</param>
        /// <param name="distributionType">true = Varying Lenght /n false = Uniform</param>
        /// <returns></returns>
        [MultiReturn(new[] { "rebars" })]
        public static Dictionary<string, object> setRebarDistributionType(List<Revit.Elements.Element> rebars, bool distributionType = false)
        {
            string message = "";
            Document doc = DocumentManager.Instance.CurrentDBDocument;
            DynaFunctions f = new DynaFunctions();
            //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
            //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;
            List<double> rDiameters = new List<double>();

            Transaction tx = new Transaction(doc, "rebars");

            DistributionType dt = DistributionType.Uniform;
            switch (distributionType)
            {
                case true: dt = DistributionType.VaryingLength; break;
                case false: dt = DistributionType.Uniform; break;
            }
            foreach (Revit.Elements.Element r in rebars)
            {
                Autodesk.Revit.DB.Element el = doc.GetElement(r.UniqueId.ToString());
                Rebar reb = el as Rebar;
                try
                {
                    tx.Start("rebars");
                    reb.DistributionType = dt;
                    tx.Commit();
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }

            }

            return new Dictionary<string, object>
            {
                { "rebarDiameter", rDiameters},
                { "Message", message },
            };
        }


    }

}
