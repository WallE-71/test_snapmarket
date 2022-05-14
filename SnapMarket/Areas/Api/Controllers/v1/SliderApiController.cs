using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SnapMarket.Entities;
using SnapMarket.Common.Api;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.ViewModels.Slider;
using SnapMarket.Common.Api.Attributes;

namespace SnapMarket.Areas.Api.Controllers.v1
{
    [DisplayName("SliderApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class SliderApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        public SliderApiController(IUnitOfWork uw)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
        }

        // [GET]  api/v1/SliderApi
        [HttpGet]
        public virtual async Task<ApiResult<List<SliderViewModel>>> GetSliders()
        {
            var viewModels = new List<SliderViewModel>();
            var sliders = await _uw.BaseRepository<Slider>().FindByConditionAsync(null, s => s.OrderBy(s => s.Id));
            foreach (var slider in sliders)
            {
                var viewModel = new SliderViewModel();
                if (slider.TypeOfSlider == TypeOfSlider.Static)
                    viewModel.ImageFile = await _uw.FileRepository.FindImageAsync(null, null, slider.Id);
                else if (slider.TypeOfSlider != 0)
                {
                    viewModel.ImageFiles = new List<string>();
                    var images = await _uw.FileRepository.GetImagesAsync(null, null, slider.Id);
                    foreach (var image in images)
                        viewModel.ImageFiles.Add(image);
                }
                viewModel.Title = slider.Name;
                viewModel.Description = slider.Description;
                viewModel.TypeOfSlider = slider.TypeOfSlider;
                viewModel.ImageLocation = slider.ImageLocation;
                viewModels.Add(viewModel);
            }
            if (viewModels.Count != 0)
                return Ok(viewModels);
            return Ok();
        }
    }
}
