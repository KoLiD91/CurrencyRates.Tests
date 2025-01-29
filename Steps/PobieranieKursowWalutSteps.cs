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
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7166/api/")
        };
    }

    [Given(@"kod waluty ""(.*)""")]
    public void MajacKodWaluty(string kodWaluty)
    {
        _kodWaluty = kodWaluty;
    }

    [When(@"wysyłam zapytanie o aktualny kurs")]
    public async Task WysylamZapytanieOAktualnyKurs()
    {
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
        Assert.NotNull(_zawartoscOdpowiedzi);
        var root = _zawartoscOdpowiedzi.RootElement;

        Assert.Equal(_kodWaluty, root.GetProperty("currency").GetString());

        var kurs = root.GetProperty("rateAgainstPLN").GetDecimal();
        Assert.True(kurs > 0);
    }

    [Then(@"system zwraca błąd serwera")]
    public void WtedySystemZwracaBladSerwera()
    {
        Assert.Equal(System.Net.HttpStatusCode.InternalServerError, _odpowiedz.StatusCode);
    }
}