using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class HealthPlayModeTests
{
    [UnityTest]
    public IEnumerator Died_IsRaised_WhenHealthReachesZero()
    {
        var go = new GameObject("Target");
        var health = go.AddComponent<Health>();
        bool died = false;
        health.Died += () => died = true;
        yield return null;   // дать отработать Awake/Start

        health.TakeDamage(1000f, Vector3.zero);

        Assert.IsTrue(died);
        Assert.IsFalse(health.IsAlive);
        Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator Ball_FallsUnderGravity()
    {
        var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.AddComponent<Rigidbody>();
        float startY = ball.transform.position.y;

        yield return new WaitForSeconds(0.5f);

        Assert.Less(ball.transform.position.y, startY);
        Object.Destroy(ball);
    }
}