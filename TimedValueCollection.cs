using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader.IO;

namespace PathOfModifiers
{
    public class TimedValueInstanceCollection : TagSerializable
    {
        public class InstanceType
        {
            public interface IStack { }
            public interface IGetMinValue { }

            public class Bleed : InstanceType { }
            public class Poison : InstanceType, IStack { }
            public class Shock : InstanceType { }
            public class ShockedAir : InstanceType { }
            public class Ignite : InstanceType { }
            public class BurningAir : InstanceType { }
            public class Chill : InstanceType, IGetMinValue { }
            public class ChilledAir : InstanceType, IGetMinValue { }
            public class DodgeChance : InstanceType, IStack { }
            public class MoveSpeed : InstanceType, IStack { }
        }

        public static readonly Func<TagCompound, TimedValueInstanceCollection> DESERIALIZER = Load;
        /// <summary>
        /// Load the timedValueInstances dictionary discarding non-existant types
        /// </summary>
        public static TimedValueInstanceCollection Load(TagCompound tag)
        {
            var instanceTypeNames = tag.GetList<string>("instanceTypeNames");
            var instanceTypes = new List<Type>(instanceTypeNames.Count);
            var timedValueInstanceLists = tag.GetList<TimedValueInstanceList>("timedValueInstanceLists");
            var tvRemoveIndicies = new Stack<int>();
            for (int i = 0; i < instanceTypeNames.Count; i++)
            {
                var instanceTypeName = instanceTypeNames[i];
                Type instanceType = Type.GetType(instanceTypeName, false);
                if (instanceType == null || !instanceType.IsSubclassOf(typeof(InstanceType)))
                {
                    instanceTypes.Add(instanceType);
                }
                else
                {
                    tvRemoveIndicies.Push(i);
                    PathOfModifiers.Instance.Logger.Warn($"Timed Value Type \"{ instanceTypeName }\" not found.");
                }
            }
            while (tvRemoveIndicies.Count > 0)
            {
                timedValueInstanceLists.RemoveAt(tvRemoveIndicies.Pop());
            }

            return new TimedValueInstanceCollection()
            {
                instances = instanceTypes.Zip(timedValueInstanceLists, (k, v) => new { Key = k, Value = v }).ToDictionary(x => x.Key, x => x.Value),
            };
        }
        public TagCompound SerializeData()
        {
            TagCompound tag = new TagCompound();
            var instanceTypeNames = new List<string>(instances.Count);
            foreach (var instanceType in instances.Keys)
            {
                instanceTypeNames.Add(instanceType.AssemblyQualifiedName);
            }
            tag.Add("instanceTypeNames", instanceTypeNames);
            tag.Add("timedValueInstanceLists", instances.Values.ToList());
            return tag;
        }

        public class TimedValueInstanceList : TagSerializable
        {
            public static readonly Func<TagCompound, TimedValueInstanceList> DESERIALIZER = Load;
            public static TimedValueInstanceList Load(TagCompound tag)
            {
                var instanceList = new TimedValueInstanceList
                {
                    totalValue = tag.GetFloat("totalValue"),
                    instances = new LinkedList<TimedValueInstance>(tag.GetList<TimedValueInstance>("instances"))
                };
                return instanceList;
            }
            public TagCompound SerializeData()
            {
                TagCompound tag = new TagCompound
                {
                    { "totalValue", totalValue },
                    { "instances", instances.ToList() }
                };
                return tag;
            }

            public float totalValue;
            /// <summary>
            /// Sorted by endtime, ascending.
            /// </summary>
            public LinkedList<TimedValueInstance> instances = new LinkedList<TimedValueInstance>();
        }
        public class TimedValueInstance : TagSerializable
        {
            public static readonly Func<TagCompound, TimedValueInstance> DESERIALIZER = Load;
            public static TimedValueInstance Load(TagCompound tag)
            {
                uint endTime = Main.GameUpdateCount + (uint)tag.GetInt("remainingTime");
                float value = tag.GetFloat("value");
                return new TimedValueInstance(endTime, value);
            }
            public TagCompound SerializeData()
            {
                TagCompound tag = new TagCompound
                {
                    //convert to remaining time because gametime changes every launch
                    { "remainingTime", endTime - Main.GameUpdateCount },
                    { "value", value }
                };
                return tag;
            }

            /// <summary>
            /// gametime in ms
            /// </summary>
            public readonly uint endTime;
            public readonly float value;

            public TimedValueInstance(uint endTime, float value)
            {
                this.endTime = endTime;
                this.value = value;
            }
        }


        public Dictionary<Type, TimedValueInstanceList> instances = new Dictionary<Type, TimedValueInstanceList>();


        public void AddInstance(Type type, float value, int durationTicks)
        {
            TimedValueInstanceList timedValueInstanceList;

            if (!instances.TryGetValue(type, out timedValueInstanceList))
            {
                timedValueInstanceList = new TimedValueInstanceList();
                instances.Add(type, timedValueInstanceList);
            }
            TimedValueInstance instance = new TimedValueInstance(Main.GameUpdateCount + (uint)durationTicks, value);

            if (typeof(InstanceType.IStack).IsAssignableFrom(type))
            {
                timedValueInstanceList.totalValue += value;
            }
            else if (typeof(InstanceType.IGetMinValue).IsAssignableFrom(type))
            {
                if (value < timedValueInstanceList.totalValue)
                {
                    timedValueInstanceList.totalValue = value;
                }
            }
            else
            {
                if (value > timedValueInstanceList.totalValue)
                {
                    timedValueInstanceList.totalValue = value;
                }
            }

            //Add instance into list sorted
            LinkedListNode<TimedValueInstance> lastNode = timedValueInstanceList.instances.Last;
            while (lastNode != null && instance.endTime < lastNode.Value.endTime)
            {
                lastNode = lastNode.Previous;
            }

            if (lastNode == null)
            {
                timedValueInstanceList.instances.AddFirst(instance);
            }
            else if (instance.endTime >= lastNode.Value.endTime)
            {
                timedValueInstanceList.instances.AddAfter(lastNode, instance);
            }
        }
        public void ResetEffects()
        {
            uint now = Main.GameUpdateCount;

            foreach (var kv in instances)
            {
                var type = kv.Key;
                var timedValueInstanceList = kv.Value;

                bool isStacking = typeof(InstanceType.IStack).IsAssignableFrom(type);
                bool needRecount = false;

                var firstNode = timedValueInstanceList.instances.First;
                while (firstNode != null && now >= firstNode.Value.endTime)
                {
                    if (isStacking)
                    {
                        timedValueInstanceList.totalValue -= firstNode.Value.value;
                    }
                    else if (timedValueInstanceList.totalValue == firstNode.Value.value)
                    {
                        needRecount = true;
                    }

                    timedValueInstanceList.instances.RemoveFirst();
                    firstNode = timedValueInstanceList.instances.First;
                }

                if (needRecount)
                {
                    if (firstNode == null)
                    {
                        timedValueInstanceList.totalValue = 0;
                    }
                    else
                    {
                        timedValueInstanceList.totalValue =
                            typeof(InstanceType.IGetMinValue).IsAssignableFrom(type)
                            ? timedValueInstanceList.instances.Min(x => x.value)
                            : timedValueInstanceList.instances.Max(x => x.value);
                    }
                }
            }
        }
        public void RemoveInstances(Type type)
        {
            instances.Remove(type);
        }
        public void Clear()
        {
            instances.Clear();
        }
    }
}