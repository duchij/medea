﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class helpers_medea : System.Web.UI.Page
{
    x2_var x2 = new x2_var();
    medea mdb = new medea();
    syncdb syncdb = new syncdb();
    log x2log = new log();

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ckeckLockStatus();
    }

    

    protected void ckeckLockStatus()
    {
        SortedList res = syncdb.getRow("SELECT GET_LOCK('medea',5) AS [lock]");
        if (res["status"] == null)
        {
            if (res["lock"].ToString() == "1")
            {
                x2log.logData(res, "", "lock running script");
                this.loadDataRDG();
            }
            else
            {
                x2log.logData(res, "", "could not get lock on table");
            }
        }
        
    }

    //protected void runSqlFnc(object sender, EventArgs e)
    //{
    //    string rowStr = this.rows_txt.Text.ToString();
    //    int rows = 0;
    //    try
    //    {
    //        rows = Convert.ToInt32(rowStr);
    //    }
    //    catch(Exception ex)
    //    {
    //        this.msg_lbl.Text = ex.ToString();
    //        rows = 10;
    //    }
    //    //this.loadDataRDG();
    //}

    protected void loadDataRDG()
    {

        SortedList lastSyncRow = syncdb.getRow("SELECT [last_time],[last_date] FROM [rdg_view_log] ORDER BY [id] DESC LIMIT 1");
       
        TimeSpan  dtTimeFrom = new TimeSpan();
        TimeSpan dtTimeTo = new TimeSpan();
        //DateTime dateFrom = new DateTime();
        DateTime date = new DateTime();

        string queryIn = "SELECT * FROM ADMINSQL.rdg_view ";
        string query = "";
        
       if (lastSyncRow["last_date"] != null)
       {
          
                DateTime lastDate = Convert.ToDateTime(x2.UnixToMsDateTime(lastSyncRow["last_date"].ToString()));
                date = DateTime.Today;

                if (lastDate == date)
                {
                    string[] tmpArr = lastSyncRow["last_time"].ToString().Split(':');
                    queryIn += "WHERE datum='{0}' AND cas >= {1} AND cas <= {2}";
                    string timeFrom = tmpArr[0]+tmpArr[1];
                    DateTime timeTo = DateTime.Now;
                    string timeToStr = String.Format("{0:HH}",timeTo)+String.Format("{0:mm}",timeTo);

                    query = x2.sprintf(queryIn, new string[] { x2.unixDate(date), timeFrom, timeToStr });
                    this.saveRdgData(query);

                }
                else
                {
                    string[] tmpArr = lastSyncRow["last_time"].ToString().Split(':');
                    queryIn += "WHERE datum='{0}' AND cas >= {1} AND cas <= {2}";
                    string timeFrom = tmpArr[0] + tmpArr[1];

                    queryIn += "WHERE datum='{0}' AND  cas >= {1} cas <= {2}";
                    query = x2.sprintf(queryIn, new string[] { x2.unixDate(lastDate),timeFrom, "2359"});
                    this.saveRdgData(query);

                   // if (res)
                    //{
                        string queryIn2 = "SELECT * FROM ADMINSQL.rdg_view ";
                        queryIn2 += "WHERE datum='{0}' AND cas <= {1}";

                        DateTime timeTo = DateTime.Now;
                        string timeToStr = String.Format("{0:HH}", timeTo) + String.Format("{0:mm}", timeTo);

                        query = x2.sprintf(queryIn2, new string[] { x2.unixDate(date), timeToStr });

                        this.saveRdgData(query);
                    //}
                    //else
                    //{
                    //    SortedList logData = new SortedList();
                    //    logData.Add("rdg_view_id", "NULL");
                    //    logData.Add("last_date", saveData[dataLn - 1]["datum"]);
                    //    logData.Add("last_time", saveData[dataLn - 1]["cas"]);
                    //    logData.Add("succes", "no");
                    //    logData.Add("log_msg", res["msg"].ToString());

                    //    SortedList res1 = syncdb.mysql_insert("rdg_view_log", logData);

                    //}
                    
                }
            
       }
       else
       {
           dtTimeFrom = TimeSpan.Parse("00:00:00");
          // DateTime dtTemp = DateTime.Now;
          // dtTimeTo = TimeSpan.Parse(String.Format("{0:HH}",dtTemp) + String.Format("{0:mm}",dtTemp));
           date = DateTime.Today;

           queryIn += "WHERE datum='{0}' AND cas >= {1}";

           query = x2.sprintf(queryIn, new string[] { x2.unixDate(date), "0001"});
           this.saveRdgData(query);
       }

        


        //DateTime dt = DateTime.Now.AddMinutes(-1);
       
        //string uDate = x2.unixDate(date);

        //string queryIn = "SELECT * FROM ADMINSQL.klinlog_view ";
        //queryIn += "WHERE datum = '2015-05-20' and cas > {0}00 ";
        //queryIn += "AND scpac <> 0";
        

        //queryIn += 
        
        //queryIn += "ORDER BY cas ASC";
        //string queryIn = "SELECT * FROM ADMINSQL.uzivatel_view ";
      //  queryIn += "WHERE datum = '2015-05-20' and cas > {0}00 ";
       // queryIn += "AND scpac <> 0";

        
        //string query = x2.sprintf(queryIn, new string[] { "2015-05-21", time1,uDateTo,time });

       //this.msg_lbl.Text = query;

       // string query = "SELECT name, snapshot_isolation_state_desc, is_read_committed_snapshot_on FROM sys.databases";

        //SortedList res = mdb.execute(query);
        //x2log.logData(res, "", "sp_who mssql");

    }

    protected Boolean saveRdgData(string query)
    {
        Boolean result = false;

        Dictionary<int, Hashtable> data = mdb.getTable(query);
        x2log.logData(data, "", "medea rdg_view_sync");

        Dictionary<int, Hashtable> saveData = new Dictionary<int, Hashtable>();

        int dataLn = data.Count;

        if (dataLn > 0)
        {
            // Table dataTbl = new Table();
            // dataTbl.Width = Unit.Percentage(100);

            // this.data_plh.Controls.Add(dataTbl);

            //  TableHeaderRow headerRow = new TableHeaderRow();
            //  headerRow.BackColor = System.Drawing.Color.Gray;
            //  dataTbl.Controls.Add(headerRow);

            //int headerLn = data[0].Count;

            //foreach (DictionaryEntry head in data[0])
            //{
            //    TableHeaderCell datCell = new TableHeaderCell();
            //    datCell.Text = head.Key.ToString();
            //    headerRow.Controls.Add(datCell);
            //}

            for (int i = 0; i < dataLn; i++)
            {


                //TableRow riadok = new TableRow();
                //dataTbl.Controls.Add(riadok);
                saveData[i] = new Hashtable();
                foreach (DictionaryEntry row in data[i])
                {


                    switch (row.Key.ToString())
                    {
                        case "cas":
                            saveData[i]["cas"] = row.Value.ToString().Substring(0, 2) + ":" + row.Value.ToString().Substring(2, 2) + ":00";
                            saveData[i]["cas_medea"] = row.Value;
                            break;
                        case "datum":
                            saveData[i]["datum"] = x2.unixDate(Convert.ToDateTime(data[i]["datum"].ToString()))+" "+saveData[i]["cas"].ToString();
                            break;
                        case "B":
                            if (row.Value == null || row.Value.ToString().Trim().Length == 0)
                            {
                                saveData[i].Add("B", "NULL");
                            }
                            else
                            {
                                saveData[i]["B"] = row.Value;
                            }

                            break;
                        case "P":
                            if (row.Value == null)
                            {
                                saveData[i]["P"] = "NULL";
                            }
                            else
                            {
                                saveData[i]["P"] = row.Value;
                            }
                            break;
                        case "uzelzkr":
                            saveData[i]["uzol_kratko"] = row.Value;
                            break;
                        case "id_prac":
                            saveData[i]["id_pracoviska_uzol"] = row.Value;
                            break;
                        case "uzelnazov":
                            saveData[i]["uzol_nazov"] = row.Value;
                            break;
                        case "scpac":
                            saveData[i]["scpac"] = row.Value;
                            break;
                        case "sczad":
                            saveData[i]["sczad"] = row.Value;
                            break;
                        case "K":
                            if (row.Value == null || row.Value.ToString().Trim().Length == 0)
                            {
                                saveData[i]["K"] = "NULL";
                            }
                            else
                            {
                                saveData[i]["K"] = row.Value;
                            }
                            break;
                        case "N":
                            if (row.Value == null || row.Value.ToString().Trim().Length == 0)
                            {
                                saveData[i]["N"] = "NULL";
                            }
                            else
                            {
                                saveData[i]["N"] = row.Value;
                            }
                            break;
                        case "A":
                            if (row.Value == null || row.Value.ToString().Trim().Length == 0)
                            {
                                saveData[i]["A"] = "NULL";
                            }
                            else
                            {
                                saveData[i]["A"] = row.Value;
                            }
                            break;


                    }

                    //TableCell dataCell = new TableCell();
                    //dataCell.Text = row.Value.ToString();
                    //riadok.Controls.Add(dataCell);
                }

            }

            SortedList res = syncdb.mysql_insert_arr("rdg_view_sync", saveData);

            if (Convert.ToBoolean(res["status"]))
            {

                SortedList lastRow = syncdb.getRow("SELECT [id] AS [last_id] FROM [rdg_view_sync] ORDER BY [id] DESC LIMIT 1");

                SortedList logData = new SortedList();
                logData.Add("rdg_view_id", lastRow["last_id"]);
                logData.Add("last_date", saveData[dataLn - 1]["datum"]);
                logData.Add("last_time", saveData[dataLn - 1]["cas"]);
                logData.Add("succes", "yes");

                SortedList res1 = syncdb.insertRow("rdg_view_log", logData);

                this.msg_lbl.Text = "OK";
                result = true;
            }
            else
            {
                SortedList logData = new SortedList();
                logData.Add("rdg_view_id", "NULL");
                logData.Add("last_date", saveData[dataLn - 1]["datum"]);
                logData.Add("last_time", saveData[dataLn - 1]["cas"]);
                logData.Add("succes", "no");
                logData.Add("log_msg", res["msg"].ToString());

                SortedList res1 = syncdb.insertRow("rdg_view_log", logData);

                this.msg_lbl.Text = "Error";
            }


        }
        else
        {
            //this.msg_lbl.Text = query + "<br>" + "<br> " + "Asi lock skus refreshnut stranku <br>";
            x2log.logData(data, "", "No data to store in sync");
            result = true;
        }

        return result;

    }
}