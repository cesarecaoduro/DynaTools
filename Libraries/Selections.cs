using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Revit.Elements;
using RevitServices.Persistence;
using System;
using System.Collections.Generic;

namespace SelectionTools
{
    /// <summary>
    /// 
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public class CategorySelectionFilter : ISelectionFilter
    {
        static string category;
        public CategorySelectionFilter(string cat)
        {
            category = cat;
        }

        public bool AllowElement(Autodesk.Revit.DB.Element element)
        {
            if (element.Category.Name == category)
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }


    public static class Selections
    {

        [MultiReturn(new[] {"elements" })]
        public static Dictionary<string, object> ActiveSelection(Boolean refresh = false)
        {
            string message = "Select somehthing";
            List<Revit.Elements.Element> elements = new List<Revit.Elements.Element>();
            if (refresh)
            {

                try
                {
                    Document doc = DocumentManager.Instance.CurrentDBDocument;
                    UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
                    Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
                    UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

                    // Get the element selection of current document.
                    Selection selection = uidoc.Selection;
                    ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();

                    if (0 == selectedIds.Count)
                    {
                        // If no elements selected.
                        message = "You haven't selected any elements";
                    }
                    else
                    {
                        foreach (ElementId id in selectedIds)
                        {
                            elements.Add(doc.GetElement(id).ToDSType(true));
                        }
                        message = "Executed";
                    }
                }
                catch (Exception e)
                {
                    message = e.Message;
                } 
            }
            return new Dictionary<string, object>
                {
                    { "elements", elements},
                    { "message", message},
                };
        }

        [MultiReturn(new[] {"elements" })]
        public static Dictionary<string, object> ActiveSelectionOfCategory(Boolean refresh = false, Revit.Elements.Category category = null)
        {
            string message = "Select something";
            List<Revit.Elements.Element> elements = new List<Revit.Elements.Element>();
            if (refresh)
            {
                try
                {
                    Document doc = DocumentManager.Instance.CurrentDBDocument;
                    UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
                    Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
                    UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

                    // Get the element selection of current document.
                    Selection selection = uidoc.Selection;
                    ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();
                    BuiltInCategory myCatEnum = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), category.Id.ToString());

                    if (0 == selectedIds.Count)
                    {
                        // If no elements selected.
                        message = "You haven't selected any elements.";
                    }
                    else
                    {
                        foreach (ElementId id in selectedIds)
                        {
                            if (doc.GetElement(id).Category.Name == category.Name)
                            {
                                elements.Add(doc.GetElement(id).ToDSType(true));
                            }
                        }
                        if (elements.Count > 0)
                        {
                            message = "Executed";
                        }
                        else
                        {
                            message = "No elements of selected category";
                        }
                    }
                }
                catch (Exception e)
                {
                    message = e.Message;
                }
            }
            return new Dictionary<string, object>
                { 
                    { "elements", elements},
                    { "message", message},
                };
        }

        [MultiReturn(new[] {"elements" })]
        public static Dictionary<string, object> SelectFromViewport(Boolean refresh = false)
        {
            string message = "Select something";
            List<Revit.Elements.Element> elements = new List<Revit.Elements.Element>();
            if (refresh)
            {
                try
                {
                    Document doc = DocumentManager.Instance.CurrentDBDocument;
                    UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
                    Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
                    UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

                    // Get the element selection of current document.
                    Selection selection = uidoc.Selection;

                    IList<Reference> selectedElements = selection.PickObjects(ObjectType.Element);

                    if (0 == selectedElements.Count)
                    {
                        // If no elements selected.
                        message = "You haven't selected any elements.";
                    }
                    else
                    {
                        foreach (Reference e in selectedElements)
                        {
                            elements.Add(doc.GetElement(e).ToDSType(true));
                        }
                        message = "Executed";
                    }
                }
                catch (Exception e)
                {
                    message = e.Message;
                }
            }
            return new Dictionary<string, object>
                {
                    { "elements", elements},
                    { "message", message},
                };
        }

