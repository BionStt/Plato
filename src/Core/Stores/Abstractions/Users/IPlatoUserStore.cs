﻿using System.Threading.Tasks;

namespace PlatoCore.Stores.Abstractions.Users
{
    public interface IPlatoUserStore<T> : IStore<T> where T : class
    {

        Task<T> GetByUserNameNormalizedAsync(string userNameNormalized);

        Task<T> GetByUserNameAsync(string userName);

        Task<T> GetByEmailAsync(string email);

        Task<T> GetByEmailNormalizedAsync(string emailNormalized);

        Task<T> GetByResetToken(string resetToken);

        Task<T> GetByConfirmationToken(string confirmationToken);

        Task<T> GetByApiKeyAsync(string apiKey);

        Task<T> GetPlatoBotAsync();

    }

}