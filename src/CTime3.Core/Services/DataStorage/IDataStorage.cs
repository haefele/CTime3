using System.Threading.Tasks;

namespace CTime3.Core.Services.DataStorage
{
    public interface IDataStorage
    {
        
        Task SaveChanges();
    }

    public class FileDataStorage : IDataStorage
    {
        public Task SaveChanges()
        {
            return Task.CompletedTask;
        }
    }
}