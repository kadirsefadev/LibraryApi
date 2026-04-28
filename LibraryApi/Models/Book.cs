namespace LibraryApi.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string Category { get; set; }

        //Foreign Key
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
    }
}
