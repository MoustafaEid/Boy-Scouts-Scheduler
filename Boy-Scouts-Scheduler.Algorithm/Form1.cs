using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Boy_Scouts_Scheduling_Algorithm_MD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StationDay stationDay1 = new StationDay(1, new List<uint>{1,2,3,4,5});
            StationDay stationDay2 = new StationDay(2, new List<uint>{1,2,3,4,5});
            StationDay stationDay3 = new StationDay(3, new List<uint>{1,2,3,4,5});
            StationDay stationDay4 = new StationDay(4, new List<uint>{1,2,3,4,5});
            StationDay stationDay5 = new StationDay(5, new List<uint>{1,2,3,4,5});

            List<StationDay> allDays = new List<StationDay>
                {stationDay1, stationDay2, stationDay3, stationDay4, stationDay5};

            Station swimming = new Station("Swimming", 2, allDays);
            Station skits = new Station("Skits", 1, allDays);
            Station artsAndCrafts = new Station("Arts and crafts", 1, allDays);
            Station volleyball = new Station("Volleyball", 1, allDays);
            Station cooking = new Station("Cooking", 1, allDays);
            Station archery1 = new Station("Archery1", 1, allDays);
            Station archery2 = new Station("Archery2", 1, allDays);
            Station shooting1 = new Station("Shooting1", 1, allDays);
            Station shooting2 = new Station("Shooting2", 1, allDays);

            IList<Station> allStations = new List<Station>{swimming, skits,
                artsAndCrafts, volleyball, cooking, archery1, archery2, shooting1, shooting2};

            Group knights1 = new Group("Knights1", 4, new List<Station>{swimming, archery1, volleyball});
            Group knights2 = new Group("Knights2", 3, new List<Station>{cooking, archery2, shooting1});
            Group sirGawain = new Group("SirGawain", 1, new List<Station>{swimming, cooking, skits});
            Group sirKay = new Group("SirKay", 4, new List<Station>{artsAndCrafts, shooting2, shooting1});
            Group sirLionel = new Group("SirLionel", 2, new List<Station>{archery1, archery2, shooting1});
            Group sirLancelot = new Group("SirLancelot", 2, new List<Station>{archery1, archery2, swimming});
            Group palladins1 = new Group("Palladins1", 3, new List<Station>{swimming, archery1, skits});
            Group palladins2 = new Group("Palladins2", 1, new List<Station>{skits, volleyball});
            Group horsemen1 = new Group("Horsemen1", 2, new List<Station>{swimming, artsAndCrafts, archery2});
            Group horsemen2 = new Group("Horsemen2", 3, new List<Station>{archery2, skits, cooking});

            IList<Group> allGroups = new List<Group> { knights1, knights2, sirGawain,
                sirKay, sirLionel, sirLancelot, palladins1, palladins2, horsemen1, horsemen2 };

            Constraint constraint1 = new Constraint
                (null, null, new List<Station> { swimming, archery1, archery2 }, 2, 4, null);

            Constraint constraint2 = new Constraint
                (palladins1, null, new List<Station> { artsAndCrafts }, 3, 3, null);

            Constraint constraint3 = new Constraint
                (null, 2, new List<Station> { shooting2, volleyball }, 2, 2, null);

            Constraint constraint4 = new Constraint
                (null, 4, new List<Station> { cooking, skits }, 1, null, null);

            Constraint constraint5 = new Constraint
                (sirGawain, null, new List<Station> { skits, shooting2 }, 3, 3, null);

            Constraint constraint6 = new Constraint
                (null, 1, new List<Station> { swimming, artsAndCrafts }, null, 3, null);

            IList<Constraint> allConstraints = new List<Constraint> { constraint1,
                constraint2, constraint3, constraint4, constraint5, constraint6 };

            List<List<Dictionary<Group, Station>>> generatedSchedule =
                HillClimbingScheduler.GenerateSchedule(allGroups, allStations, allConstraints);

            StringBuilder outputMessage = new StringBuilder(2048);
            for (int dayNum = 0; dayNum < generatedSchedule.Count; dayNum++)
            {
                outputMessage.Append("Day " + (dayNum + 1 ) + "\n" + "------------------------" + "\n");
                for (int slotNum = 0; slotNum < generatedSchedule[dayNum].Count; slotNum++)
                {
                    outputMessage.Append("\tSlot " + (slotNum + 1) + "\n\t" + "-------------" + "\n");
                    foreach(KeyValuePair<Group, Station> assignment in generatedSchedule[dayNum][slotNum])
                    {
                        outputMessage.Append("\t\t" + assignment.Key.Name +
                            " is assigned to " + assignment.Value.Name + "\n");
                    }
                }
            }
            richTextBox1.Text = outputMessage.ToString();
        }
    }
}
