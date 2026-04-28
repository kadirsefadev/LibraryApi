namespace LibraryApi.DTOs
{
    //Clienta cevap dönerken kullanırız
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string Category { get; set; }
        public string AuthorName { get; set; }
    }
    //Clienta yeni kitap olustururken gönderir 
    public class BookCreateDto
        {
        public int AuthorId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Category { get; set; } = string.Empty;
    }
    public class BookUpdateDto
    {
        public int AuthorId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Category { get; set; } = string.Empty;

    }
}
