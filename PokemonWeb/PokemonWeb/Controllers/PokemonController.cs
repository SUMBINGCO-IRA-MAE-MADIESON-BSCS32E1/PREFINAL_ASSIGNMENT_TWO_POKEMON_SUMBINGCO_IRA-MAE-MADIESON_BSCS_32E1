using Microsoft.AspNetCore.Mvc;
using PokemonMVCApp.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PokemonMVCApp.Controllers
{
    public class PokemonController : Controller
    {
        private readonly HttpClient _httpClient;

        public PokemonController()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int limit = 20;
            int offset = (page - 1) * limit;
            var response = await _httpClient.GetStringAsync($"https://pokeapi.co/api/v2/pokemon?offset={offset}&limit={limit}");
            var json = JObject.Parse(response);

            var pokemonList = new List<Pokemon>();
            foreach (var item in json["results"])
            {
                var pokemon = new Pokemon
                {
                    Name = item["name"].ToString()
                };
                pokemonList.Add(pokemon);
            }

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)json["count"] / limit;
            return View(pokemonList);
        }

        public async Task<IActionResult> Details(string name)
        {
            var response = await _httpClient.GetStringAsync($"https://pokeapi.co/api/v2/pokemon/{name}");
            var json = JObject.Parse(response);

            var pokemon = new Pokemon
            {
                Id = (int)json["id"],
                Name = json["name"].ToString(),
                Moves = new List<string>(),
                Abilities = new List<string>()
            };

            foreach (var move in json["moves"])
            {
                pokemon.Moves.Add(move["move"]["name"].ToString());
            }

            foreach (var ability in json["abilities"])
            {
                pokemon.Abilities.Add(ability["ability"]["name"].ToString());
            }

            return View(pokemon);
        }
    }
}