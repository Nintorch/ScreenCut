namespace ScreenCut
{
    partial class MainForm
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
            this.dSaveScreenshot = new System.Windows.Forms.SaveFileDialog();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.lbDraw = new System.Windows.Forms.ListBox();
            this.pDrawSettings = new System.Windows.Forms.Panel();
            this.bCopy = new System.Windows.Forms.Button();
            this.bSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nWidth = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.tbText = new System.Windows.Forms.TextBox();
            this.lSize = new System.Windows.Forms.Label();
            this.pDrawSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // dSaveScreenshot
            // 
            this.dSaveScreenshot.DefaultExt = "png";
            this.dSaveScreenshot.FileName = "Screenshot.png";
            // 
            // lbDraw
            // 
            this.lbDraw.FormattingEnabled = true;
            this.lbDraw.ItemHeight = 16;
            this.lbDraw.Items.AddRange(new object[] {
            "Draw",
            "Line",
            "Text",
            "Rectangle",
            "Arrow",
            "Color Picker"});
            this.lbDraw.Location = new System.Drawing.Point(5, 87);
            this.lbDraw.Margin = new System.Windows.Forms.Padding(4);
            this.lbDraw.Name = "lbDraw";
            this.lbDraw.Size = new System.Drawing.Size(92, 100);
            this.lbDraw.TabIndex = 0;
            this.lbDraw.SelectedIndexChanged += new System.EventHandler(this.LbDraw_SelectedIndexChanged);
            // 
            // pDrawSettings
            // 
            this.pDrawSettings.BackColor = System.Drawing.Color.Transparent;
            this.pDrawSettings.Controls.Add(this.bCopy);
            this.pDrawSettings.Controls.Add(this.bSave);
            this.pDrawSettings.Controls.Add(this.label1);
            this.pDrawSettings.Controls.Add(this.nWidth);
            this.pDrawSettings.Controls.Add(this.button1);
            this.pDrawSettings.Controls.Add(this.lbDraw);
            this.pDrawSettings.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.pDrawSettings.Location = new System.Drawing.Point(52, 50);
            this.pDrawSettings.Margin = new System.Windows.Forms.Padding(4);
            this.pDrawSettings.Name = "pDrawSettings";
            this.pDrawSettings.Size = new System.Drawing.Size(160, 324);
            this.pDrawSettings.TabIndex = 1;
            this.pDrawSettings.Visible = false;
            // 
            // bCopy
            // 
            this.bCopy.Location = new System.Drawing.Point(4, 230);
            this.bCopy.Margin = new System.Windows.Forms.Padding(4);
            this.bCopy.Name = "bCopy";
            this.bCopy.Size = new System.Drawing.Size(92, 28);
            this.bCopy.TabIndex = 5;
            this.bCopy.Text = "Copy";
            this.bCopy.UseVisualStyleBackColor = true;
            this.bCopy.Click += new System.EventHandler(this.BCopy_Click);
            // 
            // bSave
            // 
            this.bSave.Location = new System.Drawing.Point(4, 195);
            this.bSave.Margin = new System.Windows.Forms.Padding(4);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(92, 28);
            this.bSave.TabIndex = 4;
            this.bSave.Text = "Save";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.BSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(1, 36);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Width:";
            // 
            // nWidth
            // 
            this.nWidth.Location = new System.Drawing.Point(5, 55);
            this.nWidth.Margin = new System.Windows.Forms.Padding(4);
            this.nWidth.Name = "nWidth";
            this.nWidth.Size = new System.Drawing.Size(53, 22);
            this.nWidth.TabIndex = 2;
            this.nWidth.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nWidth.ValueChanged += new System.EventHandler(this.NWidth_ValueChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(5, 4);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(55, 28);
            this.button1.TabIndex = 1;
            this.button1.Text = "Color";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // tbText
            // 
            this.tbText.Location = new System.Drawing.Point(17, 16);
            this.tbText.Margin = new System.Windows.Forms.Padding(4);
            this.tbText.Name = "tbText";
            this.tbText.Size = new System.Drawing.Size(505, 22);
            this.tbText.TabIndex = 2;
            this.tbText.Visible = false;
            this.tbText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TbText_KeyDown);
            // 
            // lSize
            // 
            this.lSize.AutoSize = true;
            this.lSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lSize.ForeColor = System.Drawing.SystemColors.Control;
            this.lSize.Location = new System.Drawing.Point(273, 86);
            this.lSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lSize.Name = "lSize";
            this.lSize.Size = new System.Drawing.Size(52, 25);
            this.lSize.TabIndex = 3;
            this.lSize.Text = "XxY";
            this.lSize.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(689, 508);
            this.Controls.Add(this.lSize);
            this.Controls.Add(this.tbText);
            this.Controls.Add(this.pDrawSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.DoubleClick += new System.EventHandler(this.Form1_DoubleClick);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            this.pDrawSettings.ResumeLayout(false);
            this.pDrawSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nWidth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog dSaveScreenshot;
        private System.Windows.Forms.ListBox lbDraw;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Panel pDrawSettings;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nWidth;
        private System.Windows.Forms.TextBox tbText;
        private System.Windows.Forms.Button bCopy;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Label lSize;
    }
}

