using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MonsterBirthPointMetaData
{
	public int monsterTemplateID;
	public Vector3 position;
}

[System.Serializable]
public class DecorationMetaData
{
	public int decorationTemplateID;
	public Vector3 position;
}

[System.Serializable]
public class DoorMetaData
{
	public int animationID;
	public Vector3 position;
}

[System.Serializable]
public class LevelTemplate
{
	public int levelID;
	public int sceneID;

	public List<MonsterBirthPointMetaData> birthPoints = new List<MonsterBirthPointMetaData>();
	public List<DecorationMetaData> decorations = new List<DecorationMetaData>();
	public DoorMetaData leftDoor;
	public DoorMetaData rightDoor;
	public DoorMetaData topDoor;
	public DoorMetaData bottomDoor;
}
