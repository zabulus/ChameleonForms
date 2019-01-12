using ChameleonForms.Attributes;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace ChameleonForms.Tests.Helpers
{
    internal class CustomValidationAttributeAdapterProvider : ValidationAttributeAdapterProvider, IValidationAttributeAdapterProvider
    {
        IAttributeAdapter IValidationAttributeAdapterProvider.GetAttributeAdapter(ValidationAttribute attribute
            , IStringLocalizer stringLocalizer
            )
        {
            var ret = base.GetAttributeAdapter(attribute, stringLocalizer);
            if (ret == null)
            {
                var type = attribute.GetType();
                if (type == typeof(RequiredFlagsEnumAttribute))
                {
                    ret = new RequiredAttributeAdapter((RequiredAttribute)attribute, stringLocalizer);
                }
            }

            return ret;
        }
    }
}