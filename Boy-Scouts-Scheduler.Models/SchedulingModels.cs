using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boy_Scouts_Scheduler.Models
{
    public class Site
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }

    public class Event
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public virtual Site Site { get; set; }
        public virtual ICollection<Attendee> Attendees { get; set; }
    }

    public class Attendee
    {
        public int ID { get; set;}
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public virtual ICollection<Event> EventsAttended { get; set; }
    }

    public class GroupType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Group {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? TypeID { get; set; }
        public virtual Event Event { get; set; }
        public virtual GroupType Type { get; set; }
        public virtual Station Preference1 { get; set; }
        public virtual Station Preference2 { get; set; }
        public virtual Station Preference3 { get; set; }
        public virtual Station Preference4 { get; set; }
        public virtual Station Preference5 { get; set; }
        public virtual ICollection<Attendee> Members { get; set; }
    }

    public class Station
    {
        public int ID { get; set;}
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public string Category { get; set; }
        public bool isActivityPin { get; set; }
        public virtual Event Event { get; set; }
        public virtual ICollection<TimeSlot> AvailableTimeSlots { get; set; } // Should null mean all time slots or none?
    }

    public class TimeSlot
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public virtual Event Event { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool isGeneral { get; set; }
        public virtual ICollection<Station> OpenStations { get; set; }
    }

    public class Activity
    {
        public int ID { get; set; }
        public virtual Station Station { get; set; }
        public virtual Group Group { get; set; }
        public virtual TimeSlot TimeSlot { get; set; }
    }

    public class SchedulingConstraint
    {
        /* Null values will be assumed to be unconstrained */
        public int ID { get; set; }
        public GroupType GroupType { get; set; }
        public Group Group { get; set; }
        public Station Station { get; set; }
        public int VisitNum { get; set; }
        public string Category { get; set; }
    }
}