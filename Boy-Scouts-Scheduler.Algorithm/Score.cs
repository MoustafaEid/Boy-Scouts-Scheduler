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
                + deductStationRevisitedOnSameDayScore(schedule, groups, stations, timeSlots) +
                scoreScheduleDiversity(schedule);
        }

        public static uint ScoreTopPicks(IEnumerable<Models.Activity> schedule, IEnumerable<Models.Group> groups)
        {
            uint[] scores = new uint[5] {100, 70, 50, 35, 23};
            uint score = 0;

            foreach (Models.Group group in groups)
            {
                uint numGroupPicks = 0;
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
                            numGroupPicks++;
                            score += scores[lcv];
                            break;
                        }
                    }
                }

                //a group is only supposed to receive three out of their top five station picks
                if (numGroupPicks == 3)
                {
                    score += 200;
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

                //it is considered worse to undervisit a station twice instead of once
                //in that case, we act as if two constraints were violated
                //note that if numVisits == 0, then there is no penalty, since
                //the group visited the station the correct number of times
                numViolatedConstraints += Math.Abs(numVisits);

            }
            return (numViolatedConstraints * -500);
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
                    //must check if numVisits is not equal to zero or null here
                    if (station.Value.numVisits > 0 || station.Value.numVisits < 0)
                        numConstraintsViolated += Math.Abs((int)station.Value.numVisits);
                }
            }
            return (numConstraintsViolated * -500);
        }

        public static int deductStationRevisitedOnSameDayScore(
            IEnumerable<Models.Activity> schedule, IEnumerable<Models.Group> groups,
            IEnumerable<Models.Station> stations, IEnumerable<Models.TimeSlot> timeSlots)
        {
            DateTime a = DateTime.Now;
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

            DateTime b = DateTime.Now;
            TimeSpan c = b.Subtract(a);
            return (revisitedPenalty * -90);
        }

        //attempts to give a higher score to schedule that are more diverse
        //diverse means that if the same activity is assigned to the same group, then
        //those activities are spread out as far as possible
        //maximum score is 1000 points
        public static int scoreScheduleDiversity(IEnumerable<Models.Activity> schedule)
        {
            double score = 0;
            double maxScore = 0;

            var sortedSchedule =
                from activity in schedule
                orderby activity.Group.Name ascending, activity.TimeSlot.Start ascending
                select activity;

            IDictionary<Models.Group, List<Models.Activity>> groupSchedules =
                new Dictionary<Models.Group, List<Models.Activity>>();

            //create a dictionary with the groups mapping to their schedules
            foreach (Models.Activity activity in sortedSchedule)
            {
                if (groupSchedules.ContainsKey(activity.Group))
                {
                    groupSchedules[activity.Group].Add(activity);
                }
                else
                {
                    List<Models.Activity> activities = new List<Models.Activity> { activity };
                    groupSchedules.Add(activity.Group, activities);
                }
            }

            foreach (Models.Group group in groupSchedules.Keys)
            {
                maxScore += groupSchedules[group].Count() * groupSchedules[group].Count() * 2;
                bool[] isVisited = new bool[groupSchedules[group].Count()];
                for (int startingActivityNum = 0;  startingActivityNum < groupSchedules[group].Count;
                    startingActivityNum++)
                {
                    if (isVisited[startingActivityNum])
                        continue;

                    bool matchFound = false;
                    for (int nextActivityNum = startingActivityNum + 1; nextActivityNum < groupSchedules[group].Count;
                        nextActivityNum++)
                    {
                        if (groupSchedules[group][startingActivityNum].Station.isActivityPin)
                        {
                            nextActivityNum++;
                            if (nextActivityNum >= groupSchedules[group].Count)
                                break;
                        }

                        if (groupSchedules[group][startingActivityNum].Station.Name ==
                            groupSchedules[group][nextActivityNum].Station.Name)
                        {
                            matchFound = true;
                            isVisited[nextActivityNum] = true;
                            score += nextActivityNum - startingActivityNum;
                        }
                    }
                    if (!matchFound)
                        score += groupSchedules[group].Count * 2;
                }
             
            }

            if (score == 0 || maxScore == 0)
                return 0;

            double scoreRatio = score / maxScore;
            return (int) (scoreRatio * 1000);
        }

    }
}
