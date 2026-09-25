using System.Collections.Generic;
using UnityEngine;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes
{
    /// <summary>
    /// Maintains configuration for contributed flare lights
    /// </summary>
    public static class SaveMyEyesFromFlares
    {
        private static readonly Dictionary<Flare, FlareState> Flares = new Dictionary<Flare, FlareState>();

        private sealed class FlareState
        {
            internal float LastGameIntensity;
            internal bool HasGameIntensity;
        }

        /// <summary>
        /// Stores a contributed flare
        /// </summary>
        internal static void Register(Flare flare)
        {
            if (flare && !Flares.ContainsKey(flare))
            {
                Flares.Add(flare, new FlareState());
            }
        }

        /// <summary>
        /// Removes a flare from the registry
        /// </summary>
        internal static void Unregister(Flare flare)
        {
            Flares.Remove(flare);
        }

        /// <summary>
        /// Restores the last unscaled intensity before the game calculates its next value
        /// </summary>
        internal static void PrepareCurrentFlareIntensity(Flare flare)
        {
            FlareState state;
            if (flare && flare.light && Flares.TryGetValue(flare, out state) && state.HasGameIntensity)
            {
                flare.light.intensity = state.LastGameIntensity;
            }
        }

        /// <summary>
        /// Captures and scales the intensity freshly calculated by the game
        /// </summary>
        internal static void ConfigureCurrentFlareIntensity(Flare flare)
        {
            if (!flare || !flare.light)
            {
                return;
            }

            FlareState state;
            if (!Flares.TryGetValue(flare, out state))
            {
                state = new FlareState();
                Flares.Add(flare, state);
            }

            state.LastGameIntensity = flare.light.intensity;
            state.HasGameIntensity = true;
            flare.light.intensity = state.LastGameIntensity * ConfigFile.FlareIntensity;
        }

        /// <summary>
        /// Applies an intensity change immediately without illuminating inactive flares
        /// </summary>
        public static void ConfigureAllFlares(float intensityModifier)
        {
            foreach (KeyValuePair<Flare, FlareState> flareState in Flares)
            {
                Flare flare = flareState.Key;
                if (!flare || !flare.light)
                {
                    continue;
                }

                if (flare.flareActiveState && flareState.Value.HasGameIntensity)
                {
                    flare.light.intensity = flareState.Value.LastGameIntensity * intensityModifier;
                }
                else
                {
                    flare.light.intensity = 0.0f;
                }
            }
        }
    }
}
