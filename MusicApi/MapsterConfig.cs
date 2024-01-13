using Mapster;

namespace MusicApi
{
    public class MapsterConfigL : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // for nested mapping
            TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
            // testing custom config rules
            //config
            //    .NewConfig<Genre, GenreDto>()
            //    .Map(dest => dest.Description, src => $"hehe{src.Name}", src => src.Name.StartsWith('a'));
        }
    }
}
