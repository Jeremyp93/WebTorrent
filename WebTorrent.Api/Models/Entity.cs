namespace WebTorrent.Api.Models;
public abstract class Entity
{
    public Guid UserId { get; private set; }
    int? _requestedHashCode;
    Guid _Id;

    public DateTime? CreatedAt { get; private set; }

    public string CreatedBy { get; private set; } = string.Empty;

    public DateTime? LastModifiedAt { get; private set; }

    public string LastModifiedBy { get; private set; } = string.Empty;

    public void UpdateTrackingInformation(string authenticatedUser, DateTime modificationDate)
    {
        if (!CreatedAt.HasValue)
        {
            CreatedAt = modificationDate;
            CreatedBy = authenticatedUser;
        }
        LastModifiedAt = modificationDate;
        LastModifiedBy = authenticatedUser;
    }

    public virtual Guid Id
    {
        get
        {
            return _Id;
        }
        protected set
        {
            _Id = value;
        }
    }

    public bool IsTransient()
    {
        return Id == default;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not Entity)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (GetType() != obj.GetType())
            return false;
        Entity item = (Entity)obj;
        if (item.IsTransient() || IsTransient())
            return false;
        else
            return item.Id == Id;
    }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
                _requestedHashCode = Id.GetHashCode() ^ 31;
            // XOR for random distribution. See:
            // https://learn.microsoft.com/archive/blogs/ericlippert/guidelines-and-rules-for-gethashcode
            return _requestedHashCode.Value;
        }
        else
            return base.GetHashCode();
    }

    public static bool operator ==(Entity left, Entity right)
    {
        if (Equals(left, null))
            return (Equals(right, null));
        else
            return left.Equals(right);
    }

    public static bool operator !=(Entity left, Entity right)
    {
        return !(left == right);
    }

    public void AddUser(Guid userId)
    {
        UserId = userId;
    }
}