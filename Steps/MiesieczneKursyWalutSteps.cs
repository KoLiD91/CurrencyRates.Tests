using System.Text.Json;
using TechTalk.SpecFlow;
using Xunit;

[Binding]
public class MiesieczneKursyWalutSteps
{
    private readonly HttpClient _client;
    private string _kodWaluty;
    private string _rok;
    private string _miesiac;
    private HttpResponseMessage _odpowiedz;
    private JsonDocument _zawartoscOdpowiedzi;

    public MiesieczneKursyWalutSteps()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7166/api/")
        };
    }

    [Given(@"określoną walutę ""(.*)""")]
    public void MajacOkreslonWalute(string kodWaluty)
    {
        _kodWaluty = kodWaluty.ToLower();
    }

    [Given(@"określony rok ""(.*)""")]
    public void MajacOkreslonyRok(string rok)
    {
        _rok = rok;
    }

    [Given(@"określony miesiąc ""(.*)""")]
    public void MajacOkreslonyMiesiac(string miesiac)
    {
        _miesiac = miesiac;
    }

    [When(@"pobieram kursy miesięczne")]
    public async Task KiedyPobieramKursyMiesieczne()
    {
        _odpowiedz = await _client.GetAsync($"CurrencyHistory/history/{_kodWaluty}/monthly/{_rok}/{_miesiac}");

        if (_odpowiedz.IsSuccessStatusCode)
        {
            var content = await _odpowiedz.Content.ReadAsStringAsync();
            _zawartoscOdpowiedzi = JsonDocument.Parse(content);
        }
    }

    [Then(@"odpowiedź z kursami miesięcznymi jest poprawna")]
    public void WtedyOdpowiedzZKursamiMiesiecznymiJestPoprawna()
    {
        Assert.True(_odpowiedz.IsSuccessStatusCode);
    }

    [Then(@"lista kursów miesięcznych zawiera prawidłowe dane")]
    public void WtedyListaKursowMiesiecznychZawieraPrawidloweDane()
    {
        var root = _zawartoscOdpowiedzi.RootElement;

        Assert.Equal(_kodWaluty.ToUpper(), root.GetProperty("currency").GetString());
        Assert.Equal(int.Parse(_rok), root.GetProperty("year").GetInt32());
        Assert.Equal(int.Parse(_miesiac), root.GetProperty("month").GetInt32());

        var rates = root.GetProperty("rates").EnumerateArray();
        Assert.True(rates.Any(), "Lista kursów nie może być pusta");

        foreach (var rate in rates)
        {
            Assert.Equal(_kodWaluty.ToUpper(), rate.GetProperty("currencyCode").GetString());
            Assert.Equal("A", rate.GetProperty("tableType").GetString());
        }
    }

    [Then(@"każdy kurs w liście jest większy od (.*)")]
    public void WtedyKazdyKursWLiscieJestWiekszyOd(decimal minimalnaWartosc)
    {
        var rates = _zawartoscOdpowiedzi.RootElement.GetProperty("rates").EnumerateArray();

        foreach (var rate in rates)
        {
            var wartoscKursu = rate.GetProperty("rate").GetDecimal();
            Assert.True(wartoscKursu > minimalnaWartosc,
                $"Kurs {wartoscKursu} powinien być większy od {minimalnaWartosc}");
        }
    }

    [Then(@"system zwraca błąd niepoprawnego miesiąca")]
    public void WtedySystemZwracaBladNiepoprawnegoMiesiaca()
    {
        Assert.Equal(System.Net.HttpStatusCode.InternalServerError, _odpowiedz.StatusCode);
    }
}