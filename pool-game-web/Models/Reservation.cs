using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pool_game_web.Models
{
    public class Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReservationId {get;set;}

        [Display(Name = "Enter name")]
        [StringLength(20)]
        [Required(ErrorMessage = "{0} Is required.")]
        public string ReservationName { get; set; }

        public int VisitorId {get;set;}
        public Visitor Visitor {get;set;}

        public int PoolTableId {get;set;}
        public PoolTable PoolTable {get;set;}

    }
}