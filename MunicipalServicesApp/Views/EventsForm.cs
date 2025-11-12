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
    /// Local Events & Announcements (Part 2):
    /// - Search, category, date range filters
    /// - Clickable column headers to sort (Title/Category/Date/Location/Priority)
    /// - Recommendations panel based on simple analytics
    /// Designer-free (InitializeComponent builds all controls).
    /// 
    /// This version is C# 7.3 compatible (no switch expressions or newer features).
    /// </summary>
    public class EventsForm : Form
    {
        // --- Search Area Controls ---
        private TextBox txtSearch;
        private ComboBox cmbCategory;
        private DateTimePicker dtpFrom, dtpTo;
        private Button btnSearch;

        // --- Events List ---
        private ListView lvEvents;
        private ColumnHeader colTitle, colCategory, colDate, colLocation, colPriority;

        // --- Recommendations Panel ---
        private GroupBox grpRecommendations;
        private ListBox lstRecommended;

        // --- Navigation ---
        private Button btnBack;

        // Sorting state (default = Date ascending)
        private int _sortColumn = 2;   // Date column index
        private bool _sortAsc = true;

        // Repositories/services
        private readonly EventRepository _repo = EventRepository.Instance;
        private readonly SearchAnalyticsService _analytics = SearchAnalyticsService.Instance;

        public EventsForm()
        {
            InitializeComponent();
            WireEvents();
            SafeLoad();
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
            var lblSearch = new Label { Text = "&Search", Location = new Point(20, 20), AutoSize = true, TabIndex = 0 };
            txtSearch = new TextBox { Name = "txtSearch", Location = new Point(80, 17), Size = new Size(240, 24), TabIndex = 1 };

            var lblCategory = new Label { Text = "&Category", Location = new Point(340, 20), AutoSize = true, TabIndex = 2 };
            cmbCategory = new ComboBox
            {
                Name = "cmbCategory",
                Location = new Point(410, 17),
                Size = new Size(160, 24),
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 3
            };

            var lblFrom = new Label { Text = "F&rom", Location = new Point(590, 20), AutoSize = true, TabIndex = 4 };
            dtpFrom = new DateTimePicker { Name = "dtpFrom", Location = new Point(630, 17), Size = new Size(120, 24), Format = DateTimePickerFormat.Short, TabIndex = 5 };

            var lblTo = new Label { Text = "&To", Location = new Point(760, 20), AutoSize = true, TabIndex = 6 };
            dtpTo = new DateTimePicker { Name = "dtpTo", Location = new Point(790, 17), Size = new Size(120, 24), Format = DateTimePickerFormat.Short, TabIndex = 7 };

            btnSearch = new Button { Name = "btnSearch", Text = "&Search", Location = new Point(20, 50), Size = new Size(90, 28), TabIndex = 8 };
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
            colTitle = new ColumnHeader { Text = "Title", Width = 160 };       // index 0
            colCategory = new ColumnHeader { Text = "Category", Width = 100 };  // index 1
            colDate = new ColumnHeader { Text = "Date", Width = 90 };           // index 2
            colLocation = new ColumnHeader { Text = "Location", Width = 150 };  // index 3
            colPriority = new ColumnHeader { Text = "Priority", Width = 80 };   // index 4
            lvEvents.Columns.AddRange(new[] { colTitle, colCategory, colDate, colLocation, colPriority });

            lvEvents.MultiSelect = false;
            lvEvents.HeaderStyle = ColumnHeaderStyle.Clickable;

            // --- Recommendations panel ---
            grpRecommendations = new GroupBox { Name = "grpRecommendations", Text = "Recommended for you", Location = new Point(640, 90), Size = new Size(270, 400), TabIndex = 10 };
            lstRecommended = new ListBox { Name = "lstRecommended", Location = new Point(15, 25), Size = new Size(240, 360), TabIndex = 11 };
            grpRecommendations.Controls.Add(lstRecommended);

            // --- Navigation ---
            btnBack = new Button { Name = "btnBack", Text = "&Back", Location = new Point(820, 510), Size = new Size(90, 28), TabIndex = 12 };
            this.CancelButton = btnBack;

            Controls.AddRange(new Control[]
            {
                lblSearch, txtSearch, lblCategory, cmbCategory, lblFrom, dtpFrom, lblTo, dtpTo, btnSearch,
                lvEvents, grpRecommendations, btnBack
            });
        }

        /// <summary>
        /// Wire up all events, including ColumnClick for sorting.
        /// </summary>
        private void WireEvents()
        {
            btnSearch.Click += btnSearch_Click;
            cmbCategory.SelectedIndexChanged += cmbCategory_SelectedIndexChanged;
            dtpFrom.ValueChanged += dtpFrom_ValueChanged;
            dtpTo.ValueChanged += dtpTo_ValueChanged;
            lvEvents.DoubleClick += lvEvents_DoubleClick;
            btnBack.Click += btnBack_Click;

            // Click-to-sort on columns (C# 7.3 safe)
            lvEvents.ColumnClick += lvEvents_ColumnClick;
        }

        /// <summary>
        /// First load: seed repository (if needed), populate categories, default dates, and initial results.
        /// </summary>
        private void SafeLoad()
        {
            try
            {
                _repo.Seed();

                cmbCategory.Items.Clear();
                cmbCategory.Items.Add(""); // (All)
                foreach (var cat in _repo.GetAllCategories())
                    cmbCategory.Items.Add(cat);
                cmbCategory.SelectedIndex = 0;

                dtpFrom.Value = DateTime.Today;
                dtpTo.Value = DateTime.Today.AddDays(90);

                RefreshResultsAndRecommendations(); // default sort: Date asc
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

            MessageBox.Show(this,
                $"{tag.Title}\n\n" +
                $"Category: {tag.Category}\nDate: {tag.Date:yyyy-MM-dd}\n" +
                $"Location: {tag.Location}\nPriority: {tag.PriorityLabel}\n\n" +
                $"{tag.Description}",
                "Event Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Column header clicked → toggle sort direction and rebind.
        /// Supports Title (0), Category (1), Date (2), Location (3), Priority (4).
        /// </summary>
        private void lvEvents_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (_sortColumn == e.Column)
                _sortAsc = !_sortAsc;   // toggle
            else
            {
                _sortColumn = e.Column; // change column, reset to ascending
                _sortAsc = true;
            }

            var results = GetCurrentFilteredResults();
            BindResults(results);
        }

        // ===============================
        // Helpers: compute & bind results
        // ===============================

        /// <summary>
        /// Returns current filtered results based on search/category/date range.
        /// Does not apply sorting. Sorting is applied in BindResults.
        /// </summary>
        private List<EventItem> GetCurrentFilteredResults()
        {
            var rawCat = (cmbCategory.SelectedItem ?? "").ToString();
            var category = string.IsNullOrWhiteSpace(rawCat) ? null : rawCat;
            var from = dtpFrom.Value.Date;
            var to = dtpTo.Value.Date;
            if (to < from) to = from; // inclusive guard

            return _repo.Search(txtSearch.Text, category, from, to).ToList();
        }

        /// <summary>
        /// Apply current sort to results and bind to ListView. Also refreshes recommendations.
        /// C# 7.3–compatible (uses if/else instead of switch expressions).
        /// </summary>
        private void BindResults(List<EventItem> results)
        {
            // Choose sort key based on column index (C# 7.3: if/else ladder)
            Func<EventItem, object> keySelector;
            if (_sortColumn == 0)       // Title
                keySelector = x => x.Title ?? string.Empty;
            else if (_sortColumn == 1)  // Category
                keySelector = x => x.Category ?? string.Empty;
            else if (_sortColumn == 2)  // Date
                keySelector = x => x.Date;
            else if (_sortColumn == 3)  // Location
                keySelector = x => x.Location ?? string.Empty;
            else if (_sortColumn == 4)  // Priority
                keySelector = x => x.Priority;
            else                        // Default
                keySelector = x => x.Date;

            var sorted = _sortAsc
                ? results.OrderBy(keySelector).ToList()
                : results.OrderByDescending(keySelector).ToList();

            // Bind to ListView
            lvEvents.BeginUpdate();
            lvEvents.Items.Clear();

            if (sorted.Count == 0)
            {
                lvEvents.Items.Add(new ListViewItem(new[] { "No events found", "", "", "", "" }));
            }
            else
            {
                var za = new CultureInfo("en-ZA");
                foreach (var e in sorted)
                {
                    var row = new ListViewItem(new[]
                    {
                        e.Title,
                        e.Category,
                        e.Date.ToString("yyyy-MM-dd", za),
                        e.Location,
                        e.PriorityLabel
                    })
                    {
                        Tag = e
                    };
                    lvEvents.Items.Add(row);
                }
            }
            lvEvents.EndUpdate();

            // Refresh recommendations (independent of column sorting)
            var tokens = SearchAnalyticsService.Tokenize(txtSearch.Text);
            var recs = RecommendationEngine.Recommend(_repo.GetAll(), tokens, maxResults: 6).ToList();
            lstRecommended.Items.Clear();
            foreach (var r in recs)
                lstRecommended.Items.Add($"{r.Title}  ({r.Category}, {r.Date:yyyy-MM-dd}, {r.PriorityLabel})");
        }

        /// <summary>
        /// End-to-end refresh after any filter/search change.
        /// Keeps default sort (Date asc) unless user has changed columns.
        /// </summary>
        private void RefreshResultsAndRecommendations()
        {
            var results = GetCurrentFilteredResults();
            BindResults(results);
        }
    }
}
