using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boy_Scouts_Scheduler.Algorithm
{
    public static class Score
    {
        public static long ScoreSchedule(
           IEnumerable<Models.Activity> schedule, IEnumerable<Models.Group> groups,
           IEnumerable<Models.Station> stations, IEnumerable<Models.SchedulingConstraint> constraints,
           IEnumerable<Models.TimeSlot> timeSlots)
        {
            return ScoreTopPicks(schedule, groups) + deductConstraintsViolatedScore(schedule, constraints)
                + deductStationRevisitedOnSameDayScore(schedule, groups, stations, timeSlots);
        }

        public static uint ScoreTopPicks(IEnumerable<Models.Activity> schedule, IEnumerable<Models.Group> groups)
        {
            uint[] scores = new uint[5] { 20, 7, 4, 2, 1 };
            uint score = 0;

            foreach (Models.Group group in groups)
            {
                for (int lcv = 0; lcv < scores.Length; lcv++)
                {
                    Models.Station preference = new Models.Station();
                    if (lcv == 0)
                        preference = group.Preference1;
                    else if (lcv == 1)
                        preference = group.Preference2;
                    else if (lcv == 2)
                        preference = group.Preference3;
                    else if (lcv == 3)
                        preference = group.Preference4;
                    else if (lcv == 4)
                        preference = group.Preference5;

                    foreach (Models.Activity activity in schedule)
                    {
                        if (activity.Group == group && activity.Station == preference)
                        {
                            score += scores[lcv];
                            break;
                        }
                    }
                }
            }

            return score;
        }

        /*
         * Returns a weighted score based on the number of constraints violated.
         * Different violations are weighted differently -- if a group was underassigned
         * to a station twice, then the penalty is much greater than if they were
         * underassigned to that station only once
         */
        public static int deductConstraintsViolatedScore(IEnumerable<Models.Activity> schedule,
            IEnumerable<Models.SchedulingConstraint> constraints)
        {
            int numViolatedConstraints = 0;

            foreach (Models.SchedulingConstraint constraint in constraints)
            {
                int numVisits = constraint.VisitNum;
                foreach (Models.Activity activity in schedule)
                {
                    bool constraintAppliesToGroup =
                        (constraint.Group == null && constraint.GroupType == null) ||
                        (constraint.Group == null && constraint.GroupType == activity.Group.Type) ||
                        (constraint.Group != null && constraint.Group == activity.Group);

                    bool isCorrectStation = constraint.Station == activity.Station;

                    if (constraintAppliesToGroup && isCorrectStation)
                    {
                        numVisits--;
                    }
                }
                if (numVisits != 0)
                {
                    numViolatedConstraints++;
                }

            }
            return (numViolatedConstraints * -20);
        }

        public static int deductConstraintsViolatedScore(
            Dictionary<Models.Group, Dictionary<Models.Station,
            HillClimbingAlgorithm.StationAssignmentRange>> groupStationVisitRange)
        {
            int numConstraintsViolated = 0;
            foreach (var group in groupStationVisitRange)
            {
                foreach (var station in group.Value)
                {
                    if (station.Value.numVisits > 0 || station.Value.numVisits < 0)
                        numConstraintsViolated++;
                }
            }
            return (numConstraintsViolated * -20);
        }

        public static int deductStationRevisitedOnSameDayScore(
            IEnumerable<Models.Activity> schedule, IEnumerable<Models.Group> groups,
            IEnumerable<Models.Station> stations, IEnumerable<Models.TimeSlot> timeSlots)
        {
            int revisitedPenalty = 0;

            //assignments for station categories on each day of camp
            //example: on day 3, Group "Knights1" has been assigned to
            //category "arts and crafts" twice and category "shooting" once
            IDictionary<int, Dictionary<Models.Group, Dictionary<string, int>>> dailyCategoryAssignments =
                new Dictionary<int, Dictionary<Models.Group, Dictionary<string, int>>>();

            //assignments for stations on each day of camp
            //example: on day 2, Group "Knights2" has been assigned to station 
            IDictionary<int, Dictionary<Models.Group, Dictionary<Models.Station, int>>> dailyStationAssignments =
                new Dictionary<int, Dictionary<Models.Group, Dictionary<Models.Station, int>>>();

            foreach (Models.Activity activity in schedule)
            {
                Models.Group group = activity.Group;
                Models.Station station = activity.Station;
                string stationCategory = activity.Station.Category;

                if (stationCategory == null)
                    continue;

                int dayNum = activity.TimeSlot.Start.DayOfYear;

                //check if the same station category has already been assigned on the same day
                if (dailyCategoryAssignments.ContainsKey(dayNum))
                {
                    if (dailyCategoryAssignments[dayNum].ContainsKey(group))
                    {
                        if (dailyCategoryAssignments[dayNum][group].ContainsKey(stationCategory))
                        {
                            int numVisits = dailyCategoryAssignments[dayNum][group][stationCategory];
                            revisitedPenalty += numVisits;
                            dailyCategoryAssignments[dayNum][group][stationCategory]++;
                        }
                        else
                        {
                            dailyCategoryAssignments[dayNum][group].Add(stationCategory, 1);
                        }
                    }

                    else
                    {
                        Dictionary<string, int> stationAssignmentValue =
                           new Dictionary<string, int>();
                        stationAssignmentValue.Add(stationCategory, 1);
                        dailyCategoryAssignments[dayNum].Add(group, stationAssignmentValue);
                    }
                }

                else
                {
                    Dictionary<Models.Group, Dictionary<string, int>> groupAssignments =
                        new Dictionary<Models.Group, Dictionary<string, int>>();
                    Dictionary<string, int> stationAssignmentValue =
                        new Dictionary<string, int>();

                    stationAssignmentValue.Add(stationCategory, 1);
                    groupAssignments.Add(group, stationAssignmentValue);
                    dailyCategoryAssignments.Add(dayNum, groupAssignments);
                }

                //check if the current station has already been assigned on the same day
                if (dailyStationAssignments.ContainsKey(dayNum))
                {
                    if (dailyStationAssignments[dayNum].ContainsKey(group))
                    {
                        if (dailyStationAssignments[dayNum][group].ContainsKey(station))
                        {
                            int numVisits = dailyStationAssignments[dayNum][group][station];
                            revisitedPenalty += numVisits;
                            dailyStationAssignments[dayNum][group][station]++;
                        }
                        else
                        {
                            dailyStationAssignments[dayNum][group].Add(station, 1);
                        }
                    }

                    else
                    {
                        Dictionary<Models.Station, int> stationAssignmentValue =
                           new Dictionary<Models.Station, int>();
                        stationAssignmentValue.Add(station, 1);
                        dailyStationAssignments[dayNum].Add(group, stationAssignmentValue);
                    }
                }

                else
                {
                    Dictionary<Models.Group, Dictionary<Models.Station, int>> groupAssignments =
                        new Dictionary<Models.Group, Dictionary<Models.Station, int>>();
                    Dictionary<Models.Station, int> stationAssignmentValue =
                        new Dictionary<Models.Station, int>();

                    stationAssignmentValue.Add(station, 1);
                    groupAssignments.Add(group, stationAssignmentValue);
                    dailyStationAssignments.Add(dayNum, groupAssignments);
                }

            }
            return (revisitedPenalty * -10);
        }
    }
}
