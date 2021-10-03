using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pool_game_web.Models;
using Microsoft.EntityFrameworkCore;
using pool_game_web.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace pool_game_web.Repository.Implementation
{
    public class ReservationRepositoryImpl: IReservationRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservationRepositoryImpl(ApplicationDbContext context){
            _context = context;
        }

        public async Task<IEnumerable<Reservation>> getAllReservations(){
            return await _context.Reservations
                .Include(r => r.IdentityUser)
                .Include(r => r.PoolTable)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<Reservation>> getReservationsForUser(IdentityUser user){
            return await _context.Reservations
                 .Include(r => r.IdentityUser)
                 .Include(r => r.PoolTable)
                 .Where(r=>r.IdentityUserId==user.Id)
                 .AsNoTracking()
                 .ToListAsync();
        }
        public async Task<Reservation> getReservationById(int id){
            return await _context.Reservations
                .Include(r => r.IdentityUser)
                .Include(r => r.PoolTable)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
        }
        public bool reservationExists(int id){
            return _context.Reservations.Any(e => e.ReservationId == id);
        }
        public async Task<bool> addReservation(Reservation reservation){
                IList<Reservation> res = await getReservationsTimeAndDateIntersection(reservation);
                IList<PoolTable> tables = await _context.PoolTables.ToListAsync();
                if(tables.Count!=0)
                {    
                if(res.Count == 0||res==null){
                    reservation.PoolTableId = tables[0].PoolTableId;
                    _context.Add(reservation);
                    await _context.SaveChangesAsync();
                    return true;
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
                        return true;
                    }      
                }
                }
                return false; 
        }
        public async Task<bool> updateReservation(int reservationId ,Reservation reservation){

                Reservation old = _context.Reservations.Find(reservationId);
                bool eligableToUpdate = false;
                if(!((TimeSpan.Compare(reservation.TimeStart,old.TimeStart)==0)&&(TimeSpan.Compare(reservation.TimeEnding,old.TimeEnding)==0)&&(DateTime.Compare(reservation.Date,old.Date)==0)))
                {
                    IList<Reservation> res = await getReservationsTimeAndDateIntersection(reservation);
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
                        _context.Update(reservation);
                        await _context.SaveChangesAsync();
                        return true;
                }
                return false;
        }
        public async Task deleteReservation(int reservationId){
            //var reservation = await _context.Reservations.FindAsync(reservationId);
            _context.Reservations.Remove(await _context.Reservations.FindAsync(reservationId));
            await _context.SaveChangesAsync();
        }

        public async Task<IList<Reservation>> getReservationsTimeAndDateIntersection(Reservation reservation){
            return await _context.Reservations
                    .Where(r => (r.Date == reservation.Date) && (((TimeSpan.Compare(reservation.TimeStart,r.TimeStart)<=0)&&(TimeSpan.Compare(reservation.TimeEnding,r.TimeEnding)>=0))||
                                ((TimeSpan.Compare(reservation.TimeStart,r.TimeStart)>=0)&&(TimeSpan.Compare(reservation.TimeEnding,r.TimeEnding)<=0))||
                                ((TimeSpan.Compare(reservation.TimeStart,r.TimeEnding)<0)&&(TimeSpan.Compare(reservation.TimeEnding,r.TimeEnding)>0))||
                                ((TimeSpan.Compare(reservation.TimeStart,r.TimeStart)<0)&&(TimeSpan.Compare(reservation.TimeEnding,r.TimeStart)>0))) )
                                .ToListAsync();
        }

        
    }
}