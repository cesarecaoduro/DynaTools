using Autodesk.DesignScript.Runtime;
using ADG = Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DynaToolsFunctions;
using Revit.Elements;
using RevitServices.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewTools
{
    /// <summary>
    /// 
    /// </summary>
    public static class Views
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MultiReturn(new[] { "Viewports", "CenterPoints", "MinMaxPoints", "Outlines"})]
        public static Dictionary<string, object> ViewportsCollector()
        {
            Document doc = DocumentManager.Instance.CurrentDBDocument;
            //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
            //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

            List<Revit.Elements.Element> lstViewports = new List<Revit.Elements.Element>();
            List<ADG.Point> centerViewports = new List<ADG.Point>();
            List<List<ADG.Point>> pointsViewports = new List<List<ADG.Point>>();
            List<ADG.Rectangle> outlines = new List<ADG.Rectangle>();

            FilteredElementCollector col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Viewports).WhereElementIsNotElementType();
            foreach (Viewport c in col)
            {
                List<ADG.Point> mmP = new List<ADG.Point>();
                Revit.Elements.Element v = c.ToDSType(true);
                XYZ center = c.GetBoxCenter();
                ADG.Point cc = ADG.Point.ByCoordinates(center.X, center.Y);
                Outline o = c.GetBoxOutline();

                XYZ minPoint = o.MinimumPoint;
                XYZ maxPoint = o.MaximumPoint;

                List<ADG.Point> cP = new List<ADG.Point>();
                cP.Add(ADG.Point.ByCoordinates(minPoint.X, minPoint.Y));
                cP.Add(ADG.Point.ByCoordinates(minPoint.X, maxPoint.Y));
                cP.Add(ADG.Point.ByCoordinates(maxPoint.X, maxPoint.Y));
                cP.Add(ADG.Point.ByCoordinates(maxPoint.X, minPoint.Y));

                mmP.Add(ADG.Point.ByCoordinates(minPoint.X, minPoint.Y));
                mmP.Add(ADG.Point.ByCoordinates(maxPoint.X, maxPoint.Y));

                ADG.Rectangle rec =  ADG.Rectangle.ByCornerPoints(cP);

                lstViewports.Add(v);
                centerViewports.Add(cc);
                pointsViewports.Add(mmP);
                outlines.Add(rec);
            }

            return new Dictionary<string, object>
            {
                {"Viewports", lstViewports},
                {"CenterPoints", centerViewports},
                {"MinMaxPoints", pointsViewports},
                {"Outlines", outlines}
            };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class Sheets
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewNames"></param>
        /// <param name="viewNumbers"></param>
        /// <param name="titleBlock"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "Sheets" })]
        public static Dictionary<string, object> CreateSheets(List<string> sheetNames, List<string> sheetNumbers, Revit.Elements.Element titleBlock)
        {
            Document doc = DocumentManager.Instance.CurrentDBDocument;
            string message = "";
            List<Revit.Elements.Element> sheetsList = new List<Revit.Elements.Element>();
            ElementId titleBlockId = doc.GetElement(titleBlock.UniqueId.ToString()).Id;

            try
            {
                int k = 0;
                foreach (string vN in sheetNames)
                {
                    ViewSheet s = ViewSheet.Create(doc, titleBlockId);
                    s.Name = vN;
                    s.SheetNumber = sheetNumbers[k];
                    k++;
                    sheetsList.Add(s.ToDSType(true));
                }

                message = "Executed";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return new Dictionary<string, object>
            {
                {"Sheets", sheetsList},
                {"message", message},
            };

        }
    }
}
