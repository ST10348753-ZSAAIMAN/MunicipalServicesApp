using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Structures.Trees;
using MunicipalServicesApp.Structures.Heaps;
using MunicipalServicesApp.Structures.Graphs;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// Central store + indices for Service Requests.
    /// Integrates custom DS required by POE:
    /// - BST (ticket), AVL (created ticks), RBT (location string)
    /// - MaxHeap (priority)
    /// - BasicTree (category taxonomy), BinaryTree (status demo traversal)
    /// - Graph + MST (depots network) with BFS/DFS
    /// </summary>
    public sealed class ServiceRequestRepository
    {
        private static readonly ServiceRequestRepository _instance = new ServiceRequestRepository();
        public static ServiceRequestRepository Instance => _instance;

        private readonly List<ServiceRequest> _all = new List<ServiceRequest>();

        // Indexes:
        private readonly BinarySearchTree<string, ServiceRequest> _byTicket = new BinarySearchTree<string, ServiceRequest>();
        private readonly AvlTree<long, ServiceRequest> _byCreatedTicks = new AvlTree<long, ServiceRequest>();
        private readonly RedBlackTree<string, ServiceRequest> _byLocation = new RedBlackTree<string, ServiceRequest>();

        // Priority heap (max)
        private readonly MaxBinaryHeap<ServiceRequest> _urgentHeap =
            new MaxBinaryHeap<ServiceRequest>(Comparer<ServiceRequest>.Create((a, b) =>
            {
                int p = a.Priority.CompareTo(b.Priority); // 3..0
                if (p != 0) return p; // higher priority wins
                return b.CreatedAt.CompareTo(a.CreatedAt); // newer first among equals
            }));

        // Category tree (N-ary)
        public readonly BasicTreeNode<string> CategoryTree = BasicTree.BuildCategoryTree();

        // Graph of depots (for traversal/MST)
        public readonly Graph DepotsGraph = new Graph();

        private ServiceRequestRepository() { BuildDepotGraph(); }

        public void SeedIfEmpty()
        {
            if (_all.Count > 0) return;

            // 16 sample requests (SA context)
            var r = new[]
            {
                SR("SR-2025-0001","Water","Leak","Bellville","Burst pipe near Voortrekker Rd", 3),
                SR("SR-2025-0002","Electricity","Outage","Athlone","Area-wide outage after storm", 3),
                SR("SR-2025-0003","Roads","Pothole","Mitchells Plain","Large pothole on AZ Berman Dr", 2),
                SR("SR-2025-0004","Community Safety","Streetlight","Khayelitsha","Streetlight not working near school", 1),
                SR("SR-2025-0005","Water","Low Pressure","Strandfontein","Low water pressure evenings", 1),
                SR("SR-2025-0006","Electricity","Fault","Goodwood","Tripping in substation sector B", 2),
                SR("SR-2025-0007","Roads","Signage","Somerset West","Stop sign missing at intersection", 0),
                SR("SR-2025-0008","Solid Waste","Collection","Grassy Park","Missed collection for 2 days", 2),
                SR("SR-2025-0009","Water","Leak","Rondebosch","Persistent leak at corner hydrant", 1),
                SR("SR-2025-0010","Electricity","Meter","Claremont","Faulty pre-paid meter readings", 1),
                SR("SR-2025-0011","Roads","Resurfacing","Durbanville","Request to prioritise resurfacing", 0),
                SR("SR-2025-0012","Community Safety","Vandalism","Retreat","Broken fence at park", 1),
                SR("SR-2025-0013","Water","Burst","Macassar","Major burst main line", 3),
                SR("SR-2025-0014","Electricity","Outage","Heideveld","Isolated outage on one street", 2),
                SR("SR-2025-0015","Roads","Pothole","Mowbray","Row of potholes near bridge", 1),
                SR("SR-2025-0016","Water","Leak","Parow","Leak near taxi rank", 2)
            };

            foreach (var s in r) Add(s);
        }

        private ServiceRequest SR(string ticket, string cat, string sub, string loc, string desc, int priority)
        {
            return new ServiceRequest
            {
                TicketNumber = ticket,
                Category = cat,
                SubCategory = sub,
                Location = loc,
                Description = desc,
                Priority = priority,
                CreatedAt = DateTime.UtcNow.AddMinutes(-new Random(ticket.GetHashCode()).Next(10_000))
            };
        }

        public void Add(ServiceRequest s)
        {
            _all.Add(s);
            _byTicket.Insert(s.TicketNumber, s);
            _byCreatedTicks.Insert(s.CreatedAt.Ticks, s);
            _byLocation.Insert(s.Location ?? "", s);
            _urgentHeap.Push(s);
        }

        public IEnumerable<ServiceRequest> GetAll() => _all;

        public bool TryFindByTicket(string ticket, out ServiceRequest sr) => _byTicket.TryFind(ticket, out sr);

        public IEnumerable<ServiceRequest> GetTopUrgent(int max)
        {
            var tmp = new List<ServiceRequest>();
            int count = 0;
            while (count < max && _urgentHeap.TryPop(out var s))
            {
                tmp.Add(s); count++;
            }
            // push back to keep heap state for demo
            foreach (var s in tmp) _urgentHeap.Push(s);
            return tmp;
        }

        private void BuildDepotGraph()
        {
            // Simple Cape Town region nodes (illustrative distances)
            int bellville = DepotsGraph.AddVertex("Bellville Depot");
            int athlone = DepotsGraph.AddVertex("Athlone Depot");
            int mitchells = DepotsGraph.AddVertex("Mitchells Plain Depot");
            int khayel = DepotsGraph.AddVertex("Khayelitsha Depot");
            int durban = DepotsGraph.AddVertex("Durbanville Depot");

            DepotsGraph.AddUndirectedEdge(bellville, athlone, 18.0);
            DepotsGraph.AddUndirectedEdge(bellville, durban, 13.0);
            DepotsGraph.AddUndirectedEdge(athlone, mitchells, 20.0);
            DepotsGraph.AddUndirectedEdge(athlone, khayel, 22.0);
            DepotsGraph.AddUndirectedEdge(mitchells, khayel, 17.0);
            DepotsGraph.AddUndirectedEdge(durban, khayel, 29.0);
        }
    }
}
