using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Dynamo.Graph.Nodes;
using DynaToolsFunctions;
using Revit.Elements;
using RevitServices.Persistence;
using System;
using System.Collections.Generic;

namespace CoordinationTools
{
    /// <summary>
    /// Collection of tools to get quantities of elements and useful parameters values
    /// </summary>
    public abstract class Coordinators
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="refresh"></param>
        /// <returns></returns>
        [IsDesignScriptCompatible]
        [MultiReturn(new[] { "Elements", "Intersections" })]
        public static Dictionary<string, object> IntersectionByElementCategory(Revit.Elements.Category category, bool refresh = false)
        {
            List<Revit.Elements.Element> elList = new List<Revit.Elements.Element>();
            List<List<Revit.Elements.Element>> inList = new List<List<Revit.Elements.Element>>();

            if (refresh)
            {
                BuiltInCategory myCatEnum = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), category.Id.ToString());
                Document doc = DocumentManager.Instance.CurrentDBDocument;
                //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
                //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
                //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

                ElementCategoryFilter filter = new ElementCategoryFilter(myCatEnum);
                FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(filter).WhereElementIsNotElementType();

                foreach (Autodesk.Revit.DB.Element e in collector)
                {

                    List<Revit.Elements.Element> intersList = new List<Revit.Elements.Element>();
                    DynaFunctions f = new DynaFunctions();
                    ElementIntersectsElementFilter interFiler = new ElementIntersectsElementFilter(e);
                    FilteredElementCollector interElem = new FilteredElementCollector(doc).WherePasses(interFiler).WhereElementIsNotElementType();
                    if (interElem.GetElementCount() > 0)
                    {
                        elList.Add(doc.GetElement(e.Id).ToDSType(true));
                        foreach (Autodesk.Revit.DB.Element el in interElem)
                        {
                            intersList.Add(doc.GetElement(el.Id).ToDSType(true));
                        }
                        inList.Add(intersList);
                    }

                }

            }
            return new Dictionary<string, object>
            {
                { "Elements", elList},
                { "Intersections", inList},
            };


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="document"></param>
        /// <param name="refresh"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "Elements", "Intersections" })]
        public static Dictionary<string, object> IntersectionByElementCategoryFromLink(Revit.Elements.Category category, Document document = null, bool refresh = false)
        {
            List<Revit.Elements.Element> elList = new List<Revit.Elements.Element>();
            List<List<Revit.Elements.Element>> inList = new List<List<Revit.Elements.Element>>();
            string executed = "";

            if (refresh)
            {
                BuiltInCategory myCatEnum = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), category.Id.ToString(), true);
                Document doc = DocumentManager.Instance.CurrentDBDocument;
                //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
                //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
                //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

                ElementCategoryFilter filter = new ElementCategoryFilter(myCatEnum);
                FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(filter).WhereElementIsNotElementType();

                foreach (Autodesk.Revit.DB.Element e in collector)
                {
                    List<Revit.Elements.Element> intersList = new List<Revit.Elements.Element>();
                    DynaFunctions f = new DynaFunctions();

                    ElementIntersectsElementFilter interFiler = new ElementIntersectsElementFilter(e);
                    FilteredElementCollector interElem = new FilteredElementCollector(document).WherePasses(interFiler).WhereElementIsNotElementType();
                    if (interElem.GetElementCount() > 0)
                    {
                        elList.Add(doc.GetElement(e.Id).ToDSType(true));
                        foreach (Autodesk.Revit.DB.Element el in interElem)
                        {
                            intersList.Add(document.GetElement(el.Id).ToDSType(true));
                        }
                        inList.Add(intersList);
                    }

                }

            }
            return new Dictionary<string, object>
            {
                { "Elements", elList},
                { "Intersections", inList},
                { "Executed", executed }
            };


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="refresh"></param>
        /// <returns></returns>
        public static List<List<Autodesk.DesignScript.Geometry.Point>> IntersectionPointsByCategory(Revit.Elements.Category category, Document document = null,bool refresh = false)
        {
            List<Revit.Elements.Element> elList = new List<Revit.Elements.Element>();
            List<List<Revit.Elements.Element>> inList = new List<List<Revit.Elements.Element>>();

            List<List<Autodesk.DesignScript.Geometry.Point>> cPoints = new List<List<Autodesk.DesignScript.Geometry.Point>>();

            if (refresh)
            {
                BuiltInCategory myCatEnum = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), category.Id.ToString(), true);
                Document doc = DocumentManager.Instance.CurrentDBDocument;

                //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
                //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
                //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

                ElementCategoryFilter filter = new ElementCategoryFilter(myCatEnum);
                ElementCategoryFilter exclude = new ElementCategoryFilter(BuiltInCategory.OST_GenericModel);
                FilteredElementCollector excluded = new FilteredElementCollector(doc).WherePasses(exclude);
                FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(filter).WhereElementIsNotElementType();
                collector.Excluding(excluded.ToElementIds());

                foreach (Autodesk.Revit.DB.Element e in collector)
                {
                    DynaFunctions f = new DynaFunctions();


                    ElementIntersectsElementFilter interFiler = new ElementIntersectsElementFilter(e);
                    FilteredElementCollector interElem = new FilteredElementCollector(document).WherePasses(interFiler).WhereElementIsNotElementType();
                    if (interElem.GetElementCount() > 0)
                    {
                        List<Autodesk.Revit.DB.Solid> elGeoms = new List<Autodesk.Revit.DB.Solid>();
                        GeometryElement geomEl = e.get_Geometry(new Options());
                        foreach (GeometryObject geomObj in geomEl)
                        {
                            elGeoms.Add(geomObj as Autodesk.Revit.DB.Solid);
                        }

                        elList.Add(doc.GetElement(e.Id).ToDSType(true));
                        List<Autodesk.Revit.DB.Solid> iS = new List<Autodesk.Revit.DB.Solid>();
                        List<Autodesk.DesignScript.Geometry.Point> cPoint = new List<Autodesk.DesignScript.Geometry.Point>();
                        List<Autodesk.DesignScript.Geometry.Solid> iSS = new List<Autodesk.DesignScript.Geometry.Solid>();
                        foreach (Autodesk.Revit.DB.Element el in interElem)
                        {
                            GeometryElement intEl = el.get_Geometry(new Options());
                            foreach (GeometryObject intObj in intEl)
                            {
                                iS.Add(intObj as Autodesk.Revit.DB.Solid);
                            }
                        }
                        foreach (Autodesk.Revit.DB.Solid s0 in elGeoms)
                        {
                            foreach (Autodesk.Revit.DB.Solid s1 in iS)
                            {
                                Autodesk.Revit.DB.Solid i = BooleanOperationsUtils.ExecuteBooleanOperation(s0, s1, BooleanOperationsType.Intersect);
                                if (i != null)
                                {
                                    iSS.Add(Revit.GeometryConversion.RevitToProtoSolid.ToProtoType(i));
                                    DisplayUnitType dt = doc.GetUnits().GetFormatOptions(UnitType.UT_Length).DisplayUnits;

                                    XYZ coord = new XYZ(f.convertToUnit(i.ComputeCentroid().X, dt), f.convertToUnit(i.ComputeCentroid().Y, dt), f.convertToUnit(i.ComputeCentroid().Z, dt));
                                    //XYZ coord = new XYZ(i.ComputeCentroid().X, i.ComputeCentroid().Y, i.ComputeCentroid().Z);
                                    Autodesk.DesignScript.Geometry.Point p = Autodesk.DesignScript.Geometry.Point.ByCoordinates(coord.X, coord.Y, coord.Z);
                                    cPoint.Add(p);

                                }
                            }

                        }
                        cPoints.Add(cPoint);

                    }

                }

            }
            return cPoints;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="refresh"></param>
        /// <returns></returns>
        public static List<List<Autodesk.DesignScript.Geometry.Solid>> IntersectionSolidsByCategory(Revit.Elements.Category category, Document document = null, bool refresh = false)
        {
            List<Revit.Elements.Element> elList = new List<Revit.Elements.Element>();
            List<List<Revit.Elements.Element>> inList = new List<List<Revit.Elements.Element>>();

            List<List<Autodesk.DesignScript.Geometry.Solid>> interGeoms = new List<List<Autodesk.DesignScript.Geometry.Solid>>();


            if (refresh)
            {
                BuiltInCategory myCatEnum = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), category.Id.ToString(), true);
                Document doc = DocumentManager.Instance.CurrentDBDocument;

                //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
                //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
                //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

                ElementCategoryFilter filter = new ElementCategoryFilter(myCatEnum);
                ElementCategoryFilter exclude = new ElementCategoryFilter(BuiltInCategory.OST_GenericModel);
                FilteredElementCollector excluded = new FilteredElementCollector(doc).WherePasses(exclude);
                FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(filter).WhereElementIsNotElementType();
                collector.Excluding(excluded.ToElementIds());
                

                foreach (Autodesk.Revit.DB.Element e in collector)
                {
                    DynaFunctions f = new DynaFunctions();

                    ElementIntersectsElementFilter interFiler = new ElementIntersectsElementFilter(e);
                    FilteredElementCollector interElem = new FilteredElementCollector(document).WherePasses(interFiler).WhereElementIsNotElementType();
                    if (interElem.GetElementCount() > 0)
                    {
                        List<Autodesk.Revit.DB.Solid> elGeoms = new List<Autodesk.Revit.DB.Solid>();
                        GeometryElement geomEl = e.get_Geometry(new Options());
                        foreach (GeometryObject geomObj in geomEl)
                        {
                            elGeoms.Add(geomObj as Autodesk.Revit.DB.Solid);
                        }
                        List<Autodesk.Revit.DB.Solid> iS = new List<Autodesk.Revit.DB.Solid>();
                        List<Autodesk.DesignScript.Geometry.Solid> iSS = new List<Autodesk.DesignScript.Geometry.Solid>();
                        foreach (Autodesk.Revit.DB.Element el in interElem)
                        {
                            GeometryElement intEl = el.get_Geometry(new Options());
                            foreach (GeometryObject intObj in intEl)
                            {
                                iS.Add(intObj as Autodesk.Revit.DB.Solid);
                            }
                        }
                        foreach (Autodesk.Revit.DB.Solid s0 in elGeoms)
                        {
                            foreach (Autodesk.Revit.DB.Solid s1 in iS)
                            {
                                Autodesk.Revit.DB.Solid i = BooleanOperationsUtils.ExecuteBooleanOperation(s0, s1, BooleanOperationsType.Intersect);
                                if (i != null)
                                {
                                    Autodesk.Revit.DB.Solid bbox = f.CreateSolidFromBoundingBox(i);
                                    iSS.Add(Revit.GeometryConversion.RevitToProtoSolid.ToProtoType(i));
                                }
                            }

                        }
                        interGeoms.Add(iSS);
                    }

                }

            }
            return interGeoms;
        }


    }

  

}
