
namespace HtmlScraper
{
    partial class MainBoard
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
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvGetrecords = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.lvOrganizersucc = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelMesosoros = new System.Windows.Forms.Label();
            this.lblMo = new System.Windows.Forms.Label();
            this.lbllastdays = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblcountprod = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGetrecords)).BeginInit();
            this.SuspendLayout();
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(362, 71);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(505, 27);
            this.dateTimePicker1.TabIndex = 0;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(12, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(334, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "Επιλέξτε μέρα για εμφάνιση παραστάσεων";
            // 
            // dgvGetrecords
            // 
            this.dgvGetrecords.AllowUserToAddRows = false;
            this.dgvGetrecords.AllowUserToDeleteRows = false;
            this.dgvGetrecords.AllowUserToOrderColumns = true;
            this.dgvGetrecords.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvGetrecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGetrecords.Location = new System.Drawing.Point(16, 137);
            this.dgvGetrecords.Name = "dgvGetrecords";
            this.dgvGetrecords.ReadOnly = true;
            this.dgvGetrecords.Size = new System.Drawing.Size(1283, 227);
            this.dgvGetrecords.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(22, 396);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(576, 21);
            this.label2.TabIndex = 3;
            this.label2.Text = "Ο διοργανωτής με τις περισσότερες αναλαμβάνουσες παραστάσεις είναι :";
            // 
            // lvOrganizersucc
            // 
            this.lvOrganizersucc.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
            this.lvOrganizersucc.HideSelection = false;
            this.lvOrganizersucc.Location = new System.Drawing.Point(16, 433);
            this.lvOrganizersucc.Name = "lvOrganizersucc";
            this.lvOrganizersucc.Size = new System.Drawing.Size(1283, 67);
            this.lvOrganizersucc.TabIndex = 4;
            this.lvOrganizersucc.UseCompatibleStateImageBehavior = false;
            this.lvOrganizersucc.SelectedIndexChanged += new System.EventHandler(this.lvOrganizersucc_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 280;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Address";
            this.columnHeader2.Width = 140;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Town";
            this.columnHeader3.Width = 90;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Postcode";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Phone";
            this.columnHeader5.Width = 125;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Email";
            this.columnHeader6.Width = 160;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "ΔΟΥ";
            this.columnHeader7.Width = 120;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "ΑΦΜ";
            this.columnHeader8.Width = 130;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label3.Location = new System.Drawing.Point(873, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(271, 21);
            this.label3.TabIndex = 5;
            this.label3.Text = "Σύνολο όλων των παραστάσεων :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label4.Location = new System.Drawing.Point(873, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(372, 21);
            this.label4.TabIndex = 6;
            this.label4.Text = "Εισαγωγή παραστάσεων τις τελευταίες 3 μέρες :";
            // 
            // labelMesosoros
            // 
            this.labelMesosoros.AutoSize = true;
            this.labelMesosoros.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelMesosoros.Location = new System.Drawing.Point(22, 367);
            this.labelMesosoros.Name = "labelMesosoros";
            this.labelMesosoros.Size = new System.Drawing.Size(182, 21);
            this.labelMesosoros.TabIndex = 7;
            this.labelMesosoros.Text = "Μέση τιμή εισητηρίων :";
            // 
            // lblMo
            // 
            this.lblMo.AutoSize = true;
            this.lblMo.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblMo.Location = new System.Drawing.Point(210, 367);
            this.lblMo.Name = "lblMo";
            this.lblMo.Size = new System.Drawing.Size(0, 21);
            this.lblMo.TabIndex = 8;
            // 
            // lbllastdays
            // 
            this.lbllastdays.AutoSize = true;
            this.lbllastdays.Location = new System.Drawing.Point(1251, 98);
            this.lbllastdays.Name = "lbllastdays";
            this.lbllastdays.Size = new System.Drawing.Size(0, 21);
            this.lbllastdays.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(655, 295);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 21);
            this.label5.TabIndex = 10;
            // 
            // lblcountprod
            // 
            this.lblcountprod.AutoSize = true;
            this.lblcountprod.Location = new System.Drawing.Point(1150, 71);
            this.lblcountprod.Name = "lblcountprod";
            this.lblcountprod.Size = new System.Drawing.Size(0, 21);
            this.lblcountprod.TabIndex = 11;
            // 
            // MainBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1311, 610);
            this.Controls.Add(this.lblcountprod);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lbllastdays);
            this.Controls.Add(this.lblMo);
            this.Controls.Add(this.labelMesosoros);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lvOrganizersucc);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dgvGetrecords);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePicker1);
            this.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.Name = "MainBoard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainBoard";
            ((System.ComponentModel.ISupportInitialize)(this.dgvGetrecords)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvGetrecords;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView lvOrganizersucc;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelMesosoros;
        private System.Windows.Forms.Label lblMo;
        private System.Windows.Forms.Label lbllastdays;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblcountprod;
    }
}