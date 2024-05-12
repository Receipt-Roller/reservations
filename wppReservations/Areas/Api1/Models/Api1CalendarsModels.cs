using Newtonsoft.Json;
using wppReservations.Models;

namespace wppReservations.Areas.Api1.Models
{
    /// <summary>
    /// Represents the request model for creating a new calendar.
    /// This model includes all necessary details required to create a calendar within an organization.
    /// </summary>
    public class CalendarCreateRequestModel
    {
        /// <summary>
        /// Initializes a new instance of the CalendarCreateRequestModel with specified details.
        /// </summary>
        /// <param name="name">The name of the calendar.</param>
        /// <param name="timeZone">The time zone in which the calendar operates.</param>
        /// <param name="isPublic">Indicates whether the calendar is public or private.</param>
        /// <param name="maxAttendees">The maximum number of attendees allowed per event.</param>
        /// <param name="minAttendees">The minimum number of attendees required for an event.</param>
        /// <param name="timeScale">The time scale in minutes used for calendar events.</param>
        /// <param name="createdBy">The identifier of the user creating the calendar.</param>
        /// <param name="created">The date and time when the calendar was created.</param>
        public CalendarCreateRequestModel(string name, string timeZone,
            bool isPublic, int maxAttendees, int minAttendees, int timeScale)
        {
            Name = name;
            TimeZone = timeZone;
            IsPublic = isPublic;
            MaxAttendees = maxAttendees;
            MinAttendees = minAttendees;
            TimeScale = timeScale;
        }

        /// <summary>
        /// Gets or sets the name of the calendar.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the calendar. Optional.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the time zone of the calendar.
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets or sets the color associated with the calendar. Optional.
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the calendar is public.
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Gets or sets the default location for events in the calendar. Optional.
        /// </summary>
        public string? DefaultLocation { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of attendees allowed per event.
        /// </summary>
        public int MaxAttendees { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of attendees required for an event.
        /// </summary>
        public int MinAttendees { get; set; }

        /// <summary>
        /// Gets or sets the time scale, in minutes, for events in the calendar.
        /// </summary>
        public int TimeScale { get; set; }

   
    }


    /// <summary>
    /// Represents the response model for a calendar creation request.
    /// This model contains the details of the newly created calendar, encapsulated within a CalendarViewModel.
    /// </summary>
    public class CalendarCreateResponseModel
    {
        /// <summary>
        /// Initializes a new instance of the CalendarCreateResponseModel with the specified calendar details.
        /// </summary>
        /// <param name="calendar">The calendar view model that includes the details of the newly created calendar.</param>
        public CalendarCreateResponseModel(CalendarViewModel calendar)
        {
            Calendar = calendar;
        }

        /// <summary>
        /// Gets or sets the CalendarViewModel that includes the details of the newly created calendar.
        /// </summary>
        public CalendarViewModel Calendar { get; set; }
    }

    /// <summary>
    /// Represents the search criteria for querying calendars.
    /// This model includes pagination parameters and can optionally include search keywords and sorting instructions.
    /// </summary>
    public class CalendarsSearchRequestModel
    {
        /// <summary>
        /// Initializes a new instance of the CalendarsSearchRequestModel with specified pagination details.
        /// </summary>
        /// <param name="currentPage">The page number of the search results to retrieve.</param>
        /// <param name="itemsPerPage">The number of items to display per page in the search results.</param>
        public CalendarsSearchRequestModel(int currentPage, int itemsPerPage)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
        }

        /// <summary>
        /// Gets or sets the search keyword to filter the calendars. Optional.
        /// </summary>
        /// <remarks>
        /// If provided, the search will include only calendars that contain this keyword in their searchable fields.
        /// </remarks>
        public string? Keyword { get; set; }

        /// <summary>
        /// Gets or sets the sorting criteria for the search results. Optional.
        /// </summary>
        /// <remarks>
        /// Example formats include "Name asc" or "Created desc". If not provided, a default sort may be applied.
        /// </remarks>
        public string? Sort { get; set; }

        /// <summary>
        /// Gets or sets the current page number of the search results.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the number of items per page to be returned in the search results.
        /// </summary>
        public int ItemsPerPage { get; set; }
    }
    /// <summary>
    /// Represents the response model for a search query on calendars.
    /// This model provides a structured format for pagination and includes the results of the search.
    /// </summary>
    public class CalendarsSearchResponseModel
    {
        /// <summary>
        /// Initializes a new instance of the CalendarsSearchResponseModel with a list of calendar view models.
        /// </summary>
        /// <param name="calendars">A list of CalendarViewModels that represent the search results.</param>
        public CalendarsSearchResponseModel(List<CalendarViewModel> calendars)
        {
            Items = calendars;
        }

        /// <summary>
        /// Gets or sets the keyword used in the search query. Optional.
        /// </summary>
        /// <remarks>
        /// If provided, this was used to filter the results based on searchable fields within the calendar data.
        /// </remarks>
        public string? Keyword { get; set; }

        /// <summary>
        /// Gets or sets the sorting criteria used in the search query. Optional.
        /// </summary>
        /// <remarks>
        /// Examples include "Name asc" or "Created desc". If not provided, a default sort may have been applied.
        /// </remarks>
        public string? Sort { get; set; }

