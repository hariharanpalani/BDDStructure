using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrossCutting.Specifications
{
    public class ContainerSpecification
    {
        public ContainerSpecification()
        {
            DefaultTypeLifeStyle = TypeLifeStyle.Transient;
        }

        public List<AssemblySpecification> AssemblySpecifications { get; set; }
        public TypeLifeStyle DefaultTypeLifeStyle { get; set; }
    }
}
