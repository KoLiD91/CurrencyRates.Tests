using System.Text.Json;
using TechTalk.SpecFlow;
using Xunit;

[Binding]
public class DostepneWalutySteps
{
    private readonly HttpClient _client;
    private HttpResponseMessage _odpowiedz;
    private JsonDocument _zawartoscOdpowiedzi;

    public DostepneWalutySteps()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7166/api/")
        };
    }

    [When(@"pobieram listę dostępnych walut")]
    public async Task KiedyPobieramListeDostepnychWalut()
    {
        _odpowiedz = await _client.GetAsync("Currency/available");

        if (_odpowiedz.IsSuccessStatusCode)
        {
            var content = await _odpowiedz.Content.ReadAsStringAsync();
            _zawartoscOdpowiedzi = JsonDocument.Parse(content);
        }
    }

    [Then(@"odpowiedź z listą walut jest udana")]
    public void WtedyOdpowiedzZListaWalutJestUdana()
    {
        Assert.True(_odpowiedz.IsSuccessStatusCode);
    }

    [Then(@"lista zawiera podstawowe waluty")]
    public void WtedyListaZawieraPodstawoweWaluty()
    {
        var waluty = _zawartoscOdpowiedzi.RootElement.EnumerateArray();
        var kodyWalut = waluty.Select(w => w.GetProperty("code").GetString()).ToList();

        var podstawoweWaluty = new[] { "USD", "EUR", "GBP", "CHF" };
        foreach (var waluta in podstawoweWaluty)
        {
            Assert.Contains(waluta, kodyWalut);
        }
    }

    [Then(@"każda waluta ma wymagane pola")]
    public void WtedyKazdaWalutaMaWymaganePola()
    {
        var waluty = _zawartoscOdpowiedzi.RootElement.EnumerateArray();

        foreach (var waluta in waluty)
        {
            // Sprawdzamy czy wszystkie wymagane pola istnieją
            Assert.True(waluta.TryGetProperty("code", out var code), "Brak pola 'code'");
            Assert.True(waluta.TryGetProperty("name", out var name), "Brak pola 'name'");
            Assert.True(waluta.TryGetProperty("tableType", out var tableType), "Brak pola 'tableType'");

            // Sprawdzamy czy pola nie są puste
            Assert.False(string.IsNullOrWhiteSpace(code.GetString()), "Pole 'code' jest puste");
            Assert.False(string.IsNullOrWhiteSpace(name.GetString()), "Pole 'name' jest puste");
            Assert.False(string.IsNullOrWhiteSpace(tableType.GetString()), "Pole 'tableType' jest puste");
        }
    }

    [Then(@"lista zawiera walutę ""(.*)"" o nazwie ""(.*)""")]
    public void WtedyListaZawieraWaluteONazwie(string kod, string nazwa)
    {
        var waluty = _zawartoscOdpowiedzi.RootElement.EnumerateArray();
        var znalezionaWaluta = false;

        foreach (var waluta in waluty)
        {
            if (waluta.GetProperty("code").GetString() == kod)
            {
                Assert.Equal(nazwa, waluta.GetProperty("name").GetString());
                znalezionaWaluta = true;
                break;
            }
        }

        Assert.True(znalezionaWaluta, $"Nie znaleziono waluty o kodzie {kod}");
    }

    [Then(@"wszystkie waluty mają typ tabeli ""(.*)""")]
    public void WtedyWszystkieWalutyMajaTypTabeli(string typTabeli)
    {
        var waluty = _zawartoscOdpowiedzi.RootElement.EnumerateArray();

        foreach (var waluta in waluty)
        {
            Assert.Equal(typTabeli, waluta.GetProperty("tableType").GetString());
        }
    }
}