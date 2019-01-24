using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Configuration;

namespace CdsaContentManager.Models {

  class CdsaManager {

    #region "Grungy stuff"

    private static string cdsaInstanceId = ConfigurationManager.AppSettings["cdsa-instance-id"];
    private static string cdsaResourceUri = "https://" + cdsaInstanceId + ".crm.dynamics.com";
    private static string cdsaWebApiBaseUrl = cdsaResourceUri + "/api/data/v9.1/";


    private static string aadInstance = "https://login.microsoftonline.com/";
    
    private static string clientId = ConfigurationManager.AppSettings["client-id"];
    private static string appKey = ConfigurationManager.AppSettings["client-secret"];
    private static string replyUrl = ConfigurationManager.AppSettings["reply-url"];

    const string cdsDiscoveryUrl = "https://globaldisco.crm.dynamics.com/";

    static readonly JsonSerializerSettings jsonSerializationSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

    private static async Task<string> GetAccessTokenAsync() {
      return await GetAccessTokenAsync(cdsaResourceUri);
    }

    private static async Task<string> GetAccessTokenAsync(string resourceUri) {

      // determine authorization URL for current tenant
      string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
      string tenantAuthority = aadInstance + tenantID;

      // create ADAL cache object
      ApplicationDbContext db = new ApplicationDbContext();
      string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
      ADALTokenCache userTokenCache = new ADALTokenCache(signedInUserID);

      // create authentication context
      AuthenticationContext authenticationContext = new AuthenticationContext(tenantAuthority, userTokenCache);

      // create client credential object using client ID and client Secret"];
      ClientCredential clientCredential = new ClientCredential(clientId, appKey);

      // create user identifier object for logged on user
      string objectIdentifierId = "http://schemas.microsoft.com/identity/claims/objectidentifier";
      string userObjectID = ClaimsPrincipal.Current.FindFirst(objectIdentifierId).Value;
      UserIdentifier userIdentifier = new UserIdentifier(userObjectID, UserIdentifierType.UniqueId);

      // get access token for Power BI Service API from AAD
      AuthenticationResult authenticationResult =
        await authenticationContext.AcquireTokenSilentAsync(
            resourceUri,
            clientCredential,
            userIdentifier);

      // return access token back to user
      return authenticationResult.AccessToken;

    }

    private static async Task<string> ExecuteGetRequest(string urlRestEndpoint) {
      return await ExecuteGetRequest(urlRestEndpoint, cdsaResourceUri);
    }

    private static async Task<string> ExecuteGetRequest(string urlRestEndpoint, string resourceUri) {

      string accessToken = await GetAccessTokenAsync(resourceUri);

      HttpClient client = new HttpClient();
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, urlRestEndpoint);
      request.Headers.Add("Authorization", "Bearer " + accessToken);
      request.Headers.Add("Accept", "application/json;odata.metadata=minimal");

      HttpResponseMessage response = await client.SendAsync(request);

      if (response.StatusCode != HttpStatusCode.OK) {
        throw new ApplicationException("Error!!!!!");
      }

      return await response.Content.ReadAsStringAsync();
    }


    private static async Task<string> ExecutePostRequest(string restUri, string postBody) {

      string accessToken = await GetAccessTokenAsync();

      try {
        HttpContent body = new StringContent(postBody);
        body.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        HttpResponseMessage response = client.PostAsync(restUri, body).Result;

        if (response.IsSuccessStatusCode) {
          return response.Content.ReadAsStringAsync().Result;
        }
        else {
          Console.WriteLine();
          Console.WriteLine("OUCH! - error occurred during POST REST call");
          Console.WriteLine();
          return string.Empty;
        }
      }
      catch {
        Console.WriteLine();
        Console.WriteLine("OUCH! - error occurred during POST REST call");
        Console.WriteLine();
        return string.Empty;
      }
    }

