using System.ComponentModel.DataAnnotations;

namespace SearchJobsService.Application.DTO.Commands
{
    public class WithdrawApplicationRequestDTO
    {
        [Required, Range(1, int.MaxValue)]
        public int IdUser { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int IdPublication { get; set; }
    }
}
