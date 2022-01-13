using System;
using System.Collections.Generic;
using REITs.Domain.Models;

namespace REITs.DataLayer.Services
{
    public interface IUserDataService
    {
        ICollection<SystemUser> GetAllActiveSystemUsers();
        SystemUser GetSystemUser(Guid id);
        SystemUser GetSystemUser(string userPID);
        string GetUserName(string pID);
        bool SaveSystemUser(SystemUser tempUser);
        bool UpdateSystemUser(SystemUser tempUser);
    }
}