using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaftAppleGames.SubnauticaPetsTests
{
    internal static class SpawnTestCatalog
    {
        private const float FragmentTolerance = 1.0f;
        private const float DnaTolerance = 1.26f;
        private const float FragmentVerticalTolerance = 1.0f;
        private const float DnaDownwardSettlementTolerance = 12.0f;
        private const float DnaUpwardTolerance = 2.0f;

        internal static List<SpawnTestCase> Create(string suite)
        {
            List<SpawnTestCase> testCases = new List<SpawnTestCase>();
            bool includeFragments = string.Equals(suite, "all", StringComparison.OrdinalIgnoreCase) ||
                                    string.Equals(suite, "fragments", StringComparison.OrdinalIgnoreCase);
            bool includeDna = string.Equals(suite, "all", StringComparison.OrdinalIgnoreCase) ||
                              string.Equals(suite, "dna", StringComparison.OrdinalIgnoreCase);
            if (includeFragments) AddFragmentTests(testCases);
            if (includeDna) AddDnaTests(testCases);
            return testCases;
        }

        private static void AddDnaTests(List<SpawnTestCase> testCases)
        {
            AddDnaCluster(testCases, "CatDna[0]", "CatPetDna", -49.88f, -28.49f, -403.04f, 2);
            AddDnaCluster(testCases, "CatDna[1]", "CatPetDna", -168.27f, -41.07f, -234.29f, 3);
            AddDnaCluster(testCases, "CatDna[2]", "CatPetDna", -1628.70f, -356.51f, 77.22f, 4);

            AddDnaCluster(testCases, "AlienRobotDna[0]", "AlienRobotPetDna", 292.63f, -103.24f, 414.90f, 2);
            AddDnaCluster(testCases, "AlienRobotDna[1]", "AlienRobotPetDna", -381.88f, -122.79f, 623.95f, 3);
            AddDnaCluster(testCases, "AlienRobotDna[2]", "AlienRobotPetDna", -503.13f, -96.74f, -56.38f, 4);
            AddDnaCluster(testCases, "AlienRobotDna[3]", "AlienRobotPetDna", 16.01f, -26.85f, -243.06f, 5);

            AddDnaCluster(testCases, "BloodCrawlerDna[0]", "BloodCrawlerPetDna", 78.52f, -46.40f, 389.12f, 3);
            AddDnaCluster(testCases, "BloodCrawlerDna[1]", "BloodCrawlerPetDna", -1599.49f, -353.97f, 79.63f, 4);
            AddDnaCluster(testCases, "BloodCrawlerDna[2]", "BloodCrawlerPetDna", -628.11f, -109.64f, -37.28f, 2, 2);

            AddDnaCluster(testCases, "CaveCrawlerDna[0]", "CaveCrawlerPetDna", -394.39f, -138.44f, 666.46f, 2);
            AddDnaCluster(testCases, "CaveCrawlerDna[1]", "CaveCrawlerPetDna", -769.64f, -222.83f, -729.66f, 3);
            AddDnaCluster(testCases, "CaveCrawlerDna[2]", "CaveCrawlerPetDna", -1448.19f, -346.24f, 768.21f, 4);

            AddDnaCluster(testCases, "CrabSquidDna[0]", "CrabSquidPetDna", 85.92f, -33.90f, 128.91f, 2);
            AddDnaCluster(testCases, "CrabSquidDna[1]", "CrabSquidPetDna", -27.72f, -30.77f, -418.56f, 3);
            AddDnaCluster(testCases, "CrabSquidDna[2]", "CrabSquidPetDna", 396.70f, -26.90f, -175.90f, 4);
            AddDnaCluster(testCases, "CrabSquidDna[3]", "CrabSquidPetDna", 380.36f, -24.60f, -209.09f, 4);
        }

        private static void AddDnaCluster(List<SpawnTestCase> testCases, string clusterName, string classId,
            float x, float y, float z, int count)
        {
            AddDnaCluster(testCases, clusterName, classId, x, y, z, count, 0);
        }

        private static void AddDnaCluster(List<SpawnTestCase> testCases, string clusterName, string classId,
            float x, float y, float z, int count, int firstOffsetIndex)
        {
            Vector3 center = new Vector3(x, y, z);
            Vector3[] offsets =
            {
                Vector3.zero,
                new Vector3(3.0f, 0.0f, 0.0f),
                new Vector3(-1.5f, 0.0f, 2.6f),
                new Vector3(-1.5f, 0.0f, -2.6f),
                new Vector3(0.0f, 0.0f, 4.5f)
            };

            for (int sampleIndex = 0; sampleIndex < count; sampleIndex++)
            {
                Vector3 position = center + offsets[firstOffsetIndex + sampleIndex];
                AddDna(testCases, $"{clusterName}.{sampleIndex}", classId, position.x, position.y, position.z);
            }
        }

        private static void AddFragmentTests(List<SpawnTestCase> testCases)
        {
            AddFragment(testCases, "ConsoleFragment[0]", "PetConsoleFragment", -49.88f, -30.49f, -407.04f);
            AddFragment(testCases, "ConsoleFragment[1]", "PetConsoleFragment", 288.63f, -105.24f, 414.90f);
            AddFragment(testCases, "ConsoleFragment[2]", "PetConsoleFragment", 74.52f, -48.40f, 389.12f);
            AddFragment(testCases, "ConsoleFragment[3]", "PetConsoleFragment", -398.39f, -140.44f, 666.46f);
            AddFragment(testCases, "ConsoleFragment[4]", "PetConsoleFragment", -1632.70f, -358.51f, 77.22f);
            AddFragment(testCases, "ConsoleFragment[5]", "PetConsoleFragment", -507.13f, -98.74f, -56.38f);
            AddFragment(testCases, "ConsoleFragment[6]", "PetConsoleFragment", -632.11f, -111.64f, -37.28f);
            AddFragment(testCases, "ConsoleFragment[7]", "PetConsoleFragment", -1452.19f, -348.24f, 768.21f);
            AddFragment(testCases, "ConsoleFragment[8]", "PetConsoleFragment", 81.92f, -35.90f, 128.91f);
            AddFragment(testCases, "ConsoleFragment[9]", "PetConsoleFragment", 392.70f, -28.90f, -175.90f);

            AddFragment(testCases, "FabricatorFragment[0]", "PetFabricatorFragment", -172.27f, -43.07f, -234.29f);
            AddFragment(testCases, "FabricatorFragment[1]", "PetFabricatorFragment", -385.88f, -124.79f, 623.95f);
            AddFragment(testCases, "FabricatorFragment[2]", "PetFabricatorFragment", -1603.49f, -355.97f, 79.63f);
            AddFragment(testCases, "FabricatorFragment[3]", "PetFabricatorFragment", -773.64f, -224.83f, -729.66f);
            AddFragment(testCases, "FabricatorFragment[4]", "PetFabricatorFragment", -31.72f, -32.77f, -418.56f);
            AddFragment(testCases, "FabricatorFragment[5]", "PetFabricatorFragment", 12.01f, -28.85f, -243.06f);
            AddFragment(testCases, "FabricatorFragment[6]", "PetFabricatorFragment", 76.28f, -30.01f, -88.79f);
            AddFragment(testCases, "FabricatorFragment[7]", "PetFabricatorFragment", 82.50f, -40.76f, 117.07f);
            AddFragment(testCases, "FabricatorFragment[8]", "PetFabricatorFragment", 376.36f, -26.60f, -209.09f);
        }

        private static void AddFragment(List<SpawnTestCase> testCases, string name, string classId, float x, float y,
            float z)
        {
            string fragmentComponentName = classId == "PetConsoleFragment"
                ? "PetConsoleFragment"
                : "PetFabricatorFragment";
            testCases.Add(new SpawnTestCase(name, classId, new Vector3(x, y, z), FragmentTolerance,
                FragmentVerticalTolerance, FragmentVerticalTolerance,
                "ResourceTracker", "LargeWorldEntity", fragmentComponentName));
        }

        private static void AddDna(List<SpawnTestCase> testCases, string name, string classId, float x, float y,
            float z)
        {
            testCases.Add(new SpawnTestCase(name, classId, new Vector3(x, y, z), DnaTolerance,
                DnaDownwardSettlementTolerance, DnaUpwardTolerance,
                "Pickupable", "ResourceTracker", "LargeWorldEntity", "PetDna"));
        }

    }
}
