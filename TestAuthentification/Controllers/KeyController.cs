using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAuthentification.Models;

namespace TestAuthentification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyController : ControllerBase
    {
        private readonly BookYourCarContext _context;

        public KeyController(BookYourCarContext context)
        {
            _context = context;
        }

        // GET: api/Keys
        [HttpGet]
        public IEnumerable<Key> GetKey()
        {
            return _context.Key;
        }

        // GET: api/Keys/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetKey([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var key = await _context.Key.FindAsync(id);

            if (key == null)
            {
                return NotFound();
            }

            return Ok(key);
        }

        // PUT: api/Keys/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKey([FromRoute] int id, [FromBody] Key key)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != key.KeyId)
            {
                return BadRequest();
            }

            _context.Entry(key).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KeyExists(id))
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

        // POST: api/Keys
        [HttpPost]
        public async Task<IActionResult> PostKey([FromBody] Key key)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Key.Add(key);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKey", new { id = key.KeyId }, key);
        }

        // DELETE: api/Keys/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKey([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var key = await _context.Key.FindAsync(id);
            if (key == null)
            {
                return NotFound();
            }

            _context.Key.Remove(key);
            await _context.SaveChangesAsync();

            return Ok(key);
        }

        private bool KeyExists(int id)
        {
            return _context.Key.Any(e => e.KeyId == id);
        }
    }
}