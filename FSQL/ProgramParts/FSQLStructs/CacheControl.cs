using System;
using System.Threading;
using System.Threading.Tasks;
using FSQL.Interfaces;

namespace FSQL.ProgramParts.FSQLStructs {
    public class CacheControl : ICacheControl {
        private class CachedObjectWrapper : ICached {
            private readonly Func<bool> _onRead;
            public CachedObjectWrapper(Func<bool> onRead) {
                _onRead = onRead;
            }

            public bool OnRead() {
                return _onRead();
            }

            public async Task<bool> OnReadAsync() {                
                var results = await Task.Run(_onRead);
                return results;
            }
        }
        private ICached CachedObject { get; }
        public CacheControl(Func<bool> onRead, TimeSpan ttl = default(TimeSpan), TimeSpan maxttl = default(TimeSpan)) :
            this (new CachedObjectWrapper(onRead), ttl, maxttl){}

        public CacheControl(ICached cachedObj, TimeSpan ttl = default(TimeSpan), TimeSpan maxttl = default(TimeSpan)) {
            CachedObject = cachedObj;
            if (ttl == default(TimeSpan)) ttl = TimeSpan.FromMinutes(1);
            if (maxttl == default(TimeSpan)) maxttl = TimeSpan.FromMinutes(5);
            TimeToLive = ttl;
            MaxTimeToLive = maxttl;
        }

        public DateTime LastRefreshed { get; protected set; } = DateTime.MinValue;
        public DateTime LastTouched { get; protected set; } = DateTime.MinValue;
        public TimeSpan TimeToLive { get; set; }
        public TimeSpan MaxTimeToLive { get; set; }
        public TimeSpan TimeSinceLastTouch => DateTime.Now - LastTouched;
        public TimeSpan TimeSinceLastRefresh => DateTime.Now - LastRefreshed;
        public bool IsStale => (TimeSinceLastRefresh > MaxTimeToLive) ||
                               (TimeSinceLastTouch > TimeToLive);

        protected bool _IsRefreshing = false;
        public virtual void Refresh()
        {
            if (!IsStale || _IsRefreshing) return;
            try {
                _IsRefreshing = true;
                if (CachedObject.OnRead()) {
                    LastRefreshed = DateTime.Now;
                    Touch();
                }
            } finally {
                _IsRefreshing = false;
            }
        }

        public async Task RefreshAsync() {
            if (!IsStale || _IsRefreshing) return;
            try
            {
                _IsRefreshing = true;
                if (await CachedObject.OnReadAsync())
                {
                    LastRefreshed = DateTime.Now;
                    Touch();
                }
            }
            finally
            {
                _IsRefreshing = false;
            }
        }

        public void Touch() => LastTouched = DateTime.Now;
        public void Expire() => LastRefreshed = DateTime.MinValue;
    }
}