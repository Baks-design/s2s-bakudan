using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.LowLevel;

namespace Game.Runtime.Utilities.Helpers
{
    public static class PlayerLoopUtils
    {
        /// <summary>
        /// Removes a system from the player loop.
        /// </summary>
        /// <typeparam name="T">The type of the parent system to search within.</typeparam>
        /// <param name="loop">The root player loop system to modify.</param>
        /// <param name="systemToRemove">The system to remove.</param>
        public static void RemoveSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToRemove)
        {
            if (loop.subSystemList == null) return;

            var playerLoopSystemList = new List<PlayerLoopSystem>(loop.subSystemList);
            for (var i = 0; i < playerLoopSystemList.Count; i++)
            {
                if (playerLoopSystemList[i].type == systemToRemove.type &&
                    playerLoopSystemList[i].updateDelegate == systemToRemove.updateDelegate)
                {
                    playerLoopSystemList.RemoveAt(i);
                    loop.subSystemList = playerLoopSystemList.ToArray();
                    return;
                }
            }

            HandleSubSystemLoopForRemoval<T>(ref loop, systemToRemove);
        }

        static void HandleSubSystemLoopForRemoval<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToRemove)
        {
            if (loop.subSystemList == null) return;

            for (var i = 0; i < loop.subSystemList.Length; i++)
                RemoveSystem<T>(ref loop.subSystemList[i], systemToRemove);
        }

        /// <summary>
        /// Inserts a system into the player loop at a specified index.
        /// </summary>
        /// <typeparam name="T">The type of the parent system to insert into.</typeparam>
        /// <param name="loop">The root player loop system to modify.</param>
        /// <param name="systemToInsert">The system to insert.</param>
        /// <param name="index">The index at which to insert the system.</param>
        /// <returns>True if the system was successfully inserted; otherwise, false.</returns>
        public static bool InsertSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index)
        {
            if (loop.type == typeof(T))
            {
                var playerLoopSystemList = new List<PlayerLoopSystem>();
                if (loop.subSystemList != null)
                    playerLoopSystemList.AddRange(loop.subSystemList);

                if (index < 0 || index > playerLoopSystemList.Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

                playerLoopSystemList.Insert(index, systemToInsert);
                loop.subSystemList = playerLoopSystemList.ToArray();
                return true;
            }

            return HandleSubSystemLoop<T>(ref loop, systemToInsert, index);
        }

        static bool HandleSubSystemLoop<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index)
        {
            if (loop.subSystemList == null) return false;

            for (var i = 0; i < loop.subSystemList.Length; i++)
                if (InsertSystem<T>(ref loop.subSystemList[i], systemToInsert, index))
                    return true;
             
            return false;
        }

        /// <summary>
        /// Prints the structure of the player loop to the Unity console.
        /// </summary>
        /// <param name="loop">The root player loop system to print.</param>
        public static void PrintPlayerLoop(PlayerLoopSystem loop)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Unity Player Loop");
            foreach (var subSystem in loop.subSystemList)
                PrintSubsystem(subSystem, sb, 0);
            Debug.Log(sb.ToString());
        }

        static void PrintSubsystem(PlayerLoopSystem system, StringBuilder sb, int level)
        {
            sb.Append(' ', level * 2).AppendLine(system.type?.ToString() ?? "null");
            if (system.subSystemList == null || system.subSystemList.Length == 0) return;

            foreach (var subSystem in system.subSystemList)
                PrintSubsystem(subSystem, sb, level + 1);
        }
    }
}