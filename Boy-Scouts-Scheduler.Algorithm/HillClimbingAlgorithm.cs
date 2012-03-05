using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boy_Scouts_Scheduler.Algorithm
{
    public static class HillClimbingAlgorithm
    {
        //used to pick a random station assignment from a list of top candidates
        private static Random random = new Random();
        private static int activityNumber = 0;

        //used to keep track of which preferences the groups have received
        private static Dictionary<Models.Group, List<bool>> groupPreferenceGiven =
            new Dictionary<Models.Group, List<bool>>();

        //how many times a group has already been assigned to a station
        private static Dictionary<Models.Group, Dictionary<Models.Station, int>> groupStationAssignments =
            new Dictionary<Models.Group, Dictionary<Models.Station, int>>();

        //how many times a group is allowed to visit a station
        private static Dictionary<Models.Group, Dictionary<Models.Station, StationAssignmentRange>>
            groupStationVisitRange = new Dictionary<Models.Group,
            Dictionary<Models.Station, StationAssignmentRange>>();

        //groups that remain unassigned at a current time slot
        private static IList<Models.Group> unassignedGroups = new List<Models.Group>();

        //list of remaining unassigned stations at a given time slot
        private static IList<Models.Station> unassignedStations = new List<Models.Station>();

        //list of unassigned groups for next time slot -- used for Webelos activity pins
        private static IList<Models.Group> nextTimeSlotUnassignedGroups = new List<Models.Group>();

        //list of unassigned stations for next time slot -- used for activity pins
        private static IList<Models.Station> nextTimeSlotUnassignedStations = new List<Models.Station>();

        //list of stations that a given group is eligible for at a given time slot
        private static IList<Models.Station> eligibleStations = new List<Models.Station>();

        //remaining groups that are allowed to visit the current station
        private static IList<Models.Group> eligibleGroups = new List<Models.Group>();

        //used to contain minVisits and maxVisits -- now there is a single field that
        //keeps track of how many times, if any, that a group must visit a station
        public class StationAssignmentRange
        {
            public int? numVisits;

            public StationAssignmentRange(int? numStationVisits)
            {
                numVisits = numStationVisits;
            }

            public void incrementNumVisits()
            {
                if (numVisits.HasValue)
                    numVisits++;
            }

            public void decrementNumVisits()
            {
                if (numVisits.HasValue)
                    numVisits--;
            }
        }

        private static IList<T> Shuffle<T>(this IEnumerable<T> enumerable)
        {
            var r = new Random();
            return enumerable.OrderBy(x => r.Next()).ToList();
        } 

        public static IEnumerable<Models.Activity> GenerateSchedule
           (IEnumerable<Models.Group> modelGroups, IEnumerable<Models.Station> modelStations,
           IEnumerable<Models.SchedulingConstraint> modelConstraints, IEnumerable<Models.TimeSlot> modelTimeSlots)
        {
            InitializeData();

            //The time slots passed in are supposed to be sorted, but it's better not to assume that
            IList<Models.TimeSlot> sortedTimeSlots = new List<Models.TimeSlot>();

            foreach (Models.TimeSlot timeSlot in modelTimeSlots)
                sortedTimeSlots.Add(timeSlot);
            
            sortedTimeSlots.OrderBy(k => k.Start);

            modelStations = Shuffle(modelStations);
            initializeGroupStationVisitRange(modelConstraints, modelGroups, modelStations, ref groupStationVisitRange);
            initializeGroupPreferenceGiven(modelGroups, ref groupPreferenceGiven);
            initializeGroupStationAssignments(modelGroups, modelStations, ref groupStationAssignments);

            IList<Models.Activity> generatedSchedule = new List<Models.Activity>();

            //initialize all groups and stations to be eligible for the next time slot
            foreach (Models.Group group in modelGroups)
                nextTimeSlotUnassignedGroups.Add(group);
            foreach (Models.Station station in modelStations)
                nextTimeSlotUnassignedStations.Add(station);

            foreach (Models.TimeSlot timeSlot in sortedTimeSlots)
            {
                if (timeSlot.isGeneral)
                    continue;

                //all groups that did not take an activity pin last time slot are unassigned
                //and all groups are eligible to be assigned to the next time slot
                //all stations that are available at this time slot minus those that
                //are currently having an activity pin 
                unassignedGroups.Clear();
                unassignedStations.Clear();

                foreach (Models.Group group in nextTimeSlotUnassignedGroups)
                    unassignedGroups.Add(group);
                foreach (Models.Station station in nextTimeSlotUnassignedStations)
                    unassignedStations.Add(station);

                unassignedStations.Intersect(nextTimeSlotUnassignedStations);
                nextTimeSlotUnassignedGroups.Clear();
                nextTimeSlotUnassignedStations.Clear();
                foreach (Models.Group group in modelGroups)
                    nextTimeSlotUnassignedGroups.Add(group);
                foreach (Models.Station station in modelStations)
                    nextTimeSlotUnassignedStations.Add(station);

                unassignedGroups = Shuffle(unassignedGroups);
                for (int groupNum = 0; groupNum < unassignedGroups.Count; groupNum++)
                {
                    Models.Group group = unassignedGroups[groupNum];
                    int? greatestNumVisits = 0;

                    foreach (Models.Station station in unassignedStations)
                    {
                        int? currentNumVisits = groupStationVisitRange[group][station].numVisits;

                        if (currentNumVisits > greatestNumVisits)
                        {
                            greatestNumVisits = currentNumVisits;
                            eligibleStations.Clear();
                            eligibleStations.Add(station);
                        }

                        else if (currentNumVisits == greatestNumVisits && currentNumVisits > 0)
                        {
                            eligibleStations.Add(station);
                        } 
                    }

                    //don't assign activity pins to non-webelos
                    filterEligibleStations(group, ref eligibleStations);

                    //if the current group is still required to visit a station, then
                    //assign them to one of those stations
                    if (greatestNumVisits > 0 && eligibleStations.Count > 0)
                    {
                        if (assignStation(eligibleStations, group, timeSlot, sortedTimeSlots,
                            generatedSchedule))
                        {
                            groupNum--;
                        }
                    }
                }

                //for each group that was not required to visit one of the
                //remaining stations, assign them to their top station pick
                for (int groupNum = 0; groupNum < unassignedGroups.Count; groupNum++)
                {
                    Models.Group group = unassignedGroups[groupNum];
                    
                    bool foundTopPick = false;
                    for (int topStationPick = 0; topStationPick < 5; topStationPick++)
                    {
                        Models.Station topStation;
                        if (topStationPick == 0)
                            topStation = group.Preference1;
                        else if (topStationPick == 1)
                            topStation = group.Preference2;
                        else if (topStationPick == 2)
                            topStation = group.Preference3;
                        else if (topStationPick == 3)
                            topStation = group.Preference4;
                        else
                            topStation = group.Preference5;

                        if (topStation == null)
                            continue;

                        foreach (Models.Station station in unassignedStations)
                        {
                            //if the group has not already been given their preference and they
                            //are allowed to visit the station, then assign them to that station
                            if (topStation == station && !groupPreferenceGiven[group][topStationPick] &&
                                groupCanVisitStationAgain(group, station))
                            {
                                IList<Models.Station> eligibleStations =
                                    new List<Models.Station> { station };

                                //if the station is an activity pin and the group is not
                                //a webelos rank, remove the station
                                filterEligibleStations(group, ref eligibleStations);

                                if (station.isActivityPin && generateAssignment(
                                    group, timeSlot, sortedTimeSlots, eligibleStations, generatedSchedule))
                                {
                                    groupNum--;
                                    foundTopPick = true;
                                    break;
                                }

                                else if (generateAssignment(group, timeSlot,
                                    sortedTimeSlots, eligibleStations, generatedSchedule))
                                {
                                    groupNum--;
                                    foundTopPick = true;
                                    break;
                                }
                                
                            }
                        }
                        if (foundTopPick)
                            break;
                    }
                }

                //for each group that does not have a top station pick
                //remaining, assign them to their least assigned station
                for (int groupNum = 0; groupNum < unassignedGroups.Count; groupNum++)
                {
                    eligibleStations.Clear();
                    Models.Group group = unassignedGroups[groupNum];
                    int leastGroupAssignmentNum = 0;

                    if (unassignedStations.Count > 0)
                        leastGroupAssignmentNum = groupStationAssignments[group][unassignedStations[0]];

                    foreach (Models.Station station in unassignedStations)
                    {
                        int groupAssignmentNum = groupStationAssignments[group][station];

                        if (groupAssignmentNum == leastGroupAssignmentNum &&
                            groupCanVisitStationAgain(group, station))
                        {
                            eligibleStations.Add(station);
                        }

                        else if (groupAssignmentNum < leastGroupAssignmentNum &&
                            groupCanVisitStationAgain(group, station))
                        {
                            leastGroupAssignmentNum = groupAssignmentNum;
                            eligibleStations.Clear();
                            eligibleStations.Add(station);
                        }
                    }

                    //only assign activity pin stations to webelos
                    filterEligibleStations(group, ref eligibleStations);

                    if (assignStation(eligibleStations, group, timeSlot, sortedTimeSlots, generatedSchedule))
                    {
                        groupNum--;
                    }

                }

                //at this point, there is no criteria left to assign a group to one
                //station over another, so assign them to a station that they can visit again
                for (int groupNum = 0; groupNum < unassignedGroups.Count; groupNum++)
                {
                    Models.Group group = unassignedGroups[groupNum];

                    eligibleStations.Clear();
                    foreach (Models.Station station in unassignedStations)
                    {
                        if (groupCanVisitStationAgain(group, station))
                            eligibleStations.Add(station);
                    }

                    //only assign activity pins to webelos groups
                    filterEligibleStations(group, ref eligibleStations);
                    if (assignStation(eligibleStations, group, timeSlot, sortedTimeSlots, generatedSchedule))
                        groupNum--;
                }
            }
            return generatedSchedule;
        }

        //public IList<Models.Activity> generateMidWeekSchedule(IList<Models.Group> groups,
        //    IList<Models.Station> stations, IList<Models.SchedulingConstraint> constraints,
        //    IList<Models.TimeSlot> timeSlots, IList<Models.Activity> oldSchedule,
        //    Models.TimeSlot startingTimeSlot)
        //{
        //    return null;
        //}

        //prevent activity pins from being assigned to non-webelos ranks
        private static void filterEligibleStations(Models.Group currentGroup,
            ref IList<Models.Station> eligibleStations)
        {
            if (currentGroup.Type.ID != 4)
            {
                for (int stationNum = 0; stationNum < eligibleStations.Count; stationNum++)
                {
                    Models.Station station = eligibleStations[stationNum];
                    if (station.isActivityPin)
                    {
                        eligibleStations.Remove(station);
                        stationNum--;
                    }
                }
            }
        }

        private static IList<Models.Station> filterActivityPinStations
            (IList<Models.Station> eligibleStations, out IList<Models.Station> nonActivityPinStations)
        {
            nonActivityPinStations = new List<Models.Station>();
            List<Models.Station> activityPinStations = new List<Models.Station>();

            foreach (Models.Station station in eligibleStations)
                if (station.isActivityPin)
                    activityPinStations.Add(station);
                else
                    nonActivityPinStations.Add(station);

            return activityPinStations;
        }

        public static bool assignStation(IList<Models.Station> eligibleStations, Models.Group group,
            Models.TimeSlot timeSlot, IList<Models.TimeSlot> sortedTimeSlots,
            IList<Models.Activity> generatedSchedule)
        {
            IList<Models.Station> nonActivityPinStations = new List<Models.Station>();
            IList<Models.Station> activityPinStations =
                filterActivityPinStations(eligibleStations, out nonActivityPinStations);

            if (activityPinStations.Count > 0 && generateAssignment(
                group, timeSlot, sortedTimeSlots, activityPinStations, generatedSchedule))
            {
                return true;
            }

            else
            {
                return generateAssignment(group, timeSlot, sortedTimeSlots, nonActivityPinStations, generatedSchedule);
            }
        }

        public static bool generateAssignment(
            Models.Group assignedGroup, Models.TimeSlot timeSlot, IList<Models.TimeSlot> sortedTimeSlots, 
            IList<Models.Station> eligibleStations, IList<Models.Activity> generatedSchedule)
        {
            if (eligibleStations.Count > 0)
            {
                int stationNumber = random.Next(eligibleStations.Count);
                Models.Station assignedStation = eligibleStations[stationNumber];

                //we could be scheduling either an activity pin station or a regular station
                if (assignedStation.isActivityPin)
                {
                    int timeSlotIndex = sortedTimeSlots.IndexOf(timeSlot);
                    if (timeSlotIndex < 0 || sortedTimeSlots.Count <= (timeSlotIndex + 1))
                        return false;

                    int currentTimeSlotDayNum = sortedTimeSlots[timeSlotIndex].Start.DayOfYear;
                    int nextTimeSlotDayNum = sortedTimeSlots[timeSlotIndex + 1].Start.DayOfYear;
                    if (currentTimeSlotDayNum != nextTimeSlotDayNum ||
                        sortedTimeSlots[timeSlotIndex].isGeneral || sortedTimeSlots[timeSlotIndex + 1].isGeneral)
                    {
                        return false;
                    }

                    activityNumber++;
                    Models.Activity activity = new Models.Activity();
                    activity.ID = activityNumber;
                    activity.Group = assignedGroup;
                    activity.Station = assignedStation;
                    activity.TimeSlot = sortedTimeSlots[timeSlotIndex];
                    generatedSchedule.Add(activity);

                    Models.Activity activity1 = new Models.Activity();
                    activity1.ID = activityNumber;
                    activity1.Group = assignedGroup;
                    activity1.Station = assignedStation;
                    activity1.TimeSlot = sortedTimeSlots[timeSlotIndex + 1];
                    generatedSchedule.Add(activity1);

                    nextTimeSlotUnassignedStations.Remove(assignedStation);
                    nextTimeSlotUnassignedGroups.Remove(assignedGroup);
                }

                else
                {
                    activityNumber++;
                    Models.Activity activity = new Models.Activity();
                    activity.ID = activityNumber;
                    activity.Group = assignedGroup;
                    activity.Station = assignedStation;
                    activity.TimeSlot = timeSlot;
                    generatedSchedule.Add(activity);
                }

                if (assignedGroup.Preference1 == assignedStation)
                    groupPreferenceGiven[assignedGroup][0] = true;
                else if (assignedGroup.Preference2 == assignedStation)
                    groupPreferenceGiven[assignedGroup][1] = true;
                else if (assignedGroup.Preference3 == assignedStation)
                    groupPreferenceGiven[assignedGroup][2] = true;
                else if (assignedGroup.Preference4 == assignedStation)
                    groupPreferenceGiven[assignedGroup][3] = true;
                else if (assignedGroup.Preference5 == assignedStation)
                    groupPreferenceGiven[assignedGroup][4] = true;

                groupStationVisitRange[assignedGroup][assignedStation].decrementNumVisits();
                groupStationAssignments[assignedGroup][assignedStation]++;
                unassignedStations.Remove(assignedStation);
                unassignedGroups.Remove(assignedGroup);

                return true;
            }

            return false;
        }

        public static void InitializeData()
        {
            activityNumber = 0;
            groupPreferenceGiven.Clear();
            groupStationAssignments.Clear();
            groupStationVisitRange.Clear();
            unassignedGroups.Clear();
            unassignedStations.Clear();
            nextTimeSlotUnassignedGroups.Clear();
            nextTimeSlotUnassignedStations.Clear();
            eligibleStations.Clear();
            eligibleGroups.Clear();
        }

        private static void initializeGroupStationVisitRange(
            IEnumerable<Models.SchedulingConstraint> constraints, IEnumerable<Models.Group> groups,
            IEnumerable<Models.Station> stations,
            ref Dictionary<Models.Group, Dictionary<Models.Station, StationAssignmentRange>> groupStationVisitRange)
        {
            //first, initialize the groups so there are no constraints for all days of camp
            foreach (Models.Group group in groups)
            {
                Dictionary<Models.Station, StationAssignmentRange> groupAssignment =
                    new Dictionary<Models.Station, StationAssignmentRange>();

                foreach (Models.Station station in stations)
                {
                    groupAssignment.Add(station, new StationAssignmentRange(null));
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
                        (constraint.Group == null && constraint.GroupType != null &&
                        constraint.GroupType.ID == group.Type.ID) ||
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

        private static void initializeGroupPreferenceGiven(IEnumerable<Models.Group> groups,
            ref Dictionary<Models.Group, List<bool>> groupPreferenceGiven)
        {
            foreach (Models.Group group in groups)
            {
                List<bool> preferencesGiven = new List<bool> {false, false, false, false, false};
                groupPreferenceGiven.Add(group, preferencesGiven);
            }
        }

        private static bool groupCanVisitStationAgain(Models.Group group, Models.Station station)
        {
            return groupStationVisitRange[group][station].numVisits > 0 ||
                !groupStationVisitRange[group][station].numVisits.HasValue;
        }
    }
}
