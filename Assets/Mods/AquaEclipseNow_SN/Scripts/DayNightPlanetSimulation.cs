using System;
using UnityEngine;
using static DaftAppleGames.AquaEclipseNowPlugin.AquaEclipseNowPlugin;

namespace DaftAppleGames.AquaEclipseNowPlugin
{
    /// <summary>
    /// Provides an instance of a simulation of the games sun and planet from uSkyManager
    /// based on the simulation time, equivalent to DayNightCycle
    /// </summary>
    internal static class DayNightPlanetSimulation
    {
        // Very basic DayNightCycle properties
        private static double _simulationTimePassed;
        private static float _timeLine;

        // Simulated Planet and Sun position/rotations
        private static Vector3 _planetPosition;
        private static Vector3 _sunPosition;
        private static Quaternion _sunEuler;

        // Sun settings - derived from the game uSkyManager
        private static float _planetOrbitSpeed;
        private static float _planetZenith;
        private static float _planetDistance;

        // Sun settings - derived from the game uSkyManager
        private static float _sunMaxAngle;
        private static float _sunDirection;
        private static float _northPoleOffset;
        private static float _sunSetTime;
        private static float _sunRiseTime;

        /// <summary>
        /// Internal getter for TimePassed
        /// </summary>
        internal static double TimePassed => _simulationTimePassed;

        /// <summary>
        /// Can be called to Reset the simulation, pulling new values in from the game classes
        /// </summary>
        internal static void ResetSimulation()
        {
            // Configure the simulated planet and sun
            _planetPosition = uSkyManager.main.PlanetPos();
            _sunEuler = uSkyManager.main.sunEuler;

            // Get the current DayNightCycle and uSkyManager properties
            _simulationTimePassed = DayNightCycle.main.timePassedAsDouble;
            _planetOrbitSpeed = uSkyManager.main.planetOrbitSpeed;
            _planetZenith = uSkyManager.main.planetZenith;
            _planetDistance = uSkyManager.main.planetDistance;
            _sunMaxAngle = uSkyManager.main.sunMaxAngle;
            _sunDirection = uSkyManager.main.SunDirection;
            _northPoleOffset = uSkyManager.main.NorthPoleOffset;
            _sunSetTime = DayNightCycle.main.sunSetTime;
            _sunRiseTime = DayNightCycle.main.sunRiseTime;
        }

        /// <summary>
        /// Run the simulation
        /// </summary>
        internal static Double RunSimulation(int numIterations, double numSecondIncrements, float alignmentThreshold)
        {
            ResetSimulation();

            float maxAlignment = 0;
            double maxAlignmentTime = 0;

            for (int iteration = 0; iteration < numIterations; iteration++)
            {
                AddTime(numSecondIncrements);
                float alignment = GetSunPlanetAlignment();

                if (alignment > maxAlignment)
                {
                    maxAlignment = alignment;
                    maxAlignmentTime = _simulationTimePassed;
                }

                if (alignmentThreshold > 0 && alignment > alignmentThreshold)
                {
                    ModDebugLog.LogDebug($"Exceeded alignment threshold. Using this for eclipse! " +
                                         $"Iteration: {iteration}, Time Passed: {_simulationTimePassed}, Planet Pos: {_planetPosition}, Sun Euler: {_sunEuler}, Alignment: {alignment}, Threshold: {alignmentThreshold}");
                    return _simulationTimePassed;
                }

                ModDebugLog.LogDebug(
                    $"Time Passed: {_simulationTimePassed}, Planet Pos: {_planetPosition}, Sun Euler: {_sunEuler}, Alignment: {alignment}");
            }

            ModDebugLog.LogDebug(
                $"Simulation ran for: {numIterations} iterations in {numSecondIncrements} increments. Max Alignment: {maxAlignment} at {maxAlignmentTime}.");

            return maxAlignmentTime;
        }

        /// <summary>
        /// Simulate adding a time in seconds
        /// </summary>
        internal static void AddTime(double timePassed)
        {
            _simulationTimePassed += timePassed;
            UpdatePlanet();
            UpdateSun();
        }

        /// <summary>
        /// Simulate setting the actual time passed
        /// </summary>
        internal static void SetTime(double newTime)
        {
            _simulationTimePassed = newTime;
            UpdatePlanet();
            UpdateSun();
        }

