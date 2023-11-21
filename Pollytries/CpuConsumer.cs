namespace Pollytries;

using System.Diagnostics;
using System.Threading;

public class CpuConsumer
{
    public static void ConsumeCPUByDuration(int percentage, int durationInMs)
    {
        Stopwatch mainWatch = new Stopwatch();
        mainWatch.Start();

        Stopwatch watch = new Stopwatch();
        watch.Start();            
        while (mainWatch.ElapsedMilliseconds < durationInMs & durationInMs >= 100)
        {
            // Make the loop go on for "percentage" milliseconds then sleep the 
            // remaining percentage milliseconds. So 40% utilization means work 40ms and sleep 60ms
            if (watch.ElapsedMilliseconds > percentage)
            {
                Thread.Sleep(100-percentage);
                watch.Reset();
                watch.Start();
            }
        }
    }
    
    public static void ConsumeCPUByWorkAmount(long workAmount)
    {
        long wa = 0;
        while (wa < workAmount)
        {
            wa++;
        }
    }
}

