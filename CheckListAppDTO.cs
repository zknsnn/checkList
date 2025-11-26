
public class CheckListAppDTO
{
    

    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }

    public CheckListAppDTO(){}
    public CheckListAppDTO(CheckListApp checkListApp) =>
        (Id,Name,IsComplete) = (checkListApp.Id,checkListApp.Name,checkListApp.IsComplete);
    
}   