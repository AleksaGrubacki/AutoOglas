namespace AutoOglasi.Models
{
    public class Marka
    {
        public int Id { get; set; }
        public string Naziv { get; set; }

        public ICollection<Model> Modeli { get; set; }
    }
}