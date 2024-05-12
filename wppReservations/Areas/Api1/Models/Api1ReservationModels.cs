using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
using wppReservations.Models;

namespace wppReservations.Areas.Api1.Models
{

    /// <summary>
    /// Represents the request model for creating a new reservation.
    /// This model includes all necessary details required to create a reservation within a system.
    /// </summary>
    public class ReservationCreateRequestModel
    {
        /// <summary>
        /// Initializes a new instance of the ReservationCreateRequestModel with specified reservation details.
        /// </summary>
        /// <param name="name">The name of the reservation.</param>
        /// <param name="startFrom">The start time and date of the reservation.</param>
        /// <param name="endAt">The end time and date of the reservation.</param>
        /// <param name="isWholeDay">Indicates whether the reservation spans the whole day.</param>
        /// <param name="bookerId">The identifier of the person who made the reservation.</param>
        /// <param name="status">The current status of the reservation.</param>
        /// <param name="created">The date and time when the reservation was created.</param>
        public ReservationCreateRequestModel(string name, DateTime startFrom, DateTime endAt,
            bool isWholeDay, string status)
        {
            Name = name;
            StartFrom = startFrom;
            EndAt = endAt;
            IsWholeDay = isWholeDay;
            Status = status;
        }

        /// <summary>
        /// Name of the reservation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional description of the reservation.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Optional color code for the reservation, restricted to 36 characters.
        /// </summary>
        [MaxLength(36)]
        public string? Color { get; set; }

        /// <summary>
        /// Optional cart identifier if the reservation is part of a booking system, restricted to 36 characters.
        /// </summary>
        [MaxLength(36)]
        public string? CartId { get; set; }

        /// <summary>
        /// Start time and date of the reservation.
        /// </summary>
        public DateTime StartFrom { get; set; }

        /// <summary>
        /// End time and date of the reservation.
        /// </summary>
        public DateTime EndAt { get; set; }

        /// <summary>
        /// Indicates whether the reservation spans the entire day.
        /// </summary>
        public bool IsWholeDay { get; set; }

        /// <summary>
        /// Current status of the reservation, restricted to 36 characters.
        /// </summary>
        [MaxLength(36)]
        public string Status { get; set; }

        /// <summary>
        /// Optional name under which the reservation is booked.
        /// </summary>
        public string? UnderName { get; set; }

    }
    /// <summary>
    /// Represents the response model for a successfully created reservation.
    /// This model contains the details of the newly created reservation, encapsulated within a ReservationViewModel.
    /// </summary>
    public class ReservationCreateResponseModel
    {
        /// <summary>
        /// Initializes a new instance of the ReservationCreateResponseModel with the specified reservation details.
        /// </summary>
        /// <param name="reservation">The reservation view model that includes all relevant details of the newly created reservation.</param>
        public ReservationCreateResponseModel(ReservationViewModel reservation)
        {
            Reservation = reservation;
        }

        /// <summary>
        /// Gets or sets the ReservationViewModel that includes the details of the newly created reservation.
        /// </summary>
        public ReservationViewModel Reservation { get; set; }
    }

    /// <summary>
    /// Represents the request model for searching reservations based on pagination and optional filtering criteria.
    /// This model allows users to query reservations by page and apply filters such as keywords or sorting.
    /// </summary>
    public class ReservationsSearchRequestModel
    {
        /// <summary>
        /// Initializes a new instance of the ReservationsSearchRequestModel with specified pagination details.
        /// </summary>
        /// <param name="currentPage">The page number of the search results to retrieve.</param>
        /// <param name="itemsPerPage">The number of items to display per page in the search results.</param>
        public ReservationsSearchRequestModel(int currentPage, int itemsPerPage)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
        }

        /// <summary>
        /// Gets or sets the search keyword to filter the reservations. Optional.
        /// </summary>
        /// <remarks>
        /// If provided, the search will include only reservations that contain this keyword in their searchable fields.
        /// This is useful for quickly locating reservations by relevant identifiers or descriptions.
        /// </remarks>
        public string? Keyword { get; set; }

        /// <summary>
        /// Gets or sets the sorting criteria for the search results. Optional.
        /// </summary>
        /// <remarks>
        /// Sort criteria should be specified in the format "FieldName direction", such as "Name asc" or "Created desc".
        /// If not provided, a default sorting may be applied based on the internal logic of the application.
        /// </remarks>
        public string? Sort { get; set; }

