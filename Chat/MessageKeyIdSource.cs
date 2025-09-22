namespace Chat
{
    public static class MessageKeyIdSource
    {
        //Very simple. Because the timestamp is primarily used when deleting a message, this can be cyclicle with no risk.
        //Although one might think that in theory a restart or something could cause a duplication of the id with the timestamp
        //This is not possible because of the significant time delay between stop and restart. How the hell can that be within milliseconds. It cant.
        //So just cycle round and round
        private static long _Value;
        public static long Next() {
            return _Value++;
        }
    }
}