using System.ComponentModel.DataAnnotations;

namespace SnapMarket.ViewModels.Api.SignIn
{
    public class CustomerDto
    {
        public long id { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string phoneNumber { get; set; }
    }
}
