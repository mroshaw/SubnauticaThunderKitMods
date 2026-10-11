using System.Collections.Generic;
using UnityEngine;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes
{
    /// <summary>
    /// Scales supported lights from their captured game intensity.
    /// </summary>
    internal class SaveMyEyesLightController : MonoBehaviour
    {
        private EffectUseCase useCase;

        private Dictionary<Light, float> lights;

        private void Awake()
        {
            // Initialize may already have captured an inactive object's lights.
            if (lights is null)
            {
                FindLights();
            }
        }

        private void OnEnable()
        {
            ConfigFile.SettingsChanged += OnSettingsChanged;
            if (useCase != EffectUseCase.None)
            {
                ApplyChanges(ConfigFile.GetLightIntensity(useCase));
            }
        }
        
        private void OnDisable()
        {
            ConfigFile.SettingsChanged -= OnSettingsChanged;
        }

        internal void Initialize(EffectUseCase useCase)
        {
            // An inactive light may not have run Awake when its controller is attached.
            if (lights is null)
            {
                FindLights();
            }
            this.useCase = useCase;
            if (isActiveAndEnabled)
            {
                ApplyChanges(ConfigFile.GetLightIntensity(this.useCase));
            }
        }

        internal void PrepareCurrentIntensity(Light light)
        {
            light.intensity = lights[light];
        }

        internal void CaptureAndApplyCurrentIntensity(Light light)
        {
            lights[light] = light.intensity;
            light.intensity *= ConfigFile.GetLightIntensity(useCase);
        }

        private void OnSettingsChanged(EffectUseCase useCase)
        {
            if (this.useCase == useCase)
            {
                ApplyChanges(ConfigFile.GetLightIntensity(this.useCase));
            }
        }

        private void FindLights()
        {
            lights = new Dictionary<Light, float>();
            Light[] allLights = GetComponentsInChildren<Light>(true);
            foreach (Light foundLight in allLights)
            {
                lights.Add(foundLight, foundLight.intensity);
            }
        }

        private void ApplyChanges(float intensityMultiplier)
        {
            if (lights is null)
            {
                return;
            }
            
            ModDebugLog.LogDebug(
                $"LightController.Apply: Applying {useCase} intensity multiplier: {intensityMultiplier} to {gameObject.name}");
            
            foreach (KeyValuePair<Light, float> managedLight in lights)
            {
                if (!managedLight.Key)
                {
                    ModDebugLog.LogDebug($"LightController.Apply: Light is null!");
                    continue;
                }
                
                float newIntensity = managedLight.Value * intensityMultiplier;
                managedLight.Key.intensity = newIntensity;
            }
        }
    }
}
