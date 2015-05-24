<%@ Application Language="C#" %>

<script runat="server">
    
    
    void Application_Start(object sender, EventArgs e) 
    {
        
        string tmp = System.Web.HttpContext.Current.Server.MapPath("/");
        log x2log = new log();
       
        x2log.checkIfLogExists(tmp);

        //mysql_db mysql = new mysql_db();
        //Boolean status = mysql.offline();
        
       
        
        
        //Session["webstatus"] = "run";
        
        
        // Code that runs on application startup

    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown
        //Session.Abandon();
        

    }
        
    void Application_Error(object sender, EventArgs e) 
    {
        log x2log = new log();
        SortedList data = new SortedList();
        data.Add("object", sender.ToString());
        data.Add("type", sender.GetType().ToString());
        
        x2log.logData(data,Environment.StackTrace.ToString(),"Global ERROR");
        //Server.Transfer("error.html");
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e) 
    {
        Session["serverUrl"] = System.Web.HttpContext.Current.Server.MapPath("/");
        Session["webstatus"] = "run";
        //Session["dSession"] = this.Session.SessionID;
        
        string fg="";

        if (Request.QueryString["duch"] != null)
        {
            fg = Request.QueryString["duch"].ToString();
        }
        
        if (fg == "run0")
        {
            Session["serverUrl"] = System.Web.HttpContext.Current.Server.MapPath("/");
            Session["webstatus"] = "run";
        }
        else
        {
            //mysql_db mysql = new mysql_db();
            //Boolean status = mysql.offline();
            //if (status == false)
            //{
            //    //if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("/") + @"\app_offline.ina"))
            //    //{
            //    //    System.IO.File.Move(System.Web.HttpContext.Current.Server.MapPath("/") + @"\app_offline.ina", System.Web.HttpContext.Current.Server.MapPath("/") + @"\app_offline.htm");
            //    //}
            //    Session.Abandon();
            //    Server.Transfer("offline.html");
            //} 
            //else
            //{
                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("/") + @"\app_offline.htm"))
                {
                    System.IO.File.Move(System.Web.HttpContext.Current.Server.MapPath("/") + @"\app_offline.htm", System.Web.HttpContext.Current.Server.MapPath("/") + @"\app_offline.ina");  
                }
                
                Session["serverUrl"] = System.Web.HttpContext.Current.Server.MapPath("/"); 
                Session["webstatus"] = "run"; 
            //}
        }
        
        
        
        //Session.Timeout = 10;
        // Code that runs when a new session is started
        

    }

    void Session_End(object sender, EventArgs e) 
    {
        Server.Transfer("medea.aspx");
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
    
    void Application_EndRequest(object sender, EventArgs e)
    {
     
    }

    void Application_BeginRequest(object sender, EventArgs e)
    {
       
    }
    
   
       
</script>
