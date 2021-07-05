using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HtmlScraper
{
    public partial class MainBoard : MaterialSkin.Controls.MaterialForm
    {
        public MainBoard()
        {
            InitializeComponent();
            var skinmanager = MaterialSkin.MaterialSkinManager.Instance;
            skinmanager.AddFormToManage(this);
            skinmanager.Theme = MaterialSkin.MaterialSkinManager.Themes.DARK;
            skinmanager.ColorScheme = new MaterialSkin.ColorScheme(MaterialSkin.Primary.BlueGrey800, MaterialSkin.Primary.BlueGrey900,
                MaterialSkin.Primary.BlueGrey500, MaterialSkin.Accent.LightBlue200, MaterialSkin.TextShade.WHITE);
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy-MM-dd";
            setLastThreeDays();
            getsuccorganizer();
        }
        private void setLastThreeDays()
        {
            MySqlConnection mysqlCon = new MySqlConnection("SERVER =88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';");//δημιουργια συνδεσης με βαση
            mysqlCon.Open();//ανοιγμα συνδεσης
            MySqlCommand getcountlastdays = mysqlCon.CreateCommand();
            getcountlastdays.CommandText = "SELECT COUNT(*) FROM production WHERE timestamp >= ( CURDATE() - INTERVAL 3 DAY )";
            object last3days = getcountlastdays.ExecuteScalar().ToString(); 
            lbllastdays.Text = last3days.ToString();
            MySqlCommand getcountallprod = mysqlCon.CreateCommand();
            getcountallprod.CommandText = "SELECT COUNT(*) FROM production";
            object countallprod = getcountallprod.ExecuteScalar();
            lblcountprod.Text = countallprod.ToString();
            mysqlCon.Close();
        }
        private void getsuccorganizer()
        {
            MySqlConnection mysqlCon = new MySqlConnection("SERVER =88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';");//δημιουργια συνδεσης με βαση
            mysqlCon.Open();//ανοιγμα συνδεσης
            MySqlCommand getMostorganizer = mysqlCon.CreateCommand();
            getMostorganizer.CommandText = "select `OrganizerID`, count(`OrganizerID`) from production group by `OrganizerID` order by count(`OrganizerID`) desc LIMIT 1";
            int orginizerid = (int)getMostorganizer.ExecuteScalar();
            MySqlCommand getSuccOrgDetails = mysqlCon.CreateCommand();
            getSuccOrgDetails.CommandText = "SELECT * FROM organizer Where ID=" + orginizerid + "";
            using (MySqlConnection con = mysqlCon)
            {
                using (MySqlDataAdapter sda = new MySqlDataAdapter("SELECT * FROM organizer Where ID=" + orginizerid + "", con))
                {
                    lvOrganizersucc.View = View.Details;
                    MySqlDataAdapter ada = new MySqlDataAdapter("SELECT * FROM organizer Where ID=" + orginizerid + "", con);
                    DataTable dt = new DataTable();
                    ada.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        ListViewItem listitem = new ListViewItem(dr["Name"].ToString());
                        listitem.SubItems.Add(dr["Address"].ToString());
                        listitem.SubItems.Add(dr["Town"].ToString());
                        listitem.SubItems.Add(dr["postcode"].ToString());
                        listitem.SubItems.Add(dr["Phone"].ToString());
                        listitem.SubItems.Add(dr["Email"].ToString());
                        listitem.SubItems.Add(dr["Doy"].ToString());
                        listitem.SubItems.Add(dr["Afm"].ToString());
                        listitem.SubItems.Add(dr["SystemID"].ToString());
                        lvOrganizersucc.Items.Add(listitem);
                    }
                }
            }
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            MySqlConnection mysqlCon = new MySqlConnection("SERVER =88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';");//εγκαθιδρηση συνδεσης με την βαση
            //dgvGetrecords.Rows.Clear();
            mysqlCon.Open();//ανοιγμα της συνδεσης
            try
            {
                MySqlCommand getrowsbydate = mysqlCon.CreateCommand();//δημιουργια mysql query command
                getrowsbydate.CommandText = "SELECT `production`.`Title` as ProductionTitle,`events`.`DateEvent`,`events`.`PriceRange`,`venue`.`Title` as VenueName,`venue`.`Address` FROM `production` join `events` " +
                    "on `production`.`ID`=`events`.`ProductionID` join `venue` on `events`.`VenueID`=`venue`.`ID` WHERE `events`.`DateEvent` LIKE '"+  dateTimePicker1.Text+"%' ORDER BY DateEvent ASC";
                MySqlDataAdapter sda = new MySqlDataAdapter(getrowsbydate.CommandText,mysqlCon);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(sda);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                dgvGetrecords.DataSource = dt;
                
            }
            catch (System.InvalidCastException) { }
            mysqlCon.Close();//κλεισιμο mysql συνδεσης
            //SELECT cast(`DateEvent` as date), count(*) as TotalSignUpsPerDay FROM events GROUP BY 1
            string data = string.Empty;
            int indexOfYourColumn = 0;
            List<string> ls = new List<string>();
            foreach (DataGridViewRow row in dgvGetrecords.Rows)
            {
                data = (string)row.Cells[2].Value;
                ls.Add(data);
            }
            for (int i = 0; i < ls.Count; i++)
            {
                ls[i] = ls[i].Replace("€", "");
            }
            int c = 0;
            int num = 0;
            foreach (var i in ls)
            {
                if (i.All(char.IsDigit) & i.ToString().Length>0)
                {
                    c++;
                    int j = Int32.Parse(i.ToString().TrimStart().TrimEnd());
                    num += j;
                    Console.WriteLine(i);
                }
            }
            long sum = num / c;
            Console.WriteLine("Mesi timi " + sum);
            lblMo.Text = sum.ToString();
        }

        private void lvOrganizersucc_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
