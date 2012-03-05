using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Boy_Scouts_Scheduler.Models
{
    public class SchedulingContext : DbContext
    {
        public SchedulingContext() : base("DefaultConnection") { }

        public DbSet<Site> Sites { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<GroupType> GroupTypes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<SchedulingConstraint> SchedulingConstraints { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Station>().HasMany<TimeSlot>(s => s.AvailableTimeSlots).WithMany(t => t.OpenStations).Map(m =>
            {
                m.ToTable("StationTimeSlots");
                m.MapLeftKey("StationID");
                m.MapRightKey("TimeSlotID");
            });
        }
    }
}