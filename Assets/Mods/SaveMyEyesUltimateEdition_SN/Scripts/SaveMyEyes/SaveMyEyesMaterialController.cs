using System.Collections.Generic;
using UnityEngine;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes
{
    /// <summary>
    /// Scales supported material glow from captured baseline values.
    /// </summary>
    internal class SaveMyEyesMaterialController : MonoBehaviour
    {
        private EffectUseCase useCase;

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
        
        private Dictionary<Material, MaterialSettings> emitterMaterials;
        
        private void Awake()
        {
            FindEmitterMaterials();
        }

        private void OnEnable()
        {
            ConfigFile.SettingsChanged += OnSettingsChanged;
            if (useCase != EffectUseCase.None)
            {
                ApplyChanges(ConfigFile.GetMaterialEmissionIntensity(useCase));
            }
        }
        
        private void OnDisable()
        {
            ConfigFile.SettingsChanged -= OnSettingsChanged;
        }

        internal void Initialize(EffectUseCase useCase)
        {
            this.useCase = useCase;
            if (isActiveAndEnabled)
            {
                ApplyChanges(ConfigFile.GetMaterialEmissionIntensity(this.useCase));
            }
        }

        private void OnSettingsChanged(EffectUseCase useCase)
        {
            if (this.useCase == useCase)
            {
                ApplyChanges(ConfigFile.GetMaterialEmissionIntensity(this.useCase));
            }
        }
        
        private void FindEmitterMaterials()
        {
            emitterMaterials = new Dictionary<Material, MaterialSettings>();

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
                    
                    emitterMaterials[material] = new MaterialSettings(glowStrength, glowStrengthNight);
                }
            }
        }

        private void ApplyChanges(float intensityMultiplier)
        {
            if (emitterMaterials is null)
            {
                return;
            }
            
            ModDebugLog.LogDebug(
                $"MaterialController.Apply: Applying {useCase} intensity multiplier: {intensityMultiplier} to {gameObject.name}");
            foreach (KeyValuePair<Material, MaterialSettings> emitterMaterial in emitterMaterials)
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
