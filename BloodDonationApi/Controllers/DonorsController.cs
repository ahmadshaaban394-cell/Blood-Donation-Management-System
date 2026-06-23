using BloodDonationAPI.Data;
using BloodDonationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace BloodDonationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DonorsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Donor>>> GetDonors()
        {
            return await _context.Donors.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Donor>> GetDonor(int id)
        {
            var donor = await _context.Donors.FindAsync(id);

            if (donor == null)
            {
                return NotFound();
            }

            return donor;
        }

        [HttpPost]
        public async Task<ActionResult<Donor>> AddDonor(Donor donor)
        {
            _context.Donors.Add(donor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDonor), new { id = donor.Id }, donor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDonor(int id, Donor donor)
        {
            if (id != donor.Id)
            {
                return BadRequest();
            }

            _context.Entry(donor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDonor(int id)
        {
            var donor = await _context.Donors.FindAsync(id);

            if (donor == null)
            {
                return NotFound();
            }

            _context.Donors.Remove(donor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}