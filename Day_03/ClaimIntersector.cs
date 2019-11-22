using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

using Core.Combinatorics;

namespace Day_03
{
    class ClaimIntersector
    {
        public List<Claim> Claims { get; }

        private ClaimIntersector(IEnumerable<Claim> claims)
        {
            Claims = claims.ToList();
        }

        public static ClaimIntersector Parse(IEnumerable<string> input)
        {
            return new ClaimIntersector(input.Select(ParseClaim));
        }

        public int GetIntersectionArea()
        {
            var width = Claims.Max(c => c.Area.Right);

            var stripes = Enumerable.Range(0, width + 1).ToDictionary(x => x, x => new List<Claim>());

            foreach (var claim in Claims)
            {
                var intersectStripes = Enumerable.Range(claim.Area.Left, claim.Area.Width);
                foreach (var intersected in intersectStripes)
                    stripes[intersected].Add(claim);
            }

            // Count only y values that occur at least twice (=> no distinct)
            var count = stripes.AsParallel().Select(s =>
                s.Value.SelectMany(claim => Enumerable.Range(claim.Area.Bottom, claim.Area.Height)).Distinct().Count());



            return count.ToList().Sum();
        }

        public IEnumerable<int> GetNonIntersectingClaims()
        {
            var claimIds = new HashSet<int>(Claims.Select(c => c.Id));
            foreach (var pair in new Combinations<Claim>(Claims, 2))
            {
                if (pair[0].Area.IntersectsWith(pair[1].Area))
                {
                    claimIds.Remove(pair[0].Id);
                    claimIds.Remove(pair[1].Id);
                }
            }

            return claimIds;
        }

        private static Claim ParseClaim(string line)
        {
            // Format: #1 @ 1,3: 4x4
            var regex = new Regex(@"#(\d+) @ (\d+),(\d+): (\d+)x(\d+)");

            var groups = regex.Match(line).Groups;
            Debug.Assert(groups.Count == 6);
            var numbers = groups.Skip<Group>(1)
                                .Select(g => int.Parse(g.Value))
                                .ToArray();
            return new Claim(numbers[0], new Rectangle(numbers[1], numbers[2], numbers[3], numbers[4]));
        }

        public struct Claim
        {
            public Claim(int id, Rectangle area)
            {
                Id = id;
                Area = area;
            }

            public Rectangle Area { get; set; }
            public int Id { get; set; }
        }
    }
}
