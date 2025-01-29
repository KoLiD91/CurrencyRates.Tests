using System.Text.Json;
using TechTalk.SpecFlow;
using Xunit;

[Binding]
public class DzienneKursyWalutSteps
{
    private readonly HttpClient _client;
    private string _kodWaluty;
    private string _data;
    private HttpResponseMessage _odpowiedz;
    private JsonDocument _zawartoscOdpowiedzi;

    public DzienneKursyWalutSteps()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7166/api/")
        };
    }

    [Given(@"wybraną walutę ""(.*)""")]
    public void MajacWybranaWalute(string kodWaluty)
    {
        _kodWaluty = kodWaluty.ToLower();
    }

    [Given(@"wybraną datę ""(.*)""")]
    public void MajacWybranaDate(string data)
    {
        _data = data;
    }

    [When(@"sprawdzam historyczny kurs dzienny")]
    public async Task KiedySprawdzamHistorycznyKursDzienny()
    {
        _odpowiedz = await _client.GetAsync($"CurrencyHistory/history/{_kodWaluty}/daily/{_data}");

        if (_odpowiedz.IsSuccessStatusCode)
        {
            var content = await _odpowiedz.Content.ReadAsStringAsync();
            _zawartoscOdpowiedzi = JsonDocument.Parse(content);
        }
    }

    [Then(@"system zwraca poprawną odpowiedź")]
    public void WtedySystemZwracaPoprawnaOdpowiedz()
    {
        Assert.True(_odpowiedz.IsSuccessStatusCode);
    }

    [Then(@"odpowiedź zawiera prawidłowe dane kursu historycznego")]
    public void WtedyOdpowiedzZawieraPrawidloweDaneKursuHistorycznego()
    {
        var root = _zawartoscOdpowiedzi.RootElement;
        var rate = root.GetProperty("rate");

        Assert.Equal(_kodWaluty.ToUpper(), root.GetProperty("currency").GetString());
        Assert.Equal(_kodWaluty.ToUpper(), rate.GetProperty("currencyCode").GetString());
        Assert.Equal("A", rate.GetProperty("tableType").GetString());
    }

    [Then(@"wartość kursu jest większa od (.*)")]
    public void WtedyWartoscKursuJestWiekszaOd(decimal minimalnaWartosc)
    {
        var kurs = _zawartoscOdpowiedzi.RootElement
            .GetProperty("rate")
            .GetProperty("rate")
            .GetDecimal();

        Assert.True(kurs > minimalnaWartosc);
    }

    [Then(@"system zwraca błąd nieprawidłowego żądania")]
    public void WtedySystemZwracaBladNieprawidlowegoZadania()
    {
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, _odpowiedz.StatusCode);
    }
}