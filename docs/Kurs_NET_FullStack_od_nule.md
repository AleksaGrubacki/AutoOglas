# Kurs: od nule do prvog posla (.NET backend + moderan frontend)

Ovaj dokument pretpostavlja da **ne znaš ništa unapred**. Pojmovi se uvode redom. Posle objašnjenja slede **primeri koda** gde ima smisla. Na kraju **svake lekcije** ima tačno **jedan zadatak**.

**Kako koristiti:** čitaj redom; kopiraj i prilagođavaj primere u svom projektu. Verzije alata: **.NET 8+** (SDK LTS), **Node.js LTS**.

---

## Lekcija 0 — Šta uopšte radiš kada „programiraš“

**Računar** izvršava instrukcije zapisane u fajlovima (**izvorni kod**). **Kompajler** ili **runtime** prevodi to u oblik koji mašina izvršava.

**Program** je skup logike koji rešava jedan problem. **Programiranje** je pisanje te logike tako da bude ispravna i održiva.

### Primer (konceptualno — još nemaš alate)

Tekst u fajlu `Program.cs` u konzolnom projektu često izgleda ovako (objašnjenje linija):

```csharp
// Jednolinijski komentar — računar ovo ignoriše.

/*
  Višelinijski komentar.
*/

// Main je tačka ulaska: odavde kreće izvršavanje.
Console.WriteLine("Zdravo"); // ispis u terminal
```

`Console.WriteLine` znači: „ispiši tekst i pređi u novi red“.

**Zadatak (Lekcija 0):** U jednoj rečenici napiši šta želiš da tvoj prvi program u browseru radi (npr. prijava + lista tvojih stvari).

---

## Lekcija 1 — Web: browser, server, URL

**Browser** šalje **HTTP zahtev** na **URL**; **server** vraća **odgovor** (HTML, JSON, slike…). **Backend** = logika na serveru. **Frontend** = ono što korisnik vidi u browseru.

### Primer „šta browser šalje“ (pojednostavljeno tekstualno)

```http
GET /api/todos HTTP/1.1
Host: localhost:5287
Accept: application/json
```

Odgovor može biti:

```http
HTTP/1.1 200 OK
Content-Type: application/json

[{"id":1,"title":"Kupi mleko","isDone":false}]
```

**Zadatak (Lekcija 1):** F12 → Network → osveži stranicu → zapiši metodu, status kod i da li je odgovor HTML ili JSON.

---

## Lekcija 2 — .NET i C#

**.NET SDK** sadrži alate; **runtime** izvršava aplikaciju. **C#** je jezik; **.csproj** opisuje projekat.

### Primer `.csproj` (konzolna aplikacija)

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

- `TargetFramework` — verzija .NET-a za koji se projekat gradi.
- `Nullable` — kompajler upozorava na mogući `null`.

**Zadatak (Lekcija 2):** Instaliraj .NET SDK LTS; `dotnet --version` → zapiši verziju.

---

## Lekcija 3 — Terminal, `dotnet new`, `dotnet run`

Komande u **PowerShell**-u:

```powershell
cd C:\Projekti
dotnet new console -n MojPrviProgram
cd MojPrviProgram
dotnet run
```

### Primer `Program.cs` (konzola) — promenljive, petlja, uslov

```csharp
string ime = "Ana";
int broj = 3;

if (broj > 0)
{
    Console.WriteLine($"{ime}, broj je pozitivan: {broj}");
}

for (int i = 0; i < broj; i++)
{
    Console.WriteLine($"Korak {i}");
}
```

**Interpolacija stringa** `$"..."` ubacuje vrednosti u tekst.

**Zadatak (Lekcija 3):** `dotnet new console -n MojPrviProgram` → `dotnet run` → zapiši šta se ispisuje.

---

## Lekcija 4 — Git

### Primer komandi

```powershell
cd MojPrviProgram
git init
git add .
git commit -m "init: prvi console projekat"
git log --oneline
```

**Zadatak (Lekcija 4):** Napravi commit; zapiši prvih 7 karaktera hasha iz `git log --oneline`.

