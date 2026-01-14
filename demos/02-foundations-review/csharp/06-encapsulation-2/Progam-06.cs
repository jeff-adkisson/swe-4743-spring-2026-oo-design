// =============================
// Demo.Main
// =============================
namespace Demo.Main
{
    public class Program
    {
        public static void Main()
        {
            var collection = InitializeCollection();

            PrintCollection(collection);

            PrintReadingPlan(collection);

            PrintShortReads(collection, maxMinutes: 240);

            PrintArtifactsByCultureMaterialName(collection);
        }

        // -----------------------------
        // Initialization
        // -----------------------------
        private static List<Demo.Entities.ICollectionItem> InitializeCollection()
        {
            // Publications (readable/listenable)
            Demo.Entities.ICollectionItem collectionItem1 =
                new Demo.Entities.PaperbackBook("A Philosophy of Software Design", 183);

            // cast to IPublication to access AddPercentageRead method
            var publication1 = (Demo.Entities.IPublication)collectionItem1;
            publication1.AddPercentageRead(30.0 / 183.0); // ~16.39%

            Demo.Entities.ICollectionItem collectionItem2 =
                new Demo.Entities.Scroll("War Scroll / Dead Sea Collection", 8148);

            var publication2 = (Demo.Entities.IPublication)collectionItem2;
            publication2.AddPercentageRead(0.10); // read 10%

            Demo.Entities.ICollectionItem collectionItem3 =
                new Demo.Entities.AudioBook("The Pragmatic Programmer", 540);

            var publication3 = (Demo.Entities.IPublication)collectionItem3;
            publication3.AddPercentageRead(0.25); // listened to 25%

            // Artifacts (non-reading items)
            Demo.Entities.ICollectionItem collectionItem4 =
                new Demo.Entities.Jewelry("Labubu", culture: "Hong Kong", material: "Gold");

            Demo.Entities.ICollectionItem collectionItem5 =
                new Demo.Entities.Container("Canopic Jar (Imsety)", culture: "Egypt", material: "Limestone");

            Demo.Entities.ICollectionItem collectionItem6 =
                new Demo.Entities.Sculpture("Ushabti Figurine", culture: "Egypt", material: "Granite");

            return new List<Demo.Entities.ICollectionItem>
            {
                collectionItem1,
                collectionItem2,
                collectionItem3,
                collectionItem4,
                collectionItem5,
                collectionItem6
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
            Console.WriteLine($"- Total estimated time remaining: {totalMinutes} minutes");

            if (shortest != null)
                Console.WriteLine($"- Shortest remaining: {shortest.Title} ({shortest.EstimateReadingMinutes()} min)");

            if (longest != null)
                Console.WriteLine($"- Longest remaining:  {longest.Title} ({longest.EstimateReadingMinutes()} min)");

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

            Console.WriteLine($"Short reads (<= {maxMinutes} minutes remaining):");

            foreach (var publication in publications
                         .Where(p => p.EstimateReadingMinutes() <= maxMinutes))
            {
                Console.WriteLine($"* {publication.Title} ({publication.EstimateReadingMinutes()} min remaining)");
            }

            Console.WriteLine();
        }

        // -----------------------------
        // List artifacts by Culture, then Material, then Name (Title)
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
                if (!string.Equals(currentCulture, a.Culture, StringComparison.Ordinal))
                {
                    currentCulture = a.Culture;
                    currentMaterial = null;
                    Console.WriteLine($"* {currentCulture}");
                }

                if (!string.Equals(currentMaterial, a.Material, StringComparison.Ordinal))
                {
                    currentMaterial = a.Material;
                    Console.WriteLine($"  + {currentMaterial}");
                }

                Console.WriteLine($"   - {a.Title}");
            }

            Console.WriteLine();
        }
    }
}

