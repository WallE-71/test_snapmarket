using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace SnapMarket.ViewModels
{
    public class MessageUsersViewModel : BaseViewModel<int>
    {
        [JsonIgnore]
        public long UserId { get; set; }

        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Display(Name = "نوع پیام")]
        public Entities.MessageType Messages { get; set; }

        [Display(Name = "نوع پیام")]
        public string Message { get; set; }

        [Display(Name = "متن پاسخ"), Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string Answer { get; set; }

        [Display(Name = "پاسخ دهنده")]
        public string AnswerAuthor { get; set; }

        [JsonIgnore]
        public string MessageTargetRole { get; set; }
    }
}
