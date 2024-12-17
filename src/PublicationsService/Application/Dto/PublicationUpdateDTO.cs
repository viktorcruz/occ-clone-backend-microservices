using System.ComponentModel.DataAnnotations;

namespace PublicationsService.Application.DTO
{
    public class PublicationUpdateDTO
    {
        [Required, Range(1, int.MaxValue)]
        public int IdPublication { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int IdUser { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int IdRole { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required, Range(0, 6)]
        public int Status { get; set; }

        [Required]
        public decimal Salary { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Company { get; set; }
    }
}
