using Core.LoadBalancing;
using Statistics;

namespace MultimediaServerCore
{
    public class Initializer
    {
        public static void InitializeServer(MultimediaServerSetup setup)
        {
            Initializer.Initialize(true, setup);
        }
        public static void InitializeClient()
        {
            Initializer.Initialize(false, null);
        }
        public static void Initialize(bool isMultimediaServer, 
            MultimediaServerSetup setup) {
            MultimediaServerStatisticsFileLogger.Initialize();
            MultimediaServerLoadBalancer.Initialize();
            MultimediaServerMesh.Initialize();
            if (isMultimediaServer)
            {
                MultimediaUploadValidation.Initialize(setup);
                PendingMultimediaUploads.Initialize();
                MultimediaCache.Initialize();
                DalMultimediaDeletes.Initialize();
                MultimediaDeletesProcessor.Initialize();
                VideoProcessing.Processor.Initialize();
            }
        }
    }
}