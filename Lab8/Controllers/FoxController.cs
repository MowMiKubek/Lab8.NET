using Lab8.Data;
using Lab8.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab8.Controllers
{
    [ApiController]
    [Route("api/fox")]
    public class FoxController : Controller
    {
        private IFoxesRepository _foxRepo;
        public FoxController(IFoxesRepository foxRepo)
        {
            _foxRepo = foxRepo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var result = _foxRepo.GetAll()
                .OrderByDescending(fox => fox.Loves)
                .ThenBy(fox => fox.Hates);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetOne(int id)
        {
            return Ok(_foxRepo.Get(id));
        }

        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody] Fox fox)
        {
            _foxRepo.Add(fox);
            return CreatedAtAction(nameof(Get), new { id = fox.Id }, fox);
        }

        [HttpPut("love/{id}")]
        public IActionResult Love(int id)
        {
            var fox = _foxRepo.Get(id);

            if(fox == null)
                return NotFound();

            fox.Loves++;
            _foxRepo.Update(id, fox);

            return Ok();
        }

        [HttpPut("hate/{id}")]
        public IActionResult Hate(int id) {
            var fox = _foxRepo.Get(id);

            if (fox == null)
                return NotFound();

            fox.Hates++;
            _foxRepo.Update(id, fox);

            return Ok();
        }
    }
}
