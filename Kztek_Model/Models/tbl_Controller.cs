using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Model.Models
{
    [Table("tbl_Controller")]
  public  class tbl_Controller
    {
        [Key]
        public string id { get; set; }

        public string controller_Name { get; set; }

        public int comm_Type { get; set; }

        public string com_Port { get; set; }

        public string baud_Rate { get; set; }

        public string controller_Code { get; set; }

        public int controller_Address { get; set; }

        public int controller_Type { get; set; }

        public int Readers_Number { get; set; }

        public int Inputs_Number { get; set; }

        public int Relays_Number { get; set; }

        public string description { get; set; }
        public bool Inactive { get; set; }

    }
}
