using System.Security.Claims;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Utility;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApp.ViewComponents;

public class ShoppingCartViewComponent(IUnitOfWork unitOfWork) : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public Task<IViewComponentResult> InvokeAsync()
    {
        var claimsEntity = User.Identity as ClaimsIdentity;
        var userId = claimsEntity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId != null)
        {
            if (HttpContext.Session.GetInt32(SD.SessionCart) == null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId).Count());
            }
            return Task.FromResult<IViewComponentResult>(View(HttpContext.Session.GetInt32(SD.SessionCart)));
        }
        else
        {
            HttpContext.Session.Clear();
            return Task.FromResult<IViewComponentResult>(View(0));
        }
    }
}