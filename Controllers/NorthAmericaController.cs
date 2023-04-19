using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace NorthAmericaApi.Controllers;

[ApiController]
[Route("[controller]")]
public class NorthAmericaController : ControllerBase
{
    private readonly ILogger<NorthAmericaController> _logger;
    private readonly IMemoryCache _memoryCache;

    public NorthAmericaController(ILogger<NorthAmericaController> logger, IMemoryCache memoryCache)
    {
        _logger = logger;
        _memoryCache = memoryCache;
    }

    private Country[] GetCountriesFromJsonFile()
    {
        string countriesJsonString = System.IO.File.ReadAllText("data/countries.json");
        Country[] Countries = JsonSerializer.Deserialize<Country[]>(countriesJsonString)!;
        return Countries;
    }

    private Country[] GetOrSetCachedCountries()
    {
        var cachedCountries = _memoryCache.GetOrCreate<Country[]>("Countries", entry =>
            {
                entry.AbsoluteExpiration = DateTime.Now.AddHours(1);
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                return this.GetCountriesFromJsonFile();
            });

        return cachedCountries!;
    }

    [HttpGet("AllCountries")]
    public IEnumerable<Country> GetAllCountriesStatesProvinces()
    {
        _logger.LogInformation($"[Request] Get all countries");

        return GetOrSetCachedCountries();
    }

    [HttpGet("CountryName/{countryName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Country> GetByCountryName(string countryName)
    {
        _logger.LogInformation($"[Request] Find by country name: {countryName}");

        IEnumerable<Country> matchedCountries = GetOrSetCachedCountries()
                                .Where((country => country.Name.ToLower() == countryName.ToLower()));

        return matchedCountries.Count() > 0 ? matchedCountries.First() : NotFound();
    }

    [HttpGet("CountryAbbreviation/{countryAbbreviation}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Country> GetByCountryAbbreviation(string countryAbbreviation)
    {
        _logger.LogInformation($"[Request] Find by country abbreviation: {countryAbbreviation}");

        IEnumerable<Country> matchedCountries = GetOrSetCachedCountries()
                                .Where((country => country.Abbreviation.ToLower() == countryAbbreviation.ToLower()));

        return matchedCountries.Count() > 0 ? matchedCountries.First() : NotFound();
    }

    [HttpGet("StateProvinceName/{stateProvinceName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<StateProvince> GetByStateProvinceName(string stateProvinceName)
    {
        _logger.LogInformation($"[Request] Find by State or Province Name: {stateProvinceName}");

        IEnumerable<StateProvince> matchedStateProvince = GetOrSetCachedCountries()
                                                            .SelectMany(countries => countries.StatesProvinces!
                                                            .Where(stateProvince => stateProvince.Name.ToLower() == stateProvinceName.ToLower()));

        return matchedStateProvince.Count() > 0 ? matchedStateProvince.First() : NotFound();
    }

    [HttpGet("StateProvinceAbbreviation/{stateProvinceAbbreviation}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<StateProvince> GetByStateProvinceAbbreviation(string stateProvinceAbbreviation)
    {
        _logger.LogInformation($"[Request] Find by State or Province Abbreviation: {stateProvinceAbbreviation}");

        IEnumerable<StateProvince> matchedStateProvince = GetOrSetCachedCountries()
                                                            .SelectMany(countries => countries.StatesProvinces!
                                                            .Where(stateProvince => stateProvince.Abbreviation.ToLower() == stateProvinceAbbreviation.ToLower()));

        return matchedStateProvince.Count() > 0 ? matchedStateProvince.First() : NotFound();
    }
}
