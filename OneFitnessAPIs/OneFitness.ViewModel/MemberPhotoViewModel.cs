using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class MemberPhotoViewModel
    {
        // Image as a data URL, e.g. "data:image/jpeg;base64,/9j/4AAQ..."
        [Required]
        public string Photo { get; set; } = string.Empty;
    }
}
