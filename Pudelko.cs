using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum UnitOfMeasure
{
    Milimeter,
    Centimeter,
    Meter
}

public sealed class Pudelko : IFormattable, IEquatable<Pudelko>, IEnumerable<double>
{
    private readonly double length;
    private readonly double width;
    private readonly double height;
    private const double MaxDimensionMeters = 10;
    private const double MaxDimensionCentimeters = MaxDimensionMeters * 100;

    public Pudelko(double a = 10, double b = 10, double c = 10, UnitOfMeasure unit = UnitOfMeasure.Meter)
    {
        double lengthInMeters = ConvertToMeters(a, unit);
        double widthInMeters = ConvertToMeters(b, unit);
        double heightInMeters = ConvertToMeters(c, unit);

        if (lengthInMeters <= 0 || widthInMeters <= 0 || heightInMeters <= 0)
        {
            throw new ArgumentOutOfRangeException("Dimensions must be positive.");
        }

        if (lengthInMeters > MaxDimensionMeters || widthInMeters > MaxDimensionMeters || heightInMeters > MaxDimensionMeters)
        {
            throw new ArgumentOutOfRangeException("Dimensions cannot exceed 10 meters.");
        }

        this.length = lengthInMeters;
        this.width = widthInMeters;
        this.height = heightInMeters;
    }

    public double A => length;
    public double B => width;
    public double C => height;

    public double Objetosc => Math.Round(length * width * height, 9);

    public double Pole => Math.Round(2 * (length * width + length * height + width * height), 6);

    public override string ToString()
    {
        return ToString("m", null);
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        switch (format)
        {
            case "m":
                return String.Format("{0:F3} m × {1:F3} m × {2:F3} m", length, width, height);
            case "cm":
                return String.Format("{0:F1} cm × {1:F1} cm × {2:F1} cm", length * 100, width * 100, height * 100);
            case "mm":
                return String.Format("{0:F0} mm × {1:F0} mm × {2:F0} mm", length * 1000, width * 1000, height * 1000);
            default:
                throw new FormatException("Invalid format. Use 'm', 'cm', or 'mm'.");
        }
    }

    public bool Equals(Pudelko other)
    {
        if (other == null) return false;
        return length == other.length && width == other.width && height == other.height;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        return Equals((Pudelko)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + length.GetHashCode();
            hash = hash * 23 + width.GetHashCode();
            hash = hash * 23 + height.GetHashCode();
            return hash;
        }
    }

    public static bool operator ==(Pudelko pudelko1, Pudelko pudelko2)
    {
        if (ReferenceEquals(pudelko1, pudelko2))
            return true;
        if (ReferenceEquals(pudelko1, null))
            return false;
        return pudelko1.Equals(pudelko2);
    }

    public static bool operator !=(Pudelko pudelko1, Pudelko pudelko2)
    {
        return !(pudelko1 == pudelko2);
    }

    public static Pudelko operator +(Pudelko pudelko1, Pudelko pudelko2)
    {
        double newLength = Math.Max(pudelko1.length, pudelko2.length);
        double newWidth = Math.Max(pudelko1.width, pudelko2.width);
        double newHeight = Math.Max(pudelko1.height, pudelko2.height);
        return new Pudelko(newLength, newWidth, newHeight);
    }

    public static explicit operator double[](Pudelko pudelko)
    {
        return new double[] { pudelko.length, pudelko.width, pudelko.height };
    }

    public static implicit operator Pudelko((int a, int b, int c) dimensions)
    {
        return new Pudelko(dimensions.a / 1000.0, dimensions.b / 1000.0, dimensions.c / 1000.0, UnitOfMeasure.Milimeter);
    }

    public double this[int index]
    {
        get
        {
            switch (index)
            {
                case 0:
                    return length;
                case 1:
                    return width;
                case 2:
                    return height;
                default:
                    throw new IndexOutOfRangeException("Index must be in range 0 to 2.");
            }
        }
    }

    public IEnumerator<double> GetEnumerator()
    {
        yield return length;
        yield return width;
        yield return height;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static Pudelko Parse(string dimensionsString)
    {
        var dimensionsParts = dimensionsString.Split(new[] { '×' }, StringSplitOptions.RemoveEmptyEntries);
        if (dimensionsParts.Length != 3)
            throw new FormatException("Invalid format. Use format 'A m × B m × C m'.");

        var dimensions = dimensionsParts.Select(part =>
        {
            var valueUnitPair = part.Trim().Split();
            if (valueUnitPair.Length != 2)
                throw new FormatException("Invalid format. Use format 'A m × B m × C m'.");

            double value = double.Parse(valueUnitPair[0]);
            string unitString = valueUnitPair[1].ToLower();

            switch (unitString)
            {
                case "m":
                    return value;
                case "cm":
                    return value / 100;
                case "mm":
                    return value / 1000;
                default:
                    throw new FormatException("Invalid unit. Use 'm', 'cm', or 'mm'.");
            }
        }).ToArray();

        return new Pudelko(dimensions[0], dimensions[1], dimensions[2]);
    }

    private double ConvertToMeters(double value, UnitOfMeasure unit)
    {
        switch (unit)
        {
            case UnitOfMeasure.Milimeter:
                return value / 1000;
            case UnitOfMeasure.Centimeter:
                return value / 100;
            case UnitOfMeasure.Meter:
            default:
                return value;
        }
    }
}

public static class PudelkoExtensions
{
    public static Pudelko Kompresuj(this Pudelko original)
    {
        double volume = original.Objetosc;
        double sideLength = Math.Pow(volume, 1.0 / 3);
        return new Pudelko(sideLength, sideLength, sideLength);
    }
}

class Program
{
    static void Main(string[] args)
    {
        List<Pudelko> pudelka = new List<Pudelko>()
        {
            new Pudelko(2.5, 3, 4),
            new Pudelko(1, 5, 2),
            new Pudelko(3, 3, 3)
        };

        Console.WriteLine("Pudelka przed sortowaniem:");
        foreach (var p in pudelka)
        {
            Console.WriteLine(p);
        }

        pudelka.Sort((p1, p2) =>
        {
            if (p1.Objetosc != p2.Objetosc)
                return p1.Objetosc.CompareTo(p2.Objetosc);
            if (p1.Pole != p2.Pole)
                return p1.Pole.CompareTo(p2.Pole);
            return (p1.A + p1.B + p1.C).CompareTo(p2.A + p2.B + p2.C);
        });

        Console.WriteLine("\nPudelka po sortowaniu:");
        foreach (var p in pudelka)
        {
            Console.WriteLine(p);
        }
    }
}
