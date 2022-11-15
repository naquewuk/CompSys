using System;
using System.Collections.Generic;
using System.Text;
using Queues;
using Structures;

namespace ComputingSystem
{
    class Model
    {
        public Model()
        {
            clock = new SystemClock();
            deviceQueue = new FIFOQueue<Process, SimpleArray<Process>>(new SimpleArray<Process>());
            readyQueue = new PriorityQueue<Process, BinaryHeap<Process>>(new BinaryHeap<Process>());
            modelSettings = new Settings();
            idGen = new IdGenerator();
            processRand = new Random();
            cpu = new Resource();
            device = new Resource();
            cpuScheduler = new CPUScheduler(cpu, readyQueue);
            deviceScheduler = new DeviceScheduler(device, deviceQueue);
            memoryManager = new MemoryManager();
            ram = new Memory();
        }

        public void SaveSettings()
        {
            ram.Save(modelSettings.ValueOfRAMSize);
            memoryManager.Save(ram);
        }
        public void WorkingCycle()
        {
            clock.WorkingCycle();
            if (processRand.NextDouble() <= modelSettings.Intensity)
            {
                Process proc = new Process(idGen.Id,
                    processRand.Next(modelSettings.MinValueOfAddrSpace, modelSettings.MaxValueOfAddrSpace + 1));
               if (memoryManager.Allocate(proc) != null)
               {
                    
                    proc.BurstTime = processRand.Next(modelSettings.MinValueOfBurstTime,
                        modelSettings.MaxValueOfBurstTime + 1);
                    subscribe(proc);
                    readyQueue = readyQueue.Put(proc);
                    if (cpu.IsFree())
                    {
                        cpuScheduler.Session();
                    }
                }
            }
            cpu.WorkingCycle();
            device.WorkingCycle();
        }

        void Clear()
        {
            cpu.Clear();
            device.Clear();
            ram.Clear();
            readyQueue.Clear();
            deviceQueue.Clear();
        }

        private void FreeingResourceEventHandler (object sender, EventArgs e)
        {
            Process proc = sender as Process;
            if (proc.Status == ProcessStatus.waiting) //Процесс покидает внешнее устройство
            {
                device.Clear();
                proc.Status = ProcessStatus.ready;
                proc.BurstTime = processRand.Next(modelSettings.MinValueOfBurstTime,
                         modelSettings.MaxValueOfBurstTime + 1);
                proc.ResetWorkTime();
                
                readyQueue = readyQueue.Put(proc);

                if(cpu.IsFree())
                {
                   readyQueue = cpuScheduler.Session();
                }

                if (deviceQueue.Count != 0) 
                {
                   deviceQueue = deviceScheduler.Session();
                }
            }
            else //Процесс покидает процессор
            {
                cpu.Clear();           
                if(readyQueue.Count != 0)
                {
                   readyQueue = cpuScheduler.Session();
                }
             
                proc.Status = processRand.Next(0, 2) == 0 ? ProcessStatus.terminated :
                        ProcessStatus.waiting;
                if(proc.Status == ProcessStatus.terminated)
                {
                    memoryManager.Free(proc);
                    unsubscribe(proc);
                }
                else
                {
                    proc.BurstTime = processRand.Next(modelSettings.MinValueOfBurstTime,
                        modelSettings.MaxValueOfBurstTime + 1);
                    proc.ResetWorkTime();
                    deviceQueue = deviceQueue.Put(proc);
                    if(device.IsFree())
                    {
                        deviceQueue = deviceScheduler.Session();
                    }
                }
            }
        }
        private void subscribe(Process proc)
        {
            if (proc != null)
            {
                proc.FreeingAResource += FreeingResourceEventHandler;
            }
        }

        private void unsubscribe(Process proc)
        {
            if (proc != null)
            {
                proc.FreeingAResource -= FreeingResourceEventHandler;
            }
        }


        private SystemClock clock;
        private Resource cpu;//
        private Resource device;//
        private IdGenerator idGen;
        private IQueueable<Process> deviceQueue;//
        private IQueueable<Process> readyQueue;//
        private CPUScheduler cpuScheduler;
        private DeviceScheduler deviceScheduler;
        private MemoryManager memoryManager;
        private Settings modelSettings;//
        private Random processRand;
        private Memory ram;

        public IQueueable<Process> ReadyQueue
        {
            get { return readyQueue; }
            set { readyQueue = value; }
        }

        public IQueueable<Process> DeviceQueue
        {
            get { return deviceQueue; }
            set { deviceQueue = value; }
        }

        public Settings ModelSettings
        {
            get { return modelSettings; }
        }

        public Resource Device
        {
            get { return device; }
        }

        public Resource Cpu
        {
            get { return cpu; }
        }
    }
}
