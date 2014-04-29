using System;
using System.Collections;

namespace CrossCutting.InstanceProviders
{
    public interface IInstanceProvider
    {
        T Resolve<T>();
        T Resolve<T>(IDictionary arguments);
        T Resolve<T>(string key);
        object Resolve(Type service);
        T Resolve<T>(string key, IDictionary arguments);
        object Resolve(string key, Type service);
        object Resolve(Type service, IDictionary arguments);
        object Resolve(string key, Type service, IDictionary arguments);
        Array ResolveAll(Type service);
        void RegisterType(Type type, object instance);
    }
}
