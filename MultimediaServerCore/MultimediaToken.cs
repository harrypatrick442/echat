namespace MultimediaServerCore
{
    public class MultimediaToken
    {
        public int Version { get; protected set; }
        public int NodeId { get; protected set; }
        public MultimediaType MultimediaType { get; protected set; }
        public long MillisecondsUTC { get; protected set; }
        public string Token { get; protected set; }
        public string Extension { get; protected set; }
        public string Raw { get; protected set; }
        private MultimediaToken(
            int version, int nodeId, MultimediaType multimediaType,
            long millisecondsUTC, string token, string extension)
        {
            Version = version;
            NodeId = nodeId;
            MultimediaType = multimediaType;
            MillisecondsUTC = millisecondsUTC;
            Token = token;
            Extension = extension;
        }
        public static MultimediaToken FromString(string multimediaToken) {

            string[] splits = multimediaToken.Split('_');
            int version = int.Parse(splits[0]);
            int nodeId;
            MultimediaType multimediaType;
            long millisecondsUTC;
            string token;
            string extension;
            if (version == 1)
            {
                nodeId = int.Parse(splits[1]);
                multimediaType = (MultimediaType)int.Parse(splits[2]);
                millisecondsUTC = long.Parse(splits[3]) * 1000;
                token = splits[4];
                extension = splits[5];
            }
            else
            {
                throw new NotImplementedException($"Not implemented for version { version}");
            }
            return new MultimediaToken(version, nodeId, multimediaType,
            millisecondsUTC, token, extension);
        }
    }
}