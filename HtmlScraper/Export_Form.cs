using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HtmlScraper
{
    public partial class Export_Form : MaterialSkin.Controls.MaterialForm
    {
        public Export_Form()
        {
            InitializeComponent();
            var skinmanager = MaterialSkin.MaterialSkinManager.Instance;
            skinmanager.AddFormToManage(this);
            skinmanager.Theme = MaterialSkin.MaterialSkinManager.Themes.DARK;
            skinmanager.ColorScheme = new MaterialSkin.ColorScheme(MaterialSkin.Primary.BlueGrey800,
                MaterialSkin.Primary.Grey800, MaterialSkin.Primary.Grey800,
                MaterialSkin.Accent.LightBlue700, MaterialSkin.TextShade.WHITE);
        }
        public int countCheckboxes()
        {
            var checkedBoxes = 0;
            // Iterate through all of the Controls in your Form
            foreach (Control c in this.groupBoxTables.Controls)
            {
                // If one of the Controls is a CheckBox and it is checked, then
                // increment your count
                if (c is CheckBox && (c as CheckBox).Checked)
                {
                    checkedBoxes++;
                }
            }
            return checkedBoxes;
        }
        public string getCheckboxesChecked()
        {
            var checkedBoxes = "";
            foreach (Control c in this.groupBoxTables.Controls)
            {
                if (c is CheckBox && (c as CheckBox).Checked)
                {
                    checkedBoxes = c.Text;
                }
            }
            return checkedBoxes;
        }

        private void materialCheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            cbEvents.Checked = false;
            cbProduction.Checked = false;
            cbOrganizer.Checked = false;
            cbPersons.Checked = false;
            cbVenue.Checked = false;
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void mrbCsv_Click(object sender, EventArgs e)
        {
            if (!(cbContributions.Checked | cbEvents.Checked | cbPersons.Checked | cbProduction.Checked | cbVenue.Checked | cbOrganizer.Checked))
            {
                MessageBox.Show("You must check at least one of the checkboxes to convert into csv.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (textID.Text != "")
                {
                    for (int i = 0; i < 1; i++)
                    {
                        MySqlConnection mySqlConnection = new MySqlConnection();
                        string connetionString = "SERVER =88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';";
                        MySqlConnection mysqlCon = new MySqlConnection(connetionString);
                        mysqlCon.Open();
                        int count = 0;
                        mySqlConnection.ConnectionString = connetionString;
                        DataTable table = new DataTable();
                        SaveFileDialog save = new SaveFileDialog();
                        MySqlDataAdapter MyDA = new MySqlDataAdapter();
                        save.FileName = "Record.csv";
                        save.Filter = "Csv File | *.csv";
                        save.Title = "Save " + getCheckboxesChecked() + " into a csv file";
                        for (int j = 0; j < cbItems.Items.Count; j++)
                        {
                            string sqlSelectAll = "SELECT * from " + getCheckboxesChecked() + " where ID = " +cbItems.Items[j].ToString();
                            MyDA.SelectCommand = new MySqlCommand(sqlSelectAll, mysqlCon);
                            MyDA.Fill(table);
                        }
                        MySqlCommand countcomm = new MySqlCommand();
                        countcomm.Connection = mysqlCon;
                        countcomm.CommandText = "SELECT COUNT(*) from " + getCheckboxesChecked() + " where ID = " + textID.Text;
                        countcomm.ExecuteNonQuery();
                        
                        int num = int.Parse(countcomm.ExecuteScalar().ToString());
                        if (num > 0)
                        {
                            if (save.ShowDialog() == DialogResult.OK)
                            {
                                WriteToCsvFile(table, save.FileName);
                            }
                        }
                        else
                        {
                            MessageBox.Show("This ID cannot be found on this table..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        mysqlCon.Close();
                    }
                }
            }
        }
        public string DataTableToJsonWithJsonNet(DataTable objDataTable)
        {
            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(objDataTable);
            return jsonString;
        }
        public void WriteToCsvFile(DataTable dataTable, string filePath)
        {
            StringBuilder fileContent = new StringBuilder();

            foreach (var col in dataTable.Columns)
            {
                fileContent.Append(col.ToString() + ",");
            }

            fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);

            foreach (DataRow dr in dataTable.Rows)
            {
                foreach (var column in dr.ItemArray)
                {
                    fileContent.Append("\"" + column.ToString() + "\",");
                }

                fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);
            }

            System.IO.File.WriteAllText(filePath, fileContent.ToString());
        }

        private void mrbJson_Click(object sender, EventArgs e)
        {
            if (!(cbContributions.Checked | cbEvents.Checked | cbPersons.Checked | cbProduction.Checked | cbVenue.Checked | cbOrganizer.Checked))
            {
                MessageBox.Show("You must check at least one of the checkboxes.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (textID.Text != "")
                {
                    DataTable table = new DataTable();
                    string connetionString = "SERVER =88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';";
                    MySqlConnection mysqlCon = new MySqlConnection(connetionString);
                    MySqlDataAdapter MyDA = new MySqlDataAdapter();
                    mysqlCon.Open();
                    string s = "";

                    for (int i = 0; i < 1; i++)
                    {
                        SaveFileDialog save = new SaveFileDialog();
                        save.FileName = "Record.json";
                        save.Filter = "Json File | *.json";
                        save.Title = "Save " + getCheckboxesChecked() + " into a json file";
                        for (int j = 0; j < cbItems.Items.Count; j++)
                        {
                            string sqlSelectAll = "SELECT * from " + getCheckboxesChecked() + " WHERE ID=" + cbItems.Items[j].ToString();
                            MyDA.SelectCommand = new MySqlCommand(sqlSelectAll, mysqlCon);
                            MyDA.Fill(table);
                        }
                        MySqlCommand countcomm = new MySqlCommand();
                        countcomm.Connection = mysqlCon;
                        countcomm.CommandText = "SELECT COUNT(*) from " + getCheckboxesChecked() + " where ID=" + textID.Text;
                        countcomm.ExecuteNonQuery();
                        int num = int.Parse(countcomm.ExecuteScalar().ToString());
                        if (num > 0)
                        {
                            
                            if (save.ShowDialog() == DialogResult.OK)
                            {
                                StreamWriter writer = new StreamWriter(save.OpenFile());

                                writer.WriteLine(DataTableToJsonWithJsonNet(table));
                                writer.Dispose();
                                writer.Close();
                            }
                        }
                        else
                        {
                            MessageBox.Show("This ID cannot be found on this table..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        mysqlCon.Close();
                    }
                }
            }
        }

        private void mrbExcel_Click(object sender, EventArgs e)
        {
            if (!(cbContributions.Checked | cbEvents.Checked | cbPersons.Checked | cbProduction.Checked | cbVenue.Checked | cbOrganizer.Checked))
            {
                MessageBox.Show("You must check at least one of the checkboxes.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (textID.Text != "")
                {
                    string connetionString = "SERVER =88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';";
                    MySqlConnection mysqlCon = new MySqlConnection(connetionString);
                    SaveFileDialog save = new SaveFileDialog();
                    mysqlCon.Open();
                    save.FileName = "example.xlsx";
                    save.Filter = "Excel File | *.xlsx";
                    save.Title = "Save " + getCheckboxesChecked() + " into a excel file";
                    MySqlCommand countcomm = new MySqlCommand();
                    countcomm.Connection = mysqlCon;
                    countcomm.CommandText = "SELECT COUNT(*) from " + getCheckboxesChecked() + " where ID = " + textID.Text;
                    countcomm.ExecuteNonQuery();
                    int num = int.Parse(countcomm.ExecuteScalar().ToString());
                    if (num > 0)
                    {
                        if (save.ShowDialog() == DialogResult.OK)
                        {
                            var wb = new XLWorkbook();
                            var dataTable = GetTables(getCheckboxesChecked());
                            wb.Worksheets.Add(dataTable);
                            wb.SaveAs(save.FileName);
                        }
                        else
                        {
                            MessageBox.Show("This ID cannot be found on this table..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        mysqlCon.Close();
                    }
                }
            }
        }
        private DataTable GetTable(String tableName)
        {
            string connetionString = "SERVER =88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';";
            MySqlConnection mysqlCon = new MySqlConnection(connetionString);
            MySqlDataAdapter MyDA = new MySqlDataAdapter();
            string s = "";
            DataTable table = new DataTable();
            string sqlSelectAll = "SELECT * FROM " + getCheckboxesChecked() + " WHERE ID=" + textID.Text;
            MyDA.SelectCommand = new MySqlCommand(sqlSelectAll, mysqlCon);
            MyDA.Fill(table);
            table.TableName = tableName;
            return table;
        }
        private DataTable GetTables(String tableName)
        {
            string connetionString = "SERVER =88.99.136.47;PORT=3306;DATABASE=xuxlffke_scrapingdb;USER=xuxlffke_scraperuser;PASSWORD='lA,wA&5$w]}=';";
            MySqlConnection mysqlCon = new MySqlConnection(connetionString);
            MySqlDataAdapter MyDA = new MySqlDataAdapter();
            string s = "";
            DataTable table = new DataTable();
            for (int j = 0; j < cbItems.Items.Count; j++)
            {
                string sqlSelectAll = "SELECT * from " + getCheckboxesChecked() + " WHERE ID=" + cbItems.Items[j].ToString();
                MyDA.SelectCommand = new MySqlCommand(sqlSelectAll, mysqlCon);
                MyDA.Fill(table);
            }
            table.TableName = tableName;
            return table;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var dataTable = GetTable(getCheckboxesChecked());
            foreach(DataRow row in dataTable.Rows)
            {
                if (row.ItemArray.Length>0 & !row.Field<int>(0).Equals(textID.Text.ToString()) & textID.Text.Length>0)
                {
                    cbItems.Items.Add(textID.Text);
                }
            }    
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            cbItems.Items.Clear();
        }

        private void cbItems_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void cbProduction_CheckedChanged(object sender, EventArgs e)
        {
            cbEvents.Checked = false;
            cbContributions.Checked = false;
            cbOrganizer.Checked = false;
            cbPersons.Checked = false;
            cbVenue.Checked = false;
        }

        private void cbEvents_CheckedChanged(object sender, EventArgs e)
        {
            cbProduction.Checked = false;
            cbContributions.Checked = false;
            cbOrganizer.Checked = false;
            cbPersons.Checked = false;
            cbVenue.Checked = false;
        }

        private void cbPersons_CheckedChanged(object sender, EventArgs e)
        {
            cbEvents.Checked = false;
            cbContributions.Checked = false;
            cbOrganizer.Checked = false;
            cbProduction.Checked = false;
            cbVenue.Checked = false;
        }

        private void cbOrganizer_CheckedChanged(object sender, EventArgs e)
        {
            cbEvents.Checked = false;
            cbContributions.Checked = false;
            cbProduction.Checked = false;
            cbPersons.Checked = false;
            cbVenue.Checked = false;
        }

        private void cbVenue_CheckedChanged(object sender, EventArgs e)
        {
            cbEvents.Checked = false;
            cbContributions.Checked = false;
            cbOrganizer.Checked = false;
            cbPersons.Checked = false;
            cbProduction.Checked = false;
        }

        private void cbProduction_Click(object sender, EventArgs e)
        {
            cbProduction.Checked = true;
        }

        private void cbEvents_Click(object sender, EventArgs e)
        {
            cbEvents.Checked = true;
        }

        private void cbContributions_Click(object sender, EventArgs e)
        {
            cbContributions.Checked = true;
        }

        private void cbPersons_Click(object sender, EventArgs e)
        {
            cbPersons.Checked = true;
        }

        private void cbOrganizer_Click(object sender, EventArgs e)
        {
            cbOrganizer.Checked = true;
        }

        private void cbVenue_Click(object sender, EventArgs e)
        {
            cbVenue.Checked = true;
        }
    }
}
