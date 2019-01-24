using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace CdsaContentManager.Models {

  public class CdsaInstance {
    public bool IsUserSysAdmin { get; set; }
    public string Region { get; set; }
    public string Purpose { get; set; }
    public int StatusMessage { get; set; }
    public DateTime TrialExpirationDate { get; set; }
    public int OrganizationType { get; set; }
    public string Id { get; set; }
    public string UniqueName { get; set; }
    public string UrlName { get; set; }
    public string FriendlyName { get; set; }
    public int State { get; set; }
    public string Version { get; set; }
    public string Url { get; set; }
    public string ApiUrl { get; set; }
    public DateTime LastUpdated { get; set; }
  }

  public class CdsaInstancesResult {
    public List<CdsaInstance> value { get; set; }
  }


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

  public class AccountsResult {
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

  public class ContactsResult{
    public List<Contact> value { get; set; }
  }

  public class LocalizedLabel {
    public string Label { get; set; }
    public int LanguageCode { get; set; }
    public bool IsManaged { get; set; }
    public string MetadataId { get; set; }
    public object HasChanged { get; set; }
  }
  
  public class LocalizedLabelSet {
    public List<LocalizedLabel> LocalizedLabels { get; set; }
    public LocalizedLabel UserLocalizedLabel { get; set; }
  }

  
  public class IsAuditEnabled {
    public bool Value { get; set; }
    public bool CanBeChanged { get; set; }
    public string ManagedPropertyLogicalName { get; set; }
  }

  public class EntityDefinition {
    public int ActivityTypeMask { get; set; }
    public bool IsActivity { get; set; }
    public bool IsActivityParty { get; set; }
    public bool IsChildEntity { get; set; }
    public bool IsCustomEntity { get; set; }
    public bool IsManaged { get; set; }
    public string LogicalName { get; set; }
    public int ObjectTypeCode { get; set; }
    public string OwnershipType { get; set; }
    public string PrimaryNameAttribute { get; set; }
    public string PrimaryIdAttribute { get; set; }
    public string SchemaName { get; set; }
    public string LogicalCollectionName { get; set; }
    public string CollectionSchemaName { get; set; }
    public string EntitySetName { get; set; }
    public bool HasActivities { get; set; }
    public LocalizedLabelSet Description { get; set; }
    public LocalizedLabelSet DisplayCollectionName { get; set; }
    public LocalizedLabelSet DisplayName { get; set; }
    public IsAuditEnabled IsAuditEnabled { get; set; }
  }

  public class EntityDefinitionsResult {
    public List<EntityDefinition> value { get; set; }
  }

  public class Publisher {
    public string _organizationid_value { get; set; }
    public string uniquename { get; set; }
    public string friendlyname { get; set; }
    public string publisherid { get; set; }
    public int customizationoptionvalueprefix { get; set; }
    public string customizationprefix { get; set; }
    public DateTime createdon { get; set; }
    public object description { get; set; }
    public object supportingwebsiteurl { get; set; }
  }

  public class PublishersResult {
    public List<Publisher> value { get; set; }
  }

}