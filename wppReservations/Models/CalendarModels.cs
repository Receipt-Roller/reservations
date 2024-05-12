using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wppReservations.Models
{
    public class CalendarModels
    {
    }

    // Represents a group of calendars, typically used to organize related calendars.
    [Table("CalendarGroups")]
    public class CalendarGroupModel
    {
        // Constructor to initialize a CalendarGroup with its unique identifier and name.
        public CalendarGroupModel(string id, string name)
        {
            Id = id;
            Name = name;
        }

        [Key, MaxLength(36)]
        public string Id { get; set; }  // Unique identifier for the calendar group, restricted to 36 characters.
        public string Name { get; set; }  // Name of the calendar group.
    }

    // Represents an item or entry within a calendar group, linking specific calendars to their respective groups.
    [Table("CalendarGroupItems")]
    public class CalendarGroupItemModel
    {
        // Constructor to initialize a CalendarGroupItem with identifiers for the item, its calendar, and its group.
        public CalendarGroupItemModel(string id, string calendarId, string calendarGroupId)
        {
            Id = id;
            CalendarId = calendarId;
            CalendarGroupId = calendarGroupId;
        }

        [Key, MaxLength(36)]
        public string Id { get; set; }  // Unique identifier for the calendar group item, restricted to 36 characters.
        [MaxLength(36)]
        public string CalendarId { get; set; }  // Identifier of the calendar associated with this item, restricted to 36 characters.
        [MaxLength(36)]
        public string CalendarGroupId { get; set; }  // Identifier of the group to which this item belongs, restricted to 36 characters.
    }

    // Represents a calendar, detailing its name, associated organization, and settings like time zone and visibility.
    [Table("Calendars")]
    public class CalendarModel
    {
        // Constructor to initialize a Calendar with essential attributes.
        public CalendarModel(string id, string name, string organizationId, string timeZone, bool isPublic, string createdBy, DateTime created)
        {
            Id = id;
            Name = name;
            OrganizationId = organizationId;
            TimeZone = timeZone;
            IsPublic = isPublic;
            CreatedBy = createdBy;
            Created = created;
        }

        [Key, MaxLength(36)]
        public string Id { get; set; }  // Unique identifier for the calendar, restricted to 36 characters.
        public string Name { get; set; }  // Name of the calendar.
        public string? Description { get; set; }  // Optional description of the calendar.
        [MaxLength(36)]
        public string OrganizationId { get; set; }  // Identifier of the organization owning this calendar, restricted to 36 characters.
        [MaxLength(36)]
        public string TimeZone { get; set; }  // Time zone in which the calendar operates, restricted to 36 characters.
        [MaxLength(12)]
        public string? Color { get; set; }  // Optional color code for the calendar, restricted to 12 characters.
        public bool IsPublic { get; set; }  // Indicates if the calendar is public.
        public bool IsDeleted { get; set; }  // Indicates if the calendar has been marked as deleted.
        public string? DefaultLocation { get; set; }  // Optional default location for events in the calendar.
        public int MaxAttendees { get; set; }  // Maximum number of attendees per event.
        public int MinAttendees { get; set; }  // Minimum number of attendees required for an event.
        public int TimeScale { get; set; }  // Time scale granularity for events (in minutes).
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }

    }

    // Represents a reservation within a calendar, detailing the reservation's timeframe, booker, and status.
    [Table("Reservations")]
    public class ReservationModel
    {
        // Constructor to initialize a Reservation with its essential attributes.
        public ReservationModel(string id, string calendarId, string organizationId, string name, DateTime startFrom, DateTime endAt, bool isWholeDay, string bookerId, string status)
        {
            Id = id;
            CalendarId = calendarId;
            OrganizationId = organizationId;
            Name = name;
            StartFrom = startFrom;
            EndAt = endAt;
            IsWholeDay = isWholeDay;
            BookerId = bookerId;
            Status = status;
        }

        [Key, MaxLength(36)]
        public string Id { get; set; }  // Unique identifier for the reservation, restricted to 36 characters.
        public string OrganizationId { get; set; }
        public string CalendarId { get; set; }
        public string Name { get; set; }  // Name of the reservation.
        public string? Description { get; set; }  // Optional description of the reservation.
        [MaxLength(36)]
        public string? Color { get; set; }  // Optional color code for the reservation, restricted to 36 characters.
        [MaxLength(36)]
        public string? CartId { get; set; }  // Optional cart identifier if the reservation is part of a booking system, restricted to 36 characters.
        public DateTime StartFrom { get; set; }  // Start time and date of the reservation.
        public DateTime EndAt { get; set; }  // End time and date of the reservation.
        public bool IsWholeDay { get; set; }
        [MaxLength(36)]
        public string BookerId { get; set; }  // Identifier of the person who booked the reservation, restricted to 36 characters.
        [MaxLength(36)]
        public string Status { get; set; }  // Current status of the reservation, restricted to 36 characters.
        public string? UnderName { get; set; }  // Optional name under which the reservation is booked.
        public DateTime Created { get; set; }  // Creation date and time of the reservation.
       
        public bool IsDeleted { get; set; }  // Indicates if the reservation has been marked as deleted.
        public DateTime Deleted { get; set; }  // Date and time when the reservation was marked as deleted.
    }
}
