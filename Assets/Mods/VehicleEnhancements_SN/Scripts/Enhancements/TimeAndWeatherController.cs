using DaftAppleGames.VehicleEnhancements_SN.Patches;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DaftAppleGames.VehicleEnhancements_SN.VehicleEnhancementsPlugin_SN;

namespace DaftAppleGames.VehicleEnhancements_SN.Enhancements
{
    internal class TimeAndWeatherController : MonoBehaviour
    {
        [BoxGroup("Time and Weather UI Settings")] [SerializeField, Required] private GameObject timeAndWeatherRoot;
        [BoxGroup("Time and Weather UI Settings")] [SerializeField, Required] private TextMeshProUGUI timeText;
        [BoxGroup("Time and Weather UI Settings")] [SerializeField, Required] private RectTransform timeOfDayIndicator;
        [BoxGroup("Time and Weather UI Settings")] [SerializeField, Required] private Image timeImage;
        [BoxGroup("Time and Weather UI Settings")] [SerializeField, Required] private Image weatherImage;
        
        [BoxGroup("Time and Weather Settings")] [SerializeField, Required] private Sprite dayTimeSprite;
        [BoxGroup("Time and Weather Settings")] [SerializeField, Required] private Sprite nightTimeSprite;
        [BoxGroup("Time and Weather Settings")] [SerializeField, Required] private GameObject weatherIndicator;
        [BoxGroup("Time and Weather Settings")] [SerializeField, Required] private Sprite sunnyWeatherSprite;
        [BoxGroup("Time and Weather Settings")] [SerializeField, Required] private Sprite rainyWeatherSprite;
        [BoxGroup("Time and Weather Settings")] [SerializeField, Required] private Sprite lightningStormWeatherSprite;

        [BoxGroup("Vehicle HUD Positioning")] [SerializeField] private Vector2 seamothAnchoredPosition = new Vector2(-250.0f, 78.7f);
        [BoxGroup("Vehicle HUD Positioning")] [SerializeField] private Vector3 seamothLocalScale = Vector3.one;
        [BoxGroup("Vehicle HUD Positioning")] [SerializeField] private Vector2 prawnSuitAnchoredPosition = new Vector2(-250.0f, 78.7f);
        [BoxGroup("Vehicle HUD Positioning")] [SerializeField] private Vector3 prawnSuitLocalScale = Vector3.one;
        [BoxGroup("Vehicle HUD Positioning")] [SerializeField] private CyclopsHudLocation cyclopsHudLocation = CyclopsHudLocation.Left;
        [BoxGroup("Vehicle HUD Positioning")] [SerializeField] private Vector3 cyclopsLocalPosition = new Vector3(-85.0f, -385.0f, 0.0f);
        [BoxGroup("Vehicle HUD Positioning")] [SerializeField] private Vector3 cyclopsLocalRotation = new Vector3(0.0f, 0.0f, 1.0f);
        [BoxGroup("Vehicle HUD Positioning")] [SerializeField] private Vector3 cyclopsLocalScale = Vector3.one;

        private RectTransform previousMoon;
        private RectTransform nextMoon;
        private int lastDisplayedMinute = -1;
        private float nextWeatherUpdateTime;
        private WeatherManager weatherManager;
        private EnhancedVehicle vehicle;

        internal CyclopsHudLocation CyclopsHudLocation => cyclopsHudLocation;

        internal Vector3 CyclopsLocalPosition => cyclopsLocalPosition;

        internal Vector3 CyclopsLocalRotation => cyclopsLocalRotation;

        internal Vector3 CyclopsLocalScale => cyclopsLocalScale;

