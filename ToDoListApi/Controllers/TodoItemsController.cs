using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TodoListApi.Models;

namespace TodoListApi.Controllers
{
    [Route("api/TodoItems")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly ILogger<TodoItemsController> _logger;

        public TodoItemsController(TodoContext context, ILogger<TodoItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
                return NotFound();

            return todoItem;
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /* Sample post body:
            {
                "Name": "eat",
                "IsDone": false
            }
        */
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            if (String.IsNullOrEmpty(todoItem.Name))
                return BadRequest(new { reason = "Invalid name" });
            if (todoItem.Id != new long())
                return BadRequest(new { reason = "Should not provide an id" });

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"New item added: {todoItem}");

            // sends 201 code, with a uri that can be GETed to access the item
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // POST: api/TodoItems/csv
        /* Sample post body:
            done eat dinner
            undone do coding
            undone sleep
        */
        [HttpPost("csv")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> PostTodoItemCsv([FromBody] string csv)
        {
            var items = csv.Split('\n')
                .Select(line =>
                {
                    bool isDone;
                    string name;
                    if (line.StartsWith("done "))
                    {
                        isDone = true;
                        name = line.Substring(5);
                    }
                    else if (line.StartsWith("undone "))
                    {
                        isDone = false;
                        name = line.Substring(7);
                    }
                    else
                    {
                        return null;
                    }
                    return new TodoItem { Name = name, IsDone = isDone };
                })
                .ToList();

            if (items == null || items.Count() == 0 || items.Contains(null))
                return BadRequest(new { reason = "Invalid csv format" });

            _context.TodoItems.AddRange(items!);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"New items added via csv:\n{csv}");

            return CreatedAtAction(nameof(GetTodoItems), items);
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /* Sample post body:
            {
            "Id": 1,
            "Name": "eat",
            "IsDone": true
            }
        the whole body is needed.
        */
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
                return BadRequest();

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Item edited: {todoItem}");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
                return NotFound();

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Item deleted: {todoItem}");

            return todoItem;
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
