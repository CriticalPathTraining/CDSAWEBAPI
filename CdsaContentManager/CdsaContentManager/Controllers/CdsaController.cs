using CdsaContentManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CdsaContentManager.Controllers {

  [Authorize]
  public class CdsaController : Controller {


    public async Task<ActionResult> Environments() {
      return View(await CdsaManager.GetDiscoveredInstances());
    }


    public async Task<ActionResult> EntityDefinitions() {
      return View(await CdsaManager.GetCustomEntityDefinitions());
    }

    public async Task<ActionResult> Publishers() {
      return View(await CdsaManager.GetPublishers());
    }

    public async Task<ActionResult> Accounts() {
      return View(await CdsaManager.GetAccounts());
    }

    public async Task<ActionResult> AddAccount() {
      await CdsaManager.AddAccounts(1);
      return Redirect("Accounts");
    }

    public async Task<ActionResult> Add5Accounts() {
      await CdsaManager.AddAccounts(5);
      return Redirect("Accounts");
    }

    public async Task<ActionResult> DeleteAccounts() {
      await CdsaManager.DeleteAllAccounts();
      return Redirect("Accounts");
    }



    public async Task<ActionResult> Contacts() {
      return View(await CdsaManager.GetContacts());
    }

    public async Task<ActionResult> AddContact() {
      await CdsaManager.AddContacts(1);
      return Redirect("Contacts");
    }

    public async Task<ActionResult> Add10Contacts() {
      await CdsaManager.AddContacts(10);
      return Redirect("Contacts");
    }

    public async Task<ActionResult> DeleteContacts() {
      await CdsaManager.DeleteAllContacts();
      return Redirect("Contacts");
    }
    
  }
}