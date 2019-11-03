namespace ApplicationCore.TypeMapping
{
    public interface IAutoMapper
    {
        T Map<T>(object objectToMap);
        S Map<T, S>(T source, S destination);
    }
}
