using System.Collections.Generic;
using UnityEngine;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes
{
    /// <summary>
    /// Static class to manage emitter material effect intensities
    /// </summary>
    public static class SaveMyEyesFromEmitterMaterials
    {
        private static readonly Dictionary<Material, EmissionIntensities> EmitterMaterialIntensities = new Dictionary<Material, EmissionIntensities>();

        private readonly struct EmissionIntensities
        {
            internal readonly float GlowStrength;
            internal readonly float GlowStrengthNight;

            internal EmissionIntensities(float glowStrength, float glowStrengthNight)
            {
                GlowStrength = glowStrength;
                GlowStrengthNight = glowStrengthNight;
            }
        }

        /// <summary>
        /// Stores an emitter material and its initial emission intensities
        /// </summary>
        internal static void Register(Material emitterMaterial)
        {
            if (!emitterMaterial)
            {
                return;
            }

            bool hasGlowStrength = emitterMaterial.HasProperty(ShaderPropertyID._GlowStrength);
            bool hasGlowStrengthNight = emitterMaterial.HasProperty(ShaderPropertyID._GlowStrengthNight);
            if (!hasGlowStrength && !hasGlowStrengthNight)
            {
                return;
            }

            if (!EmitterMaterialIntensities.ContainsKey(emitterMaterial))
            {
                EmissionIntensities intensities = new EmissionIntensities(
                    hasGlowStrength ? emitterMaterial.GetFloat(ShaderPropertyID._GlowStrength) : 0.0f,
                    hasGlowStrengthNight ? emitterMaterial.GetFloat(ShaderPropertyID._GlowStrengthNight) : 0.0f);
                EmitterMaterialIntensities.Add(emitterMaterial, intensities);
            }

            ConfigureEmitterMaterial(emitterMaterial, EmitterMaterialIntensities[emitterMaterial], ConfigFile.MaterialEmissionIntensity);
        }

        /// <summary>
        /// Removes an emitter material from the registry, for example during OnDestroy
        /// </summary>
        internal static void Unregister(Material emitterMaterial)
        {
            if (!ReferenceEquals(emitterMaterial, null))
            {
                EmitterMaterialIntensities.Remove(emitterMaterial);
            }
        }

        /// <summary>
        /// Configure the light
        /// </summary>
        private static void ConfigureEmitterMaterial(Material emitterMaterial, EmissionIntensities initialIntensities, float intensityMultiplier)
        {
            if (!emitterMaterial)
            {
                ModDebugLog.LogDebug($"ConfigureEmitterMaterial: Material in HashSet is null. Removed from HashSet!");
                return;
            }
            
            string objectName = emitterMaterial.name;

            if (initialIntensities.GlowStrength > 0f && emitterMaterial.HasProperty(ShaderPropertyID._GlowStrength))
            {
                float currentGlowStrength = emitterMaterial.GetFloat("_GlowStrength");
                float newGlowStrength = initialIntensities.GlowStrength * intensityMultiplier;
                ModDebugLog.LogDebug($"ConfigureEmitterMaterial: changing {objectName} glow strength from {currentGlowStrength} to {newGlowStrength}");
                emitterMaterial.SetFloat(ShaderPropertyID._GlowStrength, newGlowStrength);
            }
            
            if (initialIntensities.GlowStrengthNight > 0f && emitterMaterial.HasProperty(ShaderPropertyID._GlowStrengthNight))
            {
                float currentGlowStrengthNight = emitterMaterial.GetFloat("_GlowStrengthNight");
                float newGlowStrengthNight = initialIntensities.GlowStrengthNight * intensityMultiplier;
                ModDebugLog.LogDebug($"ConfigureEmitterMaterial: changing {objectName} glow strength night from {currentGlowStrengthNight} to {newGlowStrengthNight}");
                emitterMaterial.SetFloat(ShaderPropertyID._GlowStrengthNight, newGlowStrengthNight);
            }
        }

        /// <summary>
        /// Public method to configure all emitter materials. Called from Config change
        /// </summary>
        public static void ConfigureAllEmitterMaterials(float intensityMultiplier)
        {
            foreach (KeyValuePair<Material, EmissionIntensities> emitterMaterialIntensity in EmitterMaterialIntensities)
            {
                ConfigureEmitterMaterial(emitterMaterialIntensity.Key, emitterMaterialIntensity.Value, intensityMultiplier);
            }
        }
    }
}
