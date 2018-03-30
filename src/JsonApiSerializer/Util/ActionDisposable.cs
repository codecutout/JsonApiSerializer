using System;

namespace JsonApiSerializer.Util
{
    internal class ActionDisposable : IDisposable
    {
        private readonly Action _dispose;
        public ActionDisposable(Action dispose)
        {
            _dispose = dispose;
        }
        public void Dispose()
        {
            _dispose();
        }
    }
}
