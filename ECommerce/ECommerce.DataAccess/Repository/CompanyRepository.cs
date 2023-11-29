using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;

namespace ECommerce.DataAccess.Repository
{
    public class CompanyRepository(ApplicationDbContext context) : Repository<Company>(context), ICompanyRepository
    {
        public void Update(Company company)
        {
            _context.Update(company);
        }
    }
}