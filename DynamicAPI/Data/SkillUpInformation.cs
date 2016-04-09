namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class SkillUpInformation
    {
        public int Farming { get; set; }
        public int Fishing { get; set; }
        public int Mining { get; set; }
        public int Combat { get; set; }
        public int Luck { get; set; }
        public int Foraging { get; set; }
        public int MaxEnergy1 { get; set; }
        public int MaxEnergy2 { get; set; }
        public int Magnetism { get; set; }
        public int Speed { get; set; }

        public static SkillUpInformation Parse(string skillUpInformation)
        {
            var parts = skillUpInformation.Split(' ');
            return new SkillUpInformation
            {
                Farming = int.Parse(parts[0]),
                Fishing = int.Parse(parts[1]),
                Mining = int.Parse(parts[2]),
                Combat = int.Parse(parts[3]),
                Luck = int.Parse(parts[4]),
                Foraging = int.Parse(parts[5]),
                MaxEnergy1 = int.Parse(parts[6]),
                MaxEnergy2 = int.Parse(parts[7]),
                Magnetism = int.Parse(parts[8]),
                Speed = int.Parse(parts[9])
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
                MaxEnergy1,
                MaxEnergy2,
                Magnetism,
                Speed,
            });
        }
    }
}