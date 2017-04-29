using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DynaToolsFunctions;
using Revit.Elements;
using RevitServices.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentTools
{
    /// <summary>
    /// 
    /// </summary>
    public static class Documents
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MultiReturn(new[] { "activeDocument" })]
        public static Dictionary<string, object> activeDocument()
        {
            Document doc = DocumentManager.Instance.CurrentDBDocument;

            return new Dictionary<string, object>
            {
                { "activeDocument", doc},
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MultiReturn(new[] { "documents", "documentNames", "documentPaths" })]
        public static Dictionary<string, object> linkedDocuments(bool refresh = false)
        {
            List<Document> docList = new List<Document>();
            List<string> docNames = new List<string>();
            List<string> docPaths = new List<string>();

            if (refresh)
            {
                Document doc = DocumentManager.Instance.CurrentDBDocument;
                FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(RevitLinkInstance));


                foreach (RevitLinkInstance l in collector)
                {
                    docList.Add(l.GetLinkDocument());
                    docNames.Add(l.Name);
                    docPaths.Add(l.GetLinkDocument().PathName);
                }
            }

            return new Dictionary<string, object>
            {
                { "documents", docList},
                {"documentNames", docNames},
                {"documentPaths", docPaths},
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="detach"></param>
        /// <param name="audit"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "document" })]
        public static Dictionary<string, object> createNavisViewBackground(Document document)
        {
            string message = "";

            //Document doc = DocumentManager.Instance.CurrentDBDocument;
            //Document openedDoc = null;
            //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
            //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

            //ModelPath path = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
            //OpenOptions openOpt = new OpenOptions();
            try
            {
                
                //openedDoc = app.OpenDocumentFile(path, openOpt);
                var direction = new XYZ(-1, 1, -1);
                var collector = new FilteredElementCollector(document);
                var viewFamilyType = collector.OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>()
                  .FirstOrDefault(x => x.ViewFamily == ViewFamily.ThreeDimensional);

                using (Transaction ttNew = new Transaction(document, "abc"))
                {
                    ttNew.Start();
                    View3D navisView = View3D.CreateIsometric(document, viewFamilyType.Id);

                    navisView.SetOrientation(new ViewOrientation3D(direction, new XYZ(0, 1, 1), new XYZ(0, 1, -1)));
                    navisView.ViewName = "Navisworks";
                    ttNew.Commit();
                    message = "Created";
                    //document.Close(true);
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return new Dictionary<string, object>
            {
                {"document", document},
                {"message", message }
            };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "document" })]
        public static Dictionary<string, object> openDocumentBackground(string filePath = null)
        {

            Document doc = DocumentManager.Instance.CurrentDBDocument;
            Document openedDoc = null;
            UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

            ModelPath path = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
            OpenOptions openOpt = new OpenOptions();
            try
            {
                openedDoc = app.OpenDocumentFile(path, openOpt);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return new Dictionary<string, object>
            {
                {"document", openedDoc}
            };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="save"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "message" })]
        public static Dictionary<string, object> closeDocumentBackground(Document document, bool save = true)
        {
            string message = "";

            //Document doc = DocumentManager.Instance.CurrentDBDocument;
            //Document openedDoc = null;
            //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
            //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

            //ModelPath path = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
            //OpenOptions openOpt = new OpenOptions();
            try
            {
                document.Close(save);
                message = "closed";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return new Dictionary<string, object>
            {
                {"message", document}
            };

        }

    }
}
