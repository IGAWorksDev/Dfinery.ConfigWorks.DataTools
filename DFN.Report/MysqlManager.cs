using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace DFN.Report
{
    public class MySqlManager
    {

        private string ConnectionString { get; set; }

        public MySqlManager(string connection_string)
        {
            ConnectionString = connection_string;
        }

        private MySqlConnection Connection { get; set; }

        private MySqlCommand Command { get; set; }

        private void Open()
        {
            try
            {
                Connection = new MySqlConnection(ConnectionString);

                Connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Close();
                throw ex;

            }
        }

        private void Close()
        {
            if (Connection != null)
            {
                Connection.Close();
            }
        }

        // execute sql query to generic
        public List<T> ExecuteDynamicQuery<T>(string sql) where T : new()
        {
            // qeury injection 최소한의 방어
            sql = sql.Split(';')[0]; // ; 로 구분하여 여러 문장 날리지 말것!!
                                     //sql = MySqlHelper.EscapeString(sql);

            List<T> objects = new List<T>();

            Open();
            try
            {
                if (Connection != null)
                {
                    if (Connection.State == ConnectionState.Open)
                    {
                        Command = new MySqlCommand(sql, Connection);
                        //Command.CommandTimeout = 30;

                        MySqlDataReader reader = Command.ExecuteReader();

                        while (reader.Read())
                        {
                            T tempObject = new T();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (reader.GetValue(i) != DBNull.Value)
                                {
                                    PropertyInfo propertyInfo = typeof(T).GetProperty(reader.GetName(i));

                                    if (propertyInfo.PropertyType == typeof(Int32) && reader.GetValue(i).GetType() == typeof(Decimal))
                                    {
                                        Decimal value = (Decimal)reader.GetValue(i);

                                        propertyInfo.SetValue(tempObject, Decimal.ToInt32(value), null);
                                    }
                                    else
                                    {
                                        propertyInfo.SetValue(tempObject, reader.GetValue(i), null);
                                    }

                                }
                            }

                            objects.Add(tempObject);

                        }


                        reader.Close();
                    }
                }
                Close();

                return objects;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw e;

            }
            finally
            {
                Close();
            }

        }

    }
}
