using System;
using System.Drawing;
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

            // Optional placeholder logo (you can replace ImageLocation)
            picLogo = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point(380, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            // Example: picLogo.ImageLocation = "Resources/municipality_logo.png";

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

            // === Add controls ===
            Controls.Add(lblHeader);
            Controls.Add(picLogo);
            Controls.Add(btnReportIssues);
            Controls.Add(btnEvents);
            Controls.Add(btnServiceStatus);
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
