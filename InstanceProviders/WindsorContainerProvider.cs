using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace CrossCutting.InstanceProviders
{
    public class WindsorContainerProvider : IInstanceProvider
    {
        private readonly IWindsorContainer _windsorContainer;

        #region C O N S T R U C T O R (S)

        public WindsorContainerProvider(IWindsorContainer windsorContainer)
        {
            _windsorContainer = windsorContainer;
            _windsorContainer.Register(Component.For<IWindsorContainer>().Instance(_windsorContainer))
                             .Register(Component.For<IInstanceProvider>().Instance(this));
        }

        #endregion

        #region I I N S T A N C E P R O V I D E R   M E T H O D(S)

        public T Resolve<T>()
        {
            return _windsorContainer.Resolve<T>();
        }

        public T Resolve<T>(System.Collections.IDictionary arguments)
        {
            return _windsorContainer.Resolve<T>(arguments);
        }

        public T Resolve<T>(string key)
        {
            return _windsorContainer.Resolve<T>(key);
        }

        public object Resolve(Type service)
        {
            return _windsorContainer.Resolve(service);
        }

        public T Resolve<T>(string key, System.Collections.IDictionary arguments)
        {
            return _windsorContainer.Resolve<T>(key, arguments);
        }

        public object Resolve(string key, Type service)
        {
            return _windsorContainer.Resolve(key, service);
        }

        public object Resolve(Type service, System.Collections.IDictionary arguments)
        {
            return _windsorContainer.Resolve(service, arguments);
        }

        public object Resolve(string key, Type service, System.Collections.IDictionary arguments)
        {
            return _windsorContainer.Resolve(key, service, arguments);
        }

        public Array ResolveAll(Type service)
        {
            return _windsorContainer.ResolveAll(service);
        }

        public void RegisterType(Type type, object instance)
        {
            _windsorContainer.Register(Component.For(type).Instance(instance));
        }

        #endregion
    }
}
