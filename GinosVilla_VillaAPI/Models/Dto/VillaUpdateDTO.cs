﻿using System.ComponentModel.DataAnnotations;

namespace GinosVilla_VillaAPI.Models.Dto
{
    public class VillaUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required!!!! Custom Message Error")] // Makes the Field Required with custom error message when is being validated
        [StringLength(30)]
        [MaxLength(30)]        
        public string Name { get; set; }

        public string Details { get; set; }
        [Required]
        public double Rate { get; set; }
        [Required]
        public int Occupancy { get; set; }
        [Required]
        public int Sqft { get; set; }        
        public string? ImageUrl { get; set; }
        public string? ImageLocalPath { get; set; }
        public IFormFile? Image { get; set; }
        public string Amenity { get; set; }
    }
}
