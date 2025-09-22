using MultimediaServerCore;
namespace UserMultimediaCore
{
    public static class Initializer
    {
        public static void Initialize()
        {
            UserMultimediaMesh.Initialize();
            UserMultimediaEventListener.Initialize();
        }
    }
}