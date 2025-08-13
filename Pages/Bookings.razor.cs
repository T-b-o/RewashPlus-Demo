using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RewashPlus.Pages
{
    /// <summary>
    /// Code-behind for the Bookings page.
    /// Handles data fetching, filtering, pagination, and modal state.
    /// </summary>
    public partial class Bookings
    {
        // ================
        // Private Fields
        // ================

        /// <summary>All bookings loaded into memory (mock/demo data for now).</summary>
        private List<BookingModel> AllBookings = new();

        /// <summary>Filtered bookings based on status filter.</summary>
        private List<BookingModel> FilteredBookings = new();

        /// <summary>The selected status for filtering.</summary>
        private string SelectedStatus = "All";

        /// <summary>The number of rows to show per page.</summary>
        private int PageSize = 10;

        /// <summary>The current page number.</summary>
        private int CurrentPage = 1;

        /// <summary>The booking currently being viewed/edited in the modal.</summary>
        private BookingModel? SelectedBooking;

        /// <summary>Flag to show or hide the modal.</summary>
        private bool IsModalOpen = false;

        /// <summary>Total number of pages based on filtered results.</summary>
        private int TotalPages => (int)Math.Ceiling((double)FilteredBookings.Count / PageSize);

        // ================
        // Lifecycle
        // ================

        protected override void OnInitialized()
        {
            // Generate mock bookings for demo purposes
            AllBookings = Enumerable.Range(1, 50).Select(i => new BookingModel
            {
                Id = i,
                CustomerName = $"Customer {i}",
                Service = i % 2 == 0 ? "Exterior Wash" : "Full Wash",
                Date = DateTime.Now.AddDays(i),
                Status = i % 3 == 0 ? "Pending" : (i % 3 == 1 ? "Confirmed" : "Cancelled")
            }).ToList();

            ApplyFilters();
        }

        // ================
        // Event Handlers
        // ================

        /// <summary>
        /// Handles the change of the status dropdown.
        /// </summary>
        private void OnStatusChanged(ChangeEventArgs e)
        {
            SelectedStatus = e.Value?.ToString() ?? "All";
            ApplyFilters();
        }

        /// <summary>
        /// Handles change in page size dropdown.
        /// </summary>
        private void OnPageSizeChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out var size))
                PageSize = size;

            CurrentPage = 1;
        }

        /// <summary>
        /// Opens the modal for a specific booking.
        /// </summary>
        private void OpenModal(BookingModel booking)
        {
            SelectedBooking = booking;
            IsModalOpen = true;
        }

        /// <summary>
        /// Closes the modal dialog.
        /// </summary>
        private void CloseModal()
        {
            IsModalOpen = false;
            SelectedBooking = null;
        }

        /// <summary>
        /// Navigates to a specific page.
        /// </summary>
        private void GoToPage(int page)
        {
            if (page >= 1 && page <= TotalPages)
                CurrentPage = page;
        }

        // ================
        // Helper Methods
        // ================

        /// <summary>
        /// Applies the currently selected status filter.
        /// </summary>
        private void ApplyFilters()
        {
            FilteredBookings = SelectedStatus == "All"
                ? AllBookings
                : AllBookings.Where(b => b.Status == SelectedStatus).ToList();

            CurrentPage = 1;
        }

        /// <summary>
        /// Gets the subset of bookings for the current page.
        /// </summary>
        private IEnumerable<BookingModel> GetCurrentPageData()
        {
            return FilteredBookings
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize);
        }

        // ================
        // Data Model
        // ================

        /// <summary>
        /// Represents a booking record.
        /// </summary>
        public class BookingModel
        {
            public int Id { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string Service { get; set; } = string.Empty;
            public DateTime Date { get; set; }
            public string Status { get; set; } = string.Empty;
        }
    }
}
