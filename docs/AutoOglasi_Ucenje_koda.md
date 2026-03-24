# AutoOglasi — obilazak koda (bez analogija)

Dokument opisuje strukturu aplikacije i ulogu glavnih delova koda. Tok obrade HTTP zahteva u MVC sloju je kraće: **HTTP zahtev → kontroler → BLL servis → DAL repozitorijum → `DbContext` / SQL Server → odgovor (View ili preusmerenje)**.

---

## 0. `appsettings.json`

Konfiguracioni fajl aplikacije. Ključ `ConnectionStrings:AutoOglasiConnection` sadrži parametre konekcije ka SQL Serveru (instanca, ime baze, autentifikacija). `Program.cs` čita ovaj string pri registraciji `DbContext`-a.

---

## 1. `Program.cs`

### `WebApplication.CreateBuilder(args)`

Kreira `WebApplicationBuilder` i učitava konfiguraciju (uključujući `appsettings.json`) i argumente komandne linije.

### `AddControllersWithViews()`

Registruje ASP.NET Core MVC: kontrolere, Razor view engine i povezane servise.

### `AddDbContext<AutoOglasiContext>(options => options.UseSqlServer(...))`

Registruje `AutoOglasiContext` kao servis sa SQL Server provajderom. Connection string se uzima iz konfiguracije.

### `AddScoped<Interfejs, Implementacija>(...)`

**Dependency injection**, životni vek **Scoped**: jedna instanca po HTTP zahtevu. Kontroler zahteva interfejs u konstruktoru; runtime dodeljuje implementaciju (`OglasRepository`, `OglasService`, itd.). Nije potrebno ručno instanciranje `new` u kontroleru.

Registrovani parovi u projektu: `IOglasRepository`/`OglasRepository`, `IOglasService`/`OglasService`, `IKorisnikRepository`/`KorisnikRepository`, `IKorisnikService`/`KorisnikService`, `IAdminRepository`/`AdminRepository`, `IAdminService`/`AdminService`.

### `AddSession(options => { ... })`

Konfiguracija sesije: trajanje neaktivnosti (`IdleTimeout`), `HttpOnly` kolačić, `IsEssential` za GDPR/saglasnost kolačića.

### `var app = builder.Build()`

Gradi `WebApplication` spreman za pokretanje pipeline-a middleware-a.

### `if (!app.Environment.IsDevelopment())`

U produkciji uključuje globalni exception handler i HSTS (HTTP Strict Transport Security).

### `UseHttpsRedirection()`

Preusmerava HTTP zahteve ka HTTPS gde je konfigurisano.

### `UseStaticFiles()`

Servira statičke fajlove iz `wwwroot`. URL tipa `/uploads/oglasi/fajl.jpg` mapira se na `wwwroot/uploads/oglasi/fajl.jpg`.

### `UseRouting()`

Omogućava usklađivanje URL-a sa endpoint rutama.

### `UseSession()`

Omogućava korišćenje `HttpContext.Session` u kasnijim slojevima (nakon ovog middleware-a).

### `UseAuthorization()`

Deo standardnog MVC pipeline-a; u ovom projektu glavna autorizacija je ručna provera sesije u kontrolerima, ne policy/role preko ASP.NET Identity.

### `MapControllerRoute(...)`

Podrazumevana konvencijska ruta `{controller=Oglasi}/{action=Index}/{id?}` — korenski URL otvara `OglasiController.Index`.

### `app.Run()`

Pokreće aplikaciju i blokira do zaustavljanja hosta.

---

## 2. Modeli (`Models/*.cs`)

### `Korisnik`

- `Id` — primarni ključ (identity u bazi).
- `Ime`, `Prezime`, `Email` — skalarna polja.
- `LozinkaHash` — čuva se BCrypt heš ulazne lozinke, ne čista lozinka.
- `Uloga` — npr. `"User"` ili `"Admin"`; podrazumevana vrednost u kodu je `"User"`.
- `DatumRegistracije` — vreme registracije.
- `ICollection<Oglas>? Oglasi` — navigacija prema kolekciji oglasa; `?` dozvoljava `null`.

### `Oglas`

- Skalarna polja: `Naslov`, `Opis`, `Cena`, `Godiste`, `Kilometraza`, `Gorivo`, `Menjac`, `DatumObjave`, `Aktivan`.
- Strani ključevi: `KorisnikId`, `ModelId`, `KategorijaId`.
- Navigacije: `Korisnik`, `Model`, `Kategorija`, `Slike` — za učitavanje povezanih entiteta preko EF Core `Include`.

### `Slika`

- `OglasId` — veza ka oglasu.
- `PutanjaFajla` — relativna URL putanja za `<img src="...">`.
- `JeNaslovna` — logičko označavanje glavne slike.
- `Oglas` — navigacija ka entitetu oglasa.

### `Marka`, `Model`, `Kategorija`

