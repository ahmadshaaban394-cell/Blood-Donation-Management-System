using BloodDonationAPI.Data;
using BloodDonationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodStocksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BloodStocksController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BloodStock>>> GetBloodStocks()
        {
            return await _context.BloodStocks.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BloodStock>> GetBloodStock(int id)
        {
            var bloodStock = await _context.BloodStocks.FindAsync(id);

            if (bloodStock == null)
            {
                return NotFound();
            }

            return bloodStock;
        }

        [HttpPost]
        public async Task<ActionResult<BloodStock>> AddBloodStock(BloodStock bloodStock)
        {
            _context.BloodStocks.Add(bloodStock);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBloodStock), new { id = bloodStock.Id }, bloodStock);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBloodStock(int id, BloodStock bloodStock)
        {
            if (id != bloodStock.Id)
            {
                return BadRequest();
            }

            _context.Entry(bloodStock).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBloodStock(int id)
        {
            var bloodStock = await _context.BloodStocks.FindAsync(id);

            if (bloodStock == null)
            {
                return NotFound();
            }

            _context.BloodStocks.Remove(bloodStock);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}