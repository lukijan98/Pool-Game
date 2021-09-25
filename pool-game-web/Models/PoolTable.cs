using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace pool_game_web.Models
{
    public class PoolTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PoolTableId {get;set;}

        [Display(Name = "Enter number")]
        [Required(ErrorMessage = "{0} Is required.")]
        public int PoolTableNumber { get; set; }

        public IList<Reservation> Reservations {get;set;}
    }
}