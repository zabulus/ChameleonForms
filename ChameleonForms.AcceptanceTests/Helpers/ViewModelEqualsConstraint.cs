﻿using System.Collections;
using System.Linq;
using System.Reflection;
using ChameleonForms.AcceptanceTests.ModelBinding.Pages;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace ChameleonForms.AcceptanceTests.Helpers
{
    public static class IsSame
    {
        public static Constraint ViewModelAs(object expectedViewModel)
        {
            return new ViewModelEqualsConstraint(expectedViewModel);
        }
    }

    public class ViewModelEqualsConstraint : Constraint
    {
        private readonly object _expectedViewModel;

        public ViewModelEqualsConstraint(object expectedViewModel)
        {
            _expectedViewModel = expectedViewModel;
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actualViewModel)
        {
            foreach (var property in actualViewModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.IsReadonly())
                    continue;
                var expectedValue = property.GetValue(_expectedViewModel, null);
                var actualValue = property.GetValue(actualViewModel, null);

                if (!property.PropertyType.IsValueType && property.PropertyType != typeof(string) && !typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    Assert.That(actualValue, IsSame.ViewModelAs(expectedValue));
                    continue;
                }

                if (expectedValue is IEnumerable && !(expectedValue as IEnumerable).Cast<object>().Any())
                    Assert.That(actualValue, Is.Null.Or.Empty);
                else
                    Assert.That(actualValue, Is.EqualTo(expectedValue), property.Name);
            }

            return new ConstraintResult(this, actualViewModel.GetType(), true);
        }

        protected override string GetStringRepresentation()
        {
            return "<ViewModelEquals>";
        }
    }
}
