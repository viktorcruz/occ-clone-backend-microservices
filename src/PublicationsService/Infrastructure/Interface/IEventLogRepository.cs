﻿using Dapper;

namespace PublicationsService.Infrastructure.Interface
{
    public interface IEventLogRepository
    {
        Task SaveEventLog(string query, DynamicParameters parameters);
    }
}