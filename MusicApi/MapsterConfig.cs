using Mapster;

namespace MusicApi
{
    public class MapsterConfigL : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // testing custom config rules
            //config
            //    .NewConfig<Genre, GenreDto>()
            //    .Map(dest => dest.Description, src => $"hehe{src.Name}", src => src.Name.StartsWith('a'));
        }
    }
}
