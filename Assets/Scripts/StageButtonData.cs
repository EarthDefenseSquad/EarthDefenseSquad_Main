using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageButtonData : MonoBehaviour
{
    [Header("이 버튼이 언락되기 위한 Finish 아이템 ID")]
    public string requiredFinishID;

    [Header("이 버튼이 로딩할 스테이지 번호 (같은 씬 안에서 구분됨)")]
    public int stageIndex; // 예: 1, 2, 3 등
    
    [Header("이 버튼이 로딩할 연도")]
    public int yearIndex;
}
