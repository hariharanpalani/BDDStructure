using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrossCutting.Specifications
{
    public class ContainerSpecificationFactory
    {
        public static ContainerSpecification GetSpecification(ApplicationName application)
        {
            switch (application)
            {
                case ApplicationName.TomTra:
                    return GetTomTraContainerSpecification();
                case ApplicationName.Sds:
                    return GetSdsSpecification();
                    case ApplicationName.WinService:
                    return GetBatchSpecification();
            }

            return null;
        }

        private static ContainerSpecification GetSdsSpecification()
        {
            var containerSpecification = new ContainerSpecification
                                             {
                                                 DefaultTypeLifeStyle = TypeLifeStyle.Transient,
                                                 AssemblySpecifications = new List<AssemblySpecification>()
                                             };

            containerSpecification.AssemblySpecifications.Add(new AssemblySpecification
            {
                FullyQualifiedName = Constants.CrosscuttingCore,
                InterfaceType = InterfaceType.All
            });
            containerSpecification.AssemblySpecifications.Add(new AssemblySpecification
            {
                FullyQualifiedName = Constants.IoSds,
                InterfaceType = InterfaceType.All
            });
            containerSpecification.AssemblySpecifications.Add(new AssemblySpecification
            {
                FullyQualifiedName = Constants.InfrastructureCommon,
                InterfaceType = InterfaceType.All
            });
            containerSpecification.AssemblySpecifications.Add(new AssemblySpecification
            {
                FullyQualifiedName = Constants.InfrastructureSds,
                InterfaceType = InterfaceType.All
            });

            return containerSpecification;
        }

        private static ContainerSpecification GetTomTraContainerSpecification()
        {
            var containerSpecification = new ContainerSpecification
                                             {
                                                 DefaultTypeLifeStyle = TypeLifeStyle.Transient,
                                                 AssemblySpecifications = new List<AssemblySpecification>()
                                             };


            containerSpecification.AssemblySpecifications.Add(new AssemblySpecification
            {
                FullyQualifiedName = Constants.TomtraWeb,
                InterfaceType = InterfaceType.None
            });
            containerSpecification.AssemblySpecifications.Add(new AssemblySpecification
            {
                FullyQualifiedName = Constants.BusinessServices,
                InterfaceType = InterfaceType.All
            });
            containerSpecification.AssemblySpecifications.Add(new AssemblySpecification
            {
                FullyQualifiedName = Constants.ApplicationServices,
                InterfaceType = InterfaceType.All
            });

            containerSpecification.AssemblySpecifications.Add(new AssemblySpecification
            {
                FullyQualifiedName = Constants.CrosscuttingCore,
                InterfaceType = InterfaceType.All
            });

            containerSpecification.AssemblySpecifications.Add(new AssemblySpecification
            {
                FullyQualifiedName = Constants.Infrastructure,
                InterfaceType = InterfaceType.All
            });

            containerSpecification.AssemblySpecifications.Add(new AssemblySpecification
            {
                FullyQualifiedName = Constants.InfrastructureCommon,
                InterfaceType = InterfaceType.All
            });

            containerSpecification.AssemblySpecifications.Add(new AssemblySpecification
            {
                FullyQualifiedName = Constants.InfrastructureSecurity,
                InterfaceType = InterfaceType.All
            });

            return containerSpecification;

        }

        private static ContainerSpecification GetBatchSpecification()
        {
            var containerSpecification = new ContainerSpecification
                                             {
                                                 DefaultTypeLifeStyle = TypeLifeStyle.Transient,
                                                 AssemblySpecifications = new List<AssemblySpecification>()
                                             };

            containerSpecification.AssemblySpecifications.Add(new AssemblySpecification
                                                                  {
                                                                      FullyQualifiedName = Constants.BusinessServices,
                                                                      InterfaceType = InterfaceType.All
                                                                  });

            containerSpecification.AssemblySpecifications.Add(new AssemblySpecification
                                                                  {
                                                                      FullyQualifiedName = Constants.CrosscuttingCore,
                                                                      InterfaceType = InterfaceType.All
                                                                  });

            containerSpecification.AssemblySpecifications.Add(new AssemblySpecification
                                                                  {
                                                                      FullyQualifiedName = Constants.Infrastructure,
                                                                      InterfaceType = InterfaceType.All
                                                                  });

            containerSpecification.AssemblySpecifications.Add(new AssemblySpecification
                                                                  {
                                                                      FullyQualifiedName =
                                                                          Constants.InfrastructureCommon,
                                                                      InterfaceType = InterfaceType.All
                                                                  });
            return containerSpecification;
        }

    }
}
