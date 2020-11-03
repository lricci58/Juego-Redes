using System;
using UnityEngine;
using UnityEngine.UI;

public class ReserveUnitButton : MonoBehaviour
{
    [NonSerialized] public bool selected = false;

    void Start() => GetComponent<Button>().onClick.AddListener(SelectReserveUnit);

    private void SelectReserveUnit() => selected = true;

    public void UnselectReserveUnit() => selected = false;
}
