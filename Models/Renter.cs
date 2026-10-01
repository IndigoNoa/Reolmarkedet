
using System;
using System.Collections.Generic;
using System.Text;

namespace Reolmarkedet.Models
{
    // Lejer klasse representerer lejeren som en enhed
    public class Renter
    {
        // Properties til lagring af lejerens oplysninger
        public string Name { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public int DesiredShelfCount { get; set; } // Ønsked antal reoler

    // konstruktør
    public Renter(string name, string address, string phone, string email, int desiredShelfCount)
    {
        Name = name;
        Address = address;
        Phone = phone;
        Email = email;
        DesiredShelfCount = desiredShelfCount;
    }
    }
}