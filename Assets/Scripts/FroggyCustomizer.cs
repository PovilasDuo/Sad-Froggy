using UnityEngine;

public class FroggyCustomizer : MonoBehaviour
{
    public PlayerSettings playerSettings;

    void Start()
    {
       ApplyItems();
    }

    void ApplyItems()
    {
        if (playerSettings.decoration == PlayerSettings.Decoration.Crown) AddObject("Armature/spina/crown");
        else if (playerSettings.decoration == PlayerSettings.Decoration.Mushroom) AddObject("Armature/spina/mushroom_hat");
        if (playerSettings.skin == PlayerSettings.Skin.Default) ApplyMaterial("ExtraMaterials/FrogMat");
        else if (playerSettings.skin == PlayerSettings.Skin.Pink) ApplyMaterial("ExtraMaterials/PinkFrogMat");
        else if (playerSettings.skin == PlayerSettings.Skin.Blue) ApplyMaterial("ExtraMaterials/BlueFrogMat");
    }

    private void AddObject(string path)
    {
        Transform obj = gameObject.transform.Find(path);
        obj.gameObject.SetActive(true);
    }

    private void ApplyMaterial (string path)
    {
        Material material = Resources.Load<Material>(path);
        Transform body = gameObject.transform.Find("Roundcube");
        Renderer renderer = body.GetComponent<Renderer>();
        renderer.material = material;
    }
}
