using UnityEngine;
using static DaftAppleGames.VehicleEnhancements_SN.VehicleEnhancementsPlugin_SN;

namespace DaftAppleGames.VehicleEnhancements_SN.Enhancements
{
    // Run after VehicleHudLayout has aligned the shared indicators container.
    [DefaultExecutionOrder(100)]
    internal class VehicleHudElementLayout : MonoBehaviour
    {
        internal enum Element
        {
            Speedometer,
            Hsi,
            TimeAndWeather
        }

        private const float EdgePadding = 10.0f;
        private readonly Vector3[] corners = new Vector3[4];
        private RectTransform elementRect;
        private RectTransform parentRect;
        private RectTransform canvasRect;
        private RectTransform[] contentRects;
        private Element element;
        private EnhancedVehicle vehicle;
        private RectTransform leftCanvas;
        private RectTransform centerCanvas;
        private RectTransform rightCanvas;
        private HudLocation lastLocation;
        private bool hasLayout;
        private Vector2 lastAdjustment;
        private float lastScale;
        private Rect lastParentRect;
        private Rect lastCanvasRect;
        private Matrix4x4 lastParentToCanvas;

        internal void Configure(EnhancedVehicle selectedVehicle, Element selectedElement,
            RectTransform left = null, RectTransform center = null, RectTransform right = null)
        {
            vehicle = selectedVehicle;
            element = selectedElement;
            elementRect = transform as RectTransform;
            if (!elementRect)
            {
                return;
            }

            leftCanvas = left;
            centerCanvas = center;
            rightCanvas = right;
            parentRect = transform.parent as RectTransform;
            hasLayout = false;
            canvasRect = null;
            Canvas canvas = GetComponentInParent<Canvas>();
            // World-space Cyclops panels can intentionally extend beyond their canvas rectangle.
            if (canvas && vehicle != EnhancedVehicle.Cyclops)
            {
                canvasRect = canvas.rootCanvas.transform as RectTransform;
            }

            // The dial roots have zero-sized rects. Their immediate children define the
            // visible footprint; deeper descendants include oversized, masked HSI artwork.
            contentRects = new RectTransform[transform.childCount + 1];
            contentRects[0] = elementRect;
            for (int index = 0; index < transform.childCount; index++)
            {
                contentRects[index + 1] = transform.GetChild(index) as RectTransform;
            }
        }

        private void LateUpdate()
        {
            ApplyLayout();
        }

        private void ApplyLayout()
        {
            if (!elementRect || !parentRect || contentRects is null)
            {
                return;
            }

            GetAdjustment(out HudLocation location, out Vector2 adjustment, out float scale);
            if (vehicle == EnhancedVehicle.Cyclops)
            {
                RectTransform selectedCanvas = location == HudLocation.Left ? leftCanvas
                    : location == HudLocation.Center ? centerCanvas : rightCanvas;
                if (!selectedCanvas)
                {
                    return;
                }

                if (parentRect != selectedCanvas)
                {
                    elementRect.SetParent(selectedCanvas, false);
                    parentRect = selectedCanvas;
                    hasLayout = false;
                }
            }

            adjustment.x = Mathf.Clamp(adjustment.x, ModConfigFile.MinimumHudAdjust, ModConfigFile.MaximumHudAdjust);
            adjustment.y = Mathf.Clamp(adjustment.y, ModConfigFile.MinimumHudAdjust, ModConfigFile.MaximumHudAdjust);
            scale = Mathf.Clamp(scale, ModConfigFile.MinimumHudScale, ModConfigFile.MaximumHudScale);
            Rect canvasBounds = canvasRect ? canvasRect.rect : new Rect();
            Matrix4x4 parentToCanvas = canvasRect
                ? canvasRect.worldToLocalMatrix * parentRect.localToWorldMatrix
                : parentRect.localToWorldMatrix;
            if (hasLayout && location == lastLocation && adjustment == lastAdjustment && scale == lastScale &&
                (vehicle == EnhancedVehicle.Cyclops ||
                 parentRect.rect == lastParentRect && canvasBounds == lastCanvasRect && parentToCanvas == lastParentToCanvas))
            {
                return;
            }

            hasLayout = true;
            lastLocation = location;
            lastAdjustment = adjustment;
            lastScale = scale;
            lastParentRect = parentRect.rect;
            lastCanvasRect = canvasBounds;
            lastParentToCanvas = parentToCanvas;
            if (vehicle == EnhancedVehicle.Cyclops)
            {
                // Keep the cockpit panel rotation while positioning relative to its canvas center.
                elementRect.localPosition = new Vector3(adjustment.x, adjustment.y, 0.0f);
            }
            else
            {
                // Use the existing left/right HUD reference points for every control.
                Vector2 anchor = location == HudLocation.Left ? new Vector2(0.15f, 0.23f)
                    : location == HudLocation.Center ? new Vector2(0.5f, 0.3f) : new Vector2(0.9f, 0.3f);
                elementRect.anchorMin = anchor;
                elementRect.anchorMax = anchor;
                elementRect.anchoredPosition = adjustment;
            }

            // Parent HUD scaling is inherited, so it is not multiplied again here.
            elementRect.localScale = Vector3.one * scale;
            KeepOnScreen();
        }

