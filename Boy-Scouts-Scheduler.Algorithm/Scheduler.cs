using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Boy_Scouts_Scheduler.Algorithm
{
	public class Scheduler
	{
        public static IEnumerable<Models.Activity> Schedule(IEnumerable<Models.Group> groups, IEnumerable<Models.Station> stations, IEnumerable<Models.SchedulingConstraint> constraints, 
				IEnumerable<Models.TimeSlot> slots, IEnumerable<Models.Activity> oldSchedule, Models.TimeSlot startingTimeSlot)
		{
            IEnumerable<Models.Activity> Greedy = GreedyAlgorithm.GreedyScheduler.getSchedule(groups, stations, constraints, slots, oldSchedule, startingTimeSlot);
			//IEnumerable<Models.Activity> HillClimbing = HillClimbingAlgorithm.GenerateSchedule(groups, stations, constraints, slots, oldSchedule, startingTimeSlot);

            // compare scores

            //int GreedyScore = 0;
            //int HillClimbingScore = 0;

            //if (GreedyScore > HillClimbingScore)
            //    return Greedy;

            //return HillClimbing;
			return Greedy;
		}
	}
}
