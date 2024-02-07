using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public class PlayerListItem : MonoBehaviour
{
    public string PlayerName;
    public int ConnectionID;
    public ulong PlayerSteamID;
    private bool AvatarReceived;

    public TextMeshProUGUI PlayerNameText;
    public RawImage PlayerIcon;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    private void Start()
    {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
    }

    public void SetPlayerValues()
    {
        PlayerNameText.text = PlayerName;
        if(!AvatarReceived)
        {
            GetPlayerIcon();
        }
    }

    void GetPlayerIcon()
    {
        int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamID);
        if (ImageID == -1)
            return;
        PlayerIcon.texture = GetSteamImageAsTexture(ImageID);
    }

    private void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if(callback.m_steamID.m_SteamID == PlayerSteamID)
        {
            PlayerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
        else
        {
            return;
        }
    }
    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        AvatarReceived = true;
        return FlipTextureUpsideDown(texture);
    }

    private Texture2D FlipTextureUpsideDown(Texture2D originalTexture)
    {
        Texture2D flippedTexture = new Texture2D(originalTexture.width, originalTexture.height);

        for (int x = 0; x < originalTexture.width; x++)
        {
            for (int y = 0; y < originalTexture.height; y++)
            {
                flippedTexture.SetPixel(x, originalTexture.height - y - 1, originalTexture.GetPixel(x, y));
            }
        }

        flippedTexture.Apply();
        return flippedTexture;
    }
}
