using System.ComponentModel.DataAnnotations;

namespace PublicationsService.Aplication.DTO
{
    public class PublicationDTO
    {
        [Required, Range(1, int.MaxValue)]
        public int IdUser { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int IdRole { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime ExpirationDate { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int Status { get; set; }

        [Required, Range(1, int.MaxValue)]
        public decimal Salary { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Company { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int IdJobType { get; set; }
    }
}

