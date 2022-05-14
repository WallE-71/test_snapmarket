#pragma checksum "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Category\_CategoryTable.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "c297eb1bdbb9d1236442908cd4596e25e5c495a9"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_Category__CategoryTable), @"mvc.1.0.view", @"/Areas/Admin/Views/Category/_CategoryTable.cshtml")]
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
#line 1 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Category\_CategoryTable.cshtml"
using SnapMarket.Services.Contracts;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c297eb1bdbb9d1236442908cd4596e25e5c495a9", @"/Areas/Admin/Views/Category/_CategoryTable.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d427e38f5e5e1d1d9a2e3ef7c542ced3d59541da", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    public class Areas_Admin_Views_Category__CategoryTable : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<div id=\"toolbar\">\r\n    <button type=\"button\" class=\"btn btn-success\" data-toggle=\"ajax-modal\" data-url=\"");
#nullable restore
#line 5 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Category\_CategoryTable.cshtml"
                                                                                Write(Url.Action("RenderCategory","Category"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@""">
        <i class=""fa fa-plus""></i> | افزودن دسته بندی جدید
    </button>
    <button type=""button"" id=""remove""  class=""btn btn-danger"" onclick=""DeleteGroup('Category', getIdSelections())"" disabled>
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
       data-url=""/Admin/Category/GetCategories""
       data-response-handler=""responseHandler"">
</table>

<script>
    va");
            WriteLiteral(@"r $table = $('#table');
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

    function getIdSelections() {
        return $.map($table.bootstrapTable('getSelections'), function (row) {
            return row.Id;
        });
    }

    function responseHandler(res) {
        $.each(res.rows, function (i, row) {
            row.state = $.inArray(row.id, selections) !== -1
        })
        return res
    }

    function detailFormatter(index, row) {
        var html = []
        $.each(row, function (key, value) {
            if (key != ""state"" && key != ""Id"" && key != ""ردیف"" && key != 'insertTime' && key != 'isComplete' && key != 'توضیحات' && key != 'description' && key != 'persianInsertTime') {
                if (key == ""name"") key =");
            WriteLiteral(@" 'دسته';
                if (key == ""parentName"") key = 'دسته پدر';

                html.push('<p><b>' + key + ':</b> ' + value + '</p>')
            }
        })
        return html.join('')
    }

    function operateFormatter(value, row, index) {
        var access = """";
        if ('");
#nullable restore
#line 79 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Category\_CategoryTable.cshtml"
        Write(securityTrimmingService.CanUserAccess(User,"Admin","Category","RenderCategory"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\' == \'True\') {\r\n            access = access + \'<button type=\"button\" class=\"btn-link text-success\" data-toggle=\"ajax-modal\" data-url=");
#nullable restore
#line 80 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Category\_CategoryTable.cshtml"
                                                                                                                Write(Url.Action("RenderCategory", "Category"));

#line default
#line hidden
#nullable disable
            WriteLiteral("?categoryId=\' + row.Id + \' title=\"ویرایش\"><i class=\"fa fa-edit\"></i></button>\';\r\n        }\r\n        if (\'");
#nullable restore
#line 82 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Category\_CategoryTable.cshtml"
        Write(securityTrimmingService.CanUserAccess(User,"Admin","Category","Delete"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\' == \'True\') {\r\n            access = access + \'<button type=\"button\" class=\"btn-link text-danger\" data-toggle=\"ajax-modal\" data-url=");
#nullable restore
#line 83 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Category\_CategoryTable.cshtml"
                                                                                                               Write(Url.Action("Delete", "Category"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"/?categoryId=' + row.Id + ' title=""حذف""><i class=""fa fa-trash""></i></button>';
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
                }, {
                    title: 'ردیف',
                    field: 'ردیف',
                    rowspan: 2,
                    align: 'center',
                    valign: 'middle',
                    footerFormatter: totalTextFormatter
                }, {
                    title: 'جزئیات اطلاعات دسته بندی ها',
                    colspan: 3,
  ");
            WriteLiteral(@"                  align: 'center'
                }],
                [{
                    field: 'name',
                    title: 'دسته',
                    sortable: true,
                    align: 'center',
                    footerFormatter: totalNameFormatter
                }, {
                    field: 'parentName',
                    title: 'دسته پدر',
                    sortable: true,
                    align: 'center'
                }, {
                    field: 'operate',
                    title: 'عملیات',
                    align: 'center',
                    events: window.operateEvents,
                    formatter: operateFormatter
                }]
            ]
        })

        $table.on('check.bs.table uncheck.bs.table ' +
            'check-all.bs.table uncheck-all.bs.table',
            function () {
                $remove.prop('disabled', !$table.bootstrapTable('getSelections').length)
                selections = getIdSelections()
   ");
            WriteLiteral("     })\r\n    }\r\n\r\n    $(function () {\r\n        initTable()\r\n        $(\'#locale\').change(initTable)\r\n    })\r\n</script>\r\n");
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
