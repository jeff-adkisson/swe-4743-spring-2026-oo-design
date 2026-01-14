namespace Demo.Main
{
    public class Program
    {
        public static void Main()
        {
            Entities.Publication item1 = new Entities.PaperbackBook("A Philosophy of Software Design", 183);
            Entities.Publication item2 = new Entities.Scroll("War Scroll / Dead Sea Collection", 8148);

            List<Entities.Publication> myCollection = new List<Entities.Publication>() { item1, item2 };

            Console.WriteLine("My Publication Collection [C#]:");
            foreach (var item in myCollection)
            {
                Console.WriteLine($"* {item.GetDetails()}");
            }
        }
    }
}
namespace Demo.Entities
{
    public abstract class Publication
    {
        public string Title { get; }

        public Publication(string title)
        {
            Title = title;
        }

        public abstract string GetDetails();
    }

    public class PaperbackBook : Publication
    {

        public PaperbackBook(string title, int pages) : base(title)
        {
            Pages = pages;
        }

        public int Pages { get; }

        public override string GetDetails()
        {
            return $"{Title} | Paperback Book | {Pages} pages";
        }
    }

    public class Scroll : Publication
    {
        public Scroll(string title, int lengthMillimeters) : base(title)
        {
            LengthMillimeters = lengthMillimeters;
        }

        public int LengthMillimeters { get; }

        public override string GetDetails()
        {
            return $"{Title} | Scroll | {LengthMillimeters} mm";
        }
    }
}