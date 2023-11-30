using ECommerce.Models.Models;

namespace ECommerce.DataAccess.Repository.IRepository;

public interface ICompanyRepository : IRepository<Company>
{
    void Update(Company company);
}