using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;

namespace wppReservations.Models
{
    public class OrganizationModels
    {
    }
    
    // Represents a user with an optional name, extending the IdentityUser class with additional properties.
    public class UserModel : IdentityUser
    {
        public string? Name { get; set; }  // Optional name of the user.
    }

    // Defines the membership details for an organization including roles and user associations.
    [Table("OrganizationMemberships")]
    public class OrganizationMembershipModel
    {
        // Constructor to initialize an OrganizationMembership instance.
        public OrganizationMembershipModel(string id, string organizationId, string userId, string roleId)
        {
            Id = id;
            OrganizationId = organizationId;
            UserId = userId;
            RoleId = roleId;
        }

        [Key, MaxLength(36)]
        public string Id { get; set; }  // Unique identifier for the membership.
        public string OrganizationId { get; set; }  // Associated organization identifier.
        public string UserId { get; set; }  // Associated user identifier.
        public string RoleId { get; set; }  // Role identifier within the organization.
    }

    // Represents a role within an organization.
    [Table("OrganizationRoles")]
    public class OrganizationRoleModel
    {
        // Constructor to initialize an OrganizationRole instance.
        public OrganizationRoleModel(string id, string name)
        {
            Id = id;
            Name = name;
        }

        [Key, MaxLength(36)]
        public string Id { get; set; }  // Unique identifier for the role.
        public string Name { get; set; }  // Name of the role.
    }

    // Represents an organization with its attributes and state.
    [Table("Organizations")]
    public class OrganizationModel
    {
        // Constructor to initialize an Organization instance.
        public OrganizationModel(string id, string name, string createdBy)
        {
            Id = id;
            Name = name;
            CreatedBy = createdBy;
            Created = DateTime.Now;
        }

        [Key, MaxLength(36)]
        public string Id { get; set; }  // Unique identifier for the organization.
        public string Name { get; set; }  // Name of the organization.
        public DateTime Created { get; set; }  // Date and time the organization was created.
        [MaxLength(36)]
        public string CreatedBy { get; set; }  // User identifier of the creator.
        public bool IsSuspended { get; set; }  // Indicates if the organization is currently suspended.
        public DateTime Suspended { get; set; }  // Date and time the organization was suspended.
        public bool IsDeleted { get; set; }  // Indicates if the organization is marked as deleted.
        public DateTime Deleted { get; set; }  // Date and time the organization was deleted.
    }
}
