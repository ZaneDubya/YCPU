using System.Collections.Generic;
using Ypsilon.Emulation;

namespace Ypsilon
{
    public interface IEmulator
    {
        void Start();
        void Stop();
        void Update(float frameSeconds);
        void Draw(List<ITexture> textures);
        void LoadBinaryToROM(string path);
    }
}