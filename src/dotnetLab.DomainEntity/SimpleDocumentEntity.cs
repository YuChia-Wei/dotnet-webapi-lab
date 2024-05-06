namespace dotnetLab.DomainEntity;

public class SimpleDocumentEntity
{
    public SimpleDocumentEntity(string newDocumentNum, string? newDescription)
    {
        this.DocumentNum = newDocumentNum;
        this.Description = newDescription;
    }

    public SimpleDocumentEntity()
    {
    }

    /// <summary>
    /// 序號
    /// </summary>
    public int SerialId { get; set; }

    /// <summary>
    /// 敘述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 文件編號
    /// </summary>
    public string DocumentNum { get; set; } = string.Empty;
}