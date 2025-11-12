using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MunicipalServicesApp.Services;
using MunicipalServicesApp.Utilities;

namespace MunicipalServicesApp.Views
{
    /// <summary>
    /// Implements the "Report Issues" workflow.
    /// - Fields: Location, Category, Description
    /// - Optional attachment via OpenFileDialog
    /// - Engagement feedback (ProgressBar + message)
    /// - Validations with friendly MessageBoxes
    /// - Diagnostics menu triggers recursion demo (non-blocking)
    /// </summary>
    public class ReportIssuesForm : Form
    {
        // --- UI fields ---
        private GroupBox grpIssueDetails;
        private Label lblLocation;
        private TextBox txtLocation;
        private Label lblCategory;
        private ComboBox cmbCategory;
        private Label lblDescription;
        private RichTextBox rtbDescription;

        private GroupBox grpMedia;
        private Button btnAttach;
        private Label lblAttachment;

        private ProgressBar prgEngage;
        private Label lblEngage;

        private Button btnSubmit;
        private Button btnBack;

        private MenuStrip menu;
        private ToolStripMenuItem toolsMenu;
        private ToolStripMenuItem diagnosticsItem;

        // --- Branding ---
        private PictureBox picLogo;   // <-- added

        // Holds the chosen attachment path (optional).
        private string _attachmentPath;

        // Fixed categories for Part 1 (SA context).
        private readonly string[] _categories = new[]
        {
            "Sanitation", "Roads", "Utilities", "Community Safety", "Other"
        };

        public ReportIssuesForm()
        {
            InitializeComponent();
            WireEvents();
        }

        /// <summary>
        /// Builds the UI tree entirely in code (Designer-free).
        /// Sets tab order, access keys (& in labels/buttons), and sizing.
        /// </summary>
        private void InitializeComponent()
        {
            // Base form setup
            this.Text = "Report an Issue";
            this.StartPosition = FormStartPosition.CenterParent; // center relative to parent
            this.Font = new Font("Segoe UI", 9f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(640, 540);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // --- Menu (Diagnostics → Recursion Check) ---
            menu = new MenuStrip();
            toolsMenu = new ToolStripMenuItem("&Tools");
            diagnosticsItem = new ToolStripMenuItem("&Diagnostics (Recursion Check)");
            toolsMenu.DropDownItems.Add(diagnosticsItem);
            menu.Items.Add(toolsMenu);
            this.MainMenuStrip = menu;

            // --- Group: Issue Details ---
            grpIssueDetails = new GroupBox
            {
                Text = "Issue Details",
                Location = new Point(20, 40),
                Size = new Size(600, 240)
            };

            lblLocation = new Label
            {
                Text = "&Location",
                Location = new Point(20, 35),
                AutoSize = true,
                TabIndex = 0
            };
            txtLocation = new TextBox
            {
                Location = new Point(120, 32),
                Size = new Size(440, 24),
                TabIndex = 1
            };

            lblCategory = new Label
            {
                Text = "&Category",
                Location = new Point(20, 75),
                AutoSize = true,
                TabIndex = 2
            };
            cmbCategory = new ComboBox
            {
                Location = new Point(120, 72),
                Size = new Size(200, 24),
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 3
            };
            cmbCategory.Items.AddRange(_categories);

            lblDescription = new Label
            {
                Text = "&Description",
                Location = new Point(20, 115),
                AutoSize = true,
                TabIndex = 4
            };
            rtbDescription = new RichTextBox
            {
                Location = new Point(120, 112),
                Size = new Size(440, 100),
                TabIndex = 5
            };

            grpIssueDetails.Controls.Add(lblLocation);
            grpIssueDetails.Controls.Add(txtLocation);
            grpIssueDetails.Controls.Add(lblCategory);
            grpIssueDetails.Controls.Add(cmbCategory);
            grpIssueDetails.Controls.Add(lblDescription);
            grpIssueDetails.Controls.Add(rtbDescription);

            // --- Group: Media ---
            grpMedia = new GroupBox
            {
                Text = "Media",
                Location = new Point(20, 290),
                Size = new Size(600, 90)
            };

            btnAttach = new Button
            {
                Text = "&Attach...",
                Location = new Point(20, 35),
                Size = new Size(100, 28),
                TabIndex = 6
            };

            lblAttachment = new Label
            {
                Text = "No file selected",
                Location = new Point(130, 40),
                AutoSize = true,
                TabIndex = 7
            };

            grpMedia.Controls.Add(btnAttach);
            grpMedia.Controls.Add(lblAttachment);

            // --- Engagement: progress + message ---
            prgEngage = new ProgressBar
            {
                Location = new Point(20, 390),
                Size = new Size(600, 20),
                TabIndex = 8,
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };

            lblEngage = new Label
            {
                Text = "Welcome! Start by entering the location.",
                Location = new Point(20, 415),
                Size = new Size(600, 20),
                AutoSize = false,
                TabIndex = 9
            };

            // --- Action buttons ---
            btnSubmit = new Button
            {
                Text = "&Submit",
                Location = new Point(420, 450),
                Size = new Size(100, 30),
                TabIndex = 10
            };
            btnBack = new Button
            {
                Text = "&Back",
                Location = new Point(530, 450),
                Size = new Size(90, 30),
                TabIndex = 11
            };

            // Keyboard conveniences:
            this.AcceptButton = btnSubmit; // Enter triggers submit
            this.CancelButton = btnBack;   // Esc triggers back/close

            // Add everything to the form
            Controls.Add(menu);
            Controls.Add(grpIssueDetails);
            Controls.Add(grpMedia);
            Controls.Add(prgEngage);
            Controls.Add(lblEngage);
            Controls.Add(btnSubmit);
            Controls.Add(btnBack);

            // Optional: run the recursion diagnostic silently on load (non-blocking).
            this.Load += (s, e) =>
            {
                try
                {
                    var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    _ = FileGuards.SafeDirectoryDepth(docs, maxDepth: 10);
                }
                catch
                {
                    // ignore; non-critical
                }
            };

            // --- Branding: Municipality Logo (top-right) ---
            picLogo = new PictureBox
            {
                // 10px below top edge; 140px from right edge for width 120
                Location = new Point(this.ClientSize.Width - 140, 8),
                Size = new Size(120, 40),
                SizeMode = PictureBoxSizeMode.Zoom,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.Transparent
            };
            try
            {
                string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "municipality_logo.png");
                if (File.Exists(logoPath))
                {
                    picLogo.Image = Image.FromFile(logoPath);
                }
                else
                {
                    // Optional fallback visual (keeps layout predictable if file is missing)
                    picLogo.BackColor = Color.LightGray;
                }
            }
            catch
            {
                // Fail gracefully—branding should never block form creation
            }
            Controls.Add(picLogo);
            // --- End branding ---
        }

