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
		public int[] StationPicks;

		public bool[] StationPicked;
		public int nStationsPicked;
		public Group(int id, string N, int R, int S1, int S2, int S3, int S4, int S5)
		{
			ID = id;
			Name = N;
			Rank = R;

			StationPicks = new int[5] { S1, S2, S3, S4, S5 };
			StationPicked = new bool[5] { false, false, false, false, false };
			nStationsPicked = 0;
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
		public string Category;
		public List<Availability> Avail;
		public int totalAvailabltSlots;
		public bool isActivityPin;

		public Station(int id, string N, int C, string Cat, bool Pin, List<Availability> A)
		{
			ID = id;
			Name = N;
			Capacity = C;
			Category = Cat == null ? "" : Cat.ToLower();
			isActivityPin = Pin;
			
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
		public int nVisits;

		public Constraint(Group g, Station s, int nV)
		{
			G = g;
			S = s;
			nVisits = nV;
		}
	}

	public static class GreedyScheduler
	{
		private static int MAXN = 150;
		private static int MAXD = 10;
		private static int MAXS = 30;

		private static int CONSTRAINT_PENALTY = -10;
		private static int[] PREF_PENALTIES = new int[5] { -100, -90, -70, -40, -20 };
		private static int SAME_CATEGORY_PENALTY = -100;
		private static int SAME_STATION_PENALTY = -200;
		
		private static int[] nSlots = new int[6];
		private static int[] lunchSlot = new int[6];
		private static List<Group> AllGroups;
		private static List<Station> AllStations;
		private static List<Constraint> AllConstraints;

		private static bool[,,] isGroupBusy = new bool[MAXN, MAXN, MAXN];
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

		// Schedule
		private static Dictionary<int, int>[,] masterSchedule = new Dictionary<int, int>[MAXD, MAXS];

		private static void convertTimeSlotsToDaySlots(IEnumerable<Models.TimeSlot> T)
		{
			List<Availability> A = new List<Availability>();
			int i, j;

			int minDay = 1 << 30;
			Dictionary<int, bool> lunch = new Dictionary<int,bool>();

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

				if (t.isGeneral)
					lunch.Add(t.ID, true);
			}

			for (i = 0; i < A.Count; i++)
			{
				nSlots[ A[i].DayNumber ] = A[i].Slots.Count;

				for (j = 0; j < A[i].Slots.Count; j++)
				{
					int D = A[i].DayNumber - minDay + 1;
					int S = j + 1;
					timeSlotsDaySlotsPairs[A[i].Slots[j]] = new KeyValuePair<int, int>(D,S);
					daySlotsTimeSlotsPairs[D, S] = A[i].Slots[j];

					if (lunch.ContainsKey(A[i].Slots[j]))
						lunchSlot[D] = S;
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

		private static void addOldScheduleToNewScheduleTillTimeSlot(IEnumerable<Models.Activity> oldSchedule, int Day, int Slot)
		{
			int i, j, groupIndex = 0, stationIndex = 0, prefIndex = -1, constraintIndex = -1;

			foreach (Models.Activity A in oldSchedule)
			{
				KeyValuePair<int, int> DS = timeSlotToDaySlot(A.TimeSlot);

				if (DS.Key > Day || DS.Key == Day && DS.Value >= Slot)
					continue;

				for (i = 0; i < AllGroups.Count; i++) if (AllGroups[i].ID == A.Group.ID) { groupIndex = i; break; }
				for (i = 0; i < AllStations.Count; i++) if (AllStations[i].ID == A.Station.ID) { stationIndex = i; break; }

				for(i=0;i<AllGroups.Count;i++)
					for(j=0;j<5;j++)
						if (AllGroups[i].StationPicks[j] != -1 && !AllGroups[i].StationPicked[j] && AllGroups[i].StationPicks[j] == stationIndex)
						{
							prefIndex = -1;
							break;
						}

				if( prefIndex == -1 )
					for (i = 0; i < AllConstraints.Count; i++)
						if (!ConstraintMet[i] && AllConstraints[i].G == AllGroups[groupIndex] && AllConstraints[i].S == AllStations[stationIndex])
						{
							constraintIndex = i;
							break;
						}
				
				scheduleGroupToStationAtDaySlot(groupIndex, stationIndex, Day, Slot, constraintIndex, prefIndex);
			}
		}
		public static IEnumerable<Models.Activity> getSchedule(IEnumerable<Models.Group> groups, IEnumerable<Models.Station> stations, IEnumerable<Models.SchedulingConstraint> constraints, 
				IEnumerable<Models.TimeSlot> slots, IEnumerable<Models.Activity> oldSchedule, Models.TimeSlot startingTimeSlot)
		{
			List<Models.Activity> schedule = new List<Models.Activity>();

			AllGroups = new List<Group>();
			AllStations = new List<Station>();
			AllConstraints = new List<Constraint>();

			int i,j,k;

			convertTimeSlotsToDaySlots(slots);

			List<Models.Group> tmpallGroups = new List<Models.Group>(groups.ToArray());
			List<Models.Station> tmpallStations = new List<Models.Station>(stations.ToArray());
			List<Models.TimeSlot> tmpallSlots = new List<Models.TimeSlot>(slots.ToArray());

			foreach (Models.Station s in stations)
				AllStations.Add(new Station(s.ID, s.Name, s.Capacity, s.Category, s.isActivityPin, timeSlotsToAvailability(s.AvailableTimeSlots)));

			foreach (Models.Group x in groups)
			{
				int p1 = -1, p2 = -1, p3 = -1, p4 = -1, p5 = -1;

				for (i = 0; i < AllStations.Count; i++)
				{
					if (x.Preference1 != null && x.Preference1.ID == AllStations[i].ID) p1 = i;
					if (x.Preference2 != null && x.Preference2.ID == AllStations[i].ID) p2 = i;
					if (x.Preference3 != null && x.Preference3.ID == AllStations[i].ID) p3 = i;
					if (x.Preference4 != null && x.Preference4.ID == AllStations[i].ID) p4 = i;
					if (x.Preference5 != null && x.Preference5.ID == AllStations[i].ID) p5 = i;
				}

				AllGroups.Add(new Group(x.ID, x.Name, x.Type.ID, p1, p2, p3, p4, p5));
			}

			foreach (Models.SchedulingConstraint c in constraints)
			{
				Group g = AllGroups[0];
				Station s = AllStations[0];

				for (i = 0; i < AllStations.Count; i++) if (AllStations[i].ID == c.Station.ID) s = AllStations[i];

				if (c.Group != null)
				{
					for (i = 0; i < AllGroups.Count; i++) if (AllGroups[i].ID == c.Group.ID) g = AllGroups[i];

					AllConstraints.Add(new Constraint(g, s, c.VisitNum));
				}
				else if (c.GroupType != null)
				{
					for (i = 0; i < AllGroups.Count; i++)
						if (c.GroupType.ID == AllGroups[i].Rank)
							AllConstraints.Add(new Constraint(g, s, c.VisitNum));
				}
				else if( c.Group == null && c.GroupType == null)
					for (i = 0; i < AllGroups.Count; i++)
						AllConstraints.Add(new Constraint(AllGroups[i], s, c.VisitNum));
			}

			for (i = 0; i < MAXD; i++)
				for (j = 0; j < MAXS; j++)
					masterSchedule[i, j] = new Dictionary<int, int>();

			for (i = 0; i < MAXN; i++)
				for (j = 0; j < MAXN; j++)
				{
					GroupStationAssignments[i, j] = GroupRankStationAssignments[i, j] = GroupAssignments[i] = StationAssignmentsCounts[i] = 0;

					for (k = 0; k < MAXN; k++)
					{
						StationSlotAssignmentsCounts[i, j, k] = 0;
						isGroupBusy[i, j, k] = false;
					}

					ConstraintMet[i] = false;
				}

			KeyValuePair<int, int> startDaySlot = timeSlotToDaySlot(startingTimeSlot);

			addOldScheduleToNewScheduleTillTimeSlot(oldSchedule, startDaySlot.Key, startDaySlot.Value);

			Schedule(startDaySlot.Key, startDaySlot.Value);

			/*
			Dictionary<string, string>[,] Q = new Dictionary<string, string>[10, 20];

			for(i=0;i<10;i++)
				for(j=0;j<20;j++)
				{
					Q[i,j] = new Dictionary<string,string>();

					foreach (KeyValuePair<int, int> P in masterSchedule[i, j])
					{
						Q[i, j].Add(AllGroups[P.Key].Name, AllStations[P.Value].Name);
					}
				}
			 * */
			for (i = 1; i <= 5; i++)
			{
				for (j = 1; j <= nSlots[i]; j++)
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

		private static void Schedule(int startingDay, int startingSlot)
		{
			// start monday end Friday
			int dayStart = startingDay, dayEnd = 5;
			int Day, Slot, i, j, k;

			for (Day = dayStart; Day <= dayEnd; Day++)
			{
				if (Day == dayStart)
					Slot = startingSlot;
				else
					Slot = 1;
				for (; Slot <= nSlots[Day]; Slot++)
				{
					if (lunchSlot[Day] == Slot)
						continue;

					// keep looking for assignments for this slot
					while (true)
					{
						int groupSelected = -1;
						int stationSelected = -1;
						int maxScore = -1 << 30;
						int minStationAssignment = 1 << 30;
						int s;

						// get best preference
						int prefIndex = -1;
						int prefScore = -1 << 30;
						int prefGroup = -1;
						int prefStation = -1;
						int tmpIndex = -1;

						// get other
						int otherGroupSelected = -1;
						int otherStationSelected = -1;
						int otherScore = -1 << 30;

						List<Group> groupsSortedByLeastAssgined = sortGroupsByLeastAssigned();
						Group curGroup;
						Station curStation;
						int groupIndex, stationIndex;

						for (i = 0; i < groupsSortedByLeastAssgined.Count; i++)
						{
							curGroup = groupsSortedByLeastAssgined[i];
							groupIndex = getGroupIndex(curGroup);

							if (curGroup.nStationsPicked >= 3 || isGroupBusy[Day, Slot, groupIndex])
								continue;

							tmpIndex = -1;

							for (j = 0; j < 5; j++)
							{
								stationIndex = curGroup.StationPicks[j];

								if (stationIndex == -1 || curGroup.StationPicked[j])
									continue;

								if (!canScheduleStationAtDaySlot(stationIndex, Day, Slot) || !canScheduleGroupToStationAtDaySlot(groupIndex, stationIndex, Day, Slot) )
									continue;

								tmpIndex = j;
								break;
							}

							if (tmpIndex == -1)
								continue;

							s = score(AllStations[curGroup.StationPicks[tmpIndex]], curGroup, Day, Slot);

							if (s > prefScore)
							{
								prefScore = s;
								prefGroup = groupIndex;
								prefIndex = tmpIndex;
								prefStation = curGroup.StationPicks[prefIndex];
							}
						}

						// get best constraint to be fullfilled
						int constraintIndex = -1;
						int constraintStation = -1;
						int constraintGroup = -1;
						int constraintScore = -1 << 30;

						for (i = 0; i < AllConstraints.Count; i++)
						{
							if (ConstraintMet[i])
								continue;

							groupIndex = getGroupIndex(AllConstraints[i].G);
							stationIndex = getStationIndex(AllConstraints[i].S);

							if (!canScheduleStationAtDaySlot(stationIndex, Day, Slot) || !canScheduleGroupToStationAtDaySlot(groupIndex, stationIndex, Day, Slot) )
								continue;

							s = score(AllConstraints[i].S, AllConstraints[i].G, Day, Slot);

							if (s > constraintScore)
							{
								constraintScore = s;
								constraintIndex = i;
								constraintGroup = groupIndex;
								constraintStation = stationIndex;
							}
						}

						if (prefIndex != -1 && constraintIndex == -1)
						{
							groupSelected = prefGroup;
							stationSelected = prefStation;
							maxScore = prefScore;
						}
						else if (prefIndex == -1 && constraintIndex != -1)
						{
							groupSelected = constraintGroup;
							stationSelected = constraintStation;
							maxScore = constraintScore;
						}
						else if (prefIndex != -1 && constraintIndex != -1)
						{
							if (prefIndex == 5)
							{
								groupSelected = constraintGroup;
								stationSelected = constraintStation;
								maxScore = constraintScore;
								prefIndex = -1;
							}
							else
							{
								if (prefScore >= constraintScore)
								{
									groupSelected = prefGroup;
									stationSelected = prefStation;
									constraintIndex = -1;
									maxScore = prefScore;
								}
								else
								{
									groupSelected = constraintGroup;
									stationSelected = constraintStation;
									prefIndex = -1;
									maxScore = constraintScore;
								}
							}
						}

						for (i = 0; i < groupsSortedByLeastAssgined.Count; i++)
						{
							curGroup = groupsSortedByLeastAssgined[i];
							groupIndex = getGroupIndex(curGroup);
								
							if (isGroupBusy[Day, Slot, groupIndex])
								continue;
								
							for (j = 0; j < AllStations.Count; j++)
							{
								curStation = AllStations[j];
								stationIndex = j;

								if (!canScheduleStationAtDaySlot(stationIndex, Day, Slot) || !canScheduleGroupToStationAtDaySlot(groupIndex, stationIndex, Day, Slot))
									continue;

								s = score(curStation, curGroup, Day, Slot);

								if (s > otherScore || s == otherScore && GroupStationAssignments[groupIndex, stationIndex] < minStationAssignment)
								{
									otherScore = s;
									otherGroupSelected = groupIndex;
									otherStationSelected = stationIndex;
									minStationAssignment = GroupStationAssignments[groupIndex, stationIndex];
								}
							}
						}

						if (otherScore > maxScore || constraintIndex == -1 && prefIndex == -1)
						{
							groupSelected = otherGroupSelected;
							stationSelected = otherStationSelected;
							constraintIndex = prefIndex = -1;
						}

						if (groupSelected == -1 || stationSelected == -1)
							break;

						scheduleGroupToStationAtDaySlot(groupSelected, stationSelected, Day, Slot, constraintIndex, prefIndex);
					}
				}
			}
		}

		private static void scheduleGroupToStationAtDaySlot(int groupSelected, int stationSelected, int Day, int Slot, int constraintIndex, int prefIndex)
		{
			// Group is busy in this slot
			isGroupBusy[Day, Slot, groupSelected] = true;
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
				if (GroupStationAssignments[groupSelected, stationSelected] == AllConstraints[constraintIndex].nVisits)
					ConstraintMet[constraintIndex] = true;
			}
			else if (prefIndex != -1)
			{
				AllGroups[groupSelected].nStationsPicked++;
				AllGroups[groupSelected].StationPicked[prefIndex] = true;
			}

			masterSchedule[Day, Slot].Add(groupSelected, stationSelected);

			if (AllStations[stationSelected].isActivityPin)
			{
				isGroupBusy[Day, Slot + 1, groupSelected] = true;
				masterSchedule[Day, Slot + 1].Add(groupSelected, stationSelected);
			}
		}

		private static bool canScheduleGroupToStationAtDaySlot(int groupIndex, int stationIndex, int Day, int Slot)
		{
			if (isGroupBusy[Day, Slot, groupIndex] || AllStations[stationIndex].isActivityPin && AllGroups[groupIndex].Rank != 4 || !canHappenGroupStationAssignment(groupIndex, stationIndex) )
				return false;

			return true;
		}

		private static bool canScheduleStationAtDaySlot(int stationIndex, int Day, int Slot)
		{
			if (!isStationAvailableAtSlot(AllStations[stationIndex], Day, Slot) || StationSlotAssignmentsCounts[stationIndex, Day, Slot] >= AllStations[stationIndex].Capacity ||
					AllStations[stationIndex].isActivityPin && !canSchedulePinnedStation(Day, Slot))
				return false;

			return true;
		}

		private static bool canSchedulePinnedStation(int Day, int Slot)
		{
			// is this is the last slot for the day or is the slot right before lunch, then I can't schedule an activity pin here
			if (Slot == nSlots[Day] || Slot == lunchSlot[Day] - 1 || Slot == lunchSlot[Day] )
				return false;

			return true;
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

		private static List<Group> sortGroupsByLeastAssigned()
		{
			int i, j;
			List<Group> ret = new List<Group>();
			bool[] v = new bool[ AllGroups.Count ];

			for (i = 0; i < AllGroups.Count; i++)
			{
				int minx = 1 << 30;
				int indx = -1;

				for (j = 0; j < AllGroups.Count; j++)
				{
					if( !v[j] && GroupAssignments[j] < minx )
					{
						minx = GroupAssignments[j];
						indx = j;
					}
				}

				v[indx] = true;
				ret.Add(AllGroups[indx]);
			}

			return ret;
		}
		private static bool canHappenGroupStationAssignment(int groupID, int stationID)
		{
			int i;

			for (i = 0; i < AllConstraints.Count; i++)
			{
				if (AllConstraints[i].G == AllGroups[groupID] && AllConstraints[i].S == AllStations[stationID])
					return GroupStationAssignments[groupID, stationID] < AllConstraints[i].nVisits;
			}

			return GroupStationAssignments[groupID, stationID] <= 2;
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
		private static int score(Station S, Group G, int Day, int Slot)
		{
			int ret = 0;

			int i, j;

			int groupIndex = getGroupIndex(G);

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

			// nNotGettingPicks[0] is the count of groups that aren't getting their first picks because of this group
			int[] nNotGettingPicks = new int[5] { 0, 0, 0, 0, 0 };

			// Check how many groups get their second pick instead of first, and how many groups aren't getting any picks

			// Copy the total assignment count for stations.
			int[] StationAssignmentsCountsTemp = (int[])StationAssignmentsCounts.Clone();
			StationAssignmentsCountsTemp[stationIndex]++;

			int stationPickAvailableSlots;
			int nPossible;

			for (i = 0; i < AllGroups.Count; i++)
			{
				if(AllGroups[i].nStationsPicked >= 3 )
					continue;

				nPossible = 0;
	
				for (j = 0; j < 5; j++)
				{
					if (AllGroups[i].StationPicks[j] == -1 || AllGroups[i].StationPicked[j] || AllStations[AllGroups[i].StationPicks[j]] != S)
						continue;

					nPossible++;
					stationPickAvailableSlots = AllStations[AllGroups[i].StationPicks[j]].totalAvailabltSlots - StationAssignmentsCountsTemp[AllGroups[i].StationPicks[j]];

					if (stationPickAvailableSlots <= 0)
						nNotGettingPicks[j]++;
					else
						StationAssignmentsCountsTemp[AllGroups[i].StationPicks[j]]++;

					break;
				}
			}

			for (i = 0; i < 5; i++)
				ret += PREF_PENALTIES[i] * nNotGettingPicks[i];

			// check if the same group was assigned to another station with the same group
			int nSameCat = 0;
			int nSameStation = 0;

			for (i = 1; i <= Slot; i++)
			{
				foreach (KeyValuePair<int, int> P in masterSchedule[Day, i])
				{
					if( groupIndex == P.Key )
					{
						if(P.Value != stationIndex && S.Category != "" && AllStations[P.Value].Category == S.Category)
							nSameCat++;
						else if(P.Value == stationIndex)
							nSameStation++;
					}
				}
			}

			ret += SAME_CATEGORY_PENALTY * nSameCat;
			ret += SAME_STATION_PENALTY * nSameStation;

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
