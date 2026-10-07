using UnityEngine;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes
{
    /// <summary>
    /// MonoBehaviour class to manage flare light intensity
    /// </summary>
    internal class SaveMyEyesFlareController : MonoBehaviour
    {
        private Flare _flare;
        private float _lastGameIntensity;
        private bool _hasGameIntensity;

        private void Awake()
        {
            _flare = GetComponent<Flare>();
        }

        private void OnEnable()
        {
            ConfigFile.SettingsChanged += OnSettingsChanged;
            if (_hasGameIntensity)
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

        /// <summary>
        /// This is called in the FlarePatches.Update patch, so we can apply our mulitpliers
        /// as the game modifies the flares in real-time
        /// </summary>
        internal void PrepareCurrentIntensity()
        {
            if (_hasGameIntensity)
            {
                _flare.light.intensity = _lastGameIntensity;
            }
        }

        internal void CaptureAndApplyCurrentIntensity()
        {
            _lastGameIntensity = _flare.light.intensity;
            _hasGameIntensity = true;
            ApplyChanges(ConfigFile.FlareIntensity);
        }

        private void ApplyChanges(float intensityMultiplier)
        {
            if (!_hasGameIntensity)
            {
                return;
            }

            if (_flare.flareActiveState)
            {
                _flare.light.intensity = _lastGameIntensity * intensityMultiplier;
            }
            else
            {
                _flare.light.intensity = 0.0f;
            }
        }
    }
}
