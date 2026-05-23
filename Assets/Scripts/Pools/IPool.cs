using Config;
using UnityEngine;

namespace Pools
{
    public interface IPool
    {
        PrefabType PrefabType { get; }
    }
}