---

## Lekcija 5 — HTTP metode i status kodovi

### Tabela (referenca)

| Metoda | Tipična upotreba        |
|--------|-------------------------|
| GET    | čitanje                 |
| POST   | kreiranje               |
| PUT    | zamena celog resursa    |
| PATCH  | delimična izmena        |
| DELETE | brisanje                |

### Primer `curl` (ako imaš curl u terminalu)

```powershell
# GET
curl -i http://localhost:5287/api/health

# POST sa JSON telom (Windows PowerShell — pažnja na navodnike)
curl -i -X POST http://localhost:5287/api/todos `
  -H "Content-Type: application/json" `
  -d '{"title":"Novi zadatak","isDone":false}'
```

**Zadatak (Lekcija 5):** Tri rečenice: GET vs POST; kada **404**.

---

## Lekcija 6 — JSON

### Primer validnog JSON-a

```json
{
  "id": 1,
  "email": "korisnik@example.com",
  "jeAktivan": true,
  "uloge": [ "User", "Admin" ],
  "profil": {
    "grad": "Beograd"
  }
}
```

### Niz objekata

```json
[
  { "id": 1, "title": "A", "isDone": false },
  { "id": 2, "title": "B", "isDone": true }
]
```

**Zadatak (Lekcija 6):** Sačuvaj `primer.json` sa `id`, `email`, `jeAktivan`.

---

## Lekcija 7 — REST endpointi (dizajn)

### Primer mape ruta za `todos`

```text
GET    /api/todos           → lista
GET    /api/todos/{id}      → jedan po id
POST   /api/todos           → kreiranje (telo JSON)
PUT    /api/todos/{id}      → puna izmena
PATCH  /api/todos/{id}      → delimična (npr. samo isDone)
DELETE /api/todos/{id}      → brisanje
```

**Zadatak (Lekcija 7):** Na papiru napiši tri rute: lista GET, kreiranje POST, brisanje DELETE sa `{id}`.

---

## Lekcija 8 — ASP.NET Core Web API: prvi projekat

```powershell
dotnet new webapi -n MojWebApi -o MojWebApi
cd MojWebApi
dotnet run
```

### Tipičan `Program.cs` (.NET 6+ šablon)

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
```

*(Ako koristiš stariji šablon sa `Swagger`, videćeš `AddEndpointsApiExplorer` + `UseSwagger` — oba pristupa su uobičajena.)*

**Zadatak (Lekcija 8):** Pokreni projekat; otvori Swagger/OpenAPI URL iz konzole; zapiši pun URL.

---

## Lekcija 9 — Kontroler, ruta, akcija

### Primer `Controllers/HealthController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;

namespace MojWebApi.Controllers;

[ApiController]
[Route("api/[controller]")] // "Health" → /api/Health
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = "ok" });
    }
}
```

- `[ApiController]` uključuje podrazumevanu validaciju i ponašanje za API.
- `Ok(...)` vraća **200** sa telom (ovde anonimni objekat → JSON).

### Alternativa: fiksna ruta

```csharp
[HttpGet("/api/health")]
public IActionResult HealthCheck() => Ok(new { status = "ok" });
```

**Zadatak (Lekcija 9):** Implementuj `GET` koji vraća `{ "status": "ok" }`; zapiši URL.

---

## Lekcija 10 — Dependency Injection

### Interfejs i implementacija

```csharp
// Services/IClock.cs
namespace MojWebApi.Services;

public interface IClock
{
    string GetUtcNowIso();
}
```

```csharp
// Services/SystemClock.cs
namespace MojWebApi.Services;

public class SystemClock : IClock
{
    public string GetUtcNowIso() => DateTime.UtcNow.ToString("o");
}
```

### Registracija u `Program.cs`

```csharp
using MojWebApi.Services;

builder.Services.AddSingleton<IClock, SystemClock>();
```

### Injekcija u kontroler

```csharp
public class TimeController : ControllerBase
{
    private readonly IClock _clock;

    public TimeController(IClock clock)
    {
        _clock = clock;
    }

