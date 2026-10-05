using UnityEngine;
using static DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyesUltimateEditionPlugin;

namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes
{
    /// <summary>
    /// MonoBehaviour class to manage Water Filtration beams
    /// </summary>
    internal class SaveMyEyesWaterFiltrationController : MonoBehaviour
    {
        private BaseFiltrationMachineGeometry _filtrationMachine;
        private bool _beamsActive;

        private void Awake()
        {
            _filtrationMachine = GetComponent<BaseFiltrationMachineGeometry>();
            FindBeamState();
        }

        private void OnEnable()
        {
            ConfigFile.WaterFiltrationBeamSettingsChanged += ApplyChanges;
            ApplyChanges(ConfigFile.DisableWaterFiltrationBeams);
        }

        private void OnDisable()
        {
            ConfigFile.WaterFiltrationBeamSettingsChanged -= ApplyChanges;
        }

        private void FindBeamState()
        {
            _beamsActive = false;
            foreach (Transform beam in _filtrationMachine.beams)
            {
                if (beam.gameObject.activeSelf)
                {
                    _beamsActive = true;
                    break;
                }
            }
        }

        internal void SetBeamsActive(bool beamsActive)
        {
            _beamsActive = beamsActive;
            ApplyChanges(ConfigFile.DisableWaterFiltrationBeams);
        }

        private void ApplyChanges(bool disableBeams)
        {
            bool enableBeams = _beamsActive && !disableBeams;
            foreach (Transform beam in _filtrationMachine.beams)
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
