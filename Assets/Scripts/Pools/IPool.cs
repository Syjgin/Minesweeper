using System;

namespace Pools
{
    public interface IPool
    {
        Type ComponentType { get; }
    }
}