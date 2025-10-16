using System;
using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp.Views
{
    /// <summary>
    /// Main Menu form (code-only layout).
    /// Part 2 requirements:
    ///  - Report Issues: enabled (opens ReportIssuesForm).
    ///  - Local Events & Announcements: enabled (opens EventsForm).
    ///  - Service Request Status: disabled (reserved for Part 3).
    /// </summary>
    public class MainMenuForm : Form
    {
        /// <summary>
        /// Buttons for the three main features.
        /// </summary>
        private Button btnReportIssues;
        private Button btnEvents;
        private Button btnServiceStatus;

        public MainMenuForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Builds entire UI Designer-free.
        /// </summary>
        private void InitializeComponent()
        {
            // Form window setup
            this.Text = "Municipal Services — Main Menu";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", NinePoint(), FontStyle.Regular, GraphicsUnit.Point);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(420, 260);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Report Issues (ENABLED) — Part 1 feature
            btnReportIssues = new Button
            {
                Name = "btnReportIssues",
                Text = "&Report Issues",
                Location = new Point(40, 40),
                Size = new Size(320, 40),
                TabIndex = 0
            };
            btnReportIssues.Click += BtnReportIssues_Click;

            // Local Events & Announcements (ENABLED) — Part 2 feature.
            // Use & for access key and && to render a literal '&'
            btnEvents = new Button
            {
                Name = "btnEvents",
                Text = "&Local Events && Announcements",
                Location = new Point(40, 100),
                Size = new Size(320, 40),
                TabIndex = 1,
                Enabled = true // enabled for Part 2
            };
            btnEvents.Click += (s, e) =>
            {
                using (var form = new EventsForm())
                {
                    form.ShowDialog(this); // modal: returns to menu after closing
                }
            };

            // Service Request Status (DISABLED) — Part 3
            btnServiceStatus = new Button
            {
                Name = "btnServiceStatus",
                Text = "Service Request Status (Part 3)",
                Location = new Point(40, 160),
                Size = new Size(320, 40),
                TabIndex = 2,
                Enabled = false
            };

            // Add controls to Form
            Controls.Add(btnReportIssues);
            Controls.Add(btnEvents);
            Controls.Add(btnServiceStatus);
        }

        /// <summary>
        /// Ensures consistent 9pt sizing (Segoe UI 9) as per spec.
        /// </summary>
        private float NinePoint() => 9f;

        /// <summary>
        /// Open the ReportIssuesForm as a modal dialog.
        /// </summary>
        private void BtnReportIssues_Click(object sender, EventArgs e)
        {
            using (var form = new ReportIssuesForm())
            {
                form.ShowDialog(this); // modal: returns to menu after closing
            }
        }
    }
}
