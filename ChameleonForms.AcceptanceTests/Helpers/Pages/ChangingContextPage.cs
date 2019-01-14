﻿using AngleSharp.Dom.Html;
using ChameleonForms.AcceptanceTests.Helpers.Pages;
using ChameleonForms.Example.Controllers;
using RazorPagesProject.Tests.Helpers;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChameleonForms.AcceptanceTests.ModelBinding.Pages
{
    public class ChangingContextPage : ChameleonFormsPage<ParentViewModel>
    {
        public ChangingContextPage(IHtmlDocument content) : base(content)
        {
        }

        public BasicViewModel ReadDifferentModel()
        {
            return GetComponent<PageAsDifferentModel>().GetFormValues();
        }

        public async Task<ChangingContextPage> PostDifferentModelAsync(HttpClient client, BasicViewModel vm)
        {
            return new ChangingContextPage(await HtmlHelpers.GetDocumentAsync(await client.SendAsync((IHtmlFormElement)Content.QuerySelector("form.different-model")
                , (IHtmlButtonElement)Content.QuerySelector("button[type=submit].different-model")
                , PageAsDifferentModel.InputModel(vm, "different-model")
                )));
        }

        public ChildViewModel ReadChildModel()
        {
            return GetComponent<PageAsChildModel>().GetFormValues();
        }

        public async Task<ChangingContextPage> PostChildModelAsync(HttpClient client, ChildViewModel vm)
        {
            return new ChangingContextPage(await HtmlHelpers.GetDocumentAsync(await client.SendAsync((IHtmlFormElement)Content.QuerySelector("form.child-model")
                , (IHtmlButtonElement)Content.QuerySelector("button[type=submit].child-model")
                , PageAsChildModel.InputModel(vm, "child-model")
                )));
        }

        public ParentViewModel ReadParentModel()
        {
            return GetFormValues();
        }

        public async Task<ChangingContextPage> PostParentModelAsync(HttpClient client, ParentViewModel vm)
        {
            return new ChangingContextPage(await HtmlHelpers.GetDocumentAsync(await client.SendAsync((IHtmlFormElement)Content.QuerySelector("form.parent-model"), (IHtmlButtonElement)Content.QuerySelector("button[type=submit].parent-model"), InputModel(vm))));
        }

        public class PageAsDifferentModel : ChameleonFormsPage<BasicViewModel>
        {
            public PageAsDifferentModel(IHtmlDocument content) : base(content)
            {
            }
        }

        public class PageAsChildModel : ChameleonFormsPage<ChildViewModel>
        {
            public PageAsChildModel(IHtmlDocument content) : base(content)
            {
            }
        }
    }
}
