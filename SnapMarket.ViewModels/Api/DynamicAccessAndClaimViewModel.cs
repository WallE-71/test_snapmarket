using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace SnapMarket.ViewModels.Api
{
    public class DynamicAccessAndClaimViewModel
    {
        [JsonPropertyName("UserId")]
        public int UserId { get; set; }


        [JsonPropertyName("نوع کلیم"), JsonIgnore]
        public string UserClaimType { get; set; } = "DynamicPermission";


        [/*JsonPropertyName("مقدار کلیم"),*/ Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string UserClaimValue { get; set; }
    }
}
