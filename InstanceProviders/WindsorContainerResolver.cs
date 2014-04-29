using System;
using System.Collections.Generic;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Gtps.Tomtra.CrossCutting.Specifications;

namespace CrossCutting.InstanceProviders
{
    public class WindsorContainerResolver : ISpecificDependencyResolver
    {
        private readonly IInstanceProvider _instanceProvider;
        private readonly IWindsorContainer _windsorContainer;
        private readonly ContainerSpecification _containerSpecification;

        #region C O N S T R U C T O R(S)

        public WindsorContainerResolver(ContainerSpecification containerSpecification)
        {
            _containerSpecification = containerSpecification;
            _windsorContainer = new WindsorContainer();
            _instanceProvider = new WindsorContainerProvider(_windsorContainer);
        }

        #endregion

        #region I S P E C I F I C D E P E N D E N C Y R E S O L V E R  M E M B E R(S)

        public IInstanceProvider GetContainer()
        {
            LifestyleType defaultLifeStyle;

            if (_containerSpecification == null)
            {
                throw new Exception("IOC Container Specification is mandatory");
            }

            var parsed = Enum.TryParse(_containerSpecification.DefaultTypeLifeStyle.ToString(), out defaultLifeStyle);

            if (!parsed)
            {
                throw new Exception("Type Life Style is invalid");
            }



            //Speficication list should not be null
            foreach (var assemblySpecification in _containerSpecification.AssemblySpecifications)
            {
                LifestyleType assemblyLifeStyle;
                var explicitTypesRegistered = new List<string>();
                Enum.TryParse(assemblySpecification.TypeLifeStyle.ToString(), out assemblyLifeStyle);
                assemblyLifeStyle = (assemblyLifeStyle == LifestyleType.Undefined) ? defaultLifeStyle : assemblyLifeStyle;

                if (assemblySpecification.TypeSpecifications != null)
                {
                    foreach (var typeSpecification in assemblySpecification.TypeSpecifications)
                    {
                        var localTypeSpec = typeSpecification;
                        LifestyleType lifeStyle;
                        Enum.TryParse<LifestyleType>(typeSpecification.TypeLifeStyle.ToString(), out lifeStyle);
                        lifeStyle = (lifeStyle == LifestyleType.Undefined) ? assemblyLifeStyle : lifeStyle;

                        if (localTypeSpec.ServiceInterfaceType == null)
                        {
                            switch (localTypeSpec.InterfaceType)
                            {
                                case InterfaceType.FirstInterface:
                                    _windsorContainer.Register(Classes
                                                                        .FromAssemblyNamed(assemblySpecification.FullyQualifiedName)
                                                                        .Where(type => type.FullName == localTypeSpec.FullyQualifiedName)
                                                                        .WithService
                                                                        .FirstInterface()
                                                                        .Configure(component => component.LifeStyle.Is(lifeStyle)));
                                    break;
                                case InterfaceType.All:
                                    _windsorContainer.Register(Classes
                                                                        .FromAssemblyNamed(assemblySpecification.FullyQualifiedName)
                                                                        .Where(type => type.FullName == localTypeSpec.FullyQualifiedName)
                                                                        .WithService
                                                                        .AllInterfaces()
                                                                        .Configure(component => component.LifeStyle.Is(lifeStyle)));
                                    break;
                            }
                        }
                        else
                        {
                            _windsorContainer.Register(Classes
                                                                        .FromAssemblyNamed(assemblySpecification.FullyQualifiedName)
                                                                        .Where(type => type.FullName == localTypeSpec.FullyQualifiedName)
                                                                        .WithService
                                                                        .FromInterface(localTypeSpec.ServiceInterfaceType)
                                                                        .Configure(component => component.LifeStyle.Is(lifeStyle)));
                        }
                        explicitTypesRegistered.Add(localTypeSpec.FullyQualifiedName);
                    }
                }

                switch (assemblySpecification.InterfaceType)
                {
                    case InterfaceType.FirstInterface:
                        _windsorContainer.Register(Classes.FromAssemblyNamed(assemblySpecification.FullyQualifiedName)
                                                  .Where(y => !explicitTypesRegistered.Contains(y.FullName))
                                                  .WithService
                                                  .FirstInterface()
                                                  .Configure(component => component.LifeStyle.Is(assemblyLifeStyle)));
                        break;
                    case InterfaceType.All:
                        _windsorContainer.Register(Classes.FromAssemblyNamed(assemblySpecification.FullyQualifiedName)
                         .Where(y => !explicitTypesRegistered.Contains(y.FullName))
                         .WithService
                         .AllInterfaces()
                         .Configure(component => component.LifeStyle.Is(assemblyLifeStyle)));
                        break;
                    default:
                        _windsorContainer.Register(Classes.FromAssemblyNamed(assemblySpecification.FullyQualifiedName)
                            .Where(y => !explicitTypesRegistered.Contains(y.FullName))
                            .Configure(component => component.LifeStyle.Is(assemblyLifeStyle)));
                        break;
                }
            }

            return _instanceProvider;
        }

        #endregion
    }
}