Struktura šifarnika: marka, model sa `MarkaId`, kategorija (karoserija). Navigaciona kolekcija (`Modeli`, `Oglasi`) služi za eager loading u LINQ upitima.

---

## 3. `Data/AutoOglasiContext.cs`

Klasa nasleđuje `DbContext` — centralna tačka EF Core za rad sa bazom.

**Konstruktor** prima `DbContextOptions<AutoOglasiContext>` i prosleđuje ga bazi — tu su provider (SQL Server), connection string i ostala podešavanja.

**`DbSet<T>`** — svojstvo po tabeli: `Korisnici`, `Marke`, `Modeli`, `Kategorije`, `Oglasi`, `Slike`. Preko njih se izvršavaju upiti i izmene.

**`OnModelCreating`** — eksplicitno mapiranje imena entiteta na ime tabele (`ToTable("Korisnici")` itd.) kada plural imena u bazi i imena klase nisu ista konvencijom.

---

## 4. DAL — interfejsi i repozitorijumi

### Svrha interfejsa

Interfejs (`IOglasRepository`, …) definiše javni ugovor. Implementacija enkapsulira LINQ i `SaveChanges`. Zavisnosti u višem sloju referenciju interfejs radi lakšeg testiranja i zamene implementacije.

### `IOglasRepository` — metode

- `QueryAktivniOglasi()` — `IQueryable<Oglas>` sa filterom `Aktivan` i `Include` za potrebne navigacije; izvršenje odloženo do `ToListAsync` itd.
- `GetDetaljiAsync` / `GetByIdAsync` — učitavanje jednog oglasa sa različitim dubinama include-a.
- `GetMarkeAsync`, `GetModeliAsync`, `GetKategorijeAsync` — podaci za padajuće liste.
- `AddAsync`, `UpdateAsync`, `DeleteAsync` — izmene u **Change Tracker**-u konteksta dok se ne pozove `SaveChanges`.
- Metode za `Slika` — dodavanje, pronalaženje, lista po `OglasId`, brisanje.
- `SaveChangesAsync` — propagacija izmena u bazu.

### `OglasRepository` — tehničke napomene

- `Include` / `ThenInclude` — eager loading povezanih entiteta u jednom ili malom broju upita.
- `!` posle navigacije (`Model!`) — C# null-forgiving operator: programer tvrdi da vrednost nije null u tom kontekstu da bi se izbeglo upozorenje kompajlera.

### `KorisnikRepository`

- `GetByIdWithOglasiAsync` — uključuje oglase i njihove slike radi konzistentnog brisanja (izbegavanje problema sa FK ako baza nema CASCADE).
- `DeleteSlikeAsync`, `DeleteOglaseAsync` — `RemoveRange` nad kolekcijama pre brisanja korisnika.

### `AdminRepository`

Agregati (`Count`), lista svih oglasa sa include-ima, `FindAsync` za pojedinačne izmene/brisanje.

---

## 5. BLL — servisi

### `OglasService`

- **`GetFilteredAsync`** — prosleđuje filtere u `Where` klauzule nad `QueryAktivniOglasi()`, sortiranje po `DatumObjave` opadajuće, materijalizacija `ToListAsync()`.
- **`GetDetaljiAsync` / `GetByIdAsync`** — delegacija na repozitorijum.
- **`KreirajAsync`** — postavlja `DatumObjave`, `Aktivan = true`, `KorisnikId`; `Add` + `SaveChanges`; vraća generisani `Id`.
- **`UrediAsync`** — učitava postojeći entitet; provera vlasništva ili admina; kopiranje polja sa DTO-modele iz forme; `Update` + `SaveChanges`.
- **`ObrisiAsync`** — ista autorizaciona provera; `Delete` + `SaveChanges`.
- **`DodajSlikeAsync`** — validacija liste fajlova; `Directory.CreateDirectory`; determinacija naslovne slike za prvu seriju; `FileStream` + `CopyToAsync`; unos redova `Slika`; jedan `SaveChanges` na kraju.
- **`ObrisiSlikuAsync`** — autorizacija; validacija da `slika.OglasId` odgovara; brisanje reda; brisanje fajla sa diska ako postoji; ako nema naslovne na oglasu, prva preostala dobija `JeNaslovna = true`.
- **`PostaviNaslovnuAsync`** — iteracija slika oglasa, tačno jedna sa `JeNaslovna = true`.
- **`GetGoriva` / `GetMenjaci` / `GetGodine`** — statičke liste u memoriji (nisu u bazi).
- **`GetMarkeSelectListAsync`** — `SelectList` za Razor `select` (dataValueField i dataTextField: `Naziv`).

### `KorisnikService`

