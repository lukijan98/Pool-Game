namespace pool_game_web.Models
{
    public class PoolTable
    {
        public int PoolTableId {get;set;}

        public IList<Reservation> Reservations {get;set;}
    }
}