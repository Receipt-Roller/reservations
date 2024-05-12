using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using wppReservations.Areas.Api1.Models;
using wppReservations.Data;
using wppReservations.Models;

namespace wppReservations.Areas.Api1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<UserModel> _userManager;

        public ReservationsController(ApplicationDbContext db,
           UserManager<UserModel> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        /// <summary>
        /// Creates a new reservation within a specified calendar and organization.
        /// </summary>
        /// <param name="organizationId">The ID of the organization under which the reservation is made.</param>
        /// <param name="calendarId">The ID of the calendar under which the reservation is made.</param>
        /// <param name="request">The reservation details.</param>
        /// <returns>Returns the created reservation details or appropriate error messages.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /{organizationId}/calendar/{calendarId}/reservation
        ///     {
        ///         "name": "Board Meeting",
        ///         "startFrom": "2024-05-12T14:00:00",
        ///         "endAt": "2024-05-12T15:00:00",
        ///         "isWholeDay": false,
        ///         "status": "Confirmed",
        ///         "description": "Annual board meeting",
        ///         "cartId": "cart123",
        ///         "color": "blue",
        ///         "underName": "John Doe"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the newly created reservation</response>
        /// <response code="400">If the request body is null</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the specified organization does not exist</response>
        [HttpPost("/{organizationId}/calendar/{calendarId}/reservation")]
        [SwaggerOperation(Summary = "Create a new reservation", Description = "Creates a new reservation within a specified calendar and organization.")]
        [SwaggerResponse(statusCode: 200, type: typeof(ReservationCreateResponseModel), description : "Successfully created the reservation")]
        [SwaggerResponse(statusCode: 400, description : "Bad request if the request body is null")]
        [SwaggerResponse(statusCode: 401, description : "Unauthorized if the user is not logged in")]
        [SwaggerResponse(statusCode: 404, description : "Not found if the specified organization or calendar does not exist")]
        public async Task<ActionResult<ReservationCreateResponseModel>> CreateReservationAsync(
          [FromRoute] string organizationId, [FromRoute] string calendarId,
          [FromBody] ReservationCreateRequestModel request)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User must be logged in to create a reservation");
            }

            var organization = _db.Organizations.Find(organizationId);
            if (organization == null)
            {
                return NotFound("Organization not found");
            }

            var reservation = new ReservationModel(Guid.NewGuid().ToString(),
                calendarId, organizationId,
                request.Name,
                request.StartFrom, request.EndAt, request.IsWholeDay, user.Id, request.Status)
            {
                Description = request.Description,
                CartId = request.CartId,
                Color = request.Color,
                UnderName = request.UnderName,
                Created = DateTime.Now
            };

            _db.Reservations.Add(reservation);
            await _db.SaveChangesAsync();

            var result = new ReservationCreateResponseModel(new ReservationViewModel()
            {
                Reservation = reservation
            });

            return Ok(result);
        }

        /// <summary>
        /// Searches for reservations within a specific calendar and organization based on the provided search criteria.
        /// </summary>
        /// <param name="organizationId">The ID of the organization to which the calendar belongs.</param>
        /// <param name="calendarId">The ID of the calendar within which to search for reservations.</param>
        /// <param name="request">The search parameters including pagination details, optional keyword, and sort criteria.</param>
        /// <returns>A list of reservations that match the search criteria along with pagination details.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /{organizationId}/calendar/{calendarId}/reservation/search
        ///     {
        ///         "keyword": "meeting",
        ///         "sort": "name asc",
        ///         "currentPage": 1,
        ///         "itemsPerPage": 10
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the list of matching reservations along with pagination info</response>
        /// <response code="400">If the pagination parameters are invalid</response>
        [HttpPost("/{organizationId}/calendar/{calendarId}/reservation/search")]
        [SwaggerOperation(Summary = "Search reservations", Description = "Performs a search for reservations within a specified calendar and organization based on search criteria.")]
        [SwaggerResponse(statusCode: 200, type: typeof(ReservationsSearchResponseModel), description : "Successful retrieval of reservation list with pagination")]
        [SwaggerResponse(statusCode: 400, description : "Invalid request parameters")]
        public ActionResult<ReservationsSearchResponseModel> SearchReservation(
            [FromRoute] string organizationId, [FromRoute] string calendarId, [FromBody] ReservationsSearchRequestModel request)
        {
            // Input validation
            if (request.CurrentPage <= 0)
            {
                return BadRequest("CurrentPage must be greater than 0.");
            }
            if (request.ItemsPerPage <= 0)
            {
                return BadRequest("ItemsPerPage must be greater than 0.");
            }

            var reservations = from o in _db.Reservations
                               where o.OrganizationId == organizationId && o.CalendarId == calendarId
                               select o;

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                reservations = reservations.Where(o => o.Name.Contains(request.Keyword));
            }

            // Count total items to support pagination
            int totalItems = reservations.Count();
            var items = reservations
                        .Skip(request.ItemsPerPage * (request.CurrentPage - 1))
                        .Take(request.ItemsPerPage)
                        .Select(r => new ReservationViewModel { Reservation = r })
                        .ToList();

            // Prepare the response model with pagination info
            var result = new ReservationsSearchResponseModel(items)
            {
                Keyword = request.Keyword,
                Sort = request.Sort,
                CurrentPage = request.CurrentPage,
                ItemsPerPage = request.ItemsPerPage,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((double)totalItems / request.ItemsPerPage)
            };

            return Ok(result);
        }
        
        
        /// <summary>
        /// Retrieves a specific reservation by its ID within a given organization and calendar.
        /// </summary>
        /// <param name="organizationId">The ID of the organization to which the reservation belongs.</param>
        /// <param name="calendarId">The ID of the calendar in which the reservation is scheduled.</param>
        /// <param name="id">The unique identifier of the reservation to retrieve.</param>
        /// <returns>Returns the detailed information of the reservation if found, or an error if not found.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /{organizationId}/calendar/{calendarId}/reservation/{id}
        ///
        /// </remarks>
        /// <response code="200">Returns the detailed information of the reservation</response>
        /// <response code="404">If the reservation is not found within the specified organization and calendar</response>
        [HttpGet("/{organizationId}/calendar/{calendarId}/reservation/{id}")]
        [SwaggerOperation(Summary = "Retrieve a reservation", Description = "Retrieves a specific reservation by its ID within a given organization and calendar.")]
        [SwaggerResponse(statusCode: 200, type: typeof(ReservationGetResponseModel), description : "Successfully retrieved the reservation")]
        [SwaggerResponse(statusCode: 404, description : "Reservation not found")]
        public ActionResult<ReservationGetResponseModel> GetReservation(
            [FromRoute] string organizationId, [FromRoute] string calendarId,
            [FromRoute] string id)
        {
            // Query the database for the reservation with the specified ID and ensure it belongs to the specified organization and calendar
            var reservation = (from o in _db.Reservations
                               where o.Id == id && o.OrganizationId == organizationId &&
                                     o.CalendarId == calendarId
                               select new ReservationViewModel
                               {
                                   Reservation = o
                               }).FirstOrDefault();

            // Check if the reservation was found
            if (reservation == null)
            {
                return NotFound("The specified reservation was not found within the given organization and calendar.");
            }

            var result = new ReservationGetResponseModel(reservation);

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing reservation within a specified calendar and organization.
        /// </summary>
        /// <param name="organizationId">The ID of the organization to which the reservation belongs.</param>
        /// <param name="calendarId">The ID of the calendar in which the reservation is scheduled.</param>
        /// <param name="id">The unique identifier of the reservation to update.</param>
        /// <param name="request">The updated reservation details.</param>
        /// <returns>Returns the updated reservation details or appropriate error messages if the reservation cannot be found or updated.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /{organizationId}/calendar/{calendarId}/reservation/{id}
        ///     {
        ///         "name": "Updated Meeting",
        ///         "description": "Updated description here",
        ///         "color": "green",
        ///         "bookerId": "user123",
        ///         "cartId": "cart456",
        ///         "deleted": false,
        ///         "endAt": "2024-05-20T15:00:00",
        ///         "isDeleted": false,
        ///         "isWholeDay": false,
        ///         "startFrom": "2024-05-20T14:00:00",
        ///         "status": "Confirmed",
        ///         "underName": "Jane Doe"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Successfully updated the reservation</response>
        /// <response code="404">If the reservation, organization, or calendar is not found</response>
        [HttpPut("/{organizationId}/calendar/{calendarId}/reservation/{id}")]
        [SwaggerOperation(Summary = "Update a reservation", Description = "Updates an existing reservation within a specified calendar and organization.")]
        [SwaggerResponse(statusCode: 200, type: typeof(ReservationUpdateResponseModel), description : "Successfully updated the reservation")]
        [SwaggerResponse(statusCode: 404, description : "Not found if the specified reservation, organization, or calendar does not exist")]
        public async Task<ActionResult<ReservationUpdateResponseModel>> UpdateReservation(
            [FromRoute] string organizationId,
            [FromRoute] string calendarId,
            [FromRoute] string id, [FromBody] ReservationUpdateRequestModel request)
        {
            var original = await _db.Reservations.FindAsync(id);

            if (original == null)
            {
                return NotFound("The reservation with the specified ID was not found.");
            }

            if (original.OrganizationId != organizationId)
            {
                return NotFound("The reservation does not belong to the specified organization.");
            }

            if (original.CalendarId != calendarId)
            {
                return NotFound("The reservation does not belong to the specified calendar.");
            }

            original.Name = request.Name;
            original.Description = request.Description;
            original.Color = request.Color;
            original.BookerId = request.BookerId;
            original.CartId = request.CartId;
            original.Deleted = request.Deleted;
            original.EndAt = request.EndAt;
            original.IsDeleted = request.IsDeleted;
            original.IsWholeDay = request.IsWholeDay;
            original.StartFrom = request.StartFrom;
            original.Status = request.Status;
            original.UnderName = request.UnderName;

            _db.Reservations.Update(original);
            await _db.SaveChangesAsync();

            var result = new ReservationUpdateResponseModel(original);

            return Ok(result);
        }

        /// <summary>
        /// Deletes a reservation within a specified calendar and organization.
        /// </summary>
        /// <param name="organizationId">The ID of the organization to which the reservation belongs.</param>
        /// <param name="calendarId">The ID of the calendar from which the reservation is to be deleted.</param>
        /// <param name="id">The unique identifier of the reservation to delete.</param>
        /// <returns>Returns a confirmation of the deletion or an error message if the reservation cannot be found or is unauthorized.</returns>
        /// <remarks>
        /// This method deletes the reservation and returns the details of the deleted reservation.
        /// </remarks>
        /// <response code="200">Successfully deleted the reservation and returns the deleted reservation details</response>
        /// <response code="404">If the reservation is not found within the specified organization and calendar</response>
        /// <response code="401">If the reservation does not belong to the specified organization or calendar</response>
        [HttpDelete("/{organizationId}/calendar/{calendarId}/reservation/{id}")]
        [SwaggerOperation(Summary = "Delete a reservation", Description = "Deletes a specific reservation within a specified calendar and organization.")]
        [SwaggerResponse(statusCode: 200, type: typeof(ReservationDeleteResponseModel), description : "Successfully deleted the reservation")]
        [SwaggerResponse(statusCode: 404, description : "Not found if the specified reservation does not exist")]
        [SwaggerResponse(statusCode: 401, description : "Unauthorized if the reservation does not belong to the specified organization or calendar")]
        public async Task<ActionResult<ReservationDeleteResponseModel>> DeleteReservationAsync(
            [FromRoute] string organizationId, [FromRoute] string calendarId,
            [FromRoute] string id)
        {
            var original = await _db.Reservations.FindAsync(id);

            if (original == null)
            {
                return NotFound("The reservation with the specified ID was not found.");
            }

            if (original.OrganizationId != organizationId)
            {
                return Unauthorized("The reservation does not belong to the specified organization.");
            }

            if (original.CalendarId != calendarId)
            {
                return Unauthorized("The reservation does not belong to the specified calendar.");
            }

            _db.Reservations.Remove(original);
            await _db.SaveChangesAsync();

            var result = new ReservationDeleteResponseModel(original);

            return Ok(result);
        }
    }
}
