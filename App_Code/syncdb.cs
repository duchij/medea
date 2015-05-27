using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Data.Odbc;
using System.Text;

/// <summary>
/// Summary description for syncdb
/// </summary>
public class syncdb 
{
    public log x2log = new log();
    x2_var x2 = new x2_var();
    public OdbcConnection my_con = new OdbcConnection();

	public syncdb()
	{
		//
		// TODO: Add constructor logic here
		//
        Configuration myConfig = WebConfigurationManager.OpenWebConfiguration("/is");
        ConnectionStringSettings connString;
        connString = myConfig.ConnectionStrings.ConnectionStrings["medea_sync"];
        my_con.ConnectionString = connString.ToString();

	}

    public SortedList execute(string query)
    {
        query = this.parseQuery(query);

        SortedList result = new SortedList();
        //OdbcTransaction trans1 = null;
        my_con.Open();
        //trans1 = my_con.BeginTransaction();

        OdbcCommand cmdtrans = new OdbcCommand();
        cmdtrans.Connection = my_con;
        //cmdtrans.Transaction = trans1;
        try
        {
            x2log.logData(query, "", "mysql execute");
            cmdtrans.CommandText = query;
            cmdtrans.ExecuteNonQuery();
            //trans1.Commit();
            result.Add("status", true);
        }
        catch (Exception e)
        {

            //trans1.Rollback();
            result.Add("status", false);
            result.Add("msg", e.ToString());
            result.Add("query", query);
            x2log.logData(result, e.ToString(), "error wrong sql in execute()");
        }
        my_con.Close();

        return result;

    }

    public string parseQuery(string query)
    {
        query = query.Replace('[', '`');
        query = query.Replace(']', '`');

        return query;
    }

    /// <summary>
    /// Vlozi priamo bez stavu transakcie na MyISAM tabulky bez duplicate key
    /// </summary>
    /// <param name="table"></param>
    /// <param name="data"></param>
    /// <returns>Vracia SortedList, status,msg,last_id,sql</returns>
    public SortedList insertRowOld(string table, SortedList data)
    {
        SortedList result = new SortedList();
        //OdbcTransaction trans1 = null;
        my_con.Open();
        //trans1 = my_con.BeginTransaction();

        OdbcCommand cmdtrans = new OdbcCommand();
        cmdtrans.Connection = my_con;
        //cmdtrans.Transaction = trans1;

        StringBuilder sb = new StringBuilder();

        string[] columns = new string[data.Count];
        string[] values = new string[data.Count];
        string[] col_val = new string[data.Count];


        int i = 0;
        foreach (DictionaryEntry row in data)
        {

            columns[i] = "`" + row.Key.ToString() + "`";
            if (row.Value == null)
            {
                values[i] = "NULL";
            }
            else if (row.Value.ToString().Trim().Length == 0)
            {
                values[i] = "NULL";
            }
            else
            {
                values[i] = "'" + row.Value.ToString() + "'";
            }
            col_val[i] = "`" + row.Key + "` =  values(`" + row.Key + "`)";
            i++;
        }

        string t_cols = string.Join(",", columns);
        string t_values = string.Join(",", values);
        string col_val_str = string.Join(",", col_val);

        sb.AppendFormat("INSERT INTO `{0}` ({1}) VALUES ({2});", table, t_cols, t_values);

        string query = sb.ToString();

        int id = 0;
        try
        {
            x2log.logData(query, "", "mysql insert");
            cmdtrans.CommandText = query;
            cmdtrans.ExecuteNonQuery();
            cmdtrans.CommandText = "SELECT last_insert_id();";
            id = Convert.ToInt32(cmdtrans.ExecuteScalar());
            //trans1.Commit();
            result.Add("status", true);
            result.Add("last_id", id);
        }
        catch (Exception e)
        {
            x2log.logData(query, e.ToString(), "error wrong sql in insertRowOld()");
            result.Add("status", false);
            result.Add("msg", e.ToString());
            result.Add("last_id", 0);
            result.Add("sql", query);
            // trans1.Rollback();

        }
        my_con.Close();
        return result;

    }

