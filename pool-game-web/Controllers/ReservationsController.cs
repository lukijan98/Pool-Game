using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pool_game_web.Data;
using pool_game_web.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace pool_game_web.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReservationsController(ApplicationDbContext context,UserManager<IdentityUser> userManager)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Reservations.Include(r => r.IdentityUser).Include(r => r.PoolTable);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.IdentityUser)
                .Include(r => r.PoolTable)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public async Task<IActionResult> Create()
        {
            var a = await _context.IdentityUsers.Where(t => t.Email == User.FindFirstValue(ClaimTypes.Email)).ToListAsync();
            ViewData["IdentityUserId"] = new SelectList(a,"Id","Email");
           // ViewData["IdentityUserId"] = new SelectList(_context.IdentityUsers, "Id", "Id");
           // ViewData["PoolTableId"] = new SelectList(_context.PoolTables, "PoolTableId", "PoolTableId");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationId,ReservationName,Date,TimeStart,TimeEnding,IdentityUserId,PoolTableId")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                IList<Reservation> res = _context.Reservations
                    .Where(r => (r.Date == reservation.Date) && (((TimeSpan.Compare(reservation.TimeStart,r.TimeStart)<=0)&&(TimeSpan.Compare(reservation.TimeEnding,r.TimeEnding)>=0))||
                                ((TimeSpan.Compare(reservation.TimeStart,r.TimeStart)>=0)&&(TimeSpan.Compare(reservation.TimeEnding,r.TimeEnding)<=0))||
                                ((TimeSpan.Compare(reservation.TimeStart,r.TimeEnding)<0)&&(TimeSpan.Compare(reservation.TimeEnding,r.TimeEnding)>0))||
                                ((TimeSpan.Compare(reservation.TimeStart,r.TimeStart)<0)&&(TimeSpan.Compare(reservation.TimeEnding,r.TimeStart)>0))) )
                                .ToList();
                   // .FirstOrDefault();
                IList<PoolTable> tables = _context.PoolTables.ToList();
                Console.WriteLine(tables.Count);
                for(int i=0;i<tables.Count;i++)
                {
                    Console.WriteLine(tables[i]);
                }       
                if(res.Count == 0||res==null){
                    reservation.PoolTableId = tables[0].PoolTableId;
                    _context.Add(reservation);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }       
                else{
                    foreach (Reservation reserv in res)
                    {
                        Console.WriteLine(reserv.PoolTableId);
                        PoolTable itemToRemove = tables.SingleOrDefault(r => r.PoolTableId == reserv.PoolTableId);
                        tables.Remove(itemToRemove);
                    }
                    if(tables.Count != 0){
                        reservation.PoolTableId = tables[0].PoolTableId;
                        _context.Add(reservation);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }      
                }    
            }
            ModelState.AddModelError("", "No available table at that time and date");
            var a = await _context.IdentityUsers.Where(t => t.Email == User.FindFirstValue(ClaimTypes.Email)).ToListAsync();
            ViewData["IdentityUserId"] = new SelectList(a,"Id","Email");
           // ViewData["IdentityUserId"] = new SelectList(_context.IdentityUsers, "Id", "Id");
            ViewData["PoolTableId"] = new SelectList(_context.PoolTables, "PoolTableId", "PoolTableId");
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["IdentityUserId"] = new SelectList(_context.IdentityUsers, "Id", "Id", reservation.IdentityUserId);
            ViewData["PoolTableId"] = new SelectList(_context.PoolTables, "PoolTableId", "PoolTableId", reservation.PoolTableId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationId,ReservationName,Date,TimeStart,TimeEnding,IdentityUserId,PoolTableId")] Reservation reservation)
        {
            if (id != reservation.ReservationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.ReservationId))
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
            ViewData["IdentityUserId"] = new SelectList(_context.IdentityUsers, "Id", "Id", reservation.IdentityUserId);
            ViewData["PoolTableId"] = new SelectList(_context.PoolTables, "PoolTableId", "PoolTableId", reservation.PoolTableId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.IdentityUser)
                .Include(r => r.PoolTable)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationId == id);
        }
    }
}
