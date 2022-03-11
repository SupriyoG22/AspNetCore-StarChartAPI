using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("api/[controller]")]
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
            var details = _context.CelestialObjects.Find(id);

            if (details == null)
            {
                return NotFound();
            }
            details.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();
            return Ok(details);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var details = _context.CelestialObjects.Where(x => x.Name.ToLower() == name.ToLower()).ToList();

            if (!details.Any())
            {
                return NotFound();
            }
            foreach (var celestialobject in details)
            {
                celestialobject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == celestialobject.Id).ToList();
            }
            return Ok(details);
        }

        [HttpGet()]
        public IActionResult GetAll()
        {
            var details = _context.CelestialObjects.ToList();

            foreach (var celestialobject in details)
            {
                celestialobject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == celestialobject.Id).ToList();
            }
            return Ok(details);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            if (ModelState.IsValid)
            {
                _context.CelestialObjects.Add(celestialObject);
                _context.SaveChanges();
            }
            return CreatedAtRoute(
                routeName: "GetById",
                routeValues: new { id = celestialObject.Id },
                value: celestialObject
                );
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var details = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (details == null)
            {
                return NotFound();
            }
            else
            {
                details.Name = celestialObject.Name;
                details.OrbitedObjectId = celestialObject.OrbitedObjectId;
                details.OrbitalPeriod = celestialObject.OrbitalPeriod;

                _context.CelestialObjects.Update(details);
                _context.SaveChanges();
                return NoContent();
            }
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var details = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (details == null)
            {
                return NotFound();
            }
            else
            {
                details.Name = name;
                _context.CelestialObjects.Update(details);
                _context.SaveChanges();
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var details = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (details == null)
            {
                return NotFound();
            }
            else
            {
                _context.CelestialObjects.Remove(details);
                _context.SaveChanges();
                return NoContent();
            }
        }
    }
}
