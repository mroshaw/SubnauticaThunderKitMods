using System.Collections.Generic;
using UnityEngine;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes
{
    /// <summary>
    /// Static class to manage Water Filtration beams
    /// </summary>
    public static class SaveMyEyesFromWaterFiltrationBeams
    {
        private static readonly Dictionary<BaseFiltrationMachineGeometry, bool> FiltrationMachineBeamStates = new Dictionary<BaseFiltrationMachineGeometry, bool>();

        /// <summary>
        /// Stores a filtration machine and the beam state requested by the game
        /// </summary>
        internal static void Register(BaseFiltrationMachineGeometry filtrationMachine)
        {
            if (FiltrationMachineBeamStates.ContainsKey(filtrationMachine))
            {
                return;
            }

            bool beamsActive = false;
            foreach (Transform beam in filtrationMachine.beams)
            {
                if (beam && beam.gameObject.activeSelf)
                {
                    beamsActive = true;
                    break;
                }
            }

            FiltrationMachineBeamStates.Add(filtrationMachine, beamsActive);
            ConfigureFiltrationMachine(filtrationMachine, beamsActive, ConfigFile.DisableWaterFiltrationBeams);
        }

        /// <summary>
        /// Removes a filtration machine from the registry, for example during OnDestroy
        /// </summary>
        internal static void Unregister(BaseFiltrationMachineGeometry filtrationMachine)
        {
            FiltrationMachineBeamStates.Remove(filtrationMachine);
        }

        /// <summary>
        /// Records and applies the beam state requested by the game
        /// </summary>
        internal static void ConfigureFromGameState(BaseFiltrationMachineGeometry filtrationMachine, bool beamsActive)
        {
            if (!filtrationMachine)
            {
                return;
            }

            FiltrationMachineBeamStates[filtrationMachine] = beamsActive;
            ConfigureFiltrationMachine(filtrationMachine, beamsActive, ConfigFile.DisableWaterFiltrationBeams);
        }

        private static void ConfigureFiltrationMachine(BaseFiltrationMachineGeometry filtrationMachine, bool beamsActive, bool disableBeams)
        {
            if (!filtrationMachine)
            {
                ModDebugLog.LogDebug("ConfigureFiltrationMachine: BaseFiltrationMachineGeometry in registry has been destroyed.");
                return;
            }

            bool enableBeams = beamsActive && !disableBeams;
            foreach (Transform beam in filtrationMachine.beams)
            {
                if (beam && beam.gameObject.activeSelf != enableBeams)
                {
                    ModDebugLog.LogDebug($"ConfigureFiltrationMachine: setting {filtrationMachine.gameObject.name} beam state to {enableBeams}");
                    beam.gameObject.SetActive(enableBeams);
                }
            }
        }

        /// <summary>
        /// Public method to configure all machines. Called from Config change
        /// </summary>
        public static void ConfigureAllFiltrationMachines(bool disableBeams)
        {
            foreach (KeyValuePair<BaseFiltrationMachineGeometry, bool> filtrationMachineState in FiltrationMachineBeamStates)
            {
                ConfigureFiltrationMachine(filtrationMachineState.Key, filtrationMachineState.Value, disableBeams);
            }
        }
    }
}
