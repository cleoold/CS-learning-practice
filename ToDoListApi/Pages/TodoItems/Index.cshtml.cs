using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TodoListApi.Models;

namespace TodoListApi.Pages.TodoItems
{
    public class IndexModel : PageModel
    {
        private readonly TodoContext _context;

        public IList<TodoItem> TodoListItems { get; set; } = null!;

        public IndexModel(TodoContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> OnGetAsync()
        {
            TodoListItems = await _context.TodoItems.ToListAsync();
            return Page();
        }
    }
}
