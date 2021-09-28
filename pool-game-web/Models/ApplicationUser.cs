using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace pool_game_web.Models
{
    public class ApplicationUser : IdentityUser<string>
    {

        public IList<Reservation> Reservations {get;set;}
    }
}