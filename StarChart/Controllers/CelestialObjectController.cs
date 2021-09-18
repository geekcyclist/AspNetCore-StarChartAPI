using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarChart.Data;
using StarChart.Models;

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
            var co = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (co == null)
            {
                return NotFound();
            }

            AddSatellites(co);
            return new OkObjectResult(co);
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Name == name);
            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            foreach (var celestialObject in celestialObjects)
            {
                AddSatellites(celestialObject);
            }

            return new OkObjectResult(celestialObjects.ToList());
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects;
            foreach (var celestialObject in celestialObjects)
            {
                AddSatellites(celestialObject);
            }

            return new OkObjectResult(celestialObjects.ToList());
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.Add(celestialObject);
            _context.SaveChanges();
            return new CreatedAtRouteResult("GetById", new {id = celestialObject.Id}, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var co = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (co == null)
            {
                return NotFound();
            }

            co.Name = celestialObject.Name;
            co.OrbitalPeriod = celestialObject.OrbitalPeriod;
            co.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.Update(co);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var co = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (co == null)
            {
                return NotFound();
            }

            co.Name = name;
            _context.Update(co);
            _context.SaveChanges();
            
            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var co = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (co == null)
            {
                return NotFound();
            }

            _context.Remove(co);
            _context.SaveChanges();

            return NoContent();
        }

        private void AddSatellites(CelestialObject co)
        {
            co.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == co.Id).ToList();
        }
    }
}
