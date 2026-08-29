using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DaftAppleGames.CuddleCam_SN.CuddleCamPluginPlugin;

namespace DaftAppleGames.CuddleCam_SN
{
    public class CuddleCamMonitor : MonoBehaviour, IConstructable
    {
        private const string NoFeedText = "NONE";
        private const string NoSignalText = "NO SIGNAL";
        private const float DistanceCheckInterval = 0.5f;
        private const float OffscreenGracePeriod = 1f;

        [Header("UI Settings")]
        [SerializeField] private TMP_Text currentFeedText;
        [SerializeField] private RawImage currentFeedImage;
        [SerializeField] private GameObject noSignalDisplay;
        [SerializeField] private GameObject signalLostDisplay;
        [SerializeField] private Renderer visibilityRenderer;
        [SerializeField] private GameObject monitorCanvas;

        [Header("Custom Texture Settings")]
        [SerializeField] private GameObject modelPlaceholder;

        internal GameObject ModelPlaceholder => modelPlaceholder;
        
        private readonly Plane[] frustumPlanes = new Plane[6];
        private Constructable constructable;
        private CuddleCamSource selectedSource;
        private CuddleCamSource viewingSource;
        private bool hasSelectedSource;
        private float nextDistanceCheckTime;
        private float offscreenSince = -1f;

        private void Awake()
        {
            constructable = GetComponent<Constructable>();
            if (!visibilityRenderer)
            {
                visibilityRenderer = GetComponentInChildren<MeshRenderer>(true);
            }
        }

        private void OnEnable()
        {
            SetCanvasActive(!constructable || constructable.constructed);
            ModDebugLog.LogDebug(
                $"CuddleCamMonitor.OnEnable on '{gameObject.name}'. ManagerInstance={CuddleCamManager.Instance}.");

            if (!CuddleCamManager.Instance)
            {
                ModDebugLog.LogDebug("Monitor enabled without a CuddleCamManager; displaying no feed.");
                SetFeedText(hasSelectedSource ? NoSignalText : NoFeedText);
                RefreshDisplayState();
                return;
            }

            CuddleCamManager.Instance.SourcesChanged += HandleSourcesChanged;

            RefreshMonitor();
        }

        private void OnDisable()
        {
            if (CuddleCamManager.Instance)
            {
                CuddleCamManager.Instance.SourcesChanged -= HandleSourcesChanged;
            }

            ReleaseCurrentFeed();
        }

        void IConstructable.OnConstructedChanged(bool constructed)
        {
            SetCanvasActive(constructed);
        }

        bool IObstacle.IsDeconstructionObstacle()
        {
            return true;
        }

        bool IObstacle.CanDeconstruct(out string reason)
        {
            reason = null;
            return true;
        }

        private void Update()
        {
            if (Time.unscaledTime < nextDistanceCheckTime)
            {
                return;
            }

            nextDistanceCheckTime = Time.unscaledTime + DistanceCheckInterval;
            bool shouldRenderFeed = ShouldRenderFeed();
            if ((!shouldRenderFeed && viewingSource) ||
                (shouldRenderFeed && selectedSource && viewingSource != selectedSource))
            {
                RefreshMonitor();
            }
        }

        /// <summary>
        /// Selects the previous active Cuddlefish feed.
        /// </summary>
        public void PreviousFeed()
        {
            ModDebugLog.LogDebug($"Previous feed requested on monitor '{gameObject.name}'.");
            SelectAdjacentSource(-1);
        }

        /// <summary>
        /// Selects the next active Cuddlefish feed.
        /// </summary>
        public void NextFeed()
        {
            ModDebugLog.LogDebug($"Next feed requested on monitor '{gameObject.name}'.");
            SelectAdjacentSource(1);
        }

        private void SelectAdjacentSource(int direction)
        {
            if (!CuddleCamManager.Instance)
            {
                ModDebugLog.LogDebug("Cannot change monitor feed because no CuddleCamManager exists.");
                return;
            }

            CuddleCamSource nextSource =
                CuddleCamManager.Instance.GetAdjacentSource(selectedSource, direction);

            selectedSource = nextSource;
            hasSelectedSource = selectedSource;

            if (selectedSource)
            {
                ModDebugLog.LogDebug(
                    $"Monitor '{gameObject.name}' selected source '{selectedSource.name}' for " +
                    $"Cuddlefish='{selectedSource.Cuddlefish}'.");
            }
            else
            {
                ModDebugLog.LogDebug($"Monitor '{gameObject.name}' selected no feed.");
            }

            RefreshMonitor();
        }

        private void HandleSourcesChanged()
        {
            ModDebugLog.LogDebug($"Monitor '{gameObject.name}' received a sources-changed notification.");
            RefreshMonitor();
        }

