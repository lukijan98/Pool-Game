using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pool_game_web.Models;
using Microsoft.AspNetCore.Identity;

namespace pool_game_web.Repository
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> getAllReservations();
        Task<IEnumerable<Reservation>> getReservationsForUser(IdentityUser user);
        Task<Reservation> getReservationById(int id);
        bool reservationExists(int id);
        Task<bool> addReservation(Reservation reservation);
        Task<bool> updateReservation(int reservationId ,Reservation reservation);
        Task deleteReservation(int reservationId);
    }
}