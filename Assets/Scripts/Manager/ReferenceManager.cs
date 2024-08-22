using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReferenceManager : MonoSingleton<ReferenceManager>
{
    [SerializeField] private GameDB gameDB;
    public RefBlockGroup FindRefBlockGroup(int key)
    {
        var item = gameDB.RefBlockGroup.FirstOrDefault(x => x.BlockGroupID == key);
        if (null == item)
            throw new Exception($"Does not exists key {key}.");

        return item;
    }

    public List<RefBlockGroup> GetRefBlockGroups()
    {
        return gameDB.RefBlockGroup;
    }

    public List<RefBlankValue> FindRefBlankValue(int key)
    {
        var items = gameDB.RefBlankValue.Where(x => x.BlockGroupID == key).ToList();
        if (!items.Any())
            throw new Exception($"Does not exists key {key}.");

        return items;
    }

    public List<RefBlankValue> GetRefBlankValues()
    {
        return gameDB.RefBlankValue;
    }

    public List<RefBlockSpawnRate> GetRefBlockSpawnRates()
    {
        return gameDB.RefBlockSpawnRate;
    }

    public RefCommonconfig FindRefCommonConfig(string key)
    {
        return gameDB.RefCommonconfig.Find(x => x.KeyName == key);
    }
}
