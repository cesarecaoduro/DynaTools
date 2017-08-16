using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.DesignScript.Runtime;
using Dynamo.Graph.Nodes;
using RevitServices.Persistence;
using Revit.Elements;
using DynaToolsFunctions;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.Creation;


using DSRevitNodesUI;

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
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
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
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
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
        public static Dictionary<string, object> ElementByCategoryFromDocument(Autodesk.Revit.DB.Document doc, Revit.Elements.Category category, Boolean refresh = false)
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
        public static Dictionary<string, object> ElementParametersByCategoryFromDocument(Autodesk.Revit.DB.Document doc, Revit.Elements.Category category, List<string> parameters = null, Boolean refresh = false)
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

    /// <summary>
    /// 
    /// </summary>
    public static class Parameters
    {
        [MultiReturn(new[] { "found", "notFound"})]
        public static Dictionary<string, object> CheckIfParameterExist(List<String> parameters)
        {
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
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
        
    }

    /// <summary>
    /// 
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="endLevel"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "elements" })]
        public static Dictionary<string, object> ChangeFittingsLevel(List<Revit.Elements.Element> elements, Revit.Elements.Level endLevel)
        {
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements">List of Ducts</param>
        /// <param name="ductInsulationType">Select the insulation type</param>
        /// <param name="insulationThickness">In millimiters</param>
        /// <returns></returns>
        [MultiReturn(new[] { "elements" })]
        public static Dictionary<string, object> CreateDuctInsulation(List<Revit.Elements.Element> elements, Revit.Elements.Element ductInsulationType, double insulationThickness)
        {
            string result = "";
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            ElementId dITId = doc.GetElement(ductInsulationType.UniqueId.ToString()).Id;
            insulationThickness *= 0.00328084;

            try
            {
                foreach (Revit.Elements.Element e in elements)
                {
                    ElementId elId = doc.GetElement(e.UniqueId.ToString()).Id;
                    DuctInsulation.Create(doc, elId, dITId, insulationThickness);
                }
                result = "Executed";

            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return new Dictionary<string, object>
            {
                {"elements", elements},
                { "result", result}
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements">List of Pipes</param>
        /// <param name="pipeInsulationType">Select the insulation type</param>
        /// <param name="insulationThickness">In millimiters</param>
        /// <returns></returns>
        [NodeName("Create pipe insulation")]
        [NodeDescription("CreatePipeInsulation")]
        [MultiReturn(new[] { "elements" })]
        public static Dictionary<string, object> CreatePipeInsulation(List<Revit.Elements.Element> elements, Revit.Elements.Element pipeInsulationType, double insulationThickness)
        {
            string result = "";
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            ElementId dITId = doc.GetElement(pipeInsulationType.UniqueId.ToString()).Id;
            insulationThickness *= 0.00328084;

            try
            {
                foreach (Revit.Elements.Element e in elements)
                {
                    ElementId elId = doc.GetElement(e.UniqueId.ToString()).Id;
                    PipeInsulation.Create(doc, elId, dITId, insulationThickness);
                }
                result = "Executed";

            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return new Dictionary<string, object>
            {
                {"elements", elements},
                { "result", result}
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "elements" })]
        public static Dictionary<string, object> SplitPipeByPoints(Revit.Elements.Element element, List<Autodesk.DesignScript.Geometry.Point> points)
        {
            string result = "";
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            List<Revit.Elements.Element> splitPipes = new List<Revit.Elements.Element>();
            ElementId eId = doc.GetElement(element.UniqueId.ToString()).Id;
            Units units = doc.GetUnits();
            FormatOptions fo =  units.GetFormatOptions(UnitType.UT_Length);

            try
            {
                foreach (Autodesk.DesignScript.Geometry.Point p in points)
                {
                    double pX = UnitUtils.ConvertToInternalUnits(p.X, fo.DisplayUnits);
                    double pY = UnitUtils.ConvertToInternalUnits(p.Y, fo.DisplayUnits);
                    double pZ = UnitUtils.ConvertToInternalUnits(p.Z, fo.DisplayUnits);
                    XYZ ptBreak = new XYZ(pX, pY, pZ);
                    ElementId pieceOfPipe = PlumbingUtils.BreakCurve(doc, eId, ptBreak);
                    splitPipes.Add(doc.GetElement(pieceOfPipe).ToDSType(true));
                }   
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return new Dictionary<string, object>
            {
                {"elements", splitPipes},
                { "result", result}
            };
        }

        [MultiReturn(new[] { "elements" })]
        public static Dictionary<string, object> SplitDuctByPoints(Revit.Elements.Element element, List<Autodesk.DesignScript.Geometry.Point> points)
        {
            string result = "";
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            List<Revit.Elements.Element> splitDucts = new List<Revit.Elements.Element>();
            ElementId eId = doc.GetElement(element.UniqueId.ToString()).Id;
            Units units = doc.GetUnits();
            FormatOptions fo = units.GetFormatOptions(UnitType.UT_Length);

            try
            {
                foreach (Autodesk.DesignScript.Geometry.Point p in points)
                {
                    double pX = UnitUtils.ConvertToInternalUnits(p.X, fo.DisplayUnits);
                    double pY = UnitUtils.ConvertToInternalUnits(p.Y, fo.DisplayUnits);
                    double pZ = UnitUtils.ConvertToInternalUnits(p.Z, fo.DisplayUnits);
                    XYZ ptBreak = new XYZ(pX, pY, pZ);
                    ElementId pieceOfPipe = MechanicalUtils.BreakCurve(doc, eId, ptBreak);
                    splitDucts.Add(doc.GetElement(pieceOfPipe).ToDSType(true));
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return new Dictionary<string, object>
            {
                {"elements", splitDucts},
                { "result", result}
            };
        }

        [MultiReturn(new[] { "ENZ" })]
        public static Dictionary<string, object> getTrueCoordinates(Revit.Elements.Element elements)
        {
            string message = "";


            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;

            Units units = doc.GetUnits();
            FormatOptions fo = units.GetFormatOptions(UnitType.UT_Length);
            List<double> output = new List<double>();

            //Document openedDoc = null;
            //UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
            //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            //UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;
            ProjectLocation pr = doc.ActiveProjectLocation;
            Transform tr = pr.GetTotalTransform().Inverse;

            try
            {
                Autodesk.Revit.DB.Element e = doc.GetElement(elements.UniqueId.ToString());
                Autodesk.Revit.DB.FamilyInstance fi = e as Autodesk.Revit.DB.FamilyInstance;

                LocationPoint loc = fi.Location as LocationPoint;

                XYZ p = tr.OfPoint(loc.Point);

                double x = UnitUtils.ConvertFromInternalUnits(p.X, fo.DisplayUnits);
                double y = UnitUtils.ConvertFromInternalUnits(p.Y, fo.DisplayUnits);
                double z = UnitUtils.ConvertFromInternalUnits(p.Z, fo.DisplayUnits);

                output.AddRange(new List<Double> {x, y, z});
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return new Dictionary<string, object>
            {
                {"ENZ", output},
            };

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class Annotations
    {
        private static object spotCoordinate;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="endLevel"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "elements" })]
        public static Dictionary<string, object> SpotCoordinatesByView(Revit.Elements.Element element, Revit.Elements.Views.View view, bool hasLeader = true, double hOffset = -10, double vOffset = 0)
        {
            Autodesk.Revit.DB.Document doc = DocumentManager.Instance.CurrentDBDocument;
            string result = "";

            try
            {
                Autodesk.Revit.DB.Element e = doc.GetElement(element.UniqueId);
                Autodesk.Revit.DB.View v = doc.GetElement(view.UniqueId) as View;

                Autodesk.Revit.DB.FamilyInstance fi = e as Autodesk.Revit.DB.FamilyInstance;
                Options opt = new Options();
                opt.ComputeReferences = true;
                opt.IncludeNonVisibleObjects = true;

                IList<Reference> rr = fi.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);
                foreach (Reference r in rr)
                {
                }
                LocationPoint loc = e.Location as LocationPoint;

                //XYZ origin = new XYZ(0, 0, 0);
                XYZ rf = loc.Point;
                XYZ bend = new XYZ(0, 10, 0);
                XYZ end = new XYZ(0, 10, 0);

                SpotDimension sp = doc.Create.NewSpotCoordinate(v, rr[0], rf, bend, end, rf, hasLeader);
                spotCoordinate = sp.ToDSType(true);

                result = "Executed";
            }
            catch (Exception ex)
            {
                result = "Not executed: " + ex.Message;
            }

            return new Dictionary<string, object>
            {
                {"elements", spotCoordinate},
                { "result", result}
            };
        }

    }

}
