using System;
using System.Drawing;
using System.Globalization;
using System.IO;                  // <-- Added for Path.Combine
using System.Linq;
using System.Windows.Forms;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;
using MunicipalServicesApp.Structures.Graphs;
using MunicipalServicesApp.Utilities;

namespace MunicipalServicesApp.Views
{
    /// <summary>
    /// Displays submitted service requests, supports ticket lookup (BST),
    /// urgent list (heap), and shows Depot MST with total weight.
    /// Includes friendly, SA-focused wording.
    /// </summary>
    public class ServiceStatusForm : Form
    {
        private TextBox txtTicket;
        private Button btnFind, btnBack, btnRefresh;
        private ListView lvRequests, lvUrgent, lvMst;
        private Label lblFound, lblMstTotal;

        private readonly ServiceRequestRepository _repo = ServiceRequestRepository.Instance;

        public ServiceStatusForm() { InitializeComponent(); Wire(); SafeLoad(); }

        private void InitializeComponent()
        {
            Text = "Service Request Status";
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(980, 600);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            // Search row
            var lblTicket = new Label { Text = "&Ticket #", Location = new Point(20, 20), AutoSize = true, TabIndex = 0 };
            txtTicket = new TextBox { Location = new Point(85, 17), Size = new Size(160, 24), TabIndex = 1 };
            btnFind = new Button { Text = "&Find", Location = new Point(255, 15), Size = new Size(70, 28), TabIndex = 2 };
            btnRefresh = new Button { Text = "&Refresh", Location = new Point(335, 15), Size = new Size(80, 28), TabIndex = 3 };
            lblFound = new Label { Text = "", Location = new Point(430, 20), Size = new Size(520, 24), TabIndex = 4 };

            // Requests list
            lvRequests = new ListView
            {
                Location = new Point(20, 60),
                Size = new Size(600, 360),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                TabIndex = 5
            };
            lvRequests.Columns.AddRange(new[]
            {
                new ColumnHeader{ Text="Ticket", Width=100 },
                new ColumnHeader{ Text="Category", Width=110 },
                new ColumnHeader{ Text="SubCategory", Width=120 },
                new ColumnHeader{ Text="Location", Width=120 },
                new ColumnHeader{ Text="Priority", Width=70 },
                new ColumnHeader{ Text="Status", Width=80 }
            });

            // Urgent (heap)
            var lblUrg = new Label { Text = "Urgent (priority queue)", Location = new Point(20, 430), AutoSize = true, TabIndex = 6 };
            lvUrgent = new ListView
            {
                Location = new Point(20, 450),
                Size = new Size(600, 120),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                TabIndex = 7
            };
            lvUrgent.Columns.AddRange(new[]
            {
                new ColumnHeader{ Text="Ticket", Width=100 },
                new ColumnHeader{ Text="Category", Width=110 },
                new ColumnHeader{ Text="Location", Width=120 },
                new ColumnHeader{ Text="Priority", Width=70 },
                new ColumnHeader{ Text="Created (SA)", Width=180 }
            });

            // MST (depots)
            var lblMst = new Label { Text = "Depot Network — Minimum Spanning Tree (Prim)", Location = new Point(640, 60), AutoSize = true, TabIndex = 8 };
            lvMst = new ListView
            {
                Location = new Point(640, 80),
                Size = new Size(320, 300),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                TabIndex = 9
            };
            lvMst.Columns.AddRange(new[]
            {
                new ColumnHeader{ Text="From", Width=120 },
                new ColumnHeader{ Text="To", Width=120 },
                new ColumnHeader{ Text="Dist (km)", Width=70 }
            });
            lblMstTotal = new Label { Text = "Total: 0 km", Location = new Point(640, 385), AutoSize = true, TabIndex = 10 };

            btnBack = new Button { Text = "&Back", Location = new Point(870, 540), Size = new Size(90, 28), TabIndex = 11 };
            CancelButton = btnBack;

            Controls.AddRange(new Control[] {
                lblTicket, txtTicket, btnFind, btnRefresh, lblFound,
                lvRequests, lblUrg, lvUrgent,
                lblMst, lvMst, lblMstTotal,
                btnBack
            });

            // --- Branding: Municipality Logo ---
            var picLogo = new PictureBox
            {
                Location = new Point(this.ClientSize.Width - 140, 10),
                Size = new Size(120, 40),
                SizeMode = PictureBoxSizeMode.Zoom,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            try
            {
                picLogo.Image = Image.FromFile(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Assets", "municipality_logo.png"));
            }
            catch
            {
                // ignore if missing
            }
            Controls.Add(picLogo);
            // --- End branding ---
        }

        private void Wire()
        {
            btnBack.Click += (s, e) => Close();
            btnFind.Click += btnFind_Click;
            btnRefresh.Click += (s, e) => RefreshAll();
            lvRequests.DoubleClick += (s, e) => ShowSelectedDetails(lvRequests);
            lvUrgent.DoubleClick += (s, e) => ShowSelectedDetails(lvUrgent);
        }

        private void SafeLoad()
        {
            try
            {
                _repo.SeedIfEmpty();
                RefreshAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to load service requests: " + ex.Message,
                    "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RefreshAll()
        {
            // All requests list
            var za = new CultureInfo("en-ZA");
            lvRequests.BeginUpdate();
            lvRequests.Items.Clear();
            foreach (var s in _repo.GetAll().OrderByDescending(x => x.Priority).ThenBy(x => x.CreatedAt))
            {
                lvRequests.Items.Add(new ListViewItem(new[]
                {
                    s.TicketNumber, s.Category, s.SubCategory, s.Location,
                    PriorityLabel(s.Priority), s.Status
                })
                { Tag = s });
            }
            lvRequests.EndUpdate();

            // Urgent heap top
            lvUrgent.BeginUpdate();
            lvUrgent.Items.Clear();
            foreach (var s in _repo.GetTopUrgent(6))
            {
                lvUrgent.Items.Add(new ListViewItem(new[]
                {
                    s.TicketNumber, s.Category, s.Location,
                    PriorityLabel(s.Priority),
                    s.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm", za)
                })
                { Tag = s });
            }
            lvUrgent.EndUpdate();

            // MST on depots (use Item1/Item2/Item3 for tuple access)
            lvMst.BeginUpdate();
            lvMst.Items.Clear();
            var res = MinimumSpanningTree.Prim(_repo.DepotsGraph, 0);
            foreach (var e in res.edges) // e = (u, v, w)
            {
                lvMst.Items.Add(new ListViewItem(new[]
                {
                    _repo.DepotsGraph.NameOf(e.Item1), // from (u)
                    _repo.DepotsGraph.NameOf(e.Item2), // to   (v)
                    e.Item3.ToString("0.0", za)        // dist (w)
                }));
            }
            lvMst.EndUpdate();
            lblMstTotal.Text = $"Total: {res.total:0.0} km";
            lblFound.Text = "";
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            var t = (txtTicket.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(t))
            {
                MessageBox.Show(this, "Please enter a ticket number (e.g., SR-2025-0001).",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (_repo.TryFindByTicket(t, out var s))
            {
                lblFound.Text = $"Found: {s.TicketNumber} — {s.Status}, {s.Category}/{s.SubCategory}, {s.Location}, Priority {PriorityLabel(s.Priority)}";
                SelectInList(lvRequests, t);
            }
            else
            {
                lblFound.Text = "Ticket not found.";
            }
        }

        private static void SelectInList(ListView lv, string ticket)
        {
            foreach (ListViewItem it in lv.Items)
            {
                if (string.Equals(it.SubItems[0].Text, ticket, StringComparison.OrdinalIgnoreCase))
                {
                    it.Selected = true; it.Focused = true; it.EnsureVisible(); break;
                }
            }
        }

        private static string PriorityLabel(int p) => p == 3 ? "Critical" : p == 2 ? "High" : p == 1 ? "Normal" : "Low";

        private void ShowSelectedDetails(ListView lv)
        {
            if (lv.SelectedItems.Count == 0) return;
            if (lv.SelectedItems[0].Tag is ServiceRequest s)
            {
                MessageBox.Show(this,
                    $"{s.TicketNumber}\n\nCategory: {s.Category} / {s.SubCategory}\nLocation: {s.Location}\n" +
                    $"Priority: {PriorityLabel(s.Priority)}\nStatus: {s.Status}\n" +
                    $"Created (UTC): {s.CreatedAt:yyyy-MM-dd HH:mm}\n\n{s.Description}",
                    "Request Details",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
