using System.ComponentModel.DataAnnotations;

namespace TM3ClientExtension.Models
{
    public class AutoshipCardDetails
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CardNumberMasked { get; set; }
        public int CardExpiryYear { get; set; }
        public int CardExpiryMonth { get; set; }
        public string NmiCustomerId { get; set; }
        public string CardLast4 { get; set; }
        public string CardType { get; set; }
    }
}
