using BloodDonationAPI.Data;
using BloodDonationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BloodRequestsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BloodRequest>>> GetBloodRequests()
        {
            return await _context.BloodRequests.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BloodRequest>> GetBloodRequest(int id)
        {
            var bloodRequest = await _context.BloodRequests.FindAsync(id);

            if (bloodRequest == null)
            {
                return NotFound();
            }

            return bloodRequest;
        }

        [HttpPost]
        public async Task<ActionResult<BloodRequest>> AddBloodRequest(BloodRequest bloodRequest)
        {
            _context.BloodRequests.Add(bloodRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBloodRequest), new { id = bloodRequest.Id }, bloodRequest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBloodRequest(int id, BloodRequest bloodRequest)
        {
            if (id != bloodRequest.Id)
            {
                return BadRequest();
            }

            _context.Entry(bloodRequest).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBloodRequest(int id)
        {
            var bloodRequest = await _context.BloodRequests.FindAsync(id);

            if (bloodRequest == null)
            {
                return NotFound();
            }

            _context.BloodRequests.Remove(bloodRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}