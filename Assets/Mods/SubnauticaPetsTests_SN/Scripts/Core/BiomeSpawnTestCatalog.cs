using System.Collections.Generic;

namespace DaftAppleGames.SubnauticaPetsTests
{
    internal static class BiomeSpawnTestCatalog
    {
        internal static List<BiomeSpawnExpectation> Create()
        {
            return new List<BiomeSpawnExpectation>
            {
                new BiomeSpawnExpectation("AlienRobotPetDna", 21),
                new BiomeSpawnExpectation("BloodCrawlerPetDna", 19),
                new BiomeSpawnExpectation("CaveCrawlerPetDna", 20),
                new BiomeSpawnExpectation("CrabSquidPetDna", 20)
            };
        }
    }
}
