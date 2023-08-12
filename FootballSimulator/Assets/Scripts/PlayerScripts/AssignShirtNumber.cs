using TMPro;

using UnityEngine;

[ExecuteInEditMode]
public class AssignShirtNumber : MonoBehaviour
{
   // Shirt Number
   public int ShirtNumber;

   private void Start ()
   {
      transform.GetComponentInChildren<TextMeshProUGUI>().text = ShirtNumber.ToString();
   }

   // Start is called before the first frame update
   private void Update ()
   {
      if (Application.isEditor)
      {
         transform.GetComponentInChildren<TextMeshProUGUI>().text = ShirtNumber.ToString();
      }
   }
}
