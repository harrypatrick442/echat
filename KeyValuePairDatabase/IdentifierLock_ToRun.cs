namespace KeyValuePairDatabases
{
    internal class IdentifierLock_ToRun
    {
        private Action<Action> _Run;
        private Action<Exception> _Failed;
        public IdentifierLock_ToRun(Action<Action> run, Action<Exception> failed)
        {
            _Run = run;
            _Failed = failed;
        }
        public void Run(Action done)
        {
            _Run(done);
        }
        public void Failed(Exception ex)
        {
            _Failed(ex);
        }
    }
}