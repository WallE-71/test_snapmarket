﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SnapMarket.Entities;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.ViewModels.Slider;

namespace SnapMarket.Data.Repositories
{
    public class SliderRepository : ISliderRepository
    {
        private readonly SnapMarketDBContext _context;
        public SliderRepository(SnapMarketDBContext context)
        {
            _context = context;
        }

        public async Task<List<SliderViewModel>> GetPaginateSlidersAsync(int offset, int limit, string orderby, string searchText)
        {
            var sliders = await _context.Sliders.Include(s => s.FileStores)
                                    .Where(s => s.Url.Contains(searchText) || s.ImageLocation.Equals(searchText))
                                    .OrderBy(orderby).Skip(offset).Take(limit)
                                    .Select(s => new SliderViewModel
                                    {
                                        Id = s.Id,
                                        Url = s.Url,
                                        Title = s.Name,
                                        InsertTime = s.InsertTime,
                                        Description = s.Description,
                                        TypeOfSlider = s.TypeOfSlider,
                                        ImageLocation = s.ImageLocation,
                                        PersianInsertTime = s.InsertTime.DateTimeEn2Fa("yyyy/MM/dd"),
                                        //TitleSlider1 = s.TypeOfSlider == TypeOfSlider.Dynamic ? s.Name : null,
                                        //DescriptionSlider1 = s.TypeOfSlider == TypeOfSlider.Control ? s.Description : null,
                                    }).AsNoTracking().ToListAsync();

            foreach (var item in sliders)
                item.Row = ++offset;
            return sliders;
        }
    }
}
