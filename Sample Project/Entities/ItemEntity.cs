namespace FuzzySearchProject.Entities
{
    public class ItemEntity
    {
        public long ItemId { get; set; }

        public required string ItemName { get; set; }

        public string? Manufacturer { get; set; }

        public string? Category { get; set; }

        public string? Description { get; set; }

        public string? PartNumber { get; set; }

        public int Quantity { get; set; }
    }
}
