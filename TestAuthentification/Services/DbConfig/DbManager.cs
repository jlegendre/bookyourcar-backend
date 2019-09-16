using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using MySql.Data.MySqlClient;
using TestAuthentification.Services.DbConfig;

namespace TestAuthentification.Services
{
    public class DbManager : IDisposable
    {

        #region Private

        private MySqlConnection Connection { get; set; }

        private MySqlCommand Command { get; set; }

        public List<DbParameter> OutParameters { get; private set; }


        private static string ConnectionString
        {
            get
            {
                return "server=mvinet.fr;port=3306;database=BookYourCar;uid=a5d;password=pwtk@[gh$!7Z#&wX";
            }
        }


        private void Open()
        {
            try
            {
                Connection = new MySqlConnection(ConnectionString);

                Connection.Open();
            }
            catch (Exception ex)
            {
                Close();
            }
        }

        private void Close()
        {
            if (Connection != null)
            {
                Connection.Close();
            }
        }

        // executes stored procedure with DB parameteres if they are passed
        private object ExecuteProcedure(string procedureName, ExecuteTypeEnum executeType, List<MySqlParameter> parameters)
        {
            object returnObject = null;

            if (Connection != null)
            {
                if (Connection.State == ConnectionState.Open)
                {
                    if (executeType == ExecuteTypeEnum.ExecuteFunctionTable || executeType == ExecuteTypeEnum.ExecuteFunctionScalar)
                    {
                        string par = "";
                        foreach (MySqlParameter dbParameter in parameters)
                        {
                            par += "@" + dbParameter.ParameterName + ",";
                        }

                        if (par.Length > 1)
                            par = par.TrimEnd(',');


                        if (executeType == ExecuteTypeEnum.ExecuteFunctionTable)
                            procedureName = "SELECT * FROM " + procedureName + "(" + par + ")";
                        if (executeType == ExecuteTypeEnum.ExecuteFunctionScalar)
                            procedureName = "SELECT " + procedureName + "(" + par + ")";
                        Command = new MySqlCommand(procedureName, Connection);
                        foreach (MySqlParameter dbParameter in parameters)
                        {
                            Command.Parameters.AddWithValue("@" + dbParameter.ParameterName, dbParameter.Value);
                        }

                        Command.CommandType = CommandType.Text;
                    }
                    else
                    {
                        Command = new MySqlCommand(procedureName, Connection);
                        Command.CommandType = CommandType.StoredProcedure;

                        // pass stored procedure parameters to command
                        if (parameters != null)
                        {
                            Command.Parameters.Clear();

                            foreach (MySqlParameter dbParameter in parameters)
                            {
                                MySqlParameter parameter = new MySqlParameter();
                                parameter.ParameterName = "@" + dbParameter.ParameterName;
                                parameter.Direction = dbParameter.Direction;
                                parameter.Value = dbParameter.Value ?? DBNull.Value;
                                Command.Parameters.Add(parameter);
                            }
                        }
                    }


                    switch (executeType)
                    {
                        case ExecuteTypeEnum.ExecuteReader:
                        case ExecuteTypeEnum.ExecuteFunctionTable:
                            returnObject = Command.ExecuteReader();
                            break;
                        case ExecuteTypeEnum.ExecuteNonQuery:
                            returnObject = Command.ExecuteNonQuery();
                            break;
                        case ExecuteTypeEnum.ExecuteScalar:
                        case ExecuteTypeEnum.ExecuteFunctionScalar:
                            returnObject = Command.ExecuteScalar();
                            break;
                        default:
                            break;
                    }

                }
            }

            return returnObject;
        }

