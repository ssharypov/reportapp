using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;

namespace Reports
{
    public static class Queries
    {
        public static async Task<Dictionary<int, string>> getStoreTypes(SqlConnection sqlConnection)
        {
            Dictionary<int, string> typeList = new Dictionary<int, string>();
            SqlDataReader reader = null;
            SqlCommand cmd = new SqlCommand("SELECT * FROM [StoreTypes]", sqlConnection);
            try
            {
                reader = await cmd.ExecuteReaderAsync();
                while(await reader.ReadAsync())
                { 
                    typeList.Add((int)reader["TypeId"], Convert.ToString(reader["TypeName"]));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (reader != null) { reader.Close(); }
            }
            return typeList;
        }

        public static async Task<Dictionary<int, string>> getStoreList(SqlConnection sqlConnection, string typeName)
        {

            Dictionary<int, string> stores = new Dictionary<int, string>();
            SqlDataReader reader = null;
            SqlCommand cmd = null;
            if (typeName == "Все")
            {
                cmd = new SqlCommand("SELECT StoreID, StoreAddress FROM [Stores]", sqlConnection);
            }
            else
            {
                cmd = new SqlCommand("SELECT StoreID, StoreAddress FROM [Stores] WHERE StoreType = dbo.GetStoreTypeId(@TypeName)", sqlConnection);
                cmd.Parameters.AddWithValue("typeName", typeName);
            }
            try
            {
                reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    stores.Add((int)reader["StoreID"], Convert.ToString(reader["StoreAddress"]));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (reader != null) { reader.Close(); }
            }
            return stores;
        }

        public static async Task<DataTable> mainReport(SqlConnection sqlConnection, Dictionary<string,string> reportParams)
        {
            DataTable table = new DataTable();
            SqlDataReader reader = null;
            SqlCommand  cmd = null;
            DateTime start_date;
            DateTime end_date;
            string store_list;
            string product_list;
            
            try
            {
                cmd = new SqlCommand("ReportProcedure2",sqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                if (reportParams.ContainsKey("startDate") && reportParams["startDate"] != "0001-01-01" && reportParams.ContainsKey("endDate") && reportParams["endDate"] != "0001-01-01")
                {
                    MessageBox.Show(reportParams["startDate"], "test", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    start_date = DateTime.Parse(reportParams["startDate"]).Date;
                    end_date = DateTime.Parse(reportParams["endDate"]).Date;
                    
                    cmd.Parameters.Add("@startDate",SqlDbType.Date).Value = start_date;
                    cmd.Parameters.Add("@endDate", SqlDbType.Date).Value = end_date;
                }

                if (reportParams.ContainsKey("storeList") && reportParams["storeList"] != "")
                {
                    store_list = reportParams["storeList"];
                    cmd.Parameters.Add("@storeList", SqlDbType.VarChar).Value = store_list;
                }
                if (reportParams.ContainsKey("productList") && reportParams["productList"] != "")
                {
                    product_list = reportParams["productList"];
                    cmd.Parameters.Add("@productList", SqlDbType.VarChar).Value = product_list;
                }
                
                reader = await cmd.ExecuteReaderAsync();
                table.Load(reader);
                return table;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (reader != null){reader.Close();}
                if (cmd != null){cmd.Dispose();}
            }
            return table;
        }
    }
}
