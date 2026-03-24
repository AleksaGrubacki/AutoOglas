namespace AutoOglasi.Models
{
    public class Korisnik
    {
        public int Id { get; set; }
        public string? Ime { get; set; }
        public string? Prezime { get; set; }
        public string? Email { get; set; }
        public string? LozinkaHash { get; set; }
        public string Uloga { get; set; } = "User";
        public DateTime DatumRegistracije { get; set; }
        public ICollection<Oglas>? Oglasi { get; set; }
    }
}