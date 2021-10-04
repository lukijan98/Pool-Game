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
using pool_game_web.Repository.Implementation;
using pool_game_web.Repository;


namespace pool_game_web.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IHubContext<SignalrServer> _signalrHub;

        private readonly IReservationRepository _reservationRepository;

        private readonly UserManager<IdentityUser> _userManager;

        public ReservationsController(ApplicationDbContext context,IHubContext<SignalrServer> signalrHub,IReservationRepository reservationRepository,UserManager<IdentityUser> userManager)
        {
            _context = context;
            _signalrHub = signalrHub;
            _reservationRepository = reservationRepository;
            _userManager = userManager;
        }

        //[Authorize(Roles="Admin")]
        public async Task<IActionResult> Index()
        {  
            if(await GetCurrentUserRole()=="Admin"){
                await _signalrHub.Clients.All.SendAsync("LoadReservations");
                return View();
            }     
            else
                return RedirectToAction(nameof(IndexForUser));      
        }

        [HttpGet]
        public async Task<IActionResult> IndexForUser(){
            return View(await _reservationRepository.getReservationsForUser(await GetCurrentUser()));
        }


        [HttpGet]
        public async Task<IActionResult> GetReservations(){
            return Ok(await _reservationRepository.getAllReservations());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var reservation = await  _reservationRepository.getReservationById(id.Value);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public async Task<IActionResult> Create()
        {
            ViewData["IdentityUserId"] = new SelectList(new List<IdentityUser>{await GetCurrentUser()},"Id","Email");
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
                if(await _reservationRepository.addReservation(reservation))
                {
                    await _signalrHub.Clients.All.SendAsync("LoadReservations");
                    return RedirectToAction(nameof(Index));

                }   
                else
                    ModelState.AddModelError("", "No available table at that time and date");
            }
            ViewData["IdentityUserId"] = new SelectList(new List<IdentityUser>{await GetCurrentUser()},"Id","Email");
            //ViewData["PoolTableId"] = new SelectList(_context.PoolTables, "PoolTableId", "PoolTableId");
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
            ViewData["IdentityUserId"] = new SelectList(new List<IdentityUser>{(await _reservationRepository.getReservationById(id.Value)).IdentityUser},"Id","Email");
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
                try
                {
                    if(await _reservationRepository.updateReservation(reservationId,reservation)){
                        await _signalrHub.Clients.All.SendAsync("LoadReservations");
                        return RedirectToAction(nameof(Index));
                    }      
                    else
                        ModelState.AddModelError("", "No available table at that time and date");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_reservationRepository.reservationExists(reservation.ReservationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["IdentityUserId"] = new SelectList(new List<IdentityUser>{(await _reservationRepository.getReservationById(reservationId)).IdentityUser},"Id","Email");
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
            var reservation = await  _reservationRepository.getReservationById(id.Value);
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
            await _reservationRepository.deleteReservation(reservationId);
            await _signalrHub.Clients.All.SendAsync("LoadReservations");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IdentityUser> GetCurrentUser()
        {
            ClaimsPrincipal currentUser = User;
            return await _userManager.GetUserAsync(User);//.Result;
        }
        public async Task<string> GetCurrentUserRole(){
            return (await _userManager.GetRolesAsync(await GetCurrentUser()))[0];
        }
    }
}
