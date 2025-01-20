using System.ComponentModel.DataAnnotations.Schema;

namespace HEALTH_SYC.Models
{
    public class Region
    {
        public int Id { get; set; }
        public string RegionName { get; set; }

        [ForeignKey("Governorate")]
        public int GovernorateId { get; set; }
    }
}
