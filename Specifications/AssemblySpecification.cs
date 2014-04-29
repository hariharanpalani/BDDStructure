using System.Collections.Generic;

namespace CrossCutting.Specifications
{
    public class AssemblySpecification
    {
        public string FullyQualifiedName { get; set; }
        public List<TypeSpecification> TypeSpecifications { get; set; }
        public InterfaceType InterfaceType { get; set; }
        public TypeLifeStyle TypeLifeStyle { get; set; }
    }
}
