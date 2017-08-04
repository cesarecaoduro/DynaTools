using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using DynaToolsFunctions;
using ProtoCore.AST.AssociativeAST;
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
        [MultiReturn(new[] { "document", "message" })]
        public static Dictionary<string, object> createNavisView(Document document)
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
        [MultiReturn(new[] { "document", "message" })]
        public static Dictionary<string, object> openDocumentBackground(string filePath = null, OpenOptions openOption = null)
        {
            string message = "";
            Document doc = DocumentManager.Instance.CurrentDBDocument;
            Document openedDoc = null;
            UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

            ModelPath path = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
            
            try
            {
                openedDoc = app.OpenDocumentFile(path, openOption);
                message = "Opened";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return new Dictionary<string, object>
            {
                {"document", openedDoc},
                {"message", message }
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
                message = "Closed";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return new Dictionary<string, object>
            {
                {"message", message}
            };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audit"></param>
        /// <param name="detachFromCentralOption">
        /// 0 = ClearTransmittedSaveAsNewCentral,
        /// 1 = DetachAndDiscardWorksets,
        /// 2 = DetachAndPreserveWorksets,
        /// 3 = DoNotDetach
        /// </param>
        /// <returns></returns>
        [MultiReturn(new[] { "openOption"})]
        public static Dictionary<string, object> openDocumentOption(bool audit = false, int detachFromCentralOption = 3)
        {
            string message = "";
            OpenOptions openOpt = new OpenOptions();

            //Document doc = DocumentManager.Instance.CurrentDBDocument;
            //Document openedDoc = null;
            //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
            //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

            //ModelPath path = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
            try
            {

                openOpt.Audit = false;
                WorksetConfiguration workConf = new WorksetConfiguration(WorksetConfigurationOption.OpenAllWorksets);
                openOpt.SetOpenWorksetsConfiguration(workConf);
                switch(detachFromCentralOption)
                {
                    case 0: openOpt.DetachFromCentralOption = DetachFromCentralOption.ClearTransmittedSaveAsNewCentral; break;
                    case 1: openOpt.DetachFromCentralOption = DetachFromCentralOption.DetachAndDiscardWorksets; break;
                    case 2: openOpt.DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets; break;
                    case 3: openOpt.DetachFromCentralOption = DetachFromCentralOption.DoNotDetach; break;
                }
                
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return new Dictionary<string, object>
            {
                {"openOption", openOpt },
            };

        }

    }

    /// <summary>
    /// 
    /// </summary>
    

}
