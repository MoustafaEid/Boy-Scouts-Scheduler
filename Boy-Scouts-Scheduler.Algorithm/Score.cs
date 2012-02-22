using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Boy_Scouts_Scheduling_Algorithm_MD
{
    //this is a utility class that scores the generated schedules based on certain criteria
    public static class Score
    {
        public static uint ScoreSchedule(
            List<List<Dictionary<Group, Station>>> schedule, IList<Group> groups,
            Dictionary<Group, Dictionary<Station, HillClimbingScheduler.StationAssignmentRange>> groupStationVisitRange)
        {
            return ScoreTopPicks(schedule, groups) - (GetNumConstraintsViolated(groupStationVisitRange) * 10);
        }

        public static uint ScoreTopPicks(List<List<Dictionary<Group, Station>>> schedule , IList<Group> groups)
        {
            uint[] scores = new uint[5] { 20, 7, 4, 2, 1 };
            uint score = 0;
            Station assignedStation;

            //groups get a score based on which of their top picks get assigned to them
            foreach (var day in schedule)
            {
                foreach (var stationNum in day)
                {
                    foreach (var assignment in stationNum)
                    {
                        //for each assignment in the master schedule, see what group was assigned
                        //and check if their assigned station was listed in their top picks
                        for (int groupNum = 0; groupNum < groups.Count; groupNum++)
                        {
                            Group group = groups[groupNum];
                            if (assignment.Key.Name == group.Name)
                            {
                                assignedStation = assignment.Value;
                                for (int topPickNum = 0; topPickNum < group.TopStationPicks.Count; topPickNum++)
                                {
                                    if (group.TopStationPicks[topPickNum] == assignedStation)
                                    {
                                        group.TopStationPicks[topPickNum] = null;
                                        score += scores[topPickNum];
                                    }
                                }
                            }
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
        public static uint GetNumConstraintsViolated(Dictionary<Group,
            Dictionary<Station, HillClimbingScheduler.StationAssignmentRange>> groupStationVisitRange)
        {
            uint[] penalties = new uint[5] {0,1,3,6,10};
            uint score = 0;
            foreach (var pair in groupStationVisitRange)
                foreach (var pair1 in pair.Value)
                    if (pair1.Value.minVisits.HasValue)
                        score += penalties[(int)pair1.Value.minVisits];
            return score;
        }

        public static uint GetDistanceScore(List<List<Dictionary<Group, Station>>> schedule, IList<Group> groups)
        {
            return 0;
        }
    }
}
