using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pool_game_web.Data;
using pool_game_web.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using pool_game_web.Hubs;


namespace pool_game_web.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IHubContext<SignalrServer> _signalrHub;

        public ReservationsController(ApplicationDbContext context,IHubContext<SignalrServer> signalrHub)
        {
            _context = context;
            _signalrHub = signalrHub;
        }

        // GET: Reservations
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Reservations.Include(r => r.IdentityUser).Include(r => r.PoolTable).AsNoTracking();
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpGet]
        public IActionResult GetReservationsForUser(){
            var user =  _context.IdentityUsers.Where(t => t.Email == User.FindFirstValue(ClaimTypes.Email)).ToList();
            var res = _context.Reservations.Include(r => r.IdentityUser).Include(r => r.PoolTable).Where(r=>r.IdentityUserId==user[0].Id).AsNoTracking().ToList();
            return View(res);
        }


        [HttpGet]
        public IActionResult GetReservations(){
            var res = _context.Reservations.Include(r => r.IdentityUser).Include(r => r.PoolTable).AsNoTracking().ToList();
            return Ok(res);
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
                if(tables.Count!=0)
                {    
                if(res.Count == 0||res==null){
                    reservation.PoolTableId = tables[0].PoolTableId;
                    _context.Add(reservation);
                    await _context.SaveChangesAsync();
                    await _signalrHub.Clients.All.SendAsync("LoadReservations");
                    return RedirectToAction(nameof(Index));
                }       
                else{
                    foreach (Reservation reserv in res)
                    {
                        PoolTable itemToRemove = tables.SingleOrDefault(r => r.PoolTableId == reserv.PoolTableId);
                        tables.Remove(itemToRemove);
                    }
                    if(tables.Count != 0){
                        reservation.PoolTableId = tables[0].PoolTableId;
                        _context.Add(reservation);
                        await _context.SaveChangesAsync();
                        await _signalrHub.Clients.All.SendAsync("LoadReservations");
                        return RedirectToAction(nameof(Index));
                    }      
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
        [HttpPost, ActionName("CreateForUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateForUser([Bind("ReservationId,ReservationName,Date,TimeStart,TimeEnding,IdentityUserId,PoolTableId")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("Usao 1");
                IList<Reservation> res = _context.Reservations
                    .Where(r => (r.Date == reservation.Date) && (((TimeSpan.Compare(reservation.TimeStart,r.TimeStart)<=0)&&(TimeSpan.Compare(reservation.TimeEnding,r.TimeEnding)>=0))||
                                ((TimeSpan.Compare(reservation.TimeStart,r.TimeStart)>=0)&&(TimeSpan.Compare(reservation.TimeEnding,r.TimeEnding)<=0))||
                                ((TimeSpan.Compare(reservation.TimeStart,r.TimeEnding)<0)&&(TimeSpan.Compare(reservation.TimeEnding,r.TimeEnding)>0))||
                                ((TimeSpan.Compare(reservation.TimeStart,r.TimeStart)<0)&&(TimeSpan.Compare(reservation.TimeEnding,r.TimeStart)>0))) )
                                .ToList();
                   // .FirstOrDefault();
                IList<PoolTable> tables = _context.PoolTables.ToList();     
                if(res.Count == 0||res==null){
                    reservation.PoolTableId = tables[0].PoolTableId;
                    _context.Add(reservation);
                    await _context.SaveChangesAsync();
                    await _signalrHub.Clients.All.SendAsync("LoadReservations");
                    return RedirectToAction(nameof(GetReservationsForUser));
                }       
                else{
                    foreach (Reservation reserv in res)
                    {

                        PoolTable itemToRemove = tables.SingleOrDefault(r => r.PoolTableId == reserv.PoolTableId);
                        tables.Remove(itemToRemove);
                    }
                    if(tables.Count != 0){
                        reservation.PoolTableId = tables[0].PoolTableId;
                        _context.Add(reservation);
                        await _context.SaveChangesAsync();
                        await _signalrHub.Clients.All.SendAsync("LoadReservations");
                        return RedirectToAction(nameof(GetReservationsForUser));
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
        public async Task<IActionResult> Edit(int reservationId, [Bind("ReservationId,ReservationName,Date,TimeStart,TimeEnding,IdentityUserId,PoolTableId")] Reservation reservation)
        {
            if (reservationId != reservation.ReservationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Reservation old = _context.Reservations.Find(reservationId);
                bool eligableToUpdate = false;
                if(!((TimeSpan.Compare(reservation.TimeStart,old.TimeStart)==0)&&(TimeSpan.Compare(reservation.TimeEnding,old.TimeEnding)==0)&&(DateTime.Compare(reservation.Date,old.Date)==0)))
                {
                    IList<Reservation> res = _context.Reservations
                    .Where(r => (r.Date == reservation.Date) && (((TimeSpan.Compare(reservation.TimeStart,r.TimeStart)<=0)&&(TimeSpan.Compare(reservation.TimeEnding,r.TimeEnding)>=0))||
                                ((TimeSpan.Compare(reservation.TimeStart,r.TimeStart)>=0)&&(TimeSpan.Compare(reservation.TimeEnding,r.TimeEnding)<=0))||
                                ((TimeSpan.Compare(reservation.TimeStart,r.TimeEnding)<0)&&(TimeSpan.Compare(reservation.TimeEnding,r.TimeEnding)>0))||
                                ((TimeSpan.Compare(reservation.TimeStart,r.TimeStart)<0)&&(TimeSpan.Compare(reservation.TimeEnding,r.TimeStart)>0))) )
                                .AsNoTracking()
                                .ToList();
                    if((res.Count == 0)||((res.Count == 1)&&(res[0].ReservationId==reservationId))){
                        eligableToUpdate = true;
                    }
                    else{
                        IList<PoolTable> tables = _context.PoolTables.ToList();
                        foreach (Reservation reserv in res)
                        {       
                            PoolTable itemToRemove = tables.SingleOrDefault(r => r.PoolTableId == reserv.PoolTableId);
                            tables.Remove(itemToRemove);
                        }
                        if(tables.Count != 0){
                            reservation.PoolTableId = tables[0].PoolTableId;
                            eligableToUpdate = true;
                        }
                    }
                }
                else
                {
                    eligableToUpdate = true;
                }
                _context.Entry(old).State = EntityState.Detached;
                if(eligableToUpdate){
                    try
                    {

                        _context.Update(reservation);
                        await _context.SaveChangesAsync();
                        await _signalrHub.Clients.All.SendAsync("LoadReservations");
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

            }
            ModelState.AddModelError("", "No available table at that time and date");
            var a = await _context.IdentityUsers.Where(t => t.Email == User.FindFirstValue(ClaimTypes.Email)).ToListAsync();
            ViewData["IdentityUserId"] = new SelectList(a,"Id","Email");
           // ViewData["IdentityUserId"] = new SelectList(_context.IdentityUsers, "Id", "Id", reservation.IdentityUserId);
            ViewData["PoolTableId"] = new SelectList(_context.PoolTables, "PoolTableId", "PoolTableId", reservation.PoolTableId);
            return View(reservation);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditForUser(int reservationId, [Bind("ReservationId,ReservationName,Date,TimeStart,TimeEnding,IdentityUserId,PoolTableId")] Reservation reservation)
        {
            if (reservationId != reservation.ReservationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                    await _signalrHub.Clients.All.SendAsync("LoadReservations");
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
                return RedirectToAction(nameof(GetReservationsForUser));
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
        public async Task<IActionResult> DeleteConfirmed(int reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            await _signalrHub.Clients.All.SendAsync("LoadReservations");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost,ActionName("DeleteConfirmedForUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedForUser(int reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            await _signalrHub.Clients.All.SendAsync("LoadReservations");
            return RedirectToAction(nameof(GetReservationsForUser));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationId == id);
        }
    }
}
