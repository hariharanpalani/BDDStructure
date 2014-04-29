namespace CrossCutting.InstanceProviders
{
    public interface ISpecificDependencyResolver
    {
        IInstanceProvider GetContainer();
    }
}
