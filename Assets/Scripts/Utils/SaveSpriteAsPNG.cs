using UnityEngine;

public class SaveSpriteAsPNG : MonoBehaviour
{
    public Sprite[] spritesToSave;

    private void Start()
    {
        SaveSprites();
    }

    public void SaveSprites()
    {
        // Check if any sprites are assigned
        if (spritesToSave == null || spritesToSave.Length == 0)
        {
            Debug.LogWarning("No sprites assigned to save.");
            return;
        }

        // Loop through each sprite and save them as PNG
        for (int i = 0; i < spritesToSave.Length; i++)
        {
            Sprite sprite = spritesToSave[i];

            // Get the texture from the sprite
            Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.RGBA32, false);
            Color[] pixels = sprite.texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height);
            texture.SetPixels(pixels);

            // Apply changes to the new texture
            texture.Apply();

            // Encode the texture to a PNG file and save it
            byte[] bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/" + sprite.name + ".png", bytes);

            Debug.Log("Sprite saved as PNG: " + Application.dataPath + "/" + sprite.name + ".png");
        }
    }
}
