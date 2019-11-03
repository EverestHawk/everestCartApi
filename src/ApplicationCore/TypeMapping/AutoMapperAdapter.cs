using AutoMapper;

namespace ApplicationCore.TypeMapping
{
    public class AutoMapperAdapter : IAutoMapper
    {
        private readonly IMapper _mapper;
        public AutoMapperAdapter(IMapper mapper)
        {
            _mapper = mapper;
        }
        public T Map<T>(object objectToMap)
        {
            return _mapper.Map<T>(objectToMap);
        }

        public S Map<T,S>(T source, S destination)
        {
            return _mapper.Map(source, destination);
        }
    }
}
