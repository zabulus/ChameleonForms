using ChameleonForms.AcceptanceTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using RazorPagesProject.Tests.Helpers;
using AngleSharp.Dom.Html;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using ChameleonForms.AcceptanceTests.ModelBinding.Pages;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System;
using ChameleonForms.Example.Controllers;
using ChameleonForms.AcceptanceTests.ModelBinding.Pages.Fields;

namespace ChameleonForms.AcceptanceTests.ModelBinding
{
    public static class PropertyExtensions
    {
        public static bool IsReadonly(this PropertyInfo property)
        {
            var attrs = property.GetCustomAttributes(typeof(ReadOnlyAttribute), false);
            if (attrs.Length > 0)
                return ((ReadOnlyAttribute)attrs[0]).IsReadOnly;
            return false;
        }
    }

    public class ModelBindingShould : IClassFixture<WebApplicationFactory<ChameleonForms.Example.Startup>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<ChameleonForms.Example.Startup>
            _factory;

        public ModelBindingShould(WebApplicationFactory<ChameleonForms.Example.Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Function_correctly_with_default_form()
        {
            var enteredViewModel = ObjectMother.ModelBindingViewModels.BasicValid;

            var page = await _client.GetPageAsync<ModelBindingExamplePage>("/ExampleForms/ModelBindingExample");
            page = await page.SubmitAsync(_client, enteredViewModel);

            //List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
            //InputModel(enteredViewModel, "", values);

            //var resp = await _client.SendAsync((IHtmlFormElement)content.QuerySelector("form"), (IHtmlElement)content.QuerySelector("button"), values);

            //content = await HtmlHelpers.GetDocumentAsync(resp);
            //var newValues = GetFormValues<ModelBindingViewModel>(content);
            IsSame.ViewModelAs(enteredViewModel, page.GetFormValues());
            Assert.False(page.HasValidationErrors());
        }

        //private T GetFormValues<T>(IHtmlDocument document)
        //{
        //    return (T)GetFormValues(document, typeof(T), "");
        //}

        //private object GetFormValues(IHtmlDocument document, Type typeOfModel, string prefix)
        //{
        //       var vm = Activator.CreateInstance(typeOfModel);
        //    foreach (var property in vm.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        //    {
        //        var propertyName = string.IsNullOrEmpty(prefix)
        //            ? property.Name
        //            : string.Format("{0}.{1}", prefix, property.Name);

        //        if (property.IsReadonly())
        //            continue;

        //        if (!property.PropertyType.IsValueType && property.PropertyType != typeof(string) && !typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
        //        {
        //            var childValue = GetFormValues(document, property.PropertyType, propertyName);
        //            property.SetValue(vm, childValue, null);
        //            continue;
        //        }

        //        var elements = document.GetElementsByName(propertyName);
        //        var format = GetFormatStringForProperty(property);
        //        property.SetValue(vm, FieldFactory.Create(elements).Get(new ModelFieldType(property.PropertyType, format)), null);
        //    }
        //    return vm;
        //}


        //private void InputModel(object model, string prefix, List<KeyValuePair<string, string>> values)
        //{
        //    foreach (var property in model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        //    {
        //        var propertyName = string.IsNullOrEmpty(prefix)
        //            ? property.Name
        //            : string.Format("{0}.{1}", prefix, property.Name);

        //        if (property.IsReadonly())
        //        {
        //            continue;
        //        }

        //        if (!property.PropertyType.IsValueType && property.PropertyType != typeof(string) && !typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
        //        {
        //            InputModel(property.GetValue(model, null), propertyName, values);
        //            continue;
        //        }

        //        var format = GetFormatStringForProperty(property);
        //        values.Add(new KeyValuePair<string, string>(propertyName, new ModelFieldValue(property.GetValue(model, null), format).Value));
        //    }
        //}

        //private static string GetFormatStringForProperty(PropertyInfo property)
        //{
        //    var displayFormatAttr = property.GetCustomAttributes(typeof(DisplayFormatAttribute), false);
        //    var format = "{0}";
        //    if (displayFormatAttr.Any())
        //        format = displayFormatAttr.Cast<DisplayFormatAttribute>().First().DataFormatString;
        //    return format;
        //}


        //[Fact]
        //public void Function_correctly_with_checkbox_and_radio_lists()
        //{
        //    var enteredViewModel = ObjectMother.ModelBindingViewModels.BasicValid;

        //    var page = Host.Instance.NavigateToInitialPage<HomePage>()
        //        .GoToModelBindingExamplePage2()
        //        .Submit(enteredViewModel);

        //    Assert.That(page.GetFormValues(), IsSame.ViewModelAs(enteredViewModel));
        //    Assert.That(page.HasValidationErrors(), Is.False, "There are validation errors on the page");
        //}
    }
}
