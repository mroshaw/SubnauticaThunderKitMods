using UnityEngine;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes
{
    /// <summary>
    /// Applies the selected flare light mode and intensity multiplier.
    /// </summary>
    internal class SaveMyEyesFlareController : MonoBehaviour
    {
        private Flare flare;
        private float lastGameIntensity;
        private bool hasGameIntensity;

        private void Awake()
        {
            flare = GetComponent<Flare>();
        }

        private void OnEnable()
        {
            ConfigFile.SettingsChanged += OnSettingsChanged;
            if (hasGameIntensity)
            {
                ApplyChanges(ConfigFile.FlareIntensity);
            }
        }

        private void OnDisable()
        {
            ConfigFile.SettingsChanged -= OnSettingsChanged;
        }

        private void OnSettingsChanged(EffectUseCase useCase)
        {
            if (useCase == EffectUseCase.Flare)
            {
                ApplyChanges(ConfigFile.FlareIntensity);
            }
        }

        // Vanilla smoothing must receive its own unscaled intensity on the next update.
        internal void PrepareCurrentIntensity()
        {
            if (hasGameIntensity)
            {
                flare.light.intensity = lastGameIntensity;
            }
        }

        internal void CaptureAndApplyCurrentIntensity()
        {
            lastGameIntensity = flare.light.intensity;
            hasGameIntensity = true;
            ApplyChanges(ConfigFile.FlareIntensity);
        }

        internal static float GetSmoothIntensity(float energyLeft)
        {
            // The shipped prefab starts with 1800 energy; preserve the reference envelope.
            if (energyLeft > 1790.0f)
            {
                return Mathf.Lerp(0.2f, 2.0f, (1800.0f - energyLeft) / 10.0f);
            }
            return energyLeft < 1000.0f ? Mathf.Lerp(1.0f, 2.0f, energyLeft / 1000.0f) : 2.0f;
        }

        private void ApplyChanges(float intensityMultiplier)
        {
            if (!hasGameIntensity)
            {
                return;
            }

            if (flare.flareActiveState)
            {
                float intensity = ConfigFile.RemoveFlareFlicker ? GetSmoothIntensity(flare.energyLeft) : lastGameIntensity;
                flare.light.intensity = intensity * intensityMultiplier;
            }
            else
            {
                flare.light.intensity = 0.0f;
            }
        }
    }
}
