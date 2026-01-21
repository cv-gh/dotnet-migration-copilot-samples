using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Data
{
    /// <summary>
    /// Legacy factory for creating SchoolContext instances.
    /// This is no longer used in ASP.NET Core where DbContext is injected via DI.
    /// Kept for reference only.
    /// </summary>
    [System.Obsolete("This factory is not used in ASP.NET Core. Use dependency injection instead.")]
    public static class SchoolContextFactory
    {
        public static SchoolContext Create()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var optionsBuilder = new DbContextOptionsBuilder<SchoolContext>();
            optionsBuilder.UseSqlServer(connectionString);
            
            return new SchoolContext(optionsBuilder.Options);
        }
    }
}
