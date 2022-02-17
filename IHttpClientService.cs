using System.Collections.Generic;
using System.Threading.Tasks;

namespace AWS.BusinessLogicLayer.Interface
{
    public interface IHttpClientService
    {
        Task<T> GetItem<T>(string route);
        Task<IEnumerable<T>> GetItems<T>(string route);
        Task UpdateItem<T>(T item, string route);
        Task<int> AddItem<T>(T item, string route);
        Task<T> AddItem<T>(string route, T item);
        Task DeleteItem(string route);
        Task<int> PostItem<T>(T item, string route);
    }
}
