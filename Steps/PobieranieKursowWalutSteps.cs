using System.Text.Json;
using TechTalk.SpecFlow;
using Xunit;

[Binding]
public class PobieranieKursowWalutSteps
{
    private readonly HttpClient _client;
    private string _kodWaluty;
    private HttpResponseMessage _odpowiedz;
    private JsonDocument _zawartoscOdpowiedzi;

    public PobieranieKursowWalutSteps()
    {
        // Inicjalizujemy klienta HTTP z bazowym adresem API
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7166/api/")
        };
    }

    [Given(@"kod waluty ""(.*)""")]
    public void MajacKodWaluty(string kodWaluty)
    {
        // Zapisujemy kod waluty do wykorzystania w następnych krokach
        _kodWaluty = kodWaluty;
    }

    [When(@"wysyłam zapytanie o aktualny kurs")]
    public async Task WysylamZapytanieOAktualnyKurs()
    {
        // Wysyłamy zapytanie do odpowiedniego endpointu
        _odpowiedz = await _client.GetAsync($"Currency/rate/{_kodWaluty}");

        if (_odpowiedz.IsSuccessStatusCode)
        {
            var content = await _odpowiedz.Content.ReadAsStringAsync();
            _zawartoscOdpowiedzi = JsonDocument.Parse(content);
        }
    }

    [Then(@"odpowiedź powinna być udana")]
    public void OdpowiedzPowinnaBycUdana()
    {
        Assert.True(_odpowiedz.IsSuccessStatusCode);
    }

    [Then(@"odpowiedź powinna zawierać prawidłowe dane kursu")]
    public void OdpowiedzPowinnaZawieracPrawidloweDaneKursu()
    {
        // Sprawdzamy czy mamy odpowiedź i czy zawiera wymagane pola
        Assert.NotNull(_zawartoscOdpowiedzi);
        var root = _zawartoscOdpowiedzi.RootElement;

        // Sprawdzamy najważniejsze pola odpowiedzi
        Assert.Equal(_kodWaluty, root.GetProperty("currency").GetString());

        // Sprawdzamy czy kurs jest wartością numeryczną większą od 0
        var kurs = root.GetProperty("rateAgainstPLN").GetDecimal();
        Assert.True(kurs > 0);
    }

    [Then(@"system zwraca błąd serwera")]
    public void WtedySystemZwracaBladSerwera()
    {
        Assert.Equal(System.Net.HttpStatusCode.InternalServerError, _odpowiedz.StatusCode);
    }
}