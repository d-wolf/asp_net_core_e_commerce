using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;

namespace ECommerce.DataAccess.Repository
{
    public class OrderHeaderRepository(ApplicationDbContext context) : Repository<OrderHeader>(context), IOrderHeaderRepository
    {
        public void Update(OrderHeader orderHeader)
        {
            _context.Update(orderHeader);
        }

        public void UpdatePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var oderFromDb = _context.OrderHeaders.SingleOrDefault(x => x.Id == id);

            if (!string.IsNullOrEmpty(sessionId))
            {
                oderFromDb!.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                oderFromDb!.PaymentIntentId = paymentIntentId;
                oderFromDb!.PaymentDate = DateTime.UtcNow;
            }
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var oderFromDb = _context.OrderHeaders.SingleOrDefault(x => x.Id == id);
            if (oderFromDb != null)
            {
                oderFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    oderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }
    }
}