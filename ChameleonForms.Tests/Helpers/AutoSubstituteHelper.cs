using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text.Encodings.Web;
using System.Web;
using Autofac;
using AutofacContrib.NSubstitute;
using ChameleonForms.Example;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NSubstitute;

namespace ChameleonForms.Tests.Helpers
{
    public static class AutoSubstituteContainer
    {
        public static AutoSubstitute Create()
        {
            return Create(null);
        }

        public static AutoSubstitute Create(Action<ContainerBuilder> createAction)
        {
            AutoSubstitute autoSubstitute = createAction != null
                ? new AutoSubstitute(createAction)
                : new AutoSubstitute();

            var httpContext = Substitute.For<HttpContext>();
            var services = Substitute.For<IServiceProvider>();
            httpContext.RequestServices = services;
            services.GetService(null).Returns(x =>
            {
                return autoSubstitute.Container.Resolve(x.ArgAt<Type>(0));
            });

            autoSubstitute.Provide(httpContext);

            var request = Substitute.For<HttpRequest>();
            var formParameters = new FormCollection(new Dictionary<string, StringValues>());
            request.Form.Returns(formParameters);
            var qsParameters = new QueryCollection();
            request.Query.Returns(qsParameters);
            var headers = new HeaderDictionary { { "Host", "localhost" } };
            request.Headers.Returns(headers);
            request.Cookies.Returns(new RequestCookieCollection());
            autoSubstitute.Provide(request);
            httpContext.Request.Returns(request);

            var response = Substitute.For<HttpResponse>();
            //response.Cookies.Returns(new ResponseCookies());
            autoSubstitute.Provide(response);
            httpContext.Response.Returns(response);

            var routeData = new RouteData();

            var actionDescriptor = new ControllerActionDescriptor();
            var requestContext = new ActionContext(httpContext, routeData, actionDescriptor);
            autoSubstitute.Provide(requestContext);

            autoSubstitute.Provide(HtmlEncoder.Default);
            autoSubstitute.Provide(UrlEncoder.Default);
            //var actionExecutingContext = Substitute.For<ActionExecutingContext>(requestContext);
            //actionExecutingContext.HttpContext.Returns(httpContext);
            //actionExecutingContext.RouteData.Returns(routeData);
            //autoSubstitute.Provide(actionExecutingContext);

            //var actionExecutedContext = Substitute.For<ActionExecutedContext>(requestContext);
            //actionExecutedContext.HttpContext.Returns(httpContext);
            //actionExecutedContext.RouteData.Returns(routeData);
            //autoSubstitute.Provide(actionExecutedContext);

            var controller = Substitute.For<ControllerBase>();
            autoSubstitute.Provide(controller);
            //actionExecutingContext.Controller.Returns(controller);

            var controllerContext = new ControllerContext(requestContext);
            controllerContext.HttpContext = httpContext;
            controllerContext.RouteData = routeData;
            autoSubstitute.Provide(controllerContext);
            controller.ControllerContext = controllerContext;

            IOptions<MvcOptions> mvcOptions = Substitute.For<IOptions<MvcOptions>>();
            mvcOptions.Value.Returns(new MvcOptions());

            IOptions<MvcDataAnnotationsLocalizationOptions> dataAnnotationOptions = Substitute.For<IOptions<MvcDataAnnotationsLocalizationOptions>>();
            dataAnnotationOptions.Value.Returns(new MvcDataAnnotationsLocalizationOptions());

            autoSubstitute.Provide(mvcOptions);

            var anotationsMetadataProvider = autoSubstitute.Resolve<DataAnnotationsMetadataProvider>();
            autoSubstitute.Provide<IBindingMetadataProvider>(anotationsMetadataProvider);
            autoSubstitute.Provide<IMetadataDetailsProvider>(anotationsMetadataProvider);
            autoSubstitute.Provide<IDisplayMetadataProvider>(anotationsMetadataProvider);
            autoSubstitute.Provide<IValidationMetadataProvider>(anotationsMetadataProvider);


            var metadataProvider = autoSubstitute.Resolve<DefaultModelMetadataProvider>();
            autoSubstitute.Provide<IModelMetadataProvider>(metadataProvider);

            var iView = Substitute.For<IView>();
            autoSubstitute.Provide(iView);
            var viewDataDictionary = new ViewDataDictionary(metadataProvider, new ModelStateDictionary());
            autoSubstitute.Provide(viewDataDictionary);

            var textWriter = Substitute.For<TextWriter>();
            autoSubstitute.Provide(textWriter);

            IValidationAttributeAdapterProvider validationAttributeAdapterProvider = new ValidationAttributeAdapterProvider();

            var mvcViewOptionsWrap = Substitute.For<IOptions<MvcViewOptions>>();

            MvcViewOptionsSetup optionsSetup = new MvcViewOptionsSetup(dataAnnotationOptions, validationAttributeAdapterProvider);
            var mvcViewOptions = new MvcViewOptions();
            mvcViewOptionsWrap.Value.Returns(mvcViewOptions);
            optionsSetup.Configure(mvcViewOptions);
            autoSubstitute.Provide<IOptions<MvcViewOptions>>(mvcViewOptionsWrap);
            autoSubstitute.Provide<ValidationHtmlAttributeProvider>(autoSubstitute.Resolve<DefaultValidationHtmlAttributeProvider>());
            autoSubstitute.Provide<IHtmlGenerator>(autoSubstitute.Resolve<DefaultHtmlGenerator>());

            var tempDataDictionary = new TempDataDictionary(httpContext, Substitute.For<ITempDataProvider>());

            //var viewContext = new ViewContext(controllerContext, iView, viewDataDictionary, tempDataDictionary, textWriter, new HtmlHelperOptions())
            //{
            //    HttpContext = httpContext,
            //    RouteData = routeData,
            //    //RequestContext = requestContext,
            //    //Controller = controller
            //};
            //autoSubstitute.Provide(viewContext);

            //var htmlHelper = new HtmlHelper(viewContext, new ViewPage());
            //autoSubstitute.Provide(htmlHelper);

            autoSubstitute.Provide(new UrlHelper(autoSubstitute.Resolve<ActionContext>()));

            return autoSubstitute;
        }

        public static T GetController<T>(this AutoSubstitute autoSubstitute) where T : Controller
        {
            var controller = autoSubstitute.Resolve<T>();
            controller.ControllerContext = autoSubstitute.Resolve<ControllerContext>();
            controller.Url = autoSubstitute.Resolve<UrlHelper>();
            return controller;
        }
    }
}
