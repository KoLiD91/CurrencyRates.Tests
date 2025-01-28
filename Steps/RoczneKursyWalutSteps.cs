using System.Text.Json;
using TechTalk.SpecFlow;
using Xunit;

[Binding]
public class RoczneKursyWalutSteps
{
    private readonly HttpClient _client;
    private string _kodWaluty;
    private string _rok;
    private HttpResponseMessage _odpowiedz;
    private JsonDocument _zawartoscOdpowiedzi;

    public RoczneKursyWalutSteps()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7166/api/")
        };
    }

    [Given(@"wskazaną walutę ""(.*)""")]
    public void MajacWskazanaWalute(string kodWaluty)
    {
        _kodWaluty = kodWaluty.ToLower();
    }

    [Given(@"wskazany rok ""(.*)""")]
    public void MajacWskazanyRok(string rok)
    {
        _rok = rok;
    }

    [When(@"pobieram kursy roczne")]
    public async Task KiedyPobieramKursyRoczne()
    {
        _odpowiedz = await _client.GetAsync($"CurrencyHistory/history/{_kodWaluty}/yearly/{_rok}");

        if (_odpowiedz.IsSuccessStatusCode)
        {
            var content = await _odpowiedz.Content.ReadAsStringAsync();
            _zawartoscOdpowiedzi = JsonDocument.Parse(content);
        }
    }

    [Then(@"odpowiedź z kursami rocznymi jest udana")]
    public void WtedyOdpowiedzZKursamiRocznymiJestUdana()
    {
        Assert.True(_odpowiedz.IsSuccessStatusCode);
    }

    [Then(@"lista kursów rocznych jest prawidłowa")]
    public void WtedyListaKursowRocznychJestPrawidlowa()
    {
        var root = _zawartoscOdpowiedzi.RootElement;

        // Sprawdzamy główne pola odpowiedzi
        Assert.Equal(_kodWaluty.ToUpper(), root.GetProperty("currency").GetString());
        Assert.Equal(int.Parse(_rok), root.GetProperty("year").GetInt32());

        // Sprawdzamy listę kursów
        var rates = root.GetProperty("rates");
        Assert.True(rates.GetArrayLength() > 0, "Lista kursów nie może być pusta");

        // Sprawdzamy każdy element listy
        foreach (JsonElement rate in rates.EnumerateArray())
        {
            // Sprawdzamy czy kod waluty jest poprawny
            Assert.Equal(_kodWaluty.ToUpper(), rate.GetProperty("currencyCode").GetString());

            // Sprawdzamy typ tabeli
            Assert.Equal("A", rate.GetProperty("tableType").GetString());

            // Sprawdzamy rok w dacie kursu
            var dataKursuText = rate.GetProperty("date").GetString();
            var dataKursu = DateTime.Parse(dataKursuText);
            var rokKursu = dataKursu.Year;
            var oczekiwanyRok = int.Parse(_rok);

            Assert.True(oczekiwanyRok == rokKursu,
                $"Data kursu ({dataKursuText}) powinna być z roku {oczekiwanyRok}");
        }
    }

    [Then(@"wszystkie kursy roczne są większe od (.*)")]
    public void WtedyWszystkieKursyRoczneSaWiekszeOd(decimal minimalnaWartosc)
    {
        var rates = _zawartoscOdpowiedzi.RootElement.GetProperty("rates").EnumerateArray();

        foreach (var rate in rates)
        {
            var wartoscKursu = rate.GetProperty("rate").GetDecimal();
            Assert.True(wartoscKursu > minimalnaWartosc,
                $"Kurs {wartoscKursu} powinien być większy od {minimalnaWartosc}");
        }
    }

    [Then(@"system zwraca błąd nieprawidłowego roku")]
    public void WtedySystemZwracaBladNieprawidlowegoRoku()
    {
        Assert.Equal(System.Net.HttpStatusCode.InternalServerError, _odpowiedz.StatusCode);
    }
}