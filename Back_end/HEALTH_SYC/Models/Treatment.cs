using System.ComponentModel.DataAnnotations.Schema;

namespace HEALTH_SYC.Models
{
    public class Treatment
    {
        public int Id { get; set; }

        public string? Description { get; set; }

        [ForeignKey("Disease")]
        public int DiseaseId { get; set; }
    }
}
