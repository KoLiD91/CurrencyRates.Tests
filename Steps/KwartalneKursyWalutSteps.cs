using System.Text.Json;
using TechTalk.SpecFlow;
using Xunit;

[Binding]
public class KwartalneKursyWalutSteps
{
    private readonly HttpClient _client;
    private string _kodWaluty;
    private string _rok;
    private string _kwartal;
    private HttpResponseMessage _odpowiedz;
    private JsonDocument _zawartoscOdpowiedzi;

    public KwartalneKursyWalutSteps()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7166/api/")
        };
    }

    [Given(@"zdefiniowaną walutę ""(.*)""")]
    public void MajacZdefiniowanaWalute(string kodWaluty)
    {
        _kodWaluty = kodWaluty.ToLower();
    }

    [Given(@"zdefiniowany rok ""(.*)""")]
    public void MajacZdefiniowanyRok(string rok)
    {
        _rok = rok;
    }

    [Given(@"zdefiniowany kwartał ""(.*)""")]
    public void MajacZdefiniowanyKwartal(string kwartal)
    {
        _kwartal = kwartal;
    }

    [When(@"pobieram kursy kwartalne")]
    public async Task KiedyPobieramKursyKwartalne()
    {
        _odpowiedz = await _client.GetAsync($"CurrencyHistory/history/{_kodWaluty}/quarterly/{_rok}/{_kwartal}");

        if (_odpowiedz.IsSuccessStatusCode)
        {
            var content = await _odpowiedz.Content.ReadAsStringAsync();
            _zawartoscOdpowiedzi = JsonDocument.Parse(content);
        }
    }

    [Then(@"odpowiedź z kursami kwartalnymi jest udana")]
    public void WtedyOdpowiedzZKursamiKwartalnymiJestUdana()
    {
        Assert.True(_odpowiedz.IsSuccessStatusCode);
    }

    [Then(@"lista kursów kwartalnych jest prawidłowa")]
    public void WtedyListaKursowKwartalnychJestPrawidlowa()
    {
        var root = _zawartoscOdpowiedzi.RootElement;

        Assert.Equal(_kodWaluty.ToUpper(), root.GetProperty("currency").GetString());
        Assert.Equal(int.Parse(_rok), root.GetProperty("year").GetInt32());
        Assert.Equal(int.Parse(_kwartal), root.GetProperty("quarter").GetInt32());

        var rates = root.GetProperty("rates").EnumerateArray();
        Assert.True(rates.Any(), "Lista kursów nie może być pusta");

        foreach (var rate in rates)
        {
            Assert.Equal(_kodWaluty.ToUpper(), rate.GetProperty("currencyCode").GetString());
            Assert.Equal("A", rate.GetProperty("tableType").GetString());
            Assert.True(DateTime.Parse(rate.GetProperty("date").GetString()).Year == int.Parse(_rok),
                "Rok w kursie musi odpowiadać zadanemu rokowi");
        }
    }

    [Then(@"wszystkie kursy kwartalne są większe od (.*)")]
    public void WtedyWszystkieKursyKwartalneSaWiekszeOd(decimal minimalnaWartosc)
    {
        var rates = _zawartoscOdpowiedzi.RootElement.GetProperty("rates").EnumerateArray();

        foreach (var rate in rates)
        {
            var wartoscKursu = rate.GetProperty("rate").GetDecimal();
            Assert.True(wartoscKursu > minimalnaWartosc,
                $"Kurs {wartoscKursu} powinien być większy od {minimalnaWartosc}");
        }
    }

    [Then(@"system zwraca błąd nieprawidłowego kwartału")]
    public void WtedySystemZwracaBladNieprawidlowegoKwartalu()
    {
        Assert.Equal(System.Net.HttpStatusCode.InternalServerError, _odpowiedz.StatusCode);
    }
}