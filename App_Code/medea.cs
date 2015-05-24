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
/// Summary description for omegadb
/// </summary>
public class medea
{

    public log x2log = new log();
    public OdbcConnection my_con = new OdbcConnection();

   

	public medea()
	{

		//
		// TODO: Add constructor logic here
		//
        Configuration myConfig = WebConfigurationManager.OpenWebConfiguration("/is");
        ConnectionStringSettings connString;
        connString = myConfig.ConnectionStrings.ConnectionStrings["medea"];
        my_con.ConnectionString = connString.ToString();

        
	}


    private string parseQuery(string query)
    {
        query = query.Replace('[', '`');
        query = query.Replace(']', '`');

        return query;
    }

    public SortedList mysql_update(string table, SortedList data, string id)
    {
        OdbcTransaction trans1 = null;
        my_con.Open();

        trans1 = my_con.BeginTransaction();

        OdbcCommand cmdtrans = new OdbcCommand();
        cmdtrans.Connection = my_con;
        cmdtrans.Transaction = trans1;

        //StringBuilder sb = sb.AppendFormat("UPDATE [{0}] SET {1} WHERE [id]={2}");


        string[] strArr = new String[data.Count];
        int i = 0;

        foreach (DictionaryEntry tmp in data)
        {
            //cols = cols + tmp.Key + ",";
            if (tmp.Value.ToString() == "NULL")
            {
                strArr[i] = "[" + tmp.Key.ToString() + "]=NULL";
            }
            else
            {
                strArr[i] = "[" + tmp.Key.ToString() + "] ='" + tmp.Value.ToString() + "'";
            }
            i++;
            // parse_str = parse_str.Replace("'", "*");
        }
        string setStr = String.Join(",", strArr);

        StringBuilder sb = new StringBuilder();

        if (id.IndexOf("WHERE") != -1)
        {
            sb.AppendFormat("UPDATE [{0}] SET {1} {2}", table, setStr, id);
        }
        else
        {
            sb.AppendFormat("UPDATE [{0}] SET {1} WHERE [id] = {2}", table, setStr, id);
        }

        //= sb.AppendFormat("UPDATE [{0}] SET {1} WHERE [id]={2}", table, setStr, id);
        string query = sb.ToString();
        query = parseQuery(query);
        //return query;

        // int id = 0;

        SortedList result = new SortedList();
        try
        {
            x2log.logData(query, "", "mysql omegadb update");
            cmdtrans.CommandText = query;
            cmdtrans.ExecuteNonQuery();
            //cmdtrans.CommandText = "SELECT last_insert_id();";
            //id = Convert.ToInt32(cmdtrans.ExecuteScalar());
            trans1.Commit();
            result.Add("status", true);
            result.Add("last_id", id);
        }
        catch (Exception e)
        {
            x2log.logData(query, e.ToString(), "error omegadb wrong sql in mysql_update()");

            result.Add("status", false);
            result.Add("msg", e.ToString());
            result.Add("last_id", 0);
            result.Add("sql", query);
            trans1.Rollback();

        }
        my_con.Close();
        return result;
    }

    

   

    public SortedList execute(string query)
    {
        query = this.parseQuery(query);

        SortedList result = new SortedList();
        OdbcTransaction trans1 = null;
        my_con.Open();
        trans1 = my_con.BeginTransaction();

        OdbcCommand cmdtrans = new OdbcCommand();
        cmdtrans.Connection = my_con;
        cmdtrans.Transaction = trans1;
        try
        {
            x2log.logData(query, "", "mysql omegadb execute");
            cmdtrans.CommandText = query;
            cmdtrans.ExecuteNonQuery();
            trans1.Commit();
            result.Add("status", true);
        }
        catch (Exception e)
        {

            trans1.Rollback();
            result.Add("status", false);
            result.Add("msg", e.ToString());
            result.Add("query", query);
            x2log.logData(result, e.ToString(), "error omegadb wrong sql in execute()");
        }
        my_con.Close();

        return result;

    }

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
            //x2log.logData(this.parseQuery(query.ToString()),"","mysql getrow");
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
            x2log.logData(this.parseQuery(query.ToString()), e.ToString(), "error omegadb wrong sql in getRow()");
            result.Add("status", false);
            result.Add("msg", e.ToString());
            result.Add("sql", this.parseQuery(query.ToString()));
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
            x2log.logData(this.parseQuery(query.ToString()), "", "mysql omegadb getTable");
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

           // x2log.logData(this.parseQuery(query.ToString()), ex.ToString(), "error omegadb wrong sql in getTable()");

            x2log.logData(this.parseQuery(query.ToString()), ex.ToString(), "error medea wrong sql in getTable()");

        }
        my_con.Close();
        return result;

    }


}