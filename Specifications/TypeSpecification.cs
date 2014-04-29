using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrossCutting.Specifications
{
    public class TypeSpecification
    {
        public string FullyQualifiedName { get; set; }
        public Type ServiceInterfaceType { get; set; }
        public InterfaceType InterfaceType { get; set; }
        public TypeLifeStyle TypeLifeStyle { get; set; }
    }
}
