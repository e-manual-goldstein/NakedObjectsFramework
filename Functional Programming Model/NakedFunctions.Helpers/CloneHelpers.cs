using System;
using System.Collections.Generic;
using System.Linq;
using Remutable;
using System.Linq.Expressions;
using System.Reflection;

namespace NakedFunctions
{
    public static class CloneHelpers
    {


        public static TInstance With<TInstance, TValue>(
            this TInstance source,
            Expression<Func<TInstance, TValue>> expression,
            TValue value)
        {

            var instanceType = source.GetType();

            var cc = instanceType.GetConstructors().Single(c => c.GetParameters().Any<ParameterInfo>());

            var config = new ActivationConfiguration().Configure(cc);

            var rm = new Remute(config);

            return rm.With(source, expression, value);
        }
    }
}
