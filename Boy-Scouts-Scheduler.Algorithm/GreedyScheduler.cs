using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Boy_Scouts_Scheduler.Algorithm
{
	public class Group
	{
		public string Name;
		public int Rank;
		public int StationPick1;
		public int StationPick2;
		public int StationPick3;

		public Group(string N, int R, int S1 = -1, int S2 = -1, int S3 = -1)
		{
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
		public string Name;
		public int Capacity;
		public List<Availability> Avail;
		public int totalAvailabltSlots;

		public Station(string N, int C, List<Availability> A)
		{
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
		public int nTimes;

		public Constraint(Group g, Station s, int n)
		{
			G = g;
			S = s;
			nTimes = n;
		}
	}

	public class TimeSlot
	{
		public List<Assignment> Assignments;

		public TimeSlot()
		{
			Assignments = new List<Assignment>();
		}
	}

	public class ScheduleStatus
	{
		public List<Group> FreeGroups;
		public int ConstraintsMet;
	}

	public class Schedule
	{
		public List<TimeSlot> Monday;
		public List<TimeSlot> Wednesday;
		public List<TimeSlot> Tuesday;
		public List<TimeSlot> Thursday;
		public List<TimeSlot> Friday;
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
		
		public static Dictionary<int, int>[,] Schedule(List<Group> groups, List<Station> stations, List<Constraint> Constraints, int slotsPerDay)
		{
			// start monday end Friday
			int dayStart = 1, dayEnd = 5;
			int Day, Slot, i,j, k;

			totalSlotsPerDay = slotsPerDay;
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
				for (Slot = 1; Slot <= slotsPerDay; Slot++)
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

						for (i = 0; i < stations.Count; i++)
						{
							Station curStation = stations[i];

							if (!isStationAvailableAtSlot(curStation, Day, Slot) || StationSlotAssignmentsCounts[i, Day,Slot] >= curStation.Capacity)
								continue;

							for (j = 0; j < groups.Count; j++)
							{
								int groupNum = getNextLeastAssignedGroup();
								//int groupNum = j;
								Group curGroup = groups[groupNum];

								if (isGroupBusy[groupNum] || !canHappenGroupStationAssignment(groupNum, i) || !canHappenGroupRankStationAssignment(curGroup.Rank, i))
									continue;

								int s = score(masterSchedule, curStation, curGroup, Day, Slot);

								if (s > maxScore)
								{
									maxScore = s;
									groupSelected = groupNum;
									stationSelected = i;
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
						GroupRankStationAssignments[ AllGroups[groupSelected].Rank, stationSelected]++;

						StationSlotAssignmentsCounts[stationSelected, Day, Slot]++;
						StationAssignmentsCounts[stationSelected]++;

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
			return GroupStationAssignments[ groupID, stationID ] <= 400;
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
				if (AllStations[i].Name == s.Name)
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
				if( AllGroups[i].StationPick1 == -1 )
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


		private static ScheduleStatus getScheduleStatus(Dictionary<int, int>[,] schedule)
		{
			ScheduleStatus ret = new ScheduleStatus();
			ret.FreeGroups = new List<Group>();

			int Day, Slot;
			int i;

			HashSet<int> groups = new HashSet<int>();

			for (Day = 1; Day <= 5; Day++)
			{
				for (Slot = 1; Slot <= totalSlotsPerDay; Slot++)
				{
					Dictionary<int, int> D = schedule[Day, Slot];

					foreach (KeyValuePair<int, int> P in D)
					{
						groups.Add(P.Key);
					}
				}
			}

			for (i = 0; i < AllGroups.Count; i++)
			{
				if (!groups.Contains(i))
				{
					ret.FreeGroups.Add(AllGroups[i]);
				}
			}

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
