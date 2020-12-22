using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.WaitStrategies;

namespace GraphQlComparer
{
    public class WaitDelay : IWaitUntil
    {
        readonly TimeSpan time;

        public WaitDelay(TimeSpan time)
        {
            this.time = time;
        }

        public async Task<Boolean> Until(Uri endpoint, String id)
        {
            await Task.Delay(time);
            return true;
        }

        public static IWaitUntil For(TimeSpan time) =>
            new WaitDelay(time);
    }
}