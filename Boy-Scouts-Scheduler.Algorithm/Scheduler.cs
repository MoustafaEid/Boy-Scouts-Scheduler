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
			IEnumerable<Models.Activity> Greedy = new List<Models.Activity>();
			IEnumerable<Models.Activity> HillClimbing = new List<Models.Activity>();

			long GreedyScore = 0;
			long HillClimbingScore = 0;

			Greedy = GreedyAlgorithm.GreedyScheduler.getSchedule(groups, stations, constraints, slots, oldSchedule, startingTimeSlot);

			// A schedule needs to be generated from scratch
			if (slots.First().ID == startingTimeSlot.ID)
			{
				HillClimbing = HillClimbingAlgorithm.GenerateSchedule(groups, stations, constraints, slots);

				GreedyScore = Score.ScoreSchedule(Greedy, groups, stations, constraints, slots);
				HillClimbingScore = Score.ScoreSchedule(HillClimbing, groups, stations, constraints, slots);

				// compare scores
				if (GreedyScore > HillClimbingScore)
				    return Greedy;

				return HillClimbing;
			}

			return Greedy;
		}
	}
}
