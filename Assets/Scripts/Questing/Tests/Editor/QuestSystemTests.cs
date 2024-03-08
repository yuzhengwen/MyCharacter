using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace YuzuValen
{
    public class QuestSystemTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void PlayerLevelReachedObjectiveTest()
        {
            PlayerStats stats = new();
            PlayerLevelReached obj = new(null, stats, 2);

            obj.Start();
            stats.AddExp(500);

            Assert.IsTrue(obj.IsComplete);
        }
    }
}
