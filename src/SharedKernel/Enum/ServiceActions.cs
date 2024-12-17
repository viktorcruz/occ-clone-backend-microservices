public enum ApplicationLayer
{
    Controller,
    Repository,
    Application,
    Infrastructure,
    Domain,
    Adapter,
    Port,
    Handler
}

public enum ActionType
{
    Login,
    Register,
    Renew,
    Apply,
    Withdraw,
    Insert,
    Get,
    Update,
    Delete,
    Query,         // Para operaciones generales de consulta
    Execute,       // Para ejecuciones de procedimientos almacenados o funciones
    FetchAll,      // Para operaciones que buscan múltiples registros
    FetchSingle,   // Para operaciones que buscan un solo registro
    Save,          // Para operaciones de guardado (una combinación de inserción o actualización)
    Config,
    Token
}