    [HttpGet("/api/time")]
    public IActionResult GetTime() => Ok(new { utc = _clock.GetUtcNowIso() });
}
```

**Zadatak (Lekcija 10):** `IClock` + registracija + endpoint koji vraća vreme.

---

## Lekcija 11 — CRUD u memoriji

### Entitet (model u memoriji)

```csharp
// Models/TodoItem.cs
namespace MojWebApi.Models;

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public bool IsDone { get; set; }
}
```

### Servis sa listom

```csharp
// Services/TodoStore.cs
using MojWebApi.Models;

namespace MojWebApi.Services;

public class TodoStore
{
    private readonly List<TodoItem> _items = new();
    private int _nextId = 1;

    public IReadOnlyList<TodoItem> GetAll() => _items.ToList();

    public TodoItem? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);

    public TodoItem Add(string title)
    {
        var item = new TodoItem { Id = _nextId++, Title = title, IsDone = false };
        _items.Add(item);
        return item;
    }

    public bool SetDone(int id, bool isDone)
    {
        var item = GetById(id);
        if (item is null) return false;
        item.IsDone = isDone;
        return true;
    }

    public bool Delete(int id) => _items.RemoveAll(x => x.Id == id) > 0;
}
```

### Registracija

```csharp
builder.Services.AddSingleton<TodoStore>();
```

### Kontroler

```csharp
using MojWebApi.Models;
using MojWebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace MojWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly TodoStore _store;

    public TodosController(TodoStore store) => _store = store;

    [HttpGet]
    public ActionResult<IEnumerable<TodoItem>> GetAll() => Ok(_store.GetAll());

    [HttpGet("{id:int}")]
    public ActionResult<TodoItem> GetById(int id)
    {
        var item = _store.GetById(id);
        return item is null ? NotFound() : Ok(item);
    }

    public record CreateTodoRequest(string Title);

    [HttpPost]
    public ActionResult<TodoItem> Create([FromBody] CreateTodoRequest body)
    {
        var created = _store.Add(body.Title);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    public record SetDoneRequest(bool IsDone);

    [HttpPatch("{id:int}")]
    public IActionResult PatchDone(int id, [FromBody] SetDoneRequest body)
    {
        if (!_store.SetDone(id, body.IsDone)) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (!_store.Delete(id)) return NotFound();
        return NoContent();
    }
}
```

**Zadatak (Lekcija 11):** Implementuj ovakav CRUD u memoriji; testiraj u Swaggeru.

---

## Lekcija 12 — DTO i validacija

### DTO sa atributima

```csharp
using System.ComponentModel.DataAnnotations;

namespace MojWebApi.Models;

public class CreateTodoDto
{
    [Required(ErrorMessage = "Naslov je obavezan.")]
    [MaxLength(500)]
    public string Title { get; set; } = "";
}
```

### Akcija koja validira

```csharp
[HttpPost("v2")]
public ActionResult<TodoItem> CreateValidated([FromBody] CreateTodoDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var created = _store.Add(dto.Title);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
}
```

**Zadatak (Lekcija 12):** Prazan naslov → **400** i vidi greške u telu odgovora.

---

## Lekcija 13 — 404 sa JSON telom

```csharp
[HttpGet("{id:int}/detail")]
public IActionResult GetDetail(int id)
{
    var item = _store.GetById(id);
    if (item is null)
        return NotFound(new { message = "Todo nije pronađen" });
    return Ok(item);
}
```

`NotFound(obj)` šalje **404** sa JSON-om ako je serializacija podešena (podrazumevano jeste).

**Zadatak (Lekcija 13):** Za nepostojeći `id` dobijaš **404** i `{ "message": "..." }`.

---

## Lekcija 14 — Baza: tabela, red, ključ

Nema obaveznog koda; connection string zavisi od sistema.

### Primer `appsettings.Development.json` (SQL Server lokalno)

```json
{
  "ConnectionStrings": {
    "Default": "Server=(localdb)\\mssqllocaldb;Database=todoapp;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Primer (PostgreSQL)

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=todoapp;Username=postgres;Password=tvojaLozinka"
  }
}
```

**Zadatak (Lekcija 14):** Kreiraj praznu bazu `todoapp`; zapiši connection string (lokalno, ne commit-uj tajne na javni GitHub).

---

## Lekcija 15 — EF Core: entitet, DbContext, migracija

### Paketi (CLI)

```powershell
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
# ili: Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet tool install --global dotnet-ef
```

### Entitet

```csharp
using System.ComponentModel.DataAnnotations;

