using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Model.Models
{
    [Table("tbl_Lane_Led")]
    public class tbl_Lane_Led
    {
        [Key]
        public string id { get; set; }
        public string lane_ID { get; set; }
        public string LED_ID { get; set; }
        public int LEDDirection { get; set; }
    }

    public class tbl_Lane_Led_Custom
    {

        public string id { get; set; }
        public string lane_ID { get; set; }
        public string LED_Name { get; set; }
        public string LED_ID { get; set; }
        public int LEDDirection { get; set; }

    }
}
