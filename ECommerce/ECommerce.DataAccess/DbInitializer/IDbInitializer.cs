using ECommerce.Models.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DataAccess.Dbinitializer;

public interface IDbInitializer
{
    void Initialize();
}