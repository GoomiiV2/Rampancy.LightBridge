using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rampancy
{
    public class WorkQueue<T>
    {
        public readonly int MaxWorkers;
        private BlockingCollection<T> Queue;
        private Action<T> Action;
        private bool ShouldRun;
        private Task DequeueTask;
        private SemaphoreSlim TasksSem;

        public int GetPendingItemsCount() => Queue.Count;
        public bool IsIdle()              => Queue.Count == 0 && TasksSem.CurrentCount == MaxWorkers;

        public WorkQueue(Action<T> action, int maxWorkers = 4)
        {
            Action     = action;
            MaxWorkers = maxWorkers;
            ShouldRun  = true;

            Queue       = new BlockingCollection<T>();
            DequeueTask = Task.Factory.StartNew(DequeueLoop);
            TasksSem    = new SemaphoreSlim(maxWorkers);
        }

        public void AddItem(T item)
        {
            Queue.Add(item);
        }

        public void Stop()
        {
            ShouldRun = false;
            DequeueTask.Wait();
        }

        private void DequeueLoop()
        {
            while (ShouldRun)
            {
                if (Queue.Count == 0)
                {
                    Thread.Sleep(100);
                }
                else
                {
                    TasksSem.Wait();
                    var item = Queue.Take();
                    var task = Task.Factory.StartNew(() =>
                    {
                        Action(item);
                        TasksSem.Release();
                    });
                }
            }
        }
    }
}
