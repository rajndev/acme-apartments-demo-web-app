﻿using AcmeApartments.Data.Provider.Identity;
using System.Threading.Tasks;

namespace AcmeApartments.Providers.Interfaces
{
    public interface IUserService
    {
        public Task<AptUser> GetUserByID(string userId);
    }
}