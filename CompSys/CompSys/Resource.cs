using System;
using System.Collections.Generic;
using System.Text;

namespace ComputingSystem
{
    class Resource
    {
        public void WorkingCycle()
        {
            if (!IsFree())
            {
                activeProcess.IncreaseWorkTime();
            }
        }

        public bool IsFree()
        {
            return activeProcess == null;
        }

        public void Clear()
        {
            activeProcess = null;
        }

        public Process ActiveProcess
        {
            get
            {
                return activeProcess;
            }
            set
            {
                activeProcess = value;
            }
        }
        private Process activeProcess;
    }
}
