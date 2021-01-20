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
            //Διαγραφή κειμένου απο τα panel 
            splitContainer1.Panel1.ResetText();
            splitContainer1.Panel2.ResetText();
            //<--- Ορισμός θέματος φορμας και χρώματων των οποίων θα χρωματισουν σημεία της 
            var skinmanager = MaterialSkin.MaterialSkinManager.Instance;
            skinmanager.AddFormToManage(this);
            skinmanager.Theme = MaterialSkin.MaterialSkinManager.Themes.DARK;
            skinmanager.ColorScheme = new MaterialSkin.ColorScheme(MaterialSkin.Primary.BlueGrey800, MaterialSkin.Primary.BlueGrey900,
                MaterialSkin.Primary.BlueGrey500, MaterialSkin.Accent.LightBlue200, MaterialSkin.TextShade.WHITE);
            splitContainer1.Panel1.ResetText();// <--

            //-->ορισμός μεταβλητων για τον timer που θα εκτελειται καθε μία ωρα
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMinutes(60);//<--
            //--> προσθήκη της εφαρμογής στα προγράμματα που εκκινούν με το ανοιγμα του υπολογιστή
            var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
            key.SetValue("ScrapMeNow", Application.ExecutablePath.ToString());// <---
            //-->Ελεγχος αν ειναι κανονικα συνδεδεμενη η εφαρμογη με το ιντερνετ ΚΑΙ με την sql βάση
            if (Ping() & Sql())
            {
                MySQL_LoadListviewData();//καλεσμα μεθοδου φόρτωσης των list views
                //δημιουργια timer οπου κάνει έλεγχο κάθε μια ώρα περιοδικά 
                var timer = new System.Threading.Timer((e) =>
                {
                    checknewlinks();//΄κλήση μεθόδου που επιστρεφει λίστα των links των νεων παραστασεων
                    if (checknewlinks().Count > 0)//αν τα link ειναι παραπανω απο 0 τοτε καλεσε την εισαγωγη της παραστασης και εμφανισε με την χρήση της notification φορμας ειδοποιηση επιτυχης εισαγωγής στον χρήστη
                    {
                        insertProduction();
                        Notification not = new Notification("Επιτυχης εισαγωγη νεας εγγραφής",Color.Green);
                        not.ShowDialog();
                    }
                }, null, startTimeSpan, periodTimeSpan);
            }
            else // αλλιως εμφανισε μηνυμα λάθους και τερματισε το προγραμμα
            {
                MessageBox.Show("This computer is has an internet or sql issue . Please fix the problem..", "Network/Sql error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
        private bool Ping()
        {
            System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();//στελνουμε πακετα στην παρακατω διευθυνση για να δουμε αν παρουμε απαντηση
            System.Net.NetworkInformation.PingReply reply = pingSender.Send("www.google.com");
            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)//αν υπαρχει απαντηση τοτε επεστρεψε αληθες
            {
                return true;
            }
            else //αλλλιως επεστρεψε λαθος
            {
                return false;
            }
        }
        private bool Sql()
        {
            try
            {
                //ελεγχος συνδεσιμοτητας με την online βαση mysql με εγκαθιδρηση μεταβλητης connection με χρηση exception για αποφυγη σφαλματος την ωρα της εκτελεσης 
                using (var connection = new MySqlConnection("SERVER =88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';"))
                {
                    connection.Open();
                    return true;//επεστρεψε αληθες
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                return false;//επεστρεψε λαθος
            }
        }
        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e) { }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();//με το κλείσιμο της κεντρικής φορμας εντολή για τερματισμο προγράμματος
        }
        private void MySQL_LoadListviewData()
        {
            //δημιουργια συνδεσης με την βαση
            MySqlConnection mysqlCon = new MySqlConnection("SERVER =88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';");
            mysqlCon.Open();//ανοιγμα συνδεσης 
            MySqlCommand cmd = new MySqlCommand(@"SELECT ID,OrganizerID,Title,Description,URL,Producer,MediaURL,Duration,SystemID,timestamp FROM production", mysqlCon);//δημιουργια αντικειμενου που εκτελει μια mysql εντολη
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);//δημιουργια αντικειμενου που παιρνει το sql αποτελεσμα
            DataTable table = new DataTable();//δημιουργια αντικειμενου για γεμισμα των αποτελεσματων του adapter ωστε να μπορουμε να προσπελασουμε στα δεδομενα
            da.Fill(table);//γεμισμα του table
            //μεθοδος για επεξεργασια του μεγεθους κελιου των List View 
            SetHeight(lvProduction, 250);
            using (MySqlConnection con = mysqlCon)
            {
                using (MySqlDataAdapter sda = new MySqlDataAdapter("SELECT * FROM production", con))
                {
                    DataTable dt = new DataTable();//δημιουργια ενος datatable
                    sda.Fill(dt);//γεμισμα του table με το sql command που επιλεγει ολα τα πεδια απο το production table
                    lvProduction.GridLines = true;//ενεργοποιηση ιδιοτητας για εμφανιση γραμμων μεταξυ των εγγραφων για να τα ξεχωριζουμε πιο ευκολα
                    lvProduction.View = System.Windows.Forms.View.Details;//επιλογη τροπου με τον οποιο θελουμε να εμφανιζονται τα δεδομενα στο list view
                    MySqlDataReader rd;//αντικειμενο διασματος ροης sql γραμμων εντολης
                    rd = cmd.ExecuteReader();//ενσωματονουμε το αποτελεσμα της mysql εντολης στον reader
                    lvProduction.Items.Clear();//καθαριζουμε τα αντικειμενα που υπηρχαν ηδη στο list view
                    while (rd.Read())//για οσο διαβαζεις mysql γραμμες , εχω προσθεσει τα πεδια του πινακα , και μεσω παραμετρων παιρνω τις τιμες και τις προσθετω στο αναλογο πεδιο, τελος το προσθετω και προχωραω στην επομενη εγγραφη,το τελος ο datareader πρεπει να κλεισει
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
                    rd.Close();//η ιδια λογικη γινεται και στα επομενα list views
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
        private void SetHeight(ListView listView, int height)//μεθοδος για αυξηση μεγεθους κελιου του listview
        {
            ImageList imgLst = new ImageList();//δημιουργια κενου αντικειμενου εικονας
            imgLst.ImageSize = new Size(60, height);//ορισμος μεγεθους εικονας με τιμη και παραμετρο
            listView.SmallImageList = imgLst;//περναμε σαν ιδιοτητα το imaglist με το μεγεθος του για να μεγαλωσουμε το κελι
        }
        private static string RemoveUnwantedTags(string data)//μεθοδος για καθαρισμα html κειμενου απο ανεπιθυμητες σημανσεις-συμβολοσειρες-στοιχεια
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;//αν η παραμετρος ειναι κενη επεστρεψε κενο string

            var document = new HtmlAgilityPack.HtmlDocument();//δημιουργια ενος html document 
            document.LoadHtml(data);//φορτωση string html αρχειου με την μεθοδο load html

            var acceptableTags = new String[] { "p", "ul", "li" };//τα tags Που θελω να διαβαζει

            var nodes = new Queue<HtmlNode>(document.DocumentNode.SelectNodes("./*|./text()"));//επελεξε τους κομβους οπου περιεχουν κειμενο
            while (nodes.Count > 0)
            {
                var node = nodes.Dequeue();//παιρνει το πρωτο στην σειρα αντικειμενο 
                var parentNode = node.ParentNode;//παιρνουμε το κομβο πατερα

                if (!acceptableTags.Contains(node.Name) && node.Name != "#text")
                {//αν ο κομβος δεν περιεχει καποιο απο τα tags που θελουμε και το id του κομβου δεν ειναι text τοτε..
                    var childNodes = node.SelectNodes("./*|./text()");//επελεξε τους κομβους με κειμενο

                    if (childNodes != null)
                    {
                        foreach (var child in childNodes)
                        {
                            nodes.Enqueue(child);//εισηγαγε το παιδι κομβος στην ουρα 
                            parentNode.InsertBefore(child, node);//εισηγαγε στον κομβο πατερα το παιδι 
                        }
                    }
                    parentNode.RemoveChild(node);//διαγραφω τον κομβο που εξετασα για να παρω τον επομενο μεχρι να τελειωσουν
                }
            }
            return document.DocumentNode.InnerHtml;//επιστροφη του αποτελεσματος σε string
        }

        private void btnRefreshLoad_Click(object sender, EventArgs e)//η μεθοδος που καλειται οταν το refresh/load button πατηθει
        {
            MySQL_LoadListviewData();//μεθοδος για ανανεωση περιεχομενου των listview 
        }

        private void btnExport_Click(object sender, EventArgs e)//η μεθοδος που καλειται οταν το export button πατηθει
        {
            Export_Form ef = new Export_Form();//δημιουργια ενος export form αντικειμενου
            ef.Show();//εμφανιση της φορμας
        }
        private void insertProduction()
        {
            int prodid = 0;//κενη μεταβλητη που θα την χρησιμοποιησω αργοτερα
            List<string> l = checknewlinks();//παιρνουμε ολα τα καινουργια link για εισαγωγη
            MySqlConnection mysqlCon = new MySqlConnection("SERVER =88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';");//συνδεση mysql
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();//δημιοργια αντικειμενου που παιρνει το html εγγραφο απο το διαδικτυο 
            mysqlCon.Open();
            foreach (var line in l)
            {
                HtmlAgilityPack.HtmlDocument doc = web.Load(line);//φορτωση ιστοτοπου
                var orgtitle = doc?.DocumentNode?.SelectSingleNode("//div[@class='playDetailsContainer']/h4");//παιρνουμε το κομβο ονομα του οργανωτη
                var fields = doc?.DocumentNode?.SelectNodes("//div[@class='field']").ToList();//παιρνουμε τα χαρακτηριστικα του οργανωτη
                string venue = "";//κενο string για χρηση 
                string org = "";//κενο string για χρηση 
                if (orgtitle != null)
                {
                    org = orgtitle.InnerText;//αν δεν ειναι null τοτε βαλε στο org την τιμη
                }
                else
                {
                    org = "";//αλλιως ειναι κενο
                }
                var title = doc?.DocumentNode?.SelectSingleNode("//h1[@id='playTitle']");//τιτλος παραστασης
                MySqlCommand cmd = mysqlCon.CreateCommand();//sql command
                cmd.CommandText = "SELECT COUNT(*) FROM organizer where Name LIKE'" + org.TrimStart().TrimEnd() + "'";//mysql command για να ελενξουμε αν υπαρχει ηδη ο οργανωτης
                cmd.ExecuteNonQuery();//εκτελεση του command
                int mysqlint = int.Parse(cmd.ExecuteScalar().ToString());//αποθηκευουμε το αποτελεσμα σε int μεταβλητη για ελεγχο
                string url = line.ToString();
                if (mysqlint > 0)
                {
                    //MessageBox.Show("Υπαρχει ηδη η εγγραφη");
                }
                else
                {
                    if (fields != null)//εδω ουσιαστικα γινεται η εγγραφη των οργανωτων που δεν υπαρχουν ηδη
                    {
                        MySqlCommand command = mysqlCon.CreateCommand();//τα πεδια του οργανωτη ειναι συγκεκριμενα γι αυτο χρησιμοποιω τους αριθμους για να παρω την ακριβη θεση της καθε τιμης
                        command.CommandText = "INSERT INTO organizer(Name,Address,Town,postcode,Phone,Email,Doy,Afm,SystemID) VALUES ('" + org + "','" + fields[0].InnerText.Replace("'", "").TrimStart().TrimEnd()
                            + "','" + fields[1].InnerText.Replace("'", "").TrimStart().TrimEnd() + "','" + fields[2].InnerText.Replace("'", "").TrimStart().TrimEnd() + "','" + fields[3].InnerText.Replace("'", "").TrimStart().TrimEnd() +
                            "','" + fields[4].InnerText.Replace("'", "").TrimStart().TrimEnd() + "','" + fields[5].InnerText.Replace("'", "").TrimStart().TrimEnd() + "','" + fields[6].InnerText.Replace("'", "").TrimStart().TrimEnd() + "','" + 3 + "')";
                        command.ExecuteNonQuery();//εκτελεση του query
                    }
                }
                var desc = doc?.DocumentNode?.SelectNodes("//div[@itemprop='description']")?.ToList();//η περιγραφη της παραστασης-χρησιμοποιω ? σε περιπτωση που ειναι κενη
                string safety = "";//κενο string για χρηση
                if (desc != null)
                {
                    foreach (var m in desc)
                    {
                        safety += m.InnerText;//αν δεν ειναι κενο το description γεμισε το string με το περιεχομενο του 
                    }
                }
                else
                {
                    safety = "No description found..";//αλλιως ορισε το πως δεν βρηκε περιγραφη
                }
                MySqlCommand check_Prod_Name = new MySqlCommand("SELECT * FROM production WHERE URL='" + url + "'", mysqlCon);//query για να ελενξουμε αν υπαρχει ηδη η παρασταση
                MySqlDataAdapter da = new MySqlDataAdapter(check_Prod_Name);
                DataSet ds1 = new DataSet();
                da.Fill(ds1);
                int i = ds1.Tables[0].Rows.Count;//παιρνουμε τον αριθμο τον αποτελεσματων
                string s = safety;
                string result = s.Replace("\"", "`").Replace("/“", "`").Replace("“", "`").Replace("'", "`");//αποφυγη ειδικων χαρακτηρων που προκαλουν error στην λειτουργια και στην εισαγωγη
                if (i > 0)//αν υπαρχει ηδη η παρασταση τοτε..
                {
                    //MessageBox.Show("Production already exists", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else//αλλιως προχωρα στην εισαγωγη της
                {
                    MySqlCommand mycmd = mysqlCon.CreateCommand();//query για την εισαγωγη της παραστασης
                    MySqlCommand orgId = mysqlCon.CreateCommand();//query για να παρουμε τον οργανωτη
                    orgId.CommandText = "SELECT ID FROM organizer where Name='" + org + "'";//παιρνουμε το id του οργανωτη που βρηκαμε απο πριν
                    orgId.ExecuteNonQuery();
                    long orgid = long.Parse(orgId.ExecuteScalar().ToString());//βαζουμε το id σε μεταβλητη
                    string ss = result.Replace("&lsquo;", "").Replace("&rsquo;", "").Replace("&ldquo;", "").Replace("&rdquo;", "").TrimStart().TrimEnd();//αντικατασταση ανεπιθυμητων χαρακτηρων 
                    mycmd.CommandText = "INSERT INTO `production`(`OrganizerID`, `Title`, `Description`, `URL`, `Producer`, `MediaURL`, `Duration`, `SystemID`) " +
                    "VALUES (@OrganizerID,@Title,@Description,@URL,@Producer,@MediaURL,@Duration,@SystemID)";
                    mycmd.Parameters.AddWithValue("@OrganizerID", orgid);
                    mycmd.Parameters.AddWithValue("@Title", title.InnerText.Replace("&lsquo;", "").Replace("&rsquo;", "").TrimStart());
                    mycmd.Parameters.AddWithValue("@Description", ss);
                    mycmd.Parameters.AddWithValue("@URL", url.TrimStart().TrimEnd());
                    mycmd.Parameters.AddWithValue("@Producer", RemoveEmptyLines(org));
                    mycmd.Parameters.AddWithValue("@MediaURL",getMedia(url));
                    mycmd.Parameters.AddWithValue("@Duration", getDuration(line.ToString()));
                    mycmd.Parameters.AddWithValue("@SystemID", 3);
                    mycmd.ExecuteNonQuery();//εκτελεση query εισαγωγης παραστασης
                }
                safety = "";
                MySqlCommand lk = mysqlCon.CreateCommand();
                lk.CommandText = "SELECT ID FROM production where URL='" + url.TrimStart().TrimEnd() + "'";//query για να παρω το url της παραστασης
                lk.ExecuteNonQuery();
                prodid = Int32.Parse(lk.ExecuteScalar().ToString());//αποθηκευω το id στην μεταβλητη
                MySqlCommand idexinevents = mysqlCon.CreateCommand();
                idexinevents.CommandText = "SELECT COUNT(ProductionID) FROM events WHERE ProductionID='" + prodid + "'";//ελεγχω αν το id της παραστασης υπαρχει μεσα στο event table
                idexinevents.ExecuteNonQuery();//εκτελεση του query
                long kl = (long)idexinevents.ExecuteScalar();
                if (kl > 0)
                {
                    //Αν υπαρχει ηδη
                }
                else//προχωρησε στην κληση των μεθοδων
                {
                     insertEvent(prodid, url);
                     insertPersons(url, prodid);
                }
            }
            mysqlCon.Close();//κλεισιμο της mysql συνδεσης
        }
        private static string getMedia(string link)//μεθοδος για να παρουμε το link εικονας/βιντεο παραστασης με το selenium library
        {
            var chromeOptions = new ChromeOptions();//δημιουργια αντικειμενου για να περασουμε τις επιλογες που θελουμε για το chrome driver
            chromeOptions.AddArguments("headless");//επιλογη ωστε το chrome driver να δουλευει χωρις κεφαλη
            var experimentalFlags = new List<string>();
            string mediasrc = "";//μεταβλητη που θα την γεμισουμε μετα
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;//κρυψιμο του prompt του selenium
            ChromeDriver driver = new ChromeDriver(driverService,chromeOptions);//δημιουργια chrome driver αντικειμενου και περασμα παραμετρων
            driver.Navigate().GoToUrl(link);
            chromeOptions.AddLocalStatePreference("browser.enabled_labs_experiments",
                experimentalFlags);
            var cookies = driver.Manage().Cookies.AllCookies;
            Boolean cookieisenabled = driver.FindElement(By.XPath("//a[contains(@class,'cc-btn--accept')]")).Displayed;//ελεγχος αν υπαρχει το στοιχειο με boolean μεταβλητη
            if (cookieisenabled) {//αν ειναι ενεργοποιημενα τα cookies τοτε αποδεξου τα
                driver.FindElement(By.XPath("//a[contains(@class,'cc-btn--accept')]")).Click();
            }
            Thread.Sleep(2000);//δινουμε λιγο χρονο στον  chrome driver για να επιλεξει το openmedia tab αν αυτο υπαρχει
            //Boolean mediaisenabled = false;
            /*
            if (driver.FindElements(By.Id("openMedia")).Count != 0 && driver.FindElement(By.Id("openMedia")).Enabled)
            {
                 mediaisenabled = driver.FindElement(By.Id("openMedia")).Enabled;
            }*/
            Boolean mediaisenabled = driver.FindElement(By.Id("openMedia")).Displayed;//ελεγχος αν υπαρχει το στοιχειο με boolean μεταβλητη
            //αν υπαρχει το media 
            if (mediaisenabled)
            {
                driver.FindElement(By.Id("openMedia")).Click();
                //driver.FindElement(By.Id("openMedia")).Click();//κανε κλικ στο tab - φορτωνεται με javascript ενα παραθυρο
                Thread.Sleep(3000);//δωσε χρονο στον driver να φορτωσει ωστε να αναζητησει τα στοιχεια που θελουμε
                var element = driver.FindElements(By.ClassName("mfp-img")).Count >= 1 ? driver.FindElement(By.ClassName("mfp-img")) : null;//ελεγχος μεσω μεταβλητης αν υπαρχει στοιχειο με κλαση mfp-img, αν δεν υπαρχει επεστρεψε null 
                if (element != null)
                {
                    var src = driver.FindElement(By.ClassName("mfp-img"));//αποθηκευση σε μεταβλητη ο κομβος με κλαση mfp-img 
                    mediasrc = src.GetAttribute("src");//παρε την τιμη του χαρακτηριστικου src 
                }
                else//αλλιως δεν βρεθηκε εικονα
                {
                    mediasrc = "Not found..";
                }
            }
            driver.Quit();//κλεισιμο του selenium driver
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(link);//φορτωση του link για να παρουμε το html εγγραφο 
            if (doc?.DocumentNode?.SelectNodes("//*[text()[contains(.,'youtube')]]") != null)//παιρνοντας ολο το κειμενο της παραστασης , αν αυτο περιεχει youtube τοτε
            {
                var container = doc?.DocumentNode?.SelectNodes("//*[text()[contains(.,'youtube')]]")?.ToList();//παρε τους κομβους που περιεχουν την λεξη youtube
                string s = "";//μεταβλητη string για χρηση
                var linkParser = new Regex(@"((https?|ftp|file)\://|www.)[A-Za-z0-9\.\-]+(/[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*", RegexOptions.Compiled | RegexOptions.IgnoreCase);//μεσω regex παιρνουμε καθε link του κειμενου
                foreach (var j in container)
                {
                    s += j.InnerText;//για καθε string που περιεχει youtube προσθεσε το στο string s 
                }
                foreach (Match m in linkParser.Matches(s))
                {
                    if (m.Value.Contains("youtube"))//αν το link περιεχει την λεξη youtube τοτε βαλτο στην μεταβλητη
                    {
                        mediasrc = m.Value;
                    }
                }
            }
            return mediasrc;//επεστρεψε το link
        }
        private static string getDuration(string link)//μεθοδος που επιστρεφει την διαρκεια της παραστασης σε λεπτα 
        {
            string dur = "";
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(link);
            var duration = doc?.DocumentNode?.SelectSingleNode("//li[@class='ui-duration']");//κομβος που εχει την διαρκεια σε ωρες 
            if (duration != null)
            {
                var groups = Regex.Match(duration.InnerText, @"[0-9]:[0-5][0-9]").Groups;//παιρνουμε απο το string Moνο το στοιχειο που μπορει να εχει την μορφη πχ 1:53
                var res = groups[0].Value;
                dur = res.ToString();//το βαζουμε σε μεταβλητη
                double mins = TimeSpan.Parse(dur).TotalMinutes;//το μετατρεπουμε σε λεπτα
                dur = mins.ToString();//το βαζουμε ξανα σε μεταβλητη
                Console.WriteLine(dur);//το επιστρεφουμε
            }
            else//αλλιως
            {
                dur = "Not found";//δεν βρεθηκε
            }

            return dur;
        }
        private static string RemoveEmptyLines(string lines)//μεθοδος για να καθαρισουμε τις κενες γραμμες
        {
            return System.Text.RegularExpressions.Regex.Replace(lines, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline).TrimEnd();
        }
        void insertEvent(int prodid, string link)//μεθοδος για εισαγωγη προβολων παραστασεων και θεατρων
        {
            MySqlConnection mysqlCon = new MySqlConnection("SERVER =88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';");//συνδεση με την mysql βαση
            mysqlCon.Open();//ανοιγμα της συνδεσης
            var chromeOptions = new ChromeOptions();//ρυθμισεις του chrome driver
            chromeOptions.AddArguments("headless");//επιλογη ωστε το chrome driver να δουλευει χωρις κεφαλη
            var experimentalFlags = new List<string>();
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;//ρυθμιση ετσι ωστε να μην ανοιγει το prompt του selenium
            ChromeDriver driver = new ChromeDriver(driverService,chromeOptions);//δημιουργια του chrome driver με τις επικειμενες παραμετρους
            driver.Navigate().GoToUrl(link);//φορτωση του link 
            experimentalFlags.Add("cookies-without-same-site-must-be-secure@2");
            chromeOptions.AddLocalStatePreference("browser.enabled_labs_experiments",
                experimentalFlags);
            var cookies = driver.Manage().Cookies.AllCookies;
            driver.FindElement(By.XPath("//a[contains(@class,'cc-btn--accept')]")).Click();//αποδοχη cookies βρισκοντας τα απο την κλαση που ανηκουν
            Thread.Sleep(2000);//δινουμε χρονο στον chrome driver ωστε να φορτωθουν τα στοιχεια που θελουμε να αντλησουμε
            List<IWebElement> date = driver.FindElements(By.XPath("//div[contains(@class,'events-container__item-date')]")).ToList();//κομβος που παιρνει τις  μερες
            List<IWebElement> hours = driver.FindElements(By.XPath("//div[contains(@class,'events-container__item-time')]")).ToList();//κομβος που παιρνει τις ωρες 
            List<IWebElement> place = driver.FindElements(By.XPath("//span[contains(@class,'events-container__item-venue')]")).ToList();//κομβος που παιρνει τις αιθουσες προβολης
            var money = driver.FindElements(By.CssSelector(".events-container__item-prices")).ToList();//κομβος που παιρνει τις τιμες
            List<string> prices = new List<string>();
            string price = "";//μεταβλητη για χρηση

            foreach (var j in money)
            {
                string[] sep = new string[] { "\r\n" };//διαχωρηστες γραμμων
                string[] s = j.Text.Split(sep, StringSplitOptions.RemoveEmptyEntries);//βαζουμε σε string τον κομβο j 
                foreach (var h in s)
                {
                    price += h;//προσθετουμε το h στο string price
                }
                prices.Add(price);//προσθηκη price στην λιστα με τις τιμες 
                price = "";//αρχικοποιηση string για να μην γεμισει αχρηστες τιμες 
            }

            for (int p = 0; p < place.Count; p++)
            {
                string s = date[p].Text.Split(' ').Last();//παιρνουμε απο καθε ημερομηνια την μερα και τον μηνα η μορφη ειναι καπως ετσι Σαβ 16/1
                string days = "", month = "", year = "", hour="", minutes="", eventvenue="", eventaddress="";
                if (Regex.Match(s, @"[0-9]{1,}(\/)[0-9]{1,}").Success)//αν ταιριαζει με το regular expression τοτε 
                {
                    days = s.Split('/')[0];//βαλε το string που ειναι απο την αριστερη μερια του /  στην μεταβλητη
                    month = s.Split('/')[1];//βαλε το string που ειναι απο την δεξια μερια του /  στην μεταβλητη
                    year = "2021";//χρονος ειναι το 2021
                }
                string date_from = "";
                if (hours[p].Text.Contains(":"))
                {
                    hour = hours[p].Text.Split(':')[0];//παρνουμε την ωρα με χαρακτηρα διασπασης το : την αριστερη μερια 
                    minutes = hours[p].Text.Split(':')[1];//παρνουμε την ωρα με χαρακτηρα διασπασης το : την δεξια μερια 
                    eventvenue = place[p].Text.Split('-')[0].TrimStart().TrimEnd();//παιρνουμε την ονομασια του θεατρου
                    eventaddress = place[p].Text.Split('-')[1].TrimStart().TrimEnd();//παιρνουμε την διευθυνση του θεατρου
                    MySqlCommand findcom = new MySqlCommand("SELECT COUNT(*) FROM venue WHERE Title = '" + eventvenue + "'", mysqlCon);//query Που ελεγχει αν υπαρχει ηδη το θεατρο με βαση το ονομα του
                    findcom.ExecuteNonQuery();
                    long venueexist = (long)findcom.ExecuteScalar();//το αποτελεσμα επιστρεφει εναν αριθμο που τον βαζουμε σε μεταβλητη
                    MySqlCommand insvencomm = mysqlCon.CreateCommand();
                    if (Regex.Match(hours[p].Text, @"[0-9]{1,}[0-9]{1,}:[0-9]{1,}[0-9]{1,}").Success)//αν ολα πανε καλα και η ωρα ειναι συμφωνα με το regular expression
                    {
                        if (venueexist < 1)//αν το θεατρο δεν υπαρχει τοτε 
                        {
                            insvencomm.CommandText = "INSERT INTO `venue`(`Title`, `Address`, `SystemID`) VALUES ('" + eventvenue + "','" + eventaddress + "','" + 3 + "')";//κανε εισαγωγη θεατρου στην βαση
                            insvencomm.ExecuteNonQuery();//εκτελεση εισαγωγης
                            MySqlCommand insEvCom = mysqlCon.CreateCommand();
                            long newid = (long)insvencomm.LastInsertedId;//παιρνουμε το τελευταιο id που εγινε εισαγωγη για να το βαλουμε σαν παραμετρο στο επομενο query εισαγωγης
                            DateTime temp = new DateTime(int.Parse(year), Int32.Parse(month), Int32.Parse(days), Int32.Parse(hour), Int32.Parse(minutes), 0);//δημιουργια datetime αντικειμενου συμφωνα με τις προηγουμενες μεταβλητες
                            date_from = temp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);//μορφοποιηση του αντικειμενου στην επιθυμητη μορφη
                            insEvCom.CommandText = "INSERT INTO events(ProductionID,VenueID,DateEvent,PriceRange,SystemID) VALUES ('" + prodid + "','" + newid + "','" + date_from + "','" + prices[p] + "','" + 3 + "')";//query εισαγωγης προβολων παραστασεων
                            insEvCom.ExecuteNonQuery();//εκτελεση query εισαγωγης
                        }
                        else if (venueexist == 1)//αν υπαρχει ηδη η αιθουσα
                        {
                            MySqlCommand gmtxm = mysqlCon.CreateCommand();
                            gmtxm.CommandText = "SELECT ID FROM venue WHERE Title = @evven";//παιρνουμε το id της αιθουσας απο τον τιτλο της
                            gmtxm.Parameters.AddWithValue("@evven", eventvenue);
                            var evven = gmtxm.ExecuteScalar();
                            DateTime temp = new DateTime(int.Parse(year), Int32.Parse(month), Int32.Parse(days), Int32.Parse(hour), Int32.Parse(minutes), 0);//δημιουργια datetime αντικειμενου συμφωνα με τις προηγουμενες μεταβλητες
                            MySqlCommand insEvent = new MySqlCommand();
                            insEvent.Connection = mysqlCon;
                            date_from = temp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);//μορφοποιηση του αντικειμενου στην επιθυμητη μορφη
                            insEvent.CommandText = "INSERT INTO events(ProductionID,VenueID,DateEvent,PriceRange,SystemID) VALUES ('" + prodid + "','" + evven.ToString() + "','" + date_from + "','" + prices[p] + "','" + 3 + "')";//query εισαγωγης προβολων παραστασεων
                            insEvent.ExecuteNonQuery();//εκτελεση query εισαγωγης
                        }
                    }
                }
                else ///αλλιως κανε εισαγωγη της παραστασης 
                {
                    MySqlCommand insEvCom = mysqlCon.CreateCommand();
                    DateTime temp = new DateTime(int.Parse(year), Int32.Parse(month), Int32.Parse(days),0, 0, 0);
                    date_from = temp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    insEvCom.CommandText = "INSERT INTO events(ProductionID,VenueID,DateEvent,PriceRange,SystemID) VALUES ('" + prodid + "','" + 81 + "','" + date_from + "','" + prices[p] + "','" + 3 + "')";
                    insEvCom.ExecuteNonQuery();
                }
            }
            driver.Quit();//κλεισιμο του selenium driver
            mysqlCon.Close();//κλεισιμο της συνδεσης mysql
        }
        private void insertContribution(List<string> people, List<string> roles, List<string> subroles, int prodid)//μεθοδος για εισαγωγη μελων παραστασης με τους ρολους τους,και τους υπορολους
        {
            long roleid = 0;//παραμετροι ειναι οι ανθρωποι-μελοι με τους ρολους και τους υπορολους τους , και το id της παραστασης
            int i = 0;
            MySqlConnection mysqlCon = new MySqlConnection("SERVER =88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';");//δημιουργια συνδεσης με την mysql βαση
            mysqlCon.Open();//ανοιγμα συνδεσης
            object peopleid = new object();//αντικειμενο για χρηση
            try
            {
                for (i = 0; i < roles.Count; i++)
                {
                    if (!people[i].ToString().Contains(",") & !people[i].ToString().Contains("–"))//αν ρολος δεν περιεχει παραπανω απο ενα ατομο τοτε
                    {
                        string[] arr = people[i].Split('-', '–');
                        MySqlCommand insContr = mysqlCon.CreateCommand();
                        for (int p = 0; p < arr.Length; p++)
                        {
                            string s = arr[p].TrimStart().TrimEnd();
                            if (s.Length > 0)
                            {
                                MySqlCommand lp = mysqlCon.CreateCommand();
                                lp.CommandText = "SELECT ID FROM persons WHERE Fullname='" + s + "'";//επιλεγουμε το id του μελους που εχει το ονομα της παραμετρου 
                                lp.ExecuteNonQuery();//εκτελεση του query
                                if (lp.ExecuteScalar() != null)
                                {
                                    peopleid = lp.ExecuteScalar().ToString();//αν βρεθει το ατομο τοτε βαλε το id σε μεταβλητη
                                    MySqlCommand lr = mysqlCon.CreateCommand();
                                    lr.CommandText = "SELECT ID FROM roles WHERE Role='" + roles[i].ToString() + "'";//δημιουργια query για να παρουμε το id του ρολου
                                    lr.ExecuteNonQuery();//εκτελεση του query
                                    roleid = long.Parse(lr.ExecuteScalar().ToString());//βαζουμε το id του ρολου σε μεταβλητη
                                    MySqlCommand insContr2 = mysqlCon.CreateCommand();
                                    insContr2.CommandText = "INSERT INTO `contributions`(`PeopleID`, `ProductionID`, `RoleID`,`subRole`, `SystemID`) VALUES ('" + Int32.Parse(peopleid.ToString()) + "','" + prodid + "','" + roleid + "','" + subroles[i] + "','" + 3 + "')";//query εισαγωγης contributions
                                    insContr2.ExecuteNonQuery();//εκτελεση query εισαγωγης
                                }
                            }
                        }
                    }
                    else if (people[i].ToString().Contains(",") & people[i].ToString().Length > 0)//αν ρολος περιεχει παραπανω απο ενα ατομο τοτε
                    {
                        string[] arr = people[i].Split(',');//σπασε αυτα τα ατομα σε εναν πινακα με διαχωρηστη το ,
                        MySqlCommand insContr = mysqlCon.CreateCommand();
                        for (int p = 0; p < arr.Length; p++)
                        {
                            string s = arr[p].TrimStart().TrimEnd();
                            if (s.Length > 0 & !s.Contains("και") & !s.Contains("@"))//αν το string δεν περιεχει τις ανεπυθημητες τιμες
                            {
                                MySqlCommand lp = mysqlCon.CreateCommand();
                                lp.CommandText = "SELECT ID FROM persons WHERE Fullname='" + s + "'";//παρε το id του ατομου
                                lp.ExecuteNonQuery();//εκτελεσε το query
                                peopleid = long.Parse(lp.ExecuteScalar().ToString());//επεστρεψε το αποτελεσμα στην μεταβλητη
                                MySqlCommand lr = mysqlCon.CreateCommand();
                                lr.CommandText = "SELECT ID FROM roles WHERE Role='" + roles[i].ToString() + "'";//παρε το id του ρόλου
                                lr.ExecuteNonQuery();//εκτελεσε το query
                                roleid = long.Parse(lr.ExecuteScalar().ToString());
                                MySqlCommand insContr2 = mysqlCon.CreateCommand();//επεστρεψε το αποτελεσμα στην μεταβλητη
                                insContr2.CommandText = "INSERT INTO `contributions`(`PeopleID`, `ProductionID`, `RoleID`,`subRole`, `SystemID`) VALUES ('" + peopleid + "','" + prodid + "','" + roleid + "','" + subroles[i] + "','" + 3 + "')";//query εισαγωγης contributions
                                insContr2.ExecuteNonQuery();//εκτελεση query εισαγωγης
                            }
                        }
                    }
                }
            }
            catch (System.ArgumentOutOfRangeException ex) { }
            mysqlCon.Close();
        }
        //πινακας που περιεχει ανεπιθυμητες λεξεις οι οποιες δεν θελω να γινεται εισαγωγη τους στην βαση
        public static string[] unwanted = { "ΠΡΩΤΑΓΩΝΙΣΤΟΥΝ", "ΠΑΙΖΟΥΝ", "ΑΛΦΑΒΗΤΙΚΑ", "ΔΙΑΝΟΜΗ" , "Παίζουν" , "Θεάτρου", "Θεάτρο","Πρωταγωνιστούν","Συντελεστές:","Συντελεστές",
                    "από", "ΣΥΝΤΕΛΕΣΤΕΣ", "Συμπαραγωγή", "Διανομή", "Ταυτότητα" , "ΑΛΦΑΒΗΤΙΚΑ","Θεάτρου" ,"Προπώληση","Εισιτήρια","Συμπαραγωγή","Πότε","Διάρκεια","ΤΟΥ ΔΗΜΗΤΡΙΟΥ ΒΥΖΑΝΤΙΟΥ","Ευχαριστούμε","*","Διατίθενται"};
        
        public  void insertPersons(string link, int prodid)//μεθοδος για εισαγωγη μελών και ρόλων στην βαση
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();//δημιουργια αντικειμενου για φορτωση link απο το διαδικτυο
            HtmlAgilityPack.HtmlDocument doc = web.Load(link);//φορτωση του Link
            MySqlConnection mysqlCon = new MySqlConnection("SERVER =88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';");//δημιουργια συνδεσης με βαση
            mysqlCon.Open();//ανοιγμα συνδεσης
            List<string> peoples = new List<string>();//λιστα τυπου string για προσωρινή αποθήκευση των μοναδικων ανθρωπων μελων και αποθηκευση τους στην βαση
            List<string> proles = new List<string>();//λιστα τυπου string για προσωρινή αποθήκευση των ρόλων μελων και αποθηκευση τους στην βαση
            List<string> subroles = new List<string>();//λιστα τυπου string για προσωρινή αποθήκευση των υπορόλων μελων και αποθηκευση τους στην βαση
            List<string> peoplesafter = new List<string>();//λιστα τυπου string για προσωρινή αποθήκευση των ανθρωπων μελων 

            bool exists = false;//μεταβλητη boolean που την δηλωνουμε ως false 
            var syntelestesexist = doc.DocumentNode.SelectNodes("//dl[@class='responsive-tabs']").ToList();//παιρνουμε τους κομβους των tab control της παραστασης
            foreach (var x in syntelestesexist)
            {
                if (x.InnerText.TrimStart().TrimEnd().Contains("Συντελεστές"))//αν ο κομβος εχει ονομα συντελεστες , υπαρχουν παραστασεις που δεν εχουν συντελεστες δηλωμενους
                {
                    exists = true;//κανε true την μεταβλητη
                }
            }

            if (exists)
            {
                string test = "";//δηλωση μεταβλητης για χρηση
                string strongtxt = "";// δηλωση μεταβλητης για χρηση
                if (doc?.DocumentNode?.SelectNodes("//dl[@class='responsive-tabs']/dd/p") != null)//αν ο κομβος του html ιστοτοπου δεν ειναι κενος 
                {
                    foreach (HtmlNode node in doc?.DocumentNode?.SelectNodes("//dl[@class='responsive-tabs']/dd/p"))//για καθε p κομβο κανε
                    {
                        strongtxt = RemoveUnwantedTags(node.InnerText.Replace("Πρωταγωνιστούν :","Ηθοποιός:").Replace("ΠΑΙΖΟΥΝ (αλφαβητικά)", "Ηθοποιός").Replace("Ερμηνεύουν (με αλφαβητική σειρά)", "Ηθοποιός").Replace("Παίζουν (αλφαβητικά)", "Ηθοποιός").Replace("ΤΑΥΤΟΤΗΤΑ ΠΑΡΑΣΤΑΣΗΣ", "").Replace("&nbsp;", "").Replace("nbsp;", "").Replace("'", " ").Replace("ΠΡΩΤΑΓΩΝΙΣΤΟΥΝ", "Ηθοποιός").Replace("Με τους", "Ηθοποιός")
                            .Replace("Διανομή", "Ηθοποιός").Replace("Παίζουν (αλφαβητικά)", "Ηθοποιός").Replace("-οι ηθοποιοί", "Ηθοποιός").Replace("-οι performers", "Ηθοποιός").Replace(" =:", "").Replace("laquo;", "<").Replace("ΠΑΙΖΟΥΝ:", "*").Replace("Συμμετέχει ο", "Ηθοποιός:").Replace("Βασικοί Συντελεστές:", "")
                            .Replace("ndash;", ",").Replace("ΗΘΟΠΟΙΟΙ", "Ηθοποιός:").Replace("Ερμηνεύουν:", "Ηθοποιός:").Replace("&", "").Replace("raquo;", ">").Replace("και ο", ",").Replace("και η", ",").Replace("&amp", "").Replace("Πρωταγωνιστεί", "Ηθοποιός").Replace("Ερμηνεύει", "Ηθοποιός:")
                            .Replace("Ηθοποιοί", "Ηθοποιός").Replace("Πρωταγωνιστούν:", "Ηθοποιός:").Replace("Παίζουν επίσης", "Ηθοποιός").Replace("ΣΥΜΜΕΤΕΧΟΥΝ", "Ηθοποιός").Replace("Παίζουν οι ηθοποιοί", "Ηθοποιός").Replace("Παίζουν", "Ηθοποιός:").TrimEnd().TrimStart());
                        test += strongtxt + "\n";//κληση της μεθοδου για απαλαγη απο τον θορυβο και αντικατασταση καποιων λεξεων κλειδιων με τον ρολο ηθοποιός
                    }
                }
                using (StringReader reader = new StringReader(test))//ανοιγουμε stream για να διαβασουμε string μεταβλητη
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)//για καθε γραμμη του προηγουμενου text κανε
                    {
                        if (line.Length > 0)
                        {
                            if (Regex.IsMatch(line, @"[\w]{1,}(.*):(.*)[\w]{1,}") & !unwanted.Any(line.Contains))//θελουμε να περνανε μονο οι γραμμες με συγκεκριμενη εμφανιση πχ Σκηνοθετης:Κωνσταντινος Γεωργιαδης τιποτα διαφορετικο
                            {
                                string role = line.Split(':')[0];//απο την αριστερη μερια παιρνουμε τον ρολο σπαζοντας το string σε 2 μερη με διασπαστη τον χαρακτηρα :
                                string person = line.Split(':')[1].Replace("-", ",").Replace("–", ",");//απο την δεξια μερια παιρνουμε τον ρολο σπαζοντας το string σε 2 μερη με διασπαστη τον χαρακτηρα :
                                if (!unwanted.Any(line.Contains) & !person.Contains(","))
                                {
                                    peoplesafter.Add(person.TrimStart().TrimEnd());//αυτη την λιστα string περναω τα ατομα ετσι οπως εμφανιζονται στον ιστοτοπο χωρις διασπαση
                                }
                                if (line.Contains(":") & !unwanted.Any(line.Contains))
                                {
                                    if (!(role.Contains(" ") | role.Contains("-") | role.Contains(",")))//ελεγχος για υπορολους
                                    {
                                        MySqlCommand roleexist = mysqlCon.CreateCommand();//δημιουργια mysql command
                                        roleexist.CommandText = "SELECT COUNT(*) FROM roles where Role LIKE'%" + role.TrimStart().TrimEnd() + "%'";//ελεγχος αν υπαρχει ηδη στην βαση
                                        roleexist.ExecuteNonQuery();//εκτελεση του command
                                        int rolecount = int.Parse(roleexist.ExecuteScalar().ToString());//επιστρεφουμε το αποτελεσμα του command σε μεταβλητη
                                        if (rolecount > 0)//αν υπαρχει ο ρολος 
                                        {
                                            proles.Add(role);//προσθεσε τον στην λιστα string για ρολους
                                            subroles.Add("");//προσθεσε κενο υπορολο
                                        }
                                        else//αλλιως
                                        {
                                            subroles.Add(role);//προσθεσε τον ρολο σαν υπορολο
                                            proles.Add("Ηθοποιός");//προσθεσε τον ρολο ως ηθοποιο
                                        }
                                    }
                                    else if (role.Contains("-") | role.Contains(",") | role.Contains(" "))//η ιδια διαδικασια απλα για ρολους οι οποιοι περιεχουν κενο η - η ,
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
                                    peoples.Add(person.TrimStart().TrimEnd());//προσθηκη ανθρωπων στην μοναδικη λιστα για εισαγωγη στην βαση
                                }
                                else if (person.Contains(','))
                                {
                                    peoplesafter.Add(person);//προσθηκων ανθρωπων που περιεχουν κομμα οπως ειναι χωρις διασπαση
                                    string[] arr = person.Split(',');//δημιουργια πινακα με διασπαση το κενο
                                    foreach (var item in arr)
                                    {
                                        if (item.Length > 0)
                                        {
                                            peoples.Add(item.TrimStart());//προσθεσε το ονομ/μο που βρισκεται χωρισμενο με κομμα στην μοναδικη λιστα για εισαγωγη στην βαση
                                        }
                                    }
                                }
                            }
                            else if (line.Contains(",") & !unwanted.Any(line.Contains))//η περιπτωση που απλα εχει ονοματα η γραμμη με , τα εισαγω ως ηθοποιους
                            {
                                peoplesafter.Add(line);//προσθηκη ανθρωπων στην λιστα
                                string[] arr = line.Split(',');//δημιουργια πινακα με διασπαση το κενο
                                foreach (var item in arr)
                                {
                                    if (item.Length > 0)
                                    {
                                        peoples.Add(item.TrimStart());//προσθεσε το μελος στην μοναδικη λιστα για εισαγωγη στην βαση
                                        proles.Add("Ηθοποιός");//προσθεσε σαν ρολο τον ηθοποιο
                                        subroles.Add("");////προσθεσε σαν υπορολο το κενο
                                    }
                                }
                            }
                            else if (line.Length > 1 & !unwanted.Any(line.Contains))//η περιπτωση που απλα εχει ονοματα η γραμμη χωρις κομμα τα εισαγω ως ηθοποιους
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
                
                if (doc?.DocumentNode?.SelectSingleNode("//tbody") != null)//η περιπτωση που συνηθως οι ηθοποιοι ειναι σε κομβο table
                {
                    var table = doc?.DocumentNode?.SelectSingleNode("//tbody");//παιρνουμε τον κομβο table
                    var rows = table?.SelectNodes("//tr/td")?.ToList();//απο τον προηγουμενο κομβο παιρνουμε τις γραμμες με τις τιμες τους 
                    for (int t = 0; t < rows.Count; t++)
                    {
                        peoples.Add(rows[t].InnerText);//προσθηκη στην λιστα για τα μελη που θα περαστουν στην βαση
                        peoplesafter.Add(rows[t].InnerText);//προσθηκη στην λιστα για να τα περασω αργοτερα στον πινακα contributions
                        proles.Add("Ηθοποιός");//προσθηκη ρολου ηθοποιου
                    }
                }
                //μετατροπη ρολων στον ρολο ηθοποιο
                for (int i = 0; i < proles.Count; i++)
                {
                    proles[i] = proles[i].Replace("Διανομή (με αλφαβητική σειρά)", "Ηθοποιός").Replace("και με αλφαβητική σειρά:", "Ηθοποιός").Replace("Παίζουν επίσης:", "Ηθοποιός").Replace("Παίζουν οι", "Ηθοποιός").Replace("ΗΘΟΠΟΙΟΙ", "Ηθοποιός").Replace("Παίζουν:", "Ηθοποιός:").Replace("ΠΡΩΤΑΓΩΝΙΣΤΟΥΝ", "Ηθοποιός")
                        .Replace("Διανομή (αλφαβητικά)", "Ηθοποιός").Replace("Ερμηνεία", "Ηθοποιός").Replace("Ερμηνεύουν", "Ηθοποιός").Replace("Ερμηνεύει", "Ηθοποιός").Replace("Με τους:", "Ηθοποιός")
                        .Replace("Πρωταγωνιστούν", "Ηθοποιός");
                }

                string[] unwanted2 = { "Ερμηνεύουν","Διατίθενται", "Συντελεστές:" ,"ΣΥΝΤΕΛΕΣΤΕΣ", "ΠΑΙΖΟΥΝ", "Παίζουν","Παίζουν:", "ΔΙΑΝΟΜΗ","Διανομή", "ΑΛΦΑΒΗΤΙΚΑ", "και", "ΠΡΩΤΑΓΩΝΙΣΤΟΥΝ", "Θεάτρου", "Θεάτρο", "Διατίθενται","από",
                "Συμπαραγωγή","Ταυτότητα","Ένας","Διατίθενται θέσεις καθήμενων","ΤΑΥΤΟΤΗΤΑ ΠΑΡΑΣΤΑΣΗΣ","Οι ηθοποιοί","Πού","Πότε","ΤΟΥ ΔΗΜΗΤΡΙΟΥ ΒΥΖΑΝΤΙΟΥ","Ώρες","Εισιτήρια","Προπώληση","ΤΟΥ ΔΗΜΗΤΡΙΟΥ ΒΥΖΑΝΤΙΟΥ","Η ΒΑΒΥΛΩΝΙΑ"};
                //εδω γινεται ενα καθαρισμα ωστε να μην μπαινουν ανεπιθυμητες εγγραφες στην βαση
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
            //ξεκιναει η εισαγωγη στην βαση
            string[] arrpeoples = peoples.Distinct().ToArray();//μετατρεπουμε το people list σε πινακα και χρησιμοποιουμε την distinct μεθοδο για διαγραφη διπλωτυπων
            long count = 0;//μεταβλητη μετρητη
            for (int u = 0; u < arrpeoples.Length; u++)
            {
                string fullname = arrpeoples[u].TrimStart().TrimEnd();//γινεται ελεγχος αν υπαρχει το ονομα ηδη στην βαση
                MySqlCommand findPerson = mysqlCon.CreateCommand();
                findPerson.CommandText = "SELECT COUNT(*) FROM persons WHERE Fullname='" + fullname + "'";
                count = (long)findPerson.ExecuteScalar();
                if (fullname.Length > 0 & count == 0)//αν το μεγεθος του ονοματος ειναι μεγαλυτερο απο το 0 και δεν εχει βρεθει στην βαση τοτε ..
                {
                    MySqlCommand insPerson = mysqlCon.CreateCommand();//δημιουργια mysql command
                    insPerson.CommandText = "INSERT INTO persons(Fullname,SystemID) VALUES ('" + fullname + "','" + "3" + "')";//γινεται εισαγωγη του μελους παραστασης στην βαση 
                    insPerson.ExecuteNonQuery();//εκτελεση του query
                }
            }
            string[] rolesarray = proles.Distinct().ToArray();//μετατρεπουμε το proles list σε πινακα και χρησιμοποιουμε την distinct μεθοδο για διαγραφη διπλωτυπων
            for (int p = 0; p < rolesarray.Length; p++)
            {
                MySqlCommand checkcmd = mysqlCon.CreateCommand();
                checkcmd.CommandText = "SELECT COUNT(*) FROM roles where Role LIKE'" + rolesarray[p].TrimStart().TrimEnd() + "'";//γινεται ελεγχος αν υπαρχει το ονομα ηδη στην βαση
                checkcmd.ExecuteNonQuery();//εκτελεση του command
                int mysqlint = int.Parse(checkcmd.ExecuteScalar().ToString());//επιστροφη αποτελεσματος query σε μεταβλητη
                if (mysqlint > 0)// αν υπαρχει ηδη ο ρολος
                {
                    //MessageBox.Show("Role already exists");
                }
                else//αλλιως προχωρα στην εισαγωγη νεου ρολου
                {
                    MySqlCommand command = mysqlCon.CreateCommand();
                    command.CommandText = "INSERT INTO `roles`(`Role`, `SystemID`) VALUES ('" + rolesarray[p].TrimStart().TrimEnd() + "','" + "3" + "')";//γινεται η εισαγωγη νεου ρολου στην βαση
                    command.ExecuteNonQuery();
                }
            }
            insertContribution(peoplesafter, proles, subroles, prodid);
            mysqlCon.Close();
        }
        List<string> checknewlinks()//μεθοδος που επιστρεφει τα νεα link παραστασεων τα οποια δεν υπαρχουν στην βαση
        {
            List<string> theatrelinks = getallproductionlinks();//παιρνουμε ολα τα link απο την  getallproductionlinks()
            List<string> links = new List<string>();//δημιουργια λιστας string 
            MySqlConnection mysqlCon = new MySqlConnection("SERVER =88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';");//εγκαθιδρηση συνδεσης με την βαση
            mysqlCon.Open();//ανοιγμα της συνδεσης
            try
            {
                MySqlCommand checknew = mysqlCon.CreateCommand();//δημιουργια mysql query command
                foreach (var j in theatrelinks)
                {
                    checknew.CommandText = "SELECT ID FROM `production` WHERE `URL` LIKE'%" + j.ToString().TrimStart().TrimEnd() + "%'";//το query command με παραμετρους  
                    checknew.ExecuteNonQuery();//εκτελεση του query
                    object prodexist = (object)checknew.ExecuteScalar();//δημιουργια προσωρινου oject αντικειμενου οπου ενσωματωνεται το αποτελεσμα του query
                    if (prodexist != null)//αν ειναι null δλδ δεν βρει καινουργια παρασταση
                    {
                        //Console.WriteLine(venueexist.ToString());αν δεν
                    }
                    else//αλλιως προσθεσε την παρασταση που δεν υπαρχει στην λιστα string 
                    {
                        links.Add(j.ToString());
                        Console.WriteLine("εισαγωγη νεας παραστασης :"+j.ToString());
                    }
                }
            }
            catch (System.InvalidCastException) { }
            mysqlCon.Close();//κλεισιμο mysql συνδεσης
            return links;//επεστρεψε τα link των νεων παραστασεων
        }
        List<string> getallproductionlinks()//μεθοδος που επιστρεφει ολα τα link της πηγης 
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();//δημιοργια αντικειμενου που θα φορτωσουμε το link πηγη
            HtmlAgilityPack.HtmlDocument doc = web.Load("https://www.viva.gr/tickets/theatre/");//καλεσμα του link πηγης
            List<string> theatrelinks = new List<string>();//δημιουργια list string για την αποθηκευση των link 
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))//για καθε κομβο που περιεχει την ιδιοτητα href 
            {
                HtmlAttribute att = link.Attributes["href"];//απο τον κομβο αυτον παρε το χαρακτηριστικο href

                if (att.Value.Contains("a") & (att.Value.StartsWith("/tickets/theatre") | att.Value.StartsWith("/tickets/theater") | (att.Value.StartsWith("/tickets/stand-up-comedy"))))
                {
                    theatrelinks.Add("https://www.viva.gr" + att.Value);//αν το link της παραστασης ξεκιναει με theatre/theater/stand-up-comedy τοτε προσθεσε το στην list string (προσθετουμε το https://www.viva.gr γιατι παιρνουμε το path μετα το /tickets
                }
            }
       
            return theatrelinks;
        }
        private void btnNotification_Click(object sender, EventArgs e){ }
        private void btnSuccess_Click(object sender, EventArgs e){ }
        private void MainForm_Resize(object sender, EventArgs e)
        {
            if(FormWindowState.Minimized == WindowState)//αν γινει ελαχιστοποιοηση της φορμας τοτε κρυψε την εφαρμογη 
            {
                Hide();
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
           Show();
           this.BringToFront();
           this.Activate();
           this.WindowState = FormWindowState.Normal;
        }
    }
}