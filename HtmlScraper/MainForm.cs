using HtmlAgilityPack;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HtmlScraper
{
    public partial class MainForm : MaterialSkin.Controls.MaterialForm
    {
        public MainForm()
        {
            InitializeComponent();
            splitContainer1.Panel1.ResetText();
            splitContainer1.Panel2.ResetText();
            var skinmanager = MaterialSkin.MaterialSkinManager.Instance;
            skinmanager.AddFormToManage(this);
            skinmanager.Theme = MaterialSkin.MaterialSkinManager.Themes.DARK;
            skinmanager.ColorScheme = new MaterialSkin.ColorScheme(MaterialSkin.Primary.BlueGrey800, MaterialSkin.Primary.BlueGrey900,
                MaterialSkin.Primary.BlueGrey500, MaterialSkin.Accent.LightBlue200, MaterialSkin.TextShade.WHITE);
            splitContainer1.Panel1.ResetText();
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMinutes(60);
            var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
            key.SetValue("ScrapMeNow", Application.ExecutablePath.ToString());
            if (Ping() & Sql())
            {
                MySqlConnection conn = new MySqlConnection("SERVER =88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';");
                conn.Open();
                MySQL_LoadListviewData();
                MySqlCommand com = conn.CreateCommand();
                com.CommandText = "SELECT MAX(ID) FROM production";
                com.ExecuteNonQuery();
                int maxid = Int32.Parse(com.ExecuteScalar().ToString());
                var timer = new System.Threading.Timer((e) =>
                {
                    checknewlinks();
                    if (checknewlinks().Count > 0)
                    {
                        insertProduction();
                        Notification not = new Notification("Επιτυχης εισαγωγη νεας εγγραφής",Color.Green);
                        not.ShowDialog();
                    }
                }, null, startTimeSpan, periodTimeSpan);
            }
            else
            {
                MessageBox.Show("This computer is has an internet or sql issue . Please fix the problem..", "Network/Sql error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
        private bool Ping()
        {
            System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
            System.Net.NetworkInformation.PingReply reply = pingSender.Send("www.google.com");
            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool Sql()
        {
            try
            {
                using (var connection = new MySqlConnection("SERVER =88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';"))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                return false;
            }
        }
        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e) { }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        private void MySQL_LoadListviewData()
        {
            try
            {
                MySqlConnection mysqlCon = new MySqlConnection("SERVER= 88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';");
                mysqlCon.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT ID,OrganizerID,Title,Description,URL,Producer,MediaURL,Duration,SystemID,timestamp FROM production", mysqlCon);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();
                da.Fill(table);

                SetHeight(lvProduction, 250);
                lvProduction.View = System.Windows.Forms.View.Details;
                using (MySqlConnection con = mysqlCon)
                {
                    using (MySqlDataAdapter sda = new MySqlDataAdapter("SELECT * FROM production", con))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        lvProduction.GridLines = true;
                        lvProduction.View = System.Windows.Forms.View.Details;
                        MySqlDataReader rd;
                        rd = cmd.ExecuteReader();
                        lvProduction.Items.Clear();
                        while (rd.Read())
                        {
                            ListViewItem lv = new ListViewItem(rd.GetInt32(0).ToString());
                            lv.SubItems.Add(rd.GetInt32(1).ToString());
                            lv.SubItems.Add(rd.GetString(2));
                            lv.SubItems.Add(rd.GetString(3).ToString());
                            lv.SubItems.Add(rd.GetString(4).ToString());
                            lv.SubItems.Add(rd.GetString(5).ToString());
                            lv.SubItems.Add(rd.GetString(6).ToString());
                            lv.SubItems.Add(rd.GetString(7).ToString());
                            lv.SubItems.Add(rd.GetString(8).ToString());
                            lv.SubItems.Add(rd.GetDateTime(9).ToString());
                            lvProduction.Items.Add(lv);
                        }
                        rd.Close();
                    }
                    cmd = new MySqlCommand("SELECT ID,ProductionID,VenueID,DateEvent,PriceRange,SystemID,timestamp FROM events", con);
                    using (MySqlDataAdapter sda = new MySqlDataAdapter("SELECT * FROM events", con))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        lvEvents.GridLines = true;
                        lvEvents.View = System.Windows.Forms.View.Details;
                        MySqlDataReader rd;
                        rd = cmd.ExecuteReader();
                        lvEvents.Items.Clear();
                        while (rd.Read())
                        {
                            ListViewItem lv = new ListViewItem(rd.GetInt32(0).ToString());
                            lv.SubItems.Add(rd.GetInt32(1).ToString());
                            lv.SubItems.Add(rd.GetInt32(2).ToString());
                            lv.SubItems.Add(rd.GetDateTime(3).ToString());
                            lv.SubItems.Add(rd.GetString(4).ToString());
                            lv.SubItems.Add(rd.GetString(5));
                            lv.SubItems.Add(rd.GetDateTime(6).ToString());
                            lvEvents.Items.Add(lv);
                        }
                        rd.Close();
                    }
                    cmd = new MySqlCommand(@"SELECT ID,Fullname,SystemID,timestamp FROM persons", mysqlCon);
                    using (MySqlDataAdapter sda = new MySqlDataAdapter("SELECT * FROM persons", con))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        lvPeople.GridLines = true;
                        lvPeople.View = System.Windows.Forms.View.Details;
                        MySqlDataReader rd;
                        rd = cmd.ExecuteReader();
                        lvPeople.Items.Clear();
                        while (rd.Read())
                        {
                            ListViewItem lv = new ListViewItem(rd.GetInt32(0).ToString());
                            lv.SubItems.Add(rd.GetString(1));
                            lv.SubItems.Add(rd.GetString(2));
                            lv.SubItems.Add(rd.GetDateTime(3).ToString());
                            lvPeople.Items.Add(lv);
                        }
                        rd.Close();
                    }
                    cmd = new MySqlCommand("SELECT ID,PeopleID,ProductionID,RoleID,subRole,SystemID,timestamp FROM contributions", con);
                    using (MySqlDataAdapter sda = new MySqlDataAdapter("SELECT * FROM contributions", con))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        lvContributions.GridLines = true;
                        lvContributions.View = System.Windows.Forms.View.Details;
                        MySqlDataReader rd;
                        rd = cmd.ExecuteReader();
                        lvContributions.Items.Clear();
                        while (rd.Read())
                        {
                            ListViewItem lv = new ListViewItem(rd.GetInt32(0).ToString());
                            lv.SubItems.Add(rd.GetInt32(1).ToString());
                            lv.SubItems.Add(rd.GetInt32(2).ToString());
                            lv.SubItems.Add(rd.GetInt32(3).ToString());
                            lv.SubItems.Add(rd.GetString(4));
                            lv.SubItems.Add(rd.GetString(5));
                            lv.SubItems.Add(rd.GetDateTime(6).ToString());
                            lvContributions.Items.Add(lv);
                        }
                        rd.Close();
                    }
                    cmd = new MySqlCommand(@"SELECT ID,Title,Address,SystemID,timestamp FROM venue", con);
                    using (MySqlDataAdapter sda = new MySqlDataAdapter("SELECT * FROM venue", con))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        lvVenue.GridLines = true;
                        lvVenue.View = System.Windows.Forms.View.Details;
                        MySqlDataReader rd;
                        rd = cmd.ExecuteReader();
                        lvVenue.Items.Clear();
                        while (rd.Read())
                        {
                            ListViewItem lv = new ListViewItem(rd.GetInt32(0).ToString());
                            lv.SubItems.Add(rd.GetString(1));
                            lv.SubItems.Add(rd.GetString(2));
                            lv.SubItems.Add(rd.GetInt32(3).ToString());
                            lv.SubItems.Add(rd.GetDateTime(4).ToString());
                            lvVenue.Items.Add(lv);
                        }
                        rd.Close();
                    }
                    cmd = new MySqlCommand(@"SELECT ID,Name,Address,Town,postcode,Phone,Email,Doy,Afm,SystemID,timestamp FROM organizer", con);
                    using (MySqlDataAdapter sda = new MySqlDataAdapter("SELECT * FROM organizer", con))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        lvOrganizer.GridLines = true;
                        lvOrganizer.View = System.Windows.Forms.View.Details;
                        MySqlDataReader rd;
                        rd = cmd.ExecuteReader();
                        lvOrganizer.Items.Clear();
                        while (rd.Read())
                        {
                            ListViewItem lv = new ListViewItem(rd.GetInt32(0).ToString());
                            lv.SubItems.Add(rd.GetString(1));
                            lv.SubItems.Add(rd.GetString(2));
                            lv.SubItems.Add(rd.GetString(3));
                            lv.SubItems.Add(rd.GetString(4));
                            lv.SubItems.Add(rd.GetString(5));
                            lv.SubItems.Add(rd.GetString(6));
                            lv.SubItems.Add(rd.GetString(7));
                            lv.SubItems.Add(rd.GetString(8));
                            lv.SubItems.Add(rd.GetString(9));
                            lv.SubItems.Add(rd.GetDateTime(10).ToString());
                            lvOrganizer.Items.Add(lv);
                        }
                        rd.Close();
                    }
                    cmd = new MySqlCommand(@"SELECT ID,Role,SystemID,timestamp FROM roles", con);
                    using (MySqlDataAdapter sda = new MySqlDataAdapter("SELECT * FROM roles", con))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        lvRoles.GridLines = true;
                        lvRoles.View = System.Windows.Forms.View.Details;
                        MySqlDataReader rd;
                        rd = cmd.ExecuteReader();
                        lvRoles.Items.Clear();
                        while (rd.Read())
                        {
                            ListViewItem lv = new ListViewItem(rd.GetInt32(0).ToString());
                            lv.SubItems.Add(rd.GetString(1));
                            lv.SubItems.Add(rd.GetString(2));
                            lv.SubItems.Add(rd.GetDateTime(3).ToString());
                            lvRoles.Items.Add(lv);
                        }
                        rd.Close();
                    }
                }
                mysqlCon.Close();
            }
            catch (System.ArgumentException ex) { }
        }
        private void SetHeight(ListView listView, int height)
        {
            ImageList imgLst = new ImageList();
            imgLst.ImageSize = new Size(60, height);
            listView.SmallImageList = imgLst;
        }
        internal static string RemoveUnwantedTags(string data)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;

            var document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(data);

            var acceptableTags = new String[] { "p", "ul", "li" };

            var nodes = new Queue<HtmlNode>(document.DocumentNode.SelectNodes("./*|./text()"));
            while (nodes.Count > 0)
            {
                var node = nodes.Dequeue();
                var parentNode = node.ParentNode;

                if (!acceptableTags.Contains(node.Name) && node.Name != "#text")
                {
                    var childNodes = node.SelectNodes("./*|./text()");

                    if (childNodes != null)
                    {
                        foreach (var child in childNodes)
                        {
                            nodes.Enqueue(child);
                            parentNode.InsertBefore(child, node);
                        }
                    }
                    parentNode.RemoveChild(node);
                }
            }
            return document.DocumentNode.InnerHtml;
        }

        private void btnRefreshLoad_Click(object sender, EventArgs e)
        {
            MySQL_LoadListviewData();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            Export_Form ef = new Export_Form();
            ef.Show();
        }
        private static void insertProduction()
        {
            int prodid = 0;
            List<string> l = checknewlinks();
            MySqlConnection mysqlCon = new MySqlConnection("SERVER= 88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';");
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            mysqlCon.Open();
            Console.WriteLine("ins production");
            foreach (var line in l)
            {
                Console.WriteLine("line " + line.ToString());
                HtmlAgilityPack.HtmlDocument doc = web.Load(line);
                var container = doc.DocumentNode.SelectNodes("//div[@class='field-group']/span").ToList();
                var orgtitle = doc?.DocumentNode?.SelectSingleNode("//div[@class='playDetailsContainer']/h4");
                var fields = doc?.DocumentNode?.SelectNodes("//div[@class='field']").ToList();
                int counter = 0;
                string venue = "";
                string org = "";
                if (orgtitle != null)
                {
                    org = orgtitle.InnerText;
                }
                else
                {
                    org = "";
                }
                var title = doc?.DocumentNode?.SelectSingleNode("//h1[@id='playTitle']");
                MySqlCommand cmd = mysqlCon.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM organizer where Name LIKE'" + org.TrimStart().TrimEnd() + "'";
                cmd.ExecuteNonQuery();
                int mysqlint = int.Parse(cmd.ExecuteScalar().ToString());
                string url = line.ToString();
                if (mysqlint > 0)
                {
                    //MessageBox.Show("Υπαρχει ηδη η εγγραφη");
                }
                else
                {
                    if (fields != null)
                    {
                        MySqlCommand command = mysqlCon.CreateCommand();
                        command.CommandText = "INSERT INTO organizer(Name,Address,Town,postcode,Phone,Email,Doy,Afm,SystemID) VALUES ('" + org + "','" + fields[0].InnerText.Replace("'", "").TrimStart().TrimEnd()
                            + "','" + fields[1].InnerText.Replace("'", "").TrimStart().TrimEnd() + "','" + fields[2].InnerText.Replace("'", "").TrimStart().TrimEnd() + "','" + fields[3].InnerText.Replace("'", "").TrimStart().TrimEnd() +
                            "','" + fields[4].InnerText.Replace("'", "").TrimStart().TrimEnd() + "','" + fields[5].InnerText.Replace("'", "").TrimStart().TrimEnd() + "','" + fields[6].InnerText.Replace("'", "").TrimStart().TrimEnd() + "','" + 3 + "')";
                        command.ExecuteNonQuery();
                        long id = command.LastInsertedId;
                    }
                }
                var desc = doc?.DocumentNode?.SelectNodes("//div[@itemprop='description']")?.ToList();
                var production = doc?.DocumentNode?.SelectSingleNode("//div[@class='playDetailsContainer']/h4");
                string safety = "";
                if (desc != null)
                {
                    foreach (var m in desc)
                    {
                        safety += m.InnerText;
                    }
                }
                else
                {
                    safety = "No description found..";
                }
                MySqlCommand check_Prod_Name = new MySqlCommand("SELECT * FROM production WHERE URL='" + url + "'", mysqlCon);
                MySqlDataAdapter da = new MySqlDataAdapter(check_Prod_Name);
                DataSet ds1 = new DataSet();
                da.Fill(ds1);
                int i = ds1.Tables[0].Rows.Count;
                string s = safety;
                string result = s.Replace("\"", "`").Replace("/“", "`").Replace("“", "`").Replace("'", "`");
                if (i > 0)
                {
                    //MessageBox.Show("Production already exists", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MySqlCommand mycmd = mysqlCon.CreateCommand();
                    MySqlCommand aa = mysqlCon.CreateCommand();
                    aa.CommandText = "SELECT MAX(ID) FROM organizer";
                    aa.ExecuteNonQuery();
                    MySqlCommand orgId = mysqlCon.CreateCommand();
                    orgId.CommandText = "SELECT ID FROM organizer where Name='" + org + "'";
                    orgId.ExecuteNonQuery();
                    long orgid = long.Parse(orgId.ExecuteScalar().ToString());
                    string ss = result.Replace("&lsquo;", "").Replace("&rsquo;", "").Replace("&ldquo;", "").Replace("&rdquo;", "").TrimStart().TrimEnd();
                    var duration = doc?.DocumentNode?.SelectSingleNode("//li[@class='ui-duration']");
                    mycmd.CommandText = "INSERT INTO `production`(`OrganizerID`, `Title`, `Description`, `URL`, `Producer`, `MediaURL`, `Duration`, `SystemID`) " +
                    "VALUES (@OrganizerID,@Title,@Description,@URL,@Producer,@MediaURL,@Duration,@SystemID)";
                    mycmd.Parameters.AddWithValue("@OrganizerID", orgid);
                    mycmd.Parameters.AddWithValue("@Title", title.InnerText.Replace("&lsquo;", "").Replace("&rsquo;", "").TrimStart());
                    mycmd.Parameters.AddWithValue("@Description", ss);
                    mycmd.Parameters.AddWithValue("@URL", url.TrimStart().TrimEnd());
                    mycmd.Parameters.AddWithValue("@Producer", RemoveEmptyLines(production.InnerText));
                    mycmd.Parameters.AddWithValue("@MediaURL",getMedia(url));
                    mycmd.Parameters.AddWithValue("@Duration", getDuration(line.ToString()));
                    mycmd.Parameters.AddWithValue("@SystemID", 3);
                    mycmd.ExecuteNonQuery();
                }
                safety = "";
                MySqlCommand lk = mysqlCon.CreateCommand();
                lk.CommandText = "SELECT ID FROM production where URL='" + url.TrimStart().TrimEnd() + "'";
                lk.ExecuteNonQuery();
                prodid = Int32.Parse(lk.ExecuteScalar().ToString());
                MySqlCommand idexinevents = mysqlCon.CreateCommand();
                idexinevents.CommandText = "SELECT COUNT(ProductionID) FROM events WHERE ProductionID='" + prodid + "'";
                idexinevents.ExecuteNonQuery();
                long kl = (long)idexinevents.ExecuteScalar();
                if (kl > 0)
                {
                    //Αν υπαρχει ηδη
                }
                else
                {
                     //insertEvent(prodid, url);
                     insertPersons(url, prodid);
                }
            }
            MySqlCommand maxprodId = mysqlCon.CreateCommand();
            maxprodId.CommandText = "SELECT MAX(ID) FROM production";
            maxprodId.ExecuteNonQuery();
            object maxprod = maxprodId.ExecuteScalar();
            mysqlCon.Close();
        }
        private static string getMedia(string link)
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            var experimentalFlags = new List<string>();
            string mediasrc = "";
            ChromeDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl(link);
            chromeOptions.AddLocalStatePreference("browser.enabled_labs_experiments",
                experimentalFlags);
            var cookies = driver.Manage().Cookies.AllCookies;
            driver.FindElement(By.XPath("//a[contains(@class,'cc-btn--accept')]")).Click();
            Thread.Sleep(2000);
            List<IWebElement> cheeses = driver.FindElements(By.Id("openMedia")).ToList();
            if (cheeses.Count > 0)
            {
                driver.FindElement(By.Id("openMedia")).Click();
                Thread.Sleep(3000);
                Boolean isPresent = driver.FindElements(By.ClassName("mfp-img")).ToString().Length > 0;
                var element = driver.FindElements(By.ClassName("mfp-img")).Count >= 1 ? driver.FindElement(By.ClassName("mfp-img")) : null;
                if (element != null)
                {
                    var src = driver.FindElement(By.ClassName("mfp-img"));
                    mediasrc = src.GetAttribute("src");
                    Console.WriteLine(mediasrc);
                }
                else
                {
                    mediasrc = "Not found..";
                }
            }
            driver.Quit();
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(link);
            if (doc?.DocumentNode?.SelectNodes("//*[text()[contains(.,'youtube')]]") != null)
            {
                Console.WriteLine("i am here");
                var container = doc?.DocumentNode?.SelectNodes("//*[text()[contains(.,'youtube')]]")?.ToList();
                string s = "";
                var linkParser = new Regex(@"((https?|ftp|file)\://|www.)[A-Za-z0-9\.\-]+(/[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                foreach (var j in container)
                {
                    s += j.InnerText;
                }
                foreach (Match m in linkParser.Matches(s))
                {
                    if (m.Value.Contains("youtube"))
                    {
                        Console.WriteLine(m.Value);
                        mediasrc = m.Value;
                    }
                }
            }
            return mediasrc;
        }
        private static string getDuration(string link)
        {
            string dur = "";
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(link);
            var duration = doc?.DocumentNode?.SelectSingleNode("//li[@class='ui-duration']");
            if (duration != null)
            {
                var groups = Regex.Match(duration.InnerText, @"[0-9]:[0-5][0-9]").Groups;
                var res = groups[0].Value;
                dur = res.ToString();
                double mins = TimeSpan.Parse(dur).TotalMinutes;
                dur = mins.ToString();
                Console.WriteLine(dur);
            }
            else
            {
                dur = "Not found";
            }

            return dur;
        }
        private static string RemoveEmptyLines(string lines)
        {
            return System.Text.RegularExpressions.Regex.Replace(lines, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline).TrimEnd();
        }
        static void insertEvent(int prodid, string link)
        {
            MySqlConnection mysqlCon = new MySqlConnection("SERVER= 88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';");
            mysqlCon.Open();
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            var experimentalFlags = new List<string>();
            string mediasrc = "";
            ChromeDriver driver = new ChromeDriver(chromeOptions);
            driver.Navigate().GoToUrl(link);
            experimentalFlags.Add("same-site-by-default-cookies@2");
            experimentalFlags.Add("cookies-without-same-site-must-be-secure@2");
            chromeOptions.AddLocalStatePreference("browser.enabled_labs_experiments",
                experimentalFlags);
            var cookies = driver.Manage().Cookies.AllCookies;
            driver.FindElement(By.XPath("//a[contains(@class,'cc-btn--accept')]")).Click();
            Thread.Sleep(2000);
            List<IWebElement> date = driver.FindElements(By.XPath("//div[contains(@class,'events-container__item-date')]")).ToList();
            List<IWebElement> hours = driver.FindElements(By.XPath("//div[contains(@class,'events-container__item-time')]")).ToList();
            List<IWebElement> place = driver.FindElements(By.XPath("//span[contains(@class,'events-container__item-venue')]")).ToList();
            var money = driver.FindElements(By.CssSelector(".events-container__item-prices")).ToList();
            List<double> numbers = new List<double>();
            List<string> prices = new List<string>();
            string price = "";
            foreach (var j in money)
            {
                string[] sep = new string[] { "\r\n" };
                string[] s = j.Text.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                foreach (var h in s)
                {
                    price += h;
                }
                Console.WriteLine(price);
                prices.Add(price);
                price = "";
            }

            for (int p = 0; p < place.Count; p++)
            {
                string s = date[p].Text.Split(' ').Last();
                string days = "";
                string month = "";
                string year = "";
                if (Regex.Match(s, @"[0-9]{1,}(\/)[0-9]{1,}(\/)[0-9]{1,}").Success)
                {
                    days = s.Split('/')[0];
                    month = s.Split('/')[1];
                    year = "2021";
                }
                else if (Regex.Match(s, @"[0-9]{1,}(\/)[0-9]{1,}").Success)
                {
                    days = s.Split('/')[0];
                    month = s.Split('/')[1];
                    year = "2020";
                }
                string date_from = "";
                if (hours[p].Text.Contains(":"))
                {
                    string hour = hours[p].Text.Split(':')[0];
                    string minutes = hours[p].Text.Split(':')[1];
                    string eventvenue = place[p].Text.Split('-')[0].TrimStart().TrimEnd();
                    string eventaddress = place[p].Text.Split('-')[1].TrimStart().TrimEnd();
                    MySqlCommand findcom = new MySqlCommand("SELECT COUNT(*) FROM venue WHERE Title = '" + eventvenue + "'", mysqlCon);
                    findcom.ExecuteNonQuery();
                    long venueexist = (long)findcom.ExecuteScalar();
                    MySqlCommand insvencomm = mysqlCon.CreateCommand();
                    if (venueexist < 1)
                    {
                        insvencomm.CommandText = "INSERT INTO `venue`(`Title`, `Address`, `SystemID`) VALUES ('" + eventvenue + "','" + eventaddress + "','" + 3 + "')";
                        insvencomm.ExecuteNonQuery();
                        MySqlCommand insEvCom = mysqlCon.CreateCommand();
                        long newid = (long)insvencomm.LastInsertedId;
                        DateTime temp = new DateTime(int.Parse(year), Int32.Parse(month), Int32.Parse(days), Int32.Parse(hour), Int32.Parse(minutes), 0);
                        date_from = temp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        insEvCom.CommandText = "INSERT INTO events(ProductionID,VenueID,DateEvent,PriceRange,SystemID) VALUES ('" + prodid + "','" + newid + "','" + date_from + "','" + prices[p] + "','" + 3 + "')";
                        insEvCom.ExecuteNonQuery();
                    }
                    else if (venueexist == 1)
                    {
                        MySqlCommand gmtxm = mysqlCon.CreateCommand();
                        gmtxm.CommandText = "SELECT ID FROM venue WHERE Title = @evven";
                        gmtxm.Parameters.AddWithValue("@evven", eventvenue);
                        var evven = gmtxm.ExecuteScalar();
                        DateTime temp = new DateTime(int.Parse(year), Int32.Parse(month), Int32.Parse(days), Int32.Parse(hour), Int32.Parse(minutes), 0);
                        MySqlCommand insEvent = new MySqlCommand();
                        insEvent.Connection = mysqlCon;
                        date_from = temp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        insEvent.CommandText = "INSERT INTO events(ProductionID,VenueID,DateEvent,PriceRange,SystemID) VALUES ('" + prodid + "','" + evven.ToString() + "','" + date_from + "','" + prices[p] + "','" + 3 + "')";
                        insEvent.ExecuteNonQuery();
                    }
                }
                else
                {
                    MySqlCommand insvencomm = mysqlCon.CreateCommand();
                    insvencomm.CommandText = "INSERT INTO `venue`(`Title`, `Address`, `SystemID`) VALUES ('" + "Online" + "','" + "" + "','" + 3 + "')";
                    insvencomm.ExecuteNonQuery();
                    MySqlCommand insEvCom = mysqlCon.CreateCommand();
                    long newid = (long)insvencomm.LastInsertedId;
                    DateTime temp = new DateTime();
                    date_from = temp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    insEvCom.CommandText = "INSERT INTO events(ProductionID,VenueID,DateEvent,PriceRange,SystemID) VALUES ('" + prodid + "','" + newid + "','" + date_from + "','" + prices[p] + "','" + 3 + "')";
                    insEvCom.ExecuteNonQuery();
                }
            }
            driver.Quit();
            mysqlCon.Close();
        }
        private static void insertContribution(List<string> people, List<string> roles, List<string> subroles, int prodid)
        {
            long roleid = 0;
            int counter = 0, i = 0;
            MySqlConnection mysqlCon = new MySqlConnection("SERVER= 88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';");
            mysqlCon.Open();
            object peopleid = new object();
            try
            {
                for (i = 0; i < roles.Count; i++)
                {
                    if (!people[i].ToString().Contains(",") & !people[i].ToString().Contains("–"))
                    {
                        string[] arr = people[i].Split('-', '–');
                        MySqlCommand insContr = mysqlCon.CreateCommand();
                        for (int p = 0; p < arr.Length; p++)
                        {
                            string s = arr[p].TrimStart().TrimEnd();
                            if (s.Length > 0)
                            {
                                Console.WriteLine(arr[p] + " --- " + roles[i] + " ----" + subroles[i]);
                                MySqlCommand lp = mysqlCon.CreateCommand();
                                lp.CommandText = "SELECT ID FROM persons WHERE Fullname='" + s + "'";
                                lp.ExecuteNonQuery();
                                if (lp.ExecuteScalar() != null)
                                {
                                    peopleid = lp.ExecuteScalar().ToString();
                                    MySqlCommand lr = mysqlCon.CreateCommand();
                                    lr.CommandText = "SELECT ID FROM roles WHERE Role='" + roles[i].ToString() + "'";
                                    lr.ExecuteNonQuery();
                                    roleid = long.Parse(lr.ExecuteScalar().ToString());
                                    MySqlCommand insContr2 = mysqlCon.CreateCommand();
                                    insContr2.CommandText = "INSERT INTO `contributions`(`PeopleID`, `ProductionID`, `RoleID`,`subRole`, `SystemID`) VALUES ('" + Int32.Parse(peopleid.ToString()) + "','" + prodid + "','" + roleid + "','" + subroles[i] + "','" + 3 + "')";
                                    insContr2.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    else if (people[i].ToString().Contains(",") & people[i].ToString().Length > 0)
                    {
                        string[] arr = people[i].Split(',');
                        MySqlCommand insContr = mysqlCon.CreateCommand();
                        for (int p = 0; p < arr.Length; p++)
                        {
                            string s = arr[p].TrimStart().TrimEnd();
                            if (s.Length > 0 & !s.Contains("και") & !s.Contains("@"))
                            {
                                Console.WriteLine(arr[p] + " --- " + roles[i] + " ----" + subroles[i]);
                                MySqlCommand lp = mysqlCon.CreateCommand();
                                lp.CommandText = "SELECT ID FROM persons WHERE Fullname='" + s + "'";
                                lp.ExecuteNonQuery();
                                peopleid = long.Parse(lp.ExecuteScalar().ToString());
                                MySqlCommand lr = mysqlCon.CreateCommand();
                                lr.CommandText = "SELECT ID FROM roles WHERE Role='" + roles[i].ToString() + "'";
                                lr.ExecuteNonQuery();
                                roleid = long.Parse(lr.ExecuteScalar().ToString());
                                MySqlCommand insContr2 = mysqlCon.CreateCommand();
                                insContr2.CommandText = "INSERT INTO `contributions`(`PeopleID`, `ProductionID`, `RoleID`,`subRole`, `SystemID`) VALUES ('" + peopleid + "','" + prodid + "','" + roleid + "','" + subroles[i] + "','" + 3 + "')";
                                insContr2.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (System.ArgumentOutOfRangeException ex) { }
            mysqlCon.Close();
        }
        public static string[] unwanted = { "ΠΡΩΤΑΓΩΝΙΣΤΟΥΝ", "ΠΑΙΖΟΥΝ", "ΑΛΦΑΒΗΤΙΚΑ", "ΔΙΑΝΟΜΗ" , "Παίζουν" , "Θεάτρου", "Θεάτρο","Πρωταγωνιστούν","Συντελεστές:","Συντελεστές",
                    "από", "ΣΥΝΤΕΛΕΣΤΕΣ", "Συμπαραγωγή", "Διανομή", "Ταυτότητα" , "ΑΛΦΑΒΗΤΙΚΑ","Θεάτρου" ,"Προπώληση","Εισιτήρια","Συμπαραγωγή","Πότε","Διάρκεια","ΤΟΥ ΔΗΜΗΤΡΙΟΥ ΒΥΖΑΝΤΙΟΥ","Ευχαριστούμε","*","Διατίθενται"};
        static List<string> checknewlinks()
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load("https://www.viva.gr/tickets/theatre/");
            List<string> theatrelinks = getallproductionlinks();
            List<string> links = new List<string>();
            MySqlConnection mysqlCon = new MySqlConnection("SERVER= 88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';");
            mysqlCon.Open();
            try
            {
                MySqlCommand checknew = mysqlCon.CreateCommand();
                foreach (var j in theatrelinks)
                {
                    checknew.CommandText = "SELECT ID FROM `production` WHERE `URL` LIKE'%" + j.ToString().TrimStart().TrimEnd() + "%'";
                    checknew.ExecuteNonQuery();
                    object venueexist = (object)checknew.ExecuteScalar();
                    if (venueexist != null)
                    {
                        //Console.WriteLine(venueexist.ToString());
                    }
                    else
                    {
                        links.Add(j.ToString());
                    }
                }
            }
            catch (System.InvalidCastException) { }
            foreach (var r in links)
            {
                Console.WriteLine("Βρεθηκε καινουργια παρασταση " + r.ToString());
            }
            mysqlCon.Close();
            return links;
        }
        public static void insertPersons(string link, int prodid)
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(link);
            MySqlConnection mysqlCon = new MySqlConnection("SERVER= 88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';");
            mysqlCon.Open();
            List<string> peoples = new List<string>();
            List<string> proles = new List<string>();
            List<string> subroles = new List<string>();
            List<string> peoplesafter = new List<string>();

            bool exists = false;
            var syntelestesexist = doc.DocumentNode.SelectNodes("//dl[@class='responsive-tabs']").ToList();
            foreach (var x in syntelestesexist)
            {
                if (x.InnerText.TrimStart().TrimEnd().Contains("Συντελεστές"))
                {
                    exists = true;
                }
            }
            var web1 = new HtmlWeb();
            var doc1 = web.Load(link);
            if (exists)
            {
                string test = "";
                string strongtxt = "";
                if (doc1?.DocumentNode?.SelectNodes("//dl[@class='responsive-tabs']/dd/p") != null)
                {
                    foreach (HtmlNode node in doc?.DocumentNode?.SelectNodes("//dl[@class='responsive-tabs']/dd/p"))
                    {
                        strongtxt = RemoveUnwantedTags(node.InnerText.Replace("ΠΑΙΖΟΥΝ (αλφαβητικά)", "Ηθοποιός").Replace("Ερμηνεύουν (με αλφαβητική σειρά)", "Ηθοποιός").Replace("Παίζουν (αλφαβητικά)", "Ηθοποιός").Replace("ΤΑΥΤΟΤΗΤΑ ΠΑΡΑΣΤΑΣΗΣ", "").Replace("&nbsp;", "").Replace("nbsp;", "").Replace("'", " ").Replace("ΠΡΩΤΑΓΩΝΙΣΤΟΥΝ", "Ηθοποιός").Replace("Με τους", "Ηθοποιός")
                            .Replace("Διανομή", "Ηθοποιός").Replace("Παίζουν (αλφαβητικά)", "Ηθοποιός").Replace("-οι ηθοποιοί", "Ηθοποιός").Replace("-οι performers", "Ηθοποιός").Replace(" =:", "").Replace("laquo;", "<").Replace("ΠΑΙΖΟΥΝ:", "*").Replace("Συμμετέχει ο", "Ηθοποιός:").Replace("Βασικοί Συντελεστές:", "")
                            .Replace("ndash;", ",").Replace("ΗΘΟΠΟΙΟΙ", "Ηθοποιός:").Replace("Ερμηνεύουν:", "Ηθοποιός:").Replace("&", "").Replace("raquo;", ">").Replace("και ο", ",").Replace("και η", ",").Replace("&amp", "").Replace("Πρωταγωνιστεί", "Ηθοποιός").Replace("Ερμηνεύει", "Ηθοποιός:")
                            .Replace("Ηθοποιοί", "Ηθοποιός").Replace("Πρωταγωνιστούν:", "Ηθοποιός:").Replace("Παίζουν επίσης", "Ηθοποιός").Replace("ΣΥΜΜΕΤΕΧΟΥΝ", "Ηθοποιός").Replace("Παίζουν οι ηθοποιοί", "Ηθοποιός").Replace("Παίζουν", "Ηθοποιός:").TrimEnd().TrimStart());
                        test += strongtxt + "\n";
                    }
                }
                long countRole = 0;

                using (StringReader reader = new StringReader(test))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Length > 0)
                        {
                            if (Regex.IsMatch(line, @"[\w]{1,}(.*):(.*)[\w]{1,}") & !unwanted.Any(line.Contains))
                            {
                                string role = line.Split(':')[0];
                                string person = line.Split(':')[1].Replace("-", ",").Replace("–", ",");
                                if (!unwanted.Any(line.Contains) & !person.Contains(","))
                                {
                                    peoplesafter.Add(person.TrimStart().TrimEnd());
                                }
                                if (line.Contains(":") & !unwanted.Any(line.Contains))
                                {
                                    if (!(role.Contains(" ") | role.Contains("-") | role.Contains(",")))
                                    {
                                        MySqlCommand roleexist = mysqlCon.CreateCommand();
                                        roleexist.CommandText = "SELECT COUNT(*) FROM roles where Role LIKE'%" + role.TrimStart().TrimEnd() + "%'";
                                        roleexist.ExecuteNonQuery();
                                        int rolecount = int.Parse(roleexist.ExecuteScalar().ToString());
                                        if (rolecount > 0)
                                        {
                                            proles.Add(role);
                                            subroles.Add("");
                                        }
                                        else
                                        {
                                            subroles.Add(role);
                                            proles.Add("Ηθοποιός");
                                        }
                                    }
                                    else if (role.Contains("-") | role.Contains(",") | role.Contains(" "))
                                    {
                                        int counter = 0;
                                        string[] spl = role.Split(new Char[] { ',', '-', ' ' });
                                        foreach (var y in spl)
                                        {
                                            MySqlCommand roleexist = mysqlCon.CreateCommand();
                                            roleexist.CommandText = "SELECT COUNT(*) FROM roles where Role LIKE'%" + y.TrimStart().TrimEnd() + "%'";
                                            roleexist.ExecuteNonQuery();
                                            counter += Int32.Parse(roleexist.ExecuteScalar().ToString());
                                        }
                                        if (counter > 0)
                                        {
                                            proles.Add(role);
                                            subroles.Add("");
                                        }
                                        else
                                        {
                                            subroles.Add(role);
                                            proles.Add("Ηθοποιός");
                                        }
                                    }
                                }
                                if (!person.Contains(',') & !unwanted.Any(person.Contains))
                                {
                                    peoples.Add(person.TrimStart().TrimEnd());
                                }
                                else if (person.Contains(','))
                                {
                                    peoplesafter.Add(person);
                                    string[] arr = person.Split(',');
                                    foreach (var item in arr)
                                    {
                                        if (item.Length > 0)
                                        {
                                            peoples.Add(item.TrimStart());
                                        }
                                    }
                                }
                            }
                            else if (line.Contains(",") & !unwanted.Any(line.Contains))
                            {
                                peoplesafter.Add(line);
                                string[] arr = line.Split(',');
                                foreach (var item in arr)
                                {
                                    if (item.Length > 0)
                                    {
                                        peoples.Add(item.TrimStart());
                                        proles.Add("Ηθοποιός");
                                        subroles.Add("");
                                    }
                                }
                            }
                            else if (line.Length > 1 & !unwanted.Any(line.Contains))
                            {
                                peoplesafter.Add(line);
                                peoples.Add(line);
                                proles.Add("Ηθοποιός");
                                subroles.Add("");
                            }
                        }
                    }
                }
                test = "";
                if (doc1?.DocumentNode?.SelectNodes("//dl[@class='responsive-tabs']/ul/li")?.ToList() != null)
                {
                    Console.WriteLine("μπηκα");
                    var lis = doc1?.DocumentNode?.SelectNodes("//dl[@class='responsive-tabs']/ul/li")?.ToList();
                    foreach (var q in lis)
                    {
                        string r = RemoveUnwantedTags(q.InnerText.Replace("nbsp;", " ").Replace("&nbsp;", "").Replace("'", " ")
                            .Replace("Ταυτότητα", "").Replace("Διανομή", "").Replace("\n", "").Replace("'", "").Replace("&ndash;", "-").Replace("Με τους:", "Ηθοποιός")
                            .Replace("=", "").Replace("=:", "").Replace("amp;", "-").Replace("text=", "").Replace("&", "").Replace("^(?:[\t ]*(?:\r?\n|\r))+", "").TrimStart().TrimEnd());
                        test += r + "\n";
                        Console.WriteLine("r " + q.InnerText);
                    }
                    test = RemoveEmptyLines(test);

                    using (StringReader reader = new StringReader(test))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.ToString().Length > 1 & !line.Any(line.Contains))
                            {
                                if (line.Contains(':'))
                                {
                                    string role = line.Split(':')[0];
                                    string person = line.Split(':')[1];
                                    if (person.Length > 0 & role.Length > 0 & !unwanted.Any(line.Contains) & !person.Contains(',') & !person.Contains('-'))
                                    {
                                        peoplesafter.Add(person.TrimStart().TrimEnd());
                                        proles.Add(role);
                                    }
                                    if (!person.Contains(',') & !unwanted.Any(line.Contains))
                                    {
                                        peoples.Add(person.TrimStart().TrimEnd());
                                    }
                                    else if (person.Contains(','))
                                    {

                                        string[] arr = person.Split(',');
                                        foreach (var item in arr)
                                        {
                                            if (item.Length > 0)
                                                peoples.Add(item.TrimStart().TrimEnd());
                                        }
                                    }
                                    else if (person.Contains('-') | person.Contains("–"))
                                    {
                                        string[] words = person.Split('–', '-');
                                        foreach (var word in words)
                                        {
                                            peoples.Add(word.TrimStart().TrimEnd());
                                        }
                                    }
                                }
                                else if (!unwanted.Any(line.Contains) & !line.Contains(","))
                                {
                                    peoplesafter.Add(line.TrimStart().TrimEnd());
                                    peoples.Add(line.TrimStart().TrimEnd());
                                    proles.Add("Ηθοποιός");
                                }
                                else if (line.Contains(','))
                                {
                                    peoplesafter.Add(line);
                                    string[] arr = line.Split(',');
                                    foreach (var item in arr)
                                    {
                                        if (item.Length > 0)
                                            peoples.Add(item.TrimStart().TrimEnd());
                                        proles.Add("Ηθοποιός");
                                    }
                                }
                            }
                        }
                    }
                }

                if (doc?.DocumentNode?.SelectSingleNode("//tbody") != null)
                {
                    var table = doc?.DocumentNode?.SelectSingleNode("//tbody");
                    var rows = table?.SelectNodes("//tr/td")?.ToList();
                    for (int t = 0; t < rows.Count; t++)
                    {
                        peoples.Add(rows[t].InnerText);
                        peoplesafter.Add(rows[t].InnerText);
                        proles.Add("Ηθοποιός");
                    }
                }

                for (int i = 0; i < proles.Count; i++)
                {
                    proles[i] = proles[i].Replace("Διανομή (με αλφαβητική σειρά)", "Ηθοποιός").Replace("και με αλφαβητική σειρά:", "Ηθοποιός").Replace("Παίζουν επίσης:", "Ηθοποιός").Replace("Παίζουν οι", "Ηθοποιός").Replace("ΗΘΟΠΟΙΟΙ", "Ηθοποιός").Replace("Παίζουν:", "Ηθοποιός:").Replace("ΠΡΩΤΑΓΩΝΙΣΤΟΥΝ", "Ηθοποιός")
                        .Replace("Διανομή (αλφαβητικά)", "Ηθοποιός").Replace("Ερμηνεία", "Ηθοποιός").Replace("Ερμηνεύουν", "Ηθοποιός").Replace("Ερμηνεύει", "Ηθοποιός").Replace("Με τους:", "Ηθοποιός")
                        .Replace("Πρωταγωνιστούν", "Ηθοποιός");
                }

                string[] unwanted2 = { "Ερμηνεύουν","Διατίθενται", "Συντελεστές:" ,"ΣΥΝΤΕΛΕΣΤΕΣ", "ΠΑΙΖΟΥΝ", "Παίζουν","Παίζουν:", "ΔΙΑΝΟΜΗ","Διανομή", "ΑΛΦΑΒΗΤΙΚΑ", "και", "ΠΡΩΤΑΓΩΝΙΣΤΟΥΝ", "Θεάτρου", "Θεάτρο", "Διατίθενται","από",
                "Συμπαραγωγή","Ταυτότητα","Ένας","Διατίθενται θέσεις καθήμενων","ΤΑΥΤΟΤΗΤΑ ΠΑΡΑΣΤΑΣΗΣ","Οι ηθοποιοί","Πού","Πότε","ΤΟΥ ΔΗΜΗΤΡΙΟΥ ΒΥΖΑΝΤΙΟΥ","Ώρες","Εισιτήρια","Προπώληση","ΤΟΥ ΔΗΜΗΤΡΙΟΥ ΒΥΖΑΝΤΙΟΥ","Η ΒΑΒΥΛΩΝΙΑ"};

                for (int h = 0; h < unwanted2.Length; h++)
                {
                    peoples.RemoveAll(u => u.Contains(unwanted2[h].ToString()));
                    peoplesafter.RemoveAll(u => u.Contains(unwanted2[h].ToString()));
                    proles.RemoveAll(u => u.Contains(unwanted2[h].ToString()));
                    subroles.RemoveAll(u => u.Contains(unwanted2[h].ToString()));
                }
                peoples.RemoveAll(u => u.Contains("Ηθοποιός"));
                peoplesafter.RemoveAll(u => u.Contains("Ηθοποιός"));
                peoples.RemoveAll(u => u.Contains("Το") & u.Length == 2);
                peoples.RemoveAll(u => u.Contains("το") & u.Length == 2);
                peoples.RemoveAll(u => u.Contains("@"));

                for (int t = 0; t < peoplesafter.Count; t++)
                {
                    if (peoplesafter[t] == string.Empty || peoplesafter[t].Length <= 2) peoplesafter.RemoveAt(t);
                }
                for (int pp = 0; pp < peoples.Count; pp++)
                {
                    string s = peoples[pp];
                    if (unwanted.Any(s.Contains) | s.Length == 0)
                    {
                        peoples.RemoveAt(pp);
                    }
                }
                for (int pa = 0; pa < peoplesafter.Count; pa++)
                {
                    string s = peoplesafter[pa];
                    if (unwanted.Any(s.Contains) | s.Length == 0)
                    {
                        peoplesafter.RemoveAt(pa);
                    }
                }
                for (int pr = 0; pr < proles.Count; pr++)
                {
                    string s = proles[pr];
                    if (unwanted.Any(s.Contains) | s.Length == 0)
                    {
                        proles.RemoveAt(pr);
                    }
                }
            }
            string[] arrpeoples = peoples.ToArray();
            long count = 0;
            for (int u = 0; u < arrpeoples.Length; u++)
            {
                string fullname = arrpeoples[u].TrimStart().TrimEnd();
                MySqlCommand findPerson = mysqlCon.CreateCommand();
                findPerson.CommandText = "SELECT COUNT(*) FROM persons WHERE Fullname='" + fullname + "'";
                count = (long)findPerson.ExecuteScalar();
                if (fullname.Length > 0 & count == 0)
                {
                    MySqlCommand insPerson = mysqlCon.CreateCommand();
                    insPerson.CommandText = "INSERT INTO persons(Fullname,SystemID) VALUES ('" + fullname + "','" + "3" + "')";
                    insPerson.ExecuteNonQuery();
                }
            }
            string[] rolesarray = proles.Distinct().ToArray();
            for (int p = 0; p < rolesarray.Length; p++)
            {
                MySqlCommand checkcmd = mysqlCon.CreateCommand();
                checkcmd.CommandText = "SELECT COUNT(*) FROM roles where Role LIKE'" + rolesarray[p].TrimStart().TrimEnd() + "'";
                checkcmd.ExecuteNonQuery();
                int mysqlint = int.Parse(checkcmd.ExecuteScalar().ToString());
                if (mysqlint > 0)
                {
                    //MessageBox.Show("Role already exists");
                }
                else
                {
                    MySqlCommand command = mysqlCon.CreateCommand();
                    command.CommandText = "INSERT INTO `roles`(`Role`, `SystemID`) VALUES ('" + rolesarray[p].TrimStart().TrimEnd() + "','" + "3" + "')";
                    command.ExecuteNonQuery();
                }
            }
            insertContribution(peoplesafter, proles, subroles, prodid);
            mysqlCon.Close();
        }

        static List<string> getallproductionlinks()
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load("https://www.viva.gr/tickets/theatre/");
            List<string> theatrelinks = new List<string>();
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];

                if (att.Value.Contains("a") & (att.Value.StartsWith("/tickets/theatre") | att.Value.StartsWith("/tickets/theater") | (att.Value.StartsWith("/tickets/stand-up-comedy"))))
                {
                    theatrelinks.Add("https://www.viva.gr" + att.Value);
                }
            }
            return theatrelinks;
        }

        private void btnNotification_Click(object sender, EventArgs e)
        {
            
        }

        private void btnSuccess_Click(object sender, EventArgs e){ }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if(FormWindowState.Minimized == WindowState)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
           Show();
        }
    }
}
