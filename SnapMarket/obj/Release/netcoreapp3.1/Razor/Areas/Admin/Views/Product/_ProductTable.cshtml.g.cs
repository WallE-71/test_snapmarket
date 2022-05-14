#pragma checksum "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Product\_ProductTable.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0cb5d24da6f0be48644728754b125e2cf90489ca"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_Product__ProductTable), @"mvc.1.0.view", @"/Areas/Admin/Views/Product/_ProductTable.cshtml")]
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
#line 3 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Product\_ProductTable.cshtml"
using SnapMarket.Services.Contracts;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0cb5d24da6f0be48644728754b125e2cf90489ca", @"/Areas/Admin/Views/Product/_ProductTable.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d427e38f5e5e1d1d9a2e3ef7c542ced3d59541da", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    public class Areas_Admin_Views_Product__ProductTable : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Byte[]>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-success text-white"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "CreateOrUpdate", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<div id=\"toolbar\">\r\n    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "0cb5d24da6f0be48644728754b125e2cf90489ca3742", async() => {
                WriteLiteral("\r\n        <i class=\"fa fa-plus\"></i> | افزودن محصول جدید\r\n    ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
    <button type=""button"" id=""remove"" class=""btn btn-danger"" onclick=""DeleteGroup('Product', getIdSelections())"" disabled>
        <i class=""fa fa-trash""></i> | حذف گروهی
    </button>
    <button type=""button"" class=""btn btn-info"" data-toggle=""ajax-modal"" data-url=""");
#nullable restore
#line 12 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Product\_ProductTable.cshtml"
                                                                             Write(Url.Action("RenderDiscount","Product"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@""">
        <i class=""fa fa-code""></i> | ایجاد کد تحفیف
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
       data-url=""/Admin/Product/GetProducts""
       data-response-handler=""responseHandler"">
</table>

<script>
    var $table = $('#table');
    var $remove = $('#remove');
    var selections = [];
    var SellerProductIds = [];

    function get_query_params(p) {
   ");
            WriteLiteral(@"     return {
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
            if (key != ""state"" && key != ""Id"" && key != ""ردیف"" && key != ""barcode"" && key != ""states"" && key != ""percentDiscount"" &&  key != ""imageFiles"" && key != ""imageName"" && key != ""stock"" && key != ""price"" && key != ""isPresent"" && key != ""productState"" && key != ""nameOfCategories"" && key != ""shortName"" && key != ""insertTime"" && key != ""isPrefe");
            WriteLiteral(@"red"" && key != ""isComplete"" && key != ""expirationDate"") {
                if (key == ""weight"") key = 'وزن';
                if (key == ""madeIn"") key = 'ساخت';
                if (key == ""name"") key = 'نام محصول';
                if (key == ""nameOfBrand"") key = 'برند';
                if (key == ""size"") key = 'ابعاد/اندازه';
                if (key == ""displayStates"") key = 'وضعیت';
                if (key == ""displayDiscount"") key = 'تخفیف';
                if (key == ""numberOfVisit"") key = 'بازدید ها';
                if (key == ""numberOfComments"") key = 'کامنت ها';
                if (key == ""actionDiscount"") key = 'محاسبه تخفیف';
                if (key == ""persianExpirationDate"") key = 'تاریخ انقضا';
                if (key == ""persianInsertTime"") key = 'تاریخ ساخت/تولید';

                if (key == ""numberOfSale"") {
                    key = 'تعداد کل سفارش داده شده از این محصول';
                    if (value == null || value == '')
                        value = '0'
                ");
            WriteLiteral("}\r\n                if (key == \"primaryColor\") {\r\n                    key = \'رنگ اصلی\';\r\n                    if (value == null || value == \'\')\r\n                        value = \'بی رنگ\'\r\n                    //else {\r\n\r\n");
            WriteLiteral(@"
                        //var dict = {
                        //    ""Home"": {
                        //        pt: ""Início""
                        //    },
                        //    ""Download plugin"": {
                        //        pt: ""Descarregar plugin"",
                        //        en: ""Download plugin""
                        //    }
                        //}

                        //var translator = $('body').translate({ lang: ""en"", t: dict });
                        //translator.lang(""fa"");

                        //'<span class=""trn"">' + value + '</span>'
                    //}
                }
                if (key == ""description"") {
                    key = 'توضیحات';
                    if (value == null || value == '')
                        value = 'ندارد'
                }
                if (key == 'guarantee') {
                    key = 'ضمانت/گارانتی';
                    if (value == null || value == '')
                        value =");
            WriteLiteral(@" 'ندارد'
                }
                if (key == 'nameOfSeller') {
                    key = 'نام فروشنده محصول';
                    if (value == null || value == '')
                        value = '<span class=""badge badge-danger"">این محصول قروشنده ندارد!</span>'
                    else
                        value = '<span class=""badge badge-light"">' + value + '</span>'
                }
                if (key == 'displayPrice') {
                    key = 'قیمت مصرف کننده';
                    value = row.displayPrice + ' <small>(ریال)</small> ';
                }
                html.push('<p><b>' + key + ':</b> ' + value + '</p>')
            }
            if (key == ""barcode"") {
                key = 'QRCode';
                if (value != null) {
                    html.push('<p><b>' + key + ':</b> <img src=""data:image/png;base64,' + value + '"" height=""200"" width=""200"" /></p>')
                }
                else {
                    value = '';
                    h");
            WriteLiteral("tml.push(\'<p><b>\' + key + \':</b> \' + value + \'</p>\')\r\n                }\r\n            }\r\n            if (key == \"imageName\") {\r\n                key = \'تصویر محصول\';\r\n                if (value != null) {\r\n                    var url = \'");
#nullable restore
#line 153 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Product\_ProductTable.cshtml"
                          Write(string.Format("{0}://{1}", Context.Request.Scheme, Context.Request.Host));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"' + '/images/' + value;
                    html.push('<p><b>' + key + ':</b> <img src=""' + url + '"" height=""200"" /></p>')
                }
            }
        })
        return html.join('')
    }

    function operateFormatter(value, row, index) {
        var access = """";
        if ('");
#nullable restore
#line 163 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Product\_ProductTable.cshtml"
        Write(securityTrimmingService.CanUserAccess(User,"Admin", "Product", "CreateOrUpdate"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\' == \'True\') {\r\n            access = access + \'<a class=\"text-success\" href=\"/Admin/Product/CreateOrUpdate?productId=\' + row.Id + \'\" title=\"ویرایش\"><i class=\"fa fa-edit\"></i></a>\';\r\n        }\r\n        if (\'");
#nullable restore
#line 166 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Product\_ProductTable.cshtml"
        Write(securityTrimmingService.CanUserAccess(User,"Admin", "Product", "Delete"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\' == \'True\') {\r\n            access = access + \'<button type=\"button\" class=\"btn-link text-danger\" data-toggle=\"ajax-modal\" data-url=");
#nullable restore
#line 167 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Product\_ProductTable.cshtml"
                                                                                                               Write(Url.Action("Delete", "Product"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"/?productId=' + row.Id + ' title=""حذف""><i class=""fa fa-trash""></i></button >';
        }
        return access;
    }

    function productStateFormatter(value, row, index) {
        return [
            //'<select class=""leave_response"" name=""leave""><option value=""1"">دمو</option><option value=""2"">در انبار</option><option value=""3"">آماده عرضه</option><option value=""4"">موجود نیست</option><option value=""5"">منقضی شده</option><option value=""6"">مرجوع شده</option><option value=""7"">آسیب دیده</option><option value=""8>بزودی</option></select>',

            '<select onchange=""ChangeProductState(' + ""'"" + row.Id + ""'"" + ')"" class=""btn btn-light btn-sm text-white dropdown-toggle select_ProductState""',
                    'type = ""button"" style = ""min-width: -webkit-fill-available;background-color: lightslategray;"">',
            '<option value=""0"">' + row.displayStates + '</option>',
                '<option value=""1"">دمو</option>',
                '<option value=""2"">در انبار</option>',
                '<op");
            WriteLiteral(@"tion value=""3"">آماده عرضه</option>',
                '<option value=""4"">موجود نیست</option>',
                '<option value=""5"">منقضی شده</option>',
                '<option value=""6"">مرجوع شده</option>',
                '<option value=""7"">آسیب دیده</option>',
                '<option value=""8"">بزودی</option>',
            '</select>',
        ].join('')
    }

    function discountFormatter(value, row, index) {
        var access = """";
        if ('");
#nullable restore
#line 193 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Product\_ProductTable.cshtml"
        Write(securityTrimmingService.CanUserAccess(User,"Admin", "Product", "RenderDiscount"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\' == \'True\') {\r\n            access = access + \'<button type=\"button\" class=\"btn btn-light btn-sm text-white\" data-toggle=\"ajax-modal\" data-url=");
#nullable restore
#line 194 "C:\Other\Training\Asp.Net Core\SnapMarket\SnapMarket\Areas\Admin\Views\Product\_ProductTable.cshtml"
                                                                                                                          Write(Url.Action("RenderDiscount", "Product"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"/?productId=' + row.Id + ' title=""تخفیف"">' + row.displayDiscount + '<i class=""fas fa-cc-discover""></i></button >';
        }
        return [
            access
        ].join('')
    }

    function isPreferedFormatter(value, row, index) {
        var attr = """";
        if (row.isPrefered == true) {
            attr = ""checked"";
        }
        return [
            '<label class=""custom-toggle mb-0""> <input type=""checkbox"" onclick=""IsPreferedOrNotPrefered(' + ""'"" + row.Id + ""'"" + ')"" id=""' + row.Id + '"" ' + attr + ' /><span class=""custom-toggle-slider rounded-circle""></span></label>'
        ].join('')
    }

    function stockFormatter(value, row, index) {
        var _stock = '';
        if (row.stock <= 1000) {
            _stock = '<a class=""btn btn-danger btn-sm text-white"">' + row.stock + '</a >'
        }
        else {
            _stock = '<a class=""btn btn-info btn-sm text-white"">' + row.stock + '</a >'
        }
        return [
            _stock
        ].join('')
 ");
            WriteLiteral(@"   }

    function commentFormatter(value, row, index) {
        return [
            '<a href=""/Admin/Comment/Index?productId=' + row.Id + '"" class=""btn btn-primary btn-sm text-white"">',
            row.numberOfComments, ' <i class=""fa fa-comments mr-1""></i>',
            '</a >'
        ].join('')
    }

    function priceFormatter(value, row, index) {
        if (row.actionDiscount == null || row.actionDiscount == """")
            row.actionDiscount = ""۰"";
        if (row.percentDiscount == null || row.percentDiscount == """")
            row.percentDiscount = ""۰"";
        return row.displayPrice + "" "" + '<small>(' + row.actionDiscount + "" - "" + row.percentDiscount + "" %"" +')</small>';
    }

    function totalTextFormatter(data) {
        return 'تعداد'
    }

    function totalNameFormatter(data) {
        return data.length
    }

    function initTable() {
        $table.bootstrapTable('destroy').bootstrapTable({
            height: 650,
            locale: 'fa-IR',
         ");
            WriteLiteral(@"   columns: [
                [{
                    field: 'state',
                    checkbox: true,
                    rowspan: 2,
                    align: 'center',
                    valign: 'middle',
                    name: 'getSelections'
                },{
                    title: 'ردیف',
                    field: 'ردیف',
                    rowspan: 2,
                    align: 'center',
                    valign: 'middle',
                    footerFormatter: totalTextFormatter
                },{
                    title: 'جزئیات اطلاعات محصولات',
                    colspan: 10,
                    align: 'center'
                }],
                [{
                    field: 'shortName',
                    title: 'محصول',
                    sortable: true,
                    footerFormatter: totalNameFormatter
                },{
                    field: 'nameOfCategories',
                    title: 'دسته',
                    sortable: false,
");
            WriteLiteral(@"                    align: 'center'
                },{
                    field: 'displayPrice',
                    title: 'قیمت<small>(ریال)</small>',
                    sortable: true,
                    align: 'center',
                    formatter: priceFormatter
                },{
                    field: 'persianInsertTime',
                    title: 'تاریخ تولید',
                    sortable: true,
                    align: 'center'
                },{
                    field: 'stock',
                    title: 'موجودی',
                    sortable: true,
                    align: 'center',
                    formatter: stockFormatter
                },{
                    field: 'numberOfComments',
                    title: 'نظرات',
                    sortable: false,
                    align: 'center',
                    formatter: commentFormatter
                },{
                    field: 'isPrefered',
                    title: '<small>محصول پیش");
            WriteLiteral(@"نهادی</small>',
                    align: 'center',
                    formatter: isPreferedFormatter
                },{
                    field: 'displayDiscount',
                    title: 'تخفیف',
                    sortable: false,
                    align: 'center',
                    formatter: discountFormatter
                },{
                    field: 'displayStates',
                    title: 'وضعیت',
                    sortable: false,
                    align: 'center',
                    formatter: productStateFormatter
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
                $remove.prop('disabled', !$table.bootstrapTable('getSelections')");
            WriteLiteral(".length)\r\n                selections = getIdSelections()\r\n        })\r\n    }\r\n\r\n    $(function () {\r\n        initTable()\r\n        $(\'#locale\').change(initTable)\r\n    })\r\n</script>\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Byte[]> Html { get; private set; }
    }
}
#pragma warning restore 1591
