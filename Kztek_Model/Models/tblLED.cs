using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Model.Models
{
    [Table("tbl_LED")]
    public  class tblLED
    {
        [Key]
        public string ID { get; set; }
        public int Sort { get; set; }
        public string Name { get; set; }

        public string Code { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public int CommunicationType { get; set; }
        public string IP { get; set; }

        public int Port { get; set; }
        public int Address { get; set; }
        public int Arrow { get; set; }
        public int Color { get; set; }
        public int ZeroColor { get; set; }
    }
    public class tblLED_Submit
    {
        public string ID { get; set; }
        public int Sort { get; set; }
        public string Name { get; set; }

        public string Code { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public int CommunicationType { get; set; }
        public string IP { get; set; }

        public int Port { get; set; }
        public int Address { get; set; }
        public int Arrow { get; set; }
        public int Color { get; set; }
        public int ZeroColor { get; set; }

    }
}