- **`RegistrujAsync`** — validacija lozinki; jedinstvenost emaila; BCrypt `HashPassword`; rezultat kao tuple `(Uspeh, Greska, Korisnik)`.
- **`PrijaviAsync`** — `GetByEmailAsync`; `BCrypt.Verify`.
- **`PromeniUloguAsync`** — izmena stringa `Uloga` između Admin/User.
- **`ObrisiKorisnikaAsync`** — učitavanje sa oglasima i slikama; brisanje u redosledu slike → oglasi → korisnik.  
  Napomena: parametar `mojeId` u potpisu se u telu trenutno ne koristi za sprečavanje brisanja sopstvenog naloga; ponašanje zavisi od UI/admin logike.

### `AdminService`

Brojanje za dashboard; lista oglasa; `ObrisiOglasAsync`; `ToggleOglasAsync` (invertovanje `Aktivan` + `Update` + `SaveChanges`).

---

## 6. Kontroleri

### `HomeController.Index`

`RedirectToAction("Index", "Oglasi")` — koren aplikacije vodi na listu oglasa.

### `OglasiController`

- Zavisnosti: `IOglasService`, `IWebHostEnvironment` (`WebRootPath` za putanju uploada).
- **`Index`** — parametri iz query stringa; `ViewBag` za filtere; `View(oglasi)`.
- **`Detalji`** — `NotFound()` ako nema entiteta; `ViewBag.MozeUrediti` iz sesije (`KorisnikId` i/ili `KorisnikUloga == "Admin"`).
- **`Novi` GET** — redirect na `Korisnici/Prijava` ako nema emaila u sesiji; `PopuniDropdowneAsync`.
- **`Novi` POST** — `[ValidateAntiForgeryToken]`; `ModelState`; `KorisnikId` iz sesije (`?? 1` je tehnički fallback; bolja praksa je zahtevati validan id bez podrazumevanja); `KreirajAsync` + `DodajSlikeAsync`; redirect na Index.
- **`Uredi` GET/POST** — ista sesija i provera vlasništva/admin; POST poziva `UrediAsync` i opciono `DodajSlikeAsync`.
- **`ObrisiSliku` / `PostaviNaslovnu`** — POST sa `slikaId`, `oglasId`; redirect na `Uredi`.
- **`Obrisi`** — `ObrisiAsync`; `NotFound` ako false.
- **`PopuniDropdowneAsync`** — punjenje `ViewBag` iz servisa.

### `KorisniciController`

Registracija i prijava: poziv servisa; pri uspehu `Session.SetString` / `SetInt32`; pri grešci `ViewBag.Greska`. `Odjava`: `Session.Clear`. `Profil`: zaštita ako nema `KorisnikId`.

### `AdminController`

`JeAdmin()` provera pre svake akcije. Akcije delegiraju na `IAdminService` i `IKorisnikService`. Ime akcije `PrommeniUlogu` mora se poklopiti sa `asp-action` u view-u.

---

## 7. Views (Razor)

### `_ViewImports.cshtml`

Globalni `@using` i `TagHelpers`.

### `_ViewStart.cshtml`

Postavlja podrazumevani `Layout = "_Layout"`.

### Uobičajeni elementi

- `@model` — tip modela view-a.
- Anti-forgery token u formama koje šalju POST na akcije označene `[ValidateAntiForgeryToken]`.
- `enctype="multipart/form-data"` obavezan za upload fajlova.
- Klijentski JS u `Novi`/`Uredi` filtrira modele po marki; server prima samo izabrani `ModelId`.

---

## 8. Tok POST forme (npr. čuvanje oglasa)

1. Browser šalje POST sa telom forme i eventualno fajlovima.  
2. Model binding popunjava `Oglas` i parametre poput `List<IFormFile>? slike`.  
3. Kontroler poziva BLL.  
4. BLL poziva DAL; DAL poziva `SaveChangesAsync`.  
5. Za slike: upis na disk + unos u `Slike`.  
6. `RedirectToAction` implementira uzorak **PRG** (Post-Redirect-Get) da se izbegne duplo slanje pri osvežavanju stranice.

---

## 9. Ključevi u sesiji

- `KorisnikEmail`
- `KorisnikIme`
- `KorisnikUloga`
- `KorisnikId`

Postavljaju se pri uspešnoj prijavi/registraciji; koriste se za uslovni prikaz i autorizacione provere.

---

## 10. Ograničenja dokumenta

- Ne sadrži red-po-red komentar svakog `.cshtml` fajla.  
- struktura SQL šeme u bazi nije opisana ovde — mora odgovarati entitetima i kolonama koje EF koristi.

---

## 11. Predlog reda učenja

1. `Program.cs` — redosled middleware-a i DI registracija.  
2. Tok prijave: `KorisniciController` → `KorisnikService` → `KorisnikRepository`.  
3. Tok novog oglasa: `OglasiController` → `OglasService` → `OglasRepository` + fajl sistem.  
4. Admin: `AdminController` → `AdminService` → `AdminRepository`.

---

Kraj dokumenta. Svi opisi vezani su za konkretan kod u repozitorijumu `AutoOglasi`; nijedna rečenica ne koristi figurativne analogije.
