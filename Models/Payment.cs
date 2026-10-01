using System;
using System.Collections.Generic;
using System.Text;
using Reolmarkedet.ViewModels;

namespace Reolmarkedet.Models
{
    public class Payment
    {
		public string PaymentID { get; set; }
		public PaymentMethod PaymentMethod { get; set; }   // Kontant eller MobilePay (samme enum som i VM_Checkout)
		public decimal AmountPaid { get; set; }
		public DateTime PaymentDate { get; set; }
	}
}
