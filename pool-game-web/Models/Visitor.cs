using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pool_game_web.Models
{
    public class Visitor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VisitorId {get;set;}

        [Display(Name = "Enter name")]
        [StringLength(20)]
        [Required(ErrorMessage = "{0} Is required.")]
        public string VisitorName { get; set; }

        public IList<Reservation> Reservations {get;set;}
    }
}