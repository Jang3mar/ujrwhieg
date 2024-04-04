using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly FinanceDBContext _context;

        public OperationsController(FinanceDBContext context)
        {
            _context = context;
        }

        // GET: api/Operations
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Operation>>> GetOperations([FromQuery] string? start_date,
            [FromQuery] string? end_date)
        {
          if (_context.Operations == null)
          {
              return NotFound();
            }
            List<Operation> operations = await _context.Operations.ToListAsync();
            DateTime dateStart, dateEnd;
            bool success1 = DateTime.TryParse(start_date, out dateStart);
            bool success2 = DateTime.TryParse(end_date, out dateEnd);
            if (success1)
            {
                if (start_date != null) operations.Where(x => x.DateOperation >= DateTime.ParseExact(start_date, "dd.MM.yyyy", CultureInfo.InvariantCulture)).ToList();
            }
            if (success2)
            {
                if (end_date != null) operations.Where(x => x.DateOperation <= DateTime.ParseExact(end_date, "dd.MM.yyyy", CultureInfo.InvariantCulture)).ToList();
            }
                return operations;
        }

        // GET: api/Operations/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Operation>> GetOperation(int id)
        {
          if (_context.Operations == null)
          {
              return NotFound();
          }
            var operation = await _context.Operations.FindAsync(id);

            if (operation == null)
            {
                return NotFound();
            }

            return operation;
        }

        // PUT: api/Operations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutOperation(int id, Operation operation)
        {
            if (id != operation.IdOperation)
            {
                return BadRequest();
            }

            _context.Entry(operation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OperationExists(id))
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

        // POST: api/Operations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Operation>> PostOperation(Operation operation)
        {
          if (_context.Operations == null)
          {
              return Problem("Entity set 'FinanceDBContext.Operations'  is null.");
          }
            _context.Operations.Add(operation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOperation", new { id = operation.IdOperation }, operation);
        }

        // DELETE: api/Operations/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteOperation(int id)
        {
            if (_context.Operations == null)
            {
                return NotFound();
            }
            var operation = await _context.Operations.FindAsync(id);
            if (operation == null)
            {
                return NotFound();
            }

            _context.Operations.Remove(operation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("mdel")]
        [Authorize]
        public async Task<IActionResult> DeleteOperations([FromQuery] List<int> ids)
        {
            if (_context.Operations == null)
            {
                return NotFound();
            }
            foreach (int item in ids)
            {
                var operation = await _context.Operations.FindAsync(item);

                if (operation == null) continue;

                _context.Operations.Remove(operation);
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OperationExists(int id)
        {
            return (_context.Operations?.Any(e => e.IdOperation == id)).GetValueOrDefault();
        }
    }
}
