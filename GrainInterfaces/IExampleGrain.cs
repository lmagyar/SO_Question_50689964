using System;
using System.Threading.Tasks;

using Orleans;

namespace GrainInterfaces
{
    public interface IExampleGrain : IGrainWithGuidKey
    {
        Task<string> Ping(string arg);
    }
}
