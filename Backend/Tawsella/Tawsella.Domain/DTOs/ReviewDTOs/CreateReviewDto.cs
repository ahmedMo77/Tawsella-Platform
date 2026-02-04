using System.ComponentModel.DataAnnotations;

namespace Tawsella.Domain.DTOs.ReviewDTOs
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