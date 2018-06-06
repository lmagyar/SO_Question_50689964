using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Orleans;

using GrainInterfaces;

namespace GrainImplementations
{
    public class ExampleGrain : Grain, IExampleGrain
    {
        Task<string> IExampleGrain.Ping(string arg)
        {
            return Task.FromResult($"received: {arg}");
        }
    }
}
