namespace AnimalService.DTOs
{
    public class ImageDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
    }
}