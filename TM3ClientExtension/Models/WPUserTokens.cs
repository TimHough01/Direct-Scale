namespace TM3ClientExtension.Models
{
    public class  WPUserTokensrequest
    {
        public string gateway_id { get; set; }
        public string token { get; set;}
        public string user_id { get; set; }
        public string type { get; set; }
        public bool is_default { get; set; }
    }
    public class WPUserTokens
    {
        public string DirectScale_ID { get; set; }
        public string Customer_vault_ID { get; set; }
        //public string user_id { get; set; }
        //public string type { get; set; }
        //public bool is_default { get; set; }
    }
}
