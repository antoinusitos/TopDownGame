using UnityEngine;
using UnityEngine.UI;

public class TextCopySlider : MonoBehaviour
{
    [SerializeField]
    private Slider      mySlider = null;

    private Text        myText = null;

    private void Start()
    {
        myText = GetComponent<Text>();
    }

    private void Update()
    {
        myText.text = mySlider.value.ToString() + " / " + mySlider.maxValue.ToString();
    }
}