namespace MojWebApi.Models;

public class TodoItemEntity
{
    public int Id { get; set; }

    [MaxLength(500)]
    public string Title { get; set; } = "";

    public bool IsDone { get; set; }
}
```

### DbContext

```csharp
using Microsoft.EntityFrameworkCore;

namespace MojWebApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TodoItemEntity> Todos => Set<TodoItemEntity>();
}
```

### `Program.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using MojWebApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
```

### Migracije

```powershell
dotnet ef migrations add InitialCreate -o Data/Migrations
dotnet ef database update
```

**Zadatak (Lekcija 15):** Tabela u bazi postoji nakon `database update`.

---

## Lekcija 16 — CRUD preko EF Core-a (u kontroleru — za učenje)

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MojWebApi.Data;
using MojWebApi.Models;

namespace MojWebApi.Controllers;

[ApiController]
[Route("api/db-todos")]
public class DbTodosController : ControllerBase
{
    private readonly AppDbContext _db;

    public DbTodosController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<TodoItemEntity>>> GetAll(CancellationToken ct)
        => await _db.Todos.AsNoTracking().ToListAsync(ct);

    [HttpPost]
    public async Task<ActionResult<TodoItemEntity>> Create(TodoItemEntity body, CancellationToken ct)
    {
        _db.Todos.Add(body);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = body.Id }, body);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoItemEntity>> GetById(int id, CancellationToken ct)
    {
        var item = await _db.Todos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return item is null ? NotFound(new { message = "Nije pronađeno" }) : Ok(item);
    }
}
```

U produkciji ovde obično stoji **servisni sloj**; za kurs je dovoljno da vidiš tok podataka.

**Zadatak (Lekcija 16):** Podaci ostaju posle restarta aplikacije.

---

## Lekcija 17 — Relacija 1:N i seed

### Entiteti

```csharp
namespace MojWebApi.Models;

public class AppUser
{
    public int Id { get; set; }
    public string Email { get; set; } = "";

    public ICollection<TodoItemEntity> Todos { get; set; } = new List<TodoItemEntity>();
}

public class TodoItemEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public bool IsDone { get; set; }

    public int UserId { get; set; }
    public AppUser? User { get; set; }
}
```

