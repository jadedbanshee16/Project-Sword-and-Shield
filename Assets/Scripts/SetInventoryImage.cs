using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetInventoryImage : MonoBehaviour
{
    public Image _image;

    public void setImage(Sprite img)
    {
        _image.sprite = img;
    }
}
