using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Boy_Scouts_Scheduler.Algorithm
{
	class Scheduler
	{
		public static List<Models.Activity> Schedule(List<Models.Group> groups, List<Models.Station> stations, List<Models.SchedulingConstraint> constraints, List<Models.Group> slots)
		{
			return GreedyAlgorithm.GreedyScheduler.getSchedule(groups, stations, constraints, slots);
		}
	}
}
