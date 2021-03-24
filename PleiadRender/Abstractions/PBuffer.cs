using Silk.NET.OpenGL;

public enum BufferType
{
    Vertex = 0,
    Element = 1
}

public class PBuffer
{
    private readonly GL _gl;
    private readonly BufferType _bufferType;

    public uint Buffer { get; init; }

    /// <summary>
    /// Creates and binds new buffer
    /// </summary>
    /// <param name="api">GL API</param>
    /// <param name="bufferType">Type of buffer</param>
    public PBuffer(GL api, BufferType bufferType)
    {
        _gl = api;
        _bufferType = bufferType;
        Buffer = _gl.GenBuffer();
        Bind();
    }

    public unsafe void SetData(float[] vertices)
    {
        fixed (void* v = &vertices[0])
        {
            _gl.BufferData(
                BufferTargetARB.ArrayBuffer,
                (nuint)(vertices.Length * sizeof(uint)),
                v,
                BufferUsageARB.StaticDraw);
        }
    }
    public unsafe void SetData(uint[] indices)
    {
        fixed (void* i = &indices[0])
        {
            _gl.BufferData(
                BufferTargetARB.ElementArrayBuffer,
                (nuint)(indices.Length * sizeof(uint)),
                i,
                BufferUsageARB.StaticDraw);
        }
    }
    public void Bind()
    {
        switch (_bufferType)
        {
            case BufferType.Vertex:
                {
                    _gl.BindBuffer(BufferTargetARB.ArrayBuffer, Buffer);
                }
                break;
            case BufferType.Element:
                {
                    _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, Buffer);
                }
                break;
            default:
                break;
        }
    }
    public void Unbind()
    {
        switch (_bufferType)
        {
            case BufferType.Vertex:
                {
                    _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
                }
                break;
            case BufferType.Element:
                {
                    _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
                }
                break;
            default:
                break;
        }
    }


    public static implicit operator uint(PBuffer pb) => pb.Buffer;
}