    /// <summary>
    /// Vlozi priamo bez stavu transakcie na MyISAM tabulky
    /// </summary>
    /// <param name="table"></param>
    /// <param name="data"></param>
    /// <returns>Vracia SortedList, status,msg,last_id,sql</returns>
    public SortedList insertRow(string table, SortedList data)
    {
        SortedList result = new SortedList();
        //OdbcTransaction trans1 = null;
        my_con.Open();
        //trans1 = my_con.BeginTransaction();

        OdbcCommand cmdtrans = new OdbcCommand();
        cmdtrans.Connection = my_con;
        //cmdtrans.Transaction = trans1;

        StringBuilder sb = new StringBuilder();

        string[] columns = new string[data.Count];
        string[] values = new string[data.Count];
        string[] col_val = new string[data.Count];


        int i = 0;
        foreach (DictionaryEntry row in data)
        {

            columns[i] = "`" + row.Key.ToString() + "`";
            if (row.Value == null)
            {
                values[i] = "NULL";
            }
            else if (row.Value.ToString().Trim().Length == 0)
            {
                values[i] = "NULL";
            }
            else
            {
                values[i] = "'" + row.Value.ToString() + "'";
            }
            col_val[i] = "`" + row.Key + "` =  values(`" + row.Key + "`)";
            i++;
        }

        string t_cols = string.Join(",", columns);
        string t_values = string.Join(",", values);
        string col_val_str = string.Join(",", col_val);

        sb.AppendFormat("INSERT INTO `{0}` ({1}) VALUES ({2}) ON DUPLICATE KEY UPDATE {3};", table, t_cols, t_values, col_val_str);

        string query = sb.ToString();

        int id = 0;
        try
        {
            x2log.logData(query, "", "mysql insert");
            cmdtrans.CommandText = query;
            cmdtrans.ExecuteNonQuery();
            cmdtrans.CommandText = "SELECT last_insert_id();";
            id = Convert.ToInt32(cmdtrans.ExecuteScalar());
            //trans1.Commit();
            result.Add("status", true);
            result.Add("last_id", id);
        }
        catch (Exception e)
        {
            x2log.logData(query, e.ToString(), "error wrong sql in insertRow()");
            result.Add("status", false);
            result.Add("msg", e.ToString());
            result.Add("last_id", 0);
            result.Add("sql", query);
           // trans1.Rollback();

        }
        my_con.Close();
        return result;

    }

