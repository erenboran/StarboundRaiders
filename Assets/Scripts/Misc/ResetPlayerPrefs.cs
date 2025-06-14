using UnityEngine;

public class ResetPlayerPrefs : MonoBehaviour
{
    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Tüm PlayerPrefs verileri sıfırlandı!");
    }
}
