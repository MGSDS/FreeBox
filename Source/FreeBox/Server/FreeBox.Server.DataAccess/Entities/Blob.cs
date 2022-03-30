namespace FreeBox.Server.DataAccess.Entities;

public class Blob
{
    public Blob(byte[] binary)
    {
        Binary = binary;
        Id = Guid.Empty;
    }
    
    private Blob() { }
    
    public Guid Id { get; private init; }
    public byte[] Binary { get; private init; }
}