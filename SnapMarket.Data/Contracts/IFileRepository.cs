using System.Threading.Tasks;
using System.Collections.Generic;
using SnapMarket.ViewModels.File;

namespace SnapMarket.Data.Contracts
{
    public interface IFileRepository
    {
        Task<List<FileViewModel>> GetPaginateFilesAsync(int offset, int limit, string Orderby, string searchText);
        Task InsertMultiImageAsync(string productId, int? sellerId, int? sliderId, List<string> nameImages);
        Task<string> FindImageAsync(string productId, int? sellerId, int? sliderId);
        Task<List<string>> GetImagesAsync(string productId, int? sellerId, int? sliderId);
    }
}
