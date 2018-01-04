using System;
using System.Threading.Tasks;

namespace FSQL.Interfaces {
    public interface ICacheControl {
        void Refresh();
        Task RefreshAsync();
        //DateTime LastRefreshed { get; }
        //DateTime LastTouched { get; }
        //TimeSpan TimeToLive { get; set; }
        //TimeSpan MaxTimeToLive { get; set; }
        //TimeSpan TimeSinceLastTouch { get; }
        //TimeSpan TimeSinceLastRefresh { get; }
        //bool IsStale { get; }
        //void Touch();
        //void Expire();
    }

    public interface ICached {
        bool OnRead();
        Task<bool> OnReadAsync();
    }
}