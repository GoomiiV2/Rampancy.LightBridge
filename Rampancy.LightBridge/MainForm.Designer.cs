namespace Rampancy.Ligthbridge
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            BTN_Settings = new Button();
            CB_Profiles = new ComboBox();
            GB_Settings = new GroupBox();
            BTN_Cancel = new Button();
            BTN_Save = new Button();
            BTN_BrowseOuputLocation = new Button();
            TB_OutputLocation = new TextBox();
            LBL_OutputLocation = new Label();
            BTN_BrowseHEKLocation = new Button();
            TB_HEKLocation = new TextBox();
            LBL_HEKLocation = new Label();
            GB_Main = new GroupBox();
            BTN_Sync = new Button();
            RTB_Log = new RichTextBox();
            PB_Icon = new PictureBox();
            CB_FullSync = new CheckBox();
            GB_Settings.SuspendLayout();
            GB_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PB_Icon).BeginInit();
            SuspendLayout();
            // 
            // BTN_Settings
            // 
            BTN_Settings.BackgroundImage = (Image)resources.GetObject("BTN_Settings.BackgroundImage");
            BTN_Settings.BackgroundImageLayout = ImageLayout.Zoom;
            BTN_Settings.FlatAppearance.BorderSize = 0;
            BTN_Settings.FlatStyle = FlatStyle.Flat;
            BTN_Settings.ImageAlign = ContentAlignment.MiddleLeft;
            BTN_Settings.Location = new Point(759, 12);
            BTN_Settings.Name = "BTN_Settings";
            BTN_Settings.Padding = new Padding(2);
            BTN_Settings.Size = new Size(23, 23);
            BTN_Settings.TabIndex = 0;
            BTN_Settings.UseVisualStyleBackColor = true;
            BTN_Settings.Click += BTN_Settings_Click;
            // 
            // CB_Profiles
            // 
            CB_Profiles.FormattingEnabled = true;
            CB_Profiles.Location = new Point(408, 12);
            CB_Profiles.Name = "CB_Profiles";
            CB_Profiles.Size = new Size(345, 23);
            CB_Profiles.TabIndex = 1;
            CB_Profiles.SelectedIndexChanged += CB_Profiles_SelectedIndexChanged;
            // 
            // GB_Settings
            // 
            GB_Settings.Controls.Add(BTN_Cancel);
            GB_Settings.Controls.Add(BTN_Save);
            GB_Settings.Controls.Add(BTN_BrowseOuputLocation);
            GB_Settings.Controls.Add(TB_OutputLocation);
            GB_Settings.Controls.Add(LBL_OutputLocation);
            GB_Settings.Controls.Add(BTN_BrowseHEKLocation);
            GB_Settings.Controls.Add(TB_HEKLocation);
            GB_Settings.Controls.Add(LBL_HEKLocation);
            GB_Settings.Location = new Point(5, 47);
            GB_Settings.Name = "GB_Settings";
            GB_Settings.Size = new Size(790, 400);
            GB_Settings.TabIndex = 2;
            GB_Settings.TabStop = false;
            GB_Settings.Text = "Settings";
            // 
            // BTN_Cancel
            // 
            BTN_Cancel.Location = new Point(403, 371);
            BTN_Cancel.Name = "BTN_Cancel";
            BTN_Cancel.Size = new Size(382, 23);
            BTN_Cancel.TabIndex = 7;
            BTN_Cancel.Text = "Cancel";
            BTN_Cancel.UseVisualStyleBackColor = true;
            BTN_Cancel.Click += BTN_Cancel_Click;
            // 
            // BTN_Save
            // 
            BTN_Save.Location = new Point(5, 371);
            BTN_Save.Name = "BTN_Save";
            BTN_Save.Size = new Size(384, 23);
            BTN_Save.TabIndex = 6;
            BTN_Save.Text = "Save";
            BTN_Save.UseVisualStyleBackColor = true;
            BTN_Save.Click += BTN_Save_Click;
            // 
            // BTN_BrowseOuputLocation
            // 
            BTN_BrowseOuputLocation.Location = new Point(661, 66);
            BTN_BrowseOuputLocation.Name = "BTN_BrowseOuputLocation";
            BTN_BrowseOuputLocation.Size = new Size(123, 23);
            BTN_BrowseOuputLocation.TabIndex = 5;
            BTN_BrowseOuputLocation.Text = "Browse";
            BTN_BrowseOuputLocation.UseVisualStyleBackColor = true;
            BTN_BrowseOuputLocation.Click += BTN_BrowseOuputLocation_Click;
            // 
            // TB_OutputLocation
            // 
            TB_OutputLocation.Location = new Point(156, 66);
            TB_OutputLocation.Name = "TB_OutputLocation";
            TB_OutputLocation.Size = new Size(499, 23);
            TB_OutputLocation.TabIndex = 4;
            // 
            // LBL_OutputLocation
            // 
            LBL_OutputLocation.AutoSize = true;
            LBL_OutputLocation.Location = new Point(7, 70);
            LBL_OutputLocation.Name = "LBL_OutputLocation";
            LBL_OutputLocation.Size = new Size(100, 15);
            LBL_OutputLocation.TabIndex = 3;
            LBL_OutputLocation.Text = "Output Location :";
            // 
            // BTN_BrowseHEKLocation
            // 
            BTN_BrowseHEKLocation.Location = new Point(661, 27);
            BTN_BrowseHEKLocation.Name = "BTN_BrowseHEKLocation";
            BTN_BrowseHEKLocation.Size = new Size(123, 23);
            BTN_BrowseHEKLocation.TabIndex = 2;
            BTN_BrowseHEKLocation.Text = "Browse";
            BTN_BrowseHEKLocation.UseVisualStyleBackColor = true;
            BTN_BrowseHEKLocation.Click += BTN_BrowseHEKLocation_Click;
            // 
            // TB_HEKLocation
            // 
            TB_HEKLocation.Location = new Point(156, 27);
            TB_HEKLocation.Name = "TB_HEKLocation";
            TB_HEKLocation.Size = new Size(499, 23);
            TB_HEKLocation.TabIndex = 1;
            // 
            // LBL_HEKLocation
            // 
            LBL_HEKLocation.AutoSize = true;
            LBL_HEKLocation.Location = new Point(7, 31);
            LBL_HEKLocation.Name = "LBL_HEKLocation";
            LBL_HEKLocation.Size = new Size(84, 15);
            LBL_HEKLocation.TabIndex = 0;
            LBL_HEKLocation.Text = "HEK Location :";
            // 
            // GB_Main
            // 
            GB_Main.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            GB_Main.Controls.Add(CB_FullSync);
            GB_Main.Controls.Add(BTN_Sync);
            GB_Main.Controls.Add(RTB_Log);
            GB_Main.Location = new Point(5, 47);
            GB_Main.Name = "GB_Main";
            GB_Main.Size = new Size(790, 400);
            GB_Main.TabIndex = 3;
            GB_Main.TabStop = false;
            GB_Main.Text = "Profile: Halo CE TrenchBroom";
            GB_Main.Enter += GB_Main_Enter;
            // 
            // BTN_Sync
            // 
            BTN_Sync.Location = new Point(697, 14);
            BTN_Sync.Name = "BTN_Sync";
            BTN_Sync.Size = new Size(88, 75);
            BTN_Sync.TabIndex = 1;
            BTN_Sync.Text = "Sync";
            BTN_Sync.UseVisualStyleBackColor = true;
            BTN_Sync.Click += BTN_Sync_Click;
            // 
            // RTB_Log
            // 
            RTB_Log.Location = new Point(0, 95);
            RTB_Log.Name = "RTB_Log";
            RTB_Log.ReadOnly = true;
            RTB_Log.Size = new Size(790, 305);
            RTB_Log.TabIndex = 0;
            RTB_Log.Text = "";
            // 
            // PB_Icon
            // 
            PB_Icon.Image = (Image)resources.GetObject("PB_Icon.Image");
            PB_Icon.Location = new Point(5, 0);
            PB_Icon.Name = "PB_Icon";
            PB_Icon.Size = new Size(790, 55);
            PB_Icon.SizeMode = PictureBoxSizeMode.StretchImage;
            PB_Icon.TabIndex = 4;
            PB_Icon.TabStop = false;
            // 
            // CB_FullSync
            // 
            CB_FullSync.AutoSize = true;
            CB_FullSync.Location = new Point(596, 66);
            CB_FullSync.Name = "CB_FullSync";
            CB_FullSync.Size = new Size(95, 19);
            CB_FullSync.TabIndex = 2;
            CB_FullSync.Text = "Force Resync";
            CB_FullSync.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(800, 450);
            Controls.Add(GB_Main);
            Controls.Add(GB_Settings);
            Controls.Add(CB_Profiles);
            Controls.Add(BTN_Settings);
            Controls.Add(PB_Icon);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MainForm";
            Text = "Rampancy.LightBridge";
            Load += MainForm_Load;
            GB_Settings.ResumeLayout(false);
            GB_Settings.PerformLayout();
            GB_Main.ResumeLayout(false);
            GB_Main.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)PB_Icon).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button BTN_Settings;
        private ComboBox CB_Profiles;
        private GroupBox GB_Settings;
        private GroupBox GB_Main;
        private Button BTN_BrowseOuputLocation;
        private TextBox TB_OutputLocation;
        private Label LBL_OutputLocation;
        private Button BTN_BrowseHEKLocation;
        private TextBox TB_HEKLocation;
        private Label LBL_HEKLocation;
        private Button BTN_Cancel;
        private Button BTN_Save;
        private RichTextBox RTB_Log;
        private PictureBox PB_Icon;
        private Button BTN_Sync;
        private CheckBox CB_FullSync;
    }
}