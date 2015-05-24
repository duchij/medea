using System;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class user : System.Web.UI.Page
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
        SortedList res = syncdb.getRow("SELECT GET_LOCK('user',5) AS [lock]");
        if (res["status"] == null)
        {
            if (res["lock"].ToString() == "1")
            {
                x2log.logData(res, "", "lock running script");
                this.loadDataUser();
            }
            else
            {
                x2log.logData(res, "", "could not get lock on table - user");
            }
        }

    }

    protected void loadDataUser()
    {
        SortedList res = syncdb.getRow("SELECT * FROM [user_view_sync] ORDER BY [id] DESC LIMIT 1");

        if (res["id"] == null)
        {
            this.startImportData();
        }

    }

    protected void startImportData()
    {
        Dictionary<int, Hashtable> importData = mdb.getTable("SELECT * FROM ADMINSQL.uzivatel_view");

        Dictionary<int, Hashtable> saveData = new Dictionary<int, Hashtable>();
        int dataLn = importData.Count;

        for (int i=0; i< dataLn; i++)
        {
            saveData[i] = new Hashtable();
            saveData[i]["medea_kod"] = importData[i]["kod"];
            saveData[i]["medea_id"] = importData[i]["id"];
            saveData[i]["surname"] = importData[i]["prijmeni"];
            saveData[i]["name"] = importData[i]["meno"];
            saveData[i]["titel"] = importData[i]["titul"];
            saveData[i]["workname"] = importData[i]["pracjmeno"];

        }

        SortedList saveRes = syncdb.mysql_insert_arr("user_view_sync", saveData);

        if (Convert.ToBoolean(saveRes["status"]))
        {
            SortedList logData = new SortedList();
            logData.Add("user_row_count", dataLn);

            SortedList logDRes = syncdb.insertRow("user_view_log", logData);
            if (Convert.ToBoolean(logDRes["status"]))
            {
                this.msg_lbl.Text = "OK";
                x2log.logData(logDRes, "", "full user view import success");
            }
           

            
        }
        else
        {
            this.msg_lbl.Text = saveRes["msg"].ToString();
        }   

    }
}

//SELECT * FROM (
//SELECT 
//ID, NAME, ROW_NUMBER() OVER(ORDER BY ID) AS ROW
//FROM TABLE 
//) AS TMP 
//WHERE ROW = n