### DbContext sa seed-om

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<TodoItemEntity> Todos => Set<TodoItemEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItemEntity>()
            .HasOne(t => t.User)
            .WithMany(u => u.Todos)
            .HasForeignKey(t => t.UserId);
    }

    public void SeedIfEmpty()
    {
        if (Users.Any()) return;

        var u = new AppUser { Email = "seed@local" };
        Users.Add(u);
        SaveChanges();

        Todos.AddRange(
            new TodoItemEntity { Title = "Jedan", IsDone = false, UserId = u.Id },
            new TodoItemEntity { Title = "Dva", IsDone = true, UserId = u.Id },
            new TodoItemEntity { Title = "Tri", IsDone = false, UserId = u.Id }
        );
        SaveChanges();
    }
}
```

### Poziv seed-a pri startu (jednostavno, za vežbu)

```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    db.SeedIfEmpty();
}
```

**Zadatak (Lekcija 17):** Migracija; u bazi tri todo-a sa istim `UserId`.

---

## Lekcija 18 — Filtriranje i paginacija

```csharp
[HttpGet("paged")]
public async Task<ActionResult<object>> GetPaged(
    [FromQuery] bool? done,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    CancellationToken ct = default)
{
    const int maxPageSize = 100;
    pageSize = Math.Clamp(pageSize, 1, maxPageSize);
    page = Math.Max(page, 1);

    var query = _db.Todos.AsNoTracking().AsQueryable();
    if (done.HasValue)
        query = query.Where(t => t.IsDone == done.Value);

    var total = await query.CountAsync(ct);
    var items = await query
        .OrderBy(t => t.Id)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(ct);

    return Ok(new { total, page, pageSize, items });
}
```

**Zadatak (Lekcija 18):** `?done=false`, `?page=2&pageSize=5` u Swaggeru.

---

## Lekcija 19 — Autentifikacija vs autorizacija

Tekstualno: autentifikacija = **ko si**; autorizacija = **šta smeš**. Lozinka se čuva kao **heš**.

**Zadatak (Lekcija 19):** 3–5 rečenica svojim rečima.

---

## Lekcija 20 — JWT tok (bez koda)

Koraci: login POST → provera → JWT u odgovoru → Bearer na zahtevima.

**Zadatak (Lekcija 20):** Napiši korake tekstualno.

---

## Lekcija 21 — JWT u ASP.NET Core (kompletan primer)

### Paketi

```powershell
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Microsoft.Extensions.Identity.Core
```

*`PasswordHasher<TUser>` dolazi iz `Microsoft.Extensions.Identity.Core`. Paket `Microsoft.AspNetCore.Identity.EntityFrameworkCore` treba ako koristiš pun Identity sa tabelama — u primeru ispod je samo tvoj `AppUser` u postojećem `DbContext`-u.*

### `appsettings.json` (Development — ključ mora biti dovoljno dug)

```json
{
  "Jwt": {
    "Issuer": "MojTodoApi",
    "Audience": "MojTodoClient",
    "Key": "OVO_JE_SAMO_ZA_VEZBU_MINIMALNO_32_KARAKTERA_DUGO!!!!"
  }
}
```

### Opcija: User entitet sa hešom

```csharp
namespace MojWebApi.Models;

public class AppUser
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
}
```

### `Program.cs` — JWT Bearer

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var jwtSection = builder.Configuration.GetSection("Jwt");
var key = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key");
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

### Servis za izdavanje tokena

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MojWebApi.Services;

public class JwtTokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config) => _config = config;

    public string CreateToken(int userId, string email)
    {
        var section = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(section["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: section["Issuer"],
            audience: section["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

Registracija: `builder.Services.AddSingleton<JwtTokenService>();`

### Auth kontroler (pojednostavljeno — u produkciji koristi Identity)

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MojWebApi.Data;
using MojWebApi.Models;
using MojWebApi.Services;

namespace MojWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtTokenService _tokens;
    private readonly PasswordHasher<AppUser> _hasher = new();

    public AuthController(AppDbContext db, JwtTokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    public record RegisterDto(string Email, string Password);
    public record LoginDto(string Email, string Password);

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDto dto)
    {
        if (_db.Users.Any(u => u.Email == dto.Email))
            return Conflict(new { message = "Email zauzet" });

        var user = new AppUser { Email = dto.Email };
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);
        _db.Users.Add(user);
        _db.SaveChanges();

        var token = _tokens.CreateToken(user.Id, user.Email);
        return Ok(new { token });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var user = _db.Users.FirstOrDefault(u => u.Email == dto.Email);
        if (user is null)
            return Unauthorized(new { message = "Pogrešan email ili lozinka" });

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = "Pogrešan email ili lozinka" });

        var token = _tokens.CreateToken(user.Id, user.Email);
        return Ok(new { token });
    }
}
```

### Zaštićen kontroler — čitanje `userId` iz tokena

```csharp
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MojWebApi.Data;
using MojWebApi.Models;
using System.IdentityModel.Tokens.Jwt;

namespace MojWebApi.Controllers;

[ApiController]
[Route("api/my-todos")]
[Authorize]
public class MyTodosController : ControllerBase
{
    private readonly AppDbContext _db;

    public MyTodosController(AppDbContext db) => _db = db;

    private int GetUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return int.Parse(sub!);
    }

    [HttpGet]
    public async Task<ActionResult<List<TodoItemEntity>>> Mine(CancellationToken ct)
    {
        var userId = GetUserId();
        return await _db.Todos.AsNoTracking()
            .Where(t => t.UserId == userId)
            .ToListAsync(ct);
    }
}
```

