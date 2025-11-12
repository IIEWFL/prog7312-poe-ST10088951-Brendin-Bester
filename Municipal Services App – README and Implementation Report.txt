Municipal Services App – README
===============================

Overview

The Municipal Services App is a WPF desktop application for citizens and municipal staff to report, view, manage, and discover local events, issues, and announcements. The app emphasizes accessibility, consistent styling, reliable local persistence, and a professional UI suited for municipal workflows.

Key Features
- Report Issues: create issues with location, category, description, and media attachments (image file paths previewed).
- View & Delete Issues: list reported issues with image previews and ability to delete items; deletes update in-memory store and persisted JSON.
- Local Events: list events, view details, add events, receive recommendations, and navigate between Home and Events pages.
- Event Delete: delete events you added; UI updates and changes are persisted.
- Recommended Events: lightweight in-app recommender exposing top items in the Events UI.
- Announcements: create announcements with Publish Date and optional Expiry Date, list announcements, and open a full-detail view for any announcement.
- Announcement Detail View: view full announcement data (title, body, category, publish/expiry dates, author) and delete the announcement from the detail page.
- Persistent Storage: JSON-based persistence for issues, events, and announcements; load on startup and save on change and on exit.
- Robust UI updates: DataGrid column sizing, text trimming, and button visibility fixes so action buttons (View/Delete) remain visible.
- Styling: added DangerButtonStyle and fallbacks so the Delete button no longer crashes the app.
- Navigation: Home, Events, Announcements, and Add pages wired with NavigationService.
Technologies Used
- UI: WPF (.NET Framework)
- Pattern: lightweight MVVM and pragmatic code-behind where appropriate
- Serialization: Newtonsoft.Json
- Data structures: ObservableCollection and custom store singletons; original CustomLinkedList retained for issue storage where required
- Styling: App.xaml resource dictionaries with ActionButtonStyle and DangerButtonStyle

==========Project Structure==========
MunicipalServicesApp/
├── Models/
│   ├── ReportIssue.cs
│   ├── EventItem.cs
│   └── Announcement.cs
├── DataStructures/
│   └── CustomLinkedList.cs
├── Storage/
│   ├── EventStore.cs
│   ├── AnnouncementStore.cs
│   └── DataStore.cs (existing issue storage)
├── Services/
│   ├── EventPersistenceService.cs
│   ├── AnnouncementPersistence.cs
│   ├── PersistenceService.cs (existing)
│   └── RecommendationService.cs (simple in-app recommender)
├── Pages/
│   ├── HomePage.xaml / .cs
│   ├── LocalEventsPage.xaml / .cs
│   ├── AddEventPage.xaml / .cs
│   ├── AnnouncementsPage.xaml / .cs
│   ├── AddAnnouncementPage.xaml / .cs
│   ├── AnnouncementDetailPage.xaml / .cs
│   ├── ReportIssuesPage.xaml / .cs
│   └── ReportedIssuesPage.xaml / .cs
├── App.xaml / App.xaml.cs
└── MainWindow.xaml / MainWindow.xaml.cs

Getting Started

Prerequisites:
- Windows 10 or later
- Visual Studio 2022+
- .NET Framework 4.7.2 or higher (match project target)
- Newtonsoft.Json NuGet package

Setup:
- Clone the repository.
- Open the solution in Visual Studio.
- Restore NuGet packages.
- Build and run the project.

On first launch the app creates local data files:
- %LOCALAPPDATA%\MunicipalServicesApp\events.json
- %LOCALAPPDATA%\MunicipalServicesApp\announcements.json
- %LOCALAPPDATA%\MunicipalServicesApp\issues.json

Persistence, Deletion, and UI Behavior
======================================
Persistence
- Each domain store (issues, events, announcements) is loaded on startup by its persistence service and saved on change and/or on app exit via App.xaml.cs hooks.
- Objects are serialized to JSON using Newtonsoft.Json. Attachments are stored as file paths and previewed if valid.

