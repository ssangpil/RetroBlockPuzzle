using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class GameDB : ScriptableObject
{
	public List<RefBlockGroup> RefBlockGroup; // Replace 'EntityType' to an actual type that is serializable.
	public List<RefBlankValue> RefBlankValue; // Replace 'EntityType' to an actual type that is serializable.
	public List<RefBlockSpawnRate> RefBlockSpawnRate; // Replace 'EntityType' to an actual type that is serializable.
	public List<RefCommonconfig> RefCommonconfig; // Replace 'EntityType' to an actual type that is serializable.
}
