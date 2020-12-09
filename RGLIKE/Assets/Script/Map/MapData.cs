using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapData
{
	public int stage;
	public int mapTotalNum;
	public int mapTotalSqrt;
	public int mapConnectNum;
	public bool[] maps; // 타일 맵 생성
	public bool[] connectDir; // 맵 연결방향(maps*4) = 0:left, 1:right, 2:up, 3:down,
    public IMacro.MAP_NAME[] mapName;
	public MapState[] mapStates;

	public MapData()
	{ // new
        stage = 0;
        initMapData();
        createMap();
    }

	public MapData(MapData md)
	{ // load
        stage = md.stage;
		mapTotalNum = md.mapTotalNum;
		mapTotalSqrt = md.mapTotalSqrt;
		mapConnectNum = md.mapConnectNum;
		maps = (bool[])md.maps.Clone();
		connectDir = (bool[])md.connectDir.Clone();
        mapName = (IMacro.MAP_NAME[])md.mapName.Clone();
		mapStates = (MapState[])md.mapStates.Clone();
	}


    //---------------------------------------------------------------------------
    // initMapData
    public void nextMap()
    {
        stage++;
        initMapData();
        createMap();
    }

    private void initMapData()
	{
        mapTotalNum = 16;
        mapTotalSqrt = (int)UnityEngine.Mathf.Sqrt(mapTotalNum);
        mapConnectNum = 9;
        maps = new bool[mapTotalNum];
        connectDir = new bool[mapTotalNum * 4];
        mapName = new IMacro.MAP_NAME[mapTotalNum];
        mapStates = new MapState[mapTotalNum];
    }

    //---------------------------------------------------------------------------
    // createMapData
    private void createMap()
    {
        int i, j;
        int totalNum = mapTotalNum;
        int connectNum = mapConnectNum;
        ref bool[] check = ref maps;
        bool[] visit = new bool[totalNum];
        int random;
        int connected = 0;
        int count;

        // MapData 생성, 연결
        while (connected != connectNum)
        {
            connected = 0;
            count = 0;
            for (i = 0; i < totalNum; i++) 
			{
                check[i] = false;
                visit[i] = false;
            }

            while (count != connectNum)
            {
                random = UnityEngine.Random.Range(0, totalNum);
                if (check[random] == false)
                {
                    check[random] = true;
                    count++;
                }
            }

            for (i = 0; i < totalNum; i++)
            {
                connectMapData(visit, check, i, ref connected);

                if (connected == connectNum)
                    break;

                connected = 0;
                for (j = 0; j < totalNum; j++)
                    visit[i] = false;
            }
        }

        // 현재맵에 연결된 맵 체크 후 맵프리팹 생성
        for (i = 0; i < totalNum; i++)
            connectCountCheck(check, i);

        for (i = totalNum - 1; i > -1; i--)
        {
            if (maps[i])
            {
                mapName[i] = IMacro.MAP_NAME.room_boss0;
                mapStates[i] = MapState.boss;
#if false // shop
                for (j = i - mapTotalSqrt - 2; j > -1; j--)
				{
                    if(maps[j])
					{
                        mapName[i] = IMacro.MAP_NAME.room_4way;
                        mapStates[i] = MapState.shop;
                        break;
                    }
				}
#endif
                break;
            }
        }
    }

    private void connectMapData(bool[] visit, bool[] check, int index, ref int connected)
    {
        // 맵이 mapConnectNum 만큼 연결됐는지 확인
        if (index < 0 || index > mapTotalNum - 1)
            return;

        if (check[index] == false || visit[index] == true)
            return;

        visit[index] = true;
        connected++;

        int sqrt = mapTotalSqrt;
        if (index % sqrt > 0) connectMapData(visit, check, index - 1, ref connected);
        if (index % sqrt < sqrt - 1) connectMapData(visit, check, index + 1, ref connected);
        if (index / sqrt > 0) connectMapData(visit, check, index - sqrt, ref connected);
        if (index / sqrt < sqrt - 1) connectMapData(visit, check, index + sqrt, ref connected);
    }

    private void connectCountCheck(bool[] check, int i)
    {
        if (!check[i])
            return;

        mapStates[i] = MapState.nomal;
        int sqrt = mapTotalSqrt;
        int n = i * 4;
        ref bool l = ref connectDir[n + 0];
        ref bool r = ref connectDir[n + 1];
        ref bool u = ref connectDir[n + 2];
        ref bool d = ref connectDir[n + 3];

        if (i % sqrt > 0) l = check[i - 1];
        if (i % sqrt < sqrt - 1) r = check[i + 1];
        if (i / sqrt < sqrt - 1) u = check[i + sqrt];
        if (i / sqrt > 0) d = check[i - sqrt];
        // 위아래 반대로 검사함

        int sum = 0;
        if (l) sum++;
        if (r) sum++;
        if (u) sum++;
        if (d) sum++;
        ref IMacro.MAP_NAME mn = ref mapName[i];
        switch (sum)
        {
            case 4:
                {
                    mn = IMacro.MAP_NAME.room_4way;
                    break;
                }
            case 3:
                {
                    if      (r && u && d) mn = IMacro.MAP_NAME.room_3way0;
                    else if (l && u && d) mn = IMacro.MAP_NAME.room_3way1;
                    else if (l && r && d) mn = IMacro.MAP_NAME.room_3way2;
                    else if (l && r && u) mn = IMacro.MAP_NAME.room_3way3;
                    else
                        Debug.Log("Map Connect Check error : " + i + "번째");
                    break;
                }
            case 2:
                {
                    if      (l && r) mn = IMacro.MAP_NAME.room_2way0;
                    else if (u && d) mn = IMacro.MAP_NAME.room_2way1;
                    else if (l && u) mn = IMacro.MAP_NAME.room_2way2;
                    else if (l && d) mn = IMacro.MAP_NAME.room_2way3;
                    else if (r && u) mn = IMacro.MAP_NAME.room_2way4;
                    else if (r && d) mn = IMacro.MAP_NAME.room_2way5;
                    else
                        Debug.Log("Map Connect Check error : " + i + "번째");
                    break;
                }
            case 1:
                {
                    if      (l) mn = IMacro.MAP_NAME.room_1way0;
                    else if (r) mn = IMacro.MAP_NAME.room_1way1;
                    else if (u) mn = IMacro.MAP_NAME.room_1way2;
                    else if (d) mn = IMacro.MAP_NAME.room_1way3;
                    else
                        Debug.Log("Map Connect Check error : " + i + "번째");
                    break;
                }
            case 0:
                {
                    Debug.Log("Map Connect Check error : " + i + "번째");
                    break;
                }
        }

    }
}
