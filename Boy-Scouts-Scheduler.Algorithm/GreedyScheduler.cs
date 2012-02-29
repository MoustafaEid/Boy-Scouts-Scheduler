using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Boy_Scouts_Scheduler.GreedyAlgorithm
{
	public class Group
	{
		public int ID;
		public string Name;
		public int Rank;
		public int StationPick1;
		public int StationPick2;
		public int StationPick3;

		public Group(int id, string N, int R, int S1 = -1, int S2 = -1, int S3 = -1)
		{
			ID = id;
			Name = N;
			Rank = R;
			StationPick1 = S1;
			StationPick2 = S2;
			StationPick3 = S3;
		}
	}

	public class Availability
	{
		public int DayNumber;
		public List<int> Slots;

		public Availability(int D, List<int> S)
		{
			DayNumber = D;
			Slots = S;
		}
	}

	public class Station
	{
		public int ID;
		public string Name;
		public int Capacity;
		public List<Availability> Avail;
		public int totalAvailabltSlots;

		public Station(int id, string N, int C, List<Availability> A)
		{
			ID = id;
			Name = N;
			Capacity = C;
			Avail = A;
			totalAvailabltSlots = 0;

			foreach (Availability x in A)
				totalAvailabltSlots += x.Slots.Count;

			totalAvailabltSlots *= Capacity;
		}
	}

	public class Assignment
	{
		public Group G;
		public Station S;

		public Assignment(Group g, Station s)
		{
			G = g;
			S = s;
		}
	}

	public class Constraint
	{
		public Group G;
		public Station S;
		public int minVisits;
		public int maxVisits;

		public Constraint(Group g, Station s, int minV, int maxV)
		{
			G = g;
			S = s;
			minVisits = minV;
			maxVisits = maxV;
		}
	}

	public static class GreedyScheduler
	{
		// is it okay to get any of the picks, or the first pick is more preferrable?
		private const int MAXN = 150;
		private const int CONSTRAINT_PENALTY = -10;
		private const int GETTING_SECOND_PICK_PENALTY = -20;
		private const int GETTING_THIRD_PICK_PENALTY = -25;
		private const int NOT_GETTING_ANY_PICKS_PENALTY = -30;

		private static int totalSlotsPerDay;
		private static List<Group> AllGroups;
		private static List<Station> AllStations;
		private static List<Constraint> AllConstraints;

		private static int[] GroupAssignments = new int[MAXN];
		// [GroupNumber, StationNumber] = Assignment Counter
		private static int[,] GroupStationAssignments = new int[MAXN, MAXN];
		private static int[,] GroupRankStationAssignments = new int[MAXN, MAXN];

		private static bool[] ConstraintMet = new bool[MAXN];

		// stations assignment counts
		// station day slot
		private static int[, ,] StationSlotAssignmentsCounts = new int[MAXN, MAXN, MAXN];
		private static int[] StationAssignmentsCounts = new int[MAXN];

		private static Dictionary<int, KeyValuePair<int, int>> timeSlotsDaySlotsPairs = new Dictionary<int, KeyValuePair<int, int>>();
		private static int[,] daySlotsTimeSlotsPairs = new int[MAXN, MAXN];

		private static void convertTimeSlotsToDaySlots(IEnumerable<Models.TimeSlot> T)
		{
			List<Availability> A = new List<Availability>();
			int i, j;

			int minDay = 1 << 30;

			foreach (Models.TimeSlot t in T)
			{
				int dayNumber = (int)t.Start.DayOfWeek;

				minDay = Math.Min(minDay, dayNumber);

				for (i = 0; i < A.Count; i++)
				{
					if (A[i].DayNumber == dayNumber)
					{
						A[i].Slots.Add(t.ID);
						break;
					}
				}

				if (i == A.Count)
					A.Add(new Availability(dayNumber, new List<int>(new int[] { t.ID })));
			}

			for (i = 0; i < A.Count; i++)
			{
				totalSlotsPerDay = Math.Max(totalSlotsPerDay, A[i].Slots.Count);

				for (j = 0; j < A[i].Slots.Count; j++)
				{
					int D = A[i].DayNumber - minDay + 1;
					int S = j + 1;
					timeSlotsDaySlotsPairs[A[i].Slots[j]] = new KeyValuePair<int, int>(D,S);
					daySlotsTimeSlotsPairs[D, S] = A[i].Slots[j];
				}
			}
		}

		private static KeyValuePair<int, int> timeSlotToDaySlot(Models.TimeSlot T)
		{
			return timeSlotsDaySlotsPairs[T.ID];
		}

		private static int daySlotToTimeSlot(int Day, int Slot)
		{
			return daySlotsTimeSlotsPairs[Day, Slot];
		}

		private static List<Availability> timeSlotsToAvailability(ICollection<Models.TimeSlot> T)
		{
			List<Availability> ret = new List<Availability>();
			int i;

			foreach (Models.TimeSlot t in T)
			{
				KeyValuePair<int, int> DS = timeSlotToDaySlot(t);

				for (i = 0; i < ret.Count; i++)
				{
					if (ret[i].DayNumber == DS.Key)
					{
						ret[i].Slots.Add(DS.Value);
						break;
					}
				}

				if (i == ret.Count)
				{
					ret.Add(new Availability(DS.Key, new List<int>(new int[] { DS.Value })));
				}
			}

			for (i = 0; i < ret.Count; i++)
			{
				ret[i].Slots.Sort();
			}

			return ret;
		}

		public static IEnumerable<Models.Activity> getSchedule(IEnumerable<Models.Group> groups, IEnumerable<Models.Station> stations, IEnumerable<Models.SchedulingConstraint> constraints, 
				IEnumerable<Models.TimeSlot> slots, IEnumerable<Models.Activity> oldSchedule, Models.TimeSlot startingTimeSlot)
		{
			List<Models.Activity> schedule = new List<Models.Activity>();

			List<Group> G = new List<Group>();
			List<Station> S = new List<Station>();
			List<Constraint> C = new List<Constraint>();

			int i,j,k;

			convertTimeSlotsToDaySlots(slots);

			List<Models.Group> tmpallGroups = new List<Models.Group>(groups.ToArray());
			List<Models.Station> tmpallStations = new List<Models.Station>(stations.ToArray());
			List<Models.TimeSlot> tmpallSlots = new List<Models.TimeSlot>(slots.ToArray());

			foreach (Models.Station s in stations)
				S.Add(new Station(s.ID, s.Name, s.Capacity, timeSlotsToAvailability(s.AvailableTimeSlots)));

			foreach (Models.Group x in groups)
			{
				int p1 = -1, p2 = -1, p3 = -1;

				for (i = 0; i < S.Count; i++)
				{
					if (x.Preference1 != null && x.Preference1.ID == S[i].ID) p1 = i;
					if (x.Preference2 != null && x.Preference2.ID == S[i].ID) p2 = i;
					if (x.Preference3 != null && x.Preference3.ID == S[i].ID) p3 = i;
				}

				G.Add(new Group(x.ID, x.Name, x.Type.ID, p1, p2, p3));
			}

			foreach (Models.SchedulingConstraint c in constraints)
			{
				// TODO: group ranks in constraints
				if (c.Group == null)
					continue;

				Group g = G[0];
				Station s = S[0];

				for (i = 0; i < G.Count; i++) if (G[i].ID == c.Group.ID) g = G[i];
				for (i = 0; i < S.Count; i++) if (S[i].ID == c.Station.ID) s = S[i];


				// int? vs int
				C.Add(new Constraint(g, s, (int)c.MinVisits, (int)c.MaxVisits));
			}

			Dictionary<int, int>[,] masterSchedule = Schedule(G, S, C);

			for (i = 1; i <= 5; i++)
			{
				for (j = 1; j <= totalSlotsPerDay; j++)
				{
					int slotID = daySlotToTimeSlot(i, j);
					Models.TimeSlot T = new Models.TimeSlot();

					foreach (Models.TimeSlot t in slots)
						if (t.ID == slotID)
							T = t;

					foreach (KeyValuePair<int, int> P in masterSchedule[i, j])
					{
						Models.Activity A = new Models.Activity();

						A.Group = tmpallGroups[P.Key];
						A.Station = tmpallStations[P.Value];

						A.TimeSlot = T;

						schedule.Add(A);
					}
				}
			}

			return schedule;
		}

		private static Dictionary<int, int>[,] Schedule(List<Group> groups, List<Station> stations, List<Constraint> Constraints)
		{
			// start monday end Friday
			int dayStart = 1, dayEnd = 5;
			int Day, Slot, i, j, k;

			AllStations = stations;
			AllGroups = groups;
			AllConstraints = Constraints;
			
			// Schedule
			Dictionary<int, int>[,] masterSchedule = new Dictionary<int, int>[10, 20];

			for (i = 0; i < MAXN; i++)
				for (j = 0; j < MAXN; j++)
				{
					GroupStationAssignments[i, j] = GroupRankStationAssignments[i, j] = GroupAssignments[i] = StationAssignmentsCounts[i] = 0;

					for (k = 0; k < MAXN; k++)
						StationSlotAssignmentsCounts[i, j, k] = 0;

					ConstraintMet[i] = false;
				}


			for (Day = dayStart; Day <= dayEnd; Day++)
			{
				for (Slot = 1; Slot <= totalSlotsPerDay; Slot++)
				{
					masterSchedule[Day, Slot] = new Dictionary<int, int>();

					// Group busy
					bool[] isGroupBusy = new bool[100];

					// keep looking for assignments for this slot
					while (true)
					{
						int groupSelected = -1;
						int stationSelected = -1;
						int maxScore = -1 << 30;
						int minStationAssignment = 1 << 30;
						int constraintIndex = -1;
						int s;

						for (i = 0; i < stations.Count; i++)
						{
							Station curStation = stations[i];

							if (!isStationAvailableAtSlot(curStation, Day, Slot) || StationSlotAssignmentsCounts[i, Day, Slot] >= curStation.Capacity)
								continue;

							// do constraints first

							for (j = 0; j < Constraints.Count; j++)
							{
								if (ConstraintMet[j] || Constraints[j].S != curStation)
									continue;

								groupSelected = getGroupIndex(Constraints[j].G);

								if (isGroupBusy[groupSelected] || !canHappenGroupStationAssignment(groupSelected, i) || !canHappenGroupRankStationAssignment(Constraints[j].G.Rank, i))
									continue;

								stationSelected = i;
								constraintIndex = j;
								break;
							}

							if (constraintIndex != -1)
								break;

							for (j = 0; j < groups.Count; j++)
							{
								int groupNum = getNextLeastAssignedGroup();
								//int groupNum = j;
								Group curGroup = groups[groupNum];

								if (isGroupBusy[groupNum] || !canHappenGroupStationAssignment(groupNum, i) || !canHappenGroupRankStationAssignment(curGroup.Rank, i))
									continue;

								s = score(masterSchedule, curStation, curGroup, Day, Slot);

								if (s > maxScore || s == maxScore && StationAssignmentsCounts[i] < minStationAssignment)
								{
									maxScore = s;
									groupSelected = groupNum;
									stationSelected = i;
									minStationAssignment = StationAssignmentsCounts[i];
								}
							}
						}

						if (groupSelected == -1)
							break;

						// Group is busy in this slot
						isGroupBusy[groupSelected] = true;
						// Increment group activites
						GroupAssignments[groupSelected]++;
						// Assign the Group to the station
						GroupStationAssignments[groupSelected, stationSelected]++;
						// Assign the Group's rank to the station
						GroupRankStationAssignments[AllGroups[groupSelected].Rank, stationSelected]++;

						StationSlotAssignmentsCounts[stationSelected, Day, Slot]++;
						StationAssignmentsCounts[stationSelected]++;

						if (constraintIndex != -1)
						{
							if (GroupStationAssignments[groupSelected, stationSelected] == Constraints[constraintIndex].maxVisits)
								ConstraintMet[constraintIndex] = true;
						}

						masterSchedule[Day, Slot].Add(groupSelected, stationSelected);
					}
				}
			}

			return masterSchedule;
		}

		private static int getNextLeastAssignedGroup()
		{
			int i;
			int min = 1 << 30;
			int index = -1;

			for (i = 0; i < AllGroups.Count; i++)
			{
				if (GroupAssignments[i] < min)
				{
					min = GroupAssignments[i];
					index = i;
				}
			}

			return index;
		}

		private static bool canHappenGroupStationAssignment(int groupID, int stationID)
		{
			int i;

			for (i = 0; i < AllConstraints.Count; i++)
			{
				if (AllConstraints[i].G == AllGroups[groupID] && AllConstraints[i].S == AllStations[stationID])
					return GroupStationAssignments[groupID, stationID] < AllConstraints[i].maxVisits;
			}

			return GroupStationAssignments[groupID, stationID] <= 2;
		}

		private static bool canHappenGroupRankStationAssignment(int groupRank, int stationID)
		{
			return GroupRankStationAssignments[groupRank, stationID] <= 2000;
		}

		private static int getStationIndex(Station s)
		{
			int i;

			for (i = 0; i < AllStations.Count; i++)
			{
				if (AllStations[i].ID == s.ID)
					return i;
			}

			return -1;
		}

		private static int getGroupIndex(Group g)
		{
			int i;

			for (i = 0; i < AllGroups.Count; i++)
			{
				if (AllStations[i].ID == g.ID)
					return i;
			}

			return -1;
		}
		private static int score(Dictionary<int, int>[,] masterSchedule, Station S, Group G, int Day, int Slot)
		{
			int ret = 0;

			int i, j;

			// check if other constraints will be violated if this assignment happens.

			// station cap - 1 for the current assigmment
			int stationIndex = getStationIndex(S);
			int stationTotalAvailableSlotsLeft = S.totalAvailabltSlots - StationAssignmentsCounts[stationIndex] - 1;

			int otherGroupsNeedThisStation = 0;

			if (stationTotalAvailableSlotsLeft < 0)
				throw new Exception("Error generating schedule. SC-1");

			// look for anyone else who wants this station.
			for (i = 0; i < AllConstraints.Count; i++)
			{
				if (ConstraintMet[i] || AllConstraints[i].S != S || AllConstraints[i].G == G)
					continue;

				otherGroupsNeedThisStation++;
			}

			if (otherGroupsNeedThisStation > stationTotalAvailableSlotsLeft)
			{
				ret += CONSTRAINT_PENALTY * (otherGroupsNeedThisStation - stationTotalAvailableSlotsLeft);
			}

			int nFirstPicks = 0;
			int nSecondPicks = 0;
			int nThirdPicks = 0;
			int nNoPicks = 0;

			// Check how many groups get their second pick instead of first, and how many groups aren't getting any picks

			// Copy the total assignment count for stations.
			int[] StationAssignmentsCountsTemp = (int[])StationAssignmentsCounts.Clone();
			StationAssignmentsCountsTemp[stationIndex]++;

			int stationPick1AvailableSlots, stationPick2AvailableSlots, stationPick3AvailableSlots;

			for (i = 0; i < AllGroups.Count; i++)
			{
				if (AllGroups[i].StationPick1 == -1)
					continue;

				stationPick1AvailableSlots = AllGroups[i].StationPick1 == -1 ? 0 : AllStations[AllGroups[i].StationPick1].totalAvailabltSlots - StationAssignmentsCountsTemp[AllGroups[i].StationPick1];
				stationPick2AvailableSlots = AllGroups[i].StationPick2 == -1 ? 0 : AllStations[AllGroups[i].StationPick2].totalAvailabltSlots - StationAssignmentsCountsTemp[AllGroups[i].StationPick2];
				stationPick3AvailableSlots = AllGroups[i].StationPick3 == -1 ? 0 : AllStations[AllGroups[i].StationPick3].totalAvailabltSlots - StationAssignmentsCountsTemp[AllGroups[i].StationPick3];

				if (AllGroups[i].StationPick1 != -1 && stationPick1AvailableSlots > 0)
				{
					StationAssignmentsCountsTemp[AllGroups[i].StationPick1]++;
					nFirstPicks++;
				}
				else if (AllGroups[i].StationPick1 != -1 && stationPick2AvailableSlots > 0)
				{
					StationAssignmentsCountsTemp[AllGroups[i].StationPick2]++;
					nSecondPicks++;
				}
				else if (AllGroups[i].StationPick1 != -1 && stationPick3AvailableSlots > 0)
				{
					StationAssignmentsCountsTemp[AllGroups[i].StationPick3]++;
					nThirdPicks++;
				}
				else
					nNoPicks++;
			}

			ret += GETTING_SECOND_PICK_PENALTY * nSecondPicks;
			ret += GETTING_THIRD_PICK_PENALTY * nThirdPicks;
			ret += NOT_GETTING_ANY_PICKS_PENALTY * nNoPicks;

			return ret;
		}

		private static int getStationTotalCapacityDuringWeek(Station s, int Day, int Slot)
		{
			int ret = 0;

			int i, j;

			for (i = 0; i < s.Avail.Count; i++)
			{
				if (s.Avail[i].DayNumber == Day)
				{
					for (j = 0; j < s.Avail[i].Slots.Count; j++)
						if (s.Avail[i].Slots[j] > Slot)
							ret++;
				}
				else if (s.Avail[i].DayNumber > Day)
				{
					ret += s.Avail[i].Slots.Count;
				}
			}

			ret *= s.Capacity;

			return ret;
		}

		private static bool isStationAvailableAtSlot(Station station, int Day, int Slot)
		{
			int i, j;

			if (station.Capacity <= 0)
				return false;

			for (i = 0; i < station.Avail.Count; i++)
			{
				if (station.Avail[i].DayNumber == Day)
				{
					if (station.Avail[i].Slots.IndexOf(Slot) != -1)
						return true;
				}
			}

			return false;
		}
	}
}
