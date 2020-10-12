namespace TodoListApi.Models
{
    public class TodoItem
    {
        public long Id { get; set; } // primary key
        public string Name { get; set; } = "";
        public bool IsDone { get; set; }

        public override string ToString()
            => $"<TodoItem Id={Id} Name=\"{Name}\" IsDone={IsDone}>";
    }
}
