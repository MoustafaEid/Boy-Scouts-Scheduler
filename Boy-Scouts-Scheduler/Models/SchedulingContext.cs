using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Boy_Scouts_Scheduler.Models
{
    public class SchedulingContext : DbContext
    {
        public SchedulingContext() : base("SchedulingConnection") { }

        public DbSet<Site> Sites { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<GroupType> GroupTypes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Constraint> Constraints { get; set; }
    }
}