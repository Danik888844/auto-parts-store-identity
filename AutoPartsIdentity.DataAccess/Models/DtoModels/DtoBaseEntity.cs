namespace AutoPartsIdentity.DataAccess.Models.DtoModels;

public class DtoBaseEntity
{
    public int Id { get; set; }
    public long CreatedDate { get; set; }
    public long ModifiedDate { get; set; }
    public bool IsDeleted { get; set; }
}