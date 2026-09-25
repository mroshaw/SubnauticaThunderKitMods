using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DaftAppleGames.VehicleEnhancements_SN.VehicleEnhancementsPlugin_SN;

namespace DaftAppleGames.VehicleEnhancements_SN.TimeAndWeather
{
    internal class TimeAndWeatherController : MonoBehaviour
    {
        [SerializeField, Required] private GameObject timeAndWeatherRoot;
        [SerializeField, Required] private TextMeshProUGUI timeText;
        [SerializeField, Required] private RectTransform timeOfDayIndicator;
        [SerializeField, Required] private Image timeImage;
        [SerializeField, Required] private Sprite dayTimeSprite;
        [SerializeField, Required] private Sprite nightTimeSprite;
        [SerializeField, Required] private GameObject weatherIndicator;
        [SerializeField, Required] private Image weatherImage;
        [SerializeField, Required] private Sprite sunnyWeatherSprite;
        [SerializeField, Required] private Sprite rainyWeatherSprite;
        [SerializeField, Required] private Sprite lightningStormWeatherSprite;

        private RectTransform previousMoon;
        private RectTransform nextMoon;
        private int lastDisplayedMinute = -1;
        private float nextWeatherUpdateTime;
        private WeatherManager weatherManager;
        private EnhancedVehicle vehicle = EnhancedVehicle.Seamoth;

        internal void Configure(EnhancedVehicle selectedVehicle)
        {
            vehicle = selectedVehicle;
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
                if (weatherManager._outputLightningScalar >= 0.25f)
                {
                    sprite = lightningStormWeatherSprite;
                }
                else if (weatherManager._outputRainScalar >= 0.1f)
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
