using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
//using iTextSharp.text.html.simpleparser;

	


/// <summary>
/// Summary description for x2_var
/// </summary>
public class x2_var
{
    //mysql_db x2MySql = new mysql_db();
    log x2log = new log();

	public x2_var()
	{
		//
		// TODO: Add constructor logic here
		//
	}

  
    public string setLabel(string label)
    {
        string result = "";
        SortedList _labels = (SortedList)System.Web.HttpContext.Current.Session["LABELS"];

        if (_labels.ContainsKey(label))
        {
            result = _labels[label].ToString();
        }
        else
        {
            result = label;
        }

        return result;
    }

    public string sprintf(string str, string[] args)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendFormat(str, args);

        return sb.ToString();
    }

    public int makeDateGroup(int rok, int mesiac)
    {
        int result = 0;

        string mesStr = mesiac.ToString();
        string rokStr = rok.ToString();

        if (mesStr.Length == 1)
        {
            mesStr = "0" + mesStr;
        }

        result = Convert.ToInt32(rokStr + mesStr);

        return result;
    }


    public static string UTFtoASCII(string value)
    {
        if (String.IsNullOrEmpty(value))
            return value;

        string normalized = value.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();

        foreach (char c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        Encoding nonunicode = Encoding.GetEncoding(850);
        Encoding unicode = Encoding.Unicode;

        byte[] nonunicodeBytes = Encoding.Convert(unicode, nonunicode, unicode.GetBytes(sb.ToString()));
        char[] nonunicodeChars = new char[nonunicode.GetCharCount(nonunicodeBytes, 0, nonunicodeBytes.Length)];
        nonunicode.GetChars(nonunicodeBytes, 0, nonunicodeBytes.Length, nonunicodeChars, 0);

        //string result = nonunicodeChars.ToString();

        return new string(nonunicodeChars);
    }

    

    public int pocetVolnychDni(DateTime datum1, DateTime datum2, string[] volneDni)
    {
        int pocetDni = (datum2 - datum1).Days;
        int pocetDniVikendu = (pocetDni / 7) * 2;
        int pom = pocetDni % 7;

        for (int i = 1; i <= pom; i++)
        {
            DateTime x = datum1.AddDays(i);
           
            if ( (x.DayOfWeek == DayOfWeek.Saturday) || (x.DayOfWeek == DayOfWeek.Sunday) )
            {
                pocetDniVikendu++;
            }

        }

        int dlzka = volneDni.Length;
        DateTime dnesJe = DateTime.Now;
        for (int i = 0; i < dlzka; i++)
        {
            DateTime ckDay = Convert.ToDateTime(volneDni[i] + "." + dnesJe.Year.ToString());
            if ((ckDay >= datum1) && (ckDay <= datum2))
            {
                if ((ckDay.DayOfWeek != DayOfWeek.Sunday) && (ckDay.DayOfWeek != DayOfWeek.Saturday))
                {
                    pocetDniVikendu++;
                }

            }
        }



        return pocetDniVikendu;
    }

    public int pocetVolnychDniBezSviatkov(DateTime datum1, DateTime datum2)
    {

        int days = DateTime.DaysInMonth(datum1.Year, datum1.Month);

        int pocetDni = (datum2 - datum1).Days;
        int pocetDniVikendu = 0;
        //int pocetDniVikendu = (pocetDni / 7) * 2;
        //int pom = pocetDni % 7;

        for (int i = 1; i <= days; i++)
        {
            DateTime x = datum1.AddDays(i-1);

            if ((x.DayOfWeek == DayOfWeek.Saturday) || (x.DayOfWeek == DayOfWeek.Sunday))
            {
                pocetDniVikendu++;
            }

        }

        return pocetDniVikendu;
    }


    public string unixDate(DateTime datum)
    {
        string mesiac = datum.Month.ToString();
        string den = datum.Day.ToString();
        string rok = datum.Year.ToString();

        return rok + "-" + mesiac + "-" + den;
    }

    public DateTime UnixToMsDateTime(string datetime)
    {
        DateTime result;

        if (datetime.IndexOf("-") != -1)
        {

            string[] tmp1 = datetime.Split(' ');
            string[] tmp2 = tmp1[0].Split('-');

            string _res = tmp2[2] + ". " + tmp2[1] + ". " + tmp2[0];
            if (tmp1.Length > 1)
            {
                try
                {
                    result = Convert.ToDateTime(_res + " " + tmp1[1]);
                }
                catch (Exception e)
                {
                    x2log.logData(datetime.ToString(), e.ToString(), "datetime error UnixToMsDateTime() Incorect date in db check db!!");
                    result = DateTime.Today;
                }
            }
            else
            {
                try
                {
                    result = Convert.ToDateTime(_res);
                }
                catch (Exception e)
                {
                    x2log.logData(datetime.ToString(), e.ToString(), "datetime error UnixToMsDateTime()  Incorect date in db check db!!");
                    result = DateTime.Today;
                }
            }
        }
        else
        {
            try
            {
                result = Convert.ToDateTime(datetime);
            }
            catch (Exception e)
            {
                x2log.logData(datetime.ToString(), e.ToString(), "datetime error UnixToMsDateTime()  Incorect date in db check db!!");
                result = DateTime.Today;
            }
        }

        return result;
    }

    public string MSDate(string datum)
    {
        string result = datum;
        if (datum.IndexOf("-") != -1)
        {
            string[] _tmp = datum.Split(new char[] { ' ' });
            string[] tmp = _tmp[0].Split(new char[] { '-' });

            
            try
            {

                result = tmp[2] + "." + tmp[1] + "." + tmp[0];
            }
            catch (Exception e)
            {
                x2log.logData(datum.ToString(), e.ToString(), "datetime error MSDate()  Incorect date in db check db!!");
                result = "chyba:" + e.ToString() + "......." + datum;
            }
        }
       

        return result;
    }

    public string make_hash(string text)
    {
        MD5CryptoServiceProvider hasher = new MD5CryptoServiceProvider();
        System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
        byte[] data = enc.GetBytes(text);
        data = hasher.ComputeHash(data);
        //Convert.
        return Convert.ToBase64String(data);

    }

    public string makeFileHash(string text)
    {
        MD5CryptoServiceProvider hasher = new MD5CryptoServiceProvider();
        System.Text.UTF7Encoding enc = new System.Text.UTF7Encoding();
        byte[] data = enc.GetBytes(text);
        data = hasher.ComputeHash(data);
        //Convert.
        string tmp = Convert.ToBase64String(data);

        tmp = tmp.Replace('\\', '_');
        tmp = tmp.Replace('*', '_');
        tmp = tmp.Replace('?', '_');
        tmp = tmp.Replace('!', '_');
        tmp = tmp.Replace('/', '_');
        tmp = tmp.Replace('+', '_');

        return tmp;

    }

    public bool isAlfaNum(string text)
    {
        Regex myReg = new Regex(@"^[a-zA-Z0-9_]*$");
        bool isValid = false;
        if (myReg.IsMatch(text))
        {
            isValid = true;
        }
        else
        {
            isValid = false;
        }
        
        return isValid;
    }

    public string[][] parseSluzba(string data)
    {
        
        string[] my_list = data.Split(new char[] { '\r' });
        
        string[][] sluzby = new string[my_list.Length][];
        
        for (int i = 0; i < my_list.Length; i++)
        {
            
            string[] lekari = my_list[i].Split(new char[] { ',' });
            sluzby[i] = lekari;           
            
        }


        return sluzby;
    }

    public string[][] parseStaz(string data)
    {

        string[] my_list = data.Split(new char[] { '\r' });

        string[][] sluzby = new string[my_list.Length][];

        for (int i = 0; i < my_list.Length; i++)
        {

            string[] lekari = my_list[i].Split(new char[] { ',' });
            sluzby[i] = lekari;

        }


        return sluzby;
    }

    public string getStr(string text)
    {
        text = text.Trim();
        if (text == "NULL")
        {
            text = "";
        }
        return text;
    }

    public string[][] parseRozpis(string data)
    {

        string[] my_list = data.Split(new char[] { '~' });

        string[][] sluzby = new string[my_list.Length][];

        for (int i = 0; i < my_list.Length; i++)
        {

            string[] lekari = my_list[i].Split(new char[] { '|' });
            sluzby[i] = lekari;

        }


        return sluzby;
    }

    public string mySendMail(string mail_server, SortedList mailData, bool isHtml)
    {
        MailAddress from = new MailAddress(mailData["from"].ToString());
        MailAddress to = new MailAddress(mailData["to"].ToString());
        MailAddress cc = new MailAddress(mailData["cc"].ToString());

        MailMessage sprava = new MailMessage();
        sprava.From = from;
        sprava.To.Add(to);
        sprava.CC.Add(cc);

        sprava.Subject = mailData["subject"].ToString();
        sprava.IsBodyHtml = isHtml;
        sprava.BodyEncoding = Encoding.UTF8;

        sprava.Body = mailData["message"].ToString();

        SmtpClient mail_klient = new SmtpClient(mail_server);
        try
        {
            mail_klient.Send(sprava);
            return "ok";
        }
        catch (Exception e)
        {
            return e.ToString();
        }


    }

    public bool isEmail(string email)
    {
        bool status = false;
        Regex mojeReg = new Regex(@"^(\w[-._\w]*@\w[-._\w]*\w\.\w{2,6})$");
        if (mojeReg.IsMatch(email.Trim()))
        {
            status = true;
        }
        return status;
    }
//Encryption and Decryption of Text in Asp.Net
//By  Krishna Garad January 21, 2011
//http://www.c-sharpcorner.com/UploadFile/krishnasarala/5184/
    public string EncryptString(string Message, string Passphrase)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));
            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            // Step 4. Convert the input string to a byte[]
            byte[] DataToEncrypt = UTF8.GetBytes(Message);
 
            // Step 5. Attempt to encrypt the string 
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            // Step 6. Return the encrypted string as a base64 encoded string
            return Convert.ToBase64String(Results);
        }



        public string DecryptString(string Message, string Passphrase)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));
            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            // Step 4. Convert the input string to a byte[]
            byte[] DataToDecrypt = Convert.FromBase64String(Message);
 
            // Step 5. Attempt to decrypt the string
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            // Step 6. Return the decrypted string in UTF8 format
            return UTF8.GetString(Results);
        }
    //end of //Encryption and Decryption of Text in Asp.Net


        

        public string getVykazName(string full_name)
        {
            char[] del = { ' ' };
            string[] nameStr = full_name.Split(del);

            return nameStr[1];

        }

        

        

        //public MemoryStream createHlaskoPDF(SortedList data)
        //{
        //    MemoryStream msOutput = new MemoryStream();

        //    //BaseColor cervena = new BaseColor(


        //    BaseFont bfHelvetica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, false);
        //    Font nadpis = new Font(bfHelvetica, 14, Font.BOLD, iTextSharp.text.BaseColor.RED);
        //    Font normalB = new Font(bfHelvetica, 11, Font.BOLD, iTextSharp.text.BaseColor.BLACK);
        //    Font normal = new Font(bfHelvetica, 11, Font.NORMAL, iTextSharp.text.BaseColor.BLACK);

            


        //    //TextReader reader = new StringReader(html);
        //    //TextReader reader1 = new Strin

        //    // step 1: creation of a document-object
        //    Document document = new Document(PageSize.A4, 30, 30, 30, 30);

        //    // step 2:
        //    // we create a writer that listens to the document
        //    // and directs a XML-stream to a file
        //    PdfWriter writer = PdfWriter.GetInstance(document, msOutput);

        //    // step 3: we create a worker parse the document
        //    HTMLWorker worker = new HTMLWorker(document);

        //    // step 4: we open document and start the worker on the document
        //    document.Open();
        //    document.NewPage();

        //    //itext.Append(" Hlasenie sluieb:{0}",
        //    //document.Add(new Paragraph("Hlasenie sluieb: datum", nadpis));
        //    //worker.StartDocument();

        //    // step 5: parse the html into the document
        //    //worker.Pa
        //    //worker.pa
           
        //    //worker.Parse(reader);
            
        //    // step 6: close the document and the worker
        //    //worker.EndDocument();
        //    //worker.Close();
        //    document.Close();

        //    return msOutput;
        //}


    


        
}