        /// <summary>
        /// Updates the simulated planet position
        /// </summary>
        private static void UpdatePlanet()
        {
            // Equivalent of the game's PlanetPos() calculation
            double orbitAngle = _planetOrbitSpeed * (_simulationTimePassed / 1200.0);

            // Convert angles to radians
            double zenithRad = _planetZenith * Mathf.Deg2Rad;
            double orbitRad = orbitAngle * 0.01745329238474369;

            // Compute sine/cosine for planet orbit
            double sinZenith = Math.Sin(zenithRad);
            double cosZenith = Math.Cos(zenithRad);
            double sinOrbit = Math.Sin(orbitRad);
            double cosOrbit = Math.Cos(orbitRad);

            // Set planet position
            Vector3 newPosition;
            newPosition.x = (float)(sinZenith * cosOrbit);
            newPosition.y = (float)cosZenith;
            newPosition.z = (float)(sinZenith * sinOrbit);

            // Scale by planet distance
            newPosition *= _planetDistance;

            // Apply to simulated planet
            _planetPosition = newPosition;
        }


        /// <summary>
        /// Updates the simulated Sun rotation
        /// </summary>
        private static void UpdateSun()
        {
            // Get the time of day (0–1) like the game's GetDayNightCycleTime
            float dayTime = GetDayNightCycleTime();

            // Convert to 24-hour timeline
            _timeLine = dayTime * 24f;

            // Normalize timeline just in case
            if (_timeLine >= 24f) _timeLine -= 24f;
            if (_timeLine < 0f) _timeLine += 24f;

            // Compute vertical angle of the sun
            float verticalAngle = 0f;
            float normalizedTime = _timeLine / 24f;

            if (_timeLine < 6f)
            {
                float t = Mathf.Clamp(normalizedTime * 4f, 0f, 1f);
                verticalAngle = Mathf.Lerp(0f, -_sunMaxAngle, t);
            }
            else if (_timeLine > 18f)
            {
                float t = Mathf.Clamp((normalizedTime - 0.75f) * 4f, 0f, 1f);
                verticalAngle = Mathf.Lerp(_sunMaxAngle, 0f, t);
            }
            else
            {
                float t = Mathf.Clamp(normalizedTime * 2f - 0.5f, 0f, 1f);
                verticalAngle = Mathf.Lerp(-_sunMaxAngle, _sunMaxAngle, t);
            }

            // Apply rotation exactly like the game
            /*
            _sunEuler = Quaternion.Euler(0f, _sunDirection, _northPoleOffset) *
                        Quaternion.Euler(verticalAngle + 90f, 0f, 0f);
            */
            
            _sunEuler = Quaternion.Euler(0f, _sunDirection, _northPoleOffset) *
                        Quaternion.Euler(_timeLine * 360f / 24f - 90f, 0f, 0f);
        }

        /// <summary>
        /// Helper to determine time of day
        /// </summary>
        private static float GetDayNightCycleTime()
        {
            {
                float dayScalar =
                    Mathf.Repeat((float)(UWE.Utils.Repeat(_simulationTimePassed, 1200.0) / 1200.0), 1f);

                float timeBetweenSunSetAndRise = _sunSetTime - _sunRiseTime;
                if (dayScalar > _sunRiseTime && dayScalar < _sunSetTime)
                {
                    return (dayScalar - _sunRiseTime) / timeBetweenSunSetAndRise * 0.5f + 0.25f;
                }

                float num3 = 1f - timeBetweenSunSetAndRise;
                if (dayScalar < _sunSetTime)
                {
                    dayScalar += 1f;
                }

                float num4 = (dayScalar - _sunSetTime) / num3 * 0.5f + 0.75f;
                if (num4 > 1f)
                {
                    num4 -= 1f;
                }

                return num4;
            }
        }

        /// <summary>
        /// Calculate the Eclipse dot value based on the simulated planet and sun
        /// </summary>
        private static float GetSunPlanetAlignment()
        {
            // Sun direction points towards the planet
            Vector3 sunDir = _sunEuler * Vector3.forward * -1f;
            Vector3 planetDir = _planetPosition.normalized;
            return Mathf.Max(Vector3.Dot(planetDir, sunDir), 0f);
        }
    }
}