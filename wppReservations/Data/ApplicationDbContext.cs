using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using wppReservations.Models;

namespace wppReservations.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserModel>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
        {
        }

        public DbSet<OrganizationModel> Organizations { get; set; }
        public DbSet<OrganizationRoleModel> OrganizationRoles { get; set; }
        public DbSet<OrganizationMembershipModel> OrganizationMemberships { get; set; }
        public DbSet<CalendarModel> Calendars { get; set; }
        public DbSet<CalendarGroupModel> CalendarGroups { get; set; }
        public DbSet<CalendarGroupItemModel> CalendarGroupItems { get; set; }
        public DbSet<ReservationModel> Reservations { get; set; }
    }
}
