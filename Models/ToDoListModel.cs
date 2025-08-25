using System.ComponentModel.DataAnnotations;

namespace Task_Management.Models
{
    public class ToDoListModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public int Priority { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public bool IsCompleted { get; set; } = false;
        public int SubTaskCount { get; set; }
    }
    public class MainTaskModel
    {
        public int MainTaskId { get; set; }
        public string Title { get; set; }
    }

}
