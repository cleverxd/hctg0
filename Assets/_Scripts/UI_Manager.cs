using TMPro;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public TMP_Text building1Text;
    public TMP_Text building2Text;
    public TMP_Text building3Text;

    public Building building1;
    public Building building2;
    public Building building3;

    private void FixedUpdate()
    {
        building1Text.text = "Production 1 Status: " + building1.status + "";
        building2Text.text = "Production 2 Status: " + building2.status + "";
        building3Text.text = "Production 3 Status: " + building3.status + "";
    }
}
