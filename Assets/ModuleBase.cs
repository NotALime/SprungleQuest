using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace Modules
{
    public abstract class ModuleBase
    {
    //    public string name;
    }
    [System.Serializable]
    [CreateAssetMenu(menuName = "Modules/Value Module")]
    public class ValueModule : ModuleBase
    {
        public float value;
    }

  // [System.Serializable]
  // public class ModuleHolder
  // {
  //     public List<ModuleBase> modules;
  //     public ModuleBase GetModule(string moduleName)
  //     {
  //         ModuleBase m = modules.Find(v => v.name == moduleName);
  //         return m;
  //     }
  // }
}

