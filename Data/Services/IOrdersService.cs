using CapstoneProject_.NETFSD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CapstoneProject_.NETFSD.Data.Services
{
    public interface IOrdersService
    {
        Task StoreOrder(List<ShoppingCartItem> items, string userId, string userEmailAddress);
        Task<List<Order>> GetOrdersByUserIdAndRole(string userId, string userRole);

    }                                                       
}
