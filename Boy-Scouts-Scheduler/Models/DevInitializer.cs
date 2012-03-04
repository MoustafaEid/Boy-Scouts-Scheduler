using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Boy_Scouts_Scheduler.Models
{
    public class DevInitializer : DropCreateDatabaseIfModelChanges<SchedulingContext>
    {
        protected override void Seed(SchedulingContext context)
        {
            //Seed Group Types to database
            context.GroupTypes.Add(new GroupType { Name = "Tiger" });
            context.GroupTypes.Add(new GroupType { Name = "Wolf" });
            context.GroupTypes.Add(new GroupType { Name = "Bear" });
            context.GroupTypes.Add(new GroupType { Name = "Webelos" });

            //Seed test event to database
            context.Events.Add(new Event
            {
                Name = "Dev Event",
                Start = new DateTime(2012, 3, 5),
                End = new DateTime(2012, 3, 9)
            });
            context.SaveChanges();

            //See test TimeSlots to database
            Event primaryEvent = context.Events.First();
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Monday: 9:00AM-10:00AM",
                Start = new DateTime(2012, 3, 5, 9, 0, 0),
                End = new DateTime(2012, 3, 5, 10, 00, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Monday: 10:15AM-11:15PM",
                Start = new DateTime(2012, 3, 5, 10, 15, 0),
                End = new DateTime(2012, 3, 5, 11, 15, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Monday: 11:30PM-12:30PM",
                Start = new DateTime(2012, 3, 5, 11, 30, 0),
                End = new DateTime(2012, 3, 5, 12, 30, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Monday: 1:30PM-2:30PM",
                Start = new DateTime(2012, 3, 5, 13, 30, 0),
                End = new DateTime(2012, 3, 5, 14, 30, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Monday: 2:45PM-3:45PM",
                Start = new DateTime(2012, 3, 5, 14, 45, 0),
                End = new DateTime(2012, 3, 5, 15, 45, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Tuesday: 9:00AM-10:00AM",
                Start = new DateTime(2012, 3, 6, 9, 0, 0),
                End = new DateTime(2012, 3, 6, 10, 00, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Tuesday: 10:15AM-11:15PM",
                Start = new DateTime(2012, 3, 6, 10, 15, 0),
                End = new DateTime(2012, 3, 6, 11, 15, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Tuesday: 11:30PM-12:30PM",
                Start = new DateTime(2012, 3, 6, 11, 30, 0),
                End = new DateTime(2012, 3, 6, 12, 30, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Tuesday: 1:30PM-2:30PM",
                Start = new DateTime(2012, 3, 6, 13, 30, 0),
                End = new DateTime(2012, 3, 6, 14, 30, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Tuesday: 2:45PM-3:45PM",
                Start = new DateTime(2012, 3, 6, 14, 45, 0),
                End = new DateTime(2012, 3, 6, 15, 45, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Wednesday: 9:00AM-10:00AM",
                Start = new DateTime(2012, 3, 7, 9, 0, 0),
                End = new DateTime(2012, 3, 7, 10, 00, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Wednesday: 10:15AM-11:15PM",
                Start = new DateTime(2012, 3, 7, 10, 15, 0),
                End = new DateTime(2012, 3, 7, 11, 15, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Wednesday: 11:30PM-12:30PM",
                Start = new DateTime(2012, 3, 7, 11, 30, 0),
                End = new DateTime(2012, 3, 7, 12, 30, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Wednesday: 1:30PM-2:30PM",
                Start = new DateTime(2012, 3, 7, 13, 30, 0),
                End = new DateTime(2012, 3, 7, 14, 30, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Wednesday: 2:45PM-3:45PM",
                Start = new DateTime(2012, 3, 7, 14, 45, 0),
                End = new DateTime(2012, 3, 7, 15, 45, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Thursday: 9:00AM-10:00AM",
                Start = new DateTime(2012, 3, 8, 9, 0, 0),
                End = new DateTime(2012, 3, 8, 10, 00, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Thursday: 10:15AM-11:15PM",
                Start = new DateTime(2012, 3, 8, 10, 15, 0),
                End = new DateTime(2012, 3, 8, 11, 15, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Thursday: 11:30PM-12:30PM",
                Start = new DateTime(2012, 3, 8, 11, 30, 0),
                End = new DateTime(2012, 3, 8, 12, 30, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Thursday: 1:30PM-2:30PM",
                Start = new DateTime(2012, 3, 8, 13, 30, 0),
                End = new DateTime(2012, 3, 8, 14, 30, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Thursday: 2:45PM-3:45PM",
                Start = new DateTime(2012, 3, 8, 14, 45, 0),
                End = new DateTime(2012, 3, 8, 15, 45, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Friday: 9:00AM-10:00AM",
                Start = new DateTime(2012, 3, 9, 9, 0, 0),
                End = new DateTime(2012, 3, 9, 10, 00, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Friday: 10:15AM-11:15PM",
                Start = new DateTime(2012, 3, 9, 10, 15, 0),
                End = new DateTime(2012, 3, 9, 11, 15, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Friday: 11:30PM-12:30PM",
                Start = new DateTime(2012, 3, 9, 11, 30, 0),
                End = new DateTime(2012, 3, 9, 12, 30, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Friday: 1:30PM-2:30PM",
                Start = new DateTime(2012, 3, 9, 13, 30, 0),
                End = new DateTime(2012, 3, 9, 14, 30, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Friday: 2:45PM-3:45PM",
                Start = new DateTime(2012, 3, 9, 14, 45, 0),
                End = new DateTime(2012, 3, 9, 15, 45, 0)
            });

			context.SaveChanges();
			//create ICollection for time slots for each station
			ICollection<TimeSlot> timeslots = context.TimeSlots.ToList();

            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Monday Lunch",
                Start = new DateTime(2012, 3, 5, 12, 30, 0),
                End = new DateTime(2012, 3, 5, 13, 30, 0),
                isGeneral = true
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Tuesday Lunch",
                Start = new DateTime(2012, 3, 6, 12, 30, 0),
                End = new DateTime(2012, 3, 6, 13, 30, 0),
                isGeneral = true
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Wednesday Lunch",
                Start = new DateTime(2012, 3, 7, 12, 30, 0),
                End = new DateTime(2012, 3, 7, 13, 30, 0),
                isGeneral = true
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Thursday Lunch",
                Start = new DateTime(2012, 3, 8, 12, 30, 0),
                End = new DateTime(2012, 3, 8, 13, 30, 0),
                isGeneral = true
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Friday Lunch",
                Start = new DateTime(2012, 3, 9, 12, 30, 0),
                End = new DateTime(2012, 3, 9, 13, 30, 0),
                isGeneral = true
            });
            context.SaveChanges();

            //Seed testing stations to database
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Fishing",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Knots",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Songs & Skits",
                Description = "",
                Location = "",
                Capacity = 1, 
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Wood Projects",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Outdoor Cooking",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Arts & Crafts",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Creeking",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Chess",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Knight Training",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Volleyball",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Flag Football",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Trading Post",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Webelos Craftsman Activity Pin",
                Description = "",
                Location = "",
                Capacity = 1,
                isActivityPin = true,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Webelos Forester Activity Pin",
                Description = "",
                Location = "",
                Capacity = 1,
                isActivityPin = true,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Webelos Naturalist Activity Pin",
                Description = "",
                Location = "",
                Capacity = 1,
                isActivityPin = true,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Webelos Artist Activity Pin",
                Description = "",
                Location = "",
                Capacity = 1,
                isActivityPin = true,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Webelos Aquanaut Activity Pin",
                Description = "",
                Location = "",
                Capacity = 1,
                isActivityPin = true,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "BB",
                Description = "",
                Location = "",
                Capacity = 1,
                Category = "Shooting",
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Archery",
                Description = "",
                Location = "",
                Capacity = 1,
                Category = "Shooting",
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Swimming",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Kickball",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Leaf Rubbing",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.SaveChanges();

            ////Seed groups
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Tegyr",
                Type = context.GroupTypes.Where(t => t.ID == 1).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Galahad",
                Type = context.GroupTypes.Where(t => t.ID == 2).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Gawain",
                Type = context.GroupTypes.Where(t => t.ID == 2).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Gareth",
                Type = context.GroupTypes.Where(t => t.ID == 2).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Bors de Ganis",
                Type = context.GroupTypes.Where(t => t.ID == 2).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Kay",
                Type = context.GroupTypes.Where(t => t.ID == 3).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Gaheris",
                Type = context.GroupTypes.Where(t => t.ID == 3).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Percivale",
                Type = context.GroupTypes.Where(t => t.ID == 3).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Lionel",
                Type = context.GroupTypes.Where(t => t.ID == 3).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Tristan",
                Type = context.GroupTypes.Where(t => t.ID == 4).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Bedivere",
                Type = context.GroupTypes.Where(t => t.ID == 4).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Dagonet",
                Type = context.GroupTypes.Where(t => t.ID == 4).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Geriant",
                Type = context.GroupTypes.Where(t => t.ID == 4).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Lamorak",
                Type = context.GroupTypes.Where(t => t.ID == 4).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Lancelot",
                Type = context.GroupTypes.Where(t => t.ID == 4).Single(),
                Preference1 = null,
                Preference2 = null,
                Preference3 = null
            });
            context.SaveChanges();

            // Seed Constraints
            context.SchedulingConstraints.Add(new SchedulingConstraint
            {
                GroupType = context.GroupTypes.Where(t => t.ID == 1).Single(),
                Group = context.Groups.Where(g => g.ID == 1).Single(),
                Station = context.Stations.Where(s => s.ID == 1).Single(),
            });
            context.SchedulingConstraints.Add(new SchedulingConstraint
            {
                GroupType = context.GroupTypes.Where(t => t.ID == 2).Single(),
                Group = context.Groups.Where(g => g.ID == 2).Single(),
                Station = context.Stations.Where(s => s.ID == 2).Single(),
            });
            context.SaveChanges();
        }
    }
}