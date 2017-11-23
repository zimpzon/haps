﻿using UnityEngine;

public static class OfflineJson
{
    public static string jsonDonees =
@"
[
{ 'id':1000, 'name':'donee1', 'link':''},
{ 'id':1001, 'name':'donee2', 'link':''},
{ 'id':1002, 'name':'donee3', 'link':''},
{ 'id':1003, 'name':'donee4', 'link':''}
]
".Replace("'", "\"");

    public static string jsonTitles =
@"
[
{'id':100, 'pre':'Nybegynder', 'post':'', 'levelRequirement':5 },
{'id':101, 'pre':'', 'post':', Mother of Dragons', 'levelRequirement':6 },
{'id':102, 'pre':'Dr.', 'post':'', 'levelRequirement':7 },
{'id':103, 'pre':'Frøken', 'post':'', 'levelRequirement':8 }
]
".Replace("'", "\"");

    public static string jsonExternalDistribution =
@"
[
{'id':'C0', 'P':0.6},
{'id':'C1', 'P':0.5},
{'id':'C2', 'P':0.4},
{'id':'C3', 'P':0.3},
{'id':'C4', 'P':0.2},
{'id':'C5', 'P':0.1},
{'id':'TI', 'P':0.4},
{'id':'JP', 'P':0.05},
{'id':'KR', 'P':0.02}
]
".Replace("'", "\"");

    public static string jsonGameplayFlags =
@"
{
'enableAutoSpin': true, 
'clearHoldOnSpin': true,
'helpLastLevel': 10
}
".Replace("'", "\"");

}
