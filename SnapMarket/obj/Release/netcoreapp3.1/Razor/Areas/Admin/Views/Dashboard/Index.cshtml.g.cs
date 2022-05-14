#pragma checksum "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Dashboard\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "3aed5c6f37f0d8d38e0f974784d669e8cadfbdc2"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_Dashboard_Index), @"mvc.1.0.view", @"/Areas/Admin/Views/Dashboard/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"3aed5c6f37f0d8d38e0f974784d669e8cadfbdc2", @"/Areas/Admin/Views/Dashboard/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d427e38f5e5e1d1d9a2e3ef7c542ced3d59541da", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    public class Areas_Admin_Views_Dashboard_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Dashboard\Index.cshtml"
  
    ViewData["Title"] = "Index";
    Layout = "../Shared/_AdminLayout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<script src=""https://code.highcharts.com/highcharts.js""></script>
<script src=""https://code.highcharts.com/modules/data.js""></script>
<script src=""https://code.highcharts.com/modules/drilldown.js""></script>

<style>
    .highcharts-root {
        font-family: Vazir_Medium !important;
    }

    .highcharts-credits {
        display: none !important;
    }
</style>

<div id=""modal-placeholder""></div>
<nav class=""navbar navbar-top navbar-expand-md navbar-dark"" id=""navbar-main"">
    <div class=""container-fluid"">
        <ul class=""nav nav-sitemap justify-content-center justify-content-xl-end"">
            <li>
                <a class=""h4 mb-0 text-white d-lg-inline-block"" href=""/Admin/Dashboard/Index""> داشبورد </a>
            </li>
        </ul>
        ");
