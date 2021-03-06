#pragma checksum "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\File\_FileTable.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f67e9433ea17438256973e1daab8a12d83d8b948"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_File__FileTable), @"mvc.1.0.view", @"/Areas/Admin/Views/File/_FileTable.cshtml")]
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
#line 1 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\File\_FileTable.cshtml"
using SnapMarket.Services.Contracts;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f67e9433ea17438256973e1daab8a12d83d8b948", @"/Areas/Admin/Views/File/_FileTable.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d427e38f5e5e1d1d9a2e3ef7c542ced3d59541da", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    public class Areas_Admin_Views_File__FileTable : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(@"
<div id=""toolbar"">
    <button type=""button"" id=""remove"" class=""btn btn-danger"" onclick=""DeleteGroup('File', getIdSelections())"" disabled>
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
       data-url=""/Admin/File/GetFiles""
       data-response-handler=""responseHandler"">
</table>

<script>
    var $table = $('#table');
    var $remove = $('#remove');
    var selec");
            WriteLiteral(@"tions = []

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
            if (key != ""state"" && key != ""Id"" && key != ""ردیف"" && key != ""تصویر"" && key != ""فایل/سند"" && key != ""نام محصول"" && key != ""نام فروشنده"") {
                if (key == 'نام محصول' && value != null)
                    value = 'تصویر محصول نیست'
                html.push('<p><b>' + key");
            WriteLiteral(" + \':</b> \' + value + \'</p>\')\r\n            }\r\n            if (key == \"تصویر\") {\r\n                if (value != null) {\r\n                    var url = \'");
#nullable restore
#line 71 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\File\_FileTable.cshtml"
                          Write(string.Format("{0}://{1}", Context.Request.Scheme, Context.Request.Host));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"' + '/gallery/' + value;
                    html.push('<p><b>' + key + ':</b>',
                        '<a href=""javascript:void(0);"" data-toggle=""modal"" data-target=""#imageModal"">',
                            '<img src=""' + url + '"" height=""200"" />',
                        '</a>',
                        '<div id=""imageModal"" class=""modal fade"" role=""dialog"">',
                            '<div class=""modal-dialog  modal-lg"">',
                                '<div class=""modal-content"">',
                                    '<div class=""modal-header"">',
                                        '<h4 class=""modal-title"" id=""myModalLabel"">' + row.تصویر + '</h4 >',
                                        '<button type=""button"" class=""close"" data-dismiss=""modal"" aria-hidden=""true"">&times;</button>',
                                    '</div>',
                                        '<div class=""modal-body"">',
                                            '<img id=""modalImage"" src=""' + url + '"" st");
            WriteLiteral(@"yle=""width:100%;height:100%;max-width:900px;max-height:500px;""/>',
                                        '</div>',
                                    '<div class=""modal-footer"">',
                                        '<input type=""button"" class=""btn btn-sm btn-light"" style=""color:red;"" value=""بزرگنمایی"" onclick=""findImage()""/>',
                                        '<button type=""button"" class=""btn btn-sm btn-secondary"" data-dismiss=""modal"">بستن</button>',
                                    '</div>',
                                '</div>',
                            '</div>',
                        '</div>',
                    '</p> ');
                    value = '';
                }
            }
            if (key == ""فایل/سند"") {
                if (value != null) {
                    var url = '");
#nullable restore
#line 99 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\File\_FileTable.cshtml"
                          Write(string.Format("{0}://{1}", Context.Request.Scheme, Context.Request.Host));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"' + '/gallery/' + value;
                    html.push('<p><b>' + key + ':</b> <img src=""' + url + '"" style=""height=200"" /></p>')
                }
            }
        })
        return html.join('')
    }

    function operateFormatter(value, row, index) {
        var access = """";
        if ('");
#nullable restore
#line 109 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\File\_FileTable.cshtml"
        Write(securityTrimmingService.CanUserAccess(User,"Admin", "File", "Delete"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\' == \'True\') {\r\n            access = access + \'<button type=\"button\" class=\"btn-link text-danger\" data-toggle=\"ajax-modal\" data-url=");
#nullable restore
#line 110 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\File\_FileTable.cshtml"
                                                                                                               Write(Url.Action("Delete", "File"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"/?brandId=' + row.Id + ' title=""حذف""><i class=""fa fa-trash""></i></button >';
        }
        return access;
    }

    function imageFormatter(index, row) {
        var html = []
        $.each(row, function (key, value) {
            if (key == ""تصویر"") {
                if (value != null) {
                    var url = '");
#nullable restore
#line 120 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\File\_FileTable.cshtml"
                          Write(string.Format("{0}://{1}", Context.Request.Scheme, Context.Request.Host));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"' + '/gallery/' + value;
                    html.push('<p><img src=""' + url + '"" height=""70"" title=""تصویر"" /></p>')
                }
            }
        })
        return html.join('')
    }

    function fileFormatter(index, row) {
        var html = []
        $.each(row, function (key, value) {
            if (key == ""فایل/سند"") {
                if (value != null) {
                    var url = '");
#nullable restore
#line 133 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\File\_FileTable.cshtml"
                          Write(string.Format("{0}://{1}", Context.Request.Scheme, Context.Request.Host));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"' + '/gallery/' + value;
                    html.push('<p><img src=""' + url + '"" height=""70"" title=""فایل"" /></p>')
                }
            }
        })
        return html.join('')
    }

    function totalTextFormatter(data) {
        return 'تعداد'
    }

    function totalNameFormatter(data) {
        return data.length
    }

    function initTable() {
        $table.bootstrapTable('destroy').bootstrapTable({
            height: 900,
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
               ");
            WriteLiteral(@"     title: 'جزئیات اطلاعات فایل ها',
                    colspan: 5,
                    align: 'center'
                }],
                [{
                    field: 'تصویر',
                    title: 'تصویر',
                    sortable: true,
                    align: 'center',
                    formatter: imageFormatter,
                },{
                    field: 'نام محصول',
                    title: 'نام محصول',
                    sortable: false,
                },{
                    field: 'فایل/سند',
                    title: 'فایل/سند',
                    sortable: true,
                    align: 'center',
                    formatter: fileFormatter,
                },{
                    field: 'نام فروشنده',
                    title: 'نام فروشنده',
                    sortable: false,
                },{
                    field: 'operate',
                    title: 'عملیات',
                    align: 'center',
                    events: wind");
            WriteLiteral(@"ow.operateEvents,
                    formatter: operateFormatter
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