        // updates output parameters from stored procedure
        private void UpdateOutParameters()
        {
            if (Command.Parameters.Count > 0)
            {
                OutParameters = new List<DbParameter>();
                OutParameters.Clear();

                for (int i = 0; i < Command.Parameters.Count; i++)
                {
                    if (Command.Parameters[i].Direction == ParameterDirection.Output)
                    {
                        OutParameters.Add(new DbParameter(Command.Parameters[i].ParameterName,
                                                          ParameterDirection.Output,
                                                          Command.Parameters[i].Value));
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Close();
        }


        #endregion

        #region Un seul objet

        // executes scalar query stored procedure without parameters
        public T ExecuteSingle<T>(string procedureName) where T : new()
        {
            return ExecuteSingle<T>(procedureName, null);
        }

        // executes scalar query stored procedure and maps result to single object
        public T ExecuteSingle<T>(string procedureName, List<MySqlParameter> parameters) where T : new()
        {
            Open();
            IDataReader reader = (IDataReader)ExecuteProcedure(procedureName, ExecuteTypeEnum.ExecuteReader, parameters);
            T tempObject = new T();

            if (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    PropertyInfo propertyInfo = typeof(T).GetProperty(reader.GetName(i));
                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    propertyInfo.SetValue(tempObject, value, null);
                }
            }
            else
            {
                reader.Close();
                return default(T);
            }

            reader.Close();

            UpdateOutParameters();

            Close();

            return tempObject;
        }

        #endregion

        #region Liste d'objet

        /// <summary>
        /// Fonction permettant de retourner une liste d'object Générique avec en paramètre le nom de la procedure stockée
        /// </summary>
        public List<T> ExecuteList<T>(string procedureName) where T : new()
        {
            return ExecuteList<T>(procedureName, null);
        }

        // executes list query stored procedure and maps result generic list of objects
        /**
        * Fonction permettant de retourner une liste d'object Générique avec en paramètre le nom de la procedure stockée ainsi que un ou plusieurs autres paramètres
        */
        public List<T> ExecuteList<T>(string procedureName, List<MySqlParameter> parameters) where T : new()
        {
            List<T> objects = new List<T>();

            Open();
            IDataReader reader = (IDataReader)ExecuteProcedure(procedureName, ExecuteTypeEnum.ExecuteReader, parameters);

            while (reader.Read())
            {
                T tempObject = new T();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetValue(i) != DBNull.Value)
                    {
                        PropertyInfo propertyInfo = typeof(T).GetProperty(reader.GetName(i));
                        var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        propertyInfo.SetValue(tempObject, value, null);
                    }
                }

                objects.Add(tempObject);
            }

            reader.Close();

            UpdateOutParameters();

            Close();

            return objects;
        }

        #endregion

        #region Scalaire (insert, update) avec retour

        public T ExecuteScalar<T>(string procedureName) where T : new()
        {
            return ExecuteScalar<T>(procedureName, null);
        }

        // executes scalar query stored procedure and maps result to single object
        public T ExecuteScalar<T>(string procedureName, List<MySqlParameter> parameters) where T : new()
        {
            Open();

            T tempObject = (T)ExecuteProcedure(procedureName, ExecuteTypeEnum.ExecuteScalar, parameters);

            Close();

            return tempObject;
        }

        #endregion

        #region Non-query (delete ..)
        /// <summary>
        /// executes non query stored procedure with parameters Fonction permettant entre autre d'inserer ou d'update un élement
        /// N'attends pas un jeux de données en sortie 
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string procedureName, List<MySqlParameter> parameters)
        {
            int returnValue;

            Open();

            returnValue = (int)ExecuteProcedure(procedureName, ExecuteTypeEnum.ExecuteNonQuery, parameters);

            UpdateOutParameters();

            Close();

            return returnValue;
        }

        #endregion

        #region Fonctions SQL

        #region Fonctions table (liste objet)

        public List<T> ExecuteFunctionTable<T>(string procedureName) where T : new()
        {
            return ExecuteFunctionTable<T>(procedureName, null);
        }

        public List<T> ExecuteFunctionTable<T>(string procedureName, List<MySqlParameter> parameters) where T : new()
        {
            List<T> objects = new List<T>();

            Open();
            IDataReader reader = (IDataReader)ExecuteProcedure(procedureName, ExecuteTypeEnum.ExecuteFunctionTable, parameters);

            while (reader.Read())
            {
                T tempObject = new T();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetValue(i) != DBNull.Value)
                    {
                        PropertyInfo propertyInfo = typeof(T).GetProperty(reader.GetName(i));
                        var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        propertyInfo.SetValue(tempObject, value, null);
                    }
                }

                objects.Add(tempObject);
            }

            reader.Close();

            UpdateOutParameters();

            Close();

            return objects;
        }

        #endregion

        #region Fonctions scalaires

        public T ExecuteFunctionScalar<T>(string procedureName) where T : new()
        {
            return ExecuteFunctionScalar<T>(procedureName, null);
        }

        // executes scalar query stored procedure and maps result to single object
        public T ExecuteFunctionScalar<T>(string procedureName, List<MySqlParameter> parameters) where T : new()
        {
            Open();

            T tempObject = (T)ExecuteProcedure(procedureName, ExecuteTypeEnum.ExecuteFunctionScalar, parameters);

            Close();

            return tempObject;
        }

        #endregion

        #endregion

    }



}