// =============================
// Demo.Entities
// =============================
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
        // Remaining time, taking progress into account
        int EstimateReadingMinutes();

        // Add progress as a fraction between 0 and 1 (inclusive)
        void AddPercentageRead(double percentageRead);
    }

    // NEW: abstract base class under IPublication
    // Encapsulates common progress state + shared AddPercentageRead behavior.
    public abstract class PublicationBase : IPublication
    {
        // requested protected fields (for teaching encapsulation / inheritance)
        protected double PercentageRead; // 0..1

        protected PublicationBase(string title)
        {
            Title = title;
            PercentageRead = 0;
        }

        public string Title { get; }

        // requested protected PagesRemaining (computed in each concrete type)
        protected abstract int PagesRemaining { get; }

        public void AddPercentageRead(double percentageRead)
        {
            if (percentageRead < 0 || percentageRead > 1)
                throw new ArgumentOutOfRangeException(nameof(percentageRead), "Must be between 0 and 1.");

            PercentageRead = Math.Min(1.0, PercentageRead + percentageRead);
        }

        // Publications still must provide their own estimate + details formatting
        public abstract int EstimateReadingMinutes();

        public abstract string GetDetails();
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
    public class PaperbackBook : PublicationBase
    {
        private const int PagesPerHour = 50;

        private readonly int _totalPages;

        public PaperbackBook(string title, int totalPages)
            : base(title)
        {
            _totalPages = totalPages;
        }

        protected override int PagesRemaining
        {
            get
            {
                double remaining = _totalPages * (1.0 - PercentageRead);
                return (int)Math.Ceiling(remaining);
            }
        }

        public override string GetDetails()
        {
            int percent = (int)Math.Round(PercentageRead * 100);
            return $"{Title} | Paperback Book | {_totalPages} total pages | {percent}% read | {PagesRemaining} pages remaining | ~{EstimateReadingMinutes()} min remaining";
        }

        public override int EstimateReadingMinutes()
        {
            double hours = (double)PagesRemaining / PagesPerHour;
            return (int)Math.Ceiling(hours * 60);
        }

        // Optional: keep this for teaching, but it's no longer needed by Main
        public void AddPagesRead(int pages)
        {
            if (pages <= 0) throw new ArgumentOutOfRangeException(nameof(pages));

            double fraction = (double)pages / _totalPages;
            AddPercentageRead(fraction);
        }
    }

    public class Scroll : PublicationBase
    {
        private const int MillimetersPerMinute = 25;

        private readonly int _lengthMillimeters;

        public Scroll(string title, int lengthMillimeters)
            : base(title)
        {
            _lengthMillimeters = lengthMillimeters;
        }

        protected override int PagesRemaining
        {
            get
            {
                // "PagesRemaining" is a teaching name here; for a scroll, treat it as remaining millimeters.
                double remaining = _lengthMillimeters * (1.0 - PercentageRead);
                return (int)Math.Ceiling(remaining);
            }
        }

        public override string GetDetails()
        {
            int percent = (int)Math.Round(PercentageRead * 100);
            return $"{Title} | Scroll | {_lengthMillimeters} mm total | {percent}% read | {PagesRemaining} mm remaining | ~{EstimateReadingMinutes()} min remaining";
        }

        public override int EstimateReadingMinutes()
        {
            double minutes = (double)PagesRemaining / MillimetersPerMinute;
            return (int)Math.Ceiling(minutes);
        }
    }

    public class AudioBook : PublicationBase
    {
        private readonly int _durationMinutes;

        public AudioBook(string title, int durationMinutes)
            : base(title)
        {
            _durationMinutes = durationMinutes;
        }

        protected override int PagesRemaining
        {
            get
            {
                // "PagesRemaining" is a teaching name here; for audio, treat it as remaining minutes.
                double remaining = _durationMinutes * (1.0 - PercentageRead);
                return (int)Math.Ceiling(remaining);
            }
        }

        public override string GetDetails()
        {
            int percent = (int)Math.Round(PercentageRead * 100);
            return $"{Title} | Audiobook | {_durationMinutes} min total | {percent}% listened | ~{EstimateReadingMinutes()} min remaining";
        }

        public override int EstimateReadingMinutes()
        {
            // For audiobooks, remaining minutes is the estimate.
            return PagesRemaining;
        }
    }

    // -----------------------------
    // Artifacts
    // -----------------------------
    public class Jewelry : IArtifact
    {
        public Jewelry(string title, string culture, string material)
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