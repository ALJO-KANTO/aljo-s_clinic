using Clinic_DataAccess;
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

namespace Clinic
{
    public partial class frmSplashScreen : Form
    {
        public int NavigationCode { get; private set; } = 0;

        private Timer progressTimer;
        private int progressValue = 0;
        private bool initDone = false;
        private bool progressDone = false;

        public frmSplashScreen()
        {
            InitializeComponent();
        }

        private void frmSplashScreen_Load(object sender, EventArgs e)
        {
            progressValue = 0;
            gunaProgressBar.Value = 0;
            lblStatus.Text = "Initializing...";

            progressTimer = new Timer();
            progressTimer.Interval = 50;
            progressTimer.Tick += ProgressTimer_Tick;
            progressTimer.Start();
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            progressValue += 2;

            if (progressValue >= 100)
            {
                gunaProgressBar.Value = 100;
                progressDone = true;
                progressTimer.Stop();
                TryClose();
            }
            else
            {
                gunaProgressBar.Value = progressValue;
            }
        }
        private void ProceedWithInitialization()
        {
            try
            {
                SafeSetStatus("Checking database connection...");

                string cc = Environment.GetEnvironmentVariable(
                    "clinic_connection",
                    EnvironmentVariableTarget.User
                );

                // No saved connection
                if (string.IsNullOrWhiteSpace(cc))
                {
                    NavigationCode = 1; // frmConfig
                    return;
                }

                // Decrypt connection string
                string connectionString = CryptoHelper.Decrypt(cc);

                // Check MySQL connection
                if (!IsValidConnection(connectionString))
                {
                    NavigationCode = 1; // frmConfig
                    return;
                }

                // Check and apply pending migrations
                SafeSetStatus("Checking database migrations...");

                ClinicDatabase.ApplyMigrations();

                // Everything completed successfully
                NavigationCode = 2; // frmLogin

                SafeSetStatus("Initialization completed.");
            }
            catch (Exception ex)
            {
                // Connection or migration failed
                NavigationCode = 1;

                System.Diagnostics.Debug.WriteLine(
                    "Startup initialization failed: " + ex
                );

                SafeSetStatus("Database initialization failed.");

                // Show error on UI thread
                try
                {
                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                        this.BeginInvoke((MethodInvoker)(() =>
                        {
                            MessageBox.Show(
                                "Database initialization failed:\n\n" +
                                ex.Message +
                                "\n\nPlease check your database configuration.",
                                "Database Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                        }));
                    }
                }
                catch (InvalidOperationException)
                {
                    // Form is closing.
                }
            }
            finally
            {
                initDone = true;
                TryClose();
            }
        }
        private static bool IsValidConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return false;

            try
            {
                using (MySqlConnection conn =
                    ClinicDatabase.GetConnection())
                {
                    conn.Open();

                    return conn.State ==
                        System.Data.ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }
        private void ProceedWithInitializationOld()
        {
            SafeSetStatus("Connecting to database...");
            string cc = Environment.GetEnvironmentVariable("clinic_connection", EnvironmentVariableTarget.User);
            string connectionString = string.IsNullOrEmpty(cc) ? "" : CryptoHelper.Decrypt(cc);

            if (!IsValidConnection(connectionString))
            {
                NavigationCode = 1; // Go to frmConfig
            }
            else
            {
                NavigationCode = 2;
            }

            SafeSetStatus("Finishing up...");

            initDone = true;
            TryClose();
        }
        static bool IsValidConnectionOld(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) return false;

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open(); // attempt connection
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }
        private void TryClose()
        {
            if (initDone && progressDone)
            {
                if (this.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        this.DialogResult = DialogResult.OK;
                    }));
                }
                else
                {
                    // Delay the close until the form is fully initialized
                    this.Shown += (s, e) =>
                    {
                        this.DialogResult = DialogResult.OK;
                    };
                }
            }
        }

        private void SafeSetStatus(string text)
        {
            if (lblStatus != null && !lblStatus.IsDisposed && lblStatus.IsHandleCreated)
            {
                try
                {
                    lblStatus.Invoke((MethodInvoker)(() => lblStatus.Text = text));
                }
                catch (ObjectDisposedException)
                {
                    // Form was disposed — safe to ignore
                }
            }
        }

        private void frmSplashScreen_Shown(object sender, EventArgs e)
        {
            Task.Run(() => ProceedWithInitialization());
        }
    }
}
