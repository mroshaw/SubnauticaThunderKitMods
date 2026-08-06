using DaftAppleGames.SubnauticaPets.Pets;
using Nautilus.Commands;
using Nautilus.Handlers;
using static DaftAppleGames.SubnauticaPets.SubnauticaPetsPlugin;

namespace DaftAppleGames.SubnauticaPets
{
    public class PetCommands
    {
        public static void RegisterAll()
        {
            ModDebugLog.LogDebug("Registering KillAllPetsCommand...");
            ConsoleCommandsHandler.RegisterConsoleCommands(typeof(KillAllPetsCommand));
            ModDebugLog.LogDebug("Done Registering KillAllPetsCommand.");
        }

        /// <summary>
        /// Command class to kill all active pets
        /// </summary>
        public class KillAllPetsCommand
        {
            [ConsoleCommand("killallpets")]
            public static void KillAllPets()
            {
                ModDebugLog.LogDebug("Killing all pets...");
                ErrorMessage.AddMessage("Killing all Pets!");
                Pet.KillAllPets();
            }
        }
    }
}