using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CdsaSampleDataGenerator.Models {

  public class Account {
    public const string OdataGetUrl = "accounts";
    public string name { get; set; }
    public string accountid { get; set; }
    public string accountnumber { get; set; }
    public string telephone1 { get; set; }
    public string address1_line1 { get; set; }
    public string address1_city { get; set; }
    public string address1_stateorprovince { get; set; }
    public string address1_postalcode { get; set; }
    public string address1_primarycontactname { get; set; }
    public string businesstypecode { get; set; }
    public string creditlimit { get; set; }
    [JsonProperty("primarycontactid@odata.bind")]
    public string primarycontactid { get; set; }
    [JsonIgnoreAttribute]
    public List<Contact> contacts = new List<Contact>();
  }

  public class AccountsCollection {
    public List<Account> value { get; set; }
  }

  public class Contact {

    public const string OdataGetUrl = "contacts/?$select=contactid,firstname,lastname,company,telephone1,mobilephone,emailaddress1,birthdate";
    public string contactid { get; set; }
    public string firstname { get; set; }
    public string lastname { get; set; }
    public string mobilephone { get; set; }
    public string company { get; set; }
    public string emailaddress1 { get; set; }
    public string address1_line1 { get; set; }
    public string address1_city { get; set; }
    public string address1_stateorprovince { get; set; }
    public string address1_postalcode { get; set; }
    public string telephone1 { get; set; }
    public string birthdate { get; set; }
    public string gendercode { get; set; }
    [JsonProperty("parentcustomerid_account@odata.bind")]
    public string parentcustomerid_account { get; set; }
  }


  public class ContactsCollection {
    public List<Contact> value { get; set; }
  }


}