        private void RefreshMonitor()
        {
            RefreshFeedBinding();
            RefreshFeedText();
            RefreshDisplayState();
        }

        private void RefreshFeedBinding()
        {
            bool selectedSourceIsActive =
                CuddleCamManager.Instance &&
                selectedSource &&
                CuddleCamManager.Instance.GetSourceNumber(selectedSource) > 0 &&
                ShouldRenderFeed();

            if (!selectedSourceIsActive)
            {
                ReleaseCurrentFeed();
                return;
            }

            if (viewingSource == selectedSource)
            {
                return;
            }

            ReleaseCurrentFeed();
            RenderTexture selectedTexture = selectedSource.AcquireFeed();
            if (!selectedTexture)
            {
                return;
            }

            viewingSource = selectedSource;
            if (currentFeedImage)
            {
                currentFeedImage.texture = selectedTexture;
                currentFeedImage.enabled = true;
                ModDebugLog.LogDebug(
                    $"Monitor '{gameObject.name}' bound to RenderTexture '{selectedTexture.name}'.");
                return;
            }

            ModDebugLog.LogError($"Monitor '{gameObject.name}' has no feed image reference.");
            ReleaseCurrentFeed();
        }

        private void ReleaseCurrentFeed()
        {
            if (viewingSource)
            {
                viewingSource.ReleaseFeed();
                viewingSource = null;
            }

            if (currentFeedImage)
            {
                currentFeedImage.texture = null;
                currentFeedImage.enabled = false;
            }
        }

        private bool ShouldRenderFeed()
        {
            if (ConfigFile == null || !Player.main)
            {
                return true;
            }

            if (ConfigFile.PauseDistantMonitors)
            {
                float activationDistance = Mathf.Max(1f, ConfigFile.MonitorActivationDistance);
                if ((Player.main.transform.position - transform.position).sqrMagnitude >
                    activationDistance * activationDistance)
                {
                    return false;
                }
            }

            if (!ConfigFile.PauseOffscreenMonitors)
            {
                offscreenSince = -1f;
                return true;
            }

            Camera playerCamera = MainCamera.camera;
            if (!playerCamera || !visibilityRenderer)
            {
                return true;
            }

            GeometryUtility.CalculateFrustumPlanes(playerCamera, frustumPlanes);
            if (GeometryUtility.TestPlanesAABB(frustumPlanes, visibilityRenderer.bounds))
            {
                offscreenSince = -1f;
                return true;
            }

            if (offscreenSince < 0f)
            {
                offscreenSince = Time.unscaledTime;
            }

            return Time.unscaledTime - offscreenSince < OffscreenGracePeriod;
        }

        private void RefreshFeedText()
        {
            if (!CuddleCamManager.Instance)
            {
                SetFeedText(hasSelectedSource ? NoSignalText : NoFeedText);
                return;
            }

            int sourceNumber = CuddleCamManager.Instance.GetSourceNumber(selectedSource);
            if (sourceNumber > 0)
            {
                SetFeedText($"Cuddlefish #{sourceNumber}");
                return;
            }

            SetFeedText(hasSelectedSource ? NoSignalText : NoFeedText);
        }

        private void RefreshDisplayState()
        {
            bool hasLiveFeed =
                viewingSource &&
                currentFeedImage &&
                currentFeedImage.texture;

            SetDisplayActive(noSignalDisplay, !hasSelectedSource, "No Signal");
            SetDisplayActive(signalLostDisplay, hasSelectedSource && !hasLiveFeed, "Signal Lost");
        }

        private void SetDisplayActive(GameObject display, bool active, string displayName)
        {
            if (!display)
            {
                ModDebugLog.LogError(
                    $"Monitor '{gameObject.name}' has no {displayName} display reference.");
                return;
            }

            if (display.activeSelf != active)
            {
                display.SetActive(active);
                ModDebugLog.LogDebug(
                    $"Monitor '{gameObject.name}' {displayName} display active={active}.");
            }
        }

        private void SetCanvasActive(bool active)
        {
            if (!monitorCanvas)
            {
                ModDebugLog.LogError(
                    $"Monitor '{gameObject.name}' has no Canvas reference.");
                return;
            }

            if (monitorCanvas.activeSelf != active)
            {
                monitorCanvas.SetActive(active);
                ModDebugLog.LogDebug(
                    $"Monitor '{gameObject.name}' Canvas active={active}.");
            }
        }

        private void SetFeedText(string text)
        {
            if (currentFeedText)
            {
                ModDebugLog.LogDebug($"Monitor '{gameObject.name}' feed text set to '{text}'.");
                currentFeedText.text = text;
                return;
            }

            ModDebugLog.LogError($"Monitor '{gameObject.name}' has no feed text reference.");
        }
    }
}
