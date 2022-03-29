namespace FreeBox.Server.DataAccess.DatabaseEntities;

public class Client
{
     public Client(IEnumerable<FileMode> files, string name, Guid id, string encriptionKey, string decriptionKey)
     {
          Files = files;
          Name = name;
          Id = id;
          EncriptionKey = encriptionKey;
          DecriptionKey = decriptionKey;
     }

     public IEnumerable<FileMode> Files { get; }
     public String Name { get; }
     public Guid Id { get; }
     public String EncriptionKey { get; }
     public String DecriptionKey { get; }
}