using System;
using System.Collections.Generic;
using System.Linq;

namespace Demo.Main
{
    public class Program
    {
        public static void Main()
        {
            var collection = InitializePublications();

            PrintCollection(collection);

            PrintReadingPlan(collection);

            PrintShortReads(collection, maxMinutes: 240);
        }

        // -----------------------------
        // Initialization
        // -----------------------------
        private static List<Demo.Entities.IPublication> InitializePublications()
        {
            Demo.Entities.IPublication item1 =
                new Demo.Entities.PaperbackBook("A Philosophy of Software Design", 183);

            Demo.Entities.IPublication item2 =
                new Demo.Entities.Scroll("War Scroll / Dead Sea Collection", 8148);

            Demo.Entities.IPublication item3 =
                new Demo.Entities.AudioBook("The Pragmatic Programmer", 540); // 9 hours

            return new List<Demo.Entities.IPublication>
            {
                item1,
                item2,
                item3
            };
        }

        // -----------------------------
        // Polymorphic listing
        // -----------------------------
        private static void PrintCollection(IEnumerable<Demo.Entities.IPublication> publications)
        {
            Console.WriteLine("My Publication Collection [C#]:");
            foreach (var publication in publications)
            {
                // Same call site; different runtime behavior
                Console.WriteLine($"* {publication.GetDetails()}");
            }

            Console.WriteLine();
        }

        // -----------------------------
        // Polymorphism driving logic
        // -----------------------------
        private static void PrintReadingPlan(IEnumerable<Demo.Entities.IPublication> publications)
        {
            int totalMinutes = publications.Sum(p => p.EstimateReadingMinutes());
            var shortest = publications.MinBy(p => p.EstimateReadingMinutes());
            var longest = publications.MaxBy(p => p.EstimateReadingMinutes());

            Console.WriteLine("Reading plan (polymorphism demo):");
            Console.WriteLine($"- Total estimated time: {totalMinutes} minutes");

            if (shortest != null)
                Console.WriteLine($"- Shortest: {shortest.Title} ({shortest.EstimateReadingMinutes()} min)");

            if (longest != null)
                Console.WriteLine($"- Longest:  {longest.Title} ({longest.EstimateReadingMinutes()} min)");

            Console.WriteLine();
        }

        // -----------------------------
        // Filtering using abstraction
        // -----------------------------
        private static void PrintShortReads(
            IEnumerable<Demo.Entities.IPublication> publications,
            int maxMinutes)
        {
            Console.WriteLine($"Short reads (<= {maxMinutes} minutes):");

            foreach (var publication in publications
                         .Where(p => p.EstimateReadingMinutes() <= maxMinutes))
            {
                Console.WriteLine($"* {publication.Title} ({publication.EstimateReadingMinutes()} min)");
            }
        }
    }
}

namespace Demo.Entities
{
    // Interface = behavior contract (capability-based abstraction)
    public interface IPublication
    {
        string Title { get; }

        string GetDetails();

        int EstimateReadingMinutes();
    }

    public class PaperbackBook : IPublication
    {
        private const int PagesPerHour = 50;

        public PaperbackBook(string title, int pages)
        {
            Title = title;
            Pages = pages;
        }

        public string Title { get; }

        public int Pages { get; }

        public string GetDetails()
        {
            return $"{Title} | Paperback Book | {Pages} pages | ~{EstimateReadingMinutes()} min";
        }

        public int EstimateReadingMinutes()
        {
            // pages / (pages/hour) => hours; convert to minutes
            double hours = (double)Pages / PagesPerHour;
            return (int)Math.Ceiling(hours * 60);
        }
    }

    public class Scroll : IPublication
    {
        // Another simple constant for demo purposes.
        private const int MillimetersPerMinute = 25;

        public Scroll(string title, int lengthMillimeters)
        {
            Title = title;
            LengthMillimeters = lengthMillimeters;
        }

        public string Title { get; }

        public int LengthMillimeters { get; }

        public string GetDetails()
        {
            return $"{Title} | Scroll | {LengthMillimeters} mm | ~{EstimateReadingMinutes()} min";
        }

        public int EstimateReadingMinutes()
        {
            double minutes = (double)LengthMillimeters / MillimetersPerMinute;
            return (int)Math.Ceiling(minutes);
        }
    }

    public class AudioBook : IPublication
    {
        public AudioBook(string title, int durationMinutes)
        {
            Title = title;
            DurationMinutes = durationMinutes;
        }

        public string Title { get; }

        public int DurationMinutes { get; }

        public string GetDetails()
        {
            return $"{Title} | Audiobook | {DurationMinutes} minutes | ~{EstimateReadingMinutes()} min";
        }

        public int EstimateReadingMinutes()
        {
            // For an audiobook, estimated reading time is just duration.
            return DurationMinutes;
        }
    }
}