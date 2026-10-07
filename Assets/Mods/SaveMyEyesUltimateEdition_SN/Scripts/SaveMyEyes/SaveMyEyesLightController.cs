using System.Collections.Generic;
using UnityEngine;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes
{
    /// <summary>
    /// MonoBehaviour class to manage Tool effect intensities
    /// </summary>
    internal class SaveMyEyesLightController : MonoBehaviour
    {
        [SerializeField] private EffectUseCase useCase;

        // Struct to hold initial values so that modifier can be applied consistently
        private readonly struct LightSettings
        {
            internal readonly float Intensity;

            internal LightSettings(float intensity)
            {
                Intensity = intensity;
            }
        }
        
        // List of lights to manage
        private Dictionary<Light, LightSettings> _lights;

        private void Awake()
        {
            // Initialize may already have captured an inactive object's lights.
            if (_lights == null)
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
            if (_lights == null)
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
            light.intensity = _lights[light].Intensity;
        }

        internal void CaptureAndApplyCurrentIntensity(Light light)
        {
            _lights[light] = new LightSettings(light.intensity);
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
            _lights = new Dictionary<Light, LightSettings>();
            Light[] allLights = GetComponentsInChildren<Light>(true);
            foreach (Light foundLight in allLights)
            {
                _lights.Add(foundLight, new LightSettings(foundLight.intensity));
            }
        }

        private void ApplyChanges(float intensityMultiplier)
        {
            // List is not yet initialised
            if (_lights == null)
            {
                return;
            }
            
            ModDebugLog.LogDebug(
                $"LightController.Apply: Applying {useCase} intensity multiplier: {intensityMultiplier} to {gameObject.name}");
            
            foreach (KeyValuePair<Light, LightSettings> managedLight in _lights)
            {
                if (!managedLight.Key)
                {
                    ModDebugLog.LogDebug($"LightController.Apply: Light is null!");
                    continue;
                }
                
                float newIntensity = managedLight.Value.Intensity * intensityMultiplier;
                managedLight.Key.intensity = newIntensity;
            }
        }
    }
}
