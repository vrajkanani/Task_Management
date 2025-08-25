namespace Task_Management.Models
{
    public class CompleteSubTaskModel
    {
        public int CompleteSubTaskId { get; set; }

        public int SubTaskId { get; set; }

        public int Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public int Priority { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public DateTime CompletedAt { get; set; } = DateTime.Now;
    }
}
