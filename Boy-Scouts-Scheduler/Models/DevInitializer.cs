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
                Name = "Monday: 10:30AM-12:00PM",
                Start = new DateTime(2012, 3, 5, 10, 30, 0),
                End = new DateTime(2012, 3, 5, 12, 0, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Monday: 1:00PM-2:00PM",
                Start = new DateTime(2012, 3, 5, 13, 0, 0),
                End = new DateTime(2012, 3, 5, 14, 0, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Monday: 2:30PM-3:30PM",
                Start = new DateTime(2012, 3, 5, 14, 30, 0),
                End = new DateTime(2012, 3, 5, 15, 30, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Monday: 4:00PM-5:00PM",
                Start = new DateTime(2012, 3, 5, 16, 0, 0),
                End = new DateTime(2012, 3, 5, 17, 0, 0)
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
                Name = "Tuesday: 10:30AM-12:00PM",
                Start = new DateTime(2012, 3, 6, 10, 30, 0),
                End = new DateTime(2012, 3, 6, 12, 0, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Tuesday: 1:00PM-2:00PM",
                Start = new DateTime(2012, 3, 6, 13, 0, 0),
                End = new DateTime(2012, 3, 6, 14, 0, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Tuesday: 2:30PM-3:30PM",
                Start = new DateTime(2012, 3, 6, 14, 30, 0),
                End = new DateTime(2012, 3, 6, 15, 30, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Tuesday: 4:00PM-5:00PM",
                Start = new DateTime(2012, 3, 6, 16, 0, 0),
                End = new DateTime(2012, 3, 6, 17, 0, 0)
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
                Name = "Wednesday: 10:30AM-12:00PM",
                Start = new DateTime(2012, 3, 7, 10, 30, 0),
                End = new DateTime(2012, 3, 7, 12, 0, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Wednesday: 1:00PM-2:00PM",
                Start = new DateTime(2012, 3, 7, 13, 0, 0),
                End = new DateTime(2012, 3, 7, 14, 0, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Wednesday: 2:30PM-3:30PM",
                Start = new DateTime(2012, 3, 7, 14, 30, 0),
                End = new DateTime(2012, 3, 7, 15, 30, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Wednesday: 4:00PM-5:00PM",
                Start = new DateTime(2012, 3, 7, 16, 0, 0),
                End = new DateTime(2012, 3, 7, 17, 0, 0)
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
                Name = "Thursday: 10:30AM-12:00PM",
                Start = new DateTime(2012, 3, 8, 10, 30, 0),
                End = new DateTime(2012, 3, 8, 12, 0, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Thursday: 1:00PM-2:00PM",
                Start = new DateTime(2012, 3, 8, 13, 0, 0),
                End = new DateTime(2012, 3, 8, 14, 0, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Thursday: 2:30PM-3:30PM",
                Start = new DateTime(2012, 3, 8, 14, 30, 0),
                End = new DateTime(2012, 3, 8, 15, 30, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Thursday: 4:00PM-5:00PM",
                Start = new DateTime(2012, 3, 8, 16, 0, 0),
                End = new DateTime(2012, 3, 8, 17, 0, 0)
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
                Name = "Friday: 10:30AM-12:00PM",
                Start = new DateTime(2012, 3, 9, 10, 30, 0),
                End = new DateTime(2012, 3, 9, 12, 0, 0)
            });
            context.TimeSlots.Add(new TimeSlot
            {
                Event = primaryEvent,
                Name = "Friday: 1:00PM-2:00PM",
                Start = new DateTime(2012, 3, 9, 13, 0, 0),
                End = new DateTime(2012, 3, 9, 14, 0, 0)
            });
            context.SaveChanges();

            //create ICollection for time slots for each station
            ICollection<TimeSlot> timeslots = context.TimeSlots.ToList();

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
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Webelos Forester Activity Pin",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Webelos Naturalist Activity Pin",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Webelos Artist Activity Pin",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Webelos Aquanaut Activity Pin",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "BB",
                Description = "",
                Location = "",
                Capacity = 1,
                AvailableTimeSlots = timeslots
            });
            context.Stations.Add(new Station
            {
                Event = primaryEvent,
                Name = "Archery",
                Description = "",
                Location = "",
                Capacity = 1,
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
                Type = context.GroupTypes.First(),
                Preference1 = context.Stations.First(),
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Galahad",
                Type = context.GroupTypes.First(),
                Preference1 = context.Stations.First(),
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Gawain",
                Type = context.GroupTypes.First(),
                Preference1 = context.Stations.First(),
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Gareth",
                Type = context.GroupTypes.First(),
                Preference1 = context.Stations.First(),
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Bors de Ganis",
                Type = context.GroupTypes.First(),
                Preference1 = context.Stations.First(),
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Kay",
                Type = context.GroupTypes.First(),
                Preference1 = context.Stations.First(),
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Gaheris",
                Type = context.GroupTypes.First(),
                Preference1 = context.Stations.First(),
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Percivale",
                Type = context.GroupTypes.First(),
                Preference1 = context.Stations.First(),
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Lionel",
                Type = context.GroupTypes.First(),
                Preference1 = context.Stations.First(),
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Tristan",
                Type = context.GroupTypes.First(),
                Preference1 = context.Stations.First(),
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Bedivere",
                Type = context.GroupTypes.First(),
                Preference1 = context.Stations.First(),
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Dagonet",
                Type = context.GroupTypes.First(),
                Preference1 = context.Stations.First(),
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Geriant",
                Type = context.GroupTypes.First(),
                Preference1 = context.Stations.First(),
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Lamorak",
                Type = context.GroupTypes.First(),
                Preference1 = context.Stations.First(),
                Preference2 = null,
                Preference3 = null
            });
            context.Groups.Add(new Group
            {
                Event = primaryEvent,
                Name = "Sir Lancelot",
                Type = context.GroupTypes.First(),
                Preference1 = context.Stations.First(),
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
                MinVisits = 1,
                MaxVisits = 1
            });
            context.SchedulingConstraints.Add(new SchedulingConstraint
            {
                GroupType = context.GroupTypes.Where(t => t.ID == 2).Single(),
                Group = context.Groups.Where(g => g.ID == 2).Single(),
                Station = context.Stations.Where(s => s.ID == 2).Single(),
                MinVisits = 1,
                MaxVisits = 1
            });
            context.SchedulingConstraints.Add(new SchedulingConstraint
            {
                GroupType = context.GroupTypes.Where(t => t.ID == 3).Single(),
                Group = context.Groups.Where(g => g.ID == 3).Single(),
                Station = context.Stations.Where(s => s.ID == 3).Single(),
                MinVisits = 1,
                MaxVisits = 1
            });
            context.SchedulingConstraints.Add(new SchedulingConstraint
            {
                GroupType = context.GroupTypes.Where(t => t.ID == 4).Single(),
                Group = context.Groups.Where(g => g.ID == 4).Single(),
                Station = context.Stations.Where(s => s.ID == 4).Single(),
                MinVisits = 1,
                MaxVisits = 1
            });
            context.SaveChanges();

            //// Seed Activities
            //context.Activities.Add(new Activity
            //{
            //    Station = context.Stations.Where(s => s.ID == 1).Single(),
            //    Group = context.Groups.Where(g => g.ID == 1).Single(),
            //    TimeSlot = context.TimeSlots.Where(t => t.ID == 1).Single()
            //});
            //context.Activities.Add(new Activity
            //{
            //    Station = context.Stations.Where(s => s.ID == 2).Single(),
            //    Group = context.Groups.Where(g => g.ID == 2).Single(),
            //    TimeSlot = context.TimeSlots.Where(t => t.ID == 2).Single()
            //});
            //context.Activities.Add(new Activity
            //{
            //    Station = context.Stations.Where(s => s.ID == 3).Single(),
            //    Group = context.Groups.Where(g => g.ID == 3).Single(),
            //    TimeSlot = context.TimeSlots.Where(t => t.ID == 3).Single()
            //});
            //context.Activities.Add(new Activity
            //{
            //    Station = context.Stations.Where(s => s.ID == 4).Single(),
            //    Group = context.Groups.Where(g => g.ID == 4).Single(),
            //    TimeSlot = context.TimeSlots.Where(t => t.ID == 4).Single()
            //});
            //context.Activities.Add(new Activity
            //{
            //    Station = context.Stations.Where(s => s.ID == 5).Single(),
            //    Group = context.Groups.Where(g => g.ID == 5).Single(),
            //    TimeSlot = context.TimeSlots.Where(t => t.ID == 5).Single()
            //});
            //context.SaveChanges();

        }
    }
}