using UnityEngine;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes
{
    /// <summary>
    /// Suppresses filtration beams while preserving their game-controlled activity.
    /// </summary>
    internal class SaveMyEyesWaterFiltrationController : MonoBehaviour
    {
        private BaseFiltrationMachineGeometry filtrationMachine;
        private bool beamsActive;

        private void Awake()
        {
            filtrationMachine = GetComponent<BaseFiltrationMachineGeometry>();
            FindBeamState();
        }

        private void OnEnable()
        {
            ConfigFile.SettingsChanged += OnSettingsChanged;
            ApplyChanges(ConfigFile.DisableWaterFiltrationBeams);
        }

        private void OnDisable()
        {
            ConfigFile.SettingsChanged -= OnSettingsChanged;
        }

        private void OnSettingsChanged(EffectUseCase useCase)
        {
            if (useCase == EffectUseCase.WaterFiltration)
            {
                ApplyChanges(ConfigFile.DisableWaterFiltrationBeams);
            }
        }

        private void FindBeamState()
        {
            beamsActive = false;
            foreach (Transform beam in filtrationMachine.beams)
            {
                if (beam.gameObject.activeSelf)
                {
                    beamsActive = true;
                    break;
                }
            }
        }

        internal void SetBeamsActive(bool beamsActive)
        {
            this.beamsActive = beamsActive;
            ApplyChanges(ConfigFile.DisableWaterFiltrationBeams);
        }

        private void ApplyChanges(bool disableBeams)
        {
            bool enableBeams = beamsActive && !disableBeams;
            foreach (Transform beam in filtrationMachine.beams)
            {
                if (beam.gameObject.activeSelf != enableBeams)
                {
                    ModDebugLog.LogDebug(
                        $"WaterFiltrationController.Apply: Setting {gameObject.name} beam state to {enableBeams}");
                    beam.gameObject.SetActive(enableBeams);
                }
            }
        }
    }
}
