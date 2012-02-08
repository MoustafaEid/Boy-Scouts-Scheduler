using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boy_Scouts_Scheduling_Algorithm
{
    /*
     * MDD 2/02/2012
     * I am getting rid of the day field for constraints, because it adds an extra layer of
     * complexity when keeping track of how many times a given cub scout group must
     * visit a station. I'm not sure if there's a real use case for the days constraint either.
     * For example, I don't think Michelle would need to have group 1 visit the pool
     * at least twice on Tuesday and once on Thursday. Instead, she would say group 1
     * visits the pool at least three times throughout the week
     */
    
    public class Constraint
    {
        public Group Group { get; set; }
        public uint? GroupRank { get; set; }
        public IList<Station> Stations { get; set; }
        public uint? MinVisits { get; set; }
        public uint? MaxVisits { get; set; }
        public uint? Priority { get; set; }
        //public IList<StationDay> Days { get; set; }

        public Constraint(Group group, uint? groupRank, IList<Station> stations,
            uint? minVisits, uint? maxVisits, uint? priority)
        {
            Group = group;
            GroupRank = groupRank;
            Stations = stations;
            MinVisits = minVisits;
            MaxVisits = maxVisits;
            Priority = priority;
        }

    }

    public class StationDay
    {
        public uint DayNumber { get; set; }
        public IList<uint> OpenSlotNumbers { get; set; }

        public StationDay(uint stationDayNumber, IList<uint> stationSlotNumbers)
        {
            DayNumber = stationDayNumber;
            OpenSlotNumbers = stationSlotNumbers;
        }
    }

    public class Station
    {
        public string Name { get; set; }
        public uint Capacity { get; set; }
        public IList<StationDay> Availabilities { get; set; }

        public Station(string stationName, uint stationCapacity, IList<StationDay> stationAvailabilities)
        {
            Name = stationName;
            Capacity = stationCapacity;
            Availabilities = stationAvailabilities;
        }
    }

    public class Group
    {
        public string Name { get; set; }
        public uint Rank { get; set; }
        public IList<Station> TopStationPicks { get; set; }

        public Group(string groupName, uint groupRank, IList<Station> groupStationPicks)
        {
            Name = groupName;
            Rank = groupRank;
            TopStationPicks = groupStationPicks;
        }
    }

    public static class Scheduler
    {
        //each group must visit a station between minVisit and maxVisit times
        public struct StationAssignmentRange
        {
            public uint? minVisits;
            public uint? maxVisits;

            public StationAssignmentRange(uint? minVisits, uint? maxVisits)
            {
                this.minVisits = minVisits;
                this.maxVisits = maxVisits;
            }
        }

        public static List<List<Dictionary<Group, Station>>> GenerateSchedule
            (IList<Group> groups, IList<Station> stations, IList<Constraint> constraints)
        {
            Random random = new Random();

            //generated schedule for the week of camp
            List<List<Dictionary<Group, Station>>> generatedSchedule =
                new List<List<Dictionary<Group, Station>>>();

            //generated schedule for one day of camp
            //List<Dictionary<Group, Station>> dailySchedule = new List<Dictionary<Group, Station>>();

            //list of all groups who are unassigned at a given time slot
            IList<Group> unassignedGroups = new List<Group>();

            //compute the numbers of days in camp and the number of stations on each day
            uint numCampDays = findNumCampDays(stations);

            //number of times that each group should be assigned to each station for the week
            Dictionary<Group, Dictionary<Station, StationAssignmentRange>> groupStationVisitRange =
                new Dictionary<Group, Dictionary<Station, StationAssignmentRange>>();

            //populate the newly created groupStationvisitRange with the appropriate values
            populateGroupStationVisitEntries(constraints, groups, stations, ref groupStationVisitRange);

            //for each station at a given time slot, find the groups that have the station listed as their top picks
            IList<Group> eligableGroups = new List<Group>();

            // [GroupNumber, StationNumber] = Assignment Counter -- Currently unused, but we
            // don't want to overassign a group to a single station, so this will be used later
            int[,] groupStationAssignments = new int[groups.Count, stations.Count];

            //keep track of how many times each group needs to visit a station at a minimum
            uint greatestMinVisits;

            IList<uint> slotsPerDay = new List<uint>();
            findNumSlotsPerDay(stations, numCampDays, ref slotsPerDay);

            //initialize the schedule that will be generated
            initializeGeneratedSchedule(numCampDays, slotsPerDay, ref generatedSchedule);


            for (int dayNum = 0; dayNum < numCampDays; dayNum++)
            {
                for (int slotNum = 0; slotNum < slotsPerDay[(int)dayNum]; slotNum++)
                {
                    //assign as many groups as possible to their stations for a given time slot
                    Dictionary<Group, Station> assignments = new Dictionary<Group, Station>();

                    //at each new time slot, all groups are unassigned and no assignments are made
                    unassignedGroups.Clear();
                    foreach (Group group in groups)
                    {
                        unassignedGroups.Add(group);
                    }

                    foreach (Station station in stations)
                    {
                        if (!isStationAvailableAtSlot(station, dayNum + 1, slotNum + 1))
                            continue;

                        for (int capacityNum = 0; capacityNum < station.Capacity; capacityNum++)
                        {
                            greatestMinVisits = 0;
                            eligableGroups.Clear();

                            /*
                             * Loop through each group to see who is the best match for
                             * the current station at the current time slot.
                             * The following criteria is used to determine which group
                             * is the best match for the current station:
                             * 1. The group has one of their top choices to visit this
                             * station, and the other groups have this station listed
                             * as a lower choice
                             * 2. The group has to visit the station more times (min visits)
                             * than all of the other groups
                             * 3. Groups that have visited the station more than another
                             * group should be assigned to a different station -- not implemented
                             * 4. Groups with a higher rank have priority over groups
                             * with a lower rank -- currently not implemented
                             * If there is a tie after these criteria are applied, a
                             * random group will be chosen from the list of best matches
                            */

                            //filter group assignments based on their top three station picks
                            for (int topPickNum = 0; topPickNum < 3; topPickNum++)
                            {
                                foreach (Group group in unassignedGroups)
                                {
                                    if (group.TopStationPicks.Count > topPickNum &&
                                        group.TopStationPicks[topPickNum] == station)
                                    {
                                        eligableGroups.Add(group);
                                    }
                                }

                                //if we found at least one group who had the station as their top pick,
                                //then assign one of those groups to the station
                                if (eligableGroups.Count > 0)
                                    break;
                            }

                            //if none of the groups picked the current station, then
                            //they are all eligable to be assigned to the station
                            if (eligableGroups.Count == 0)
                                foreach (Group group in unassignedGroups)
                                    eligableGroups.Add(group);

                            //remove all groups who are not allowed to visit the station anymore
                            //cannot use a foreach loop because the collection is being modified
                            for (int groupNum = 0; groupNum < eligableGroups.Count; groupNum++)
                            {
                                Group group = eligableGroups[groupNum];
                                if (groupStationVisitRange[group][station].maxVisits == 0)
                                {
                                    eligableGroups.Remove(group);
                                    groupNum--;
                                }
                            }

                            //if all groups are not allowed to visit the station again, then
                            //that station will be unassigned for the current time slot
                            if (eligableGroups.Count == 0)
                                break;

                            //filter eleigable groups based on minimum number of visit for a station
                            IList<Group> eligableGroupsFilter1 = new List<Group>();

                            //if there is a tie based on top station picks, look at the
                            //minimum number of visits for a station
                            foreach (Group group in eligableGroups)
                            {
                                uint? groupMinVisits = groupStationVisitRange[group][station].minVisits;
                                if (groupMinVisits == greatestMinVisits ||
                                    (!groupMinVisits.HasValue && greatestMinVisits == 0))
                                {
                                    eligableGroupsFilter1.Add(group);
                                }
                                else if (groupMinVisits > greatestMinVisits)
                                {
                                    greatestMinVisits = (uint)groupMinVisits;
                                    eligableGroupsFilter1.Clear();
                                    eligableGroupsFilter1.Add(group);
                                }
                            }

                            //from the remaining candidates, pick a random group and assign it to the station
                            Group assignedGroup = null;
                            if (eligableGroupsFilter1.Count > 0)
                            {
                                int groupNumber = random.Next(eligableGroupsFilter1.Count);
                                assignedGroup = eligableGroupsFilter1[groupNumber];
                            }
                            else if (eligableGroups.Count > 0)
                            {
                                int groupNumber = random.Next(eligableGroups.Count);
                                assignedGroup = eligableGroups[groupNumber];
                            }

                            //update the topStationPicks and stationVisitRange for the assigned
                            //group, and remove them from the list of unassigned groups
                            if (assignedGroup != null)
                            {
                                assignments.Add(assignedGroup, station);

                                if (assignedGroup.TopStationPicks.Contains(station))
                                    assignedGroup.TopStationPicks.Remove(station);

                                uint? minVisits = groupStationVisitRange[assignedGroup][station].minVisits;
                                uint? maxVisits = groupStationVisitRange[assignedGroup][station].maxVisits;

                                if (minVisits.HasValue && minVisits > 0)
                                    minVisits--;
                                if (maxVisits.HasValue && maxVisits > 0) //maxVisits shouldn't equal 0
                                    maxVisits--;

                                groupStationVisitRange[assignedGroup].Remove(station);
                                groupStationVisitRange[assignedGroup].Add(station,
                                    new StationAssignmentRange(minVisits, maxVisits));

                                unassignedGroups.Remove(assignedGroup);
                                if (unassignedGroups.Count == 0)
                                    break;
                            }
                        }
                    }

                    generatedSchedule[dayNum][slotNum] = assignments;
                }
            }
            return generatedSchedule;
        }

        private static void initializeGeneratedSchedule(uint numCampDays,
            IList<uint> slotsPerDay, ref List<List<Dictionary<Group, Station>>> generatedSchedule)
        {
            for (uint dayNum = 0; dayNum < numCampDays; dayNum++)
            {
                List<Dictionary<Group, Station>> dayAssignments = new List<Dictionary<Group, Station>>();
                for (uint slotNum = 0; slotNum < slotsPerDay[(int)dayNum]; slotNum++)
                {
                    Dictionary<Group, Station> assignment = new Dictionary<Group, Station>();
                    dayAssignments.Add(assignment);
                }
                generatedSchedule.Add(dayAssignments);
            }
        }

        private static void populateGroupStationVisitEntries(
            IList<Constraint> constraints, IList<Group> groups, IList<Station> stations,
            ref Dictionary<Group, Dictionary<Station, StationAssignmentRange>> groupStationVisitRange)
        {
            //first, initialize the groups so there are no constraints for all days of camp
            foreach (Group group in groups)
            {
                Dictionary<Station, StationAssignmentRange> groupAssignment =
                    new Dictionary<Station, StationAssignmentRange>();

                foreach (Station station in stations)
                {
                    groupAssignment.Add(station, new StationAssignmentRange(null, null));
                }
                groupStationVisitRange.Add(group, groupAssignment);
            }

            //go through each constraint for all groups and update the groupStationVisitRange
            foreach (Constraint constraint in constraints)
            {
                foreach (Group group in groups)
                {
                    //group names match or there's no group name specified in the constraints
                    //but the ranks match, or there's no group name or rank specified
                    if ((constraint.Group == null && constraint.GroupRank == null) ||
                        (constraint.Group == null && group.Rank == constraint.GroupRank) ||
                        (constraint.Group != null && group.Name == constraint.Group.Name))
                    {
                        //if the constraint applies to the group, update the
                        //stationAssignmentRanges for each station
                        foreach (Station station in constraint.Stations)
                        {
                            groupStationVisitRange[group][station] =
                                new StationAssignmentRange(constraint.MinVisits, constraint.MaxVisits);
                        }
                    }
                }
            }
        }

        private static bool isStationAvailableAtSlot
            (Station station, int dayNumber, int slotNumber)
        {
            if (station.Capacity == 0)
                return false;

            foreach (StationDay day in station.Availabilities)
            {
                if (day.DayNumber == dayNumber && day.OpenSlotNumbers.Contains((uint)slotNumber))
                    return true;
            }

            return false;
        }

        private static uint findNumCampDays(IList<Station> stations)
        {
            uint numCampDays = 0;
            foreach (Station currentStation in stations)
            {
                foreach (StationDay currentDay in currentStation.Availabilities)
                {
                    if (currentDay.DayNumber > numCampDays)
                        numCampDays = currentDay.DayNumber;
                }
            }
            return numCampDays;
        }

        private static void findNumSlotsPerDay(IList<Station> stations, uint numCampDays, ref IList<uint> slotsPerDay)
        {
            for (int dayNum = 1; dayNum <= numCampDays; dayNum++)
            {
                uint numSlots = 0;
                foreach (Station currentStation in stations)
                {
                    foreach (StationDay currentDay in currentStation.Availabilities)
                    {
                        if (currentDay.DayNumber == dayNum)
                        {
                            foreach (uint slotNumber in currentDay.OpenSlotNumbers)
                            {
                                if (slotNumber > numSlots)
                                    numSlots = slotNumber;
                            }
                        }
                    }
                }
                slotsPerDay.Add(numSlots);
            }
        }

    }
}
