using wppReservations.Models;
namespace wppReservations.Areas.Api1.Models
{

    /// <summary>
    /// Represents a request model for creating an organization.
    /// This model captures all necessary information needed to create a new organization.
    /// </summary>
    public class OrganizationsCreateRequestModel
    {
        /// <summary>
        /// Initializes a new instance of the OrganizationsCreateRequestModel with specified details.
        /// </summary>
        /// <param name="name">The name of the organization to be created.</param>
        /// <param name="createdBy">The identifier of the user creating the organization.</param>
        public OrganizationsCreateRequestModel(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the name of the organization.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Represents the response model returned after creating an organization.
    /// This model encapsulates the details of the newly created organization.
    /// </summary>
    public class OrganizationsCreateResponseModel
    {
        /// <summary>
        /// Initializes a new instance of the OrganizationsCreateResponseModel with the created organization.
        /// </summary>
        /// <param name="organization">The organization that has been successfully created.</param>
        public OrganizationsCreateResponseModel(OrganizationModel organization, List<OrganizationMembershipModel> organizationMembers)
        {
            Organization = organization;
            OrganizationMembers = organizationMembers;
        }

        /// <summary>
        /// Gets or sets the details of the created organization.
        /// </summary>
        public OrganizationModel Organization { get; set; }
        public List<OrganizationMembershipModel> OrganizationMembers { get; set; }
    }

    /// <summary>
    /// Represents a view model for an organization.
    /// This model is typically used to format or present data in a specific way for views or API responses.
    /// </summary>
    public class OrganizationViewModel
    {
        /// <summary>
        /// Gets or sets the organization details. This property can be null if no organization data is available.
        /// </summary>
        public OrganizationModel? Organization { get; set; }
    }

    /// <summary>
    /// Represents the search criteria for querying organizations.
    /// This model captures the parameters used to filter and page the search results.
    /// </summary>
    public class OrganizationsSearchRequestModel
    {
        /// <summary>
        /// Gets or sets the keyword used for searching organizations by name or other attributes.
        /// This can be null, in which case the filter should not apply.
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// Gets or sets the sorting criteria (e.g., 'Name ASC', 'Name DESC').
        /// This can be null, in which case a default sort should be applied.
        /// </summary>
        public string? Sort { get; set; }

        /// <summary>
        /// Gets or sets the current page number in pagination.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the number of items per page in pagination.
        /// </summary>
        public int ItemsPerPage { get; set; }
    }

    /// <summary>
    /// Represents the response model returned from a search query for organizations.
    /// This model includes the list of organizations that match the search criteria along with additional pagination details.
    /// </summary>
    public class OrganizationsSearchResponseModel
    {
        /// <summary>
        /// Initializes a new instance of the OrganizationsSearchResponseModel with the list of organizations.
        /// </summary>
        /// <param name="organizations">The list of organizations that match the search criteria.</param>
        public OrganizationsSearchResponseModel(List<OrganizationViewModel> organizations)
        {
            Organizations = organizations;
        }

        /// <summary>
        /// Gets or sets the keyword used in the search query.
        /// This can be null if no keyword was used.
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// Gets or sets the sorting criteria used in the search query.
        /// This can be null if no specific sorting was applied.
        /// </summary>
        public string? Sort { get; set; }

        /// <summary>
        /// Gets or sets the current page number in the search results pagination.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the number of items per page in the search results pagination.
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the total number of items that match the search criteria.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages available based on the current pagination settings.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the list of organization view models that match the search criteria.
        /// </summary>
        public List<OrganizationViewModel> Organizations { get; set; }
    }

    /// <summary>
    /// Represents the response model returned when retrieving an organization by its ID.
    /// This model encapsulates the details of the retrieved organization.
    /// </summary>
    public class OrganizationGetResponseModel
    {
        /// <summary>
        /// Initializes a new instance of the OrganizationsGetResponseModel with the retrieved organization.
        /// </summary>
        /// <param name="organization">The organization that has been successfully retrieved.</param>
        public OrganizationGetResponseModel(OrganizationViewModel organization)
        {
            Organization = organization;
        }

        /// <summary>
        /// Gets or sets the details of the retrieved organization.
        /// </summary>
        public OrganizationViewModel Organization { get; set; }
    }

    /// <summary>
    /// Represents the request model for updating an organization.
    /// This model captures the necessary information needed to update an existing organization.
    /// </summary>
    public class OrganizationUpdateRequestModel
    {
        /// <summary>
        /// Initializes a new instance of the OrganizationsUpdateRequestModel with the specified name.
        /// </summary>
        /// <param name="name">The new name of the organization.</param>
        public OrganizationUpdateRequestModel(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the name of the organization to be updated.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Represents the response model returned after updating an organization.
    /// This model encapsulates the details of the updated organization.
    /// </summary>
    public class OrganizationUpdateResponseModel
    {
        /// <summary>
        /// Initializes a new instance of the OrganizationsUpdateResponseModel with the updated organization.
        /// </summary>
        /// <param name="organization">The organization that has been successfully updated.</param>
        public OrganizationUpdateResponseModel(OrganizationModel organization)
        {
            Organization = organization;
        }

        /// <summary>
        /// Gets or sets the details of the updated organization.
        /// </summary>
        public OrganizationModel Organization { get; set; }
    }

    /// <summary>
    /// Represents the response model returned after deleting an organization.
    /// This model encapsulates the details of the organization that has been deleted.
    /// </summary>
    public class OrganizationDeleteResponseModel
    {
        /// <summary>
        /// Initializes a new instance of the OrganizationsDeleteResponseModel with the organization that has been deleted.
        /// </summary>
        /// <param name="deletedOrganization">The organization that has been successfully deleted.</param>
        public OrganizationDeleteResponseModel(OrganizationModel deletedOrganization)
        {
            DeletedOrganization = deletedOrganization;
        }

        /// <summary>
        /// Gets or sets the details of the deleted organization.
        /// </summary>
        public OrganizationModel DeletedOrganization { get; set; }
    }
}
