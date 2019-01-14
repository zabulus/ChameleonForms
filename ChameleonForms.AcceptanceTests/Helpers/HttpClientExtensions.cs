using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using ChameleonForms.AcceptanceTests.Helpers.Pages;
using Xunit;

namespace RazorPagesProject.Tests.Helpers
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> SendAsync(
            this HttpClient client,
            IHtmlFormElement form,
            IHtmlElement submitButton)
        {
            return client.SendAsync(form, submitButton, new Dictionary<string, string>());
        }

        public static Task<HttpResponseMessage> SendAsync(
            this HttpClient client,
            IHtmlFormElement form,
            IEnumerable<KeyValuePair<string, string>> formValues)
        {
            var submitElement = Assert.Single(form.QuerySelectorAll("[type=submit]"));
            var submitButton = Assert.IsAssignableFrom<IHtmlElement>(submitElement);

            return client.SendAsync(form, submitButton, formValues);
        }

        public static async Task<HttpResponseMessage> SendAsync(this HttpClient client
            , IHtmlFormElement form
            , IHtmlElement submitButton
            , IEnumerable<KeyValuePair<string, string>> formValues
            )
        {
            return await SendAsync(client, new[] { form }, submitButton, formValues);
        }

        public static Task<HttpResponseMessage> SendAsync(
            this HttpClient client,
            IEnumerable<IHtmlFormElement> forms,
            IHtmlElement submitButton,
            IEnumerable<KeyValuePair<string, string>> formValues)
        {
            IHtmlFormElement form = null;
            foreach (var kvp in formValues)
            {
                var el = forms.Select(x => new { el = x.Elements[kvp.Key], form = x }).SingleOrDefault(x => x.el != null);
                if (el == null)
                {
                    Assert.False(el == null, kvp.Key);
                }
                else if (el.el is IHtmlInputElement inputElement)
                {
                    inputElement.Value = kvp.Value;
                    form = el.form;
                }
                else if (el.el is IHtmlSelectElement selectElement)
                {
                    selectElement.Value = kvp.Value;
                    form = el.form;
                }
                else
                {
                    Assert.False(true, el.el.GetType().Name);
                }
            }

            var submit = form.GetSubmission(submitButton);
            var target = (Uri)submit.Target;
            if (submitButton.HasAttribute("formaction"))
            {
                var formaction = submitButton.GetAttribute("formaction");
                target = new Uri(formaction, UriKind.Relative);
            }
            var submision = new HttpRequestMessage(new HttpMethod(submit.Method.ToString()), target)
            {
                Content = new StreamContent(submit.Body)
            };

            foreach (var header in submit.Headers)
            {
                submision.Headers.TryAddWithoutValidation(header.Key, header.Value);
                submision.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return client.SendAsync(submision);
        }

        public static async Task<T> GetPageAsync<T>(this HttpClient self, string url) where T : ChameleongFormsPageBase
        {
            var html = await self.GetAsync(url);
            var content = await HtmlHelpers.GetDocumentAsync(html);
            return (T)Activator.CreateInstance(typeof(T), content);
        }
    }
}
