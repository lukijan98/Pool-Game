using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using FoolProof.Core;

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


        [DataType(DataType.Date)]
        [Display(Name="Date")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH\:mm}")]
        public DateTime Date { get; set; }


        [DataType(DataType.Time)]
        [Display(Name="Start time")]
        //[DisplayFormat(ApplyFormatInEditMode = true,   DataFormatString = "{0:HH:mm:ss}")]
        public TimeSpan TimeStart { get; set; }

        [DataType(DataType.Time)]
        [Display(Name="End time")]
        [GreaterThan("TimeStart")]
       // [DisplayFormat(ApplyFormatInEditMode = true,   DataFormatString = "{0:HH:mm:ss}")]
        public TimeSpan TimeEnding { get; set; }


        public string IdentityUserId {get;set;}
        public IdentityUser IdentityUser {get;set;}

        public int PoolTableId {get;set;}
        public PoolTable PoolTable {get;set;}

    }
}