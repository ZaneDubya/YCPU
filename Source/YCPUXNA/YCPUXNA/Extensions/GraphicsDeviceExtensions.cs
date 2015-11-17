using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

// Original code from http://xboxforums.create.msdn.com/forums/t/67895.aspx
// Gang of Penguins_01 and Michael Leone
// Conversion to extensionmethods by Jakob "xnafan" Krarup (www.xnafan.net)

/// <summary>
/// Class with extensionmethods for GraphicsDevice.
/// </summary>
public static class GraphicsDeviceExtensions
{

    #region Constants

    //the format of the screenshots' filename (with three digits to make many screenshots sort properly alphabetically)
    private const string ScreenshotFormat = "ScreenShot_{0:000}.png";

    #endregion


    #region public Screenshot code

    /// <summary>
    /// Prepares for a screenshot by using a RenderTarget2D to write content to in Reach mode.
    /// This method should be called before your spriteBatch.Begin(),
    /// and the SaveScreenshot() method should be called after spriteBatch.End()
    /// </summary>
    /// <param name="device">The GraphicsDevice to use</param>
    public static void PrepareScreenShot(this GraphicsDevice device)
    {
        if (device.GraphicsProfile == GraphicsProfile.Reach)
        {
            //sets the rendertarget to a new RenderTarget2D for this Draw()
            device.SetRenderTarget(new RenderTarget2D(device,
                device.PresentationParameters.BackBufferWidth,
                device.PresentationParameters.BackBufferHeight));
        }
    }

    /// <summary>
    /// Create a screenshot of the current screen. You should call ScreenshotPrepare() before your spriteBatch.Begin(), and this method after spriteBatch.End();
    /// </summary>
    /// <param name="device">The GraphicsDevice to use</param>
    /// <param name="fileName">The filename to save to. If fileName is null - a "ScreenShot_[number].png" format is used and the screenshot is saved to the game's running directory (where the EXE is)</param>
    public static void SaveScreenshot(this GraphicsDevice device, string fileName = null)
    {
        switch (device.GraphicsProfile)
        {
            case GraphicsProfile.Reach:
                device.SaveScreenShotReach(fileName);
                break;
            case GraphicsProfile.HiDef:
                device.SaveScreenshotHiDef(fileName);
                break;
        }
    }

    #endregion


    #region Internal screenshot code

    //saves a screenshot from the GPU to disk - HiDef only!
    internal static void SaveScreenshotHiDef(this GraphicsDevice device, string fileName = null)
    {
        //for storing the imagedata
        byte[] screenData = new byte[device.PresentationParameters.BackBufferWidth * device.PresentationParameters.BackBufferHeight * 4];

        //get the image as byte data
        device.GetBackBufferData(screenData);

        //use "using" to ensure .Dispose() is called
        using (Texture2D texture = new Texture2D(device, device.PresentationParameters.BackBufferWidth, device.PresentationParameters.BackBufferHeight, false, device.PresentationParameters.BackBufferFormat))
        {
            //insert the imagedata in the Texture2D
            texture.SetData(screenData);

            //save the texture
            SaveTexture(texture, fileName);
        }
    }


    // Creates a screenshot of the current screen in Reach mode
    internal static void SaveScreenShotReach(this GraphicsDevice device, string fileName = null)
    {
        //give useful advice if game is in Reach mode, but trying to use the SaveScreenshotHiDef method
        if (device.GetRenderTargets().Length == 0 || device.GetRenderTargets()[0].RenderTarget == null)
        {
            throw new Exception("It seems you didn't call the PrepareScreenShotReach() method before your spriteBatch.Begin().");
        }

        //get the current RenderTarget as a Texture2D
        Texture2D texture = (Texture2D)device.GetRenderTargets()[0].RenderTarget;

        //stop using the RenderTarget2D from now on
        device.SetRenderTarget(null);

        //save the rendertarget as a screenshot
        SaveTexture(texture, fileName);
    }

    #endregion


    #region Helpermethods

    //saves the texture
    private static void SaveTexture(Texture2D texture, string fileName)
    {

        //if we need to generate a filename - find an available one
        if (fileName == null)
        {
            fileName = GetUnusedFileName();
        }

        //try saving the texture
        try
        {

            //use "using" to ensure proper closing even if exceptions occur
            using (Stream st = new FileStream(fileName, FileMode.Create))
            {
                texture.SaveAsPng(st, texture.Width, texture.Height);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(string.Format("Error trying to save texture to '{0}'. Error is '{1}'.",
                                                fileName,
                                                ex.ToString()));
        }
    }

    //gives you an unused filename
    private static string GetUnusedFileName()
    {
        int i = 0;
        string fileName;
        do
        {
            //increment filenumber
            i++;
            //generate filename for testing
            fileName = string.Format(ScreenshotFormat, i);
        } while (File.Exists(fileName)); //keep incrementing filenumber till we find an unused name

        return fileName;
    }

    #endregion

}