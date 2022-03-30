namespace FreeBox.Server.DataAccess.DatabaseModels;

public class ClientModel
{
     public ClientModel(string name)
     {
          Name = name;
          Id = Guid.Empty;
          Files = new List<FileModel>();
     }

     private ClientModel(){}

     public List<FileModel> Files { get; private init; }
     public String Name { get; private init; }
     public Guid Id { get; private init; }
}