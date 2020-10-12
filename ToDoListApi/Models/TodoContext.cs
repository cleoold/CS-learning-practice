using Microsoft.EntityFrameworkCore;

namespace TodoListApi.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        // https://stackoverflow.com/questions/57342964/how-can-i-hint-the-c-sharp-8-0-nullable-reference-system-that-a-property-is-init
        public DbSet<TodoItem> TodoItems { get; set; } = null!;
    }
}
