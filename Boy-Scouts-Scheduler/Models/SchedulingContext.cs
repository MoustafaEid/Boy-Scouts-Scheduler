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

        public void Seed()
        {
            //Seed Group Types to database
            GroupTypes.Add(new GroupType { Name = "Tiger" });
            GroupTypes.Add(new GroupType { Name = "Wolf" });
            GroupTypes.Add(new GroupType { Name = "Bear" });
            GroupTypes.Add(new GroupType { Name = "Webelos" });

            //Seed test event to database
            Events.Add(new Event
            {
                Name = "Dev Event",
                Start = new DateTime(2012, 3, 5),
                End = new DateTime(2012, 3, 9)
            });
            SaveChanges();

            //See test TimeSlots to database
            Event primaryEvent = Events.First();
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Monday: 9:00AM-10:00AM",
                Start = new DateTime(2012, 3, 5, 9, 0, 0),
                End = new DateTime(2012, 3, 5, 10, 00, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Monday: 10:15AM-11:15PM",
                Start = new DateTime(2012, 3, 5, 10, 15, 0),
                End = new DateTime(2012, 3, 5, 11, 15, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Monday: 11:30PM-12:30PM",
                Start = new DateTime(2012, 3, 5, 11, 30, 0),
                End = new DateTime(2012, 3, 5, 12, 30, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Monday: 1:30PM-2:30PM",
                Start = new DateTime(2012, 3, 5, 13, 30, 0),
                End = new DateTime(2012, 3, 5, 14, 30, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Monday: 2:45PM-3:45PM",
                Start = new DateTime(2012, 3, 5, 14, 45, 0),
                End = new DateTime(2012, 3, 5, 15, 45, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Tuesday: 9:00AM-10:00AM",
                Start = new DateTime(2012, 3, 6, 9, 0, 0),
                End = new DateTime(2012, 3, 6, 10, 00, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Tuesday: 10:15AM-11:15PM",
                Start = new DateTime(2012, 3, 6, 10, 15, 0),
                End = new DateTime(2012, 3, 6, 11, 15, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Tuesday: 11:30PM-12:30PM",
                Start = new DateTime(2012, 3, 6, 11, 30, 0),
                End = new DateTime(2012, 3, 6, 12, 30, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Tuesday: 1:30PM-2:30PM",
                Start = new DateTime(2012, 3, 6, 13, 30, 0),
                End = new DateTime(2012, 3, 6, 14, 30, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Tuesday: 2:45PM-3:45PM",
                Start = new DateTime(2012, 3, 6, 14, 45, 0),
                End = new DateTime(2012, 3, 6, 15, 45, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Wednesday: 9:00AM-10:00AM",
                Start = new DateTime(2012, 3, 7, 9, 0, 0),
                End = new DateTime(2012, 3, 7, 10, 00, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Wednesday: 10:15AM-11:15PM",
                Start = new DateTime(2012, 3, 7, 10, 15, 0),
                End = new DateTime(2012, 3, 7, 11, 15, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Wednesday: 11:30PM-12:30PM",
                Start = new DateTime(2012, 3, 7, 11, 30, 0),
                End = new DateTime(2012, 3, 7, 12, 30, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Wednesday: 1:30PM-2:30PM",
                Start = new DateTime(2012, 3, 7, 13, 30, 0),
                End = new DateTime(2012, 3, 7, 14, 30, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Wednesday: 2:45PM-3:45PM",
                Start = new DateTime(2012, 3, 7, 14, 45, 0),
                End = new DateTime(2012, 3, 7, 15, 45, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Thursday: 9:00AM-10:00AM",
                Start = new DateTime(2012, 3, 8, 9, 0, 0),
                End = new DateTime(2012, 3, 8, 10, 00, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Thursday: 10:15AM-11:15PM",
                Start = new DateTime(2012, 3, 8, 10, 15, 0),
                End = new DateTime(2012, 3, 8, 11, 15, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Thursday: 11:30PM-12:30PM",
                Start = new DateTime(2012, 3, 8, 11, 30, 0),
                End = new DateTime(2012, 3, 8, 12, 30, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Thursday: 1:30PM-2:30PM",
                Start = new DateTime(2012, 3, 8, 13, 30, 0),
                End = new DateTime(2012, 3, 8, 14, 30, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Thursday: 2:45PM-3:45PM",
                Start = new DateTime(2012, 3, 8, 14, 45, 0),
                End = new DateTime(2012, 3, 8, 15, 45, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Friday: 9:00AM-10:00AM",
                Start = new DateTime(2012, 3, 9, 9, 0, 0),
                End = new DateTime(2012, 3, 9, 10, 00, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Friday: 10:15AM-11:15PM",
                Start = new DateTime(2012, 3, 9, 10, 15, 0),
                End = new DateTime(2012, 3, 9, 11, 15, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Friday: 11:30PM-12:30PM",
                Start = new DateTime(2012, 3, 9, 11, 30, 0),
                End = new DateTime(2012, 3, 9, 12, 30, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Friday: 1:30PM-2:30PM",
                Start = new DateTime(2012, 3, 9, 13, 30, 0),
                End = new DateTime(2012, 3, 9, 14, 30, 0)
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Friday: 2:45PM-3:45PM",
                Start = new DateTime(2012, 3, 9, 14, 45, 0),
                End = new DateTime(2012, 3, 9, 15, 45, 0)
            });

            SaveChanges();
            //create ICollection for time slots for each station
            ICollection<TimeSlot> timeslots = TimeSlots.ToList();

            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Monday Lunch",
                Start = new DateTime(2012, 3, 5, 12, 30, 0),
                End = new DateTime(2012, 3, 5, 13, 30, 0),
                isGeneral = true
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Tuesday Lunch",
                Start = new DateTime(2012, 3, 6, 12, 30, 0),
                End = new DateTime(2012, 3, 6, 13, 30, 0),
                isGeneral = true
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Wednesday Lunch",
                Start = new DateTime(2012, 3, 7, 12, 30, 0),
                End = new DateTime(2012, 3, 7, 13, 30, 0),
                isGeneral = true
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Thursday Lunch",
                Start = new DateTime(2012, 3, 8, 12, 30, 0),
                End = new DateTime(2012, 3, 8, 13, 30, 0),
                isGeneral = true
            });
            TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Friday Lunch",
                Start = new DateTime(2012, 3, 9, 12, 30, 0),
                End = new DateTime(2012, 3, 9, 13, 30, 0),
                isGeneral = true
            });
            SaveChanges();

            //Seed testing stations to database
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Fishing",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Knots",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Songs & Skits",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Wood Projects",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Outdoor Cooking",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Arts & Crafts",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Creeking",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Chess",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Knight Training",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Volleyball",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Flag Football",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Trading Post",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Webelos Craftsman Activity Pin",
                Description = "",
                Location = "",
                Capacity = 1,
                isActivityPin = true,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Webelos Forester Activity Pin",
                Description = "",
                Location = "",
                Capacity = 1,
                isActivityPin = true,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Webelos Naturalist Activity Pin",
                Description = "",
                Location = "",
                Capacity = 1,
                isActivityPin = true,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Webelos Artist Activity Pin",
                Description = "",
                Location = "",
                Capacity = 1,
                isActivityPin = true,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Webelos Aquanaut Activity Pin",
                Description = "",
                Location = "",
                Capacity = 1,
                isActivityPin = true,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "BB",
                Description = "",
                Location = "",
                Capacity = 1,
                Category = "Shooting",
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Archery",
                Description = "",
                Location = "",
                Capacity = 1,
                Category = "Shooting",
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Swimming",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Kickball",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Leaf Rubbing",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            SaveChanges();

            ////Seed groups
            Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Tegyr",
                Type = GroupTypes.Where(t => t.ID == 1).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Galahad",
                Type = GroupTypes.Where(t => t.ID == 2).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Gawain",
                Type = GroupTypes.Where(t => t.ID == 2).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Gareth",
                Type = GroupTypes.Where(t => t.ID == 2).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Bors de Ganis",
                Type = GroupTypes.Where(t => t.ID == 2).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Kay",
                Type = GroupTypes.Where(t => t.ID == 3).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Gaheris",
                Type = GroupTypes.Where(t => t.ID == 3).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Percivale",
                Type = GroupTypes.Where(t => t.ID == 3).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Lionel",
                Type = GroupTypes.Where(t => t.ID == 3).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Tristan",
                Type = GroupTypes.Where(t => t.ID == 4).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Bedivere",
                Type = GroupTypes.Where(t => t.ID == 4).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Dagonet",
                Type = GroupTypes.Where(t => t.ID == 4).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Geriant",
                Type = GroupTypes.Where(t => t.ID == 4).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Lamorak",
                Type = GroupTypes.Where(t => t.ID == 4).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Lancelot",
                Type = GroupTypes.Where(t => t.ID == 4).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            SaveChanges();

            // Seed Constraints
            SchedulingConstraints.Add(new SchedulingConstraint
            {
                Event = primaryEvent,
                GroupType = GroupTypes.Where(t => t.ID == 1).Single(),
                Group = null,
                Station = Stations.Where(s => s.ID == 1).Single(),
            });
            SchedulingConstraints.Add(new SchedulingConstraint
            {
                Event = primaryEvent,
                GroupType = null,
                Group = Groups.Where(g => g.ID == 2).Single(),
                Station = Stations.Where(s => s.ID == 2).Single(),
            });
            SaveChanges();
        }
        
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