Deletion
- Delete actions (Issues, Events, Announcements) remove the item from the in-memory store and persist the updated collection to JSON.
- UI collections bound to the stores (ObservableCollection wrappers) are refreshed after deletions so DataGrids and Lists update immediately.
- No undo is implemented; deletions are permanent unless restored from a backup.

UI and Navigation Notes
- DataGrid column widths were reduced for Title and use TextTrimming to prevent action buttons from being pushed off-screen.
- Delete button styling was added as DangerButtonStyle to avoid XAML resource exceptions.
- Announcements are clickable from the list and navigate to AnnouncementDetailPage to show all fields and allow deletion.
- LocalEventsPage includes a Return to Home button; recommended items are clickable for quick preview.

Known Limitations
- No cloud sync or multi-user concurrency.
- Limited validation on attachment file types beyond basic image filtering.
- No undo
- Recommendation service is a lightweight heuristic, not ML-driven.
- Some pages still use code-behind navigation rather than full MVVM command binding.

Future Enhancements
- Mark as Resolved toggle for reported issues.
- Export reports to PDF or CSV.
- Role-based access for municipal staff and admin functions.
- Optional cloud sync via Azure or SharePoint for multi-user collaboration.
- Implement undo or soft-delete with a recycle/trash recovery.

Data structures and their roles
1. Binary Search Tree (BST)
   - Implemented as BinarySearchTree<T> keyed by a string (GUID.ToString()).
   - Role: fast lookup by request identifier for the "Find by ID" operation.
   - Complexity: average O(log n), worst-case O(n).

2. AVL Tree
   - Implemented as AvlTree<T> keyed by CreatedAt.Ticks.
   - Role: ordered retrieval by creation time; used to return recent requests efficiently.
   - Complexity: guaranteed O(log n) insert/delete.

3. Red-Black Tree (simplified)
   - Implemented as a SortedDictionary wrapper (RedBlackTree<T>) keyed by priority.
   - Role: balanced ordering by priority for ordered traversals.
   - Complexity: underlying SortedDictionary offers O(log n).

4. MinHeap
   - Implemented as MinHeap<T> with a custom comparison.
   - Role: return top-priority requests (lowest numeric priority) quickly for dispatching.
   - Complexity: O(log n) insert/pop; O(k log n) to get top-k.

5. Graph (/Prim MST)
   - Implemented as Graph<T> with adjacency lists and weighted edges.
   - Role: model relationships between requests (nearby issues, dependencies) and compute MST to help plan minimal routes or groups for field teams.
   - Traversals: BFS and DFS implemented; PrimMstEdges returns a set of edges representing MST (simple implementation).

Persistence
- Data persists to %LOCALAPPDATA%\MunicipalServicesApp\servicerequests.json
- Save is invoked on application exit and when adding requests in the form.

Extensibility notes
- Replace the simplified Red-Black wrapper with a full RBT for academic completeness.
- Use a proper priority queue (binary heap) with index mapping for efficient removal by value for large data.
- For larger graphs use a binary-heap-backed Prim implementation for O(E log V) performance.

Implementation Report — Service Request Status

Summary
This feature demonstrates practical application of core data structures: BST, AVL, Red-Black (wrapper), MinHeap, and Graph with MST, integrated into a Windows Forms UI.

Role & Examples
- BST: used for identity lookup. Example: user pastes GUID -> IdBst.Find -> O(log n).
- AVL: keeps requests ordered by time. Example: returned recent requests via InOrder().Reverse().Take(n).
- Red-Black: maintains sorted priority buckets for group operations and ordered scans.
- MinHeap: provides top-priority requests immediately for dispatching; example GetByPriorityTopN(3).
- Graph: models dependency/relationship; Example: MST via Prim gives minimal connecting edges: useful to plan combined jobs.

Performance and complexity
- Insert/delete must maintain consistency across all structures; ServiceRequestRepository ensures all structures update on add/remove.
- For moderate dataset sizes (hundreds/thousands) current implementations are adequate. For production (10k+), consider database indexing and optimized graph algorithms.

Testing notes
- Seed data demonstrates behavior; add requests via the dialog and verify persistence and graph responses.

YOUTUBE LINK https:

Author
Brendin Bester ST10088951

