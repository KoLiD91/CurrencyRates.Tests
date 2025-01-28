using CurrencyRates.Data;
using CurrencyRates.Models;
using Microsoft.EntityFrameworkCore;
using TechTalk.SpecFlow;

[Binding]
public class WarstwaBazodanowaSteps
{
    private readonly ApplicationDbContext _context;
    private ExchangeRate _nowyKurs;
    private ExchangeRate _odczytanyKurs;
    private bool _operacjaZakonczylaSieNiepowodzeniem;

    public WarstwaBazodanowaSteps()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=currency_rates_test;Username=postgres;Password=password")
            .EnableSensitiveDataLogging()
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();
    }

    [Given(@"nowy kurs waluty ""(.*)""")]
    public void MajacNowyKursWaluty(string kodWaluty)
    {
        _nowyKurs = new ExchangeRate
        {
            CurrencyCode = kodWaluty,
            TableType = "A"
        };
    }

    [Given(@"wartość kursu (.*)")]
    public void MajacWartoscKursu(string wartoscKursuText)
    {
        var wartoscKursu = decimal.Parse(
            wartoscKursuText.Replace(".", ","),
            System.Globalization.CultureInfo.GetCultureInfo("pl-PL")
        );
        _nowyKurs.Rate = wartoscKursu;
    }

    [Given(@"datę kursu ""(.*)""")]
    public void MajacDateKursu(string data)
    {
        _nowyKurs.Date = DateTime.Parse(data).ToUniversalTime();
        _nowyKurs.FetchDate = DateTime.UtcNow;
    }

    [When(@"zapisuję kurs w bazie danych")]
    public async Task KiedyZapisujeKursWBazieDanych()
    {
        await _context.ExchangeRates.AddAsync(_nowyKurs);
        await _context.SaveChangesAsync();
    }

    [Then(@"kurs zostaje poprawnie zapisany")]
    public async Task WtedyKursZostajePoprawnieZapisany()
    {
        _odczytanyKurs = await _context.ExchangeRates
            .FirstOrDefaultAsync(r =>
                r.CurrencyCode == _nowyKurs.CurrencyCode &&
                r.Date.Date == _nowyKurs.Date.Date);

        Assert.NotNull(_odczytanyKurs);
    }

    [Then(@"mogę odczytać ten sam kurs z bazy")]
    public void WtedyMogeOdczytacTenSamKursZBazy()
    {
        Assert.Equal(_nowyKurs.CurrencyCode, _odczytanyKurs.CurrencyCode);
        Assert.Equal(_nowyKurs.Rate, _odczytanyKurs.Rate);
        Assert.Equal(_nowyKurs.Date, _odczytanyKurs.Date);
        Assert.Equal(_nowyKurs.TableType, _odczytanyKurs.TableType);
    }

    [Given(@"istniejący kurs waluty ""(.*)"" z dnia ""(.*)""")]
    public async Task MajacIstniejacyKursWalutyZDnia(string kodWaluty, string data)
    {
        // Czyścimy bazę przed testem
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"ExchangeRates\" RESTART IDENTITY");

        // Prawidłowo parsujemy datę z uwzględnieniem strefy czasowej
        var dataKursu = DateTime.Parse(data);
        // Konwertujemy na UTC, co jest wymagane przez PostgreSQL
        var dataUtc = DateTime.SpecifyKind(dataKursu, DateTimeKind.Utc);

        _nowyKurs = new ExchangeRate
        {
            CurrencyCode = kodWaluty,
            Rate = 4.2182m,
            Date = dataUtc, // Używamy przekonwertowanej daty UTC
            TableType = "A",
            FetchDate = DateTime.UtcNow
        };

        await _context.ExchangeRates.AddAsync(_nowyKurs);
        await _context.SaveChangesAsync();

        // Weryfikujemy zapis
        var zapisanyKurs = await _context.ExchangeRates
            .FirstOrDefaultAsync(x =>
                x.CurrencyCode == kodWaluty &&
                x.Date.Date == dataUtc.Date);
        Assert.NotNull(zapisanyKurs);
    }

    [When(@"próbuję zapisać ten sam kurs ponownie")]
    public async Task KiedyProbujeZapisacTenSamKursPonownie()
    {
        var duplikat = new ExchangeRate
        {
            CurrencyCode = _nowyKurs.CurrencyCode,
            Rate = _nowyKurs.Rate,
            Date = _nowyKurs.Date, // Ta data jest już w UTC
            TableType = _nowyKurs.TableType,
            FetchDate = DateTime.UtcNow
        };

        try
        {
            await _context.ExchangeRates.AddAsync(duplikat);
            await _context.SaveChangesAsync();
            _operacjaZakonczylaSieNiepowodzeniem = false;
        }
        catch (DbUpdateException)
        {
            _operacjaZakonczylaSieNiepowodzeniem = true;
        }
    }

    [Then(@"operacja kończy się niepowodzeniem")]
    public void WtedyOperacjaKonczySieNiepowodzeniem()
    {
        Assert.True(_operacjaZakonczylaSieNiepowodzeniem,
            "Oczekiwano niepowodzenia operacji ze względu na naruszenie ograniczenia unikalności");

        // Sprawdzamy liczbę wpisów w bazie
        var liczbaWpisow = _context.ExchangeRates
            .Count(r =>
                r.CurrencyCode == _nowyKurs.CurrencyCode &&
                r.Date.Date == _nowyKurs.Date.Date);
        Assert.Equal(1, liczbaWpisow);
    }

}