        internal void Configure(EnhancedVehicle selectedVehicle)
        {
            vehicle = selectedVehicle;
            if (!timeAndWeatherRoot)
            {
                return;
            }

            RectTransform timeAndWeatherRect = timeAndWeatherRoot.transform as RectTransform;
            if (!timeAndWeatherRect)
            {
                return;
            }

            switch (vehicle)
            {
                case EnhancedVehicle.PrawnSuit:
                    timeAndWeatherRect.anchoredPosition = prawnSuitAnchoredPosition;
                    timeAndWeatherRect.localScale = prawnSuitLocalScale;
                    break;
                case EnhancedVehicle.Cyclops:
                    break;
                default:
                    timeAndWeatherRect.anchoredPosition = seamothAnchoredPosition;
                    timeAndWeatherRect.localScale = seamothLocalScale;
                    break;
            }
        }

        private void Awake()
        {
            if (!timeAndWeatherRoot || !timeText || !timeOfDayIndicator || !timeImage ||
                !dayTimeSprite || !nightTimeSprite || !weatherIndicator || !weatherImage ||
                !sunnyWeatherSprite || !rainyWeatherSprite || !lightningStormWeatherSprite)
            {
                ModDebugLog.LogError("Could not find the time indicator objects or sprites.");
                enabled = false;
                return;
            }

            if (!timeOfDayIndicator.GetComponent<RectMask2D>())
            {
                timeOfDayIndicator.gameObject.AddComponent<RectMask2D>();
            }

            timeImage.sprite = dayTimeSprite;
            timeImage.raycastTarget = false;
            previousMoon = CreateMoon("PreviousMoon");
            nextMoon = CreateMoon("NextMoon");
            timeText.SetText("--:--");
            weatherImage.sprite = sunnyWeatherSprite;
            timeAndWeatherRoot.SetActive(ConfigFile.IsTimeAndWeatherEnabled(vehicle));
        }

        private void Update()
        {
            bool showTime = ConfigFile.IsTimeAndWeatherEnabled(vehicle);
            if (timeAndWeatherRoot.activeSelf != showTime)
            {
                timeAndWeatherRoot.SetActive(showTime);
            }

            DayNightCycle cycle = DayNightCycle.main;
            if (showTime && cycle)
            {
                UpdateTime(cycle.GetDayScalar());
            }

            if (showTime && Time.unscaledTime >= nextWeatherUpdateTime)
            {
                nextWeatherUpdateTime = Time.unscaledTime + 1.0f;
                UpdateWeather();
            }
        }

        private RectTransform CreateMoon(string objectName)
        {
            Image moonImage = Instantiate(timeImage, timeOfDayIndicator);
            moonImage.name = objectName;
            moonImage.sprite = nightTimeSprite;
            moonImage.raycastTarget = false;
            return moonImage.rectTransform;
        }

        private void UpdateTime(float dayFraction)
        {
            int minuteOfDay = Mathf.FloorToInt(dayFraction * 1440.0f);
            if (minuteOfDay != lastDisplayedMinute)
            {
                lastDisplayedMinute = minuteOfDay;
                timeText.SetText("{0:00}:{1:00}", minuteOfDay / 60, minuteOfDay % 60);
            }

            float viewportHeight = timeOfDayIndicator.rect.height;
            float sunPosition = (dayFraction * 2.0f - 1.0f) * viewportHeight;
            timeImage.rectTransform.anchoredPosition = new Vector2(0.0f, sunPosition);
            previousMoon.anchoredPosition = new Vector2(0.0f, sunPosition + viewportHeight);
            nextMoon.anchoredPosition = new Vector2(0.0f, sunPosition - viewportHeight);
        }

        private void UpdateWeather()
        {
            if (!weatherManager)
            {
                weatherManager = FindObjectOfType<WeatherManager>();
            }

            Sprite sprite = sunnyWeatherSprite;
            if (weatherManager)
            {
                if (weatherManager._outputLightningScalar > 0.0f)
                {
                    sprite = lightningStormWeatherSprite;
                }
                else if (weatherManager._outputRainScalar > 0.0f)
                {
                    sprite = rainyWeatherSprite;
                }
            }

            if (weatherImage.sprite != sprite)
            {
                weatherImage.sprite = sprite;
            }
        }
    }
}
