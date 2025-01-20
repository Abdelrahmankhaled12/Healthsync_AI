using System.ComponentModel.DataAnnotations.Schema;

namespace HEALTH_SYC.Models
{
    public class Symptom_of_disease
    {
        public int Id { get; set; }

        [ForeignKey("Symptom")]
        public int SymptomId { get; set; }

        [ForeignKey("Disease")]
        public int DiseaseId { get; set; }
      
    }
}
