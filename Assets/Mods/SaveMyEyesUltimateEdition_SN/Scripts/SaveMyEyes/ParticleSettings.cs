namespace DaftAppleGames.SaveMyEyesUltimateEdition_SN.SaveMyEyes
{
    /// <summary>
    /// Current particle settings for one use case.
    /// </summary>
    internal readonly struct ParticleSettings
    {
        internal readonly float Density;
        internal readonly float Size;
        internal readonly float Speed;
        internal readonly float Brightness;

        internal ParticleSettings(float density, float size, float speed, float brightness)
        {
            Density = density;
            Size = size;
            Speed = speed;
            Brightness = brightness;
        }
    }
}
