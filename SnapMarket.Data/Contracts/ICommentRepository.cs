using System.Threading.Tasks;
using System.Collections.Generic;
using SnapMarket.Entities;
using SnapMarket.ViewModels.Comments;

namespace SnapMarket.Data.Contracts
{
    public interface ICommentRepository
    {
        int CountUnAnsweredComments();
        Task<List<CommentViewModel>> GetPaginateCommentsAsync(int offset, int limit, string orderBy, string searchText, string productId, bool? isConfirm);
        Task<List<CommentViewModel>> GetProductCommentsAsync(string productId);
        CommentViewModel NumberOfLikeAndDislike(int commentId);
    }
}
