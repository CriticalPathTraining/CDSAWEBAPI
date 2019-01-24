using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CdsaSampleDataGenerator.Models {

  class CdsaManager {
    // 
    const string cdsaInstanceId = "org4bbc0847";

    const string clientId = "0a36422b-60fc-462b-9cfb-4bbfdefb6147";
    static readonly Uri redirectUri = new Uri("https://localhost/app1234");

    #region "Grungy stuff"

    const string aadAuthorizationEndpoint = "https://login.windows.net/common";

    const string cdsaInstanceUrl = "https://" + cdsaInstanceId + ".crm.dynamics.com";
    const string cdsDiscoveryUrl = "https://globaldisco.crm.dynamics.com/";
    const string cdsaWebApiBaseUrl = cdsaInstanceUrl + "/api/data/v9.1/";

    static readonly JsonSerializerSettings jsonSerializationSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

    static string GetAccessToken(string resourceUri = cdsaInstanceUrl) {

      var authContext = new AuthenticationContext(aadAuthorizationEndpoint);
      var promptBehavior = new PlatformParameters(PromptBehavior.SelectAccount);
      AuthenticationResult result =
        authContext.AcquireTokenAsync(resourceUri, clientId, redirectUri, promptBehavior).Result;
      return result.AccessToken;
    }

    private static string cachedAccessToken = "";

    static string GetAccessTokenWithInteractiveUserLogin(string resourceUri) {
      var authContext = new AuthenticationContext(aadAuthorizationEndpoint);
      var promptBehavior = new PlatformParameters(PromptBehavior.SelectAccount);
      AuthenticationResult result =
        authContext.AcquireTokenAsync(resourceUri, clientId, redirectUri, promptBehavior).Result;
      return result.AccessToken;
    }

    static string GetAccessTokenUnattended(string resourceUri = cdsaInstanceUrl) {
      if (cachedAccessToken.Equals("")) {
        string userName = "student@cpt2019.onMicrosoft.com";
        string userPassword = "Pa$$word!";
        var authContext = new AuthenticationContext(aadAuthorizationEndpoint);
        var userPasswordCredential = new UserPasswordCredential(userName, userPassword);
        AuthenticationResult result =
          authContext.AcquireTokenAsync(resourceUri, clientId, userPasswordCredential).Result;
        cachedAccessToken = result.AccessToken;
      }
      return cachedAccessToken;
    }

    static string ExecuteGetRequest(string restUrl, string resourceUri = cdsaInstanceUrl) {
      HttpClient client = new HttpClient();
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, restUrl);
      request.Headers.Add("Authorization", "Bearer " + GetAccessTokenUnattended(resourceUri));
      request.Headers.Add("Accept", "application/json;odata.metadata=minimal");
      HttpResponseMessage response = client.SendAsync(request).Result;
      if (response.StatusCode != HttpStatusCode.OK) {
        throw new ApplicationException("Error occured calling the CDSA Web API");
      }
      return response.Content.ReadAsStringAsync().Result;
    }

    private static string ExecutePostRequest(string restUri, string postBody) {

      try {
        HttpContent body = new StringContent(postBody);
        body.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + GetAccessTokenUnattended());
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

    private static string ExecutePostRequestForCdsa(string restUri, string postBody) {

      try {
        HttpContent body = new StringContent(postBody);
        body.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + GetAccessTokenUnattended());
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

    private static string ExecutePatchRequest(string restUri, string postBody) {

      try {
        HttpContent body = new StringContent(postBody);
        body.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + GetAccessTokenUnattended());
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

    private static string ExecuteDeleteRequest(string restUri) {
      HttpClient client = new HttpClient();
      client.DefaultRequestHeaders.Add("Accept", "application/json");
      client.DefaultRequestHeaders.Add("Authorization", "Bearer " + GetAccessTokenUnattended());
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

    public static void DisplayDiscoveredInstance() {

      string restUrl = @"https://globaldisco.crm.dynamics.com/api/discovery/v2.0/Instances";
      var json = ExecuteGetRequest(restUrl, cdsDiscoveryUrl);
      FileStream fs = new FileStream(@"..\..\Models\JSON\DiscoveredInstances.json", FileMode.Create);
      StreamWriter writer = new StreamWriter(fs);
      writer.Write(json);
      writer.Flush();
      writer.Close();
      fs.Close();
      Console.WriteLine(json);
      
    }

    public static void DisplayAccounts() {

      string restUrl = cdsaWebApiBaseUrl + "accounts/";
      var json = ExecuteGetRequest(restUrl);

      AccountsCollection accounts = JsonConvert.DeserializeObject<AccountsCollection>(json);
      Console.WriteLine("Count of accounts: " + accounts.value.Count.ToString());
      foreach (var account in accounts.value) {
        Console.WriteLine(account.name);
      }

    }

    public static string AddAccount(Account newAccount) {
      string restUrl = cdsaWebApiBaseUrl + "accounts";
      string jsonRequestBody = JsonConvert.SerializeObject(newAccount, Formatting.None, jsonSerializationSettings);

      string EntityId = ExecutePostRequestForCdsa(restUrl, jsonRequestBody);

      return EntityId;
    }

    public static void AddAccounts(int AccountCount) {

      IEnumerable<CustomerData> customers = RandomCustomerGenerator.GetCustomerList(AccountCount);

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

        CdsaManager.AddAccount(newAccount);
      }
    }

    public static void DeleteAllAccounts() {

      string restUrl = cdsaWebApiBaseUrl + "accounts?$select=accountid";
      var json = ExecuteGetRequest(restUrl);

      AccountsCollection accounts = JsonConvert.DeserializeObject<AccountsCollection>(json);
      foreach (var account in accounts.value) {
        string deleteUrl = cdsaWebApiBaseUrl + "accounts(" + account.accountid + ")";
        ExecuteDeleteRequest(deleteUrl);
      }

    }

    public static void DisplayContacts() {

      string restUrl = cdsaWebApiBaseUrl + "contacts/"; //  Contact.OdataGetUrl;
      var json = ExecuteGetRequest(restUrl);

      ContactsCollection contacts = JsonConvert.DeserializeObject<ContactsCollection>(json);
      Console.WriteLine("Count of contacts: " + contacts.value.Count.ToString());
      foreach (var contact in contacts.value) {
        Console.WriteLine(contact.firstname + " " + contact.lastname);
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

    public static string AddContact(Contact newContact) {
      string restUrl = cdsaWebApiBaseUrl + "contacts";
      string jsonRequestBody = JsonConvert.SerializeObject(newContact, Formatting.None, jsonSerializationSettings);
      return ExecutePostRequestForCdsa(restUrl, jsonRequestBody);
    }

    public static void AddContacts(int CustomerCount) {

      IEnumerable<CustomerData> customers = RandomCustomerGenerator.GetCustomerList(CustomerCount);

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

        CdsaManager.AddContact(newContact);


      }
    }

    public static void DeleteAllContacts() {

      string restUrl = cdsaWebApiBaseUrl + "contacts?$select=contactid";
      var json = ExecuteGetRequest(restUrl);

      ContactsCollection contacts = JsonConvert.DeserializeObject<ContactsCollection>(json);
      foreach (var contact in contacts.value) {
        string deleteUrl = cdsaWebApiBaseUrl + "contacts(" + contact.contactid + ")";
        ExecuteDeleteRequest(deleteUrl);
      }

    }

    public static void CreateAcmeProductEntity() {

      string deleteUrl = cdsaWebApiBaseUrl + "EntityDefinitions(LogicalName%20%3D%20'cpt_acmeproduct')";
      ExecuteDeleteRequest(deleteUrl);

      string restUrl = cdsaWebApiBaseUrl + "/EntityDefinitions";
      string jsonRequestBody = Properties.Resources.ProductEntityDefinition_json;
      var json = ExecutePostRequest(restUrl, jsonRequestBody);

    }

    public static void PopulateCdsa() {

      DeleteAllContacts();
      DeleteAllAccounts();

      foreach (Account account in CdsaSampleData.Accounts) {
        string accountEntityPath = AddAccount(account);
        string accountEntityPathRelative = accountEntityPath.Substring(accountEntityPath.LastIndexOf('/'));
        bool PrimaryContactAssigned = false;
        foreach (Contact contact in account.contacts) {
          contact.parentcustomerid_account = accountEntityPathRelative;
          string contactEntityPath = AddContact(contact);
          if (!PrimaryContactAssigned) {
            var contactEntityPathRelative = contactEntityPath.Substring(contactEntityPath.LastIndexOf('/'));
            Account updatedAccount = new Account { primarycontactid = contactEntityPathRelative };
            string jsonRequestBody = JsonConvert.SerializeObject(updatedAccount, Formatting.None, jsonSerializationSettings);
            ExecutePatchRequest(accountEntityPath, jsonRequestBody);
          }
        }
      }
    }

    public static void DisplayEntityDefinitions() {

      string restUrl = cdsaWebApiBaseUrl + "EntityDefinitions/"; //  Contact.OdataGetUrl;
      var json = ExecuteGetRequest(restUrl);
      Console.WriteLine(json);
      FileStream fs = new FileStream(@"..\..\Models\JSON\EntityDefinitions.json", FileMode.Create);
      StreamWriter writer = new StreamWriter(fs);
      writer.Write(json);
      writer.Flush();
      writer.Close();
      fs.Close();
      Console.WriteLine(json);
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
