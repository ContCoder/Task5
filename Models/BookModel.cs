namespace Task5Web.Models
{
    public class BookModel
    {
        public int Index { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public string Publisher { get; set; }
        public int Likes { get; set; }
        public List<ReviewModel> Reviews { get; set; }
    }
    public class ReviewModel
    {
        public string ReviewText { get; set; }
        public string ReviewerName { get; set; }
    }
}
