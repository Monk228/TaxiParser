using System;
using System.Collections.Generic;
using System.Text;

namespace TaxiParser.Models
{
   public class ParsedTaxi
    {
        public string Status { get; set; }
        public string PermissionNumber { get; set; }
        public string RegisterNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public DateTime EndTime { get; set; }
        public string LicensePlate { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public int ReleaseYear { get; set; }
        public long OGRN { get; set; }
        public long OGRNIP { get; set; }
        public string IPFullName { get; set; }
        public string CompanyName { get; set; }      
        public string Adress { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactDetails { get; set; }
    }
}
