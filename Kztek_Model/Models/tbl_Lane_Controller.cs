using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Model.Models
{
    [Table("tbl_Lane_Controller")]
    public class tbl_Lane_Controller
    {
        [Key]
        public string id { get; set; }
        public string controller_ID { get; set; }
        public string lane_ID { get; set; }
        public string reader_Index { get; set; }
        public string input_Index { get; set; }
        public string barrie_Index { get; set; }
    }

    public class tbl_Lane_Controller_Custom
    {
        public string id { get; set; }
        public string controller_ID { get; set; }
        public string lane_ID { get; set; }
        public string reader_Index { get; set; }
        public string input_Index { get; set; }
        public string barrie_Index { get; set; }
        public string controller_Name { get; set; }
    }
}
