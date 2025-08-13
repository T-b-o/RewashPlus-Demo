using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RewashPlus.Admin
{
    /// <summary>
    /// Code-behind for AdminBookings. Handles data, filters, paging, bulk actions, and modal state.
    /// </summary>
    public partial class AdminBookings : ComponentBase
    {
        // -------- Data model --------
        public class Booking
        {
            public int Id { get; set; }
            public string UserName { get; set; } = string.Empty;
            public string CarModel { get; set; } = string.Empty;
            public string ServiceName { get; set; } = string.Empty;
            public DateTime Date { get; set; }
            public decimal Amount { get; set; }
            public string Status { get; set; } = "Pending";
            public bool Selected { get; set; }   // for bulk actions
        }

        // -------- State: data & filters --------
        private List<Booking> _all = new();
        protected string SearchTerm { get; set; } = string.Empty;
        protected string SelectedStatus { get; set; } = "All";
        protected DateTime? DateFrom { get; set; }
        protected DateTime? DateTo { get; set; }

        protected List<string> StatusOptions { get; } = new() { "All", "Pending", "Confirmed", "Cancelled" };

        // -------- Paging --------
        protected int CurrentPage { get; set; } = 1;
        protected int PageSize { get; set; } = 10;
        protected List<int> PageSizeOptions { get; } = new() { 5, 10, 20, 50 };

        // Computed collections
        protected IEnumerable<Booking> Filtered => ApplyFilters(_all);
        protected IEnumerable<Booking> PagedBookings => Filtered
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        protected int TotalPages => Math.Max(1, (int)Math.Ceiling(Filtered.Count() / (double)PageSize));
        protected bool CanPrev => CurrentPage > 1;
        protected bool CanNext => CurrentPage < TotalPages;

        // Bulk selection helpers
        protected int SelectedCount => _all.Count(b => b.Selected);
        protected bool AllOnPageSelected => PagedBookings.All(b => b.Selected) && PagedBookings.Any();

        // -------- Modal --------
        protected bool IsModalOpen { get; set; }
        protected Booking? Editing { get; set; }

        /// <summary>
        /// Helper for binding DateTime to datetime-local input (which expects local, no TZ).
        /// </summary>
        protected string EditingDateLocal
        {
            get => Editing is null ? "" : Editing.Date.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
            set
            {
                if (Editing is null) return;
                if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                {
                    Editing.Date = dt;
                }
            }
        }

        // -------- Lifecycle --------
        protected override void OnInitialized()
        {
            // Seed demo data. Replace with your API/db call.
            var rnd = new Random(7);
            _all = Enumerable.Range(1001, 87).Select(i => new Booking
            {
                Id = i,
                UserName = $"user{i % 12 + 1}@rewash.plus",
                CarModel = i % 2 == 0 ? "VW Polo" : "Toyota Corolla",
                ServiceName = (i % 3) switch { 0 => "Express Wash", 1 => "Full Wash", _ => "Premium Detail" },
                Date = DateTime.Today.AddDays(-(i % 15)).AddHours(i % 9 * 1.5),
                Amount = 120 + (i % 7) * 35,
                Status = (i % 3) switch { 0 => "Pending", 1 => "Confirmed", _ => "Cancelled" }
            }).ToList();
        }

        // -------- Filters & paging handlers --------
        protected void OnStatusChanged(ChangeEventArgs e)
        {
            SelectedStatus = e.Value?.ToString() ?? "All";
            CurrentPage = 1;
        }

        protected void OnPageSizeChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out var size) && size > 0)
            {
                PageSize = size;
                CurrentPage = 1;
            }
        }

        protected void PrevPage() { if (CanPrev) CurrentPage--; }
        protected void NextPage() { if (CanNext) CurrentPage++; }

        // -------- Selection & bulk actions --------
        protected void ToggleSelectAll(ChangeEventArgs _)
        {
            var selectAll = !AllOnPageSelected;
            foreach (var b in PagedBookings) b.Selected = selectAll;
        }

        protected void ApproveSelected()
        {
            foreach (var b in _all.Where(b => b.Selected)) b.Status = "Confirmed";
            ClearSelectionOnInvisibleRows();
        }

        protected void CancelSelected()
        {
            foreach (var b in _all.Where(b => b.Selected)) b.Status = "Cancelled";
            ClearSelectionOnInvisibleRows();
        }

        protected void DeleteSelected()
        {
            _all.RemoveAll(b => b.Selected);
            // reset page if we fell off the end
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;
        }

        private void ClearSelectionOnInvisibleRows()
        {
            // optional: clear selection after action
            foreach (var b in _all) b.Selected = false;
        }

        // -------- Modal --------
        protected void OpenModal(Booking b)
        {
            // clone to avoid editing the live row until Save
            Editing = new Booking
            {
                Id = b.Id,
                UserName = b.UserName,
                CarModel = b.CarModel,
                ServiceName = b.ServiceName,
                Date = b.Date,
                Amount = b.Amount,
                Status = b.Status
            };
            IsModalOpen = true;
        }

        protected void CloseModal()
        {
            IsModalOpen = false;
            Editing = null;
        }

        protected void SaveEditing()
        {
            if (Editing is null) return;
            var existing = _all.FirstOrDefault(x => x.Id == Editing.Id);
            if (existing != null)
            {
                existing.UserName = Editing.UserName;
                existing.CarModel = Editing.CarModel;
                existing.ServiceName = Editing.ServiceName;
                existing.Date = Editing.Date;
                existing.Amount = Editing.Amount;
                existing.Status = Editing.Status;
            }
            CloseModal();
        }

        // -------- Filter logic --------
        private IEnumerable<Booking> ApplyFilters(IEnumerable<Booking> source)
        {
            var q = source;

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim();
                q = q.Where(b =>
                      b.Id.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                   || b.UserName.Contains(term, StringComparison.OrdinalIgnoreCase)
                   || b.CarModel.Contains(term, StringComparison.OrdinalIgnoreCase)
                   || b.ServiceName.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            if (SelectedStatus != "All")
                q = q.Where(b => b.Status == SelectedStatus);

            if (DateFrom.HasValue)
                q = q.Where(b => b.Date.Date >= DateFrom.Value.Date);

            if (DateTo.HasValue)
                q = q.Where(b => b.Date.Date <= DateTo.Value.Date);

            return q.OrderByDescending(b => b.Date);
        }
    }
}
