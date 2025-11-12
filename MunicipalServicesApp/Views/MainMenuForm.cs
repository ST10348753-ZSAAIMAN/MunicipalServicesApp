using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MunicipalServicesApp.Views
{
    /// <summary>
    /// Main Menu form.
    /// Part 3: fully functional menu providing access to
    ///  - Report Issues (Part 1)
    ///  - Local Events & Announcements (Part 2)
    ///  - Service Request Status (Part 3)
    /// </summary>
    public class MainMenuForm : Form
    {
        private Button btnReportIssues;
        private Button btnEvents;
        private Button btnServiceStatus;
        private PictureBox picLogo;
        private Label lblHeader;

        public MainMenuForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Builds entire UI (Designer-free, code-only).
        /// </summary>
        private void InitializeComponent()
        {
            // === Form setup ===
            this.Text = "Municipal Services — Main Menu";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(460, 340);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            // === Branding header ===
            lblHeader = new Label
            {
                Text = "City of Cape Town Municipal Services",
                Font = new Font("Segoe UI Semibold", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(40, 20)
            };

            // === Buttons ===
            // Report Issues (ENABLED) — Part 1 feature
            btnReportIssues = new Button
            {
                Name = "btnReportIssues",
                Text = "&Report Issues",
                Location = new Point(60, 100),
                Size = new Size(340, 45),
                TabIndex = 0
            };
            btnReportIssues.Click += BtnReportIssues_Click;

            // Local Events & Announcements (ENABLED) — Part 2 feature
            btnEvents = new Button
            {
                Name = "btnEvents",
                Text = "&Local Events && Announcements",
                Location = new Point(60, 165),
                Size = new Size(340, 45),
                TabIndex = 1,
                Enabled = true
            };
            btnEvents.Click += (s, e) =>
            {
                using (var form = new EventsForm())
                {
                    form.ShowDialog(this);
                }
            };

            // Service Request Status (ENABLED) — Part 3 feature
            btnServiceStatus = new Button
            {
                Name = "btnServiceStatus",
                Text = "&Service Request Status",
                Location = new Point(60, 230),
                Size = new Size(340, 45),
                TabIndex = 2,
                Enabled = true
            };
            btnServiceStatus.Click += (s, e) =>
            {
                using (var form = new ServiceStatusForm())
                {
                    form.ShowDialog(this);
                }
            };

            // === Branding: Municipality Logo ===
            picLogo = new PictureBox
            {
                Location = new Point(this.ClientSize.Width - 140, 10),
                Size = new Size(120, 50),
                SizeMode = PictureBoxSizeMode.Zoom,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.Transparent
            };

            try
            {
                // Loads logo dynamically from Assets folder in your project
                string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "municipality_logo.png");
                if (File.Exists(logoPath))
                {
                    picLogo.Image = Image.FromFile(logoPath);
                }
                else
                {
                    // Optional fallback visual cue if logo missing
                    picLogo.BackColor = Color.LightGray;
                    using (Graphics g = Graphics.FromImage(new Bitmap(picLogo.Width, picLogo.Height)))
                    {
                        g.Clear(Color.LightGray);
                    }
                }
            }
            catch
            {
                // Fail gracefully if image cannot load
            }

            // === Add controls ===
            Controls.Add(lblHeader);
            Controls.Add(btnReportIssues);
            Controls.Add(btnEvents);
            Controls.Add(btnServiceStatus);
            Controls.Add(picLogo); // add logo last to keep it on top
        }

        /// <summary>
        /// Opens ReportIssuesForm (modal) — Part 1 feature.
        /// </summary>
        private void BtnReportIssues_Click(object sender, EventArgs e)
        {
            using (var form = new ReportIssuesForm())
            {
                form.ShowDialog(this);
            }
        }
    }
}
