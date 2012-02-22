using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Boy_Scouts_Scheduler.Algorithm
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

	public static class HillClimbingAlgorithm
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

            //We need to make a copy of groups, because their list of top station picks
            //is modified when generating an initial schedule, but the hill climbing
            //algorithm needs to know their top station picks to do scoring
            IList<Group> groupCopy = new List<Group>();
            foreach (Group group in groups)
            {
                IList<Station> topStationPicks = new List<Station>();
                foreach (Station station in group.TopStationPicks)
                {
                    topStationPicks.Add(station);
                }
                groupCopy.Add(new Group(group.Name, group.Rank, topStationPicks));
            }

            //generated schedule for the week of camp
            List<List<Dictionary<Group, Station>>> generatedSchedule =
                new List<List<Dictionary<Group, Station>>>();

            //list of all groups who are unassigned at a given time slot
            IList<Group> unassignedGroups = new List<Group>();

            //compute the numbers of days in camp and the number of stations on each day
            uint numCampDays = findNumCampDays(stations);

            //number of times (min, max) that each group should be assigned to each station for the week
            Dictionary<Group, Dictionary<Station, StationAssignmentRange>> groupStationVisitRange =
                new Dictionary<Group, Dictionary<Station, StationAssignmentRange>>();

            //number of times that each group has been assigned to a station
            Dictionary<Group, Dictionary<Station, uint>> groupStationAssignments =
                new Dictionary<Group, Dictionary<Station, uint>>();

            //initialize the newly created groupStationvisitRange and
            //groupStationAssignments with the appropriate values
            initializeGroupStationVisitRange(constraints, groups, stations, ref groupStationVisitRange);
            initializeGroupStationAssignments(groups, stations, ref groupStationAssignments);
            
            //for each station at a given time slot, find the
            //groups that have the station listed as their top picks
            IList<Group> eligibleGroups = new List<Group>();

            //keep track of how many times each group needs to visit a station at a minimum
            //and which group has visited the current station the least number of times
            uint greatestMinVisits, leastGroupAssignmentNum;

            //number of slots for each day of camp
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
                            leastGroupAssignmentNum = 0;
                            eligibleGroups.Clear();

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
                             * group should be assigned to a different station
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
                                        eligibleGroups.Add(group);
                                    }
                                }

                                //if we found at least one group who had the station as their top pick,
                                //then assign one of those groups to the station
                                if (eligibleGroups.Count > 0)
                                    break;
                            }

                            //if none of the groups picked the current station, then
                            //they are all eligible to be assigned to the station
                            if (eligibleGroups.Count == 0)
                                foreach (Group group in unassignedGroups)
                                    eligibleGroups.Add(group);

                            //remove all groups who are not allowed to visit the station anymore
                            //cannot use a foreach loop because the collection is being modified
                            for (int groupNum = 0; groupNum < eligibleGroups.Count; groupNum++)
                            {
                                Group group = eligibleGroups[groupNum];
                                if (groupStationVisitRange[group][station].maxVisits == 0)
                                {
                                    eligibleGroups.Remove(group);
                                    groupNum--;
                                }
                            }

                            //if all groups are not allowed to visit the station again, then
                            //that station will be unassigned for the current time slot
                            if (eligibleGroups.Count == 0)
                                break;

                            //filter eleigable groups based on minimum number of visit for a station
                            IList<Group> eligibleGroupsFilter1 = new List<Group>();

                            //if there is a tie based on top station picks, look at the
                            //minimum number of visits for a station
                            foreach (Group group in eligibleGroups)
                            {
                                uint? groupMinVisits = groupStationVisitRange[group][station].minVisits;
                                if (groupMinVisits == greatestMinVisits ||
                                    (!groupMinVisits.HasValue && greatestMinVisits == 0))
                                {
                                    eligibleGroupsFilter1.Add(group);
                                }
                                else if (groupMinVisits > greatestMinVisits)
                                {
                                    greatestMinVisits = (uint)groupMinVisits;
                                    eligibleGroupsFilter1.Clear();
                                    eligibleGroupsFilter1.Add(group);
                                }
                            }

                            if (eligibleGroupsFilter1.Count > 0)
                                leastGroupAssignmentNum = groupStationAssignments[eligibleGroupsFilter1[0]][station];

                            IList<Group> eligibleGroupsFilter2 = new List<Group>();

                            //if there is a tie based on minimum visits, look at how
                            //many times each group has already been assigned to a station
                            foreach (Group group in eligibleGroupsFilter1)
                            {
                                uint groupAssignmentNum = groupStationAssignments[group][station];
                                if (groupAssignmentNum == leastGroupAssignmentNum)
                                    eligibleGroupsFilter2.Add(group);
                                else if (groupAssignmentNum < leastGroupAssignmentNum)
                                {
                                    leastGroupAssignmentNum = groupAssignmentNum;
                                    eligibleGroupsFilter2.Clear();
                                    eligibleGroupsFilter2.Add(group);
                                }
                            }

                            //from the remaining candidates, pick a random group and assign it to the station
                            Group assignedGroup = null;
                            if (eligibleGroupsFilter2.Count > 0)
                            {
                                int groupNumber = random.Next(eligibleGroupsFilter2.Count);
                                assignedGroup = eligibleGroupsFilter2[groupNumber];
                            }
                            else if (eligibleGroupsFilter1.Count > 0)
                            {
                                int groupNumber = random.Next(eligibleGroupsFilter1.Count);
                                assignedGroup = eligibleGroupsFilter1[groupNumber];
                            }
                            else if (eligibleGroups.Count > 0)
                            {
                                int groupNumber = random.Next(eligibleGroups.Count);
                                assignedGroup = eligibleGroups[groupNumber];
                            }

                            //update the topStationPicks, stationVisitRange, and groupStationAssignments
                            //for the assigned group, and remove them from the list of unassigned groups
                            if (assignedGroup != null)
                            {
                                assignments.Add(assignedGroup, station);

                                int index = assignedGroup.TopStationPicks.IndexOf(station);
                                if (index >= 0)
                                    assignedGroup.TopStationPicks[index] = null;

                                uint? minVisits = groupStationVisitRange[assignedGroup][station].minVisits;
                                uint? maxVisits = groupStationVisitRange[assignedGroup][station].maxVisits;

                                if (minVisits.HasValue && minVisits > 0)
                                    minVisits--;
                                if (maxVisits.HasValue && maxVisits > 0) //maxVisits shouldn't equal 0
                                    maxVisits--;

                                groupStationVisitRange[assignedGroup].Remove(station);
                                groupStationVisitRange[assignedGroup].Add(station,
                                    new StationAssignmentRange(minVisits, maxVisits));

                                groupStationAssignments[assignedGroup][station]++;
                                unassignedGroups.Remove(assignedGroup);
                                if (unassignedGroups.Count == 0)
                                    break;
                            }
                        }
                    }
                    generatedSchedule[dayNum][slotNum] = assignments;
                }
            }


            /* 
             * Now we have generated a schedule -- go back and fix any violated constraints that we can.
             * Loop through all values in groupStationVisitRange and check that the min value is 0 in
             * all cases -- if the min value is > 0, then a group needs to be assigned to that station
             * more times 
            */
            //foreach (var pair in groupStationVisitRange)
            //{
            //    foreach (var pair1 in pair.Value)
            //    {
            //        //if the group is underassigned to a station, pick a random slot where they're
            //        //unassigned and see if giving them that station won't introduce another violation
            //        if (pair1.Value.minVisits > 0)
            //        {
            //            Group underAssignedGroup = pair.Key;
            //            int campDayNum = random.Next((int)numCampDays);
            //            int slotNum = random.Next((int)slotsPerDay[campDayNum]);
            //            Dictionary<Group, Station> Assignments = generatedSchedule[campDayNum][slotNum];

            //            if (Assignments.ContainsKey(underAssignedGroup) &&
            //                !Assignments[underAssignedGroup].Equals(pair1.Key))
            //            {
            //                //foreach group in Assignments
            //                //if the group is not the underAssignedGroup and their minVisit number < 0 or null
            //                //and the underAssignedGroup is allowed to be assigned to that station again
            //                //assign them to underAssignedGroup's current station and assign
            //                //underAssignedGroup to the other station


            //                //for (int lcv = 0; lcv < Assignments.Count; lcv++) //var pair2 in Assignments)
            //                foreach (var pair2 in Assignments)
            //                {
            //                    Dictionary<Group, Station>.KeyCollection assignmentKeys = Assignments.Keys;
            //                    //Assignments.Intersect(KeyValuePair<Group, Station> 
            //                    //Assignments.Keys
            //                    //Station otherGroupStation = Assignments[Assignments.Keys[lcv]];
            //                    Group otherGroup = pair2.Key;
            //                    Station otherGroupStation = pair2.Value;

            //                    uint? otherGroupMinVisits =
            //                        groupStationVisitRange[otherGroup][otherGroupStation].minVisits;

            //                    uint? underAssignedGroupMaxVisits =
            //                        groupStationVisitRange[underAssignedGroup][otherGroupStation].maxVisits;

            //                    if (otherGroup != underAssignedGroup &&
            //                        (otherGroupMinVisits < 0 || !otherGroupMinVisits.HasValue) &&
            //                        (underAssignedGroupMaxVisits > 0 || !underAssignedGroupMaxVisits.HasValue))
            //                    {
            //                        Station temp = Assignments[underAssignedGroup];
            //                        Assignments[underAssignedGroup] = Assignments[otherGroup];
            //                        Assignments[otherGroup] = temp;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            //now we are ready to start hill climbing, so keep track of the top scoring schedule so far
            uint score = Score.ScoreSchedule(generatedSchedule, groupCopy, groupStationVisitRange);
            //for (int lcv = 0; lcv < 100; lcv++)
            //{
                
            //    score = Score.ScoreSchedule(generatedSchedule, groupCopy, groupStationVisitRange);
            //}
            MessageBox.Show(score.ToString());
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

        private static void initializeGroupStationAssignments(IList<Group> groups,
            IList<Station> stations, ref Dictionary<Group, Dictionary<Station, uint>> groupStationAssignments)
        {
            foreach (Group group in groups)
            {
                Dictionary<Station, uint> groupAssignment =
                    new Dictionary<Station, uint>();
                foreach (Station station in stations)
                {
                    groupAssignment.Add(station, 0);
                }
                groupStationAssignments.Add(group, groupAssignment);
            }
        }

        private static void initializeGroupStationVisitRange(
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