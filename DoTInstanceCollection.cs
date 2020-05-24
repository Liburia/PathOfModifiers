using Microsoft.Xna.Framework;
using PathOfModifiers.Affixes.Items;
using PathOfModifiers.Buffs;
using PathOfModifiers.ModNet.PacketHandlers;
using PathOfModifiers.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Net;

namespace PathOfModifiers
{
    public class DoTInstanceCollection : TagSerializable
    {
        public static readonly Func<TagCompound, DoTInstanceCollection> DESERIALIZER = Load;
        public static DoTInstanceCollection Load(TagCompound tag)
        {
            //Load the dotInstances dictionary discarding non-existant buff types and buffs that don't inherit from DamageOverTime
            var dotTypeNames = tag.GetList<string>("dotTypeNames");
            var dotTypes = new List<Type>(dotTypeNames.Count);
            var dotInstanceLists = tag.GetList<DoTInstanceList>("dotInstanceLists");
            var dvRemoveIndicies = new Stack<int>();
            for (int i = 0; i < dotTypeNames.Count; i++)
            {
                var dotTypeName = dotTypeNames[i];
                Type dotType = Type.GetType(dotTypeName, false);
                if (dotType == null || !dotType.IsSubclassOf(typeof(DamageOverTime)))
                {
                    dvRemoveIndicies.Push(i);
                    PathOfModifiers.Instance.Logger.Warn("Buff \"{dotTypeName}\" not found.");
                }
                else
                {
                    dotTypes.Add(dotType);
                }
            }
            while (dvRemoveIndicies.Count > 0)
            {
                dotInstanceLists.RemoveAt(dvRemoveIndicies.Pop());
            }

            return new DoTInstanceCollection()
            {
                dotInstances = dotTypes.Zip(dotInstanceLists, (k, v) => new { Key = k, Value = v }).ToDictionary(x => x.Key, x => x.Value),
            };
        }
        public TagCompound SerializeData()
        {
            TagCompound tag = new TagCompound();
            var dotTypeNames = new List<string>(dotInstances.Count);
            foreach (var dotType in dotInstances.Keys)
            {
                dotTypeNames.Add(dotType.AssemblyQualifiedName);
            }
            tag.Add("dotTypeNames", dotTypeNames);
            tag.Add("dotInstanceLists", dotInstances.Values.ToList());
            return tag;
        }

        public class DoTInstanceList : TagSerializable
        {
            public static readonly Func<TagCompound, DoTInstanceList> DESERIALIZER = Load;
            public static DoTInstanceList Load(TagCompound tag)
            {
                var dil = new DoTInstanceList
                {
                    dps = tag.GetInt("dps"),
                    instances = new LinkedList<DoTInstance>(tag.GetList<DoTInstance>("instances"))
                };
                return dil;
            }
            public TagCompound SerializeData()
            {
                TagCompound tag = new TagCompound
                {
                    { "dps", dps },
                    { "instances", instances.ToList() }
                };
                return tag;
            }

            public int dps = 0;
            /// <summary>
            /// Sorted by endtime ascending.
            /// </summary>
            public LinkedList<DoTInstance> instances = new LinkedList<DoTInstance>();

        }
        public class DoTInstance : TagSerializable
        {
            public static readonly Func<TagCompound, DoTInstance> DESERIALIZER = Load;
            public static DoTInstance Load(TagCompound tag)
            {
                double endTime = PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds + tag.GetDouble("remainingTime");
                int dps = tag.GetInt("dps");
                return new DoTInstance(endTime, dps);
            }
            public TagCompound SerializeData()
            {
                TagCompound tag = new TagCompound
                {
                    { "remainingTime", endTime - PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds },
                    { "dps", dps }
                };
                return tag;
            }

            /// <summary>
            /// gametime in ms
            /// </summary>
            public readonly double endTime;
            public readonly int dps;

            public DoTInstance(double endTime, int dps)
            {
                this.endTime = endTime;
                this.dps = dps;
            }
        }


        public Dictionary<Type, DoTInstanceList> dotInstances = new Dictionary<Type, DoTInstanceList>();


        public void AddInstance(Type type, int dps, double durationMs)
        {
            DoTInstanceList dotInstaceList;

            if (!dotInstances.TryGetValue(type, out dotInstaceList))
            {
                dotInstaceList = new DoTInstanceList();
                dotInstances.Add(type, dotInstaceList);
            }
            DoTInstance instance = new DoTInstance(PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds + durationMs, dps);

            if (type.IsSubclassOf(typeof(StackingDamageOverTime)))
            {
                dotInstaceList.dps += dps;
            }
            else if (dps > dotInstaceList.dps)
            {
                dotInstaceList.dps = dps;
            }

            //Add instance into list sorted
            LinkedListNode<DoTInstance> lastNode = dotInstaceList.instances.Last;
            while (lastNode != null && instance.endTime < lastNode.Value.endTime)
            {
                lastNode = lastNode.Previous;
            }

            if (lastNode == null)
            {
                dotInstaceList.instances.AddFirst(instance);
            }
            else if (instance.endTime > lastNode.Value.endTime)
            {
                dotInstaceList.instances.AddAfter(lastNode, instance);
            }
        }
        public void ResetEffects()
        {
            foreach (var kv in dotInstances)
            {
                var type = kv.Key;
                var dotInstaceList = kv.Value;

                bool isStacking = type.IsSubclassOf(typeof(StackingDamageOverTime));
                bool needRecount = false;

                double now = PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds;
                var firstNode = dotInstaceList.instances.First;
                while (firstNode != null && now > firstNode.Value.endTime)
                {
                    if (isStacking)
                    {
                        dotInstaceList.dps -= firstNode.Value.dps;
                    }
                    else if (dotInstaceList.dps == firstNode.Value.dps)
                    {
                        needRecount = true;
                    }

                    dotInstaceList.instances.RemoveFirst();
                    firstNode = dotInstaceList.instances.First;
                }

                if (needRecount)
                {
                    if (firstNode == null)
                    {
                        dotInstaceList.dps = 0;
                    }
                    else
                    {
                        dotInstaceList.dps = dotInstaceList.instances.Max(x => x.dps);
                    }
                }
            }
        }
        public void Clear()
        {
            dotInstances.Clear();
        }
    }
}