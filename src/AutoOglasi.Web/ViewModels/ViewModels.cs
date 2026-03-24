namespace AutoOglasi.Web.ViewModels;

public class SlikaViewModel
{
    public int Id { get; set; }
    public int OglasId { get; set; }
    public string? PutanjaFajla { get; set; }
    public bool JeNaslovna { get; set; }
}

public class MarkaViewModel
{
    public int Id { get; set; }
    public string? Naziv { get; set; }
}

public class ModelViewModel
{
    public int Id { get; set; }
    public string? Naziv { get; set; }
    public int MarkaId { get; set; }
    public MarkaViewModel? Marka { get; set; }
}

public class KategorijaViewModel
{
    public int Id { get; set; }
    public string? Naziv { get; set; }
}

public class KorisnikKratkoViewModel
{
    public int Id { get; set; }
    public string? Ime { get; set; }
    public string? Prezime { get; set; }
    public string? Email { get; set; }
}

/// <summary> Prikaz oglasa (lista, detalji, admin tabela). </summary>
public class OglasViewModel
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
    public DateTime DatumObjave { get; set; }
    public bool Aktivan { get; set; }
    public ModelViewModel? Model { get; set; }
    public KategorijaViewModel? Kategorija { get; set; }
    public KorisnikKratkoViewModel? Korisnik { get; set; }
    public ICollection<SlikaViewModel>? Slike { get; set; }
}

/// <summary> Formular Novi / Uredi (bez navigacionih objekata). </summary>
public class OglasFormViewModel
{
    public int Id { get; set; }
    public string? Naslov { get; set; }
    public string? Opis { get; set; }
    /// <summary> Nullable da prazan unos ne pravi praznu ModelState grešku bez teksta. </summary>
    public decimal? Cena { get; set; }
    public int Godiste { get; set; }
    public int? Kilometraza { get; set; }
    public string? Gorivo { get; set; }
    public string? Menjac { get; set; }
    public int ModelId { get; set; }
    public int KategorijaId { get; set; }
}

public class OglasMiniViewModel
{
    public int Id { get; set; }
    public string? Naslov { get; set; }
    public decimal Cena { get; set; }
    public int Godiste { get; set; }
    public int Kilometraza { get; set; }
    public ModelViewModel? Model { get; set; }
    public ICollection<SlikaViewModel>? Slike { get; set; }
}

public class KorisnikProfilViewModel
{
    public int Id { get; set; }
    public string? Ime { get; set; }
    public string? Prezime { get; set; }
    public string? Email { get; set; }
    public DateTime DatumRegistracije { get; set; }
    public List<OglasMiniViewModel> Oglasi { get; set; } = new();
}

public class KorisnikAdminRowViewModel
{
    public int Id { get; set; }
    public string? Ime { get; set; }
    public string? Prezime { get; set; }
    public string? Email { get; set; }
    public string Uloga { get; set; } = "User";
    public DateTime DatumRegistracije { get; set; }
    public int BrojOglasa { get; set; }
}