    public Dictionary<int, Hashtable> getTable(string query)
    {

        Dictionary<int, Hashtable> result = new Dictionary<int, Hashtable>();

        my_con.Open();

        try
        {

            OdbcCommand my_com = new OdbcCommand(this.parseQuery(query.ToString()), my_con);
            x2log.logData(this.parseQuery(query.ToString()), "", "syncdb mysql getTable");
            OdbcDataReader reader = my_com.ExecuteReader();
            int row = 0;

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Hashtable tmp = new Hashtable();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {


                        //string inData = reader.GetValue(i).ToString();
                        // byte[] buffer = Encoding.UTF8.GetBytes(inData);
                        //string outData = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                        if (reader.GetValue(i) == DBNull.Value)
                        {
                            tmp.Add(reader.GetName(i).ToString(), "NULL");
                        }
                        else
                        {
                            string tf = reader.GetFieldType(i).ToString();

                            if (tf == "System.Byte[]")
                            {
                                byte[] dl = (byte[])reader.GetValue(i);
                                string rr = System.Text.Encoding.UTF8.GetString(dl);

                                // tmp.Add(reader.GetName(i).ToString(), reader.GetString(i).ToString());
                                tmp.Add(reader.GetName(i).ToString(), rr);
                            }
                            else
                            {
                                tmp.Add(reader.GetName(i).ToString(), reader.GetValue(i));
                            }
                        }
                    }
                    result.Add(row, tmp);
                    row++;
                }
            }
        }
        catch (Exception ex)
        {
            x2log.logData(this.parseQuery(query.ToString()), ex.ToString(), "syncdb error wrong sql in getTable()");
        }
        my_con.Close();


        return result;

    }


    /// <summary>
    /// Vlozi novy riadok no db a vrati posledne ID
    /// </summary>
    /// <param name="table">string table name</param>
    /// <param name="data">SortedList data to insert</param>
    /// <returns>SortedList kyes: status (true/false), msg(error),last_id(on succes),sql(sql in error)</returns>

    public SortedList mysql_insert(string table, SortedList data)
    {
        SortedList result = new SortedList();
        OdbcTransaction trans1 = null;
        my_con.Open();
        trans1 = my_con.BeginTransaction();

        OdbcCommand cmdtrans = new OdbcCommand();
        cmdtrans.Connection = my_con;
        cmdtrans.Transaction = trans1;

        StringBuilder sb = new StringBuilder();

        string[] columns = new string[data.Count];
        string[] values = new string[data.Count];
        string[] col_val = new string[data.Count];


        int i = 0;
        foreach (DictionaryEntry row in data)
        {

            columns[i] = "`" + row.Key.ToString() + "`";
            if (row.Value == null)
            {
                values[i] = "NULL";
            }
            else if (row.Value.ToString().Trim().Length == 0)
            {
                values[i] = "NULL";
            }
            else if (row.Value.ToString() =="NULL")
            {
                values[i] = "NULL";
            }
            else
            {
                values[i] = "'" + row.Value.ToString() + "'";
            }
            col_val[i] = "`" + row.Key + "` =  values(`" + row.Key + "`)";
            i++;
        }

        string t_cols = string.Join(",", columns);
        string t_values = string.Join(",", values);
        string col_val_str = string.Join(",", col_val);

        sb.AppendFormat("INSERT INTO `{0}` ({1}) VALUES ({2}) ON DUPLICATE KEY UPDATE {3};", table, t_cols, t_values, col_val_str);

        string query = sb.ToString();

        int id = 0;
        try
        {
            x2log.logData(query, "", "mysql insert");
            cmdtrans.CommandText = query;
            cmdtrans.ExecuteNonQuery();
            cmdtrans.CommandText = "SELECT last_insert_id();";
            id = Convert.ToInt32(cmdtrans.ExecuteScalar());
            trans1.Commit();
            result.Add("status", true);
            result.Add("last_id", id);
        }
        catch (Exception e)
        {
            x2log.logData(query, e.ToString(), "error wrong sql in mysql_insert()");
            result.Add("status", false);
            result.Add("msg", e.ToString());
            result.Add("last_id", 0);
            result.Add("sql", query);
            trans1.Rollback();

        }
        my_con.Close();
        return result;

    }

    /// <summary>
    /// returns row of SQL request
    /// </summary>
    /// <param name="query"></param>
    /// <returns>
    /// SortedList if error the result[status],result[msg],result[sql]
    /// </returns>
    public SortedList getRow(string query)
    {
        if (query.IndexOf("LIMIT 1") == -1)
        {
            query += " LIMIT 1";
        }
        SortedList result = new SortedList();
        my_con.Open();


        try
        {
            OdbcCommand my_com = new OdbcCommand(this.parseQuery(query.ToString()), my_con);
            x2log.logData(this.parseQuery(query.ToString()), "", "mysql getrow");
            OdbcDataReader reader = my_com.ExecuteReader();
            // result.Add("status", true);

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (reader.GetValue(i) == DBNull.Value)
                        {
                            result.Add(reader.GetName(i).ToString(), "NULL");
                        }
                        else
                        {
                            string tf = reader.GetFieldType(i).ToString();
                            if (tf == "System.Byte[]")
                            {
                                byte[] dl = (byte[])reader.GetValue(i);
                                string rr = System.Text.Encoding.UTF8.GetString(dl);
                                result.Add(reader.GetName(i).ToString(), rr);
                            }
                            else
                            {
                                result.Add(reader.GetName(i).ToString(), reader.GetValue(i));
                            }
                        }
                    }

                }
            }

        }
        catch (Exception e)
        {
            x2log.logData(this.parseQuery(query.ToString()), e.ToString(), "error wrong sql in getRow() syncdb");
            result.Add("status", false);
            result.Add("msg", e.ToString());
            result.Add("sql", this.parseQuery(query.ToString()));
        }
        my_con.Close();


        return result;
    }

    public SortedList mysql_insert_arr(string table, Dictionary<int, Hashtable> data)
    {
        SortedList result = new SortedList();

        int dataCnt = data.Count;

        string[] arr = new string[dataCnt];
        string[] columns = new string[data[0].Count];
        string[] col_vals = new string[data[0].Count];

        for (int i = 0; i < dataCnt; i++)
        {
            string[] _tmp = new string[data[i].Count];
            int j = 0;
            foreach (DictionaryEntry _row in data[i])
            {
                if (i == 0)
                {
                    columns[j] = "`" + _row.Key.ToString() + "`";
                    col_vals[j] = "`" + _row.Key.ToString() + "` =  values(`" + _row.Key.ToString() + "`)";
                }

                if (_row.Value == null)
                {
                    _tmp[j] = "NULL";
                }
                else if (_row.Value.ToString().Trim().Length == 0)
                {
                    _tmp[j] = "NULL";
                }
                else if (_row.Value.ToString() == "NULL")
                {
                    _tmp[j] = "NULL";
                }
                else
                {
                    _tmp[j] = "'" + _row.Value.ToString() + "'";
                }
                j++;
            }
            arr[i] = "(" + string.Join(",", _tmp) + ")";
        }

        string t_cols = string.Join(",", columns);
        string t_cols_vals = string.Join(",", arr);
        string t_vals = string.Join(",", col_vals);

        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("INSERT INTO `{0}` ({1}) VALUES {2} ON DUPLICATE KEY UPDATE {3};", table, t_cols, t_cols_vals, t_vals);
        string query = sb.ToString();

        OdbcTransaction trans1 = null;
        my_con.Open();
        trans1 = my_con.BeginTransaction();

        OdbcCommand cmdtrans = new OdbcCommand();
        cmdtrans.Connection = my_con;
        cmdtrans.Transaction = trans1;

        try
        {
            x2log.logData(query, "", "mysql insert");
            cmdtrans.CommandText = query;
            cmdtrans.ExecuteNonQuery();
            // cmdtrans.CommandText = "SELECT last_insert_id();";
            // id = Convert.ToInt32(cmdtrans.ExecuteScalar());
            trans1.Commit();
            result.Add("status", true);
            //result.Add("last_id", id);
        }
        catch (Exception e)
        {
            x2log.logData(query, e.ToString(), "error wrong sql in mysql_insert_arr()");
            result.Add("status", false);
            result.Add("msg", e.ToString());
            //result.Add("last_id", 0);
            result.Add("sql", query);
            trans1.Rollback();

        }
        my_con.Close();



        return result;
    }
}