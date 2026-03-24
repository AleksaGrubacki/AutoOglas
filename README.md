## Struktura repozitorijuma

| Putanja | Sadržaj |
|---------|---------|
| `src/AutoOglasi.Web` | Prezentacioni sloj — ASP.NET Core MVC, ViewModel-i, pogledi, statički fajlovi |
| `src/AutoOglasi.BLL` | Poslovna logika — servisi, DTO-evi za komunikaciju sa UI |
| `src/AutoOglasi.DAL` | Pristup podacima — EF Core `DbContext`, entiteti, repozitorijumi |
| `docs/` | Dokumentacija rada u **.pdf / .docx** |

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