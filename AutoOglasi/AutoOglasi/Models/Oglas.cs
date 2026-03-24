namespace AutoOglasi.Models
{
    public class Oglas
    {
        public int Id { get; set; }
        public string? Naslov { get; set; }
        public string? Opis { get; set; }
        public decimal Cena { get; set; }
        public int Godiste { get; set; }
        public int Kilometraza { get; set; }
        public string? Gorivo { get; set; }
        public string? Menjac { get; set; }
        public int KorisnikId { get; set; }
        public int ModelId { get; set; }
        public int KategorijaId { get; set; }
        public DateTime DatumObjave { get; set; }
        public bool Aktivan { get; set; }
        public Korisnik? Korisnik { get; set; }
        public Model? Model { get; set; }
        public Kategorija? Kategorija { get; set; }
        public ICollection<Slika>? Slike { get; set; }
    }
}