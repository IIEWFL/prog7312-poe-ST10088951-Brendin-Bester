using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MunicipalServicesApp.Services
{
    public class ServiceRequestRepository
    {
        private static readonly Lazy<ServiceRequestRepository> lazy = new Lazy<ServiceRequestRepository>(() => new ServiceRequestRepository());
        public static ServiceRequestRepository Instance => lazy.Value;

        public List<ServiceRequest> AllRequests { get; } = new List<ServiceRequest>();

        public BinarySearchTree<ServiceRequest> IdBst { get; }
        public AvlTree<ServiceRequest> CreatedAtAvl { get; }
        public RedBlackWrapper<ServiceRequest> PriorityRb { get; }
        public MinHeap<ServiceRequest> PriorityHeap { get; }
        public Graph<ServiceRequest> RequestGraph { get; }

        private ServiceRequestRepository()
        {
            IdBst = new BinarySearchTree<ServiceRequest>(r => r.Id.ToString());
            CreatedAtAvl = new AvlTree<ServiceRequest>(r => r.CreatedAt.Ticks);
            PriorityRb = new RedBlackWrapper<ServiceRequest>(r => r.Priority);
            PriorityHeap = new MinHeap<ServiceRequest>((a, b) => a.Priority.CompareTo(b.Priority));
            RequestGraph = new Graph<ServiceRequest>();

            SeedSampleData();
        }

        private void SeedSampleData()
        {
            if (AllRequests.Count > 0) return;

            var r1 = new ServiceRequest { Title = "Pothole on Main St", Description = "Large pothole near 3rd Ave", Location = "Main St & 3rd", Status = RequestStatus.InProgress, Priority = 3, CreatedAt = DateTime.UtcNow.AddDays(-5) };
            var r2 = new ServiceRequest { Title = "Broken Streetlight", Description = "Streetlight not functioning", Location = "Elm St", Status = RequestStatus.Received, Priority = 4, CreatedAt = DateTime.UtcNow.AddDays(-10) };
            var r3 = new ServiceRequest { Title = "Overflowing Drain", Description = "Drain blocked", Location = "Main St & 5th", Status = RequestStatus.Submitted, Priority = 2, CreatedAt = DateTime.UtcNow.AddDays(-1) };
            var r4 = new ServiceRequest { Title = "Graffiti Removal", Description = "Graffiti on library wall", Location = "Central Library", Status = RequestStatus.OnHold, Priority = 6, CreatedAt = DateTime.UtcNow.AddDays(-20) };
            var r5 = new ServiceRequest { Title = "Water Leak", Description = "Burst pipe under road", Location = "Oak Rd", Status = RequestStatus.InProgress, Priority = 1, CreatedAt = DateTime.UtcNow.AddDays(-2) };

            AddRequest(r1);
            AddRequest(r2);
            AddRequest(r3);
            AddRequest(r4);
            AddRequest(r5);

            RequestGraph.AddVertex(r1); RequestGraph.AddVertex(r2); RequestGraph.AddVertex(r3); RequestGraph.AddVertex(r4); RequestGraph.AddVertex(r5);
            RequestGraph.AddEdge(r1, r3, 1.0);
            RequestGraph.AddEdge(r1, r5, 2.5);
            RequestGraph.AddEdge(r2, r4, 5.0);
            RequestGraph.AddEdge(r3, r5, 1.2);
            RequestGraph.AddEdge(r1, r2, 3.0); // ✅ This connects the graph fully
        }

        public void AddRequest(ServiceRequest r)
        {
            if (r == null) return;
            AllRequests.Add(r);
            IdBst.Insert(r);
            CreatedAtAvl.Insert(r);
            PriorityRb.Insert(r);
            PriorityHeap.Add(r);
            RequestGraph.AddVertex(r);
        }

        public bool RemoveRequest(Guid id)
        {
            var found = AllRequests.FirstOrDefault(x => x.Id == id);
            if (found == null) return false;
            AllRequests.Remove(found);
            IdBst.Delete(found);
            CreatedAtAvl.Delete(found);
            PriorityRb.Delete(found);
            PriorityHeap.Remove(found);
            RequestGraph.RemoveVertex(found);
            return true;
        }

        public ServiceRequest FindById(Guid id) => IdBst.Find(id.ToString());

        public IEnumerable<ServiceRequest> GetByPriorityTopN(int n) => PriorityHeap.PeekN(n);

        public IEnumerable<ServiceRequest> GetRecent(int n) => CreatedAtAvl.InOrder().Reverse().Take(n);

        public IEnumerable<ServiceRequest> GetRelated(ServiceRequest r) => RequestGraph.Neighbors(r);

        public IEnumerable<(ServiceRequest a, ServiceRequest b, double w)> GetMstEdges() => RequestGraph.PrimMstEdges();
    }

    // Simple balanced wrapper using SortedDictionary to simulate RBT semantics for priority buckets
    public class RedBlackWrapper<T>
    {
        private readonly SortedDictionary<int, List<T>> map = new SortedDictionary<int, List<T>>();
        private readonly Func<T, int> keySelector;
        public RedBlackWrapper(Func<T, int> keySelector) { this.keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector)); }
        public void Insert(T value)
        {
            var k = keySelector(value);
            if (!map.TryGetValue(k, out var list)) { list = new List<T>(); map[k] = list; }
            list.Add(value);
        }
        public void Delete(T value)
        {
            var k = keySelector(value);
            if (map.TryGetValue(k, out var list))
            {
                list.Remove(value);
                if (list.Count == 0) map.Remove(k);
            }
        }
        public IEnumerable<T> InOrder()
        {
            foreach (var kv in map) foreach (var v in kv.Value) yield return v;
        }
    }
}