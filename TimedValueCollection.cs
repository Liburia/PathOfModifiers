using Microsoft.Xna.Framework;
using PathOfModifiers.Affixes.Items;
using PathOfModifiers.Buffs;
using PathOfModifiers.ModNet.PacketHandlers;
using PathOfModifiers.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Net;

namespace PathOfModifiers
{
    public class TimedValueInstanceCollection : TagSerializable
    {
        public class InstanceType
        {
            public class StackingType { }

            public class Bleed : InstanceType { }
            public class Poison : StackingType { }
            public class DodgeChance : StackingType { }
        }

        public static readonly Func<TagCompound, TimedValueInstanceCollection> DESERIALIZER = Load;
        public static TimedValueInstanceCollection Load(TagCompound tag)
        {
            //Load the timedValueInstances dictionary discarding non-existant types
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
            /// Sorted by endtime ascending.
            /// </summary>
            public LinkedList<TimedValueInstance> instances = new LinkedList<TimedValueInstance>();
        }
        public class TimedValueInstance : TagSerializable
        {
            public static readonly Func<TagCompound, TimedValueInstance> DESERIALIZER = Load;
            public static TimedValueInstance Load(TagCompound tag)
            {
                double endTime = PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds + tag.GetDouble("remainingTime");
                float value = tag.GetFloat("value");
                return new TimedValueInstance(endTime, value);
            }
            public TagCompound SerializeData()
            {
                TagCompound tag = new TagCompound
                {
                    //convert to remaining time because gametime changes every launch
                    { "remainingTime", endTime - PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds },
                    { "value", value }
                };
                return tag;
            }

            /// <summary>
            /// gametime in ms
            /// </summary>
            public readonly double endTime;
            public readonly float value;

            public TimedValueInstance(double endTime, float value)
            {
                this.endTime = endTime;
                this.value = value;
            }
        }


        public Dictionary<Type, TimedValueInstanceList> instances = new Dictionary<Type, TimedValueInstanceList>();


        public void AddInstance(Type type, float value, double durationMs)
        {
            TimedValueInstanceList timedValueInstanceList;

            if (!instances.TryGetValue(type, out timedValueInstanceList))
            {
                timedValueInstanceList = new TimedValueInstanceList();
                instances.Add(type, timedValueInstanceList);
            }
            TimedValueInstance instance = new TimedValueInstance(PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds + durationMs, value);

            if (type.IsSubclassOf(typeof(InstanceType.StackingType)))
            {
                timedValueInstanceList.totalValue += value;
            }
            else if (value > timedValueInstanceList.totalValue)
            {
                timedValueInstanceList.totalValue = value;
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
            else if (instance.endTime > lastNode.Value.endTime)
            {
                timedValueInstanceList.instances.AddAfter(lastNode, instance);
            }
        }
        public void ResetEffects()
        {
            foreach (var kv in instances)
            {
                var type = kv.Key;
                var timedValueInstanceList = kv.Value;

                bool isStacking = type.IsSubclassOf(typeof(InstanceType.StackingType));
                bool needRecount = false;

                double now = PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds;
                var firstNode = timedValueInstanceList.instances.First;
                while (firstNode != null && now > firstNode.Value.endTime)
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
                        timedValueInstanceList.totalValue = timedValueInstanceList.instances.Max(x => x.value);
                    }
                }
            }
        }
        public void Clear()
        {
            instances.Clear();
        }
    }
}