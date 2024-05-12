using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class CalendarsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<UserModel> _userManager;

        public CalendarsController(ApplicationDbContext db,
            UserManager<UserModel> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        /// <summary>
        /// Creates a new calendar associated with a specified organization.
        /// </summary>
        /// <param name="organizationId">The ID of the organization to which the calendar will be associated.</param>
        /// <param name="request">The details of the calendar to be created.</param>
        /// <returns>Returns the created calendar details or appropriate error messages.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /{organizationId}/calendar
        ///     {
        ///        "name": "Team Meetings",
        ///        "timeZone": "Eastern Standard Time",
        ///        "isPublic": true,
        ///        "description": "Calendar for all team meetings",
        ///        "defaultLocation": "Board Room",
        ///        "color": "blue",
        ///        "maxAttendees": 50,
        ///        "minAttendees": 1,
        ///        "timeScale": 15
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the newly created calendar</response>
        /// <response code="400">If the request is null or parameters are missing</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the specified organization is not found</response>
        [HttpPost("/{organizationId}/calendar")]
        [SwaggerOperation(Summary = "Create a new calendar", Description = "Creates a new calendar associated with a specified organization.")]
        [SwaggerResponse(statusCode: 200, type: typeof(CalendarCreateResponseModel), description: "Successfully created the calendar")]
        [SwaggerResponse(statusCode: 400, description: "Bad request if the request body is null or missing required fields")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized if the user is not logged in")]
        [SwaggerResponse(statusCode: 404, description: "Not found if no organization matches the provided ID")]
        public async Task<ActionResult<CalendarCreateResponseModel>> CreateCalendarAsync(
            [FromRoute] string organizationId,
            [FromBody] CalendarCreateRequestModel request)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null");
            }

            // Retrieve the currently authenticated user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User must be logged in to create an organization");
            }

            // Make sure the organization exists
            var organization = _db.Organizations.Find(organizationId);
            if (organization == null)
            {
                return NotFound("Organization not found");
            }

            var calendar = new CalendarModel(Guid.NewGuid().ToString(), request.Name,
                organization.Id, request.TimeZone, request.IsPublic, user.Id, DateTime.UtcNow)
            {
                Description = request.Description,
                DefaultLocation = request.DefaultLocation,
                Color = request.Color,
                MaxAttendees = request.MaxAttendees,
                MinAttendees = request.MinAttendees,
                TimeScale = request.TimeScale
            };

            _db.Calendars.Add(calendar);
            await _db.SaveChangesAsync();

            var result = new CalendarCreateResponseModel(new CalendarViewModel()
            {
                Calendar = calendar
            });

            return Ok(result);
        }

        /// <summary>
        /// Searches calendars within an organization based on the provided criteria.
        /// </summary>
        /// <param name="organizationId">The ID of the organization whose calendars are being searched.</param>
        /// <param name="request">The search criteria including keyword, pagination, and sorting information.</param>
        /// <returns>Returns a list of calendars that match the search criteria along with pagination details.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /{organizationId}/calendar/search
        ///     {
        ///        "keyword": "Team",
        ///        "sort": "name",
        ///        "currentPage": 1,
        ///        "itemsPerPage": 10
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the list of calendars that match the search criteria</response>
        /// <response code="400">If the pagination parameters are invalid</response>
        [HttpPost("/{organizationId}/calendar/search")]
        [SwaggerOperation(Summary = "Search calendars", Description = "Searches for calendars within an organization based on the provided criteria.")]
        [SwaggerResponse(statusCode: 200, type: typeof(CalendarsSearchResponseModel), description: "Successful retrieval of the list of calendars")]
        [SwaggerResponse(statusCode: 400, description: "Bad request if the pagination parameters are incorrect")]
        public ActionResult<CalendarsSearchResponseModel> SearchCalendar(
            [FromRoute] string organizationId, [FromBody] CalendarsSearchRequestModel request)
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

            var calendars = from o in _db.Calendars where o.OrganizationId == organizationId select o;
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                calendars = calendars.Where(o => o.Name.Contains(request.Keyword));
            }

            // Count total items to support pagination
            int totalItems = calendars.Count();
            var items = calendars
                        .Skip(request.ItemsPerPage * (request.CurrentPage - 1))
                        .Take(request.ItemsPerPage)
                        .Select(r => new CalendarViewModel {
                            Calendar = r,
                            NumOfValidReservations = (from res in _db.Reservations
                                                      where res.CalendarId == r.Id &&
                                                      res.StartFrom > DateTime.Now &&
                                                      res.IsDeleted == false
                                                      select res).Count()
                        })
                        .ToList();

            // Prepare the response model with pagination info
            var result = new CalendarsSearchResponseModel(items)
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
        /// Retrieves a calendar by its ID within a specified organization.
        /// </summary>
        /// <param name="organizationId">The ID of the organization to which the calendar belongs.</param>
        /// <param name="id">The ID of the calendar to retrieve.</param>
        /// <returns>Returns the calendar details if found or a not found error if no calendar is found with the provided ID within the organization.</returns>
        /// <response code="200">Returns the calendar details if found</response>
        /// <response code="404">If no calendar is found with the specified ID within the organization</response>
        [HttpGet("/{organizationId}/calendar/{id}")]
        [SwaggerOperation(Summary = "Retrieve a calendar", Description = "Retrieves a calendar by its ID within a specified organization.")]
        [SwaggerResponse(statusCode: 200, type: typeof(CalendarGetResponseModel), description: "Successfully retrieved the calendar")]
        [SwaggerResponse(statusCode: 404, description: "Not found if no calendar exists with the specified ID within the organization")]
        public ActionResult<CalendarGetResponseModel> GetCalendar(
            [FromRoute] string organizationId,
            [FromRoute] string id)
        {
            // Query the database for the calendar with the specified ID and ensure it belongs to the specified organization
            var calendar = (from o in _db.Calendars
                            where o.Id == id && o.OrganizationId == organizationId
                            select new CalendarViewModel
                            {
                                Calendar = o,
                                NumOfValidReservations = (from res in _db.Reservations
                                                          where res.CalendarId == o.Id &&
                                                          res.StartFrom > DateTime.Now &&
                                                          res.IsDeleted == false
                                                          select res).Count(),
                                Reservations = (from res in _db.Reservations
                                                where res.CalendarId == o.Id
                                                orderby res.StartFrom
                                                select new ReservationViewModel()
                                                {
                                                    Reservation = res
                                                }).ToList()
                            }).FirstOrDefault();

            // Check if the calendar was found
            if (calendar == null)
            {
                return NotFound();
            }

            var result = new CalendarGetResponseModel(calendar);

            return Ok(result);
        }

        /// <summary>
        /// Updates a calendar within a specified organization.
        /// </summary>
        /// <param name="organizationId">The ID of the organization to which the calendar belongs.</param>
        /// <param name="id">The ID of the calendar to update.</param>
        /// <param name="request">The updated data for the calendar.</param>
        /// <returns>Returns the updated calendar details or appropriate error messages.</returns>
        /// <response code="200">If the calendar is successfully updated</response>
        /// <response code="404">If no calendar is found with the specified ID within the organization</response>
        /// <response code="401">If the calendar does not belong to the given organization or the user is not authorized</response>
        [HttpPut("/{organizationId}/calendar/{id}")]
        [SwaggerOperation(Summary = "Update a calendar", Description = "Updates a specific calendar within a specified organization based on the provided calendar ID.")]
        [SwaggerResponse(statusCode: 200, type: typeof(CalendarUpdateResponseModel), description: "Successfully updated the calendar")]
        [SwaggerResponse(statusCode: 404, description: "Not found if no calendar exists with the specified ID within the organization")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized if the calendar does not belong to the given organization")]
        public async Task<ActionResult<CalendarUpdateResponseModel>> UpdateCalendar(
            [FromRoute] string organizationId, [FromRoute] string id, [FromBody] CalendarUpdateRequestModel request)
        {
            // Attempt to find the existing calendar by ID
            var original = await _db.Calendars.FindAsync(id);

            // Check if the calendar exists and belongs to the correct organization
            if (original == null)
            {
                return NotFound("The calendar with the specified ID was not found.");
            }

            if (original.OrganizationId != organizationId)
            {
                return Unauthorized("The calendar does not belong to the specified organization.");
            }

            // Update the calendar's properties
            original.Name = request.Name;
            original.Description = request.Description; // Example of updating more fields
            original.Color = request.Color;
            original.DefaultLocation = request.DefaultLocation;
            original.IsPublic = request.IsPublic;
            original.MaxAttendees = request.MaxAttendees;
            original.MinAttendees = request.MinAttendees;
            original.TimeScale = request.TimeScale;
            original.TimeZone = request.TimeZone;

            // Save the updated calendar back to the database
            _db.Calendars.Update(original);
            await _db.SaveChangesAsync();

            // Prepare the response model with the updated calendar details
            var result = new CalendarUpdateResponseModel(original);

            return Ok(result);
        }
        
        /// <summary>
        /// Deletes a calendar within a specified organization.
        /// </summary>
        /// <param name="organizationId">The ID of the organization from which the calendar is to be deleted.</param>
        /// <param name="id">The ID of the calendar to delete.</param>
        /// <returns>Returns a confirmation of the deletion or appropriate error messages.</returns>
        /// <response code="200">If the calendar is successfully deleted</response>
        /// <response code="404">If no calendar is found with the specified ID within the organization</response>
        /// <response code="401">If the calendar does not belong to the given organization or the user is not authorized</response>
        [HttpDelete("/{organizationId}/calendar/{id}")]
        [SwaggerOperation(Summary = "Delete a calendar", Description = "Deletes a specific calendar within a specified organization based on the provided calendar ID.")]
        [SwaggerResponse(statusCode: 200, type: typeof(CalendarDeleteResponseModel), description: "Successfully deleted the calendar")]
        [SwaggerResponse(statusCode: 404, description: "Not found if no calendar exists with the specified ID within the organization")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized if the calendar does not belong to the given organization")]
        public async Task<ActionResult<CalendarDeleteResponseModel>> DeleteCalendarAsync(
            [FromRoute] string organizationId,
            [FromRoute] string id)
        {
            // Attempt to find the calendar by ID
            var original = await _db.Calendars.FindAsync(id);

            // Check if the calendar exists
            if (original == null)
            {
                return NotFound("The calendar with the specified ID was not found.");
            }

            // Check if the calendar belongs to the specified organization
            if (original.OrganizationId != organizationId)
            {
                return Unauthorized("The calendar does not belong to the specified organization.");
            }

            // Remove the calendar from the database
            _db.Calendars.Remove(original);
            await _db.SaveChangesAsync();

            // Prepare the response model with the deleted calendar details
            var result = new CalendarDeleteResponseModel(original);

            return Ok(result);
        }
    }
}
