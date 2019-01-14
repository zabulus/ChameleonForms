using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChameleonForms.ModelBinders
{
    class DateTimeModelBinderProvider<T> : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return new DateTimeModelBinder<T>();
        }
    }
}
