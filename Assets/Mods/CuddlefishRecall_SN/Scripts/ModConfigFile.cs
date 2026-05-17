using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace DaftAppleGames.CuddlefishRecall_SN
{    /// <summary>
    /// Nautilus mod config class
    /// </summary>
    [Menu("Cuddle Fish Recall")]
    public class ModConfigFile : ConfigFile
    {
        [Choice("Recall Method", "Teleport", "Swim")]
        public RecallMoveMethod RecallMoveMethod = RecallMoveMethod.SwimTo;

        /// <summary>
        /// Health regen
        /// </summary>
        [Slider("Health Regen Multiplier", Step = 0.1f, Format = "{0:F2}", Min = 0.0f, Max = 1.0f, DefaultValue = 0.01f)]
        public float HealthRegenModifier = 0.01f;

        [Slider("Recall Swim Velocity", Step = 0.1f, Format = "{0:F2}", Min = 1.0f, Max = 10.0f, DefaultValue = 3.0f)]
        public float RecallSwimVelocity = 3.0f;
    }
}