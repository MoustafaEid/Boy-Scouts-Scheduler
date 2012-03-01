using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boy_Scouts_Scheduler.Algorithm
{
    public static class HillClimbingAlgorithm
    {
        public class StationAssignmentRange
        {
            public int numVisits;

            public StationAssignmentRange(int numStationVisits)
            {
                numVisits = numStationVisits;
            }

            public void incrementNumVisits()
            {
                numVisits++;
            }

            public void decrementNumVisits()
            {
                numVisits--;
            }
       
            //public StationAssignmentRange(int? minVisits, int? maxVisits)
            //{
            //    this.minVisits = minVisits;
            //    this.maxVisits = maxVisits;
            //}

            //public void incrementMinVisits()
            //{
            //    if (minVisits.HasValue)
            //        minVisits++;
            //}

            //public void incrementMaxVisits()
            //{
            //    if (maxVisits.HasValue)
            //        maxVisits++;
            //}

            //public void decrementMinVisits()
            //{
            //    if (minVisits.HasValue)
            //        minVisits--;
            //}

            //public void decrementMaxVisits()
            //{
            //    if (maxVisits.HasValue)
            //        maxVisits--;
            //}
        }

        private static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
        {
            var r = new Random();
            return enumerable.OrderBy(x => r.Next()).ToList();
        } 

        public static IEnumerable<Models.Activity> GenerateSchedule
           (IEnumerable<Models.Group> modelGroups, IEnumerable<Models.Station> modelStations,
           IEnumerable<Models.SchedulingConstraint> modelConstraints, IEnumerable<Models.TimeSlot> modelTimeSlots)
        {
            modelTimeSlots = Shuffle(modelTimeSlots);
            modelStations = Shuffle(modelStations);

            Random random = new Random();

            //Copy of groups, so their preferences don't get lost when scheduling
            IList<Models.Group> groupCopy = new List<Models.Group>();
            foreach (Models.Group group in modelGroups)
            {
                Models.Group newGroup = new Models.Group();
                groupCopy.Add(newGroup);
            }

            //groups that remain unassigned at a current time slot
            IList<Models.Group> unassignedGroups = new List<Models.Group>();

            //remaining groups that are allowed to visit the current station
            IList<Models.Group> eligibleGroups = new List<Models.Group>();

            //how many times a group is allowed to visit a station
            Dictionary<Models.Group, Dictionary<Models.Station, StationAssignmentRange>> groupStationVisitRange =
                new Dictionary<Models.Group, Dictionary<Models.Station, StationAssignmentRange>>();

            initializeGroupStationVisitRange(modelConstraints, modelGroups, modelStations, ref groupStationVisitRange);

            //how many times a group has already been assigned to a station
            Dictionary<Models.Group, Dictionary<Models.Station, int>> groupStationAssignments =
                new Dictionary<Models.Group, Dictionary<Models.Station, int>>();

            initializeGroupStationAssignments(modelGroups, modelStations, ref groupStationAssignments);

            int greatestNumVisits; int? leastGroupAssignmentNum; int activityNumber = 0;

            IList<Models.Activity> generatedSchedule = new List<Models.Activity>();
            foreach (Models.TimeSlot timeSlot in modelTimeSlots)
            {
                //at the beginning of each time slot, all groups are unassigned
                unassignedGroups.Clear();
                foreach (Models.Group group in modelGroups)
                {
                    unassignedGroups.Add(group);
                }

                foreach (Models.Station currentStation in timeSlot.OpenStations)
                {
                    for (int capacityNum = 0; capacityNum < currentStation.Capacity; capacityNum++)
                    {
                        greatestNumVisits = 0;
                        leastGroupAssignmentNum = 0;
                        eligibleGroups.Clear();

                        //remove all groups who are not allowed to visit the station anymore
                        foreach (Models.Group group in unassignedGroups)
                        {
                            if (groupStationVisitRange[group][currentStation].numVisits > 0)
                                eligibleGroups.Add(group);
                        }

                        if (eligibleGroups.Count == 0)
                            continue;

                        //if a group is required to visit another station more times than the current
                        //station, don't assign them to the current station if possible
                        for (int lcv = 0; lcv < eligibleGroups.Count; lcv++)
                        {
                            Models.Group group = eligibleGroups[lcv];
                            foreach (Models.Station otherStation in timeSlot.OpenStations)
                            {
                                if (groupStationVisitRange[group][otherStation].numVisits >
                                    groupStationVisitRange[group][currentStation].numVisits) 
                                {
                                    eligibleGroups.Remove(group);
                                    lcv--;
                                }
                            }
                        }

                        IList<Models.Group> eligibleGroupsFilter1 = new List<Models.Group>();
                        for (int topStationPick = 0; topStationPick < 5; topStationPick++)
                        {
                            foreach (Models.Group currentGroup in eligibleGroups)
                            {
                                Models.Station topStation;
                                if (topStationPick == 0)
                                    topStation = currentGroup.Preference1;
                                else if (topStationPick == 1)
                                    topStation = currentGroup.Preference2;
                                else if (topStationPick == 2)
                                    topStation = currentGroup.Preference3;
                                else if (topStationPick == 3)
                                    topStation = currentGroup.Preference4;
                                else
                                    topStation = currentGroup.Preference5;

                                if (topStation == currentStation)
                                    eligibleGroupsFilter1.Add(currentGroup);
                            }
                            if (eligibleGroupsFilter1.Count > 0)
                                break;
                        }

                        //if none of the groups picked the current station as one of their
                        //preferences, they are all eligible to be assigned to that station
                        if (eligibleGroupsFilter1.Count == 0)
                        {
                            foreach (Models.Group group in eligibleGroups)
                            {
                                eligibleGroupsFilter1.Add(group);
                            }
                        }

                        IList<Models.Group> eligibleGroupsFilter2 = new List<Models.Group>();

                        foreach (Models.Group group in eligibleGroupsFilter1)
                        {
                            int groupNumVisits = groupStationVisitRange[group][currentStation].numVisits;
                            if (groupNumVisits == greatestNumVisits)
                            {
                                eligibleGroupsFilter2.Add(group);
                            }
                            else if (groupNumVisits > greatestNumVisits)
                            {
                                greatestNumVisits = groupNumVisits;
                                eligibleGroupsFilter2.Clear();
                                eligibleGroupsFilter2.Add(group);
                            }
                        }

                        if (eligibleGroupsFilter2.Count > 0)
                        {
                            leastGroupAssignmentNum =
                                groupStationAssignments[eligibleGroupsFilter2[0]][currentStation];
                        }

                        IList<Models.Group> eligibleGroupsFilter3 = new List<Models.Group>();

                        //if there is a tie based on minimum visits, look at how
                        //many times each group has already been assigned to a station
                        foreach (Models.Group group in eligibleGroupsFilter2)
                        {
                            int groupAssignmentNum = groupStationAssignments[group][currentStation];
                            if (groupAssignmentNum == leastGroupAssignmentNum)
                                eligibleGroupsFilter3.Add(group);
                            else if (groupAssignmentNum < leastGroupAssignmentNum)
                            {
                                leastGroupAssignmentNum = groupAssignmentNum;
                                eligibleGroupsFilter3.Clear();
                                eligibleGroupsFilter3.Add(group);
                            }
                        }

                        //from the remaining candidates, pick a random group and assign it to the station
                        Models.Group assignedGroup = null;

                        if (eligibleGroupsFilter3.Count > 0)
                        {
                            int groupNumber = random.Next(eligibleGroupsFilter3.Count);
                            assignedGroup = eligibleGroupsFilter2[groupNumber];
                        }
                        else if (eligibleGroupsFilter2.Count > 0)
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
                            activityNumber++;
                            Models.Activity activity = new Models.Activity();
                            activity.ID = activityNumber;
                            activity.Group = groupCopy.First(x => x.ID == assignedGroup.ID);// assignedGroup;
                            activity.Station = currentStation;
                            activity.TimeSlot = timeSlot;

                            generatedSchedule.Add(activity);

                            if (assignedGroup.Preference1 == currentStation)
                                assignedGroup.Preference1 = null;
                            else if (assignedGroup.Preference2 == currentStation)
                                assignedGroup.Preference2 = null;
                            else if (assignedGroup.Preference3 == currentStation)
                                assignedGroup.Preference3 = null;
                            else if (assignedGroup.Preference4 == currentStation)
                                assignedGroup.Preference4 = null;
                            else if (assignedGroup.Preference5 == currentStation)
                                assignedGroup.Preference5 = null;

                            groupStationVisitRange[assignedGroup][currentStation].decrementNumVisits();

                            groupStationAssignments[assignedGroup][currentStation]++;
                            unassignedGroups.Remove(assignedGroup);

                            if (unassignedGroups.Count == 0)
                                break;
                        }
                    }
                }
            }
            return generatedSchedule;
        }

        private static void initializeGroupStationVisitRange(
            IEnumerable<Models.SchedulingConstraint> constraints, IEnumerable<Models.Group> groups,
            IEnumerable<Models.Station> stations,
            ref Dictionary<Models.Group, Dictionary<Models.Station, StationAssignmentRange>> groupStationVisitRange)
        {
            //first, initialize the groups so there are no constraints for all days of camp
            //foreach (Models.Group group in groups)
            //{
            //    Dictionary<Models.Station, StationAssignmentRange> groupAssignment =
            //        new Dictionary<Models.Station, StationAssignmentRange>();

            //    foreach (Models.Station station in stations)
            //    {
            //        groupAssignment.Add(station, new StationAssignmentRange(null, null));
            //    }
            //    groupStationVisitRange.Add(group, groupAssignment);
            //}

            //go through each constraint for all groups and update the groupStationVisitRange
            foreach (Models.SchedulingConstraint constraint in constraints)
            {
                foreach (Models.Group group in groups)
                {
                    //group names match or there's no group name specified in the constraints
                    //but the ranks match, or there's no group name or rank specified
                    if ((constraint.Group == null && constraint.GroupType == null) ||
                        (constraint.Group == null && constraint.GroupType == group.Type) ||
                        (constraint.Group != null && constraint.Group == group))
                    {
                        groupStationVisitRange[group][constraint.Station] =
                            new StationAssignmentRange(constraint.VisitNum);
                    }
                }
            }
        }

        private static void initializeGroupStationAssignments(IEnumerable<Models.Group> groups,
            IEnumerable<Models.Station> stations,
            ref Dictionary<Models.Group, Dictionary<Models.Station, int>> groupStationAssignments)
        {
            foreach (Models.Group group in groups)
            {
                Dictionary<Models.Station, int> groupAssignment =
                    new Dictionary<Models.Station, int>();
                foreach (Models.Station station in stations)
                {
                    groupAssignment.Add(station, 0);
                }
                groupStationAssignments.Add(group, groupAssignment);
            }
        }

    }
}
