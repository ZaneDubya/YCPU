using System.Collections.Generic;
using Ypsilon.Emulation;

namespace Ypsilon
{
    public interface IEmulator
    {
        void Initialize(IDisplayProvider display, IInputProvider input);
        void Start();
        void Stop();
        void Update(float frameMS);
        void Draw(List<ITexture> textures);
        void LoadBinaryToROM(string path);
    }
}