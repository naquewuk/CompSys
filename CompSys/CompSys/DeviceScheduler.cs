using System;
using System.Collections.Generic;
using System.Text;
using Queues;

namespace ComputingSystem
{
    class DeviceScheduler
    {
        public DeviceScheduler(Resource resource, IQueueable<Process> queue)
        {
            this.resourсe = resource;
            this.queue = queue;
        }

        public IQueueable<Process> Session()
        {
            Process newActiveProcess = queue.Item();
            queue.Remove();
            resourсe.ActiveProcess = newActiveProcess;
            return queue;
        }

        private Resource resourсe;
        private IQueueable<Process> queue;
    }
}
