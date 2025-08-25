using System.ComponentModel.DataAnnotations;

namespace Task_Management.Models
{
    public class SubTaskModel
    {
        public int SubTaskId { get; set; }

        public int MainTaskId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        public int Priority { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public bool IsCompleted { get; set; }
    }
    public class MainTaskDropDownModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