        /// <summary>
        /// Wire all event handlers here to keep InitializeComponent focused on layout only.
        /// </summary>
        private void WireEvents()
        {
            btnAttach.Click += btnAttach_Click;
            btnSubmit.Click += btnSubmit_Click;
            btnBack.Click += btnBack_Click;

            // Lightweight change handlers update engagement feedback in real-time.
            txtLocation.TextChanged += (_, __) => UpdateEngagement();
            cmbCategory.SelectedIndexChanged += (_, __) => UpdateEngagement();
            rtbDescription.TextChanged += (_, __) => UpdateEngagement();

            // Menu → Diagnostics triggers the recursion demo and shows a friendly result.
            diagnosticsItem.Click += (_, __) =>
            {
                try
                {
                    var root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var ok = FileGuards.SafeDirectoryDepth(root, 10);
                    MessageBox.Show(this,
                        ok ? "Diagnostics passed: directory depth within safe limits."
                           : "Diagnostics warning: excessive directory depth detected.",
                        "Diagnostics",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show(this,
                        "Diagnostics encountered an error but the app remains fully functional.",
                        "Diagnostics",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
        }

        /// <summary>
        /// Recompute progress percentage and update the encouragement label.
        /// </summary>
        private void UpdateEngagement()
        {
            bool hasLocation = Validation.Required(txtLocation.Text);
            bool hasCategory = cmbCategory.SelectedItem != null;
            bool hasDescription = Validation.Required(rtbDescription.Text, minLen: 5, maxLen: 1000);
            bool hasAttachment = !string.IsNullOrWhiteSpace(_attachmentPath);

            int percent = EngagementMessages.ComputeCompletionPercent(
                hasLocation, hasCategory, hasDescription, hasAttachment);

            // Clamp just in case.
            prgEngage.Value = Math.Max(prgEngage.Minimum, Math.Min(prgEngage.Maximum, percent));
            lblEngage.Text = EngagementMessages.PickMessage(percent);
        }

        /// <summary>
        /// Opens the file dialog via MediaService and, if successful, stores the path and displays the file name.
        /// </summary>
        private void btnAttach_Click(object sender, EventArgs e)
        {
            if (MediaService.TrySelectAttachment(this, out var path))
            {
                _attachmentPath = path;
                lblAttachment.Text = Path.GetFileName(path);
            }
            // If user cancels or validation fails, keep previous state.
            UpdateEngagement();
        }

        /// <summary>
        /// Validates inputs and, on success, writes a new IssueItem into the repository.
        /// Shows friendly messages and clears the form for the next entry.
        /// </summary>
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            var location = txtLocation.Text?.Trim();
            var category = cmbCategory.SelectedItem?.ToString();
            var description = rtbDescription.Text?.Trim();

            // 1) Location
            if (!Validation.Required(location))
            {
                MessageBox.Show(this, "Please enter a valid location (minimum 2 characters).",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLocation.Focus();
                return;
            }

            // 2) Category
            if (!Validation.ValidCategory(category, _categories))
            {
                MessageBox.Show(this, "Please select a valid category.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCategory.Focus();
                return;
            }

            // 3) Description
            if (!Validation.Required(description, minLen: 5, maxLen: 1000))
            {
                MessageBox.Show(this, "Please enter a short description (minimum 5 characters).",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rtbDescription.Focus();
                return;
            }

            // 4) Store in repository (List<T>) using overloaded Add.
            var repo = IssueRepository.Instance;
            repo.Add(location, category, description, _attachmentPath);

            // 5) User feedback and reset the form for next submission.
            MessageBox.Show(this,
                "Thank you. Your issue has been recorded.",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ClearForm();
        }

        /// <summary>
        /// Resets all inputs and engagement state.
        /// </summary>
        private void ClearForm()
        {
            txtLocation.Text = string.Empty;
            cmbCategory.SelectedIndex = -1;
            rtbDescription.Clear();
            _attachmentPath = null;
            lblAttachment.Text = "No file selected";
            UpdateEngagement();
        }

        /// <summary>
        /// Close the dialog and return to Main Menu.
        /// </summary>
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