#nullable restore
#line 28 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Dashboard\Index.cshtml"
   Write(await Html.PartialAsync("_AdminLogin"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
    </div>
</nav>
<div class=""header bg-gradient-primary pb-6 pt-5 pt-md-8""></div>

<div class=""container-fluid mt--7"">
    <div class=""header-body"">
        <!-- Card stats -->
        <div class=""row"">
            <div class=""col-xl-3 col-lg-6"">
                <div class=""card card-stats mb-4 mb-xl-0"">
                    <div class=""card-body"">
                        <div class=""row"">
                            <div class=""col"">
                                <h5 class=""card-title text-uppercase text-muted mb-0"">کل محصولات موجود</h5>
                                <span class=""h2 mb-0 font_Vazir_FD"" id=""counter1"">0</span>
                            </div>
                            <div class=""col-auto"">
                                <div class=""icon icon-shape bg-success text-white rounded-circle shadow"">
                                    <i class=""fas fa-newspaper""></i>
                                </div>
                            </div>
                        </di");
            WriteLiteral(@"v>
                    </div>
                </div>
            </div>
            <div class=""col-xl-3 col-lg-6"">
                <div class=""card card-stats mb-4 mb-xl-0"">
                    <div class=""card-body"">
                        <div class=""row"">
                            <div class=""col"">
                                <h5 class=""card-title text-uppercase text-muted mb-0"">محصولات عرضه شده</h5>
                                <span class=""h2 mb-0 font_Vazir_FD"" id=""counter2"">0</span>
                            </div>
                            <div class=""col-auto"">
                                <div class=""icon icon-shape bg-warning text-white rounded-circle shadow"">
                                    <i class=""fas fa-check""></i>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class=""col-xl-3 col-lg-6"">
                <div ");
            WriteLiteral(@"class=""card card-stats mb-4 mb-xl-0"">
                    <div class=""card-body"">
                        <div class=""row"">
                            <div class=""col"">
                                <h5 class=""card-title text-uppercase text-muted mb-0"">محصولات دمو</h5>
                                <span class=""h2 mb-0 font_Vazir_FD"" id=""counter3"">0</span>
                            </div>
                            <div class=""col-auto"">
                                <div class=""icon icon-shape bg-yellow text-white rounded-circle shadow"">
                                    <i class=""fas fa-edit""></i>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class=""col-xl-3 col-lg-6"">
                <div class=""card card-stats mb-4 mb-xl-0"">
                    <div class=""card-body"">
                        <div class=""row"">
                      ");
            WriteLiteral(@"      <div class=""col"">
                                <h5 class=""card-title text-uppercase text-muted mb-0"">محصولات آینده</h5>
                                <span class=""h2 mb-0 font_Vazir_FD"" id=""counter4"">0</span>
                            </div>
                            <div class=""col-auto"">
                                <div class=""icon icon-shape bg-info text-white rounded-circle shadow"">
                                    <i class=""fas fa-paper-plane""></i>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class=""row mt-5"">
        <div class=""card shadow w-100"">
            <div class=""card-header font_Vazir_Medium"">
                بازدیدهای محصولات عرضه شده در هر ماه
            </div>
            <div class=""card-body"">
                <div id=""container"" style=""min-width: 310px; margin: 0 auto""></div>
       ");
            WriteLiteral(@"     </div>
        </div>
    </div>
</div>

<div id=""container"" style=""min-width: 310px; margin: 0 auto""></div>

<script>
    // Create the chart
Highcharts.chart('container', {
    chart: {
        type: 'column'
    },

    title: {
        text: 'نمودار بازدیدهای محصولات عرضه شده در هر ماه'
    },

    xAxis: {
        type: 'category'
    },

    yAxis: {
        title: {
            text: 'تعداد بازدید'
        }
    },

    legend: {
        enabled: false
    },

    plotOptions: {
        series: {
            borderWidth: 0,
            dataLabels: {
                enabled: true,
                format: '{point.y}'
            }
        }
    },

    tooltip: {
        headerFormat: '<span style=""font-size:12px"">{series.name}</span><br>',
        pointFormat: '<span style=""color:{point.color}"">{point.name}</span>: <b>{point.y}'
    },

    series: [
        {
            name: ""ماه ها"",
            colorByPoint: true,
            data: ");
#nullable restore
#line 165 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Dashboard\Index.cshtml"
             Write(Html.Raw(Json.Serialize(ViewBag.NumberOfVisitChart)));

#line default
#line hidden
#nullable disable
            WriteLiteral(",\r\n        }\r\n    ],\r\n});\r\n\r\n    var count1 = 0;\r\n    var counter1 = setTimeout(timer1, 200);\r\n    function timer1() {\r\n        count1 = count1 + 1;\r\n        if (count1 <= ");
#nullable restore
#line 174 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Dashboard\Index.cshtml"
                 Write(ViewBag.Products);

#line default
#line hidden
#nullable disable
            WriteLiteral(@") {
            document.getElementById(""counter1"").innerHTML = count1;
            counter1 = setTimeout(timer1, 200);
        }
    }
    var count2 = 0;
    var counter2 = setTimeout(timer2, 250);
    function timer2() {
        count2 = count2 + 1;
        if (count2 <= ");
#nullable restore
#line 183 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Dashboard\Index.cshtml"
                 Write(ViewBag.ProductsPresentation);

#line default
#line hidden
#nullable disable
            WriteLiteral(@") {
            document.getElementById(""counter2"").innerHTML = count2;
            counter2 = setTimeout(timer2, 250);
        }
    }
    var count3 = 0;
    var counter3 = setTimeout(timer3, 300);
    function timer3() {
        count3 = count3 + 1;
        if (count3 <= ");
#nullable restore
#line 192 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Dashboard\Index.cshtml"
                 Write(ViewBag.DemoProducts);

#line default
#line hidden
#nullable disable
            WriteLiteral(@") {
            document.getElementById(""counter3"").innerHTML = count3;
            counter3 = setTimeout(timer3, 300);
        }
    }
    var count4 = 0;
    var counter4 = setTimeout(timer4, 350);
    function timer4() {
        count4 = count4 + 1;
        if (count4 <= ");
#nullable restore
#line 201 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Dashboard\Index.cshtml"
                 Write(ViewBag.FutureProducts);

#line default
#line hidden
#nullable disable
            WriteLiteral(") {\r\n            document.getElementById(\"counter4\").innerHTML = count4;\r\n            counter4 = setTimeout(timer4, 350);\r\n        }\r\n    }\r\n</script>\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
