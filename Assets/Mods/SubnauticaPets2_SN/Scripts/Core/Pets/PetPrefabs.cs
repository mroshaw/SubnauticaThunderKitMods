namespace DaftAppleGames.SubnauticaPets.Pets
{
    /// <summary>
    /// Registers all vanilla-creature pet prefabs.
    /// </summary>
    internal static class PetPrefabs
    {
        internal static void RegisterAll()
        {
            AlienRobotPrefab.Register();
            BloodCrawlerPrefab.Register();
            CaveCrawlerPrefab.Register();
            CrabSquidPrefab.Register();
        }
    }
}
