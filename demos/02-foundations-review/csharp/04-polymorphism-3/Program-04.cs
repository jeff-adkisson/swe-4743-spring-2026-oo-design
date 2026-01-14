namespace Demo.Main
{
    public class Program
    {
        public static void Main()
        {
            var collection = InitializeCollection();

            PrintCollection(collection);

            PrintReadingPlan(collection);

            PrintShortReads(collection, maxMinutes: 60);

            PrintArtifactsByCultureMaterialName(collection);
        }

        // -----------------------------
        // Initialization
        // -----------------------------
        private static List<Demo.Entities.ICollectionItem> InitializeCollection()
        {
            // Publications (readable/listenable)
            Demo.Entities.ICollectionItem publication1 =
                new Demo.Entities.PaperbackBook("A Philosophy of Software Design", 183);

            Demo.Entities.ICollectionItem publication2 =
                new Demo.Entities.Scroll("War Scroll / Dead Sea Collection", 8148);

            Demo.Entities.ICollectionItem publication3 =
                new Demo.Entities.AudioBook("The Pragmatic Programmer", 540); // 9 hours

            // Artifacts (non-reading items)
            Demo.Entities.ICollectionItem artifact1 =
                new Demo.Entities.Jewelery("Labubu", culture: "Hong Kong", material: "Gold");

            Demo.Entities.ICollectionItem artifact2 =
                new Demo.Entities.Container("Canopic Jar (Imsety)", culture: "Egypt", material: "Limestone");

            Demo.Entities.ICollectionItem artifact3 =
                new Demo.Entities.Sculpture("Ushabti Figurine", culture: "Egypt", material: "Granite");

            return new List<Demo.Entities.ICollectionItem>
            {
                publication1,
                publication2,
                publication3,
                artifact1,
                artifact2,
                artifact3
            };
        }

        // -----------------------------
        // Polymorphic listing (ICollectionItem)
        // -----------------------------
        private static void PrintCollection(IEnumerable<Demo.Entities.ICollectionItem> items)
        {
            Console.WriteLine("My Collection [C#]:");
            foreach (var item in items)
            {
                Console.WriteLine($"* {item.GetDetails()}");
            }

            Console.WriteLine();
        }

        // -----------------------------
        // Polymorphism driving logic (only publications participate)
        // -----------------------------
        private static void PrintReadingPlan(IEnumerable<Demo.Entities.ICollectionItem> items)
        {
            var publications = items.OfType<Demo.Entities.IPublication>().ToList();

            if (publications.Count == 0)
            {
                Console.WriteLine("Reading plan (polymorphism demo):");
                Console.WriteLine("- No readable publications found.");
                Console.WriteLine();
                return;
            }

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
        // Filtering using abstraction (only publications participate)
        // -----------------------------
        private static void PrintShortReads(
            IEnumerable<Demo.Entities.ICollectionItem> items,
            int maxMinutes)
        {
            var publications = items.OfType<Demo.Entities.IPublication>();

            Console.WriteLine($"Short reads (<= {maxMinutes} minutes):");

            foreach (var publication in publications
                         .Where(p => p.EstimateReadingMinutes() <= maxMinutes))
            {
                Console.WriteLine($"* {publication.Title} ({publication.EstimateReadingMinutes()} min)");
            }

            Console.WriteLine();
        }

        // -----------------------------
        // NEW: List artifacts by Culture, then Material, then Name (Title)
        // -----------------------------
        private static void PrintArtifactsByCultureMaterialName(
            IEnumerable<Demo.Entities.ICollectionItem> items)
        {
            var artifacts =
                items.OfType<Demo.Entities.IArtifact>()
                     .OrderBy(a => a.Culture)
                     .ThenBy(a => a.Material)
                     .ThenBy(a => a.Title)
                     .ToList();

            Console.WriteLine("My Artifacts (by Culture, Material, Name):");

            if (artifacts.Count == 0)
            {
                Console.WriteLine("- No artifacts found.");
                Console.WriteLine();
                return;
            }

            string? currentCulture = null;
            string? currentMaterial = null;

            foreach (var a in artifacts)
            {
                // Culture header
                if (!string.Equals(currentCulture, a.Culture, StringComparison.Ordinal))
                {
                    currentCulture = a.Culture;
                    currentMaterial = null; // reset material when culture changes
                    Console.WriteLine($"* {currentCulture}");
                }

                // Material sub-header
                if (!string.Equals(currentMaterial, a.Material, StringComparison.Ordinal))
                {
                    currentMaterial = a.Material;
                    Console.WriteLine($"  + {currentMaterial}");
                }

                // Item line
                Console.WriteLine($"   - {a.Title}");
            }

            Console.WriteLine();
        }
    }
}

namespace Demo.Entities
{
    // Base interface for anything in the collection
    public interface ICollectionItem
    {
        string Title { get; }

        string GetDetails();
    }

    // Publications are collection items that have estimated reading time
    public interface IPublication : ICollectionItem
    {
        int EstimateReadingMinutes();
    }

    // Artifacts are collection items with physical/cultural metadata
    public interface IArtifact : ICollectionItem
    {
        string Culture { get; }

        string Material { get; }
    }

    // -----------------------------
    // Publications
    // -----------------------------
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
            return DurationMinutes;
        }
    }

    // -----------------------------
    // Artifacts
    // -----------------------------
    public class Jewelery : IArtifact
    {
        public Jewelery(string title, string culture, string material)
        {
            Title = title;
            Culture = culture;
            Material = material;
        }

        public string Title { get; }

        public string Culture { get; }

        public string Material { get; }

        public string GetDetails()
        {
            return $"{Title} | Artifact | {Culture} | {Material}";
        }
    }

    public class Container : IArtifact
    {
        public Container(string title, string culture, string material)
        {
            Title = title;
            Culture = culture;
            Material = material;
        }

        public string Title { get; }

        public string Culture { get; }

        public string Material { get; }

        public string GetDetails()
        {
            return $"{Title} | Artifact | {Culture} | {Material}";
        }
    }

    public class Sculpture : IArtifact
    {
        public Sculpture(string title, string culture, string material)
        {
            Title = title;
            Culture = culture;
            Material = material;
        }

        public string Title { get; }

        public string Culture { get; }

        public string Material { get; }

        public string GetDetails()
        {
            return $"{Title} | Artifact | {Culture} | {Material}";
        }
    }
}