using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SnapMarket.Data.Contracts;
using SnapMarket.Common.Extensions;
using SnapMarket.ViewModels.Dashboard;
using SnapMarket.ViewModels.DynamicAccess;

namespace SnapMarket.Areas.Admin.Controllers
{
    [DisplayName("داشبورد")]
    public class DashboardController : BaseController
    {
        private readonly IUnitOfWork _uw;
        public DashboardController(IUnitOfWork uw)
        {
            _uw = uw;
        }

        [HttpGet, DisplayName("مشاهده"), Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index()
        {
            ViewBag.Products = _uw.ProductRepository.CountProducts();
            ViewBag.FutureProducts = _uw.ProductRepository.CountFutureProducts();
            ViewBag.DemoProducts = _uw.ProductRepository.CountProductPresentationOrDemo(true);
            ViewBag.ProductsPresentation = _uw.ProductRepository.CountProductPresentationOrDemo(false);

            int numberOfVisit;
            var month = StringExtensions.GetMonth();
            var year = DateTimeExtensions.ConvertMiladiToShamsi(DateTime.Now, "yyyy");
            var numberOfVisitList = new List<NumberOfVisitChartViewModel>();
            DateTime StartDateTimeMiladi;
            DateTime EndDateTimeMiladi;

            for (int i = 0; i < month.Length; i++)
            {
                StartDateTimeMiladi = DateTimeExtensions.ConvertShamsiToMiladi($"{year}/{i + 1}/01");
                if (i < 11)
                    EndDateTimeMiladi = DateTimeExtensions.ConvertShamsiToMiladi($"{year}/{i + 2}/01");
                else
                    EndDateTimeMiladi = DateTimeExtensions.ConvertShamsiToMiladi($"{year}/01/01");

                numberOfVisit = _uw.Context.Products.Where(n => n.InsertTime < EndDateTimeMiladi && StartDateTimeMiladi <= n.InsertTime)
                    .Include(v => v.Visits).Select(k => k.Visits.Sum(v => v.NumberOfVisit)).AsEnumerable().Sum();

                numberOfVisitList.Add(new NumberOfVisitChartViewModel { Name = month[i], Value = numberOfVisit });
            }

            ViewBag.NumberOfVisitChart = numberOfVisitList;
            return View();
        }
    }
}
