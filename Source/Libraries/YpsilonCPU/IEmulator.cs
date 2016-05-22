using System.Collections.Generic;
using Ypsilon.Emulation;

namespace Ypsilon {
    public interface IEmulator {
        void Draw(List<ITexture> textures);
        void LoadBinaryToROM(string path);
        void Start();
        void Stop();
        void Update(float frameSeconds);
    }
}