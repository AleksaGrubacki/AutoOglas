# Seminarski rad — popunjen tekst (po PDF template-u)

> **Napomena:** Ovaj fajl je spreman za kopiranje u Word preko tvog `SeminarskiRad_Template.docx`. Naslovna strana i sadržaj zadržavaju istu strukturu kao u PDF-u.  
> **Štampanje:** Za sadržaj ažuriraj brojeve strana kada završiš dokument u Wordu.

---

## Naslovna strana (zameni vitičaste zagrade)

**Univerzitet u Novom Sadu**  
**Tehnički fakultet „Mihajlo Pupin“, Zrenjanin**

**Predmet:** Arhitektura informacionih sistema  
**Seminarski rad**

**Naziv teme:** Web aplikacija za oglašavanje polovnih automobila (AutoOglasi)

**Profesor:** prof. dr Biljana Radulović  
**Asistent:** MSc Velibor Premčevski  

**Student:** {Ime Prezime, broj indeksa}

---

## Sadržaj (ažuriraj brojeve strana u Wordu)

1. Uvod .............................................................  
2. Korišćene tehnologije .........................................  
3. Korisničko uputstvo ..........................................  
4. Ključni delovi koda ..........................................  
5. Zaključak .....................................................  
6. Literatura ....................................................  

---

## 1. Uvod

U ovom radu predstavljena je web aplikacija **AutoOglasi**, namenjena oglašavanju i pregledu polovnih automobila. Korisnici mogu da se registruju i prijave, postavljaju oglase sa fotografijama, menjaju i brišu sopstvene oglase, kao i da pretražuju aktivne oglase po filterima (marka, gorivo, cena, godište). Administrator ima dodatnu kontrolu nad oglasima i korisnicima.

Aplikacija je izgrađena po slojevitoj arhitekturi: prezentacioni sloj (MVC kontroleri i Razor pogledi), **poslovni sloj (BLL)** i **sloj pristupa podacima (DAL)** preko **Entity Framework Core** i **Microsoft SQL Server** baze. Autentifikacija za obične korisnike i admin ulogu realizovana je putem **sesije** nakon prijave; lozinke se čuvaju heširane (**BCrypt**).

Cilj rada je da se pokaže funkcionalan portal za male oglašivače automobila uz jasno odvajanje odgovornosti slojeva i jednostavno korisničko uputstvo.

---

## 2. Korišćene tehnologije

| Tehnologija / alat | Uloga u projektu |
|--------------------|------------------|
| **ASP.NET Core** (MVC, minimalni `Program.cs`) | Web host, rutiranje, middleware, statički fajlovi, sesija |
| **C#** | Implementacija kontrolera, servisa, repozitorijuma i modela |
| **Razor Views (.cshtml)** | HTML šabloni i UI |
| **Entity Framework Core** | ORM, rad sa SQL Server bazom |
| **Microsoft SQL Server** | Relaciona baza podataka |
| **Bootstrap / Bootswatch / ikone** | Ako je uključeno u `_Layout` — responsive izgled (prema projektu) |
| **BCrypt.Net-Next** | Bezbedno heširanje lozinki |
| **Visual Studio / dotnet CLI** | Razvoj i pokretanje aplikacije |

---

## 3. Korisničko uputstvo

*Prema template-u: štampaj screenshot stranica i uz svaku ukratko opiši funkcionalnost.*

### 3.1. Početna / lista oglasa

**`[SCREENSHOT 1]`** Stranica **Oglasi / Index** — hero sekcija sa formom za pretragu (marka, gorivo, cena od/do, godište od), ispod kartice sa listom aktivnih oglasa.

**Opis:** Korisnik vidi samo **aktivne** oglase. Može filtrirati rezultate bez ponovnog učitavanja kompletne logike na klijentu — filteri idu kao GET parametri.

---

### 3.2. Detalji oglasa

**`[SCREENSHOT 2]`** Stranica **Oglasi / Detalji** — galerija slika, opis, specifikacije, cena, podaci o prodavcu.

**Opis:** Prikazuju se svi ključni podaci o vozilu i oglasu. Dugme **„Kontaktiraj prodavca“** otvara podrazumevani mejl klijent sa adresom prodavca i predmetom poruke vezanom za oglas.

**`[SCREENSHOT 3]`** (opciono) Isti ekran kada je ulogovan vlasnik ili admin — vidljiva su dugmad **Izmeni oglas** i **Obriši oglas**.

---

### 3.3. Novi oglas

**`[SCREENSHOT 4]`** **Oglasi / Novi** — forma sa markom/modelom, karoserijom, gorivom, menjačem, cenama, upload više slika.

**Opis:** Pristup imaju samo prijavljeni korisnici; inače sledi preusmerenje na prijavu. Prva otpremljena slika postaje naslovna; ostale se dodaju uz oglas u bazi.

---

### 3.4. Izmena oglasa

**`[SCREENSHOT 5]`** **Oglasi / Uredi** — izmena polja oglasa, sekcija postojećih slika (postavljanje naslovne, brisanje pojedinačne slike), dodavanje novih slika.

**Opis:** Oglas može menjati vlasnik oglasa ili admin. Ostali korisnici ne mogu pristupiti izmeni tuđeg oglasa.

