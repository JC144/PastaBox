namespace PastaBox.Models
{
    public class CopyableItem
    {
        public Guid Id { get; private set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }

        public CopyableItem()
        {
            Id = Guid.NewGuid();
        }
    }
}
