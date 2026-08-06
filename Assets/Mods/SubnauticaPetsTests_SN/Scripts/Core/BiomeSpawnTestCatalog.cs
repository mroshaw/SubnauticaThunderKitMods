using System.Collections.Generic;

namespace DaftAppleGames.SubnauticaPetsTests
{
    internal static class BiomeSpawnTestCatalog
    {
        internal static List<BiomeSpawnExpectation> Create()
        {
            return new List<BiomeSpawnExpectation>
            {
                new BiomeSpawnExpectation("CatPetDna", 8),
                new BiomeSpawnExpectation("AlienRobotPetDna", 16),
                new BiomeSpawnExpectation("BloodCrawlerPetDna", 14),
                new BiomeSpawnExpectation("CaveCrawlerPetDna", 15),
                new BiomeSpawnExpectation("CrabSquidPetDna", 15)
            };
        }
    }
}