---

### 3.5. Profil korisnika

**`[SCREENSHOT 6]`** **Korisnici / Profil** — lični podaci i lista „Moji oglasi“ sa linkovima za pregled, izmenu i brisanje.

---

### 3.6. Registracija i prijava

**`[SCREENSHOT 7]`** **Korisnici / Registracija** — forma sa potvrdom lozinke; validacija jedinstvenog emaila.

**`[SCREENSHOT 8]`** **Korisnici / Prijava** — email i lozinka; uspešna prijava postavlja sesiju (email, ime, uloga, id).

---

### 3.7. Admin panel

**`[SCREENSHOT 9]`** **Admin / Index** — statistike (broj oglasa, aktivnih, korisnika).

**`[SCREENSHOT 10]`** **Admin / Oglasi** — tabela svih oglasa; aktivacija/deaktivacija, pregled detalja, brisanje.

**`[SCREENSHOT 11]`** **Admin / Korisnici** — lista korisnika; promena uloge (User/Admin), brisanje korisnika.

**Opis:** Pristup imaju korisnici čija je uloga u sesiji **Admin**. Ostali dobijaju preusmerenje na javni deo aplikacije.

---

## 4. Ključni delovi koda

*Poslovna pravila, ograničenja, specifična ponašanja — primeri kako ih objasniti na odbrani.*

### 4.1. Autorizacija „u aplikaciji“ (sesija)

- Nakon prijave/registracije u sesiju se upisuju npr. `KorisnikEmail`, `KorisnikId`, `KorisnikUloga`.
- Akcije koje zahtevaju nalog (novi oglas, profil, izmena oglasa) proveravaju sesiju i preusmeravaju na **Prijava** ako korisnik nije ulogovan.
- Admin rute u **AdminController** na početku proveravaju da li je `KorisnikUloga == "Admin"`.

### 4.2. Vlasništvo nad oglasom

- **Izmena i brisanje oglasa (korisnički deo):** dozvoljeno samo ako `KorisnikId` oglasa odgovara ulogovanom korisniku **ili** ako je korisnik admin.
- **Detalji oglasa:** u pogledu se prikazuju dugmad za izmenu/brisanje samo kad `ViewBag.MozeUrediti` indikuje vlasnika ili admina.

### 4.3. Aktivni oglasi na javnoj listi

- Na **Index** listing prikazuju se samo oglasi sa `Aktivan == true`. Admin može deaktivirati oglas — tada nestaje sa javne liste, ali ostaje u admin tabeli.

### 4.4. Registracija i lozinke

- Lozinka se ne čuva u čitljivom obliku; čuva se **BCrypt** heš.
- Pri registraciji: provera da se lozinke poklapaju i da email nije već zauzet.

### 4.5. Slike oglasa

- Upload ide u **`wwwroot/uploads/oglasi`**; u bazi se čuva relativna putanja (`/uploads/oglasi/...`) radi prikaza preko statičkih fajlova.
- Pri dodavanju prvog skupa slika prva slika je **naslovna**; pri izmeni oglasa mogu se dodati nove slike ili obrisati pojedinačne; ako nema naslovne posle brisanja, jedna od preostalih se postavlja kao naslovna (poslovna logika u BLL).

### 4.6. Slojevi BLL i DAL

- **DAL:** klase tipa `*Repository` izvršavaju upite i CRUD nad entitetima preko `DbContext`.
- **BLL:** klase tipa `*Service` sadrže poslovnu logiku (npr. filtriranje oglasa, pravilnik za izmenu, upload slika).
- **Kontroleri** ostaju tanki: učitavaju sesiju, pozivaju servis i vraćaju view ili redirect.

---

## 5. Zaključak

U radu je razvijena funkcionalna web aplikacija za oglašavanje automobila sa jasno izdvojenim slojevima, sesijskom prijavom i administratorskim modulom. Korisnici mogu jednostavno da objavljuju i upravljaju svojim oglasima uz vizuelno konzistentan interfejs.

**Moguća unapređenja:**  
- Uvoditi **ASP.NET Identity** umesto ručne sesije za robusniju autorizaciju.  
- Čuvanje slika u **cloud storage** radi lakšeg deploy-a.  
- **Email** slanje preko SMTP umesto samo `mailto` linka.  
- Paginacija i naprednija pretraga po više polja.

**GitHub repozitorijum:** {nalepi link ka javnom repozitorijumu, npr. `https://github.com/korisnik/AutoOglas`}

---

## 6. Literatura

1. Microsoft Learn — *ASP.NET Core MVC* — https://learn.microsoft.com/aspnet/core/mvc/  
2. Microsoft Learn — *Entity Framework Core* — https://learn.microsoft.com/ef/core/  
3. Microsoft Learn — *Razor Pages / Views* — https://learn.microsoft.com/aspnet/core/mvc/views/  
4. SQL Server dokumentacija — https://learn.microsoft.com/sql/  
5. OWASP — *Session Management* — https://cheatsheetseries.owasp.org/cheatsheets/Session_Management_Cheat_Sheet.html  

*(Dopuni prema pravilima fakulteta za citiranje monografija, članaka ili udžbenika ako zahtevaju Harvard/IEEE format.)*
