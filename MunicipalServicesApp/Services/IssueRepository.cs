using System.Collections.Generic;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// In-memory repository for IssueItem.
    /// Demonstrates List<T>, IEnumerable<T>, and method overloading.
    /// </summary>
    public sealed class IssueRepository
    {
        // Singleton instance keeps one authoritative in-memory store across forms.
        private static readonly IssueRepository _instance = new IssueRepository();
        public static IssueRepository Instance => _instance;

        // Authoritative store.
        private readonly List<IssueItem> _issues = new List<IssueItem>();

        private IssueRepository() { }

        /// <summary>
        /// Add an existing IssueItem (overload 1).
        /// </summary>
        public void Add(IssueItem issue)
        {
            if (issue != null)
            {
                _issues.Add(issue);
            }
        }

        /// <summary>
        /// Create and add a new IssueItem from field parameters (overload 2).
        /// </summary>
        public void Add(string location, string category, string description, string attachmentPath = null)
        {
            var item = new IssueItem
            {
                Location = location,
                Category = category,
                Description = description,
                AttachmentPath = attachmentPath
            };
            _issues.Add(item);
        }

        /// <summary>
        /// Enumerate all issues.
        /// </summary>
        public IEnumerable<IssueItem> GetAll() => _issues;

        /// <summary>
        /// Current count.
        /// </summary>
        public int Count => _issues.Count;
    }
}
