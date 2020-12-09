
namespace HtmlScraper
{
    partial class Export_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxTables = new System.Windows.Forms.GroupBox();
            this.cbProduction = new MaterialSkin.Controls.MaterialCheckBox();
            this.cbEvents = new MaterialSkin.Controls.MaterialCheckBox();
            this.cbContributions = new MaterialSkin.Controls.MaterialCheckBox();
            this.cbPersons = new MaterialSkin.Controls.MaterialCheckBox();
            this.cbOrganizer = new MaterialSkin.Controls.MaterialCheckBox();
            this.cbVenue = new MaterialSkin.Controls.MaterialCheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textID = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.materialRaisedButton1 = new MaterialSkin.Controls.MaterialRaisedButton();
            this.mrbCsv = new MaterialSkin.Controls.MaterialRaisedButton();
            this.mrbJson = new MaterialSkin.Controls.MaterialRaisedButton();
            this.mrbExcel = new MaterialSkin.Controls.MaterialRaisedButton();
            this.groupBoxTables.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxTables
            // 
            this.groupBoxTables.Controls.Add(this.cbVenue);
            this.groupBoxTables.Controls.Add(this.cbOrganizer);
            this.groupBoxTables.Controls.Add(this.cbPersons);
            this.groupBoxTables.Controls.Add(this.cbContributions);
            this.groupBoxTables.Controls.Add(this.cbEvents);
            this.groupBoxTables.Controls.Add(this.cbProduction);
            this.groupBoxTables.Location = new System.Drawing.Point(25, 81);
            this.groupBoxTables.Name = "groupBoxTables";
            this.groupBoxTables.Size = new System.Drawing.Size(197, 318);
            this.groupBoxTables.TabIndex = 0;
            this.groupBoxTables.TabStop = false;
            // 
            // cbProduction
            // 
            this.cbProduction.AutoSize = true;
            this.cbProduction.Depth = 0;
            this.cbProduction.Font = new System.Drawing.Font("Roboto", 10F);
            this.cbProduction.Location = new System.Drawing.Point(17, 26);
            this.cbProduction.Margin = new System.Windows.Forms.Padding(0);
            this.cbProduction.MouseLocation = new System.Drawing.Point(-1, -1);
            this.cbProduction.MouseState = MaterialSkin.MouseState.HOVER;
            this.cbProduction.Name = "cbProduction";
            this.cbProduction.Ripple = true;
            this.cbProduction.Size = new System.Drawing.Size(96, 30);
            this.cbProduction.TabIndex = 0;
            this.cbProduction.Text = "production";
            this.cbProduction.UseVisualStyleBackColor = true;
            // 
            // cbEvents
            // 
            this.cbEvents.AutoSize = true;
            this.cbEvents.Depth = 0;
            this.cbEvents.Font = new System.Drawing.Font("Roboto", 10F);
            this.cbEvents.Location = new System.Drawing.Point(17, 77);
            this.cbEvents.Margin = new System.Windows.Forms.Padding(0);
            this.cbEvents.MouseLocation = new System.Drawing.Point(-1, -1);
            this.cbEvents.MouseState = MaterialSkin.MouseState.HOVER;
            this.cbEvents.Name = "cbEvents";
            this.cbEvents.Ripple = true;
            this.cbEvents.Size = new System.Drawing.Size(71, 30);
            this.cbEvents.TabIndex = 1;
            this.cbEvents.Text = "events";
            this.cbEvents.UseVisualStyleBackColor = true;
            // 
            // cbContributions
            // 
            this.cbContributions.AutoSize = true;
            this.cbContributions.Depth = 0;
            this.cbContributions.Font = new System.Drawing.Font("Roboto", 10F);
            this.cbContributions.Location = new System.Drawing.Point(17, 123);
            this.cbContributions.Margin = new System.Windows.Forms.Padding(0);
            this.cbContributions.MouseLocation = new System.Drawing.Point(-1, -1);
            this.cbContributions.MouseState = MaterialSkin.MouseState.HOVER;
            this.cbContributions.Name = "cbContributions";
            this.cbContributions.Ripple = true;
            this.cbContributions.Size = new System.Drawing.Size(111, 30);
            this.cbContributions.TabIndex = 2;
            this.cbContributions.Text = "contributions";
            this.cbContributions.UseVisualStyleBackColor = true;
            this.cbContributions.CheckedChanged += new System.EventHandler(this.materialCheckBox3_CheckedChanged);
            // 
            // cbPersons
            // 
            this.cbPersons.AutoSize = true;
            this.cbPersons.Depth = 0;
            this.cbPersons.Font = new System.Drawing.Font("Roboto", 10F);
            this.cbPersons.Location = new System.Drawing.Point(17, 174);
            this.cbPersons.Margin = new System.Windows.Forms.Padding(0);
            this.cbPersons.MouseLocation = new System.Drawing.Point(-1, -1);
            this.cbPersons.MouseState = MaterialSkin.MouseState.HOVER;
            this.cbPersons.Name = "cbPersons";
            this.cbPersons.Ripple = true;
            this.cbPersons.Size = new System.Drawing.Size(80, 30);
            this.cbPersons.TabIndex = 3;
            this.cbPersons.Text = "persons";
            this.cbPersons.UseVisualStyleBackColor = true;
            // 
            // cbOrganizer
            // 
            this.cbOrganizer.AutoSize = true;
            this.cbOrganizer.Depth = 0;
            this.cbOrganizer.Font = new System.Drawing.Font("Roboto", 10F);
            this.cbOrganizer.Location = new System.Drawing.Point(17, 223);
            this.cbOrganizer.Margin = new System.Windows.Forms.Padding(0);
            this.cbOrganizer.MouseLocation = new System.Drawing.Point(-1, -1);
            this.cbOrganizer.MouseState = MaterialSkin.MouseState.HOVER;
            this.cbOrganizer.Name = "cbOrganizer";
            this.cbOrganizer.Ripple = true;
            this.cbOrganizer.Size = new System.Drawing.Size(88, 30);
            this.cbOrganizer.TabIndex = 4;
            this.cbOrganizer.Text = "organizer";
            this.cbOrganizer.UseVisualStyleBackColor = true;
            // 
            // cbVenue
            // 
            this.cbVenue.AutoSize = true;
            this.cbVenue.Depth = 0;
            this.cbVenue.Font = new System.Drawing.Font("Roboto", 10F);
            this.cbVenue.Location = new System.Drawing.Point(17, 273);
            this.cbVenue.Margin = new System.Windows.Forms.Padding(0);
            this.cbVenue.MouseLocation = new System.Drawing.Point(-1, -1);
            this.cbVenue.MouseState = MaterialSkin.MouseState.HOVER;
            this.cbVenue.Name = "cbVenue";
            this.cbVenue.Ripple = true;
            this.cbVenue.Size = new System.Drawing.Size(67, 30);
            this.cbVenue.TabIndex = 5;
            this.cbVenue.Text = "venue";
            this.cbVenue.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label1.Location = new System.Drawing.Point(290, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 22);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select ID ";
            // 
            // textID
            // 
            this.textID.Depth = 0;
            this.textID.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.textID.Hint = "";
            this.textID.Location = new System.Drawing.Point(294, 123);
            this.textID.MouseState = MaterialSkin.MouseState.HOVER;
            this.textID.Name = "textID";
            this.textID.PasswordChar = '\0';
            this.textID.SelectedText = "";
            this.textID.SelectionLength = 0;
            this.textID.SelectionStart = 0;
            this.textID.Size = new System.Drawing.Size(90, 23);
            this.textID.TabIndex = 2;
            this.textID.UseSystemPasswordChar = false;
            // 
            // materialRaisedButton1
            // 
            this.materialRaisedButton1.Depth = 0;
            this.materialRaisedButton1.Location = new System.Drawing.Point(271, 354);
            this.materialRaisedButton1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialRaisedButton1.Name = "materialRaisedButton1";
            this.materialRaisedButton1.Primary = true;
            this.materialRaisedButton1.Size = new System.Drawing.Size(113, 60);
            this.materialRaisedButton1.TabIndex = 6;
            this.materialRaisedButton1.Text = "Cancel";
            this.materialRaisedButton1.UseVisualStyleBackColor = true;
            this.materialRaisedButton1.Click += new System.EventHandler(this.materialRaisedButton1_Click);
            // 
            // mrbCsv
            // 
            this.mrbCsv.Depth = 0;
            this.mrbCsv.Location = new System.Drawing.Point(463, 107);
            this.mrbCsv.MouseState = MaterialSkin.MouseState.HOVER;
            this.mrbCsv.Name = "mrbCsv";
            this.mrbCsv.Primary = true;
            this.mrbCsv.Size = new System.Drawing.Size(113, 60);
            this.mrbCsv.TabIndex = 7;
            this.mrbCsv.Text = "Export to CSV";
            this.mrbCsv.UseVisualStyleBackColor = true;
            this.mrbCsv.Click += new System.EventHandler(this.mrbCsv_Click);
            // 
            // mrbJson
            // 
            this.mrbJson.Depth = 0;
            this.mrbJson.Location = new System.Drawing.Point(463, 188);
            this.mrbJson.MouseState = MaterialSkin.MouseState.HOVER;
            this.mrbJson.Name = "mrbJson";
            this.mrbJson.Primary = true;
            this.mrbJson.Size = new System.Drawing.Size(113, 60);
            this.mrbJson.TabIndex = 8;
            this.mrbJson.Text = "Export to JSON";
            this.mrbJson.UseVisualStyleBackColor = true;
            this.mrbJson.Click += new System.EventHandler(this.mrbJson_Click);
            // 
            // mrbExcel
            // 
            this.mrbExcel.Depth = 0;
            this.mrbExcel.Location = new System.Drawing.Point(463, 274);
            this.mrbExcel.MouseState = MaterialSkin.MouseState.HOVER;
            this.mrbExcel.Name = "mrbExcel";
            this.mrbExcel.Primary = true;
            this.mrbExcel.Size = new System.Drawing.Size(113, 60);
            this.mrbExcel.TabIndex = 9;
            this.mrbExcel.Text = "Export to Excel";
            this.mrbExcel.UseVisualStyleBackColor = true;
            this.mrbExcel.Click += new System.EventHandler(this.mrbExcel_Click);
            // 
            // Export_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 450);
            this.Controls.Add(this.mrbExcel);
            this.Controls.Add(this.mrbJson);
            this.Controls.Add(this.mrbCsv);
            this.Controls.Add(this.materialRaisedButton1);
            this.Controls.Add(this.textID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBoxTables);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Export_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Export_Form";
            this.groupBoxTables.ResumeLayout(false);
            this.groupBoxTables.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxTables;
        private MaterialSkin.Controls.MaterialCheckBox cbProduction;
        private MaterialSkin.Controls.MaterialCheckBox cbContributions;
        private MaterialSkin.Controls.MaterialCheckBox cbEvents;
        private MaterialSkin.Controls.MaterialCheckBox cbVenue;
        private MaterialSkin.Controls.MaterialCheckBox cbPersons;
        private MaterialSkin.Controls.MaterialCheckBox cbOrganizer;
        private System.Windows.Forms.Label label1;
        private MaterialSkin.Controls.MaterialSingleLineTextField textID;
        private MaterialSkin.Controls.MaterialRaisedButton materialRaisedButton1;
        private MaterialSkin.Controls.MaterialRaisedButton mrbCsv;
        private MaterialSkin.Controls.MaterialRaisedButton mrbJson;
        private MaterialSkin.Controls.MaterialRaisedButton mrbExcel;
    }
}