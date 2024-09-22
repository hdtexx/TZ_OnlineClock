using System;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services.TimeService
{
    public interface ITimeService
    {
        UniTask<DateTime> GetOnlineTimeAsync();
        UniTask PreloadTimeAsync();
        DateTime GetPreloadedTime();
        public bool IsTimePreloaded { get; }
    }
}