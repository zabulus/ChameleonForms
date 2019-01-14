using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChameleonForms.ModelBinders
{
    class FlagsEnumModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return new FlagsEnumModelBinder();
        }
    }
}
