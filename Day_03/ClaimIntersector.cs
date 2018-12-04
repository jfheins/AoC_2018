using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
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
            var maxX = Claims.Max(c => c.Area.Right);
            var maxY = Claims.Max(c => c.Area.Bottom);
            int intersectionArea = 0;
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    var intersection = Claims.Where(claim => claim.Area.Contains(x, y)).ToArray();

                    if (intersection.Length > 1)
                    {
                        intersectionArea++;
                    }
                }
            }

            return intersectionArea;
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
            var numbers = groups.Skip(1).Select(g => int.Parse(g.Value)).ToArray();
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
