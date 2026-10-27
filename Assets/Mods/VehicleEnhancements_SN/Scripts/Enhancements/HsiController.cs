using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using static DaftAppleGames.VehicleEnhancements_SN.VehicleEnhancementsPlugin_SN;

namespace DaftAppleGames.VehicleEnhancements_SN.Enhancements
{
    internal class HsiController : MonoBehaviour
    {
        [BoxGroup("HSI UI Settings")] [SerializeField, Required] private GameObject inclinometerRoot;
        [BoxGroup("HSI UI Settings")] [SerializeField, Required] private RectTransform rollHinge;
        [BoxGroup("HSI UI Settings")] [SerializeField, Required] private TextMeshProUGUI pitchText;
        [BoxGroup("HSI UI Settings")] [SerializeField, Required] private TextMeshProUGUI rollText;
        [BoxGroup("HSI Settings")] [SerializeField, MinValue(0.1f)] private float pitchUnitsPerDegree = 2.6f;
        [BoxGroup("HSI Settings")][SerializeField, MinValue(0.0f)] private float smoothTime = 0.12f;
        
        [BoxGroup("Vehicle HUD Positioning")] [SerializeField] private Vector3 cyclopsLocalRotation = new Vector3(0.0f, 0.0f, 1.0f);

        // Preserve the authored hinge baseline when VF clones an already-updated HUD.
        [SerializeField, HideInInspector] private Vector2 hingeInitialPosition;
        [SerializeField, HideInInspector] private bool hasHingeInitialPosition;
        private float smoothedPitch;
        private float smoothedRoll;
        private float pitchVelocity;
        private float rollVelocity;
        private int lastDisplayedPitchDegrees = int.MinValue;
        private int lastDisplayedRollDegrees = int.MinValue;
        private EnhancedVehicle vehicle;

        internal Vector3 CyclopsLocalRotation => cyclopsLocalRotation;

        internal void Configure(EnhancedVehicle selectedVehicle)
        {
            vehicle = selectedVehicle;
            if (!inclinometerRoot)
            {
                return;
            }

            RectTransform inclinometerRect = inclinometerRoot.transform as RectTransform;
            if (!inclinometerRect)
            {
                return;
            }

            if (vehicle != EnhancedVehicle.Cyclops)
            {
                ConfigureLayout(inclinometerRoot, VehicleHudElementLayout.Element.Hsi);
            }
        }

        private void ConfigureLayout(GameObject root, VehicleHudElementLayout.Element element)
        {
            VehicleHudElementLayout layout = root.GetComponent<VehicleHudElementLayout>();
            if (!layout)
            {
                layout = root.AddComponent<VehicleHudElementLayout>();
            }

            layout.Configure(vehicle, element);
        }

        private void Awake()
        {
            if (!inclinometerRoot || !rollHinge || !pitchText || !rollText)
            {
                ModDebugLog.LogError("Could not find the inclinometer objects.");
                enabled = false;
                return;
            }

            if (!hasHingeInitialPosition)
            {
                hingeInitialPosition = rollHinge.anchoredPosition;
                hasHingeInitialPosition = true;
            }
            inclinometerRoot.SetActive(ConfigFile.IsHsiEnabled(vehicle));
            SetDisplay(0.0f, 0.0f);
            SetNumericDisplay(0.0f, 0.0f);
        }

        private void Update()
        {
            bool showInclinometer = ConfigFile.IsHsiEnabled(vehicle);
            if (inclinometerRoot.activeSelf != showInclinometer)
            {
                inclinometerRoot.SetActive(showInclinometer);
            }

            if (!showInclinometer)
            {
                return;
            }

            float pitch = 0.0f;
            float roll = 0.0f;
            if (VehicleMotion.TryGet(vehicle, out Transform vehicleTransform, out _))
            {
                Calculate(
                    vehicleTransform.forward,
                    vehicleTransform.up,
                    out pitch,
                    out roll);
            }

            smoothedPitch = Mathf.SmoothDamp(smoothedPitch, pitch, ref pitchVelocity, smoothTime);
            smoothedRoll = Mathf.SmoothDampAngle(smoothedRoll, roll, ref rollVelocity, smoothTime);
            SetDisplay(smoothedPitch, smoothedRoll);
            SetNumericDisplay(pitch, roll);
        }

        private void SetDisplay(float pitch, float roll)
        {
            rollHinge.anchoredPosition = new Vector2(
                hingeInitialPosition.x,
                hingeInitialPosition.y - pitch * pitchUnitsPerDegree);
            rollHinge.localRotation = Quaternion.Euler(0.0f, 0.0f, roll);
        }

        private void SetNumericDisplay(float pitch, float roll)
        {
            int pitchDegrees = Mathf.RoundToInt(pitch);
            if (pitchDegrees != lastDisplayedPitchDegrees)
            {
                lastDisplayedPitchDegrees = pitchDegrees;
                SetAngleText(pitchText, pitchDegrees);
            }

            int rollDegrees = Mathf.RoundToInt(roll);
            if (rollDegrees != lastDisplayedRollDegrees)
            {
                lastDisplayedRollDegrees = rollDegrees;
                SetAngleText(rollText, rollDegrees);
            }
        }

        private static void SetAngleText(TextMeshProUGUI angleText, int angleDegrees)
        {
            int absoluteDegrees = Mathf.Abs(angleDegrees);
            angleText.SetText(angleDegrees < 0 ? "-{0}°" : "+{0}°", absoluteDegrees);
        }
        
        /// <summary>
        /// Returns nose-up pitch and right-side-down roll in signed degrees.
        /// </summary>
        private void Calculate(Vector3 forward, Vector3 up, out float pitch, out float roll)
        {
            forward.Normalize();
            up.Normalize();

            pitch = Mathf.Asin(Mathf.Clamp(forward.y, -1.0f, 1.0f)) * Mathf.Rad2Deg;

            Vector3 levelUp = Vector3.ProjectOnPlane(Vector3.up, forward);
            if (levelUp.sqrMagnitude < 0.000001f)
            {
                // Roll has no horizon reference when the nose points straight up or down.
                roll = 0.0f;
                return;
            }

            roll = -Vector3.SignedAngle(levelUp, up, forward);
        }
    }
}
