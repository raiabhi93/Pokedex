using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokeApiNet;
using System.Net.Http;
using RestSharp;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Collections;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Configuration;
using PokemonAPI.Extensions;

namespace PokemonAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly ILogger<PokemonController> _logger;
        private readonly IConfiguration _config;
        public PokemonController(ILogger<PokemonController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        [HttpGet("{name}")]
        //[Route("~/pokemon/")]
        public Array Get(string name)
        {
            try
            {
                if(!(name is null) || name != null)
                {
                    string pokeAPIUrl = _config[Constants.PokeAPIURL].ToString();
                    string url = pokeAPIUrl + name;
                    var client = new RestClient(url);
                    var request = new RestRequest(Method.GET);
                    IRestResponse response = client.Execute(request);
                    var pokemonJsonString = response.Content;
                    var status = response.StatusCode.ToString();
                    PokemonResult[] array = Enumerable.Empty<PokemonResult>().ToArray();
                    if (status == "OK")
                    {
                        var jsonResult = JsonConvert.DeserializeObject<PokemonSpecies>(pokemonJsonString);
                        return Enumerable.Range(1, 1).Select(index => new PokemonResult
                        {
                            Name = jsonResult.Name,
                            Description = Regex.Replace(jsonResult.FlavorTextEntries[0].FlavorText, @"\t|\n|\r|\f", " "),
                            Habitat = jsonResult.Habitat.Name,
                            IsLegendary = jsonResult.IsLegendary
                        })
                        .ToArray();
                    }
                    else
                    {
                        string[] errorList = { "Error connecting with the API. Request failed with Status Code: " + status };
                        return errorList;
                    }
                }
                else
                {
                    string[] errorList = { "Please provide pokemon name!!" };
                    return errorList;
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[HttpGet("{name}")]
        [Route("~/pokemon/translated")]
        [Route("~/pokemon/translated/{name}")]
        public Array Translated(string name)
        {
            try
            {
                if (!(name is null) || name != null)
                {
                    string pokeAPIUrl = _config[Constants.PokeAPIURL].ToString();
                    string url = pokeAPIUrl + name;
                    var client = new RestClient(url);

                    var request = new RestRequest(Method.GET);
                    IRestResponse response = client.Execute(request);
                    var pokemonJsonString = response.Content;
                    var status = response.StatusCode.ToString();

                    if (status == "OK")
                    {
                        var jsonResult = JsonConvert.DeserializeObject<PokemonSpecies>(pokemonJsonString);

                        jsonResult.FlavorTextEntries[0].FlavorText = Regex.Replace(jsonResult.FlavorTextEntries[0].FlavorText, @"\t|\n|\r|\f", " ");

                        if (jsonResult.Habitat.Name == "cave" || jsonResult.IsLegendary == true)
                        {
                            string yodaAPIUrl = _config[Constants.YodaAPIURL].ToString();
                            string yodaURL = yodaAPIUrl + "?text=" + jsonResult.FlavorTextEntries[0].FlavorText;
                            var yodaClient = new RestClient(yodaURL);
                            var yodaRequest = new RestRequest(Method.GET);
                            IRestResponse yodaResponse = yodaClient.Execute(yodaRequest);
                            var yodaDescription = yodaResponse.Content;

                            var yodaResult = JsonConvert.DeserializeObject<TranslationResponse>(yodaDescription);

                            if (!(yodaResult.Contents is null) && yodaResponse.StatusCode.ToString() == "OK")
                            {
                                jsonResult.FlavorTextEntries[0].FlavorText = yodaResult.Contents.Translated;
                            }
                        }
                        else
                        {
                            string shakespeareAPIUrl = _config[Constants.ShakespeareAPURL].ToString();
                            string shakespeareURL = shakespeareAPIUrl + "?text=" + jsonResult.FlavorTextEntries[0].FlavorText;
                            var shakespeareClient = new RestClient(shakespeareURL);
                            var shakespeareRequest = new RestRequest(Method.GET);
                            IRestResponse shakespeareResponse = shakespeareClient.Execute(shakespeareRequest);
                            var shakespeareDescription = shakespeareResponse.Content;

                            var shakespeareResult = JsonConvert.DeserializeObject<TranslationResponse>(shakespeareDescription);
                            if (!(shakespeareResult.Contents is null) && shakespeareResponse.StatusCode.ToString()=="OK")
                            {
                                jsonResult.FlavorTextEntries[0].FlavorText = shakespeareResult.Contents.Translated;
                            }
                        }

                        return Enumerable.Range(1, 1).Select(index => new PokemonResult
                        {
                            Name = jsonResult.Name,
                            Description = Regex.Replace(jsonResult.FlavorTextEntries[0].FlavorText, @"\t|\n|\r|\f", " "),
                            Habitat = jsonResult.Habitat.Name,
                            IsLegendary = jsonResult.IsLegendary
                        }).ToArray();

                    }
                    else
                    {
                        string[] errorList = { "Error connecting with the API. Request failed with Status Code: " + status };
                        return errorList;
                    }
                }
                else
                {
                    string[] errorList = { "Please provide pokemon name!!" };
                    return errorList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}
