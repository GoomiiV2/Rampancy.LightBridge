using Rampancy.LightBridge;
using Serilog;
using Serilog.Core;

namespace Rampancy.Ligthbridge
{
    public partial class MainForm : Form
    {
        public List<Profile> Profiles = new();
        public Profile CurrentProfile = null;
        public Profile EditingProfile = null;
        public Processor Processor = new();

        public MainForm()
        {
            InitializeComponent();

            this.ForeColor = Color.DarkGray;
            this.BackColor = Color.FromArgb(255, 30, 30, 30);
            DarkTheme.ApplyTheme_Dark(this);

            Processor.OnSyncingStarted = SyncingStarted;
            Processor.OnSyncingDone = SyncingStoped;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error("Global Exception: {Exception}", e);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetupLogging();
            Log.Information("{Name} {Version}", Data.DirName, "v0.1");
            LoadProfiles();
        }

        private void GB_Main_Enter(object sender, EventArgs e)
        {

        }

        private void BTN_Settings_Click(object sender, EventArgs e)
        {
            ShowSettings();
        }

        private void BTN_Save_Click(object sender, EventArgs e)
        {
            CurrentProfile = EditingProfile.Copy();
            CurrentProfile.Save(Data.ProfilesDir);
            Processor.SetProfile(CurrentProfile);
        }

        private void BTN_Cancel_Click(object sender, EventArgs e)
        {
            ShowSettings(false);
        }

        private void ShowSettings(bool? show = null)
        {
            if (show ?? GB_Main.Visible)
            {
                GB_Main.Hide();
                GB_Settings.Show();
                EditingProfile = CurrentProfile.Copy();

                TB_HEKLocation.Text = EditingProfile.HEKPath;
                TB_OutputLocation.Text = EditingProfile.OutputPath;
            }
            else
            {
                GB_Main.Show();
                GB_Settings.Hide();
            }
        }

        private void SetupLogging()
        {
            var latestLogPath = Path.Combine(Data.LogsDir, "latest.txt");

            if (File.Exists(latestLogPath))
                File.Delete(latestLogPath);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(Data.LogsDir, "log.txt"), rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:HH:mm:ss} [{Category}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(latestLogPath)
                .WriteTo.TextBoxLogSink(RTB_Log, "[{Level:u4}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

        private void LoadProfiles()
        {
            Profiles = Data.LoadProfiles();
            CB_Profiles.Items.Clear();
            foreach (var profile in Profiles)
            {
                CB_Profiles.Items.Add(profile.Name);
            }

            CB_Profiles.SelectedIndex = 0;
            CurrentProfile = Profiles[0];
            EditingProfile = CurrentProfile.Copy();
        }

        public void SelectProfile(int idx)
        {
            var name = CB_Profiles.Items[idx].ToString();
            CurrentProfile = Profiles.FirstOrDefault(x => x.Name == name);
            if (!CheckIfProfileIsValid(CurrentProfile))
                return;

            Processor.SetProfile(CurrentProfile);

            GB_Main.Text = $"Profile: {name}";
            GB_Settings.Text = $"Settings for {name}";

            TB_HEKLocation.Text = CurrentProfile.HEKPath;
            TB_OutputLocation.Text = CurrentProfile.OutputPath;
        }

        public bool CheckIfProfileIsValid(Profile profile)
        {
            var state = profile.IsValid();

            if (state == ProfileValidState.Valid)
                return true;

            string msgStr = "";
            if (state.HasFlag(ProfileValidState.NoName))
                msgStr += $"Profile ({profile.Name}) doesn't have a name set.\n";

            if (state.HasFlag(ProfileValidState.NoHEKPath))
                msgStr += $"Profile ({profile.Name}) doesn't have the HEK path set.\n";

            if (state.HasFlag(ProfileValidState.NoOutputPath))
                msgStr += $"Profile ({profile.Name}) doesn't have the output path set.\n";

            MessageBox.Show(msgStr, $"Profile ({profile.Name}) wasn't valid.");
            Log.Warning(msgStr);

            return false;
        }

        private void CB_Profiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectProfile(CB_Profiles.SelectedIndex);
        }

        private string BrowseForFolder(string name, string startPath = null)
        {
            var dirBrowser = new FolderBrowserDialog();
            dirBrowser.Description = name;
            dirBrowser.UseDescriptionForTitle = true;
            dirBrowser.SelectedPath = startPath;
            var result = dirBrowser.ShowDialog();
            if (result == DialogResult.OK)
            {
                return dirBrowser.SelectedPath;
            }

            return null;
        }

        private void BTN_BrowseHEKLocation_Click(object sender, EventArgs e)
        {
            var path = BrowseForFolder("Browse for HEK location", EditingProfile.HEKPath);
            TB_HEKLocation.Text = path;
            EditingProfile.HEKPath = path;
        }

        private void BTN_BrowseOuputLocation_Click(object sender, EventArgs e)
        {
            var path = BrowseForFolder("Browse for the location to export to", EditingProfile.OutputPath);
            TB_OutputLocation.Text = path;
            EditingProfile.OutputPath = path;
        }

        private void BTN_Sync_Click(object sender, EventArgs e)
        {
            if (!CheckIfProfileIsValid(CurrentProfile))
                return;

            Processor.Sync(CB_FullSync.Checked);
        }

        private void SyncingStarted()
        {
            BTN_Sync.Enabled     = false;
            BTN_Settings.Enabled = false;
            CB_Profiles.Enabled  = false;
            CB_FullSync.Enabled  = false;
        }

        private void SyncingStoped()
        {
            Invoke(() =>
            {
                BTN_Sync.Enabled     = true;
                BTN_Settings.Enabled = true;
                CB_Profiles.Enabled  = true;
                CB_FullSync.Enabled  = true;
            });
        }
    }
}