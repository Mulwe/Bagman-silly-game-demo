using UnityEngine;

public class UI_MenuPause
{
    private string _name;
    private int _id;
    private GameObject _UIMenu;

    public UI_MenuPause(UI_Reference reference)
    {
        if (reference != null)
        {
            this._UIMenu = reference.gameObject;
            this._id = reference.GetInstanceID();
            this._name = reference.name;
        }
        else
        {
            Debug.Log("В класс передан null UI_reference");
        }
    }

    public int GetId()
    {
        return _id;
    }
    public string GetName()
    {
        return _name;
    }

    public void ShowUI(bool status)
    {
        if (_UIMenu != null)
            _UIMenu.SetActive(status);
    }

    public bool isShown()
    {
        if (_UIMenu != null)
            return (_UIMenu.activeSelf);
        return false;
    }


    public string ReturnStatus()
    {
        if (_UIMenu != null)
            return (_UIMenu.activeSelf == true ? "active" : "inactive");
        else
            return "null";
    }

    public GameObject hasBeenInitialized()
    {
        if (_UIMenu != null)
            return _UIMenu;
        return _UIMenu;
    }

}
