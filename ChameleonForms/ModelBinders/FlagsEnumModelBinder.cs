using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ChameleonForms.ModelBinders
{
    /// <summary>
    /// Binds a flags enum in a model.
    /// </summary>
    public class FlagsEnumModelBinder : IModelBinder
    {
        /// <inheritdoc />
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var underlyingType = Nullable.GetUnderlyingType(bindingContext.ModelType) ?? bindingContext.ModelType;
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var enumValues = value.Values;
            
            var enumValueAsLong = 0L;
            var error = true;

            foreach (var enumValue in enumValues)
            {
                if (Enum.IsDefined(underlyingType, enumValue))
                {
                    var valueAsEnum = Enum.Parse(underlyingType, enumValue, true);
                    enumValueAsLong |= Convert.ToInt64(valueAsEnum);
                    error = false;
                }
                else
                {
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, string.Format("The value '{0}' is not valid for {1}.", enumValue, bindingContext.ModelMetadata.DisplayName ?? bindingContext.ModelMetadata.PropertyName));
                }
            }

            if (error)
            {
                bindingContext.Result = ModelBindingResult.Success(Activator.CreateInstance(bindingContext.ModelType));
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Success(Enum.Parse(underlyingType, enumValueAsLong.ToString()));
            }
        }
    }
}
