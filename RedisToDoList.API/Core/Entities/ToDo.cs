namespace RedisToDoList.API.Core.Entities;

public class ToDo
{
    public ToDo(int id, string title, string description)
    {
        Id = id;
        Title = title;
        Description = description;
        Done = false;
    }

    public int Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool Done { get; private set; }
    
    public ToDo UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
        {
            throw new ArgumentNullException(nameof(newTitle), "O título não pode ser nulo ou vazio.");
        }
        Title = newTitle;
        return this;
    }
   
    public ToDo UpdateDescription(string newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription))
        {
            throw new ArgumentNullException(nameof(newDescription), "O título não pode ser nulo ou vazio.");
        }
        
        Description = newDescription;
        return this;
    }
    
    public ToDo MarkAsDone()
    {
        Done = true;
        return this;
    }
    
    public ToDo MarkAsNotDone()
    {
        Done = false;
        return this;
    }
    
}