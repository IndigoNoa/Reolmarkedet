using System;
using System.Collections.Generic;
using System.Text;

namespace Reolmarkedet.Models
{
    public class Shelves
    {
        public string RenterID {  get; set; }
        public string ShelfID { get; set; }
        public string ShelfType { get; set; }
        public string ShelfStatus { get; set; }
        public DateTime? RentalStartDate { get; set; } // Udlejet fra d.
        public DateTime? CancellationDate { get; set; } // Opsigelsesdato
    }
}
