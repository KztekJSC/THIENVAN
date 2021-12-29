using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Model.Models
{
    [Table("tbl_Event")]
    public class tbl_Event
    {
        [Key]
        public string id { get; set; }

        public int event_Code { get; set; }

        public int event_Number { get; set; }

        public string plate_Number1 { get; set; }

        public string plate_Number2 { get; set; }

        public int weight { get; set; }

        public string customer_Name { get; set; }

        public string driver_Name { get; set; }

        public string commodity_Name { get; set; }

        public string description { get; set; }

        public string imgPath_Lpr1 { get; set; }

        public string imgPath_Lpr2 { get; set; }

        public string imgPath_Panorama1 { get; set; }

        public string imgPath_Panorama2 { get; set; }

        public DateTime event_DateTime_Gate { get; set; }

        public DateTime event_DateTime_Service { get; set; }

        public DateTime event_DateTime_Warehouse { get; set; }

        public bool abnormal_event { get; set; }


    }

    public class tbl_Event_Submit
    {
        public long RowNumber { get; set; }

        public string id { get; set; }

        public int event_Code { get; set; }

        public int event_Number { get; set; }

        public string plate_Number1 { get; set; }

        public int weight { get; set; }

        public string customer_Name { get; set; }

        public string driver_Name { get; set; }

        public string commodity_Name { get; set; }

        public string description { get; set; }

        public string imgPath_Lpr1 { get; set; }

        public string imgPath_Lpr2 { get; set; }

        public string imgPath_Panorama1 { get; set; }

        public string imgPath_Panorama2 { get; set; }

        public string event_DateTime_Gate { get; set; }

        public string event_DateTime_Service { get; set; }

        public string event_DateTime_Warehouse { get; set; }

        public bool abnormal_event { get; set; }


    }
    public class tbl_Event_Excel {

    public long RowNumber { get; set; }

    public string customer_Name { get; set; }

    public string driver_Name { get; set; }

    public string plate_Number1 { get; set; }

    public string plate_Number2 { get; set; }

    public string commodity_Name { get; set; }

    public DateTime event_DateTime_Gate { get; set; }

    public DateTime event_DateTime_Service { get; set; }

    public DateTime event_DateTime_Warehouse { get; set; }

    public int event_Code { get; set; }

}}
public class tbl_Event_Custom
{

    public long RowNumber { get; set; }

    public string customer_Name { get; set; }

    public string driver_Name { get; set; }

    public string plate_Number1 { get; set; }

    public string plate_Number2 { get; set; }

    public string commodity_Name { get; set; }

    public string event_DateTime_Gate { get; set; }

    public string event_DateTime_Service { get; set; }

    public string event_DateTime_Warehouse { get; set; }
  
    public string event_Code_Name { get; set; }


}