        /// <summary>
        /// Gets or sets the current page number of the search results, facilitating pagination.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the number of items per page to be returned in the search results.
        /// This helps in managing the volume of data returned and supports efficient navigation through large sets of data.
        /// </summary>
        public int ItemsPerPage { get; set; }
    }

    /// <summary>
    /// Represents the response model for a search operation on reservations.
    /// This model encapsulates the results and pagination details, providing a structured response to search queries.
    /// </summary>
    public class ReservationsSearchResponseModel
    {
        /// <summary>
        /// Initializes a new instance of the ReservationsSearchResponseModel with a list of ReservationViewModels.
        /// These models represent the search results, formatted according to the specified search and pagination parameters.
        /// </summary>
        /// <param name="reservations">A list of ReservationViewModels that represent the search results.</param>
        public ReservationsSearchResponseModel(List<ReservationViewModel> reservations)
        {
            Items = reservations;
        }

        /// <summary>
        /// Gets or sets the keyword used in the search query, if any. This is used to filter the results based on searchable fields within the reservation data.
        /// </summary>
        /// <remarks>
        /// Providing a keyword helps to refine search results to only include items that contain the keyword in relevant fields.
        /// </remarks>
        public string? Keyword { get; set; }

        /// <summary>
        /// Gets or sets the sorting criteria used in the search query. This parameter is optional and determines the order of the search results.
        /// </summary>
        /// <remarks>
        /// Examples include "Name asc" or "Created desc". If no sorting parameter is provided, a default sort order may be applied.
        /// </remarks>
        public string? Sort { get; set; }

        /// <summary>
        /// Gets or sets the current page number of the search results. This parameter helps in navigating through paginated data.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the number of items per page that were returned in this segment of the search results.
        /// This controls how much data is presented to the user at one time and aids in pagination control.
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the total number of items that matched the search criteria. This is crucial for calculating the total number of pages.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages available, calculated based on the total number of items and the number of items per page.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the list of ReservationViewModels that represent the search results. Each model contains details of a reservation found in the search.
        /// </summary>
        public List<ReservationViewModel> Items { get; set; }
    }

    /// <summary>
    /// Represents the response model for retrieving a reservation.
    /// This model is used to provide detailed information about a specific reservation following a retrieval request.
    /// </summary>
    public class ReservationGetResponseModel
    {
        /// <summary>
        /// Initializes a new instance of the ReservationGetResponseModel with the specified reservation view model.
        /// This constructor sets up the response model with all relevant details of the retrieved reservation.
        /// </summary>
        /// <param name="reservation">The reservation view model that includes all relevant details of the retrieved reservation.</param>
        public ReservationGetResponseModel(ReservationViewModel reservation)
        {
            Reservation = reservation;
        }

        /// <summary>
        /// Gets or sets the ReservationViewModel that includes all the details of the retrieved reservation.
        /// This property provides access to detailed attributes and values of the reservation, such as date, time, participants, and status, among others.
        /// </summary>
        public ReservationViewModel Reservation { get; set; }
    }

    /// <summary>
    /// Represents a view model for a reservation. This model is typically used to encapsulate the reservation data 
    /// that is transferred between the backend and the frontend layers, making it easier to manage data transformations 
    /// and customizations specific to the view requirements.
    /// </summary>
    public class ReservationViewModel
    {
        /// <summary>
        /// Gets or sets the ReservationModel. This property may contain the detailed information of a reservation, 
        /// including dates, times, participant details, and other relevant reservation metadata.
        /// </summary>
        /// <remarks>
        /// The property is nullable, meaning that there may be contexts where a reservation detail is not required 
        /// or not available. This flexibility allows the view model to be used in a variety of scenarios, such as 
        /// creating new reservations or updating existing ones without fully specifying all details initially.
        /// </remarks>
        public ReservationModel? Reservation { get; set; }
    }

    /// <summary>
    /// Represents the request model for updating an existing reservation.
    /// This model captures all necessary details required to modify a reservation within an organization,
    /// including scheduling details, participant information, and reservation status.
    /// </summary>
    public class ReservationUpdateRequestModel
    {
        /// <summary>
        /// Initializes a new instance of the ReservationUpdateRequestModel with mandatory parameters for updating a reservation.
        /// </summary>
        /// <param name="name">Name of the reservation.</param>
        /// <param name="startFrom">Start time and date of the reservation.</param>
        /// <param name="endAt">End time and date of the reservation.</param>
        /// <param name="isWholeDay">Indicates whether the reservation spans the entire day.</param>
        /// <param name="bookerId">Identifier of the person who booked the reservation.</param>
        /// <param name="status">Current status of the reservation.</param>
        public ReservationUpdateRequestModel(string name, DateTime startFrom, DateTime endAt,
            bool isWholeDay, string bookerId, string status)
        {
            Name = name;
            StartFrom = startFrom;
            EndAt = endAt;
            IsWholeDay = isWholeDay;
            BookerId = bookerId;
            Status = status;
        }

        /// <summary>
        /// Name of the reservation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional description providing additional details about the reservation.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Optional color code for the reservation, used for visual identification, restricted to 36 characters.
        /// </summary>
        [MaxLength(36)]
        public string? Color { get; set; }

        /// <summary>
        /// Optional cart identifier, used if the reservation is part of a booking system, restricted to 36 characters.
        /// </summary>
        [MaxLength(36)]
        public string? CartId { get; set; }

        /// <summary>
        /// Start time and date of the reservation, defining when the reservation begins.
        /// </summary>
        public DateTime StartFrom { get; set; }

        /// <summary>
        /// End time and date of the reservation, defining when the reservation concludes.
        /// </summary>
        public DateTime EndAt { get; set; }

        /// <summary>
        /// Indicates whether the reservation is booked for the entire day.
        /// </summary>
        public bool IsWholeDay { get; set; }

        /// <summary>
        /// Identifier of the person who made the reservation, providing a link to the responsible party, restricted to 36 characters.
        /// </summary>
        [MaxLength(36)]
        public string BookerId { get; set; }

        /// <summary>
        /// Current status of the reservation, such as 'Confirmed', 'Cancelled', etc., restricted to 36 characters.
        /// </summary>
        [MaxLength(36)]
        public string Status { get; set; }

        /// <summary>
        /// Optional name under which the reservation is booked, providing an alternative reference or alias for the booking.
        /// </summary>
        public string? UnderName { get; set; }

        /// <summary>
        /// Indicates if the reservation has been marked as deleted, providing a flag for soft deletion scenarios.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Date and time when the reservation was marked as deleted, used in tracking changes and managing records.
        /// </summary>
        public DateTime Deleted { get; set; }
    }
    /// <summary>
    /// Represents the response model for an update request on a reservation.
    /// This model provides the details of the updated reservation, ensuring that the requester can verify the new state of the reservation post-update.
    /// </summary>
    public class ReservationUpdateResponseModel
    {
        /// <summary>
        /// Initializes a new instance of the ReservationUpdateResponseModel with the updated reservation details.
        /// </summary>
        /// <param name="reservation">The reservation model that includes all updated details of the reservation.</param>
        public ReservationUpdateResponseModel(ReservationModel reservation)
        {
            Reservation = reservation;
        }

        /// <summary>
        /// Gets or sets the ReservationModel that includes the details of the updated reservation.
        /// This property encapsulates the entire set of reservation details after they have been modified,
        /// allowing for a comprehensive view of the updated reservation state.
        /// </summary>
        public ReservationModel Reservation { get; set; }
    }

    /// <summary>
    /// Represents the response model for a reservation deletion request.
    /// This model provides details of the reservation that has been deleted, allowing for verification and record-keeping.
    /// </summary>
    public class ReservationDeleteResponseModel
    {
        /// <summary>
        /// Initializes a new instance of the ReservationDeleteResponseModel with the specified deleted reservation details.
        /// This constructor sets up the model with a snapshot of the reservation as it was just before deletion, 
        /// aiding in confirming the correct reservation was removed and providing a record for audit purposes.
        /// </summary>
        /// <param name="deletedReservation">The reservation model that includes all relevant details of the deleted reservation.</param>
        public ReservationDeleteResponseModel(ReservationModel deletedReservation)
        {
            Reservation = deletedReservation;
        }

        /// <summary>
        /// Gets or sets the ReservationModel that contains the details of the deleted reservation.
        /// This property provides a final snapshot of the reservation prior to its deletion, useful for logging, auditing, or
        /// other post-deletion processes that might require a record of the reservation's final state.
        /// </summary>
        public ReservationModel Reservation { get; set; }
    }

}
