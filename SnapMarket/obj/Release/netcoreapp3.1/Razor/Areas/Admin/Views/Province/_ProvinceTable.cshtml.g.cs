#pragma checksum "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Province\_ProvinceTable.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "d22ddedc7d5762b68578c87bc2287f183da3c397"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_Province__ProvinceTable), @"mvc.1.0.view", @"/Areas/Admin/Views/Province/_ProvinceTable.cshtml")]
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
#nullable restore
#line 1 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Province\_ProvinceTable.cshtml"
using SnapMarket.Services.Contracts;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d22ddedc7d5762b68578c87bc2287f183da3c397", @"/Areas/Admin/Views/Province/_ProvinceTable.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d427e38f5e5e1d1d9a2e3ef7c542ced3d59541da", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    public class Areas_Admin_Views_Province__ProvinceTable : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<div id=\"toolbar\">\r\n    <button type=\"button\" class=\"btn btn-success\" data-toggle=\"ajax-modal\" data-url=\"");
#nullable restore
#line 5 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Province\_ProvinceTable.cshtml"
                                                                                Write(Url.Action("RenderProvince","Province"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@""">
        <i class=""fa fa-plus""></i> | ???????????? ?????????? ????????
    </button>
    <button type=""button"" id=""remove"" class=""btn btn-danger"" onclick=""DeleteGroup('Province', getIdSelections())"" disabled>
        <i class=""fa fa-trash""></i> | ?????? ??????????
    </button>
</div>
<table id=""table""
       data-toolbar=""#toolbar""
       data-search=""true""
       data-show-refresh=""true""
       data-show-toggle=""true""
       data-show-fullscreen=""true""
       data-show-columns=""true""
       data-show-columns-toggle-all=""false""
       data-detail-view=""true""
       data-show-export=""true""
       data-click-to-select=""false""
       data-detail-formatter=""detailFormatter""
       data-minimum-count-columns=""2""
       data-show-pagination-switch=""true""
       data-pagination=""true""
       data-id-field=""id""
       data-page-list=""[10, 25, 50, 100, all]""
       data-show-footer=""true""
       data-side-pagination=""server""
       data-url=""/Admin/Province/GetProvinces""
       data-response-handler=""responseHa");
            WriteLiteral(@"ndler"">
</table>

<script>
    var $table = $('#table');
    var $remove = $('#remove');
    var selections = [];
    var SellerProductIds = [];

    function get_query_params(p) {
        return {
            extraParam: 'abc',
            search: p.title,
            sort: p.sort,
            order: p.order,
            limit: p.limit,
            offset: p.offset
        }
    }

    function responseHandler(res) {
        $.each(res.rows, function (i, row) {
            row.state = $.inArray(row.id, selections) !== -1
        })
        return res
    }

    function getIdSelections() {
        return $.map($table.bootstrapTable('getSelections'), function (row) {
            return row.Id;
        });
    }

    function detailFormatter(index, row) {
        var html = []
        $.each(row, function (key, value) {
            if (key != ""state"" && key != ""Id"" && key != ""????????"" && key != ""isComplete"" && key != ""cityName"" && key != ""insertTime"" && key != ""persianInsertTim");
            WriteLiteral(@"e"" && key != ""numberOfUser"" && key != ""cityId"") {
                if (key == ""provinceName"") key = '??????????';
                if (key == ""numberOfCities"") key = '?????????? ??????';

                html.push('<p><b>' + key + ':</b> ' + value + '</p>')
            }
        })
        return html.join('')
    }

    function operateFormatter(value, row, index) {
        var access = """";
        if ('");
#nullable restore
#line 80 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Province\_ProvinceTable.cshtml"
        Write(securityTrimmingService.CanUserAccess(User,"Admin", "Province", "RenderProvince"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\' == \'True\') {\r\n            access = access + \'<button type=\"button\" class=\"btn-link text-success\" data-toggle=\"ajax-modal\" data-url=");
#nullable restore
#line 81 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Province\_ProvinceTable.cshtml"
                                                                                                                Write(Url.Action("RenderProvince", "Province"));

#line default
#line hidden
#nullable disable
            WriteLiteral("?provinceId=\' + row.Id + \' title=\"????????????\"><i class=\"fa fa-edit\"></i></button >\';\r\n        }\r\n        if (\'");
#nullable restore
#line 83 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Province\_ProvinceTable.cshtml"
        Write(securityTrimmingService.CanUserAccess(User,"Admin", "Province", "Delete"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\' == \'True\') {\r\n            access = access + \'<button type=\"button\" class=\"btn-link text-danger\" data-toggle=\"ajax-modal\" data-url=");
#nullable restore
#line 84 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Province\_ProvinceTable.cshtml"
                                                                                                               Write(Url.Action("Delete", "Province"));

#line default
#line hidden
#nullable disable
            WriteLiteral("/?provinceId=\' + row.Id + \' title=\"??????\"><i class=\"fa fa-trash\"></i></button >\';\r\n        }\r\n        return access;\r\n    }\r\n\r\n    function citiesFormatter(value, row, index) {\r\n        var access = \"\";\r\n        if (\'");
#nullable restore
#line 91 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Province\_ProvinceTable.cshtml"
        Write(securityTrimmingService.CanUserAccess(User,"Admin", "City", "Index"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"' == 'True') {
            access = access + '<a type=""button"" class=""btn btn-primary btn-sm text-white"" href=""/Admin/City/Index?cityId=' + row.cityId + '"" title=""??????????"">' + row.numberOfCities + '<i class=""fa fa-city mr-1""></i></a >';
        }
        return access;
    }

    function totalTextFormatter(data) {
        return '??????????'
    }

    function totalNameFormatter(data) {
        return data.length
    }

    function initTable() {
        $table.bootstrapTable('destroy').bootstrapTable({
            height: 600,
            locale: 'fa-IR',
            columns: [
                [{
                    field: 'state',
                    checkbox: true,
                    rowspan: 2,
                    align: 'center',
                    valign: 'middle',
                    name: 'getSelections'
                },{
                    field: '????????',
                    title: '????????',
                    rowspan: 2,
                    align: 'center',
             ");
            WriteLiteral(@"       valign: 'middle',
                    footerFormatter: totalTextFormatter
                },{
                    title: '???????????? ?????????????? ??????????',
                    colspan: 3,
                    align: 'center'
                }],
                [{
                    field: 'provinceName',
                    title: '??????????',
                    sortable: true,
                    footerFormatter: totalNameFormatter
                }, {
                    field: '',
                    title: '???????????? ???????????? ??????????',
                    align: 'center',
                    formatter: citiesFormatter
                },{
                    field: 'operate',
                    title: '????????????',
                    align: 'center',
                    formatter: operateFormatter
                }]
            ]
        })

        $table.on('check.bs.table uncheck.bs.table ' +
            'check-all.bs.table uncheck-all.bs.table',
            function () {
                $r");
            WriteLiteral("emove.prop(\'disabled\', !$table.bootstrapTable(\'getSelections\').length)\r\n                selections = getIdSelections()\r\n        })\r\n    }\r\n\r\n    $(function () {\r\n        initTable()\r\n        $(\'#locale\').change(initTable)\r\n    })\r\n</script>\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public ISecurityTrimmingService securityTrimmingService { get; private set; }
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
