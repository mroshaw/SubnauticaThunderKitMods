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
            FindLights();
        }

        private void OnEnable()
        {
            ConfigFile.ToolLightSettingsChanged += ApplyChanges;
            ApplyChanges(ConfigFile.ToolLightIntensity);
        }
        
        private void OnDisable()
        {
            ConfigFile.ToolLightSettingsChanged -= ApplyChanges;
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
                $"LightController.Apply: Applying intensity multiplier: {intensityMultiplier} to {gameObject.name}");
            
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
