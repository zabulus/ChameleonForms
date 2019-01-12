using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace ChameleonForms.ModelBinders
{
    /// <summary>
    /// Binds a datetime in a model using the display format string.
    /// </summary>
    public class DateTimeModelBinder<T> : SimpleTypeModelBinder, IModelBinder
    {
        public DateTimeModelBinder() : base(typeof(T), NullLoggerFactory.Instance)
        {
        }

        /// <inheritdoc />
        async Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            var underlyingType = Nullable.GetUnderlyingType(bindingContext.ModelType) ?? bindingContext.ModelType;

            if(underlyingType != typeof(DateTime))
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "DateTimeModelBinder used not for DateTime property");
            }

            var formatString = bindingContext.ModelMetadata.DisplayFormatString;
            if (string.IsNullOrEmpty(formatString))
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "DisplayFormatString is null or empty");
                return;
            }

            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var submittedValue = value == default(ValueProviderResult) ? null : value.FirstValue;

            if (string.IsNullOrEmpty(submittedValue))
            {
                await base.BindModelAsync(bindingContext);
                return;
            }

            if (!DateTime.TryParseExact(submittedValue
                , formatString.Replace("{0:", "").Replace("}", "")
                , CultureInfo.CurrentCulture.DateTimeFormat
                , DateTimeStyles.None
                , out DateTime parsedDate
                ))
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName
                    , string.Format("The value '{0}' is not valid for {1}. Format of date is {2}.", submittedValue, bindingContext.ModelMetadata.DisplayName ?? bindingContext.ModelMetadata.PropertyName, formatString)
                    );
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Success(parsedDate);
            }
        }
    }
}
