// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core.Configuration;
using NakedObjects.Meta.Component;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Meta.Test;
using NakedObjects.ParallelReflect.Component;
using NakedObjects.ParallelReflect.FacetFactory;
using NakedObjects.ParallelReflect.Test;
using NakedObjects.ParallelReflect.TypeFacetFactory;


namespace NakedFunctions.Reflect.Test {

    public class SimpleClass {
        public virtual SimpleClass SimpleProperty { get; set; }
    }

    public class NavigableClass {
        public SimpleClass SimpleProperty { get; set; }
    }

    public static class SimpleFunctions {
        public static SimpleClass SimpleFunction(SimpleClass target) {
            return target;
        }
    }

    public static class SimpleInjectedFunctions {
        public static SimpleClass SimpleInjectedFunction(IQueryable<SimpleClass> injected) {
            return injected.First();
        }
    }

    public static class TupleFunctions {
        public static Tuple<SimpleClass, SimpleClass> TupleFunction(IQueryable<SimpleClass> injected) {
            return new Tuple<SimpleClass, SimpleClass>(injected.First(), injected.First());
        }
    }

    public static class ValueTupleFunctions
    {
        public static (SimpleClass, SimpleClass) TupleFunction(IQueryable<SimpleClass> injected)
        {
            return (injected.First(), injected.First());
        }
    }


    [TestClass]
    public class ReflectorTest {
        protected IUnityContainer GetContainer() {
            ImmutableSpecFactory.ClearCache();
            var c = new UnityContainer();
            RegisterTypes(c);

            //c.RegisterType<IFunctionalReflector, OldReflector>(new ContainerControlledLifetimeManager());

            return c;
        }

