using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SnapMarket.Entities;
using SnapMarket.Data.Contracts;
using SnapMarket.ViewModels.File;
using SnapMarket.Common.Extensions;

namespace SnapMarket.Data.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly SnapMarketDBContext _context;
        public FileRepository(SnapMarketDBContext context)
        {
            _context = context;
        }

        public async Task<List<FileViewModel>> GetPaginateFilesAsync(int offset, int limit, string Orderby, string searchText)
        {
            var files = await _context.FileStores
                                   .OrderBy(Orderby).Skip(offset).Take(limit)
                                   .Select(f => new FileViewModel
                                   {
                                       ImageName = f.ImageName,
                                       SellerName = _context.Sellers.FirstOrDefault(s => s.Id == f.SellerId).Name,
                                       ProductName = _context.Products.FirstOrDefault(p => p.Id == f.ProductId).Name,
                                   }).AsNoTracking().ToListAsync();

            foreach (var item in files)
                item.Row = ++offset;
            return files;
        }

        public async Task InsertMultiImageAsync(string productId, int? sellerId, int? sliderId, List<string> nameImages)
        {
            foreach (var item in nameImages)
            {
                var file = new FileStore
                {
                    ImageName = item,
                    SliderId = sliderId,
                    SellerId = sellerId,
                    ProductId = productId,
                    InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now)
                };
                await _context.FileStores.AddAsync(file);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> FindImageAsync(string productId, int? sellerId, int? sliderId)
        {
            if (productId != null)
            {
                var fileStore = await _context.FileStores.FirstOrDefaultAsync(f => f.ProductId == productId);
                if (fileStore != null)
                {
                    var check = !fileStore.ImageName.Contains($"product-{productId}-");
                    if (check)
                        return fileStore.ImageName.Trim().Replace(" ", "");
                }
            }
            if (sellerId != 0 && sellerId != null)
            {
                var fileStore = await _context.FileStores.FirstOrDefaultAsync(f => f.SellerId == sellerId);
                if (fileStore != null)
                {
                    var check = !fileStore.ImageName.Contains($"seller-{sellerId}-");
                    if (check)
                        return fileStore.ImageName.Trim().Replace(" ", "");
                }
            }
            if (sliderId != 0 && sliderId != null)
            {
                var fileStore = await _context.FileStores.FirstOrDefaultAsync(f => f.SliderId == sliderId);
                if (fileStore != null)
                {
                    var check = !fileStore.ImageName.Contains($"slider-{sliderId}-");
                    if (check)
                        return fileStore.ImageName.Trim().Replace(" ", "");
                }
            }
            return null;
        }

        public async Task<List<string>> GetImagesAsync(string productId, int? sellerId, int? sliderId)
        {
            var images = new List<string>();
            if (productId != null)
            {
                var fileStores = await _context.FileStores.Where(f => f.ProductId == productId).ToListAsync();
                foreach (var fileStore in fileStores)
                    images.Add(fileStore.ImageName.Trim().Replace(" ", ""));
            }
            if (sellerId != 0 && sellerId != null)
            {
                var fileStores = await _context.FileStores.Where(f => f.SellerId == sellerId).ToListAsync();
                foreach (var fileStore in fileStores)
                    images.Add(fileStore.ImageName.Trim().Replace(" ", ""));
            }
            if (sliderId != 0 && sliderId != null)
            {
                var fileStores = await _context.FileStores.Where(f => f.SliderId == sliderId).ToListAsync();
                foreach (var fileStore in fileStores)
                    images.Add(fileStore.ImageName.Trim().Replace(" ", ""));
            }
            if (images.Count != 0)
                return images;
            return new List<string>();
        }
    }
}