    private static async Task<string> ExecutePostRequestForCdsa(string restUri, string postBody) {

      try {
        HttpContent body = new StringContent(postBody);
        body.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + await GetAccessTokenAsync());
        HttpResponseMessage response = client.PostAsync(restUri, body).Result;

        if (response.IsSuccessStatusCode) {
          IEnumerable<string> values;
          string EntityId = string.Empty;
          if (response.Headers.TryGetValues("OData-EntityId", out values)) {
            EntityId = values.FirstOrDefault();
          }
          return EntityId;
        }
        else {
          Console.WriteLine();
          Console.WriteLine("OUCH! - error occurred during POST REST call");
          Console.WriteLine();
          return string.Empty;
        }
      }
      catch {
        Console.WriteLine();
        Console.WriteLine("OUCH! - error occurred during POST REST call");
        Console.WriteLine();
        return string.Empty;
      }
    }

    private static async Task<string> ExecutePatchRequest(string restUri, string postBody) {

      string accessToken = await GetAccessTokenAsync();

      try {
        HttpContent body = new StringContent(postBody);
        body.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), restUri);
        request.Content = body;
        HttpResponseMessage response = client.SendAsync(request).Result;

        if (response.IsSuccessStatusCode) {
          return response.Content.ReadAsStringAsync().Result;
        }
        else {
          Console.WriteLine();
          Console.WriteLine("OUCH! - error occurred during REST PATCH call");
          Console.WriteLine();
          return string.Empty;
        }
      }
      catch {
        Console.WriteLine();
        Console.WriteLine("OUCH! - error occurred during POST REST call");
        Console.WriteLine();
        return string.Empty;
      }
    }

    private static async Task<string> ExecuteDeleteRequest(string restUri) {

      string accessToken = await GetAccessTokenAsync();
      HttpClient client = new HttpClient();
      client.DefaultRequestHeaders.Add("Accept", "application/json");
      client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
      HttpResponseMessage response = client.DeleteAsync(restUri).Result;

      if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound) {
        return response.Content.ReadAsStringAsync().Result;
      }
      else {
        Console.WriteLine();
        Console.WriteLine("OUCH! - error occurred during Delete REST call");
        Console.WriteLine();
        return string.Empty;
      }
    }

    #endregion

    public static async Task<List<CdsaInstance>> GetDiscoveredInstances() {
      string restUrl = @"https://globaldisco.crm.dynamics.com/api/discovery/v2.0/Instances";
      string discoveryResourceUri = @"https://globaldisco.crm.dynamics.com/";
      var json = await ExecuteGetRequest(restUrl, discoveryResourceUri);
      CdsaInstancesResult cdsaInstances = JsonConvert.DeserializeObject<CdsaInstancesResult>(json);
      return cdsaInstances.value;
    }

    public static async Task<List<EntityDefinition>> GetEntityDefinitions() {
      string restUrl = cdsaWebApiBaseUrl + "EntityDefinitions/";
      var json = await ExecuteGetRequest(restUrl);
      EntityDefinitionsResult entityDefinitions = JsonConvert.DeserializeObject<EntityDefinitionsResult>(json);
      return entityDefinitions.value;
    }

    public static async Task<List<EntityDefinition>> GetCustomEntityDefinitions() {
      string restUrl = cdsaWebApiBaseUrl + "EntityDefinitions/?$filter=(IsCustomEntity+eq+true)";
      var json = await ExecuteGetRequest(restUrl);
      EntityDefinitionsResult entityDefinitions = JsonConvert.DeserializeObject<EntityDefinitionsResult>(json);
      return entityDefinitions.value;
    }


    public static async Task<List<Publisher>> GetPublishers() {
      string restUrl = cdsaWebApiBaseUrl + "publishers/";
      var json = await ExecuteGetRequest(restUrl);
      PublishersResult publishers = JsonConvert.DeserializeObject<PublishersResult>(json);
      return publishers.value;
    }

    public static async Task<List<Contact>> GetContacts() {
      string restUrl = cdsaWebApiBaseUrl + "contacts/"; //  Contact.OdataGetUrl;
      var json = await ExecuteGetRequest(restUrl);
      ContactsResult contacts = JsonConvert.DeserializeObject<ContactsResult>(json);
      return contacts.value;
    }


    public static async Task<List<Account>> GetAccounts() {
      string restUrl = cdsaWebApiBaseUrl + "accounts/";
      var json = await ExecuteGetRequest(restUrl);
      var accounts = JsonConvert.DeserializeObject<AccountsResult>(json);
      return accounts.value;
    }

    public static async Task<string> AddAccount(Account newAccount) {
      string restUrl = cdsaWebApiBaseUrl + "accounts";
      string jsonRequestBody = JsonConvert.SerializeObject(newAccount, Formatting.None, jsonSerializationSettings);

      string EntityId = await ExecutePostRequestForCdsa(restUrl, jsonRequestBody);

      return EntityId;
    }

    public static async Task AddAccounts(int AccountCount) {

      IEnumerable<CustomerData> customers = RandomCustomerGenerator.GetCustomerList(AccountCount, false);

      foreach (CustomerData customer in customers) {
        Account newAccount = new Account {
          name = customer.Company,
          telephone1 = customer.WorkPhone,
          address1_primarycontactname = customer.FirstName + " " + customer.LastName,
          address1_line1 = customer.Address,
          address1_city = customer.City,
          address1_stateorprovince = customer.State,
          address1_postalcode = customer.ZipCode
        };

        await CdsaManager.AddAccount(newAccount);
      }
    }

    public static async Task DeleteAllAccounts() {

      string restUrl = cdsaWebApiBaseUrl + "accounts?$select=accountid";
      var json = await ExecuteGetRequest(restUrl);

      AccountsResult accounts = JsonConvert.DeserializeObject<AccountsResult>(json);
      foreach (var account in accounts.value) {
        string deleteUrl = cdsaWebApiBaseUrl + "accounts(" + account.accountid + ")";
        await ExecuteDeleteRequest(deleteUrl);
      }
      
    }

  
    public static void DisplayWhoAmIInfo() {

      string restUrl = cdsaWebApiBaseUrl + "WhoAmI()"; //  Contact.OdataGetUrl;
      var json = ExecuteGetRequest(restUrl);

      FileStream fs = new FileStream(@"..\..\Models\JSON\WhoAmI.json", FileMode.Create);
      StreamWriter writer = new StreamWriter(fs);
      writer.Write(json);
      writer.Flush();
      writer.Close();
      fs.Close();
      Console.WriteLine(json);

    }

    public static async Task<string> AddContact(Contact newContact) {
      string restUrl = cdsaWebApiBaseUrl + "contacts";
      string jsonRequestBody = JsonConvert.SerializeObject(newContact, Formatting.None, jsonSerializationSettings);
      return await ExecutePostRequestForCdsa(restUrl, jsonRequestBody);
    }

    public static async Task AddContacts(int CustomerCount) {

      IEnumerable<CustomerData> customers = RandomCustomerGenerator.GetCustomerList(CustomerCount, false);

      foreach (CustomerData customer in customers) {
        Contact newContact = new Contact {
          firstname = customer.FirstName,
          lastname = customer.LastName,
          emailaddress1 = customer.EmailAddress,
          company = customer.Company,
          telephone1 = customer.WorkPhone,
          mobilephone = customer.HomePhone,
          address1_line1 = customer.Address,
          address1_city = customer.City,
          address1_stateorprovince = customer.State,
          address1_postalcode = customer.ZipCode
        };

        await CdsaManager.AddContact(newContact);


      }
    }

    public static async Task DeleteAllContacts() {

      string restUrl = cdsaWebApiBaseUrl + "contacts?$select=contactid";
      var json = await ExecuteGetRequest(restUrl);

      ContactsResult contacts = JsonConvert.DeserializeObject<ContactsResult>(json);
      foreach (var contact in contacts.value) {
        string deleteUrl = cdsaWebApiBaseUrl + "contacts(" + contact.contactid + ")";
        await ExecuteDeleteRequest(deleteUrl);
      }

    }

    public static async Task CreateAcmeProductEntity() {

      string deleteUrl = cdsaWebApiBaseUrl + "EntityDefinitions(LogicalName%20%3D%20'cpt_acmeproduct')";
      await ExecuteDeleteRequest(deleteUrl);

      string restUrl = cdsaWebApiBaseUrl + "/EntityDefinitions";
      string jsonRequestBody = Properties.Resources.ProductEntityDefinition_json;
      var json = ExecutePostRequest(restUrl, jsonRequestBody);

    }

    public static async Task PopulateCdsa() {

      await DeleteAllContacts();
      await DeleteAllAccounts();

      foreach (Account account in CdsaSampleData.Accounts) {
        string accountEntityPath = await AddAccount(account);
        string accountEntityPathRelative = accountEntityPath.Substring(accountEntityPath.LastIndexOf('/'));
        bool PrimaryContactAssigned = false;
        foreach (Contact contact in account.contacts) {
          contact.parentcustomerid_account = accountEntityPathRelative;
          string contactEntityPath = await AddContact(contact);
          if (!PrimaryContactAssigned) {
            var contactEntityPathRelative = contactEntityPath.Substring(contactEntityPath.LastIndexOf('/'));
            Account updatedAccount = new Account { primarycontactid = contactEntityPathRelative };
            string jsonRequestBody = JsonConvert.SerializeObject(updatedAccount, Formatting.None, jsonSerializationSettings);
            await ExecutePatchRequest(accountEntityPath, jsonRequestBody);
          }
        }
      }
    }
  
    public static void DisplayRelationshipDefinitions() {

      string restUrl = cdsaWebApiBaseUrl + "RelationshipDefinitions/"; //  Contact.OdataGetUrl;
      var json = ExecuteGetRequest(restUrl);
      Console.WriteLine(json);
      FileStream fs = new FileStream(@"..\..\Models\JSON\RelationshipDefinitions.json", FileMode.Create);
      StreamWriter writer = new StreamWriter(fs);
      writer.Write(json);
      writer.Flush();
      writer.Close();
      fs.Close();
      Console.WriteLine(json);
    }

    public static void DisplayGlobalOptionSetDefinitions() {

      string restUrl = cdsaWebApiBaseUrl + "GlobalOptionSetDefinitions/"; //  Contact.OdataGetUrl;
      var json = ExecuteGetRequest(restUrl);
      Console.WriteLine(json);
      FileStream fs = new FileStream(@"..\..\Models\JSON\GlobalOptionSetDefinitions.json", FileMode.Create);
      StreamWriter writer = new StreamWriter(fs);
      writer.Write(json);
      writer.Flush();
      writer.Close();
      fs.Close();
      Console.WriteLine(json);
    }

    public static void DisplayPublishers() {

      string restUrl = cdsaWebApiBaseUrl + "publishers";
      var json = ExecuteGetRequest(restUrl);
      Console.WriteLine(json);
      FileStream fs = new FileStream(@"..\..\Models\JSON\Publishers.json", FileMode.Create);
      StreamWriter writer = new StreamWriter(fs);
      writer.Write(json);
      writer.Flush();
      writer.Close();
      fs.Close();

    }

    public static void DisplaySolutions() {

      string restUrl = cdsaWebApiBaseUrl + "solutions";
      var json = ExecuteGetRequest(restUrl);
      Console.WriteLine(json);
      FileStream fs = new FileStream(@"..\..\Models\JSON\Solutions.json", FileMode.Create);
      StreamWriter writer = new StreamWriter(fs);
      writer.Write(json);
      writer.Flush();
      writer.Close();
      fs.Close();

    }

    public static void DisplayAcmeProductEntityDefinition() {

      string restUrl = cdsaWebApiBaseUrl + @"EntityDefinitions(LogicalName%3d'cpt_AcmeProduct')";
      var json = ExecuteGetRequest(restUrl);

      FileStream fs = new FileStream(@"..\..\Models\JSON\ProductEntity.json", FileMode.Create);
      StreamWriter writer = new StreamWriter(fs);
      writer.Write(json);
      writer.Flush();
      writer.Close();
      fs.Close();
      Console.WriteLine(json);

      restUrl = cdsaWebApiBaseUrl + @"EntityDefinitions(LogicalName%3d'cpt_acmeproduct')/Attributes/?$select=LogicalName,DisplayName";
      json = ExecuteGetRequest(restUrl);
      fs = new FileStream(@"..\..\Models\JSON\ProductEntityAttributes.json", FileMode.Create);
      writer = new StreamWriter(fs);
      writer.Write(json);
      writer.Flush();
      writer.Close();
      fs.Close();
      Console.WriteLine(json);



    }


   


  }
}