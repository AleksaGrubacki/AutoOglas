namespace AutoOglasi.DAL.Entities;

public class Slika
{
    public int Id { get; set; }
    public int OglasId { get; set; }
    public string? PutanjaFajla { get; set; }
    public bool JeNaslovna { get; set; }
    public Oglas? Oglas { get; set; }
}
