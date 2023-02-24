using System;
using UnityEngine;

//ShiftDataGenerate类
//该类自动生成请勿修改，以避免不必要的损失
public class ShiftDataGenerate : DataGenerateBase 
{
	public string m_key;
	public DirectionEnum m_Direction; //方向
	public float m_Distance; //距离
	public float m_Height; //高度
	public float m_Time; //时间
	public Vector3 m_GhostColor; //残影颜色

	public override void LoadData(string key) 
	{
		DataTable table =  DataManager.GetData("ShiftData");

		if (!table.ContainsKey(key))
		{
			throw new Exception("ShiftDataGenerate LoadData Exception Not Fond key ->" + key + "<-");
		}

		SingleData data = table[key];

		m_key = key;
		m_Direction = data.GetEnum<DirectionEnum>("Direction");
		m_Distance = data.GetFloat("Distance");
		m_Height = data.GetFloat("Height");
		m_Time = data.GetFloat("Time");
		m_GhostColor = data.GetVector3("GhostColor");
	}
}
