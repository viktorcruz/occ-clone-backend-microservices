using System.ComponentModel;

namespace SharedKernel.Enum
{
    public enum RecruitmentStatus
    {
        [Description("The candidate has not applied")]
        WithoutApplying = 0,

        [Description("The candidate has sent an application")]
        Applied = 1,

        [Description("The application is under review by the recruiter")]
        InReview = 2,

        [Description("The candidate has accepted the offer")]
        OfferAccepted = 3,

        [Description("The candidate has declined the offer")]
        OfferDeclined = 4,

        [Description("The application has been rejected by the recruiter")]
        Rejected = 5,

        [Description("The candidate has withdrawn from the process")]
        Withdrawn = 6,
    }
}
