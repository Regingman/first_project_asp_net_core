using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;

namespace WebApplication3.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyInfoesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CompanyInfoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CompanyInfoes
        [HttpGet]
        public IEnumerable<CompanyInfo> GetCompanyInfos()
        {
            return _context.CompanyInfos;
        }

        // GET: api/CompanyInfoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyInfo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyInfo = await _context.CompanyInfos.FindAsync(id);

            if (companyInfo == null)
            {
                return NotFound();
            }

            return Ok(companyInfo);
        }

        // PUT: api/CompanyInfoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompanyInfo([FromRoute] int id, [FromBody] CompanyInfo companyInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != companyInfo.Id)
            {
                return BadRequest();
            }

            _context.Entry(companyInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyInfoExists(id))
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

        // POST: api/CompanyInfoes
        [HttpPost]
        public async Task<IActionResult> PostCompanyInfo([FromBody] CompanyInfo companyInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CompanyInfos.Add(companyInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompanyInfo", new { id = companyInfo.Id }, companyInfo);
        }

        // DELETE: api/CompanyInfoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyInfo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyInfo = await _context.CompanyInfos.FindAsync(id);
            if (companyInfo == null)
            {
                return NotFound();
            }

            _context.CompanyInfos.Remove(companyInfo);
            await _context.SaveChangesAsync();

            return Ok(companyInfo);
        }

        private bool CompanyInfoExists(int id)
        {
            return _context.CompanyInfos.Any(e => e.Id == id);
        }
    }
}