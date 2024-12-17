public enum AuditEntityType
{
    Unknown = 0,
    Authorize = 1,
    Publication = 2,
    Job = 3,
    User = 4,
}

public enum AuditOperationType
{
    Login = 1,
    Confirm = 2,
    Register = 3,
    Renew = 4,
    ChangePassword = 5,
    Create = 6,
    Update = 7,
    Delete = 8,
    Get = 9,
    GetAll = 10,
    Application = 11,
    Apply = 12,
    Search = 13,
    Withdraw = 14,
    
}

public enum AuditActionType
{
    Created,
    Updated,
    Deleted,
    Applied,
    Confirmed,
    Failed,
    Retrieved,
    RetrievedAll,
    CreatedSuccess,
    CreatedError,
    CreatedFailed,
    LoggedIn,
    UpdatedSuccess,
    UpdatedError,
    UpdatedFailed,
    DeletedSuccess,
    DeletedError,
    DeletedFailed,
    Registered,
    Renewed,
    Reverted,
    Searched,
    Withdrawn,
}

public enum AuditActionStatus
{
    Success,
    Error,
    Failed
}