**Zadatak (Lekcija 21):** Dva korisnika; A ne vidi todo od B.

---

## Lekcija 22 — Node.js i npm

```powershell
node -v
npm -v
```

**Zadatak (Lekcija 22):** Zapiši verzije.

---

## Lekcija 23 — TypeScript primer

```typescript
// primer.ts
export interface Todo {
  id: number;
  title: string;
  isDone: boolean;
}

export function formatTodo(t: Todo): string {
  return `[${t.isDone ? "x" : " "}] ${t.id}: ${t.title}`;
}
```

**Zadatak (Lekcija 23):** Sačuvaj `primer.ts`.

---

## Lekcija 24 — React + Vite + `useState`

### `App.tsx` (minimalan brojač)

```tsx
import { useState } from "react";

export default function App() {
  const [count, setCount] = useState(0);

  return (
    <main style={{ padding: "1rem", fontFamily: "system-ui" }}>
      <h1>Moj React</h1>
      <p>Tvoje ime: ___</p>
      <button type="button" onClick={() => setCount((c) => c + 1)}>
        Klikova: {count}
      </button>
    </main>
  );
}
```

**Zadatak (Lekcija 24):** Vite React TS; brojač + tvoje ime.

---

## Lekcija 25 — `fetch` ka API-ju (login + lista)

### `api.ts` — baza URL i helper

```typescript
const API_BASE = import.meta.env.VITE_API_URL ?? "https://localhost:7XXX"; // zameni port

export async function login(email: string, password: string): Promise<string> {
  const res = await fetch(`${API_BASE}/api/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password }),
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error((err as { message?: string }).message ?? `HTTP ${res.status}`);
  }
  const data = (await res.json()) as { token: string };
  return data.token;
}

export async function fetchMyTodos(token: string) {
  const res = await fetch(`${API_BASE}/api/my-todos`, {
    headers: { Authorization: `Bearer ${token}` },
  });
  if (res.status === 401) throw new Error("Nisi prijavljen");
  if (!res.ok) throw new Error(`HTTP ${res.status}`);
  return res.json();
}
```

U Vite projektu fajl `.env.development`:

```env
VITE_API_URL=https://localhost:5287
```

### Komponenta (pojednostavljeno)

```tsx
import { useState } from "react";
import { login, fetchMyTodos } from "./api";

export default function TodoApp() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [token, setToken] = useState<string | null>(
    () => localStorage.getItem("token")
  );
  const [todos, setTodos] = useState<unknown[]>([]);
  const [error, setError] = useState<string | null>(null);

  async function handleLogin(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    try {
      const t = await login(email, password);
      localStorage.setItem("token", t);
      setToken(t);
    } catch (ex) {
      setError(ex instanceof Error ? ex.message : "Greška");
    }
  }

  async function loadTodos() {
    if (!token) return;
    setError(null);
    try {
      const data = await fetchMyTodos(token);
      setTodos(Array.isArray(data) ? data : []);
    } catch (ex) {
      setError(ex instanceof Error ? ex.message : "Greška");
    }
  }

  return (
    <div style={{ padding: "1rem" }}>
      <form onSubmit={handleLogin}>
        <input value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Email" />
        <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
        <button type="submit">Prijava</button>
      </form>
      {token && (
        <button type="button" onClick={loadTodos}>
          Učitaj moje todo-e
        </button>
      )}
      {error && <p style={{ color: "crimson" }}>{error}</p>}
      <pre>{JSON.stringify(todos, null, 2)}</pre>
    </div>
  );
}
```

**Napomena:** `localStorage` za JWT je **rizičan** ako postoji XSS; za učenje je prihvatljivo; u produkciji često **httpOnly cookie** + isti sajt ili BFF.

**Zadatak (Lekcija 25):** Login + lista sa tokenom.

---

## Lekcija 26 — CORS na API-ju

```csharp
var builder = WebApplication.CreateBuilder(args);

