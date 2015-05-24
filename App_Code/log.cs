using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Net;
using System.Net.Mail;

/// <summary>
/// Summary description for log
/// </summary>
public class log
{
   // x2_var x2 = new x2_var(); 
    public string serverPath = "";
    
	public log()
	{
        //this.ServerPath = path;
		//
		// TODO: Add constructor logic here
		//
	}

    private string Ip()
    {
        string ipAddress = "unknown";
        try
        {

            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {

                ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                /* if (ipAddress == null || ipAddress == "unknown")
                 {
                     ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                 }*/
            }
            else
            {
                ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
        }
        catch (Exception e)
        {

        }
        return ipAddress;
    }

    private string unixDate(DateTime datum)
    {
        string mesiac = datum.Month.ToString();
        string den = datum.Day.ToString();
        string rok = datum.Year.ToString();

        return rok + "-" + mesiac + "-" + den;
    }

    private void sendMail(SortedList data)
    {
        MailAddress from = new MailAddress("kdch@kdch.sk");
        MailAddress to = new MailAddress("bduchaj@gmail.com");

        MailMessage sprava = new MailMessage();
        sprava.From = from;
        sprava.To.Add(to);
        sprava.Subject = "Error is.kdch.sk - " + DateTime.Now.ToLongDateString();
        sprava.IsBodyHtml = true;
        sprava.BodyEncoding = Encoding.UTF8;

        string regSprava = "";
        regSprava += "<h3>Error in is.kdch.sk - "+DateTime.Now.ToLongDateString()+"</h3>";
        regSprava += "<hr>";
        regSprava += "<p>"+data["stack"].ToString()+"</p>";
        regSprava += "<p>" + data["error"].ToString() + "</p>";
        regSprava += "<p>"+data["data"].ToString()+"</p>";

        sprava.Body = regSprava;
        SmtpClient mail_klient = new SmtpClient("mail3.webglobe.sk");
        try
        {
            mail_klient.Send(sprava);
        }
        catch (Exception e)
        {
            
        }


    }

    public void checkIfLogExists(string serverPath)
    {
       /* string serverPath = "";
        try
        {
        
            serverPath = HttpContext.Current.Server.MapPath("~");
        }
        catch (Exception e)
        {
             serverPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
        }*/

        DateTime dt = DateTime.Today;
        string shortDate = this.unixDate(dt);
        //string path = @"..\App_Data\";

        string complFile = serverPath+@"\App_Data\medeasync_"+shortDate+".log";

        if (!File.Exists(complFile))
        {
            File.Create(complFile,1,FileOptions.Asynchronous);
        }

        if (dt.Hour >= 22)
        {
            dt.AddDays(1);
            shortDate = this.unixDate(dt);
            complFile = serverPath + @"\App_Data\medeasync_" + shortDate + ".log";

            if (!File.Exists(complFile))
            {
                File.Create(complFile,1,FileOptions.Asynchronous);
            }

        }
    }

    private StreamWriter openFile()
    {
        try
        {
            this.serverPath = System.Web.HttpContext.Current.Session["serverUrl"].ToString();
        }
        catch (Exception e)
        {
            this.serverPath = "end";
        }
      //  System.Web.HttpContext.Current.Server.
       /* try
        {
            serverPath = HttpContext.Current.Server.MapPath("~");
        }
        catch(Exception e)
        {
            serverPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            
        }*/
        StreamWriter sfw =null;
        if (serverPath != "end")
        {
            DateTime dt = DateTime.Today;

            string shortDate = this.unixDate(dt);
            //string path = @"..\App_Data\";

            string complFile = serverPath + @"\App_Data\medeasync_" + shortDate + ".log";

            /*if (!File.Exists(complFile))
            {
                File.Create(complFile);
            } */

            sfw  = new StreamWriter(@complFile, true);
        }
       
        

        return sfw;
    }

    public void logData(object data, string error, string idf)
    {
        string logIp = this.Ip();
        
        string dt = DateTime.Today.ToShortDateString();
        string dh = DateTime.Now.ToLongTimeString();
        StringBuilder sb = new StringBuilder();
        SortedList errorDt = new SortedList();

        Boolean sendMail = false;

        if (error.Length > 0)
        {
            sb.AppendFormat("ERROR   {0} {1} -- {2}, IP:{3} ERROR:\r\n {4} ", dt, dh, idf,logIp, error);
            sb.AppendFormat("Stack trace: {0} \r\n", Environment.StackTrace.ToString());
            sb.AppendLine("\r\n-----------------------------------------------------------END OF ERROR\r\n");

            sendMail = true;
            
            errorDt.Add("stack", Environment.StackTrace.ToString());
            errorDt.Add("error", error);
        }
        //sw.WriteLine(sb.ToString());
       // sb.Length = 0;
        if (data.GetType() == typeof(SortedList))
        {
            SortedList sl = (SortedList)data;
            sb.AppendFormat("{0} {1} -- {2} --IP:{3} ---- SortedList:\r\n",dt,dh,idf,logIp);
            foreach (DictionaryEntry row in sl)
            {
                sb.AppendFormat("       ['{0}'] = {1} \r\n", row.Key.ToString(), row.Value.ToString());
            }
            sb.AppendLine("\r\n-----------------------------------------------------------END OF  SortedList\r\n");

            if (sendMail) errorDt.Add("data", sb.ToString());
            
        }

        if (data.GetType() == typeof(Dictionary<int, Hashtable>))
        {
            Dictionary<int, Hashtable> table = (Dictionary<int, Hashtable>)data;
            sb.AppendFormat("{0} {1} -- {2} --IP:{3} -- Dictionary<int, Hashtable>:\r\n", dt, dh, idf,logIp);
            int cnt = table.Count;
            for (int row = 0; row < cnt; row++)
            {
                foreach (DictionaryEntry riad in table[row])
                {
                    sb.AppendFormat("        [{0}]['{1}'] = {2} \r\n", row, riad.Key.ToString(), riad.Value.ToString());
                }
            }
            sb.AppendLine("\r\n-----------------------------------------------------------END OF  Dictionary<int, Hashtable>\r\n");
            if (sendMail) errorDt.Add("data", sb.ToString());
        }

        if (data.GetType() == typeof(Dictionary<int, SortedList>))
        {
            Dictionary<int, SortedList> table = (Dictionary<int, SortedList>)data;
            sb.AppendFormat("{0} {1} -- {2} -- IP:{3} -- Dictionary<int, SortedList>:\r\n", dt, dh, idf,logIp);
            int cnt = table.Count;
            for (int row = 0; row < cnt; row++)
            {
                foreach (DictionaryEntry riad in table[row])
                {
                    sb.AppendFormat("        [{0}]['{1}'] = {2} \r\n", row, riad.Key.ToString(), riad.Value.ToString());
                }
            }
            sb.AppendLine("\r\n-----------------------------------------------------------END OF  Dictionary<int, SortedList>\r\n");
            if (sendMail) errorDt.Add("data", sb.ToString());
        }


        if (data.GetType() == typeof(string))
        {
            sb.AppendFormat("{0} {1} -- {2} -- IP:{3} -- string data:\r\n", dt, dh, idf,logIp); 
            sb.AppendFormat("        string = {0} \r\n", data.ToString());
            sb.AppendLine("\r\n-----------------------------------------------------------END OF  string\r\n");

           
            if (sendMail) errorDt.Add("data", sb.ToString());
        }

        

        StreamWriter sw = this.openFile();
        if (sw != null)
        {
            sw.WriteLine(sb.ToString());
            sw.Close();
            if (sendMail) this.sendMail(errorDt);
        }
        
        
    }

   

}