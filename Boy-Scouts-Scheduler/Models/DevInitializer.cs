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
            context.GroupTypes.Add(new GroupType { Name = "Tiger" });
            context.GroupTypes.Add(new GroupType { Name = "Wolf" });
            context.GroupTypes.Add(new GroupType { Name = "Bear" });
            context.GroupTypes.Add(new GroupType { Name = "Webelos" });

            context.Events.Add(new Event
            {
                Name = "Dev Event",
                Start = new DateTime(2012, 3, 5),
                End = new DateTime(2012, 3, 9)
            });
            context.SaveChanges();

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
        }
    }
}