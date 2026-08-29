using UnityEngine;
using static DaftAppleGames.CuddleCam_SN.CuddleCamPluginPlugin;

namespace DaftAppleGames.CuddleCam_SN
{
    public class CuddleCamSource : MonoBehaviour
    {
        private const int RenderTextureDepth = 24;
        private const int DefaultFeedRefreshRate = 30;
        private const int LowRenderTextureWidth = 256;
        private const int LowRenderTextureHeight = 144;
        private const int MediumRenderTextureWidth = 512;
        private const int MediumRenderTextureHeight = 288;
        private const int HighRenderTextureWidth = 1024;
        private const int HighRenderTextureHeight = 576;

        [SerializeField] private Vector3 attachOffset = new Vector3(0f, 0f, 0.06f);
        [SerializeField] [Min(1)] private int feedRefreshRate = DefaultFeedRefreshRate;

        private Camera attachedCamera;
        private CuteFish cuddlefish;
        private RenderTexture renderTexture;
        private WaterscapeVolumeOnCamera waterscapeEffect;
        private bool waterscapeConfigured;
        private float nextRenderTime;
        private float renderPeriod;
        private int lastRenderedFrame = -1;
        private int viewerCount;

        internal Camera Camera => attachedCamera;
        internal CuteFish Cuddlefish => cuddlefish;

        /// <summary>
        /// Acquires the shared video feed for this Cuddlefish.
        /// </summary>
        internal RenderTexture AcquireFeed()
        {
            if (!attachedCamera)
            {
                ModDebugLog.LogError($"Cannot acquire feed from '{gameObject.name}' without a Camera component.");
                return null;
            }

            EnsureRenderTexture();

            ConfigureWaterscapeEffect();
            viewerCount++;
            attachedCamera.targetTexture = renderTexture;
            RenderFeed();
            RefreshRenderSchedule();
            ModDebugLog.LogDebug(
                $"Acquired feed from '{gameObject.name}'. ViewerCount={viewerCount}, " +
                $"FeedRefreshRate={feedRefreshRate} FPS.");
            return renderTexture;
        }

        /// <summary>
        /// Releases one viewer from this Cuddlefish video feed.
        /// </summary>
        internal void ReleaseFeed()
        {
            if (viewerCount > 0)
            {
                viewerCount--;
            }

            ModDebugLog.LogDebug($"Released feed from '{gameObject.name}'. ViewerCount={viewerCount}.");
            if (viewerCount == 0)
            {
                ReleaseRenderResources();
            }
        }

        private void Awake()
        {
            ModDebugLog.LogDebug(
                $"CuddleCamSource.Awake on '{gameObject.name}'. ActiveSelf={gameObject.activeSelf}, " +
                $"ActiveInHierarchy={gameObject.activeInHierarchy}.");

            attachedCamera = GetComponent<Camera>();
            if (!attachedCamera)
            {
                ModDebugLog.LogError($"CuddleCam source '{gameObject.name}' has no Camera component.");
                return;
            }

            attachedCamera.enabled = false;
            attachedCamera.targetTexture = null;
            RefreshRenderSchedule();
            ConfigureWaterscapeEffect();
            ModDebugLog.LogDebug(
                $"Camera on '{gameObject.name}' initialised disabled with a " +
                $"{feedRefreshRate} FPS feed refresh rate.");
        }

        private void Update()
        {
            int configuredFeedRefreshRate = GetConfiguredFeedRefreshRate();
            if (feedRefreshRate != configuredFeedRefreshRate)
            {
                RefreshRenderSchedule();
            }

            if (viewerCount == 0 || !renderTexture)
            {
                return;
            }

            if (EnsureRenderTexture())
            {
                RenderFeed();
            }

            if (Time.unscaledTime >= nextRenderTime)
            {
                RenderFeed();
                nextRenderTime = Time.unscaledTime + renderPeriod;
            }
        }

        private int GetConfiguredFeedRefreshRate()
        {
            if (ConfigFile != null)
            {
                return Mathf.Max(1, ConfigFile.FeedRefreshRate);
            }

            return Mathf.Max(1, feedRefreshRate);
        }

        private void RefreshRenderSchedule()
        {
            feedRefreshRate = GetConfiguredFeedRefreshRate();
            renderPeriod = 1f / feedRefreshRate;

            int positiveInstanceId = GetInstanceID() & int.MaxValue;
            float stagger = (positiveInstanceId % 1000) / 1000f * renderPeriod;
            nextRenderTime = Time.unscaledTime + stagger;
        }

        private bool EnsureRenderTexture()
        {
            int width;
            int height;
            GetConfiguredRenderTextureSize(out width, out height);

            if (!renderTexture)
            {
                renderTexture = new RenderTexture(
                    width,
                    height,
                    RenderTextureDepth,
                    RenderTextureFormat.ARGB32)
                {
                    name = $"CuddleCamFeed_{GetInstanceID()}",
                    antiAliasing = 1,
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp,
                    useMipMap = false,
                    autoGenerateMips = false
                };
                renderTexture.Create();
                ModDebugLog.LogDebug(
                    $"Created RenderTexture '{renderTexture.name}' at {width}x{height}.");
                return true;
            }

            if (renderTexture.width == width && renderTexture.height == height)
            {
                return false;
            }

            renderTexture.Release();
            renderTexture.width = width;
            renderTexture.height = height;
            renderTexture.Create();
            ModDebugLog.LogDebug(
                $"Resized RenderTexture '{renderTexture.name}' to {width}x{height}.");
            return true;
        }

