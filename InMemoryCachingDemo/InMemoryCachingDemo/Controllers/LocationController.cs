using InMemoryCachingDemo.Database.Entites;
using InMemoryCachingDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace InMemoryCachingDemo.Controllers
{
    [ApiController]
    [Route("[Controller]/[Action]")]
    public class LocationController:ControllerBase
    {
        private readonly ILocationService service;

        public LocationController(ILocationService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddCountry([Required][FromForm]string Name, [Required][FromForm] string NameOfVietnamese)
        {
            var data = await service.AddCountry(Name,NameOfVietnamese);
            return CreatedAtAction(nameof(GetCountryById), new { Id = data.Id }, data);
        }

        [HttpPut]
        [Route("{Id:required:int:min(1)}")]
        public async Task<IActionResult> UpdateCountry([Required][FromRoute]int Id,[Required][FromForm] string Name, [Required][FromForm] string NameOfVietnamese)
        {
            var result = await service.UpdateCountry(Id, Name, NameOfVietnamese);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await service.GetCountries();
            return Ok(countries);
        }

        [HttpGet]
        [Route("{Id:required:int:min(1)}")]
        public async Task<IActionResult> GetCountryById([FromRoute][Required] int Id)
        {
            var data = await service.GetCountryById(Id);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetCities()
        {
            var data = await service.GetCities();
            return Ok(data);
        }
    }
}
