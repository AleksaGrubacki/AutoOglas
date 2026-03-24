namespace AutoOglasi.BLL.Dto;

/// <summary> Opcija za HTML padajuću listu (prezentacioni sloj pravi SelectList). </summary>
public class SelectOptionDto
{
    public string Value { get; set; } = "";
    public string Text { get; set; } = "";
}

public class SlikaDto
{
    public int Id { get; set; }
    public int OglasId { get; set; }
    public string? PutanjaFajla { get; set; }
    public bool JeNaslovna { get; set; }
}

public class MarkaDto
{
    public int Id { get; set; }
    public string? Naziv { get; set; }
}

public class ModelDto
{
    public int Id { get; set; }
    public string? Naziv { get; set; }
    public int MarkaId { get; set; }
    public MarkaDto? Marka { get; set; }
}

public class KategorijaDto
{
    public int Id { get; set; }
    public string? Naziv { get; set; }
}

public class KorisnikKratkoDto
{
    public int Id { get; set; }
    public string? Ime { get; set; }
    public string? Prezime { get; set; }
    public string? Email { get; set; }
}

/// <summary> Podaci za sesiju / prikaz — bez osetljivih polja. </summary>
public class KorisnikDto
{
    public int Id { get; set; }
    public string? Ime { get; set; }
    public string? Prezime { get; set; }
    public string? Email { get; set; }
    public string Uloga { get; set; } = "User";
    public DateTime DatumRegistracije { get; set; }
}

public class OglasUpsertDto
{
    public int Id { get; set; }
    public string? Naslov { get; set; }
    public string? Opis { get; set; }
    public decimal Cena { get; set; }
    public int Godiste { get; set; }
    public int Kilometraza { get; set; }
    public string? Gorivo { get; set; }
    public string? Menjac { get; set; }
    public int ModelId { get; set; }
    public int KategorijaId { get; set; }
}

/// <summary> Kartica oglasa na listi. </summary>
public class OglasListaDto
{
    public int Id { get; set; }
    public string? Naslov { get; set; }
    public decimal Cena { get; set; }
    public int Godiste { get; set; }
    public string? Gorivo { get; set; }
    public string? Menjac { get; set; }
    public int Kilometraza { get; set; }
    public DateTime DatumObjave { get; set; }
    public List<SlikaDto> Slike { get; set; } = new();
    public ModelDto? Model { get; set; }
}

/// <summary> Stranica detalji. </summary>
public class OglasDetaljiDto
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
    public List<SlikaDto> Slike { get; set; } = new();
    public ModelDto? Model { get; set; }
    public KategorijaDto? Kategorija { get; set; }
    public KorisnikKratkoDto? Korisnik { get; set; }
}

/// <summary> Red u admin tabeli oglasa. </summary>
public class OglasAdminDto
{
    public int Id { get; set; }
    public string? Naslov { get; set; }
    public decimal Cena { get; set; }
    public DateTime DatumObjave { get; set; }
    public bool Aktivan { get; set; }
    public ModelDto? Model { get; set; }
    public KorisnikKratkoDto? Korisnik { get; set; }
}

public class OglasMiniDto
{
    public int Id { get; set; }
    public string? Naslov { get; set; }
    public decimal Cena { get; set; }
    public int Godiste { get; set; }
    public int Kilometraza { get; set; }
    public List<SlikaDto> Slike { get; set; } = new();
    public ModelDto? Model { get; set; }
}

/// <summary> Profil korisnika sa oglasima. </summary>
public class KorisnikProfilDto
{
    public int Id { get; set; }
    public string? Ime { get; set; }
    public string? Prezime { get; set; }
    public string? Email { get; set; }
    public DateTime DatumRegistracije { get; set; }
    public List<OglasMiniDto> Oglasi { get; set; } = new();
}

/// <summary> Admin lista korisnika (bez kolekcije oglasa u DTO-u). </summary>
public class KorisnikAdminDto
{
    public int Id { get; set; }
    public string? Ime { get; set; }
    public string? Prezime { get; set; }
    public string? Email { get; set; }
    public string Uloga { get; set; } = "User";
    public DateTime DatumRegistracije { get; set; }
    public int BrojOglasa { get; set; }
}
