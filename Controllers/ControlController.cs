using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace FTBAPISERVER.Controllers
{
    public class ControlController : Controller
    {
        // GET: Control
        public ActionResult Index()
        {
            return View();
        }

        public string GetuserAccess(string Id)
        {
            if (String.IsNullOrEmpty(Id)) return "False";
            string returnstring = "";
            //using (SqlConnection conn = new SqlConnection("Data Source=RCGFTD01.rimkus.net;Initial Catalog=RCGFileMgt;User ID=sa;Password=w[UsDv!b8/sGnMHsgeb^%@y*]8^l"))
            using (SqlConnection conn = new SqlConnection("Data Source=houserveracc.rimkus.net;Initial Catalog=rcg_db;User ID=FTBytes;Password=Ftpbytes@2017"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select FTBSU from ad_users where UPPER(UserID)=@userid", conn);
                cmd.Parameters.AddWithValue("@userid", Id.ToUpper());
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        if (rdr["FTBSU"].ToString() != "0")
                        {
                            returnstring += rdr["FTBSU"].ToString();
                        }
                    }
                }
            }

            //var bola = RimkusHelper.CanAccess("RA000185");
            return returnstring;
        }


        public string GetJobData(string Id)
        {
            if (String.IsNullOrEmpty(Id)) return " ";
            var encodedTextBytes = Convert.FromBase64String(Id);

            string plainText = Encoding.UTF8.GetString(encodedTextBytes);

            string variable;
            using (var connection = new SqlConnection("Data Source=houserveracc.rimkus.net;Initial Catalog=rcg_db;User ID=rcgweb;Password=rcgweb"))
            using (var command = new SqlCommand(plainText, connection))
            {
                //command.Parameters.AddWithValue("@Parameter", someValue);
                connection.Open();
                variable = (string)command.ExecuteScalar();
            }

            List<string> bola = new List<string>();
            bola.Add(variable);
            return JsonConvert.SerializeObject(bola);
        }

        public string GetJobDataSet(string Id)
        {
            try
            {
                if (String.IsNullOrEmpty(Id)) return " ";
                var encodedTextBytes = Convert.FromBase64String(Id);
                var dset = new DataSet();
                string plainText = Encoding.UTF8.GetString(encodedTextBytes);

                string variable;
                using (var connection = new SqlConnection("Data Source=houserveracc.rimkus.net;Initial Catalog=rcg_db;User ID=rcgweb;Password=rcgweb"))
                using (var command = new SqlCommand(plainText, connection))
                {

                    var adp = new System.Data.SqlClient.SqlDataAdapter();
                    //var dset = new DataSet();
                    {
                        adp.SelectCommand = command;
                        adp.Fill(dset, "Results");
                    }
                    //command.Parameters.AddWithValue("@Parameter", someValue);
                    //connection.Open();
                    //variable = (string)command.ExecuteScalar();
                }
                /*var cmd = new System.Data.SqlClient.SqlCommand();
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sqlStatement;
                }
                //.ExecuteReader()        'Forward only Dataset

                //   Create a data adapter to store the inforamtion
                adp = new System.Data.SqlClient.SqlDataAdapter();
                dset = new DataSet();
                {
                    adp.SelectCommand = cmd;
                    adp.Fill(dset, "Results");
                }*/

                //List<string> bola = new List<string>();
                ///bola.Add(variable);
                //return JsonConvert.SerializeObject(bola);
                /*DataSet dataSet = new DataSet("dataSet");
                dataSet.Namespace = "NetFrameWork";
                DataTable table = new DataTable();
                DataColumn idColumn = new DataColumn("id", typeof(int));
                idColumn.AutoIncrement = true;

                DataColumn itemColumn = new DataColumn("item");
                table.Columns.Add(idColumn);
                table.Columns.Add(itemColumn);
                dataSet.Tables.Add(table);

                for(int i = 0; i < 2; i++)
                    {
                        DataRow newRow = table.NewRow();
                        newRow["item"] = "item " + i;
                        table.Rows.Add(newRow);
                    }

                dataSet.AcceptChanges();
                */
                
                string json = JsonConvert.SerializeObject(dset, Formatting.Indented);
                return json;
            }
            catch (Exception)
            {
                return "";
              //  throw;
            }
        }


        public string DataParserFZ(string id, string data)
        {
            /*
             *
             */
            if (id == "") { return "-"; };
            if (id == null) { return "-"; };

            string endo = "";
            double lossvalue = 0;
            string ParseFLAG = "";
            //string QEndorsement ="";
            string conditionall = "";
            string str = id;
            if (str.Contains("}!("))
            {
                conditionall = str.Substring(str.IndexOf("}!(") + 3);
                conditionall = conditionall.Replace(")", "");
                str = id.Substring(0, id.IndexOf("}!(") + 1);
            }
            string key = data;
            string upperStr = id.ToUpper();
            string linedata = "";
            string tempdata = "";
            string[] _fieldSplitter = { };
            bool labelwa = false;
            string labeltext = "";
            if (str.Contains("FFLAGG"))
            {
                ParseFLAG = "FLAG";
                str = str.Replace("FFLAGG", "");
            }

            if (str.Contains("DDESCC"))
            {
                ParseFLAG = "DESC";
                str = str.Replace("DDESCC", "");
            }

            /*
            if (str.Contains("ADDITIONAL1TO3"))
            {
                return getadditional(13, data);
            }

            if (str.Contains("ADDITIONAL4TO5"))
            {
                return getadditional(45, data);
            }
            */

            if (str.Contains("RREPORTT"))
            {
                ParseFLAG = "REPORT";
                str = str.Replace("RREPORTT", "");
            }

            if (str.Contains("LLOOKUPP"))
            {
                ParseFLAG = "LOOKUP";
                str = str.Replace("LLOOKUPP", "");

                /* if (str.Contains("BASICONLY"))
                 { return gettheendos(data, true); }
                 else
                 { return gettheendos(data); }
                 */
            }

            str = str.Replace("^", ":");

            str = str.Replace("[[", "{");
            str = str.Replace("]]", "}");
            str = str.Replace("~", "+");
            int aCounter = 0;
            if (str == "") return "";
            try
            {
                if (str.IndexOf("Date()") > -1)
                {
                    return DateTime.Now.ToString("MM/dd/yyyy");
                }
                if (str.IndexOf("[") == -1)
                {
                    return id;
                }
                string whereclause = "";
                StringBuilder LinQString = new StringBuilder(); // value which is returned as a block
                string _table;
                string _tableblock;
                string _fields;
                string _fieldsSplit;
                string getTrimmed = "";
                string _search;
                string sqlString = "";

                bool conditionalLookup;
                string conditionalLookupstring;
                bool internalLookup;
                string internalLookUpString;
                List<string> _strPDF;
                string mfield = "";
                if (str.IndexOf("(") > 0)
                {
                    string getwhere = str.Substring(str.IndexOf("("));
                    getwhere = getwhere.Substring(1, getwhere.IndexOf(")") - 1);
                    var returnval = "";
                    var returnkey = "";
                    var mygets = getwhere.Split(',');

                    // now get the primary key of all tables - Should be Id
                    string[] primekey = new string[mygets.Length];
                    string[] primetype = new string[mygets.Length];

                    // GET THE KEY OF the FIRST TABLE BY DEFAULT DONT EXPECT IT TO BE ID
                    for (var a = 0; a < mygets.Length; a++)
                    {
                        returnval = mygets[a].Substring(0, mygets[a].IndexOf("."));
                        if (returnval.Contains("="))
                        {
                            primekey[a] = returnval.Substring(returnval.IndexOf("=") + 1);
                            returnval = returnval.Substring(0, returnval.IndexOf("="));
                        }
                        else
                        {
                            _strPDF = GetParseDataFZ("SELECT name FROM sys.columns WHERE object_id = OBJECT_ID('" + returnval + "')");//.ToList();
                            primekey[a] = _strPDF[0];
                        }
                        _strPDF = GetParseDataFZ("SELECT CAST(System_Type_id as nvarchar(MAX))  FROM sys.columns WHERE object_id = OBJECT_ID('" + returnval + "')"); //.ToList();
                        primetype[a] = _strPDF[0];
                        // get the prime key here
                    }
                    for (var a = 0; a < mygets.Length; a++)
                    {
                        returnval = mygets[a].Substring(0, mygets[a].IndexOf("."));
                        if (returnval.Contains("="))
                        {
                            returnval = returnval.Substring(0, returnval.IndexOf("="));
                        }
                        returnkey = mygets[a].Substring(mygets[a].IndexOf(".") + 1);
                        _strPDF = GetParseDataFZ("SELECT CAST(" + "[" + returnkey + "]" + " as nvarchar(MAX)) FROM " + returnval + " WHERE UPPER(CONVERT(varchar(MAX)," + primekey[a] + ")) ='" + key.ToUpper() + "'");//.ToList();
                        key = _strPDF[0];
                    }
                }
                if (str.IndexOf("[") < 0)
                { return "-"; }
                else if (str.IndexOf("{") < 0)
                { return "-"; }
                string[] NumberOfObjects = str.Split(' ');
                //Boolean _concatmode = false;
                var word = str;
                //foreach (string word in NumberOfObjects)
                //{
                try
                {
                    _tableblock = word.Substring(str.IndexOf("["));
                    _tableblock = _tableblock.Substring(1, _tableblock.IndexOf("]") - 1);

                    if (_tableblock.IndexOf(".") == -1) { _tableblock += ".Id"; } // HERE YOU WILL USE THE DEFAUTLT ID OF THE TABLE

                    _table = _tableblock.Substring(0, _tableblock.IndexOf("."));
                    _search = _tableblock.Substring(_tableblock.IndexOf(".") + 1);

                    //was working till 29feb
                    //whereclause = " WHERE CONVERT(varchar(MAX), " + _search + ")='" + key + "'";

                    whereclause = " WHERE UPPER(CONVERT(varchar(MAX), " + _search + "))='" + key.ToUpper() + "'";
                    if (ParseFLAG == "DESC")
                    {
                        whereclause = " WHERE UPPER(CONVERT(varchar(MAX), " + _search + "))='" + key.ToUpper() + "' ORDER BY Id DESC";
                    }
                    _fields = word.Substring(str.IndexOf("{"));
                    _fields = _fields.Substring(1, _fields.IndexOf("}") - 1);
                    _fieldsSplit = _fields.Replace("+", "~+");
                    _fieldsSplit = _fieldsSplit.Replace(",", "~,");
                    _fieldSplitter = _fieldsSplit.Split('~');
                    // why not use a list fieldsplitter = fieldSplit.Split(',').ToList();

                    if (_fields.IndexOf("+") > 0)
                    {
                        _fields = _fields.Replace("+", ",");
                        //_concatmode = true;
                    }
                    string[] _innerField = _fields.Split(',');
                    aCounter = 0;
                    foreach (string field in _innerField)
                    {
                        if (field == "SPACE")
                        {
                            LinQString.Append(" ");
                            if (aCounter < _fieldSplitter.Length - 2)
                            { aCounter++; }
                            continue;
                        }

                        if (field == "DASH")
                        {
                            LinQString.Append("-");
                            if (aCounter < _fieldSplitter.Length - 2)
                            { aCounter++; }
                            continue;
                        }
                        if (field.Contains("LABEL"))
                        {
                            LinQString.Append(field.Replace("LABEL", "") + " ");
                            labelwa = true;
                            labeltext = field.Replace("LABEL", "");
                            if (aCounter < _fieldSplitter.Length - 2)
                            { aCounter++; }
                            continue;
                        }
                        if (field == "SLASH")
                        {
                            LinQString.Append("/");
                            if (aCounter < _fieldSplitter.Length - 2)
                            { aCounter++; }
                            continue;
                        }

                        if (field == "COMMA")
                        {
                            LinQString.Append(", ");
                            if (aCounter < _fieldSplitter.Length - 2)
                            { aCounter++; }
                            continue;
                        }
                        if (field == "LINEFEED")
                        {
                            LinQString.Append("\r\n");
                            if (aCounter < _fieldSplitter.Length - 2)
                            { aCounter++; }
                            continue;
                        }

                        mfield = field;
                        internalLookup = false;
                        internalLookUpString = "";
                        if (field.Contains(":"))
                        {
                            internalLookup = true;
                            internalLookUpString = field.Substring(field.IndexOf(":") + 1);
                            mfield = field.Substring(0, field.IndexOf(":"));
                        }

                        if (ParseFLAG == "REPORT")
                        { sqlString = "SELECT " + "CAST(" + "[" + mfield + "]" + " as nvarchar(MAX)) from " + _table + " " + whereclause; }
                        else if (ParseFLAG == "LOOKUP")
                        {
                            sqlString = "SELECT " + "CAST(" + "[" + mfield + "]" + " as nvarchar(MAX)) from " + _table + " " + whereclause;
                        }
                        else
                        { sqlString = "SELECT TOP 1 " + "CAST(" + "[" + mfield + "]" + " as nvarchar(MAX)) from " + _table + " " + whereclause; }

                        // sqlString = "SELECT TOP 1 " + "CAST(" + field + " as nvarchar(MAX)) from " + _table + " " + whereclause;
                        _strPDF = GetParseDataFZ(sqlString);//ToList();
                        foreach (string test1 in _strPDF)
                        {
                            if (labelwa)
                            {
                                labelwa = false;
                                if (string.IsNullOrEmpty(test1))
                                {
                                    LinQString.Replace(labeltext, "");
                                }
                            }

                            if (test1 != null)
                            {
                                if (test1.Trim() != "")
                                {
                                    tempdata = test1;
                                    if (internalLookup)
                                    {
                                        tempdata = DataParserFZ("[" + internalLookUpString.Replace("-", "]{") + "}",
                                            test1);
                                    }

                                    if (!String.IsNullOrEmpty(conditionall))
                                    {
                                        var bola = conditionall.Split('^');
                                        foreach (var item in bola)
                                        {
                                            // check if the condition is met
                                            var tester = item.Substring(0, item.IndexOf("="));
                                            if (tester == tempdata)
                                            {
                                                // evaluate next
                                                var bolaid = id;
                                                var tempolocator = id.Substring(0, id.IndexOf("]") + 1) + "{" + item.Substring(item.IndexOf("=") + 1) + "}";
                                                tempdata = DataParserFZ(tempolocator, data);
                                            }
                                        }
                                    };

                                    // here you will use the Ken to test conditin and recurvice get

                                    try
                                    {
                                        if (ParseFLAG == "FLAG")
                                        {
                                            tempdata = (test1 == "1") ? "YES" : "NO";
                                        }

                                        if (ParseFLAG == "REPORT")
                                        {
                                            LinQString.Append(tempdata + "\r\n");
                                        }
                                        else if (ParseFLAG == "LOOKUP")
                                        {
                                            endo = GetParseDataFZ("Select crdf_Name from crpdfbank where Id=" +
                                                                         tempdata + "").ToList()[0].ToString();
                                            if (endo.Contains("ES-2402"))
                                            {
                                                lossvalue =
                                                    Convert.ToDouble(
                                                        DataParserFZ(
                                                            "[eID]{LAmount}",
                                                            key));

                                                //endo += " $ "+ DataParser("[DB_LA_HO_Application.QuoteID]{LossAssessmentsAmount}", key);
                                                endo += "                           " +
                                                        String.Format("{0:C}", lossvalue);
                                                endo = endo.Split('.').First();
                                            }
                                            /* old dataSt);
                                            //tempdata = tempdata + "\r\n";

                                            LinQString.Append(
                                                GetParseData("Select pdf_Name from pdfbank where Id=" +
                                                                         tempdata + "").ToList()[0].ToString() +
                                                "\r\n");
                                            */
                                            LinQString.Append(endo + "\r\n");
                                        }
                                        else
                                        {
                                            if (_fieldSplitter[aCounter + 1].IndexOf(",") > -1)
                                            {
                                                LinQString.Append(tempdata + "\r\n");
                                            }
                                            else
                                            {
                                                LinQString.Append(tempdata + " ");
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        LinQString.Append(tempdata + " ");
                                    }
                                }
                                else
                                {
                                    if (_fieldSplitter[aCounter + 1].IndexOf(",") > -1)
                                    {
                                        LinQString.Append("\r\n");
                                    }
                                    else
                                    {
                                        LinQString.Append("");
                                    }
                                }
                            }
                            else
                            {
                                if (_fieldSplitter[aCounter + 1].IndexOf(",") > -1)
                                {
                                    LinQString.Append("\r\n");
                                }
                                else
                                {
                                    LinQString.Append("");
                                }
                            }
                            // if it is null and the the previous contains info

                            // if (_concatmode) { LinQString.Append(test1 + " "); }
                            // else { LinQString.Append(test1 + "\n"); }
                            //test1 ?? "";
                        }
                        if (aCounter < _fieldSplitter.Length - 2)
                        { aCounter++; }
                    }
                }
                catch (Exception e)
                {
                    if (mfield.IndexOf("VERBATIM") > -1)
                    {
                        LinQString.Append("\r\n" + mfield.Replace("VERBATIM", "") + " " + "\r\n");
                        if (aCounter < _fieldSplitter.Length - 2)
                        { aCounter++; }
                    }
                    else
                    {
                        /*
                        if (_fieldSplitter[aCounter + 1].IndexOf(",") > -1)
                        {
                            LinQString.Append(tempdata + "\r\n");
                        }
                        else
                        {
                            LinQString.Append(tempdata + " ");
                        }
                        */

                        LinQString.Append("");
                        linedata += "";
                        if (aCounter < _fieldSplitter.Length - 2)
                        { aCounter++; }
                    }
                    // e.Message);
                }
                //}

                getTrimmed = LinQString.ToString();
                // if (labelwa)
                // {
                //     if (labeltext.Trim() == getTrimmed.Trim()) getTrimmed = "";
                // }

                getTrimmed = getTrimmed.Replace("\r\n\r\n", "\r\n");
                return getTrimmed; //LinQString.ToString();
            }
            catch (Exception e)
            {
                return " "; // "Information Denied";
            }
        }

        public List<string> GetParseDataFZ(string str)
        {
            SqlConnection con = new SqlConnection("Data Source=rcgsql04.rimkus.net;Initial Catalog=rcg_db; MultipleActiveResultSets=True; User ID=rcgweb;Password=rcgweb");
            // SqlConnection con = new SqlConnection("Data Source=houserveracc.rimkus.net;Initial Catalog=rcg_db; MultipleActiveResultSets=True; User ID=FTBytes;Password=Ftpbytes@2017");
            //SqlConnection con = new SqlConnection("Data Source = RCGSQL04-D.rimkus.dev; Initial Catalog =RCGCReport; User ID = sa; Password = Star@1234");

            // _repository.GetParseData(str);

            try
            {
                string formatted = "";
                if (str.Contains("FFFD"))
                {
                    str = str.Replace("FFFD", "");
                    formatted = "date";
                }
                if (str.Contains("FFFU"))
                {
                    str = str.Replace("FFFU", "");
                    formatted = "upper";
                }

                if (str.Contains("FFFL"))
                {
                    str = str.Replace("FFFL", "");
                    formatted = "lower";
                }
                if (str.Contains("FFFP"))
                {
                    str = str.Replace("FFFP", "");
                    formatted = "proper";
                }


                //var bolasql = GetParseData(str);
                //return bolasql;

                SqlCommand SelectCommand = new SqlCommand(str, con);
                SqlDataReader myreader;
                //str = str + " COLLATE SQL_Latin1_General_Cp437_CI_AS_KI_WI";
                List<string> LstReader = new List<string>();
                con.Open();
                myreader = SelectCommand.ExecuteReader();
                var mydata = "";
                List<String> lstEmails = new List<String>();
                while (myreader.Read())
                {
                    if (String.IsNullOrEmpty(myreader[0].ToString()))
                    {
                        mydata = "";
                    }
                    else
                    {
                        if (formatted == "date")
                        {
                            //mydata = myreader[0].ToString();
                            //DateTime asDate = DateTime.ParseExact(mydata,"dd/MM/yyyy hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                            //DateTime asDate = DateTime.ParseExact(mydata,"MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            //mydata = asDate.ToString("MM/dd/yyyy");
                            var boladate = myreader[0].ToString().Replace("  ", " ").Split(' ');
                            var newdate = boladate[0] + "/" + boladate[1] + "/" + boladate[2];
                            //LstReader.Add(myreader[0].ToString());
                            var tola = DateTime.Parse(newdate);
                            //LstReader.Add(tola.ToString("MM/DD/yyyy"));
                            mydata = tola.ToString("MM/dd/yyyy");
                        }
                        else if (formatted == "upper") { mydata = myreader[0].ToString().ToUpper(); }
                        else if (formatted == "lower") { mydata = myreader[0].ToString().ToLower(); }
                        else if (formatted == "proper") { mydata = myreader[0].ToString().ToUpper(); }
                        else
                        {
                            mydata = myreader[0].ToString();
                        };
                    }

                    LstReader.Add(mydata);
                }
                con.Close();
                return LstReader;
            }
            catch (Exception e)
            {
                con.Close();
                var ddd = e.Message;
                return new List<string>();
            }
        }




        public string GetWindowStatus()
        {
            /*
                        var sb = new StringBuilder();
                        PerformanceCounterCategory[] categories = PerformanceCounterCategory.GetCategories();

                        var desiredCategories = new HashSet<string> { "Process", "Memory" };

                        foreach (var category in categories)
                        {
                         //  sb.AppendLine("Category: " + category.CategoryName +Environment.NewLine );
                            if (desiredCategories.Contains(category.CategoryName))
                            {
                                PerformanceCounter[] counters;
                                try
                                {
                                    counters = category.GetCounters("devenv");
                                }
                                catch (Exception)
                                {
                                    counters = category.GetCounters();
                                }

                                foreach (var counter in counters)
                                {
                                    sb.AppendLine(counter.CounterName + ": " + counter.CounterHelp);
                                }
                            }
                        }

                       // return sb.ToString();

                */
            //Process p = /*get the desired process here*/
            //PerformanceCounter ramCounter = new PerformanceCounter("Process", "Working Set", p.ProcessName);
            //PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", p.ProcessName);
            //while (true)
            //{
            //    Thread.Sleep(500);
            //    double ram = ramCounter.NextValue();
            //    double cpu = cpuCounter.NextValue();
            //Console.WriteLine("RAM: " + (ram / 1024 / 1024) + " MB; CPU: " + (cpu) + " %");
            //}

            PerformanceCounter CCounter;
            PerformanceCounter RCounter;

             CCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
             RCounter = new PerformanceCounter("Memory", "Available MBytes");
            CCounter.NextValue(); RCounter.NextValue(); Thread.Sleep(2000);
            return CCounter.NextValue() + "%" + "|" + RCounter.NextValue() + "MB";




            PerformanceCounter cpuCounter;
                        cpuCounter = new PerformanceCounter();
                        cpuCounter.CategoryName = "Processor";
                        cpuCounter.CounterName = "% Processor Time";
                        cpuCounter.InstanceName = "_Total";
                        // Get Current Cpu Usage

                        string currentCpuUsage =cpuCounter.NextValue() + "%";
            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Memory";
                        cpuCounter.CounterName = "% Committed Bytes In Use"; 
                        //cpuCounter.InstanceName = "_Total";
                        // Get Current Cpu Usage
                        //*/
            string currentCpuUsageplus = currentCpuUsage + "|" + cpuCounter.NextValue().ToString();

            return currentCpuUsageplus;
        }
    }
}