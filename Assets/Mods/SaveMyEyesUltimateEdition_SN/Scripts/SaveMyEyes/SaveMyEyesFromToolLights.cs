using System.Collections.Generic;
using UnityEngine;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes
{
    /// <summary>
    /// Static class to manage Tool effect intensities
    /// </summary>
    public static class SaveMyEyesFromToolLights
    {
        private static readonly Dictionary<Light, float> ToolLightIntensities = new Dictionary<Light, float>();

        /// <summary>
        /// Stores a contributed light and its initial intensity
        /// </summary>
        internal static void Register(Light light)
        {
            if (!light)
            {
                return;
            }

            if (!ToolLightIntensities.ContainsKey(light))
            {
                ToolLightIntensities.Add(light, light.intensity);
            }

            ConfigureToolLight(light, ToolLightIntensities[light], ConfigFile.ToolLightIntensity);
        }

        /// <summary>
        /// Removes a light from the registry, for example during OnDestroy
        /// </summary>
        internal static void Unregister(Light light)
        {
            if (!ReferenceEquals(light, null))
            {
                ToolLightIntensities.Remove(light);
            }
        }

        /// <summary>
        /// Configure the light
        /// </summary>
        private static void ConfigureToolLight(Light light, float initialIntensity, float intensityMultiplier)
        {
            if (!light)
            {
                ModDebugLog.LogDebug($"ConfigureToolLight: light in HashSet is null!");
                return;
            }
            
            string objectName = light.transform.parent != null ? light.transform.parent.gameObject.name : light.gameObject.name;
            float currentIntensity = light.intensity;
            float newIntensity = initialIntensity * intensityMultiplier;
            ModDebugLog.LogDebug($"ConfigureToolLight: setting {objectName} intensity to {intensityMultiplier}");
            ModDebugLog.LogDebug($"ConfigureToolLight: changing {objectName} light intensity from {currentIntensity} to {newIntensity}");
            light.intensity = newIntensity;
        }

        /// <summary>
        /// Public method to configure all tool lights. Called from Config change
        /// </summary>
        public static void ConfigureAllToolLights(float intensityMultiplier)
        {
            foreach (KeyValuePair<Light, float> toolLightIntensity in ToolLightIntensities)
            {
                ConfigureToolLight(toolLightIntensity.Key, toolLightIntensity.Value, intensityMultiplier);
            }
        }
    }
}
