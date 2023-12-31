using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetInventoryImage : MonoBehaviour
{
    public Image _image;

    public Text _tex;

    public TextMeshProUGUI _meshTex;

    public Transform _bar;

    public void setImage(Sprite img)
    {
        _image.sprite = img;
    }

    public void setTex(string t)
    {
        _tex.text = t;
    }

    public void setTexMesh(string t)
    {
        _meshTex.text = t;
    }

    public void setBarScale(float num, float maxNum)
    {
        Vector3 scale = new Vector3((num - 0) / (maxNum - 0), 1, 1);
        _bar.localScale = scale;
    }
}
