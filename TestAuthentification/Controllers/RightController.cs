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
    public class RightController : ControllerBase
    {
        private readonly A5dContext _context;

        public RightController(A5dContext context)
        {
            _context = context;
        }

        // GET: api/Rights
        [HttpGet]
        public IEnumerable<Right> GetRight()
        {
            return _context.Right;
        }

        // GET: api/Rights/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRight([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var right = await _context.Right.FindAsync(id);

            if (right == null)
            {
                return NotFound();
            }

            return Ok(right);
        }

        // PUT: api/Rights/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRight([FromRoute] int id, [FromBody] Right right)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != right.RightId)
            {
                return BadRequest();
            }

            _context.Entry(right).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RightExists(id))
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

        // POST: api/Rights
        [HttpPost]
        public async Task<IActionResult> PostRight([FromBody] Right right)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Right.Add(right);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRight", new { id = right.RightId }, right);
        }

        // DELETE: api/Rights/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRight([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var right = await _context.Right.FindAsync(id);
            if (right == null)
            {
                return NotFound();
            }

            _context.Right.Remove(right);
            await _context.SaveChangesAsync();

            return Ok(right);
        }

        private bool RightExists(int id)
        {
            return _context.Right.Any(e => e.RightId == id);
        }
    }
}