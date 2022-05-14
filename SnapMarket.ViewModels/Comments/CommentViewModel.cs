using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using SnapMarket.Entities;

namespace SnapMarket.ViewModels.Comments
{
    public class CommentViewModel : BaseViewModel<int>
    {
        public CommentViewModel() { }
        public CommentViewModel(int parentCommentId, string productId)
        {
            ProductId = productId;
            ParentCommentId = parentCommentId;
        }

        [Display(Name = "ایمیل"), Required(ErrorMessage = "وارد نمودن {0} الزامی است."), EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نمی باشد.")]
        public string Email { get; set; }

        [JsonIgnore]
        public string ProductId { get; set; }
        public string ProductName { get; set; }

        public int NumberOfLike { get; set; }

        public int NumberOfDisLike { get; set; }

        [JsonIgnore]
        public int? ParentCommentId { get; set; }

        public virtual ICollection<CommentViewModel> SubComments { get; set; }
    }
}
