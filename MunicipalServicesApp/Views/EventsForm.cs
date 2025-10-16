using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;
using MunicipalServicesApp.Recommendations;

namespace MunicipalServicesApp.Views
{
    /// <summary>
    /// Implements Part 2 controls and event wiring.
    /// </summary>
    public class EventsForm : Form
    {
        // --- Search Area Controls ---
        private TextBox txtSearch;                 // Text search terms
        private ComboBox cmbCategory;              // Category filter (DropDownList)
        private DateTimePicker dtpFrom, dtpTo;     // Inclusive date range
        private Button btnSearch;                  // Execute search

        // --- Events List ---
        private ListView lvEvents;                 // Results list
        private ColumnHeader colTitle, colCategory, colDate, colLocation, colPriority;

        // --- Recommendations Panel ---
        private GroupBox grpRecommendations;
        private ListBox lstRecommended;

        // --- Navigation ---
        private Button btnBack;                    // Return to Main Menu

        // Repositories/services (injected via singleton for brevity)
        private readonly EventRepository _repo = EventRepository.Instance;
        private readonly SearchAnalyticsService _analytics = SearchAnalyticsService.Instance;

        public EventsForm()
        {
            InitializeComponent();   // Build UI
            WireEvents();            // Hook up events
            SafeLoad();              // Seed and first render
        }

        /// <summary>
        /// Build all controls without the Designer.
        /// </summary>
        private void InitializeComponent()
        {
            // Base window settings
            this.Text = "Local Events & Announcements";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(900, 560);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // --- Search area labels/controls ---
            var lblSearch = new Label
            {
                Text = "&Search",
                Location = new Point(20, 20),
                AutoSize = true,
                TabIndex = 0
            };
            txtSearch = new TextBox
            {
                Name = "txtSearch",
                Location = new Point(80, 17),
                Size = new Size(240, 24),
                TabIndex = 1
            };

            var lblCategory = new Label
            {
                Text = "&Category",
                Location = new Point(340, 20),
                AutoSize = true,
                TabIndex = 2
            };
            cmbCategory = new ComboBox
            {
                Name = "cmbCategory",
                Location = new Point(410, 17),
                Size = new Size(160, 24),
                DropDownStyle = ComboBoxStyle.DropDownList, 
                TabIndex = 3
            };

            var lblFrom = new Label
            {
                Text = "F&rom",
                Location = new Point(590, 20),
                AutoSize = true,
                TabIndex = 4
            };
            dtpFrom = new DateTimePicker
            {
                Name = "dtpFrom",
                Location = new Point(630, 17),
                Size = new Size(120, 24),
                Format = DateTimePickerFormat.Short,
                TabIndex = 5
            };

            var lblTo = new Label
            {
                Text = "&To",
                Location = new Point(760, 20),
                AutoSize = true,
                TabIndex = 6
            };
            dtpTo = new DateTimePicker
            {
                Name = "dtpTo",
                Location = new Point(790, 17),
                Size = new Size(120, 24),
                Format = DateTimePickerFormat.Short,
                TabIndex = 7
            };

            btnSearch = new Button
            {
                Name = "btnSearch",
                Text = "&Search",
                Location = new Point(20, 50),
                Size = new Size(90, 28),
                TabIndex = 8
            };
            this.AcceptButton = btnSearch;

            // --- Events list ---
            lvEvents = new ListView
            {
                Name = "lvEvents",
                Location = new Point(20, 90),
                Size = new Size(600, 400),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                TabIndex = 9
            };
            colTitle = new ColumnHeader { Text = "Title", Width = 160 };
            colCategory = new ColumnHeader { Text = "Category", Width = 100 };
            colDate = new ColumnHeader { Text = "Date", Width = 90 };
            colLocation = new ColumnHeader { Text = "Location", Width = 150 };
            colPriority = new ColumnHeader { Text = "Priority", Width = 80 };
            lvEvents.Columns.AddRange(new[] { colTitle, colCategory, colDate, colLocation, colPriority });

            lvEvents.MultiSelect = false;
            lvEvents.HeaderStyle = ColumnHeaderStyle.Clickable;

            // --- Recommendations panel ---
            grpRecommendations = new GroupBox
            {
                Name = "grpRecommendations",
                Text = "Recommended for you",
                Location = new Point(640, 90),
                Size = new Size(270, 400),
                TabIndex = 10
            };
            lstRecommended = new ListBox
            {
                Name = "lstRecommended",
                Location = new Point(15, 25),
                Size = new Size(240, 360),
                TabIndex = 11
            };
            grpRecommendations.Controls.Add(lstRecommended);

            // --- Navigation ---
            btnBack = new Button
            {
                Name = "btnBack",
                Text = "&Back",
                Location = new Point(820, 510),
                Size = new Size(90, 28),
                TabIndex = 12
            };
            this.CancelButton = btnBack; // ESC will trigger Back

            // Add controls to form
            Controls.AddRange(new Control[]
            {
                lblSearch, txtSearch, lblCategory, cmbCategory, lblFrom, dtpFrom, lblTo, dtpTo, btnSearch,
                lvEvents, grpRecommendations, btnBack
            });
        }

