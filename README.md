# Municipal Services Application — Full POE (Part 3)
**Module:** PROG7312  
**Institution:** IIE Varsity College  
**Student:** Zinedean Saaiman (ST10348753)  
**GitHub Repository:** https://github.com/ST10348753-ZSAAIMAN/MunicipalServicesApp

---

## 1  Introduction
Task 3 finalises the *Municipal Services Application for South Africa*, integrating all previous components into one functioning solution.  
It demonstrates advanced data-structure and algorithmic concepts — including **basic trees, binary trees, binary search trees, AVL trees, red-black trees, heaps, graphs, graph traversal, and minimum spanning trees (MST)** — to manage and display **service-request information** efficiently.

---

## 2  Scenario
The Municipal Services Application is a citizen-facing desktop tool that allows residents to:
- **Report local issues** (Part 1).  
- **Access events and announcements** (Part 2).  
- **Track service requests and their progress** (Part 3).  

All components share a consistent South African context, clear branding, and code-only WinForms interfaces (no Designer files).

---

## 3  Application Overview
### 3.1 Main Menu
The startup form presents three clearly labelled options:
1. **Report Issues** — active and fully implemented.  
2. **Local Events & Announcements** — active, with search, filter, and recommendations.  
3. **Service Request Status** — newly implemented in this task.

### 3.2 Service Request Status Page
Users can:
- View a well-organised list of 15 + sampled municipal service requests.  
- Search by **unique ticket ID**.  
- See a live **urgent-priority list** generated from a heap.  
- Inspect the **Depot Network MST**, showing optimal connections between depots using **Prim’s algorithm**.  
- Double-click any entry to view detailed information.

---

## 4  Compilation and Execution Guide
### Prerequisites
- **Operating System:** Windows 10 or 11  
- **IDE:** Visual Studio 2022 Community Edition  
- **Framework:** .NET Framework 4.8 Developer Pack  

