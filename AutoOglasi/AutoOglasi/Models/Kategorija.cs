namespace AutoOglasi.Models
{
    public class Kategorija
    {
        public int Id { get; set; }
        public string? Naziv { get; set; }
        public ICollection<Oglas>? Oglasi { get; set; }
    }
}