        /// <summary>
        /// Gets or sets the current page number of the search results, indicating where in the pagination the returned data is situated.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the number of items per page that were returned in this segment of the search results.
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the total number of items that matched the search criteria, useful for calculating the total number of pages.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages available based on the current pagination settings.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the list of CalendarViewModels that represent the search results.
        /// </summary>
        public List<CalendarViewModel> Items { get; set; }
    }

    /// <summary>
    /// Represents the response model for retrieving a calendar.
    /// This model is used to provide detailed information about a specific calendar following a retrieval request.
    /// </summary>
    public class CalendarGetResponseModel
    {
        /// <summary>
        /// Initializes a new instance of the CalendarGetResponseModel with the specified calendar view model.
        /// </summary>
        /// <param name="calendar">The calendar view model that includes all relevant details of the retrieved calendar.</param>
        public CalendarGetResponseModel(CalendarViewModel calendar)
        {
            Calendar = calendar;
        }

        /// <summary>
        /// Gets or sets the CalendarViewModel that includes all the details of the retrieved calendar.
        /// </summary>
        public CalendarViewModel Calendar { get; set; }
    }

    /// <summary>
    /// Represents the view model for a calendar.
    /// This model is used primarily for data transfer within the API, especially for encapsulating calendar details in responses.
    /// </summary>

    public class CalendarViewModel
    {
        /// <summary>
        /// Gets or sets the CalendarModel that contains detailed information about the calendar.
        /// This property may be null if the specific calendar details are not available or not necessary for the context in which the view model is used.
        /// </summary>
        public CalendarModel? Calendar { get; set; }
        public int NumOfValidReservations { get; set; }
        public List<ReservationViewModel>? Reservations { get; set; }
    }

    /// <summary>
    /// Represents the request model for updating an existing calendar.
    /// This model captures all necessary details required to modify a calendar within an organization.
    /// </summary>
    public class CalendarUpdateRequestModel
    {
        /// <summary>
        /// Initializes a new instance of the CalendarUpdateRequestModel with specified details.
        /// </summary>
        /// <param name="name">The new name of the calendar.</param>
        /// <param name="organizationId">The ID of the organization to which the calendar belongs.</param>
        /// <param name="timeZone">The time zone in which the calendar operates.</param>
        /// <param name="isPublic">Indicates whether the calendar is public or private.</param>
        /// <param name="maxAttendees">The maximum number of attendees allowed per event.</param>
        /// <param name="minAttendees">The minimum number of attendees required for an event.</param>
        /// <param name="timeScale">The time scale in minutes used for calendar events.</param>
        public CalendarUpdateRequestModel(string name, string organizationId, string timeZone,
            bool isPublic, int maxAttendees, int minAttendees, int timeScale)
        {
            Name = name;
            OrganizationId = organizationId;
            TimeZone = timeZone;
            IsPublic = isPublic;
            MaxAttendees = maxAttendees;
            MinAttendees = minAttendees;
            TimeScale = timeScale;
        }

        /// <summary>
        /// Gets or sets the name of the calendar.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the optional description of the calendar.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the ID of the organization to which the calendar belongs.
        /// </summary>
        public string OrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the time zone of the calendar.
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets or sets the optional color associated with the calendar.
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the calendar is public.
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Gets or sets the optional default location for events in the calendar.
        /// </summary>
        public string? DefaultLocation { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of attendees allowed per event.
        /// </summary>
        public int MaxAttendees { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of attendees required for an event.
        /// </summary>
        public int MinAttendees { get; set; }

        /// <summary>
        /// Gets or sets the time scale, in minutes, for events in the calendar.
        /// </summary>
        public int TimeScale { get; set; }
    }

    /// <summary>
    /// Represents the response model for an update request on a calendar.
    /// This model provides the details of the updated calendar, ensuring that the requester can verify the new state of the calendar post-update.
    /// </summary>
    public class CalendarUpdateResponseModel
    {
        /// <summary>
        /// Initializes a new instance of the CalendarUpdateResponseModel with the updated calendar details.
        /// </summary>
        /// <param name="calendar">The calendar model that includes all updated details of the calendar.</param>
        public CalendarUpdateResponseModel(CalendarModel calendar)
        {
            Calendar = calendar;
        }

        /// <summary>
        /// Gets or sets the CalendarModel that includes the details of the updated calendar.
        /// </summary>
        public CalendarModel Calendar { get; set; }
    }

    /// <summary>
    /// Represents the response model for a calendar deletion request.
    /// This model provides details of the calendar that has been deleted, allowing for verification and record-keeping.
    /// </summary>
    public class CalendarDeleteResponseModel
    {
        /// <summary>
        /// Initializes a new instance of the CalendarDeleteResponseModel with the specified deleted calendar details.
        /// </summary>
        /// <param name="deletedCalendar">The calendar model that includes all relevant details of the deleted calendar.</param>
        public CalendarDeleteResponseModel(CalendarModel deletedCalendar)
        {
            Calendar = deletedCalendar;
        }

        /// <summary>
        /// Gets or sets the CalendarModel that contains the details of the deleted calendar.
        /// This property provides a final snapshot of the calendar prior to its deletion.
        /// </summary>
        public CalendarModel Calendar { get; set; }
    }

}
