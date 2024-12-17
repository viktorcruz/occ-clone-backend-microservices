using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Events.JobSearch
{
    public class JobSearchApplyEvent
    {
        public int IdPublication { get; set; }
        public int IdApplicant { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantResume { get; set; }
        public string CoverLetter { get; set; }
        public DateTime ApplicationDate { get; set; }
        public int Status { get; set; }
    }
}
