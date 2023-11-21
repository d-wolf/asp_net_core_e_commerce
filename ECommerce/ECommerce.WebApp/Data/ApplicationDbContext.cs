using Microsoft.EntityFrameworkCore;

namespace ECommerce.WebApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){

    }
}