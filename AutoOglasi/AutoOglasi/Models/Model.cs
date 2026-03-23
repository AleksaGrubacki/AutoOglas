namespace AutoOglasi.Models
{
    public class Model
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public int MarkaId { get; set; }

        public Marka Marka { get; set; }
        public ICollection<Oglas> Oglasi { get; set; }
    }
}