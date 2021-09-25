namespace pool_game_web.Models
{
    public class Reservation
    {
        public int ReservationId {get;set;}

        public int VisitorId {get;set;}
        public Visitor Visitor {get;set;}

        public int PoolTableId {get;set;}
        public PoolTable PoolTable {get;set;}

    }
}