### Build and Run Steps
1. Clone the repository from GitHub Classroom:  
   ```bash
   git clone [https://github.com/ST10348753-ZSAAIMAN/MunicipalServicesApp]
2. Open **MunicipalServicesApp.sln** in Visual Studio.
3. Ensure the **Startup object** = `MunicipalServicesApp.Program`.
4. Build → Run (**Ctrl + F5**).
5. Navigate via the **Main Menu** between all three features.

---

## 5. Features Summary

| Feature                     | Description                                                                 | Core Data Structures                                             |
|----------------------------|-----------------------------------------------------------------------------|------------------------------------------------------------------|
| **Report Issues**          | Capture and store citizen-reported issues with attachments and engagement feedback. | `List<T>`, Method Overloading, Recursion                         |
| **Local Events & Announcements** | Display events, filter by category/date, and recommend events using search analytics. | `SortedDictionary`, `Dictionary`, `HashSet`, `Queue`, `Stack`, Priority Simulation |
| **Service Request Status** | Track requests, search by ticket, display urgent items, and visualise depot MST. | **BST**, **AVL**, **Red-Black Tree**, **Max Heap**, **Graph**, **MST**, **Basic/Binary Trees** |

---

## 6. Data Structures and Algorithms

### 6.1 Trees

| Type                        | Implementation File                         | Role & Efficiency                                                                 |
|-----------------------------|----------------------------------------------|------------------------------------------------------------------------------------|
| **Basic Tree (N-ary)**      | `Structures/Trees/BasicTree.cs`              | Represents a hierarchical taxonomy of service categories (e.g., Water → Leak → Burst). |
| **Binary Tree**             | `Structures/Trees/BinaryTree.cs`             | Demonstrates traversal (pre-order, in-order, post-order) for status auditing.      |
| **Binary Search Tree (BST)**| `Structures/Trees/BinarySearchTree.cs`       | Maps `TicketNumber → ServiceRequest` for efficient O(log n) look-ups.             |
| **AVL Tree**                | `Structures/Trees/AvlTree.cs`                | Balanced index keyed by `CreatedAt` ticks, improving date-range query performance. |
| **Red-Black Tree**          | `Structures/Trees/RedBlackTree.cs`           | Provides balanced ordering by location string for region-based search operations.  |

---

### 6.2 Heaps

| Type              | File                                      | Purpose                                                                 |
|-------------------|-------------------------------------------|-------------------------------------------------------------------------|
| **Max Binary Heap** | `Structures/Heaps/MaxBinaryHeap.cs`       | Produces the **Urgent Requests** list by priority (3 = Critical → 0 = Low). |

---

### 6.3 Graphs and Traversal

| Structure                   | File                                             | Role                                                                                   |
|-----------------------------|--------------------------------------------------|----------------------------------------------------------------------------------------|
| **Graph (Adjacency List)** | `Structures/Graphs/Graph.cs`                     | Models municipal depots and routes; supports **BFS** and **DFS** for connectivity checks. |
| **Minimum Spanning Tree (Prim)** | `Structures/Graphs/MinimumSpanningTree.cs` | Calculates the optimal depot-link network (minimum total distance in km).              |

---

### 6.4 Efficiency Contribution
Each data structure improves system responsiveness by enabling either **O(log n)** search operations (BST, AVL, RBT, heap) or **O(1)** access in many cases (`Dictionary`, `HashSet`).  
This significantly reduces latency in:
- Ticket look-ups  
- Urgent-case triage  
- Network analysis of depots  
- Filtering and categorisation  

These optimisations ensure that both municipal staff and residents experience fast, reliable navigation throughout the Service Request Status system.

---

## 7. User Interface Design

- **Theme:** Segoe UI 9 pt, code-only layout, consistent spacing and tab order.  
- **Navigation:** Main Menu → Feature Form → Back → Main Menu (state preserved).  
- **Branding:** Municipal logo placeholder positioned top-right for identity consistency.  
- **Feedback:** Real-time list updates, search results, MST rendering, and dialog confirmations.  

---

## 8. Testing and Validation

### Manual Test Checklist

| # | Test Action                              | Expected Result                                                   |
|---|-------------------------------------------|-------------------------------------------------------------------|
| 1 | Launch app                                | Main Menu displays all three options.                             |
| 2 | Open Service Status                       | All service requests load (≥15); Urgent and MST sections visible. |
| 3 | Enter ticket `SR-2025-0001` → Find        | BST returns record instantly; matching row is highlighted.        |
| 4 | Check Urgent list                         | High-priority items appear first (Heap ordering).                  |
| 5 | Review MST panel                          | Depots connected; edges and total distance (km) are shown.        |
| 6 | Double-click record                       | Detailed request dialog opens correctly.                          |
| 7 | Refresh                                   | All lists reload without errors.                                  |
| 8 | Back to menu                              | Form closes gracefully; returns to Main Menu.                     |

---

## 9. Challenges and Solutions

| Challenge                                      | Resolution                                                                 |
|------------------------------------------------|----------------------------------------------------------------------------|
| Implementing self-balancing trees (AVL / RBT)  | Created rotation and colour-flip logic; tested inserts/queries manually. |
| Priority queue without NuGet                   | Built `MaxBinaryHeap<T>` manually using array-backed binary heap logic.  |
| Graph and MST visualisation                    | Implemented Prim’s algorithm; displayed edge table and total distance.    |
| Designer-free UI layout                        | Used precise `InitializeComponent()` calls with consistent dimensions.    |

---

## 10. Key Learnings

- Effective **data-structure selection** tailored to municipal use cases and performance.  
- Hands-on implementation of **AVL** and **Red-Black** balancing techniques.  
- Application of **graph theory** (BFS, DFS, MST) in infrastructure routing contexts.  
- Strengthened **C# OOP**, algorithmic thinking, and architecture patterns.  
- Improved ability to build modular, scalable systems with clean separation of concerns.  

---

## 11. Technology Recommendations

| Technology / Tool          | Justification & Benefit                                                | Compatibility                                       |
|----------------------------|------------------------------------------------------------------------|------------------------------------------------------|
| **SQLite or SQL Server Express** | Enables persistent storage and scalable querying for live municipal deployment. | Supported via ADO.NET in .NET Framework.            |
| **Serilog (File Logging)** | Adds structured logs for audits, debugging, and system diagnostics.     | Simple NuGet package for WinForms.                 |
| **ASP.NET Web API Extension** | Enables web/mobile access; backend services integrate with existing repositories. | High compatibility; future-proof architecture.      |
| **GitHub Actions CI/CD**  | Ensures automated build/test pipelines for every commit.               | Native GitHub Classroom support.                    |

---


