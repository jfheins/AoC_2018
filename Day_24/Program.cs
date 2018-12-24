using Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day_24
{
    class Program
    {
        static void Main(string[] args)
        {

            var immuneSystemInput = File.ReadAllLines(@"../../../ImmuneSystem.txt");
            var infectionInput = File.ReadAllLines(@"../../../Infection.txt");

            var sw = new Stopwatch();
            sw.Start();

            for (int boost = 0; boost < 32; boost++)
            {
                Console.WriteLine($"Boosting by {boost} ... ");

                var immuneSystem = BuildTeamFromInput(immuneSystemInput, "ImSy", boost);
                var infection = BuildTeamFromInput(infectionInput, "Infect");

                var combat = new CombatSimulator(immuneSystem, infection);

                while (combat.FightRound()) { }

                Console.WriteLine($"Winning side: {combat.Winners}");
                Console.WriteLine($"Combat ended because: {combat.TerminationReason}");
                Console.WriteLine($"Remaining units: {combat.Groups.Sum(g => g.UnitCount)}");
                Console.WriteLine();
            }

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            Console.ReadLine();
        }

        private static List<BattleGroup> BuildTeamFromInput(string[] input, string teamname, int boost = 0)
        {
            var result = input
                .Select(l => BattleGroup.FromString(l, teamname)).ToList();
            foreach (var group in result)
            {
                group.AttackDamage += boost;
            }
            return result;
        }
    }

    class CombatSimulator
    {
        public List<BattleGroup> Groups { get; set; }
        public string TerminationReason { get; private set; } = "";
        public string Winners { get; private set; } = "";

        public CombatSimulator(params IEnumerable<BattleGroup>[] groups)
        {
            Groups = groups.SelectMany(g => g).ToList();
        }

        public IEnumerable<BattleGroup> EnemyGroupsFor(BattleGroup grp)
        {
            return Groups.Where(g => g.Team != grp.Team);
        }

        private IEnumerable<BattleGroup> GroupsInSelectionOrder => Groups
            .OrderByDescending(g => g.EffectivePower)
            .ThenByDescending(g => g.Initiative);

        public bool FightRound()
        {
            // Target selection
            var mapAttackerToVictim = new Dictionary<BattleGroup, BattleGroup>();
            foreach (var group in GroupsInSelectionOrder)
            {
                var target = EnemyGroupsFor(group)
                    .Except(mapAttackerToVictim.Values)
                    .OrderByDescending(t => group.PotentialDamageTo(t))
                    .ThenByDescending(t => t.EffectivePower)
                    .ThenByDescending(t => t.Initiative)
                    .Where(t => group.PotentialDamageTo(t) > 0)
                    .FirstOrDefault();

                if (target != null)
                {
                    mapAttackerToVictim.Add(group, target);
                }
            }

            // Perform fight
            var unitsKilled = false;
            foreach (var pair in mapAttackerToVictim.OrderByDescending(kvp => kvp.Key.Initiative))
            {
                if (pair.Key.UnitCount > 0)
                {
                    unitsKilled |= pair.Key.Attack(pair.Value);
                }
            }
            if (!unitsKilled)
            {
                Winners = "None";
                TerminationReason = "No unit could deal any damage. Combat will last indefinitely";
                return false;
            }
            Groups.RemoveAll(g => g.UnitCount <= 0);
            if (Groups.Select(g => g.Team).AreAllEqual())
            {
                Winners = Groups.First().Team;
                TerminationReason = $"Only Team {Winners} remains.";
                return false;
            }
            return true;
        }
    }

    public class BattleGroup
    {
        public int UnitCount { get; set; }
        /// <summary>
        /// Number of Hitpoints each unit has
        /// </summary>
        public int HitPoints { get; set; }

        public ICollection<string> Weaknesses { get; private set; } = new List<string>();
        public ICollection<string> Immunities { get; private set; } = new List<string>();

        public int AttackDamage { get; set; }
        public string AttackKind { get; set; }
        public int Initiative { get; set; }
        public string Team { get; set; }

        public int EffectivePower => UnitCount * AttackDamage;

        public BattleGroup()
        {
        }

        public int PotentialDamageTo(BattleGroup other)
        {
            if (other.Immunities.Contains(AttackKind))
            {
                return 0;
            }
            return other.Weaknesses.Contains(AttackKind) ? 2 * EffectivePower : EffectivePower;
        }

        public static BattleGroup FromString(string str, string team)
        {
            var numbers = str.ParseInts(4);

            var weaknessRegex = new Regex(@"weak to ([^;)]+)[;)]");
            var immuneRegex = new Regex(@"immune to ([^;)]+)[;)]");
            var dmgKindRegex = new Regex(@"\d (\w+) damage");

            var weaknesses = weaknessRegex.Match(str).Groups[1].Value.Split(',');
            weaknesses = weaknesses.Select(s => s.Trim()).Where(s => s != "").ToArray();

            var immunities = immuneRegex.Match(str).Groups[1].Value.Split(',');
            immunities = immunities.Select(s => s.Trim()).Where(s => s != "").ToArray();

            var dmgKind = dmgKindRegex.Match(str).Groups[1].Value.Trim();

            return new BattleGroup
            {
                UnitCount = numbers[0],
                HitPoints = numbers[1],
                AttackDamage = numbers[2],
                Initiative = numbers[3],
                AttackKind = dmgKind,
                Weaknesses = new HashSet<string>(weaknesses),
                Immunities = new HashSet<string>(immunities),
                Team = team
            };
        }

        /// <summary>
        /// Performs an attack on an enemy unit
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True, if attack killed at least one unit</returns>
        internal bool Attack(BattleGroup other)
        {
            var damage = PotentialDamageTo(other);
            var unitKills = damage / other.HitPoints;
            other.UnitCount -= unitKills;
            return unitKills > 0;
        }
    }
}
