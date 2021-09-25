namespace pool_game_web.Models
{
    public class Visitor
    {
        public int VisitorId {get;set;}

        public IList<Reservation> Reservations {get;set;}
    }
}