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

            var immuneSystem = File.ReadAllLines(@"../../../ImmuneSystem.txt")
                .Select(l => BattleGroup.FromString(l, "ImSy")).ToList();

            var infection = File.ReadAllLines(@"../../../Infection.txt")
                .Select(l => BattleGroup.FromString(l,"Infect")).ToList();

            var sw = new Stopwatch();
            sw.Start();

            for (int boost = 29; boost < 32; boost++)
            {
                Console.WriteLine($"Boosting by {boost} ... ");

                immuneSystem = File.ReadAllLines(@"../../../ImmuneSystem.txt")
                .Select(l => BattleGroup.FromString(l, "ImSy")).ToList();
                foreach (var g in immuneSystem)
                {
                    g.AttackDamage += boost;
                }
                infection = File.ReadAllLines(@"../../../Infection.txt")
                .Select(l => BattleGroup.FromString(l, "Infect")).ToList();

                Console.WriteLine($"ImSy strength: {immuneSystem.Sum(g => g.EffectivePower)}");
                Console.WriteLine($"Infect strength: {infection.Sum(g => g.EffectivePower)}");

                var combat = new CombatSimulator(immuneSystem, infection);

                while (!combat.Groups.Select(g => g.Team).AreAllEqual())
                {
                    // Target selection
                    var mapAttackerToVictim = new Dictionary<BattleGroup, BattleGroup>();
                    foreach (var group in combat.Groups.OrderByDescending(g => g.EffectivePower).ThenByDescending(g => g.Initiative))
                    {
                        var target = combat.EnemyGroupsFor(group)
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

                    // Fight
                    foreach (var pair in mapAttackerToVictim.OrderByDescending(kvp => kvp.Key.Initiative))
                    {
                        if (pair.Key.UnitCount > 0)
                        {
                            pair.Key.Attack(pair.Value);
                        }
                    }
                    combat.Groups.RemoveAll(g => g.UnitCount <= 0);
                }

                Console.WriteLine($"Winning side: {combat.Groups.First().Team}");
                Console.WriteLine($"Remaining units: {combat.Groups.Sum(g => g.UnitCount)}");
                Console.WriteLine();
            }
            
           


            //Console.WriteLine($"Part 1: {combat.Groups.Sum(g => g.UnitCount)}");
            Console.WriteLine($"Part 2: x");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            Console.ReadLine();
        }
    }

    class CombatSimulator
    {
        public List<BattleGroup> Groups { get; set; }

        public CombatSimulator(params IEnumerable<BattleGroup>[] groups)
        {
            Groups = groups.SelectMany(g => g).ToList();
        }

        public IEnumerable<BattleGroup> EnemyGroupsFor(BattleGroup grp)
        {
            return Groups.Where(g => g.Team != grp.Team);
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

        internal void Attack(BattleGroup other)
        {
            var damage = PotentialDamageTo(other);
            var unitsKills = damage / other.HitPoints;
            other.UnitCount -= unitsKills;
        }
    }
}
