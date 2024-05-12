using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Swashbuckle.AspNetCore.Annotations;
using wppReservations.Areas.Api1.Models;
using wppReservations.Data;
using wppReservations.Models;

namespace wppReservations.Areas.Api1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrganizationsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<UserModel> _userManager;
        private readonly ILogger<OrganizationsController> _logger;

        public OrganizationsController(ApplicationDbContext db,
            UserManager<UserModel> userManager,
            ILogger<OrganizationsController> logger)
        {
            _db = db;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new organization.
        /// </summary>
        /// <param name="request">The organization creation request containing the name of the organization.</param>
        /// <returns>Returns the newly created organization details.</returns>
        /// <response code="200">Returns the newly created organization</response>
        /// <response code="400">If the request is null or invalid</response>
        /// <response code="500">If there is an internal server error</response>
        [HttpPost("/organization")]
        [SwaggerOperation(
            Summary = "Create a new organization",
            Description = "Creates a new organization with the specified name. Requires user authentication.",
            OperationId = "CreateOrganization",
            Tags = new[] { "Organization" }
        )]
        [SwaggerResponse(statusCode: 200, type: typeof(OrganizationsCreateResponseModel), description: "Returns the newly created organization")]
        [SwaggerResponse(statusCode: 400, description: "If the input is null or invalid")]
        [SwaggerResponse(statusCode: 500, description: "If there is an internal server error")]
        public async Task<ActionResult<OrganizationsCreateResponseModel>> CreateOrganizationAsync([FromBody] OrganizationsCreateRequestModel request)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null");
            }


            foreach (var claim in User.Claims)
            {
                _logger.LogInformation($"{claim.Type}: {claim.Value}");
            }

            // Retrieve the currently authenticated user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User must be logged in to create an organization");
            }

            // Create a new organization object with a unique identifier
            var organization = new OrganizationModel(Guid.NewGuid().ToString(), request.Name, user.Id)
            {
                Created = DateTime.Now
            };

            // Add the new organization to the database
            _db.Organizations.Add(organization);
            
            // Add creater as a member (admin) of the organization
            _db.OrganizationMemberships.Add(new OrganizationMembershipModel(Guid.NewGuid().ToString(), 
                organization.Id, user.Id, "admin"));

            await _db.SaveChangesAsync();

            var members = (from m in _db.OrganizationMemberships
                          where m.OrganizationId == organization.Id
                          select m).ToList();

            // Prepare the response model with the created organization details
            var result = new OrganizationsCreateResponseModel(organization,members);

            return Ok(result);
        }


        /// <summary>
        /// Searches organizations based on the specified criteria.
        /// </summary>
        /// <param name="request">The search request containing keyword, sort criteria, and pagination details.</param>
        /// <returns>A list of organizations that match the search criteria along with pagination details.</returns>
        /// <response code="200">Returns a list of organizations that match the search criteria</response>
        /// <response code="400">If the pagination parameters are invalid</response>
        /// <response code="500">If there is an internal server error</response>
        [HttpPost("/organization/search")]
        [SwaggerOperation(
            Summary = "Search organizations",
            Description = "Searches for organizations based on keywords, sort criteria, and pagination settings. Requires user authentication.",
            OperationId = "SearchOrganization",
            Tags = new[] { "Organization" }
        )]
        [SwaggerResponse(statusCode: 200, type: typeof(OrganizationsSearchResponseModel), description: "Successful search results with pagination")]
        [SwaggerResponse(statusCode: 400, description: "Invalid pagination parameters")]
        [SwaggerResponse(statusCode: 500, description: "Internal server error")]
        public ActionResult<OrganizationsSearchResponseModel> SearchOrganization([FromBody] OrganizationsSearchRequestModel request)
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

            // Query organizations based on keyword search
            var organizations = from o in _db.Organizations select o;
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                organizations = organizations.Where(o => o.Name.Contains(request.Keyword));
            }

            // Count total items to support pagination
            int totalItems = organizations.Count();
            var items = organizations
                        .Skip(request.ItemsPerPage * (request.CurrentPage - 1))
                        .Take(request.ItemsPerPage)
                        .Select(r => new OrganizationViewModel { Organization = r })
                        .ToList();

            // Prepare the response model with pagination info
            var result = new OrganizationsSearchResponseModel(items)
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
        /// Retrieves an organization by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the organization to retrieve.</param>
        /// <returns>Returns the organization details if found; otherwise, returns a NotFound result.</returns>
        /// <response code="200">Returns the organization details if found</response>
        /// <response code="404">If no organization is found with the provided ID</response>
        [HttpGet("/organization/{id}")]
        [SwaggerOperation(
            Summary = "Retrieve an organization by ID",
            Description = "Retrieves the details of an organization based on the unique identifier provided. Requires user authentication.",
            OperationId = "GetOrganization",
            Tags = new[] { "Organization" }
        )]
        [SwaggerResponse(statusCode: 200, type: typeof(OrganizationGetResponseModel), description: "Successful retrieval of the organization")]
        [SwaggerResponse(statusCode: 404, description: "The organization with the specified ID was not found")]
        public ActionResult<OrganizationGetResponseModel> GetOrganization([FromRoute] string id)
        {
            // Query the database for the organization with the specified ID
            var organization = (from o in _db.Organizations
                                where o.Id == id
                                select new OrganizationViewModel
                                {
                                    Organization = o
                                }).FirstOrDefault();

            // Check if the organization was found
            if (organization == null)
            {
                return NotFound();
            }

            // Prepare the response model with the found organization details
            var result = new OrganizationGetResponseModel(organization);

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing organization.
        /// </summary>
        /// <param name="id">The unique identifier of the organization to update.</param>
        /// <param name="request">The request model containing updated fields for the organization.</param>
        /// <returns>Returns the updated organization details. If the organization is not found, returns NotFound.</returns>
        /// <response code="200">Returns the updated organization details</response>
        /// <response code="404">If no organization is found with the provided ID</response>
        /// <response code="400">If the request data is invalid</response>
        [HttpPut("/organization/{id}")]
        [SwaggerOperation(
            Summary = "Update an existing organization",
            Description = "Updates the specified fields of an existing organization. Requires user authentication and the organization ID in the route.",
            OperationId = "UpdateOrganization",
            Tags = new[] { "Organization" }
        )]
        [SwaggerResponse(statusCode: 200, type: typeof(OrganizationUpdateResponseModel), description: "Successful update of the organization")]
        [SwaggerResponse(statusCode: 404, description: "The organization with the specified ID was not found")]
        [SwaggerResponse(statusCode: 400, description: "Invalid data in request")]
        public async Task<ActionResult<OrganizationUpdateResponseModel>> UpdateOrganization(
            [FromRoute] string id,
            [FromBody] OrganizationUpdateRequestModel request)
        {
            // Attempt to find the existing organization by ID
            var original = await _db.Organizations.FindAsync(id);

            // Check if the organization exists
            if (original == null)
            {
                return NotFound();
            }

            // Update the organization's properties
            original.Name = request.Name;
            // Add more fields to update as necessary

            // Save the updated organization back to the database
            _db.Organizations.Update(original);
            await _db.SaveChangesAsync();

            // Prepare the response model with the updated organization details
            var result = new OrganizationUpdateResponseModel(original);

            return Ok(result);
        }

        /// <summary>
        /// Deletes an organization by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the organization to be deleted.</param>
        /// <returns>Returns a response indicating the result of the deletion process.</returns>
        /// <response code="200">Returns the details of the deleted organization</response>
        /// <response code="404">If no organization is found with the provided ID</response>
        [HttpDelete("/organization/{id}")]
        [SwaggerOperation(
            Summary = "Delete an organization",
            Description = "Deletes an existing organization based on the unique identifier provided. Requires user authentication.",
            OperationId = "DeleteOrganization",
            Tags = new[] { "Organization" }
        )]
        [SwaggerResponse(statusCode: 200, type: typeof(OrganizationDeleteResponseModel), description: "Successful deletion of the organization")]
        [SwaggerResponse(statusCode: 404, description: "The organization with the specified ID was not found")]
        public async Task<ActionResult<OrganizationDeleteResponseModel>> DeleteOrganizationAsync(
            [FromRoute] string id)
        {
            // Attempt to find the organization by ID
            var original = await _db.Organizations.FindAsync(id);

            // Check if the organization exists
            if (original == null)
            {
                return NotFound();
            }

            // Remove the organization from the database
            _db.Organizations.Remove(original);
            await _db.SaveChangesAsync();

            // Prepare the response model with the deleted organization details
            var result = new OrganizationDeleteResponseModel(original);

            return Ok(result);
        }
    }
}
