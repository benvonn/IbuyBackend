namespace Ibuy.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }
        public bool IsSold { get; set; } = false;
        public DateTime? SoldAt {  get; set; }
    }

    public class ItemRequestDto
    {
        public string ItemName { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public int UserId { get; set; }
    }

}
