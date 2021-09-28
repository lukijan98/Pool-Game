using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pool_game_web.Data;
using pool_game_web.Models;

namespace pool_game_web.Controllers
{
    public class PoolTablesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PoolTablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PoolTables
        public async Task<IActionResult> Index()
        {
            return View(await _context.PoolTables.ToListAsync());
        }

        // GET: PoolTables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poolTable = await _context.PoolTables
                .FirstOrDefaultAsync(m => m.PoolTableId == id);
            if (poolTable == null)
            {
                return NotFound();
            }

            return View(poolTable);
        }

        // GET: PoolTables/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PoolTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PoolTableId,PoolTableNumber")] PoolTable poolTable)
        {
            if (ModelState.IsValid)
            {
                _context.Add(poolTable);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(poolTable);
        }

        // GET: PoolTables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poolTable = await _context.PoolTables.FindAsync(id);
            if (poolTable == null)
            {
                return NotFound();
            }
            return View(poolTable);
        }

        // POST: PoolTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PoolTableId,PoolTableNumber")] PoolTable poolTable)
        {
            if (id != poolTable.PoolTableId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(poolTable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PoolTableExists(poolTable.PoolTableId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(poolTable);
        }

        // GET: PoolTables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var poolTable = await _context.PoolTables
                .FirstOrDefaultAsync(m => m.PoolTableId == id);
            if (poolTable == null)
            {
                return NotFound();
            }

            return View(poolTable);
        }

        // POST: PoolTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var poolTable = await _context.PoolTables.FindAsync(id);
            _context.PoolTables.Remove(poolTable);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PoolTableExists(int id)
        {
            return _context.PoolTables.Any(e => e.PoolTableId == id);
        }
    }
}