        protected virtual void RegisterFacetFactories(IUnityContainer container) {
            int order = 0;
            container.RegisterType<IFacetFactory, FallbackFacetFactory>("FallbackFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, IteratorFilteringFacetFactory>("IteratorFilteringFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, SystemClassMethodFilteringFactory>("UnsupportedParameterTypesMethodFilteringFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RemoveSuperclassMethodsFacetFactory>("RemoveSuperclassMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RemoveDynamicProxyMethodsFacetFactory>("RemoveDynamicProxyMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RemoveEventHandlerMethodsFacetFactory>("RemoveEventHandlerMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TypeMarkerFacetFactory>("TypeMarkerFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            // must be before any other FacetFactories that install MandatoryFacet.class facets
            container.RegisterType<IFacetFactory, MandatoryDefaultFacetFactory>("MandatoryDefaultFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PropertyValidateDefaultFacetFactory>("PropertyValidateDefaultFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ComplementaryMethodsFilteringFacetFactory>("ComplementaryMethodsFilteringFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ActionMethodsFacetFactory>("ActionMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, CollectionFieldMethodsFacetFactory>("CollectionFieldMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PropertyMethodsFacetFactory>("PropertyMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, IconMethodFacetFactory>("IconMethodFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, CallbackMethodsFacetFactory>("CallbackMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TitleMethodFacetFactory>("TitleMethodFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ValidateObjectFacetFactory>("ValidateObjectFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ComplexTypeAnnotationFacetFactory>("ComplexTypeAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ViewModelFacetFactory>("ViewModelFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, BoundedAnnotationFacetFactory>("BoundedAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, EnumFacetFactory>("EnumFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ActionDefaultAnnotationFacetFactory>("ActionDefaultAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PropertyDefaultAnnotationFacetFactory>("PropertyDefaultAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DescribedAsAnnotationFacetFactory>("DescribedAsAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DisabledAnnotationFacetFactory>("DisabledAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PasswordAnnotationFacetFactory>("PasswordAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ExecutedAnnotationFacetFactory>("ExecutedAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PotencyAnnotationFacetFactory>("PotencyAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PageSizeAnnotationFacetFactory>("PageSizeAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, HiddenAnnotationFacetFactory>("HiddenAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, HiddenDefaultMethodFacetFactory>("HiddenDefaultMethodFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DisableDefaultMethodFacetFactory>("DisableDefaultMethodFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, AuthorizeAnnotationFacetFactory>("AuthorizeAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ValidateProgrammaticUpdatesAnnotationFacetFactory>("ValidateProgrammaticUpdatesAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ImmutableAnnotationFacetFactory>("ImmutableAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, MaxLengthAnnotationFacetFactory>("MaxLengthAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RangeAnnotationFacetFactory>("RangeAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, MemberOrderAnnotationFacetFactory>("MemberOrderAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, MultiLineAnnotationFacetFactory>("MultiLineAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, NamedAnnotationFacetFactory>("NamedAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, NotPersistedAnnotationFacetFactory>("NotPersistedAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ProgramPersistableOnlyAnnotationFacetFactory>("ProgramPersistableOnlyAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, OptionalAnnotationFacetFactory>("OptionalAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RequiredAnnotationFacetFactory>("RequiredAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PluralAnnotationFacetFactory>("PluralAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DefaultNamingFacetFactory>("DefaultNamingFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++)); // must come after Named and Plural factories
            container.RegisterType<IFacetFactory, ConcurrencyCheckAnnotationFacetFactory>("ConcurrencyCheckAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ContributedActionAnnotationFacetFactory>("ContributedActionAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, FinderActionFacetFactory>("FinderActionFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            // must come after any facets that install titles
            container.RegisterType<IFacetFactory, MaskAnnotationFacetFactory>("MaskAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            // must come after any facets that install titles, and after mask
            // if takes precedence over mask.
            container.RegisterType<IFacetFactory, RegExAnnotationFacetFactory>("RegExAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TypeOfAnnotationFacetFactory>("TypeOfAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TableViewAnnotationFacetFactory>("TableViewAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TypicalLengthDerivedFromTypeFacetFactory>("TypicalLengthDerivedFromTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TypicalLengthAnnotationFacetFactory>("TypicalLengthAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, EagerlyAnnotationFacetFactory>("EagerlyAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PresentationHintAnnotationFacetFactory>("PresentationHintAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, BooleanValueTypeFacetFactory>("BooleanValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ByteValueTypeFacetFactory>("ByteValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, SbyteValueTypeFacetFactory>("SbyteValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ShortValueTypeFacetFactory>("ShortValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, IntValueTypeFacetFactory>("IntValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, LongValueTypeFacetFactory>("LongValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, UShortValueTypeFacetFactory>("UShortValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, UIntValueTypeFacetFactory>("UIntValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ULongValueTypeFacetFactory>("ULongValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, FloatValueTypeFacetFactory>("FloatValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DoubleValueTypeFacetFactory>("DoubleValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DecimalValueTypeFacetFactory>("DecimalValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, CharValueTypeFacetFactory>("CharValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DateTimeValueTypeFacetFactory>("DateTimeValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TimeValueTypeFacetFactory>("TimeValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, StringValueTypeFacetFactory>("StringValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, GuidValueTypeFacetFactory>("GuidValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, EnumValueTypeFacetFactory>("EnumValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, FileAttachmentValueTypeFacetFactory>("FileAttachmentValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ImageValueTypeFacetFactory>("ImageValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ArrayValueTypeFacetFactory<byte>>("ArrayValueTypeFacetFactory<byte>", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, CollectionFacetFactory>("CollectionFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order)); // written to not trample over TypeOf if already installed

            container.RegisterType<IFacetFactory, FunctionsFacetFactory>("FunctionsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));

        }

        protected virtual void RegisterTypes(IUnityContainer container) {
            RegisterFacetFactories(container);

            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(new InjectionConstructor());
            container.RegisterType<IClassStrategy, DefaultClassStrategy>();
            container.RegisterType<IReflector, ParallelReflector>();
            container.RegisterType<IMetamodel, Metamodel>();
            container.RegisterType<IMetamodelBuilder, Metamodel>();
            container.RegisterType<IMenuFactory, CacheTest.NullMenuFactory>();
        }


        private static ReflectorConfiguration RegisterObjectConfig(IUnityContainer container) {
            var rc = new ReflectorConfiguration(new Type[] { }, new Type[] { }, new string[] { "NakedFunctions" });
            rc.SupportedSystemTypes.Clear();


            container.RegisterInstance<IReflectorConfiguration>(rc);
            return rc;
        }



        [TestMethod]
        public void ReflectNoTypes() {
            IUnityContainer container = GetContainer();
            ReflectorConfiguration.NoValidate = true;
            RegisterObjectConfig(container);

            var rc = new FunctionalReflectorConfiguration(new Type[0], new Type[0]);

            container.RegisterInstance<IFunctionalReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            Assert.IsFalse(reflector.AllObjectSpecImmutables.Any());
        }

        [TestMethod]
        public void ReflectSimpleType() {
            IUnityContainer container = GetContainer();
            ReflectorConfiguration.NoValidate = true;
            RegisterObjectConfig(container);

            var rc = new FunctionalReflectorConfiguration(new[] {typeof(SimpleClass)}, new Type[0]);

            container.RegisterInstance<IFunctionalReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(1, specs.Length);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
        }

        [TestMethod]
        public void ReflectSimpleFunction() {
            IUnityContainer container = GetContainer();
            ReflectorConfiguration.NoValidate = true;
            RegisterObjectConfig(container);

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(SimpleClass) }, new Type[] {typeof (SimpleFunctions)});

            container.RegisterInstance<IFunctionalReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(2, specs.Length);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleFunctions), specs);
        }

        [TestMethod]
        public void ReflectTupleFunction() {
            IUnityContainer container = GetContainer();
            ReflectorConfiguration.NoValidate = true;
            var rcO = RegisterObjectConfig(container);
            rcO.SupportedSystemTypes.Add(typeof(IQueryable<>));

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(SimpleClass) }, new Type[] { typeof(TupleFunctions) });

            container.RegisterInstance<IFunctionalReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(3, specs.Length);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
            AbstractReflectorTest.AssertSpec(typeof(TupleFunctions), specs);
            AbstractReflectorTest.AssertSpec(typeof(IQueryable<>), specs);
        }

        [TestMethod]
        public void ReflectValueTupleFunction()
        {
            IUnityContainer container = GetContainer();
            ReflectorConfiguration.NoValidate = true;
            var rcO = RegisterObjectConfig(container);
            rcO.SupportedSystemTypes.Add(typeof(IQueryable<>));

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(SimpleClass) }, new Type[] { typeof(ValueTupleFunctions) });

            container.RegisterInstance<IFunctionalReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(3, specs.Length);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
            AbstractReflectorTest.AssertSpec(typeof(ValueTupleFunctions), specs);
            AbstractReflectorTest.AssertSpec(typeof(IQueryable<>), specs);
        }

        [TestMethod]
        public void ReflectSimpleInjectedFunction() {
            IUnityContainer container = GetContainer();
            ReflectorConfiguration.NoValidate = true;
            var rcO = RegisterObjectConfig(container);
            rcO.SupportedSystemTypes.Add(typeof(IQueryable<>));

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(SimpleClass) }, new Type[] { typeof(SimpleInjectedFunctions) });

            container.RegisterInstance<IFunctionalReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();

            
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            Assert.AreEqual(3, specs.Length);
            AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
            AbstractReflectorTest.AssertSpec(typeof(SimpleInjectedFunctions), specs);
            AbstractReflectorTest.AssertSpec(typeof(IQueryable<>), specs);

            Assert.AreEqual(1, specs[0].ObjectActions.Count);
        }


        [TestMethod]
        public void ReflectNavigableType() {
            IUnityContainer container = GetContainer();
            ReflectorConfiguration.NoValidate = true;
            RegisterObjectConfig(container);

            var rc = new FunctionalReflectorConfiguration(new[] { typeof(NavigableClass) }, new Type[0]);

            container.RegisterInstance<IFunctionalReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            var specs = reflector.AllObjectSpecImmutables;
            //Assert.AreEqual(2, specs.Length);
            //AbstractReflectorTest.AssertSpec(typeof(NavigableClass), specs);
            //AbstractReflectorTest.AssertSpec(typeof(SimpleClass), specs);
        }

    }
}