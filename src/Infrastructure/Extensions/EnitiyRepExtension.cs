using Microsoft.Data.SqlClient;
using Serilog;
using System.Data;
using System.Reflection;

namespace Infrastructure.Extensions
{
    public static class EnitiyRepExtension
    {
        public static List<T> ConvertDataTable<T>(this DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        public static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name.ToUpper() == column.ColumnName.Replace("_", "").ToUpper())
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        public static void AddParameter(this SqlCommand command, List<SqlParameter> SqlParameters)
        {
            foreach (var item in SqlParameters)
            {
                command.Parameters.Add(item);
            }
        }

        public static List<string> GetCursor(this List<SqlParameter> item2)
        {
            return item2.Where(w => w.DbType != DbType.Object).Select(s => s.ParameterName).ToList();
        }


        public static List<object> FetchExecuteReader(this SqlCommand cmd, List<string> refcur)
        {
            var _array = new List<object>();
            try
            {
                foreach (var item in refcur)
                {
                    var dt = new DataTable(item);
                    cmd.CommandText = $"fetch all in \"{item}\";";
                    cmd.CommandType = CommandType.Text;
                    //cmd.AllResultTypesAreUnknown = true;

                    var reader = cmd.ExecuteReader();
                    var columns = Enumerable.Range(0, reader.FieldCount)
                        .Select((x) =>
                        {
                            var colName = reader.GetName(x);
                            return new DataColumn(colName);
                        })
                        .ToArray();
                    dt.Columns.AddRange(columns);
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var row = dt.NewRow();
                            foreach (var column in columns)
                            {
                                var fieldValue = reader.GetValue(reader.GetOrdinal(column.ColumnName));
                                row[column] = fieldValue;
                            }
                            dt.Rows.Add(row);
                        }
                    }
                    _array.Add(dt);
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occured FetchExecuteReader");
            }
            return _array;
        }
    }
}
