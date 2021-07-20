using System;
using System.Runtime.InteropServices;
using Pleiad.Extensions.Files;
using Pleiad.Render.Handles;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Pleiad.Render.Models
{
    public sealed class PTexture : IDisposable
    {
        public unsafe PTexture(GL api, FileContract textureFile)
        {
            _gl = api;

            // load an image
            Image<Rgba32> img = (Image<Rgba32>)Image.Load(textureFile.FileName);
            // flip (imagesharp uses another coordinate system
            img.Mutate(x => x.Flip(FlipMode.Vertical));

            Width = (uint)img.Width;
            Height = (uint)img.Height;

            // load the image
            fixed (void* data = &MemoryMarshal.GetReference(img.GetPixelRowSpan(0)))
            {
                Load(data);
            }

            img.Dispose();
        }
        public unsafe PTexture(GL api, Span<byte> data, uint width, uint height)
        {
            _gl = api;
            Width = width;
            Height = height;

            fixed (void* d = &data[0])
            {
                Load(d);
            }
        }


        public TextureHandle Handle { get; private set; }
        public uint Width { get; private set; }
        public uint Height { get; private set; }


        public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
        {
            _gl.ActiveTexture(textureSlot);
            _gl.BindTexture(TextureTarget.Texture2D, Handle);
        }
        public void Dispose()
        {
            _gl.DeleteTexture(Handle);
        }
        public static implicit operator uint(PTexture t) => t.Handle;


        private unsafe void Load(void* data)
        {
            Handle = new(_gl.GenTexture());
            Bind();

            // set the data of the texture
            _gl.TexImage2D(TextureTarget.Texture2D,
                0,
                InternalFormat.Rgba,
                Width, Height,
                0,
                PixelFormat.Rgba, PixelType.UnsignedByte,
                data);

            // set texture parameters
            // wrap
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
            // filter
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            // generate mipmaps
            _gl.GenerateMipmap(TextureTarget.Texture2D);
        }
        private GL _gl;
    }
}