        private void GetAdjustment(out HudLocation location, out Vector2 adjustment, out float scale)
        {
            switch (vehicle)
            {
                case EnhancedVehicle.PrawnSuit:
                    switch (element)
                    {
                        case Element.Hsi:
                            location = ConfigFile.PrawnHsiLocation;
                            adjustment = new Vector2(ConfigFile.PrawnHsiHorizontalAdjust, ConfigFile.PrawnHsiVerticalAdjust);
                            scale = ConfigFile.PrawnHsiUiScale;
                            return;
                        case Element.TimeAndWeather:
                            location = ConfigFile.PrawnTimeLocation;
                            adjustment = new Vector2(ConfigFile.PrawnTimeHorizontalAdjust, ConfigFile.PrawnTimeVerticalAdjust);
                            scale = ConfigFile.PrawnTimeUiScale;
                            return;
                        default:
                            location = ConfigFile.PrawnSpeedometerLocation;
                            adjustment = new Vector2(ConfigFile.PrawnSpeedometerHorizontalAdjust, ConfigFile.PrawnSpeedometerVerticalAdjust);
                            scale = ConfigFile.PrawnSpeedometerUiScale;
                            return;
                    }
                case EnhancedVehicle.Cyclops:
                    switch (element)
                    {
                        case Element.Hsi:
                            location = ConfigFile.CyclopsHsiLocation;
                            adjustment = new Vector2(ConfigFile.CyclopsHsiHorizontalAdjust, ConfigFile.CyclopsHsiVerticalAdjust);
                            scale = ConfigFile.CyclopsHsiUiScale;
                            return;
                        case Element.TimeAndWeather:
                            location = ConfigFile.CyclopsTimeLocation;
                            adjustment = new Vector2(ConfigFile.CyclopsTimeHorizontalAdjust, ConfigFile.CyclopsTimeVerticalAdjust);
                            scale = ConfigFile.CyclopsTimeUiScale;
                            return;
                        default:
                            location = ConfigFile.CyclopsSpeedometerLocation;
                            adjustment = new Vector2(ConfigFile.CyclopsSpeedometerHorizontalAdjust, ConfigFile.CyclopsSpeedometerVerticalAdjust);
                            scale = ConfigFile.CyclopsSpeedometerUiScale;
                            return;
                    }
                case EnhancedVehicle.CustomVehicle:
                    CustomVehicleSettings settings = ConfigFile.CurrentCustomVehicleSettings;
                    switch (element)
                    {
                        case Element.Hsi:
                            location = settings.HsiLocation;
                            adjustment = new Vector2(settings.HsiHorizontalAdjust,
                                settings.HsiVerticalAdjust);
                            scale = settings.HsiUiScale;
                            return;
                        case Element.TimeAndWeather:
                            location = settings.TimeLocation;
                            adjustment = new Vector2(settings.TimeHorizontalAdjust,
                                settings.TimeVerticalAdjust);
                            scale = settings.TimeUiScale;
                            return;
                        default:
                            location = settings.SpeedometerLocation;
                            adjustment = new Vector2(settings.SpeedometerHorizontalAdjust,
                                settings.SpeedometerVerticalAdjust);
                            scale = settings.SpeedometerUiScale;
                            return;
                    }
                default:
                    switch (element)
                    {
                        case Element.Hsi:
                            location = ConfigFile.SeamothHsiLocation;
                            adjustment = new Vector2(ConfigFile.SeamothHsiHorizontalAdjust, ConfigFile.SeamothHsiVerticalAdjust);
                            scale = ConfigFile.SeamothHsiUiScale;
                            return;
                        case Element.TimeAndWeather:
                            location = ConfigFile.SeamothTimeLocation;
                            adjustment = new Vector2(ConfigFile.SeamothTimeHorizontalAdjust, ConfigFile.SeamothTimeVerticalAdjust);
                            scale = ConfigFile.SeamothTimeUiScale;
                            return;
                        default:
                            location = ConfigFile.SeamothSpeedometerLocation;
                            adjustment = new Vector2(ConfigFile.SeamothSpeedometerHorizontalAdjust, ConfigFile.SeamothSpeedometerVerticalAdjust);
                            scale = ConfigFile.SeamothSpeedometerUiScale;
                            return;
                    }
            }
        }

        private void KeepOnScreen()
        {
            if (!canvasRect)
            {
                return;
            }

            Rect available = canvasRect.rect;
            available.xMin += EdgePadding;
            available.xMax -= EdgePadding;
            available.yMin += EdgePadding;
            available.yMax -= EdgePadding;
            if (available.width <= 0.0f || available.height <= 0.0f)
            {
                return;
            }

            Bounds bounds = GetContentBounds();
            float fitScale = Mathf.Min(1.0f, Mathf.Min(
                available.width / Mathf.Max(bounds.size.x, 0.001f),
                available.height / Mathf.Max(bounds.size.y, 0.001f)));
            if (fitScale < 1.0f)
            {
                elementRect.localScale *= fitScale;
                bounds = GetContentBounds();
            }

            Vector3 shift = new Vector3(
                Mathf.Clamp(bounds.center.x, available.xMin + bounds.extents.x, available.xMax - bounds.extents.x) - bounds.center.x,
                Mathf.Clamp(bounds.center.y, available.yMin + bounds.extents.y, available.yMax - bounds.extents.y) - bounds.center.y,
                0.0f);
            elementRect.position += canvasRect.TransformVector(shift);
        }

        private Bounds GetContentBounds()
        {
            Bounds bounds = new Bounds(canvasRect.InverseTransformPoint(elementRect.position), Vector3.zero);
            foreach (RectTransform contentRect in contentRects)
            {
                if (!contentRect)
                {
                    continue;
                }

                contentRect.GetWorldCorners(corners);
                foreach (Vector3 corner in corners)
                {
                    bounds.Encapsulate(canvasRect.InverseTransformPoint(corner));
                }
            }

            return bounds;
        }
    }
}
