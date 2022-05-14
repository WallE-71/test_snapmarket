using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SnapMarket.Entities;
using SnapMarket.Common.Api;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.Common.Api.Attributes;

namespace SnapMarket.Areas.Api.Controllers.v1
{
    [ApiController]
    [DisplayName("SellerApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class SellerApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        private readonly IWebHostEnvironment _env;
        public SellerApiController(IUnitOfWork uw, IWebHostEnvironment env)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _env = env;
            _env.CheckArgumentIsNull(nameof(_env));
        }

        [HttpPost]
        public virtual async Task<ApiResult<string>> RequestRegister(string imageFile, string name, string surName, string nationalId, string phoneNumber, string email, string webSite, string brand, int activityType, string store, string telNumber, string address, string postalCode, string establishmentDate, string sampleProduct, string description, string scanNationalIdCart, string scanDocument)
        {
            if (name.HasValue() && surName.HasValue() && phoneNumber.HasValue() && activityType != 0)
            {
                var seller = await _uw.Context.Sellers.FirstOrDefaultAsync(s => s.NationalId == nationalId || s.PhonNumber == phoneNumber);
                if (seller != null)
                    return Ok();

                int maxId;
                var sellers = await _uw.BaseRepository<Seller>().FindAllAsync();
                if (sellers.Count() != 0)
                    maxId = sellers.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
                else
                    maxId = 1;
                seller = new Seller
                {
                    Id = maxId,
                    Name = name,
                    Email = email,
                    Brand = brand,
                    SurName = surName,
                    WebSite = webSite,
                    NationalId = nationalId,
                    PhonNumber = phoneNumber,
                    Description = description,
                    ActivityType = (ActivityType)activityType,
                    InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now)
                };
                await _uw.BaseRepository<Seller>().CreateAsync(seller);

                if (store.HasValue())
                {
                    var newStore = new Store
                    {
                        Id = maxId,
                        Name = store,
                        SellerId = maxId,
                        Address = address,
                        TelNumber = telNumber,
                        PostalCode = postalCode,
                        Description = sampleProduct,
                        EstablishmentDate = establishmentDate.ConvertShamsiToMiladi(),
                        InsertTime = DateTimeExtensions.DateTimeWithOutMilliSecends(DateTime.Now)
                    };
                    await _uw.BaseRepository<Store>().CreateAsync(newStore);
                }

                await _uw.Commit();
                if (imageFile.HasValue())
                    await InsertImage(imageFile, seller.Id, null);
                if (scanNationalIdCart.HasValue())
                    await InsertImage(scanDocument, seller.Id, null);
                if (scanDocument.HasValue())
                    await InsertImage(scanNationalIdCart, seller.Id, null);
                return Ok(email);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<ApiResult<string>> ReciveAnswer(string email)
        {
            if (email.HasValue())
            {
                var seller = await _uw.Context.Sellers.FirstOrDefaultAsync(s => s.Email == email);
                if (seller == null)
                    return Ok();
                else
                {
                    if (seller.IsComplete)
                        return Ok("success");
                    else
                        return Ok();
                }
            }
            return Ok();
        }

        public async Task<string> InsertImage(string file, int sellerId, string imageName)
        {
            var checkRootFolder = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(checkRootFolder))
                Directory.CreateDirectory(checkRootFolder);

            var checkAngularFolder = Path.Combine(_env.WebRootPath + "/../../Angular/src/assets/images/", "sellerImages");
            if (!Directory.Exists(checkAngularFolder))
                Directory.CreateDirectory(checkAngularFolder);

            if (imageName.HasValue())
            {
                FileExtensions.DeleteFile($"{_env.WebRootPath}/images/{imageName}");
                FileExtensions.DeleteFile($"{_env.WebRootPath}/../../Angular/src/assets/images/sellerImages/{imageName}"); ;
            }
            var image = $"seller-{StringExtensions.GenerateId(10)}.jpg";
            file.UploadFileBase64($"{_env.WebRootPath}/images/{image}");
            file.UploadFileBase64($"{_env.WebRootPath}/../../Angular/src/assets/images/sellerImages/{image}");

            var images = new List<string>();
            images.Add(image);
            await _uw.FileRepository.InsertMultiImageAsync(null, sellerId, null, images);
            return image;
        }

    }

    public class Model
    {
        public long phoneNumber { get; set; }
        public string name { get; set; }
        public string store { get; set; }
        public string brand { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string surName { get; set; }
        public string webSite { get; set; }
        public string telNumber { get; set; }
        public string imageFile { get; set; }
        public string postalCode { get; set; }
        public string nationalId { get; set; }
        public string description { get; set; }
        public string scanDocument { get; set; }
        public string activityType { get; set; }
        public string sampleProduct { get; set; }
        public string establishmentDate { get; set; }
        public string scanNationalIdCart { get; set; }
    }
}
