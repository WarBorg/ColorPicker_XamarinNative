using System;
namespace ColorPickerTest.Utils
{
    public static class NumericExtensions
    {
        /// <summary>
        /// Convert to Radians.
        /// </summary>
        /// <param name="degrees">The value to convert to radians</param>
        /// <returns>The value in radians</returns>
        public static float ToRadians(this float degrees) => degrees / 180 * (float)Math.PI;

        /// <summary>
        /// Convert to Degrees.
        /// </summary>
        /// <param name="radians">The value to convert to degrees</param>
        /// <returns>The value in degrees</returns>
        public static float ToDegrees(this float radians) => radians * 180 / (float)Math.PI;
    }
}

