using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;

public class DecoratorLoopUntil : Decorator
{
    public DecoratorLoopUntil(Node child)
            : base(child)
    {
    }

    public override IEnumerable<RunStatus> Execute()
    {
        DecoratedChild.Start();

        // While the child subtree is running, report that as our status
        RunStatus result;
        while ((result = this.TickNode(this.DecoratedChild)) == RunStatus.Running)
            yield return RunStatus.Running;

        DecoratedChild.Stop();

        // Return the opposite result that we received
        if (result == RunStatus.Success)
        {
            yield return RunStatus.Success;
            yield break;
        }

        yield return RunStatus.Failure;
        yield break;
    }
}
