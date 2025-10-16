# ST10348753: Municipal Services Application — Part 2 (Local Events & Announcements)

**Module:** PROG7312 
**Target:** Windows Forms (.NET Framework 4.8), Visual Studio  
**Locale:** South Africa (`en-ZA`)

## Overview
Part 2 extends Part 1 by adding a **Local Events & Announcements** page with search, category/date filters, and **smart recommendations**. The implementation explicitly demonstrates:  
- **SortedDictionary** (chronological store), **Dictionary** (category index), **HashSet** (unique categories/tags), **Queue** (recent items), **Stack** (search history), and **Priority** simulation via **SortedDictionary<int, Queue<EventItem>>**.

## Prerequisites
- Windows 10/11
- Visual Studio 2022 Community
- .NET Framework 4.8 Developer Pack

## Build & Run
1. Open the solution in VS 2022.  
2. Ensure **Startup object**: `MunicipalServicesApp.Program`.  
3. Press **F5**.

## Usage
- From **Main Menu**, click **Local Events & Announcements**.  
- Use **Search**, **Category**, and **From/To** dates to filter.  
- Double-click an event for details.  
- **Recommended for you** updates based on your search terms, overlap with categories/tags, recency, and priority.

## Data Structures
- **Stack**: `SearchAnalyticsService` (search history).  
- **Queue**: `EventRepository._recentQueue` and priority buckets’ queues.  
- **Priority (simulated)**: `EventRepository._priorityBuckets` (0=High,1=Medium,2=Low).  
- **HashSet**: `EventItem.Tags`, `EventRepository._allCategories`.  
- **Dictionary**: `EventRepository._byCategory`.  
- **SortedDictionary**: `EventRepository._byDate`, `_priorityBuckets`.

## Troubleshooting
- Empty results: check category/date range; try clearing Search.  
- Date validation: the `To` date auto-clamps to `From` if set earlier.

## Rationale & Docs
- **SortedDictionary** provides O(log n) ordered access by date; ideal for range filters.  
- **Dictionary/HashSet** give O(1) lookups/uniques.  
- **Stack/Queue** demonstrate LU structures for history and FIFO flows.

