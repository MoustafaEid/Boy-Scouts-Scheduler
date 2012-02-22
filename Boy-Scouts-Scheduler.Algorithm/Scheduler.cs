using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Boy_Scouts_Scheduler.Algorithm
{
	class Scheduler
	{
		public static Dictionary<int, int>[,] Schedule(List<Group> groups, List<Station> stations, List<Constraint> Constraints, int slotsPerDay)
		{
			return GreedyScheduler.Schedule(groups, stations, Constraints, slotsPerDay);
		}
	}
}
