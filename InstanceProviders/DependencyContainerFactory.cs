using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtps.Tomtra.CrossCutting.Specifications;

namespace CrossCutting.InstanceProviders
{
    public static class DependencyContainerFactory
    {

        public static IInstanceProvider GetContainer(ContainerType containerType, ApplicationName application)
        {
            IInstanceProvider instanceProvider = null;

            var containerSpecification = ContainerSpecificationFactory.GetSpecification(application);
            switch (containerType)
            {
                case ContainerType.Windsor:
                    var windsorContainerResolver = new WindsorContainerResolver(containerSpecification);
                    instanceProvider = windsorContainerResolver.GetContainer();
                    break;
                case ContainerType.StructureMap:
                case ContainerType.NInject:
                    throw new NotImplementedException("No implementation for these provider types...");
            }

            return instanceProvider;
        }
    }
}
