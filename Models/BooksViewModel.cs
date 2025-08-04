namespace Task5Web.Models
{
    public class BooksViewModel
    {
        public List<BookModel> Books { get; set; } = new List<BookModel>();
        public bool HasMore { get; set; }
        public int CurrentPage { get; set; }
        public int Seed { get; set; }
        public string Language { get; set; }
        public double Likes { get; set; }
        public double Review { get; set; }

    }
}
