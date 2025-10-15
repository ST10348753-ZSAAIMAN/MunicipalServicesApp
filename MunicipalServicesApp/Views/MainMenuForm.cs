using System;
using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp.Views
{
    /// <summary>
    /// Startup menu with three tasks (two disabled for Part 1).
    /// Designed code-only: no Designer files.
    /// </summary>
    public class MainMenuForm : Form
    {
        private Button btnReportIssues;
        private Button btnEvents;
        private Button btnServiceStatus;

        public MainMenuForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Builds the entire UI tree in code (Designer-free).
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

            // Report Issues (enabled for Part 1)
            btnReportIssues = new Button
            {
                Name = "btnReportIssues",
                Text = "&Report Issues",
                Location = new Point(40, 40),
                Size = new Size(320, 40),
                TabIndex = 0
            };
            btnReportIssues.Click += BtnReportIssues_Click;

            // Local Events & Announcements (placeholder, disabled in Part 1)
            btnEvents = new Button
            {
                Name = "btnEvents",
                Text = "Local Events & Announcements (Part 2)",
                Location = new Point(40, 100),
                Size = new Size(320, 40),
                TabIndex = 1,
                Enabled = false
            };

            // Service Request Status (placeholder, disabled in Part 1)
            btnServiceStatus = new Button
            {
                Name = "btnServiceStatus",
                Text = "Service Request Status (Part 2)",
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
