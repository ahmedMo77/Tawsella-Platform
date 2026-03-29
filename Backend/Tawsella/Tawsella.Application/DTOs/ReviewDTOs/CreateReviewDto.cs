using System.ComponentModel.DataAnnotations;

namespace Tawsella.Application.DTOs.ReviewDTOs
{
    public class CreateReviewDto
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [MaxLength(1000)]
        public string Comment { get; set; }
    }
}