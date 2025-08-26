using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wc3_Combat_Game.Util.UnitConversion
{
    enum Temperature
    {
        Celcius,
        Fahrenheit,
        Kelvin
    }

    class TemperatureConverter: UnitConverter<Temperature, float>
    {
        static TemperatureConverter()
        {
            BaseUnit = Temperature.Celcius;

            // Correct Fahrenheit <-> Celsius conversions
            RegisterConversion(Temperature.Fahrenheit,
                c => (c * 9f / 5f) + 32f,
                f => (f - 32f) * 5f / 9f);

            // Correct Kelvin <-> Celsius conversions
            RegisterConversion(Temperature.Kelvin,
                c => c + 273.15f,
                k => k - 273.15f);
        }
    }

    /// <summary>
    /// Represents a temperature in Celsius.
    /// </summary>
    public readonly struct Celsius
    {
        public float Value { get; }

        public Celsius(float value)
        {
            Value = value;
        }

        // Explicit conversion from Celsius to Fahrenheit
        public static explicit operator Fahrenheit(Celsius c)
        {
            var converter = new TemperatureConverter();
            var fahrenheitValue = converter.Convert(c.Value, Temperature.Celcius, Temperature.Fahrenheit);
            return new Fahrenheit(fahrenheitValue);
        }

        public override string ToString() => $"{Value} C";
    }

    /// <summary>
    /// Represents a temperature in Fahrenheit.
    /// </summary>
    public readonly struct Fahrenheit
    {
        public float Value { get; }

        public Fahrenheit(float value)
        {
            Value = value;
        }

        // Explicit conversion from Fahrenheit to Celsius
        public static explicit operator Celsius(Fahrenheit f)
        {
            var converter = new TemperatureConverter();
            var celsiusValue = converter.Convert(f.Value, Temperature.Fahrenheit, Temperature.Celcius);
            return new Celsius(celsiusValue);
        }

        public override string ToString() => $"{Value} F";
    }
}