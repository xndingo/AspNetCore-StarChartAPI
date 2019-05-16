using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObj = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (celestialObj != null)
            {
                celestialObj.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == id).ToList();
                return Ok(celestialObj);
            }

            return NotFound();
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Name == name).ToList();
            if (!celestialObjects.Any()) return NotFound();

            foreach (var obj in celestialObjects)
                obj.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == obj.Id).ToList();

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects;
            foreach (var obj in celestialObjects)
            {
                obj.Satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == obj.Id).ToList();
            }
            return Ok(celestialObjects);
        }


        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new {id = celestialObject.Id}, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var celObj = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (celObj == null)
            {
                return NotFound();
            }

            celObj.Name = celestialObject.Name;
            celObj.OrbitalPeriod = celestialObject.OrbitalPeriod;
            celObj.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.CelestialObjects.Update(celObj);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celObj = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (celObj == null)
            {
                return NotFound();
            }

            celObj.Name = name;
            _context.CelestialObjects.Update(celObj);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celObjects = _context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id).ToList();
            if (!celObjects.Any())
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(celObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
