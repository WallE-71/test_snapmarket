using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SnapMarket.Common.Api;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.ViewModels.Category;
using SnapMarket.Common.Api.Attributes;
using SnapMarket.Services.Api.Contract;

namespace SnapMarket.Areas.Api.Controllers.v1
{
    [DisplayName("CategoryApi"), ApiResultFilter, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
    public class CategoryApiController : ControllerBase
    {
        private readonly IUnitOfWork _uw;
        private readonly IMapper _mapper;
        public CategoryApiController(IUnitOfWork uw, IMapper mapper, IJwtService jwtService)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
        }

        // [GET]  api/v1/CategoryApi
        [HttpGet]
        public virtual async Task<ApiResult<List<TreeViewCategory>>> ShowCategories()
        {
            return Ok(await _uw.CategoryRepository.GetAllCategoriesAsync());
        }

        // [GET]  api/v1/CategoryApi/Get/{parentName}
        [HttpGet("SubCategories")]
        public virtual async Task<ApiResult<List<TreeViewCategory>>> Get(string parentName)
        {//Display
            var treeViewCategories = await _uw.CategoryRepository.GetSubCategoriesByName(parentName);
            if (treeViewCategories.Count == 0)
                return NotFound();
            else
                return Ok(treeViewCategories);
        }
    }
}
