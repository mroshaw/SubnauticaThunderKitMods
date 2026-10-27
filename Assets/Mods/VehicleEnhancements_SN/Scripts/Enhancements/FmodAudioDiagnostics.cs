using System.Reflection;
using FMOD;
using FMODUnity;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DaftAppleGames.VehicleEnhancements_SN.VehicleEnhancementsPlugin_SN;

namespace DaftAppleGames.VehicleEnhancements_SN.Enhancements
{
    internal static class FmodAudioDiagnostics
    {
        private const string ListenerWarning = "[FMOD] Please add an 'FMOD Studio Listener' component to your a camera in the scene for correct 3D positioning of sounds.";

        // Reading backing fields avoids creating RuntimeManager through its Instance/CoreSystem getters.
        private static readonly FieldInfo instanceField = AccessTools.Field(typeof(RuntimeManager), "instance");
        private static readonly FieldInfo listenerCountField = AccessTools.Field(typeof(RuntimeManager), "numListeners");
        private static readonly FieldInfo coreSystemField = AccessTools.Field(typeof(RuntimeManager), "coreSystem");
        private static readonly FieldInfo studioSystemField = AccessTools.Field(typeof(RuntimeManager), "studioSystem");
        private static bool observing;

        internal static void Initialize(Harmony harmony)
        {
            if (!ConfigFile.DetailedLogging)
            {
                return;
            }

            MethodInfo initialize = AccessTools.Method(typeof(RuntimeManager), "Initialize");
            harmony.Patch(initialize,
                prefix: new HarmonyMethod(typeof(FmodAudioDiagnostics), nameof(BeforeInitialize)),
                postfix: new HarmonyMethod(typeof(FmodAudioDiagnostics), nameof(AfterInitialize)));
            Application.logMessageReceived += OnLogMessage;
            SceneManager.sceneLoaded += OnSceneLoaded;
            observing = true;
            LogState("Plugin.Awake: audio diagnostics installed");
        }

        internal static void Stop()
        {
            observing = false;
            Application.logMessageReceived -= OnLogMessage;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        internal static void LogState(string context)
        {
            if (!observing || !ConfigFile.DetailedLogging)
            {
                return;
            }

            RuntimeManager manager = (RuntimeManager)instanceField.GetValue(null);
            int registeredListeners = (int)listenerCountField.GetValue(null);
            StudioListener[] studioListeners = Resources.FindObjectsOfTypeAll<StudioListener>();
            AudioListener[] unityListeners = Resources.FindObjectsOfTypeAll<AudioListener>();
            int activeStudioListeners = 0;
            int activeUnityListeners = 0;
            foreach (StudioListener listener in studioListeners)
            {
                if (listener && listener.isActiveAndEnabled)
                {
                    activeStudioListeners++;
                }
            }

            foreach (AudioListener listener in unityListeners)
            {
                if (listener && listener.isActiveAndEnabled)
                {
                    activeUnityListeners++;
                }
            }

            ModDebugLog.LogDebug($"[FMOD diagnostics] {context}; frame={Time.frameCount}; scene='{SceneManager.GetActiveScene().name}'; runtimeExists={(bool)manager}; registeredStudioListeners={registeredListeners}; StudioListener components={studioListeners.Length} (active={activeStudioListeners}); Unity AudioListener components={unityListeners.Length} (active={activeUnityListeners}).");
            if (!manager)
            {
                return;
            }

            FMOD.System core = (FMOD.System)coreSystemField.GetValue(manager);
            FMOD.Studio.System studio = (FMOD.Studio.System)studioSystemField.GetValue(manager);
            if (core.hasHandle())
            {
                RESULT result = core.get3DNumListeners(out int listeners);
                ModDebugLog.LogDebug($"[FMOD diagnostics] Existing core system=0x{core.handle.ToInt64():X}; get3DNumListeners={result}; listeners={listeners}.");
            }

            if (studio.isValid())
            {
                RESULT result = studio.getNumListeners(out int listeners);
                ModDebugLog.LogDebug($"[FMOD diagnostics] Existing studio system=0x{studio.handle.ToInt64():X}; getNumListeners={result}; listeners={listeners}.");
            }
        }

        private static void BeforeInitialize()
        {
            if (!observing || !ConfigFile.DetailedLogging)
            {
                return;
            }

            LogState("RuntimeManager.Initialize entered");
            ModDebugLog.LogDebug($"[FMOD diagnostics] RuntimeManager initialization caller:\n{new System.Diagnostics.StackTrace(1, true)}");
        }

        private static void AfterInitialize(RESULT __result)
        {
            if (!observing || !ConfigFile.DetailedLogging)
            {
                return;
            }

            LogState($"RuntimeManager.Initialize returned {__result}");
        }

        private static void OnLogMessage(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Warning || condition != ListenerWarning || !ConfigFile.DetailedLogging)
            {
                return;
            }

            LogState("FMOD emitted its missing Studio Listener warning");
            ModDebugLog.LogDebug($"[FMOD diagnostics] Listener warning stack:\n{stackTrace}");
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (ConfigFile.DetailedLogging)
            {
                LogState($"Scene loaded '{scene.name}' ({mode})");
            }
        }
    }
}
