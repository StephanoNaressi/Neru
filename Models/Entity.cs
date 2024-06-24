using System.ComponentModel.DataAnnotations;
namespace Neru.Models
{
    public abstract class Entity
    {
        [Key] public int Id { get; set; }
    }
}
