using System.Collections.Generic;
using UnityEngine;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes
{
    /// <summary>
    /// MonoBehaviour class to manage emitter material effect intensities
    /// </summary>
    internal class SaveMyEyesMaterialController : MonoBehaviour
    {
        // Struct to hold initial values so that modifier can be applied consistently
        private readonly struct MaterialSettings
        {
            internal readonly float GlowStrength;
            internal readonly float GlowStrengthNight;

            internal MaterialSettings(float glowStrength, float glowStrengthNight)
            {
                GlowStrength = glowStrength;
                GlowStrengthNight = glowStrengthNight;
            }
        }
        
        // List of materials to manage
        private Dictionary<Material, MaterialSettings> _emitterMaterials;
        
        private void Awake()
        {
            FindEmitterMaterials();
        }

        private void OnEnable()
        {
            ConfigFile.MaterialSettingsChanged += ApplyChanges;
            ApplyChanges(ConfigFile.MaterialEmissionIntensity);
        }
        
        private void OnDisable()
        {
            ConfigFile.MaterialSettingsChanged -= ApplyChanges;
        }
        
        private void FindEmitterMaterials()
        {
            _emitterMaterials = new Dictionary<Material, MaterialSettings>();

            // Iterate over all materials in all renderers
            Renderer[] allRenderers = GetComponentsInChildren<Renderer>(true);
            foreach (Renderer foundRenderer in allRenderers)
            {
                foreach (Material material in foundRenderer.materials)
                {
                    bool hasGlowStrength = material.HasProperty(ShaderPropertyID._GlowStrength);
                    bool hasGlowStrengthNight = material.HasProperty(ShaderPropertyID._GlowStrengthNight);

                    if (!hasGlowStrength && !hasGlowStrengthNight)
                    {
                        continue;
                    }

                    float glowStrength = hasGlowStrength ? material.GetFloat(ShaderPropertyID._GlowStrength) : 0f;
                    float glowStrengthNight = hasGlowStrengthNight ? material.GetFloat(ShaderPropertyID._GlowStrengthNight) : 0f;
                    
                    _emitterMaterials[material] = new MaterialSettings(glowStrength, glowStrengthNight);
                }
            }
        }

        private void ApplyChanges(float intensityMultiplier)
        {
            // Material list is not yet initialised
            if (_emitterMaterials == null)
            {
                return;
            }
            
            ModDebugLog.LogDebug(
                $"MaterialController.Apply: Applying intensity multiplier: {intensityMultiplier} to {gameObject.name}");
            foreach (KeyValuePair<Material, MaterialSettings> emitterMaterial in _emitterMaterials)
            {
                if (!emitterMaterial.Key)
                {
                    ModDebugLog.LogDebug($"MaterialController.Apply: Material is null!");
                    continue;
                }

                bool hasGlowStrength = emitterMaterial.Key.HasProperty(ShaderPropertyID._GlowStrength);
                bool hasGlowStrengthNight = emitterMaterial.Key.HasProperty(ShaderPropertyID._GlowStrengthNight);
                
                if (hasGlowStrength)
                {
                    float newGlowStrength = emitterMaterial.Value.GlowStrength * intensityMultiplier;
                    emitterMaterial.Key.SetFloat(ShaderPropertyID._GlowStrength, newGlowStrength);
                }

                if (hasGlowStrengthNight)
                {
                    float newGlowStrengthNight = emitterMaterial.Value.GlowStrengthNight * intensityMultiplier;
                    emitterMaterial.Key.SetFloat(ShaderPropertyID._GlowStrengthNight, newGlowStrengthNight);
                }
            }
        }
    }
}