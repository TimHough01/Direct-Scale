using System.Collections.Generic;
using System;

namespace TM3ClientExtension.Models
{
    public class PillarUserModel
    {
        public string Id { get; set; }
        public List<string> ExternalIds { get; set; }
        public int CustomerType { get; set; }
        public DateTime SignupDate { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public List<Address> Addresses { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; }
        public string EmailAddress { get; set; }
        public string Language { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? ProfileImage { get; set; }
        public string WebAlias { get; set; }
        public string? SocialMedia { get; set; }
        public string? CustomData { get; set; }
        public MerchantData MerchantData { get; set; }
        public string ScopeLevel { get; set; }
    }

    public class Address
    {
        public string Type { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string Zip { get; set; }
        public string CountryCode { get; set; }
    }

    public class PhoneNumber
    {
        public string Number { get; set; }
        public string Type { get; set; }
        public string? CountryCode { get; set; }
    }

    public class MerchantData
    {
        public BankAccount BankAccount { get; set; }
    }

    public class BankAccount
    {
        public string CustomerId { get; set; }
        public string MerchantId { get; set; }
        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }
        public string NameOnAccount { get; set; }
        public string Other { get; set; }
        public string? CustomData { get; set; }
    }
}
