using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CdsaContentManager.Models {

  public class ApplicationDbContext : DbContext {

    public ApplicationDbContext()
        : base("DefaultConnection") {
      Database.SetInitializer(new ApplicationDbInitializer());
    }

    public DbSet<UserTokenCache> UserTokenCacheList { get; set; }
  }

  public class UserTokenCache {
    [Key]
    public int UserTokenCacheId { get; set; }
    public string webUserUniqueId { get; set; }
    public byte[] cacheBits { get; set; }
    public DateTime LastWrite { get; set; }
  }

  public class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext> {
    protected override void Seed(ApplicationDbContext context) {
      base.Seed(context);

    }
  }
}
