using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boy_Scouts_Scheduler.Algorithm
{
    public static class Score
    {
        public static uint ScoreSchedule(
           IEnumerable<Models.Activity> schedule, IEnumerable<Models.Group> groups,
           IEnumerable<Models.Station> stations, IEnumerable<Models.SchedulingConstraint> constraints,
           IEnumerable<Models.TimeSlot> timeSlots)
        {
            return ScoreTopPicks(schedule, groups) - (GetNumConstraintsViolated(schedule, constraints) * 20);
        }

        public static uint ScoreTopPicks(IEnumerable<Models.Activity> schedule, IEnumerable<Models.Group> groups)
        {
            uint[] scores = new uint[3] { 20, 7, 4 };
            uint score = 0;

            foreach (Models.Group group in groups)
            {
                for (int lcv = 0; lcv < 3; lcv++)
                {
                    Models.Station preference = new Models.Station();
                    if (lcv == 0)
                        preference = group.Preference1;
                    else if (lcv == 1)
                        preference = group.Preference2;
                    else if (lcv == 2)
                        preference = group.Preference3;

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
        public static uint GetNumConstraintsViolated(IEnumerable<Models.Activity> schedule,
            IEnumerable<Models.SchedulingConstraint> constraints)
        {
            uint[] penalties = new uint[5] { 0, 1, 3, 6, 10 };
            uint numViolatedConstraints = 0;

            foreach (Models.SchedulingConstraint constraint in constraints)
            {
                int? minVisits = constraint.MinVisits;
                int? maxVisits = constraint.MaxVisits;
                foreach (Models.Activity activity in schedule)
                {
                    bool constraintAppliesToGroup =
                        (constraint.Group == null && constraint.GroupType == null) ||
                        (constraint.Group == null && constraint.GroupType == activity.Group.Type) ||
                        (constraint.Group != null && constraint.Group == activity.Group);

                    bool isCorrectStation = constraint.Station == activity.Station;

                    if (constraintAppliesToGroup && isCorrectStation)
                    {
                        if (minVisits.HasValue)
                            minVisits--;
                        if (maxVisits.HasValue)
                            maxVisits--;
                    }
                }
                if (minVisits > 0 || maxVisits < 0)
                    numViolatedConstraints++;

            }
            return numViolatedConstraints;
        }

        public static int getNumConstraintsViolated(
            Dictionary<Models.Group, Dictionary<Models.Station,
            HillClimbingAlgorithm.StationAssignmentRange>> groupStationVisitRange)
        {
            int numConstraintsViolated = 0;
            foreach (var group in groupStationVisitRange)
            {
                foreach (var station in group.Value)
                {
                    if (station.Value.minVisits > 0 || station.Value.maxVisits < 0)
                        numConstraintsViolated++;
                }
            }
            return numConstraintsViolated;
        }
    }
}
