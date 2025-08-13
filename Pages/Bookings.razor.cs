﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RewashPlus.Pages
{
    /// <summary>
    /// Code-behind for Bookings.razor. Keeps UI markup clean and testable.
    /// </summary>
    public partial class Bookings : ComponentBase
    {
        #region State

        /// <summary>
        /// Search filter text (customer name or booking ID).
        /// </summary>
        protected string SearchTerm { get; set; } = string.Empty;

        /// <summary>
        /// Backing store for all bookings. In production, load from API.
        /// </summary>
        private List<Booking> _allBookings = new();

        /// <summary>
        /// Computed, filtered list based on <see cref="SearchTerm"/>.
        /// </summary>
        protected IEnumerable<Booking> FilteredBookings =>
            string.IsNullOrWhiteSpace(SearchTerm)
                ? _allBookings
                : _allBookings.Where(b =>
                       (!string.IsNullOrEmpty(b.CustomerName) &&
                        b.CustomerName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                       || b.Id.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

        #endregion

        #region Services

        [Inject] protected NavigationManager Nav { get; set; } = default!;

        #endregion

        #region Lifecycle

        /// <summary>
        /// Initialize with sample data. Replace with API call in production.
        /// </summary>
        protected override void OnInitialized()
        {
            _allBookings = new List<Booking>
            {
                new Booking { Id = 1001, CustomerName = "John Doe",    Date = DateTime.Today,     Time = "10:00 AM", Status = BookingStatus.Confirmed },
                new Booking { Id = 1002, CustomerName = "John Doe",  Date = DateTime.Today,     Time = "11:30 AM", Status = BookingStatus.Pending   },
                new Booking { Id = 1003, CustomerName = "John Doe",Date = DateTime.Today.AddDays(1), Time = "01:00 PM", Status = BookingStatus.Cancelled }
            };
        }

        #endregion

        #region UI Helpers

        /// <summary>
        /// Tailwind badge classes based on booking status.
        /// </summary>
        protected string GetStatusBadgeClass(BookingStatus status) =>
            status switch
            {
                BookingStatus.Confirmed => "px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-green-100 text-green-800",
                BookingStatus.Pending => "px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-yellow-100 text-yellow-800",
                BookingStatus.Cancelled => "px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-red-100 text-red-800",
                _ => "px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-gray-100 text-gray-800"
            };

        #endregion

        #region Actions

        protected void ViewBooking(int id)
        {
            // Navigate to a details page when you create it, e.g. /bookings/{id}
            // For now, just navigate to a placeholder
            Nav.NavigateTo($"/bookings/{id}", forceLoad: false);
        }

        protected void EditBooking(int id)
        {
            // Navigate to edit page when available
            Nav.NavigateTo($"/bookings/{id}/edit", forceLoad: false);
        }

        protected void CancelBooking(int id)
        {
            var booking = _allBookings.FirstOrDefault(b => b.Id == id);
            if (booking is null) return;

            booking.Status = BookingStatus.Cancelled;
            StateHasChanged();
        }

        #endregion

        #region Model

        /// <summary>
        /// Booking status enumeration for type-safety.
        /// </summary>
        public enum BookingStatus
        {
            Unknown = 0,
            Pending = 1,
            Confirmed = 2,
            Cancelled = 3
        }

        /// <summary>
        /// A single booking entry.
        /// </summary>
        public sealed class Booking
        {
            public int Id { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public DateTime Date { get; set; }
            public string Time { get; set; } = string.Empty;
            public BookingStatus Status { get; set; } = BookingStatus.Unknown;
        }

        #endregion
    }
}
