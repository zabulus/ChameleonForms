using ChameleonForms.Attributes;
using ChameleonForms.ModelBinders;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChameleonForms
{
    public static class ServiceCollectionExtensions
    {
        public static void AddChameleonForms(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException("services");
            }

            services.AddTransient<IModelBinderProvider, DateTimeModelBinderProvider<DateTime>>();
            services.AddTransient<IModelBinderProvider, DateTimeModelBinderProvider<DateTime?>>();
            services.AddTransient<IModelBinderProvider, FlagsEnumModelBinderProvider>();
            services.AddTransient<IValidationAttributeAdapterProvider, RequiredFlagsEnumAttributeAdapterProvider>();
        }
    }
}