        [MultiReturn(new[] {"elements" })]
        public static Dictionary<string, object> SelectFromViewportOfCategory(Boolean refresh = false, Revit.Elements.Category category = null)
        {
            string message = "Select something";
            List<Revit.Elements.Element> elements = new List<Revit.Elements.Element>();
            if (refresh)
            {
                try
                {
                    Document doc = DocumentManager.Instance.CurrentDBDocument;
                    UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
                    Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
                    UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

                    // Get the element selection of current document.
                    Selection selection = uidoc.Selection;
                    ISelectionFilter categoryFitler = new CategorySelectionFilter(category.Name);

                    IList<Reference> selectedElements = selection.PickObjects(ObjectType.Element, categoryFitler);

                    if (0 == selectedElements.Count)
                    {
                        // If no elements selected.
                        message = "You haven't selected any elements.";
                    }
                    else
                    {
                        foreach (Reference e in selectedElements)
                        {
                            elements.Add(doc.GetElement(e).ToDSType(true));
                        }
                        message = "Executed";
                    }
                }
                catch (Exception e)
                {
                    message = e.Message;
                }
            }
            return new Dictionary<string, object>
                {                   
                    { "elements", elements},
                    { "message", message},
                };
        }

        [MultiReturn(new[] {"elements" })]
        public static Dictionary<string, object> SelectFromViewportRetainOrder(Boolean refresh = false)
        {
            string message = "Select something";
            List<Revit.Elements.Element> elements = new List<Revit.Elements.Element>();
            if (refresh)
            {
                try
                {
                    Document doc = DocumentManager.Instance.CurrentDBDocument;
                    UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
                    Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
                    UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

                    // Get the element selection of current document.
                    Selection selection = uidoc.Selection;

                    List<Reference> selectedElements = new List<Reference>();

                    bool flag = true;
                    while (flag)
                    {
                        try
                        {
                            Reference reference = uidoc.Selection.PickObject(ObjectType.Element, "Pick elements in the desired order and hit ESC to stop picking.");
                            selectedElements.Add(reference);
                        }
                        catch
                        {
                            flag = false;
                        }
                    }

                    if (0 == selectedElements.Count)
                    {
                        // If no elements selected.
                        message = "You haven't selected any elements.";
                    }
                    else
                    {
                        foreach (Reference e in selectedElements)
                        {
                            elements.Add(doc.GetElement(e).ToDSType(true));
                        }
                        message = "Executed";
                    }
                }
                catch (Exception e)
                {
                    message = e.Message;
                }
            }
            return new Dictionary<string, object>
                {
                    { "elements", elements},
                    { "message", message},
                };
        }

        [MultiReturn(new[] {"elements" })]
        public static Dictionary<string, object> SelectFromViewportOfCategoryRetainOrder(Boolean refresh = false, Revit.Elements.Category category = null)
        {
            string message = "Select something";
            List<Revit.Elements.Element> elements = new List<Revit.Elements.Element>();
            if (refresh)
            {
                try
                {
                    Document doc = DocumentManager.Instance.CurrentDBDocument;
                    UIApplication uiapp = DocumentManager.Instance.CurrentUIApplication;
                    Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
                    UIDocument uidoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;

                    // Get the element selection of current document.
                    Selection selection = uidoc.Selection;

                    List<Reference> selectedElements = new List<Reference>();
                    ISelectionFilter categoryFitler = new CategorySelectionFilter(category.Name);

                    bool flag = true;
                    while (flag)
                    {
                        try
                        {
                            Reference reference = uidoc.Selection.PickObject(ObjectType.Element, categoryFitler, "Pick elements in the desired order and hit ESC to stop picking.");
                            selectedElements.Add(reference);
                        }
                        catch
                        {
                            flag = false;
                        }
                    }

                    if (0 == selectedElements.Count)
                    {
                        // If no elements selected.
                        message = "You haven't selected any elements.";
                    }
                    else
                    {
                        foreach (Reference e in selectedElements)
                        {
                            elements.Add(doc.GetElement(e).ToDSType(true));
                        }
                        message = "Executed";
                    }
                }
                catch (Exception e)
                {
                    message = e.Message;
                }
            }
            return new Dictionary<string, object>
                {
                    { "elements", elements},
                    { "message", message},
                };
        }

    }
}
