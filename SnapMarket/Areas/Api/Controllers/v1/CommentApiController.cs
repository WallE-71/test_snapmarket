using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SnapMarket.Entities;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.Services.Contracts;
using SnapMarket.ViewModels.Comments;
using SnapMarket.Common.Api.Attributes;
using Microsoft.AspNetCore.Http;

namespace SnapMarket.Areas.Api.Controllers.v1
{
    [DisplayName("CommentApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class CommentApiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uw;
        private readonly IHttpContextAccessor _accessor;
        private readonly IApplicationUserManager _userManager;
        public CommentApiController(IUnitOfWork uw, IMapper mapper, IHttpContextAccessor accessor, IApplicationUserManager userManager)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
            _accessor = accessor;
            _accessor.CheckArgumentIsNull(nameof(_accessor));
            _userManager = userManager;
            _userManager.CheckArgumentIsNull(nameof(_userManager));
        }

        [HttpPost]
        public async Task<object> SendComment(string name, string email, string description, string productId, int? parentCommentId)
        {
            var viewModel = new CommentViewModel();
            if (name.HasValue() && email.HasValue() && description.HasValue())
            {
                int maxId;
                var comments = await _uw.BaseRepository<Comment>().FindAllAsync();
                if (comments.Count() != 0)
                    maxId = comments.OrderByDescending(c => c.Id).First().Id + 1;
                else
                    maxId = 1;
                viewModel.Id = maxId;
                viewModel.Name = name;
                viewModel.Email = email;
                viewModel.ProductId = productId;
                viewModel.Description = description;
                viewModel.ParentCommentId = parentCommentId == 0 ? null : parentCommentId;
                viewModel.InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now);
                await _uw.BaseRepository<Comment>().CreateAsync(_mapper.Map<Comment>(viewModel));
                await _uw.Commit();
                return Ok("با تشکر از بیان نظر ,دیدگاه شما بعد از تایید در سایت نمایش داده می شود.");
            }
            return BadRequest(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> LikeOrDisLike(int commentId, string browserId, bool isLiked)
        {
            var likeOrDislike = _uw.BaseRepository<Like>().FindByConditionAsync(l => l.CommentId == commentId && l.BrowserId == browserId).Result.FirstOrDefault();
            if (likeOrDislike == null)
            {
                likeOrDislike = new Like { CommentId = commentId, BrowserId = browserId, IsLiked = isLiked };
                await _uw.BaseRepository<Like>().CreateAsync(likeOrDislike);
            }
            else
                likeOrDislike.IsLiked = isLiked;

            await _uw.Commit();
            var likeAndDislike = _uw.CommentRepository.NumberOfLikeAndDislike(commentId);
            return Ok(new { like = likeAndDislike.NumberOfLike, dislike = likeAndDislike.NumberOfDisLike });
        }
    }
}
