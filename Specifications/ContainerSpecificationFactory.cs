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
                //based on case, we need to include methods to register only necessary components.
            }

            return null;
        }
    }
}
