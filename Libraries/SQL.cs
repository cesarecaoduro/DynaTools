using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.DesignScript.Runtime;
using Dynamo.Graph.Nodes;
using Autodesk.Revit.Attributes;
using RevitServices.Persistence;
using System.Windows.Controls;
using Autodesk.DesignScript.Geometry;
using Revit.Elements;
using System.Xml;
using System.Data.SqlClient;
using CoreNodeModels;
using CoreNodeModelsWpf.Nodes;
using RevitServices.EventHandler;
using Dynamo.Wpf;
using Dynamo.Controls;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using Dynamo.Configuration;
using System.Windows;
using DynaToolsFunctions;

namespace SQLTools
{

    /// <summary>
    /// Collection of nodes to manage SQL Server operations
    /// </summary>
    public static class Connect
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName">SQL server name</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        [MultiReturn(new[] { "message", "connectionString" })]
        public static Dictionary<string, object> Server(String serverName, string username, string password)
        {
            string result = "";
            string connectionString = @"Data Source=.\" + serverName + ";" +                
                @"User id=" + username + ";" +
                @"Password=" + password + ";";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    result = "Connected!";
                    conn.Close();
                }
                catch (SqlException ex)
                {
                    result = ex.Message;
                    connectionString = "";
                }
            }

            return new Dictionary<string, object>
                {
                    { "message", result},
                    { "connectionString", connectionString}
                };
        }

        /// <summary>
        /// Connect to a specific database
        /// </summary>
        /// <param name="serverName">SQL server name</param>
        /// <param name="dbName">Database name</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        [MultiReturn(new[] { "message", "connectionString" })]
        public static Dictionary<string, object> Database(String serverName, string dbName, string username, string password)
        {
            string result = "";
            string connectionString = @"Data Source=.\" + serverName + ";" +
                "Initial Catalog=" + dbName + ";" +
                @"User id=" + username + ";" +
                @"Password=" + password + ";";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    result = "Connected!";
                    conn.Close();
                }
                catch (SqlException ex)
                {
                    result = ex.Message;
                    connectionString = "";
                }
            }

            return new Dictionary<string, object>
                {
                    { "message", result},
                    { "connectionString", connectionString}
                };
        }

    }

    public static class Create
    {
        /// <summary>
        /// Create a new Database
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="dbName">Database name</param>
        /// <returns></returns>
        [MultiReturn(new[] { "message", "dbName" })]
        public static Dictionary<string, object> Database(String connectionString, string dbName)
        {
            DynaFunctions f = new DynaFunctions();
            string commandString = "create database " + dbName;
            string result = f.executeSqlCommand(commandString, connectionString);


            return new Dictionary<string, object>
            {
                { "message", result},
                { "dbName", dbName},
            };
        }

        /// <summary>
        /// Create a table in a specific schema
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="tableName">Table name</param>
        /// <param name="dbSchema">Database schema (default = dbo)</param>
        /// <returns></returns>
        [MultiReturn(new[] { "message", "tableName" })]
        public static Dictionary<string, object> Table(List<String> fieldNames, List<string> fieldTypes, string connectionString, string tableName, string dbSchema = "dbo")
        {
            DynaFunctions f = new DynaFunctions();
            int k = 0;
            string tableFields = "(";
            foreach (string fn in fieldNames)
            {
                tableFields += fn + " " + fieldTypes[k] + ",";
            }
            tableFields += ");";

            string commandString = "CREATE TABLE [" + dbSchema + "]." + tableName + tableFields;
            string result = f.executeSqlCommand(commandString, connectionString);

            return new Dictionary<string, object>
            {
                { "message", result},
                { "tablename", tableName},
                
            };
        }

        /// <summary>
        /// Add columns to a specific table
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="fieldNames">List of columns names</param>
        /// <param name="fieldTypes">Type of fields</param>
        /// <param name="tableName">Table name</param>
        /// <param name="dbSchema">Database schema (default = dbo)</param>
        /// <returns></returns>
        [MultiReturn(new[] { "message" })]
        public static Dictionary<string, object> AddColumnsToTable(string connectionString, List<string> fieldNames, List<string> fieldTypes, string tableName, string dbSchema = "dbo")
        {
            DynaFunctions f = new DynaFunctions();
            int k = 0;
            string result = "";
            foreach (string fn in fieldNames)
            {
                string commandString = "ALTER TABLE [" + dbSchema + "]." + tableName + " ADD " + fn + " " + fieldTypes[k] + ";";
                result = f.executeSqlCommand(commandString, connectionString);
            }
            return new Dictionary<string, object>
            {
                { "message", result},
            };
        }



    }

    public static class Drop
    {
        /// <summary>
        /// Delete a database
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <param name="dbName">Database name</param>
        /// <returns></returns>
        [MultiReturn(new[] { "message", "dbName" })]
        public static Dictionary<string, object> Database(String connectionString, string dbName)
        {
            DynaFunctions f = new DynaFunctions();
            string commandString = "USE [master]; DROP DATABASE " + dbName;
            string result = f.executeSqlCommand(commandString, connectionString);

            return new Dictionary<string, object>
            {
                { "message", result},
                { "dbName", dbName},
            };
        }

        /// <summary>
        /// Delete a table from a specific schema
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="tableName">Table name</param>
        /// <param name="dbSchema">Database schema (default = dbo)</param>
        /// <returns></returns>
        [MultiReturn(new[] { "message", "tableName" })]
        public static Dictionary<string, object> Table(string connectionString, string tableName, string dbSchema = "dbo")
        {
            DynaFunctions f = new DynaFunctions();
            string commandString = "DROP TABLE [" + dbSchema + "]." + tableName;
            string result = f.executeSqlCommand(commandString, connectionString);

            return new Dictionary<string, object>
            {
                { "message", result},
                { "tablename", tableName},
                
            };
        }

        /// <summary>
        /// Remove columns from a table
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="fieldNames">Columns names</param>
        /// <param name="tableName">Table name</param>
        /// <param name="dbSchema">Database schema (default = dbo)</param>
        /// <returns></returns>
        [MultiReturn(new[] { "message" })]
        public static Dictionary<string, object> ColumnsFromTable(string connectionString, List<string> fieldNames, string tableName, string dbSchema = "dbo")
        {
            DynaFunctions f = new DynaFunctions();
            string result = "";
            foreach (string fn in fieldNames)
            {
                string commandString = "ALTER TABLE [" + dbSchema + "]." + tableName + " DROP COLUMN " + fn + ";";
                result = f.executeSqlCommand(commandString, connectionString);
            }
            return new Dictionary<string, object>
            {
                { "message", result},
            };
        }
                       

    }

    public static class Select
    {
        /// <summary>
        /// Select the content from a column
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="tableName">Table name</param>
        /// <param name="fieldName">Column name</param>
        /// <param name="dbSchema">Database schema (default = dbo)</param>
        /// <returns></returns>
        [MultiReturn(new[] { "message", "output"})]
        public static Dictionary<string, object> ColumnFromTableByName(string connectionString, string tableName, string fieldName, string dbSchema = "dbo")
        {
            DynaFunctions f = new DynaFunctions();
            string commandString = "SELECT * FROM [" + dbSchema + "]." + tableName;
            string result = "";
            List<string> output = new List<string>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(commandString, conn);
                    conn.Open();
                    command.ExecuteNonQuery();
                    result = "Executed";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader[fieldName.ToString()].ToString() != null)
                                output.Add(reader[fieldName.ToString()].ToString());
                            else
                            {
                                output.Add("null");
                            }
                        }
                    }
                    conn.Close();
                }
                catch (SqlException ex)
                {
                    result = ex.Message;
                }
            }

            return new Dictionary<string, object>
            {
                { "message", result},
                { "output", output},
            };
        }

        /// <summary>
        /// Select the names of columns
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="tableName">Table name</param>
        /// <param name="dbSchema">Database schema (default = dbo)</param>
        /// <returns></returns>
        [MultiReturn(new[] { "message", "output" })]
        public static Dictionary<string, object> ColumnsNamesFromTable(string connectionString, string tableName, string dbSchema = "dbo")
        {
            DynaFunctions f = new DynaFunctions();
            string commandString = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tableName +"' AND TABLE_SCHEMA = '" + dbSchema + "';";
            string result = "";
            List<string> output = new List<string>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(commandString, conn);
                    conn.Open();
                    command.ExecuteNonQuery();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            output.Add(reader[0].ToString());
                        }
                    }
                    conn.Close();
                    result = "Executed";
                }
                catch (SqlException ex)
                {
                    result = ex.Message;
                }
            }

            return new Dictionary<string, object>
            {
                { "message", result},
                { "output", output},
            };
        }

    }

    public static class Insert
    {
        /// <summary>
        /// Insert values in a table
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="fieldNames">Columns names</param>
        /// <param name="values">List of values as string</param>
        /// <param name="tableName">Table name</param>
        /// <param name="dbSchema">Database schema (default = dbo)</param>
        /// <returns></returns>
        [MultiReturn(new[] { "message", "data" })]
        public static Dictionary<string, object> IntoTable(string connectionString, List<string> fieldNames, List<List<string>> values, string tableName, string dbSchema = "dbo")
        {
            DynaFunctions f = new DynaFunctions();
            string result = "";
            string names = "";
            List<string> data = new List<string>();
            foreach (string fN in fieldNames)
            {
                names += fN + ",";
            }
            names = names.Substring(0, names.Length - 1);
            data.Add(names);

            foreach (List<string> j in values)
            {
                string s = "";
                foreach (string k in j)
                {
                    s += "'" + k.ToString() + "',";
                }
                s = s.Substring(0, s.Length - 1);
                data.Add(s);
                string commandString = "INSERT INTO [" + dbSchema + "]." + tableName + " (" + names + ") VALUES (" + s + ");";
                result = f.executeSqlCommand(commandString, connectionString);
            }
            return new Dictionary<string, object>
            {
                { "message", result},
                { "data", data},
            };
        }
    }

}
