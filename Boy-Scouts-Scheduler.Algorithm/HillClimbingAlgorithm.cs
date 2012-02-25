using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boy_Scouts_Scheduler.Algorithm
{
	public static class HillClimbingAlgorithm
    {
        public struct StationAssignmentRange
        {
            public int? minVisits;
            public int? maxVisits;

            public StationAssignmentRange(int? minVisits, int? maxVisits)
            {
                this.minVisits = minVisits;
                this.maxVisits = maxVisits;
            }

            public void incrementMinVisits()
            {
                if (minVisits.HasValue)
                    minVisits++;
            }

            public void incrementMaxVisits()
            {
                if (maxVisits.HasValue)
                    maxVisits++;
            }

            public void decrementMinVisits()
            {
                if (minVisits.HasValue)
                    minVisits--;
            }

            public void decrementMaxVisits()
            {
                if (maxVisits.HasValue)
                    maxVisits--;
            }
        }

        public static IList<Models.Activity> GenerateSchedule
           (IList<Models.Group> modelGroups, IList<Models.Station> modelStations,
           IList<Models.SchedulingConstraint> modelConstraints, IList<Models.TimeSlot> modelTimeSlots)
        {
            Random random = new Random();

            //groups that remain unassigned at a current time slot
            IList<Models.Group> unassignedGroups = new List<Models.Group>();

            //remaining groups that are allowed to visit the current station
            IList<Models.Group> eligibleGroups = new List<Models.Group>();

            Dictionary<Models.Group, Dictionary<Models.Station, StationAssignmentRange>> groupStationVisitRange =
                new Dictionary<Models.Group, Dictionary<Models.Station, StationAssignmentRange>>();

            initializeGroupStationVisitRange(modelConstraints, modelGroups, modelStations, ref groupStationVisitRange);

            Dictionary<Models.Group, Dictionary<Models.Station, int>> groupStationAssignments =
                new Dictionary<Models.Group, Dictionary<Models.Station, int>>();

            initializeGroupStationAssignments(modelGroups, modelStations, ref groupStationAssignments);

            int? greatestMinVisits; int? leastGroupAssignmentNum; int activityNumber = 0;

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
                        greatestMinVisits = 0;
                        leastGroupAssignmentNum = 0;
                        eligibleGroups.Clear();

                        for (int topStationPick = 0; topStationPick < 3; topStationPick++)
                        {
                            for (int groupNum = 0; groupNum < unassignedGroups.Count; groupNum++)
                            {
                                Models.Group currentGroup = unassignedGroups[groupNum];
                                Models.Station topStation;
                                if (topStationPick == 0)
                                    topStation = currentGroup.Preference1;
                                else if (topStationPick == 1)
                                    topStation = currentGroup.Preference2;
                                else
                                    topStation = currentGroup.Preference3;
                                if (topStation == currentStation)
                                    eligibleGroups.Add(currentGroup);
                            }
                            if (eligibleGroups.Count > 0)
                                break;
                        }

                        //if none of the groups picked the current station as one of their
                        //preferences, they are all eligible to be assigned to that station
                        if (eligibleGroups.Count == 0)
                        {
                            foreach (Models.Group group in unassignedGroups)
                            {
                                eligibleGroups.Add(group);
                            }
                        }

                        //remove all groups who are not allowed to visit the station anymore
                        //cannot use a foreach loop because the collection is being modified
                        for (int groupNum = 0; groupNum < eligibleGroups.Count; groupNum++)
                        {
                            Models.Group group = eligibleGroups[groupNum];
                            if (groupStationVisitRange[group][currentStation].maxVisits == 0)
                            {
                                eligibleGroups.Remove(group);
                                groupNum--;
                            }
                        }

                        if (eligibleGroups.Count == 0)
                            continue;

                        IList<Models.Group> eligibleGroupsFilter1 = new List<Models.Group>();

                        foreach (Models.Group group in eligibleGroups)
                        {
                            int? groupMinVisits = groupStationVisitRange[group][currentStation].minVisits;
                            if (groupMinVisits == greatestMinVisits ||
                                (!groupMinVisits.HasValue && greatestMinVisits == 0))
                            {
                                eligibleGroupsFilter1.Add(group);
                            }
                            else if (groupMinVisits > greatestMinVisits)
                            {
                                greatestMinVisits = groupMinVisits;
                                eligibleGroupsFilter1.Clear();
                                eligibleGroupsFilter1.Add(group);
                            }
                        }

                        if (eligibleGroupsFilter1.Count > 0)
                        {
                            leastGroupAssignmentNum =
                                groupStationAssignments[eligibleGroupsFilter1[0]][currentStation];
                        }

                        IList<Models.Group> eligibleGroupsFilter2 = new List<Models.Group>();

                        //if there is a tie based on minimum visits, look at how
                        //many times each group has already been assigned to a station
                        foreach (Models.Group group in eligibleGroupsFilter1)
                        {
                            int groupAssignmentNum = groupStationAssignments[group][currentStation];
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
                        Models.Group assignedGroup = null;
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
                            activityNumber++;
                            Models.Activity activity = new Models.Activity();
                            activity.ID = activityNumber;
                            activity.Group = assignedGroup;
                            activity.Station = currentStation;
                            activity.TimeSlot = timeSlot;

                            generatedSchedule.Add(activity);

                            if (assignedGroup.Preference1 == currentStation)
                                assignedGroup.Preference1 = null;
                            else if (assignedGroup.Preference2 == currentStation)
                                assignedGroup.Preference2 = null;
                            else if (assignedGroup.Preference3 == currentStation)
                                assignedGroup.Preference3 = null;

                            groupStationVisitRange[assignedGroup][currentStation].decrementMinVisits();
                            groupStationVisitRange[assignedGroup][currentStation].decrementMaxVisits();

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
            IList<Models.SchedulingConstraint> constraints, IList<Models.Group> groups, IList<Models.Station> stations,
            ref Dictionary<Models.Group, Dictionary<Models.Station, StationAssignmentRange>> groupStationVisitRange)
        {
            //first, initialize the groups so there are no constraints for all days of camp
            foreach (Models.Group group in groups)
            {
                Dictionary<Models.Station, StationAssignmentRange> groupAssignment =
                    new Dictionary<Models.Station, StationAssignmentRange>();

                foreach (Models.Station station in stations)
                {
                    groupAssignment.Add(station, new StationAssignmentRange(null, null));
                }
                groupStationVisitRange.Add(group, groupAssignment);
            }

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
                            new StationAssignmentRange(constraint.MinVisits, constraint.MaxVisits);
                    }
                }
            }
        }

        private static void initializeGroupStationAssignments(IList<Models.Group> groups,
            IList<Models.Station> stations,
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
