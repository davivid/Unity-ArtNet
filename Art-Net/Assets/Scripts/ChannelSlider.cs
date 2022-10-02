using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChannelSlider : MonoBehaviour
{
    public int channel;
    private Slider _slider;
    private ArtNet _artnet;

    void Start()
    {
        _artnet = FindObjectOfType<ArtNet>();

        _slider = this.GetComponent <Slider> ();
        _slider.onValueChanged.AddListener (delegate {OnValueChanged (_slider.value);});
    }

    public void OnValueChanged(float value)
    {
        int v = (int) (255f * value);
        bool result = _artnet.setChannel(channel, v);

        if (!result) {Debug.Log("Out of range");}
    }
}
