using MediatR;
using SearchJobsService.Application.Dto;
using SearchJobsService.Domain.Enum;
using SharedKernel.Interface;

namespace SearchJobsService.Application.Commands
{
    public class ApplyCommand : IRequest<IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        public int IdPublication { get; set; }
        public int IdApplicant { get; set; }
        public string ApplicantName { get; set; }
        public string? ApplicantResume { get; set; }
        public string? CoverLetter { get; set; }
        public DateTime ApplicationDate { get; set; }
        //public RecruitmentStatus Status { get; set; }
        #endregion

        #region Constructor
        public ApplyCommand(JobApplicationRequestDTO applyDto)
        {
            IdPublication = applyDto.IdPublication;
            IdApplicant = applyDto.IdApplicant;
            ApplicantName = applyDto.ApplicantName;
            ApplicantResume = applyDto.ApplicantResume;
            CoverLetter = applyDto.CoverLetter;
            ApplicationDate = DateTime.UtcNow;
            //Status = applyDto.Status;
        }
        #endregion

        #region Methods
        public string GetRecruitmentStatus(RecruitmentStatus status)
        {
            string recruitment = string.Empty;
            switch (status)
            {
                case RecruitmentStatus.Without_Applying:
                    recruitment = RecruitmentStatus.Without_Applying.ToString().ToLower();
                    break;
                case RecruitmentStatus.Applied:
                    recruitment = RecruitmentStatus.Applied.ToString().ToLower();
                    break;
                case RecruitmentStatus.In_Review:
                    recruitment = RecruitmentStatus.In_Review.ToString().ToLower();
                    break;
                case RecruitmentStatus.Offer_Accepted:
                    recruitment = RecruitmentStatus.Offer_Accepted.ToString().ToLower();
                    break;
                case RecruitmentStatus.Offer_Declined:
                    recruitment = RecruitmentStatus.Offer_Declined.ToString().ToLower();
                    break;
                case RecruitmentStatus.Rejected:
                    recruitment = RecruitmentStatus.Rejected.ToString().ToLower();
                    break;
                case RecruitmentStatus.Withdrawn:
                    recruitment = RecruitmentStatus.Withdrawn.ToString().ToLower();
                    break;
            }
            return recruitment;
        }
        #endregion
    }
}
