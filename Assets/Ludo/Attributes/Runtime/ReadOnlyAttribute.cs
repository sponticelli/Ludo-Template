using System;
using UnityEngine;

namespace Ludo.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
}