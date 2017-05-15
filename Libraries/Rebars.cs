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
    public static class Rebars
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elList"></param>
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

        
    }

   

   

}
