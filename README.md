# AutoOglasi — n-tier (.NET 10)

## Struktura repozitorijuma

| Putanja | Sadržaj |
|---------|---------|
| `src/AutoOglasi.Web` | Prezentacioni sloj — ASP.NET Core MVC, ViewModel-i, pogledi, statički fajlovi |
| `src/AutoOglasi.BLL` | Poslovna logika — servisi, DTO-evi za komunikaciju sa UI |
| `src/AutoOglasi.DAL` | Pristup podacima — EF Core `DbContext`, entiteti, repozitorijumi |
| `docs/` | Dokumentacija rada u **.pdf / .docx** (uputstvo u `docs/README.txt`) |

Rešenje: otvori **`AutoOglas.sln`** u korenu.

## Pokretanje

```text
cd src/AutoOglasi.Web
dotnet ef database update --project ../AutoOglasi.DAL/AutoOglasi.DAL.csproj
dotnet run
```

Migracije su u **`src/AutoOglasi.DAL/Migrations/`**. Nova migracija:

```text
dotnet ef migrations add ImeMigracije --project src/AutoOglasi.DAL/AutoOglasi.DAL.csproj --startup-project src/AutoOglasi.Web/AutoOglasi.Web.csproj
```

Connection string: `src/AutoOglasi.Web/appsettings.json` → `AutoOglasiConnection`.

### Baza već postoji (tabele od starog projekta)

Ako `dotnet ef database update` prijavi da objekat (npr. `Kategorije`) već postoji, šema je verovatno ista — označi migraciju kao primenjenu bez ponovnog kreiranja tabela: pokreni SQL iz **`src/AutoOglasi.DAL/Scripts/Baseline_postojeca_baza.sql`** u SQL Server Management Studio (proveri `MigrationId` da odgovara fajlu u `Migrations/`). Zatim `database update` neće pokušavati ponovo `CREATE TABLE`.


## Arhitektura (zahtev predmeta)

- **UI** se oslanja isključivo na **BLL** (projekat `Web` referiše samo `AutoOglasi.BLL`). Komunikacija sa prikazom ide preko **ViewModel-a**, ne preko entiteta.
- **BLL** referiše **DAL**, sadrži pravila i mapiranje entitet ↔ DTO.
- **DAL** sadrži entitete i direktan rad sa bazom.
