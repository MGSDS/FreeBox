namespace FreeBox.Server.DataAccess.DatabaseEntities;

public class File
{
    public File(Client client, Guid id, string path, string name, string extension)
    {
        Client = client;
        Id = id;
        Path = path;
        Name = name;
        Extension = extension;
    }

    public Client Client { get; }
    public Guid Id { get; }
    public String Path { get; }
    public String Name { get; }
    public String Extension { get; }
}