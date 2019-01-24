using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CdsaSampleDataGenerator.Models {
  class CdsaSampleData {

    public static Account[] Accounts = {
      new Account{ name="Acme Corp", accountnumber="Acc01", address1_line1="123 Main Street", address1_city="Tampa", address1_stateorprovince="FL", address1_postalcode="33629", telephone1="818 555 2222",
          contacts = {
            new Contact{ firstname="Chuck", lastname="Sterling", emailaddress1="chuck@bigdaddy.com", mobilephone="508 333 2222", telephone1="508 555 1111" },
            new Contact{ firstname="Ted", lastname="Pattison", emailaddress1="ted@ted.ted", mobilephone="813 555 3333", telephone1="813 777 3333" },
            new Contact{ firstname="Shane", lastname="Young", emailaddress1="shane@crckers.com", mobilephone="848 555 7777", telephone1="812 444 5555" }
        }
      },
       new Account{ name="Mega Corp", accountnumber="Meg01", address1_line1="234 OakStreet", address1_city="Odessa", address1_stateorprovince="FL", address1_postalcode="33633", telephone1="818 444 2222",
          contacts = {
            new Contact{ firstname="Magic", lastname="Johnson", emailaddress1="magig@bigdaddy.com", mobilephone="508 333 2222", telephone1="508 555 1111" },
            new Contact{ firstname="Bill", lastname="Russel", emailaddress1="ted@ted.ted", mobilephone="813 555 3333", telephone1="813 777 3333" },
            new Contact{ firstname="Larry", lastname="Bird", emailaddress1="larry@crckers.com", mobilephone="848 555 7777", telephone1="812 444 5555" }
        }
      },
        new Account{ name="Evil Corp", accountnumber="Evil01", address1_line1="345 Pine Street", address1_city="Tampa", address1_stateorprovince="FL", address1_postalcode="33629", telephone1="818 555 2222",
          contacts = {
            new Contact{ firstname="Sam", lastname="Malone", emailaddress1="chuck@bigdaddy.com", mobilephone="508 333 2222", telephone1="508 555 1111" },
            new Contact{ firstname="Ernie", lastname="Banks", emailaddress1="ted@ted.ted", mobilephone="813 555 3333", telephone1="813 777 3333" },
            new Contact{ firstname="Franco", lastname="Ballena", emailaddress1="shane@crckers.com", mobilephone="848 555 7777", telephone1="812 444 5555" }
        }
      }
    };

  }
}