        private void GetConfiguredRenderTextureSize(out int width, out int height)
        {
            string quality = ConfigFile != null ? ConfigFile.FeedQuality : "Medium";
            switch (quality)
            {
                case "Low":
                    width = LowRenderTextureWidth;
                    height = LowRenderTextureHeight;
                    return;
                case "High":
                    width = HighRenderTextureWidth;
                    height = HighRenderTextureHeight;
                    return;
                default:
                    width = MediumRenderTextureWidth;
                    height = MediumRenderTextureHeight;
                    return;
            }
        }

        private void OnEnable()
        {
            ModDebugLog.LogDebug(
                $"CuddleCamSource.OnEnable on '{gameObject.name}'. Cuddlefish={cuddlefish}, " +
                $"ManagerInstance={CuddleCamManager.Instance}.");
            RegisterWithManager();
        }

        private void OnDisable()
        {
            ModDebugLog.LogDebug($"CuddleCamSource.OnDisable on '{gameObject.name}'. Cuddlefish={cuddlefish}.");
            viewerCount = 0;
            ReleaseRenderResources();
            UnregisterFromManager();
        }

        private void OnDestroy()
        {
            viewerCount = 0;
            ReleaseRenderResources();
        }

        internal void AttachTo(CuteFish newCuddlefish)
        {
            ModDebugLog.LogDebug(
                $"Attaching CuddleCam source '{gameObject.name}' to Cuddlefish={newCuddlefish}. " +
                $"SourceActiveAndEnabled={isActiveAndEnabled}, ManagerInstance={CuddleCamManager.Instance}.");

            if (!newCuddlefish)
            {
                ModDebugLog.LogError("Cannot attach a CuddleCam source without a Cuddlefish.");
                return;
            }

            UnregisterFromManager();

            cuddlefish = newCuddlefish;
            transform.SetParent(cuddlefish.transform, false);
            transform.localPosition = attachOffset;
            transform.localRotation = Quaternion.identity;

            ModDebugLog.LogDebug(
                $"CuddleCam source '{gameObject.name}' parented to '{cuddlefish.name}' at " +
                $"localPosition={transform.localPosition}, localRotation={transform.localRotation.eulerAngles}.");

            RegisterWithManager();
        }

        private void RegisterWithManager()
        {
            bool sourceActive = isActiveAndEnabled;
            bool hasCuddlefish = cuddlefish;
            bool hasManager = CuddleCamManager.Instance;

            ModDebugLog.LogDebug(
                $"RegisterWithManager for '{gameObject.name}': SourceActiveAndEnabled={sourceActive}, " +
                $"HasCuddlefish={hasCuddlefish}, HasManager={hasManager}.");

            if (!sourceActive || !hasCuddlefish || !hasManager)
            {
                ModDebugLog.LogDebug($"CuddleCam source '{gameObject.name}' registration deferred or skipped.");
                return;
            }

            CuddleCamManager.Instance.RegisterSource(this);
        }

        private void UnregisterFromManager()
        {
            ModDebugLog.LogDebug(
                $"UnregisterFromManager for '{gameObject.name}'. ManagerInstance={CuddleCamManager.Instance}.");

            if (CuddleCamManager.Instance)
            {
                CuddleCamManager.Instance.UnregisterSource(this);
            }
        }

        private void ConfigureWaterscapeEffect()
        {
            if (!attachedCamera)
            {
                return;
            }

            bool enableWaterscape = ConfigFile == null || ConfigFile.EnableWaterscapeEffects;
            if (!enableWaterscape)
            {
                if (waterscapeEffect)
                {
                    waterscapeEffect.enabled = false;
                }

                waterscapeConfigured = false;
                return;
            }

            if (waterscapeConfigured && waterscapeEffect && waterscapeEffect.enabled)
            {
                return;
            }

            if (!waterscapeEffect)
            {
                waterscapeEffect = attachedCamera.GetComponent<WaterscapeVolumeOnCamera>();
                if (!waterscapeEffect)
                {
                    waterscapeEffect = attachedCamera.gameObject.AddComponent<WaterscapeVolumeOnCamera>();
                }
            }

            Camera mainCamera = MainCamera.camera;
            if (!mainCamera)
            {
                waterscapeEffect.enabled = false;
                ModDebugLog.LogDebug($"Could not configure waterscape for '{gameObject.name}': MainCamera is unavailable.");
                return;
            }

            WaterscapeVolumeOnCamera mainWaterscapeEffect =
                mainCamera.GetComponent<WaterscapeVolumeOnCamera>();
            if (!mainWaterscapeEffect || !mainWaterscapeEffect.settings)
            {
                waterscapeEffect.enabled = false;
                ModDebugLog.LogDebug(
                    $"Could not configure waterscape for '{gameObject.name}': MainCamera has no waterscape settings.");
                return;
            }

            waterscapeEffect.settings = mainWaterscapeEffect.settings;
            waterscapeEffect.enabled = true;
            waterscapeConfigured = true;
            ModDebugLog.LogDebug($"Configured waterscape rendering for '{gameObject.name}'.");
        }

        private void RenderFeed()
        {
            if (!attachedCamera || !renderTexture || lastRenderedFrame == Time.frameCount)
            {
                return;
            }

            ConfigureWaterscapeEffect();
            attachedCamera.Render();
            lastRenderedFrame = Time.frameCount;
        }

        private void ReleaseRenderResources()
        {
            if (attachedCamera)
            {
                attachedCamera.enabled = false;
                attachedCamera.targetTexture = null;
            }

            if (!renderTexture)
            {
                return;
            }

            string textureName = renderTexture.name;
            renderTexture.Release();
            Destroy(renderTexture);
            renderTexture = null;
            ModDebugLog.LogDebug($"Released RenderTexture '{textureName}' for '{gameObject.name}'.");
        }
    }
}
