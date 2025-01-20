using System.ComponentModel.DataAnnotations.Schema;

namespace HEALTH_SYC.Models
{
    public class Disease_of_patient
    {
        public int Id { get; set; }
        [ForeignKey("Patient")]
        public int PatienId { get; set; }
        [ForeignKey("Disease")]
        public int DiseaseId { get; set; }

    }
}
