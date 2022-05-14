using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.ViewModels.Comments;

namespace SnapMarket.Data.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly SnapMarketDBContext _context;
        public CommentRepository(SnapMarketDBContext context)
        {
            _context = context;
        }

        public int CountUnAnsweredComments() => _context.Comments.Where(c => c.IsComplete == false).Count();

        public async Task<List<CommentViewModel>> GetPaginateCommentsAsync(int offset, int limit, string orderBy, string searchText, string productId, bool? isConfirm)
        {
            var convertConfirm = Convert.ToBoolean(isConfirm);
            var getDateTimesForSearch = searchText.GetDateTimeForSearch();

            var comments = await _context.Comments.Include(c => c.Product)
                                   .Where(n => (isConfirm == null || (convertConfirm == true ? n.IsComplete : !n.IsComplete)) && n.ProductId.Contains(productId) && (n.Name.Contains(searchText)
                                       || n.Email.Contains(searchText) || (n.InsertTime >= getDateTimesForSearch.First() && n.InsertTime <= getDateTimesForSearch.Last())))
                                   .OrderBy(orderBy).Skip(offset).Take(limit)
                                   .Select(c => new CommentViewModel
                                   {
                                       Id = c.Id,
                                       Name = c.Name,
                                       Email = c.Email,
                                       IsComplete = c.IsComplete,
                                       Description = c.Description,
                                       ProductName = c.Product.Name.Length >= 30 ? c.Product.Name.Substring(0, 30) + "..." : c.Product.Name,
                                       PersianInsertTime = c.InsertTime.DateTimeEn2Fa("yyyy/MM/dd ساعت HH:mm:ss"),
                                   }).AsNoTracking().ToListAsync();

            foreach (var item in comments)
                item.Row = ++offset;

            return comments;
        }

        public async Task<List<CommentViewModel>> GetProductCommentsAsync(string productId)
        {
            var comments = await (from c in _context.Comments.Include(c => c.Likes)
                                  where ((c.ParentId == null || c.ParentId == 0) && c.ProductId == productId && c.IsComplete == true)
                                  select new CommentViewModel
                                  {
                                      Id = c.Id,
                                      Name = c.Name,
                                      Email = c.Email,
                                      ProductId = c.ProductId,
                                      Description = c.Description,
                                      ParentCommentId = c.ParentId,
                                      PersianInsertTime = c.InsertTime.ConvertMiladiToShamsi("yyyy/MM/dd ساعت HH:mm:ss"),
                                  }).ToListAsync();

            foreach (var item in comments)
            {
                item.NumberOfLike = NumberOfLikeAndDislike(item.Id).NumberOfLike;
                item.NumberOfDisLike = NumberOfLikeAndDislike(item.Id).NumberOfDisLike;
                await BindSubComments(item);
            }
            return comments;
        }

        public async Task BindSubComments(CommentViewModel viewModel)
        {
            var subComments = await (from c in _context.Comments.Include(p => p.Likes)
                                     where (c.ParentId == viewModel.Id && c.IsComplete == true)
                                     select new CommentViewModel
                                     {
                                         Id = c.Id,
                                         Name = c.Name,
                                         Email = c.Email,
                                         ProductId = c.ProductId,
                                         Description = c.Description,
                                         ParentCommentId = c.ParentId,
                                         PersianInsertTime = c.InsertTime.ConvertMiladiToShamsi("yyyy/MM/dd ساعت HH:mm:ss"),
                                     }).ToListAsync();

            if (viewModel.SubComments != null)
            {
                foreach (var item in subComments)
                {
                    item.NumberOfLike = NumberOfLikeAndDislike(item.Id).NumberOfLike;
                    item.NumberOfDisLike = NumberOfLikeAndDislike(item.Id).NumberOfDisLike;
                    //await BindSubComments(item);
                    viewModel.SubComments.Add(item);
                }
            }
            else
            {
                viewModel.SubComments = new List<CommentViewModel>();
                foreach (var item in subComments)
                {
                    item.NumberOfLike = NumberOfLikeAndDislike(item.Id).NumberOfLike;
                    item.NumberOfDisLike = NumberOfLikeAndDislike(item.Id).NumberOfDisLike;
                    await BindSubComments(item);
                    viewModel.SubComments.Add(item);
                }
            }
        }

        public CommentViewModel NumberOfLikeAndDislike(int commentId)
        {
            return (from c in _context.Comments.Include(p => p.Likes)
                    where (c.Id == commentId)
                    select new CommentViewModel
                    {
                        NumberOfLike = c.Likes.Where(l => l.IsLiked == true).Count(),
                        NumberOfDisLike = c.Likes.Where(l => l.IsLiked == false).Count()

                        //NumberOfLike = c.SubComments.Select(c => c.Likes.Select(l => l.IsLiked == true)).Count(),
                        //NumberOfDisLike = c.SubComments.Select(c => c.Likes.Select(l => l.IsLiked == false)).Count()
                    }).FirstOrDefault();
        }
    }
}
