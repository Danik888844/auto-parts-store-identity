using System.ComponentModel.DataAnnotations;

namespace AutoPartsIdentity.Core.DataAccess;

public interface IEntity
{
    [Key] public int Id { get; set; }
    public long CreatedDate { get; set; }
    public long ModifiedDate { get; set; }
    public bool IsDeleted { get; set; }
}