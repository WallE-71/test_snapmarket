#pragma checksum "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\City\_CityTable.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "75be86e3a2f7f9f3929da4d1e4d569a4e751c76e"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_City__CityTable), @"mvc.1.0.view", @"/Areas/Admin/Views/City/_CityTable.cshtml")]
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
#line 1 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\City\_CityTable.cshtml"
using SnapMarket.Services.Contracts;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"75be86e3a2f7f9f3929da4d1e4d569a4e751c76e", @"/Areas/Admin/Views/City/_CityTable.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d427e38f5e5e1d1d9a2e3ef7c542ced3d59541da", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    public class Areas_Admin_Views_City__CityTable : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<div id=\"toolbar\">\r\n    <button type=\"button\" class=\"btn btn-success\" data-toggle=\"ajax-modal\" data-url=\"");
#nullable restore
#line 5 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\City\_CityTable.cshtml"
                                                                                Write(Url.Action("RenderCity","City"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@""">
        <i class=""fa fa-plus""></i> | افزودن شهر جدید
    </button>
    <button type=""button"" id=""remove"" class=""btn btn-danger""  onclick=""DeleteGroup('City', getIdSelections())"" disabled>
        <i class=""fa fa-trash""></i> | حذف گروهی
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
       data-url=""/Admin/City/GetCities?cityId=");
#nullable restore
#line 31 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\City\_CityTable.cshtml"
                                         Write(Model.CityId);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"""
       data-response-handler=""responseHandler"">
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
            return sellerProductIds = row.Id;
        })
    }

    function detailFormatter(index, row) {
        var html = []
        $.each(row, function (key, value) {
            if (key != ""state"" && key != ""Id"" && key != ""ردیف"" && key != ""isComplete"" && key != ");
            WriteLiteral(@"""insertTime"" && key != ""persianInsertTime"" && key != ""cityId"" && key != ""numberOfCities"") {
                if (key == ""cityName"") key = 'شهر';
                if (key == ""provinceName"") key = 'استان';
                if (key == ""numberOfUser"") key = 'کاربران درون این شهر';

                html.push('<p><b>' + key + ':</b> ' + value + '</p>')
            }
        })
        return html.join('')
    }

    function operateFormatter(value, row, index) {
        var access = """";
");
            WriteLiteral("        if (\'");
#nullable restore
#line 84 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\City\_CityTable.cshtml"
        Write(securityTrimmingService.CanUserAccess(User,"Admin", "City", "RenderCity"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\' == \'True\') {\r\n            access = access + \'<button type=\"button\" class=\"btn-link text-success\" data-toggle=\"ajax-modal\" data-url=");
#nullable restore
#line 85 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\City\_CityTable.cshtml"
                                                                                                                Write(Url.Action("RenderCity", "City"));

#line default
#line hidden
#nullable disable
            WriteLiteral("?cityId=\' + row.Id + \' title=\"ویرایش\"><i class=\"fa fa-edit\"></i></button >\';\r\n        }\r\n        if (\'");
#nullable restore
#line 87 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\City\_CityTable.cshtml"
        Write(securityTrimmingService.CanUserAccess(User,"Admin", "City", "Delete"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\' == \'True\') {\r\n            access = access + \'<button type=\"button\" class=\"btn-link text-danger\" data-toggle=\"ajax-modal\" data-url=");
#nullable restore
#line 88 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\City\_CityTable.cshtml"
                                                                                                               Write(Url.Action("Delete", "City"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"/?cityId=' + row.Id + ' title=""حذف""><i class=""fa fa-trash""></i></button >';
        }
        return access;
    }

    function totalTextFormatter(data) {
        return 'تعداد'
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
                    field: 'ردیف',
                    title: 'ردیف',
                    rowspan: 2,
                    align: 'center',
                    valign: 'middle',
                    footerFormatter: totalTextFormatter
                },{
                    title: 'جزئیات اطلاعات شهرها',
    ");
            WriteLiteral(@"                colspan: 4,
                    align: 'center'
                }],
                [{
                    field: 'cityName',
                    title: 'شهر',
                    sortable: true,
                    footerFormatter: totalNameFormatter
                },{
                    field: 'provinceName',
                    title: 'استان',
                    footerFormatter: totalNameFormatter
                },{
                    field: 'numberOfUser',
                    title: 'کاربران درون هر شهر',
                    align: 'center'
                },{
                    field: 'operate',
                    title: 'عملیات',
                    align: 'center',
                    formatter: operateFormatter
                }]
            ]
        })

        $table.on('check.bs.table uncheck.bs.table ' +
            'check-all.bs.table uncheck-all.bs.table',
            function () {
                $remove.prop('disabled', !$table.bootstrapTable");
            WriteLiteral("(\'getSelections\').length)\r\n                selections = getIdSelections()\r\n        })\r\n    }\r\n\r\n    $(function () {\r\n        initTable()\r\n        $(\'#locale\').change(initTable)\r\n    })\r\n</script>\r\n");
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
