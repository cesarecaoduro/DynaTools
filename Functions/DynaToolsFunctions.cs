
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Controls;
using Revit.AnalysisDisplay;

namespace DynaToolsFunctions
{
    class DynaFunctions
    {
        [IsVisibleInDynamoLibrary(false)]
        public double feetToMillimeter(double n)
        {
            return n * 304.8;
        }

        [IsVisibleInDynamoLibrary(false)]
        public double sqfToSqm(double n)
        {
            return n * 0.092903;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public double paramAsDouble(Element e, BuiltInParameter paramName)
        {
            return e.get_Parameter(paramName).AsDouble();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public string paramAsValueString(Element e, BuiltInParameter paramName)
        {
            return e.get_Parameter(paramName).AsValueString();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public string paramAsString(Element e, BuiltInParameter paramName)
        {
            return e.get_Parameter(paramName).AsString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridView"></param>
        /// <param name="paramName"></param>
        [IsVisibleInDynamoLibrary(false)]
        public void dataGridCreateColumn(GridView gridView, string paramName)
        {
            gridView.Columns.Add(new GridViewColumn
            {
                Header = paramName,
                DisplayMemberBinding = new System.Windows.Data.Binding(paramName)
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public string executeSqlCommand(string commandString, string connectionString)
        {
            string result = "";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(commandString, conn);
                    conn.Open();
                    command.ExecuteNonQuery();
                    result = "Executed";
                }
                catch (SqlException ex)
                {
                    result = ex.Message;
                }
                conn.Close();
            }
            return result;

        }

        [IsVisibleInDynamoLibrary(false)]
        public double convertToUnit(double value, DisplayUnitType dt)
        {
            value = UnitUtils.ConvertFromInternalUnits(value, dt);
            return value;
        }

        public Autodesk.Revit.DB.Solid CreateSolidFromBoundingBox(Autodesk.Revit.DB.Solid inputSolid)
        {
            BoundingBoxXYZ bbox = inputSolid.GetBoundingBox();

            // Corners in BBox coords

            XYZ pt0 = new XYZ(bbox.Min.X, bbox.Min.Y, bbox.Min.Z);
            XYZ pt1 = new XYZ(bbox.Max.X, bbox.Min.Y, bbox.Min.Z);
            XYZ pt2 = new XYZ(bbox.Max.X, bbox.Max.Y, bbox.Min.Z);
            XYZ pt3 = new XYZ(bbox.Min.X, bbox.Max.Y, bbox.Min.Z);

            // Edges in BBox coords

            Autodesk.Revit.DB.Line edge0 = Autodesk.Revit.DB.Line.CreateBound(pt0, pt1);
            Autodesk.Revit.DB.Line edge1 = Autodesk.Revit.DB.Line.CreateBound(pt1, pt2);
            Autodesk.Revit.DB.Line edge2 = Autodesk.Revit.DB.Line.CreateBound(pt2, pt3);
            Autodesk.Revit.DB.Line edge3 = Autodesk.Revit.DB.Line.CreateBound(pt3, pt0);

            // Create loop, still in BBox coords

            List<Autodesk.Revit.DB.Curve> edges = new List<Autodesk.Revit.DB.Curve>();
            edges.Add(edge0);
            edges.Add(edge1);
            edges.Add(edge2);
            edges.Add(edge3);

            double height = bbox.Max.Z - bbox.Min.Z;

            CurveLoop baseLoop = CurveLoop.Create(edges);

            List<CurveLoop> loopList = new List<CurveLoop>();
            loopList.Add(baseLoop);

            Autodesk.Revit.DB.Solid preTransformBox = GeometryCreationUtilities
              .CreateExtrusionGeometry(loopList, XYZ.BasisZ,
                height);
            

            Autodesk.Revit.DB.Solid transformBox = SolidUtils.CreateTransformed(
              preTransformBox, bbox.Transform);


            return preTransformBox;
        }



    }
}
