using System.Linq;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class SkillUpInformation
    {
        public int Farming { get; set; }
        public int Fishing { get; set; }
        public int Mining { get; set; }
        private int Combat { get; set; }
        public int Luck { get; set; }
        public int Foraging { get; set; }
        private int Crafting { get; set; }
        public int MaxEnergy { get; set; }
        public int Magnetism { get; set; }
        public int Speed { get; set; }
        public int Defence { get; set; }

        public static SkillUpInformation Parse(string skillUpInformation)
        {
            var parts = skillUpInformation.Split(' ').Select(int.Parse).ToList();
            return new SkillUpInformation
            {
                Farming = parts[0],
                Fishing = parts[1],
                Mining = parts[2],
                Combat = parts[3],
                Luck = parts[4],
                Foraging = parts[5],
                Crafting = parts[6],
                MaxEnergy = parts[7],
                Magnetism = parts[8],
                Speed = parts[9],
                Defence = parts[10],
            };
        }

        public override string ToString()
        {
            return string.Join(" ", new []
            {
                Farming,
                Fishing,
                Mining,
                Combat,
                Luck,
                Foraging,
                Crafting,
                MaxEnergy,
                Magnetism,
                Speed,
                Defence,
            });
        }
    }
}