        /// <summary>
        /// Wire up all events.
        /// </summary>
        private void WireEvents()
        {
            // Search button click
            btnSearch.Click += btnSearch_Click;

            // Category change triggers refresh
            cmbCategory.SelectedIndexChanged += cmbCategory_SelectedIndexChanged;

            // Either date change triggers refresh 
            dtpFrom.ValueChanged += dtpFrom_ValueChanged;
            dtpTo.ValueChanged += dtpTo_ValueChanged;

            // Double-click on a result shows details
            lvEvents.DoubleClick += lvEvents_DoubleClick;

            // Back to Main Menu
            btnBack.Click += btnBack_Click;
        }

        /// <summary>
        /// First load: seed repository (if needed), populate categories, default dates, and initial results.
        /// </summary>
        private void SafeLoad()
        {
            try
            {
                // Ensure demo data is available
                _repo.Seed();

                // Populate categories 
                cmbCategory.Items.Clear();
                cmbCategory.Items.Add("");
                foreach (var cat in _repo.GetAllCategories())
                    cmbCategory.Items.Add(cat);
                cmbCategory.SelectedIndex = 0;

                // Default: today to +90 days
                dtpFrom.Value = DateTime.Today;
                dtpTo.Value = DateTime.Today.AddDays(90);

                RefreshResultsAndRecommendations();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to load events: " + ex.Message, "Load Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // =======================
        // Event handler methods
        // =======================

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // Record search to analytics 
            _analytics.LogSearch(txtSearch.Text);
            RefreshResultsAndRecommendations();
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshResultsAndRecommendations();
        }

        private void dtpFrom_ValueChanged(object sender, EventArgs e)
        {
            RefreshResultsAndRecommendations();
        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            RefreshResultsAndRecommendations();
        }

        private void lvEvents_DoubleClick(object sender, EventArgs e)
        {
            if (lvEvents.SelectedItems.Count == 0) return;
            var tag = lvEvents.SelectedItems[0].Tag as EventItem;
            if (tag == null) return;

            // Simple details dialog
            MessageBox.Show(this,
                $"{tag.Title}\n\n" +
                $"Category: {tag.Category}\nDate: {tag.Date:yyyy-MM-dd}\n" +
                $"Location: {tag.Location}\nPriority: {tag.PriorityLabel}\n\n" +
                $"{tag.Description}",
                "Event Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close(); // Return to Main Menu (dialog closes)
        }

        // ===============================
        // Helper: Refresh results + recs
        // ===============================

        /// <summary>
        /// Re-runs the search pipeline 
        /// Ensures date range is inclusive (from <= date <= to).
        /// </summary>
        private void RefreshResultsAndRecommendations()
        {
            var rawCat = (cmbCategory.SelectedItem ?? "").ToString();
            var category = string.IsNullOrWhiteSpace(rawCat) ? null : rawCat;
            var from = dtpFrom.Value.Date;
            var to = dtpTo.Value.Date;
            if (to < from) to = from;

            // Filter via repository
            var results = _repo.Search(txtSearch.Text, category, from, to).ToList();

            // Fill ListView
            lvEvents.BeginUpdate();
            lvEvents.Items.Clear();

            if (results.Count == 0)
            {
                // "no results" row
                lvEvents.Items.Add(new ListViewItem(new[] { "No events found", "", "", "", "" }));
            }
            else
            {
                foreach (var e in results.OrderBy(ev => ev.Date))
                {
                    var row = new ListViewItem(new[]
                    {
                        e.Title,
                        e.Category,
                        e.Date.ToString("yyyy-MM-dd", new CultureInfo("en-ZA")),
                        e.Location,
                        e.PriorityLabel
                    })
                    {
                        Tag = e // store full object for double-click details
                    };
                    lvEvents.Items.Add(row);
                }
            }

            lvEvents.EndUpdate();

            // Build recommendations from current search tokens
            var tokens = SearchAnalyticsService.Tokenize(txtSearch.Text);
            var recs = RecommendationEngine.Recommend(_repo.GetAll(), tokens, maxResults: 6).ToList();

            // Render recommendations
            lstRecommended.Items.Clear();
            foreach (var r in recs)
                lstRecommended.Items.Add($"{r.Title}  ({r.Category}, {r.Date:yyyy-MM-dd}, {r.PriorityLabel})");
        }
    }
}
