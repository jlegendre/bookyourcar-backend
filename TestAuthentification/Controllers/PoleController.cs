﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAuthentification.Models;
using TestAuthentification.ViewModels;

namespace TestAuthentification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PoleController : ControllerBase
    {
        private readonly A5dContext _context;

        public PoleController(A5dContext context)
        {
            _context = context;
        }

        // GET: api/Poles
        [HttpGet]
        public IEnumerable<Pole> GetPole()
        {
            return _context.Pole;
        }

        // GET: api/Poles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPole([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pole = await _context.Pole.FindAsync(id);

            if (pole == null)
            {
                return NotFound();
            }

            return Ok(pole);
        }

        // PUT: api/Poles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPole([FromRoute] int id, [FromBody] PoleViewModel poleModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var poleToModifie = await _context.Pole.FindAsync(id);

            poleToModifie.PoleAddress = poleModel.PoleAddress;
            poleToModifie.PoleCity = poleModel.PoleCity;
            poleToModifie.PoleCp = poleModel.PoleCp;
            poleToModifie.PoleName = poleModel.PoleName;

            _context.Entry(poleToModifie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PoleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Poles
        [HttpPost]
        public async Task<IActionResult> PostPole([FromBody] PoleViewModel poleModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var pole = new Pole()
            {
                PoleAddress = poleModel.PoleAddress,
                PoleCity = poleModel.PoleCity,
                PoleCp = poleModel.PoleCp,
                PoleName = poleModel.PoleName
            };

            _context.Pole.Add(pole);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPole", new { id = pole.PoleId }, pole);
        }

        // DELETE: api/Poles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePole([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pole = await _context.Pole.FindAsync(id);
            if (pole == null)
            {
                return NotFound();
            }

            _context.Pole.Remove(pole);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PoleExists(int id)
        {
            return _context.Pole.Any(e => e.PoleId == id);
        }
    }
}