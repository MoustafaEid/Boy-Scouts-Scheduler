﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Boy_Scouts_Scheduler.Algorithm
{
	class Scheduler
	{
        public static IEnumerable<Models.Activity> Schedule(IEnumerable<Models.Group> groups, IEnumerable<Models.Station> stations, IEnumerable<Models.SchedulingConstraint> constraints, IEnumerable<Models.TimeSlot> slots)
		{
			return GreedyAlgorithm.GreedyScheduler.getSchedule(groups, stations, constraints, slots);
		}
	}
}
