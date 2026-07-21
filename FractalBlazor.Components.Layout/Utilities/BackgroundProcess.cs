using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace  FractalBlazor.Components.Layout.Utilities
{
    internal static class BackgroundProcess
    {
        private static object _locker = new object();
        private static List<WeakReference<FbUpdatable>> _updatableComponents { get; } = new List<WeakReference<FbUpdatable>>();

        private static Timer timer;

        static BackgroundProcess()
        {
            StartTimer();
        }

        public static void AddUpdatable(FbUpdatable cpnt)
        {
            lock (_locker)
                _updatableComponents.Add(new WeakReference<FbUpdatable>(cpnt));
        }

        private static void StartTimer()
        {
            if (timer == null)
                timer = new Timer(new TimerCallback(_ =>
                {
                    // -------- Update permanents
                    lock (_locker)
                    {
                        List<WeakReference<FbUpdatable>> _toRemove = null;
                        foreach (var wr in _updatableComponents)
                        {
                            if (wr.TryGetTarget(out var target))
                            {
                                if ((DateTime.Now - target.LastUpdate) > target.Interval && !target.Updated)
                                {
                                    target.LastUpdate = DateTime.Now;
                                    target.Updated = !target.Periodic;
                                    target.UpdateState();
                                }
                            } else
                            {
                                if (_toRemove == null)
                                    _toRemove = new List<WeakReference<FbUpdatable>>();
                            }
                        }
                        _toRemove?.ForEach(w => _updatableComponents.Remove(w));
                    }
                }), null, 100, 100);
        }
    }
}
