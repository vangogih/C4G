using System;
using System.Collections.Generic;
using C4G.Core.ConfigsSerialization;
using C4G.Core.Utils;

public struct Reward
{
    public int Id;
    public string Value;
}

public sealed class RewardsParser : IC4GTypeParser
{
    public Type ParsingType { get; } = typeof(List<Reward>);

    public Result<object, string> Parse(string value)
    {
        string[] splited = value.Split(':');
        var rewards = new List<Reward>();
        var reward = new Reward() {Id = int.Parse(splited[0]), Value = splited[1]};
        rewards.Add(reward);
        return Result<object, string>.FromValue(rewards);
    }
}