var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
                  ?? new[] { "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevFront", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ...

var app = builder.Build();

app.UseCors("DevFront");
app.UseAuthentication();
app.UseAuthorization();
```

`appsettings.Development.json`:

```json
{
  "Cors": {
    "Origins": [ "http://localhost:5173" ]
  }
}
```

**Zadatak (Lekcija 26):** Nema CORS greške u konzoli browsera.

---

## Lekcija 27 — Konfiguracija i environment

### Čitanje sa prioritetom (env nadjačava JSON)

```csharp
var jwtKey = builder.Configuration["Jwt:Key"];
```

Postavi u `launchSettings.json` pod `environmentVariables`:

```json
"Jwt__Key": "tajni_kljuc_iz_env_min_32_karaktera__"
```

*(Dupli underscore `__` u env odgovara `Jwt:Key` u konfiguraciji.)*

**Zadatak (Lekcija 27):** JWT key u Development + rečenica u README o ne-commit-ovanju tajni.

---

## Lekcija 28 — Docker Compose (samo PostgreSQL)

```yaml
# docker-compose.yml
services:
  db:
    image: postgres:16-alpine
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
      POSTGRES_DB: todoapp
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data

volumes:
  pgdata: {}
```

Connection string za API:

```text
Host=localhost;Port=5432;Database=todoapp;Username=postgres;Password=postgres
```

**Zadatak (Lekcija 28):** `docker compose up -d`; API se povezuje na tu bazu; README sa komandama.

---

## Lekcija 29 — xUnit primer

### Test projekat

```powershell
dotnet new xunit -n MojWebApi.Tests
dotnet sln add MojWebApi.Tests/MojWebApi.Tests.csproj
dotnet add MojWebApi.Tests reference MojWebApi/MojWebApi.csproj
```

### Čista funkcija u glavnom projektu (npr. `Paging.cs`)

```csharp
namespace MojWebApi;

public static class Paging
{
    public static int ClampPageSize(int pageSize, int max = 100)
        => Math.Clamp(pageSize, 1, max);

    public static int NormalizePage(int page) => Math.Max(page, 1);
}
```

### Test

```csharp
using MojWebApi;

namespace MojWebApi.Tests;

public class PagingTests
{
    [Fact]
    public void ClampPageSize_ne_prelazi_max()
    {
        Assert.Equal(100, Paging.ClampPageSize(500));
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    public void NormalizePage_minimum_je_1(int input, int expected)
    {
        Assert.Equal(expected, Paging.NormalizePage(input));
    }
}
```

```powershell
dotnet test
```

**Zadatak (Lekcija 29):** Najmanje 3 testa; `dotnet test` prolazi.

---

## Lekcija 30 — Portfolio

**Zadatak (Lekcija 30):** GitHub + README sa koracima; mejl sebi sa linkom.

---

## Dodatak — Mapa pojmova

| Pojam | Kratak smisao |
|--------|----------------|
| HTTP | Zahtev–odgovor između klijenta i servera |
| JSON | Tekstualni format podataka |
| API | Ugovor URL-ova i pravila |
| ASP.NET Core | .NET web framework |
| EF Core | ORM |
| JWT | Potpisani token |
| React | UI komponente |
| TypeScript | JS + tipovi |
| CORS | Cross-origin pravila u browseru |
| Git | Verzionisanje |

---

## Dodatak — Česte greške

- **CORS** — proveri tačan origin (port), red middleware-a (`UseCors` pre `UseAuthorization` gde treba).
- **401** — nedostaje `Authorization: Bearer`, istekao token, pogrešan ključ issuer/audience.
- **404** — pogrešna ruta ili `[Route]` prefiks.
- **EF** — zaboravljen `SaveChangesAsync`; migracija nije primenjena na bazu koju API koristi.

---

*Jedna lekcija = jedan zadatak. Primeri koda su ilustrativni; prilagodi namespace, portove i imena projekata svom rešenju.*
