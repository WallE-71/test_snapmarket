#pragma checksum "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Color\_ColorTable.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "fe66bd907439108e5a2725be756c04b4592dad20"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_Color__ColorTable), @"mvc.1.0.view", @"/Areas/Admin/Views/Color/_ColorTable.cshtml")]
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
#line 1 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Color\_ColorTable.cshtml"
using SnapMarket.Services.Contracts;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"fe66bd907439108e5a2725be756c04b4592dad20", @"/Areas/Admin/Views/Color/_ColorTable.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d427e38f5e5e1d1d9a2e3ef7c542ced3d59541da", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    public class Areas_Admin_Views_Color__ColorTable : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<div id=\"toolbar\">\r\n    <button type=\"button\" class=\"btn btn-success\" data-toggle=\"ajax-modal\" data-url=\"");
#nullable restore
#line 5 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Color\_ColorTable.cshtml"
                                                                                Write(Url.Action("RenderColor","Color"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@""">
        <i class=""fa fa-plus""></i> | افزودن رنگ جدید
    </button>
    <button type=""button"" id=""remove"" class=""btn btn-danger"" onclick=""DeleteGroup('Color', getIdSelections())"" disabled>
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
       data-url=""/Admin/Color/GetColors""
       data-response-handler=""responseHandler"">
</table>

<script>
    var $table = $('#ta");
            WriteLiteral(@"ble');
    var $remove = $('#remove');
    var selections = []

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
            if (key != ""state"" && key != ""Id"" && key != ""ردیف"" && key != ""insertTime"" && key != ""description"" && key != ""isComplete"" && key != ""persianInsertTime"") {
                if (key == 'R') key = 'مقدار قرمر';
                if (key ");
            WriteLiteral(@"== 'G') key = 'مقدار آبی';
                if (key == 'B') key = 'مقدار سبز';
                if (key == 'A') key = 'مقدار شفافیت';
                if (key == 'hexadecimal') key = 'کد رنگ';
                if (key == 'name') {
                    key = 'رنگ';
                    if (value == null)
                        value = '';
                }
                html.push('<p><b>' + key + ':</b> ' + value + '</p>')
            }
        })
        return html.join('')
    }

    function operateFormatter(value, row, index) {
        var access = """";
        if ('");
#nullable restore
#line 86 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Color\_ColorTable.cshtml"
        Write(securityTrimmingService.CanUserAccess(User,"Admin", "Color", "RenderColor"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\' == \'True\') {\r\n            access = access + \'<button type=\"button\" class=\"btn-link text-success\" data-toggle=\"ajax-modal\" data-url=");
#nullable restore
#line 87 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Color\_ColorTable.cshtml"
                                                                                                                Write(Url.Action("RenderColor", "Color"));

#line default
#line hidden
#nullable disable
            WriteLiteral("?colorId=\' + row.Id + \' title=\"ویرایش\"><i class=\"fa fa-edit\"></i></button >\';\r\n        }\r\n        if (\'");
#nullable restore
#line 89 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Color\_ColorTable.cshtml"
        Write(securityTrimmingService.CanUserAccess(User,"Admin", "Color", "Delete"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\' == \'True\') {\r\n            access = access + \'<button type=\"button\" class=\"btn-link text-danger\" data-toggle=\"ajax-modal\" data-url=");
#nullable restore
#line 90 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Color\_ColorTable.cshtml"
                                                                                                               Write(Url.Action("Delete", "Color"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"/?colorId=' + row.Id + ' title=""حذف""><i class=""fa fa-trash""></i></button >';
        }
        return access;
    }

    function colorFormatter(value, row, index) {
        return [
            '<a class=""btn btn-info"" style=""background-color:' + row.nameOfColor + ';border-color:black;border-radius:10px;;padding:0 5px;"">' + row.nameOfColor + '</a >'
        ].join('')
    }

    function totalNameFormatter(data) {
        return data.length;
    }

    function totalTextFormatter(data) {
        return 'تعداد'
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
                }, {
                    title: 'ردیف',
                    field: 'ردیف',
                ");
            WriteLiteral(@"    rowspan: 2,
                    align: 'center',
                    valign: 'middle',
                    footerFormatter: totalTextFormatter
                }, {
                    title: 'جزئیات اطلاعات رنگ ها',
                    colspan: 3,
                    align: 'center'
                }],
                [{
                    field: 'hexadecimal',
                    title: 'رنگ',
                    sortable: true,
                    align: 'center',
                    formatter: colorFormatter,
                    footerFormatter: totalNameFormatter,
                },{
                    field: 'description',
                    title: 'تعداد محصول دارای این رنگ',
                    sortable: false,
                    width: 1,
                    align: 'center'
                },{
                    field: 'operate',
                    title: 'عملیات',
                    align: 'center',
                    events: window.operateEvents,
              ");
            WriteLiteral(@"      formatter: operateFormatter
                }]
            ]
        })

        $table.on('check.bs.table uncheck.bs.table ' +
            'check-all.bs.table uncheck-all.bs.table',
            function () {
                $remove.prop('disabled', !$table.bootstrapTable('getSelections').length)
                selections = getIdSelections()
        })
    }

    $(function () {
        initTable()
        $('#locale').change(initTable)
    })
</script>
");
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
