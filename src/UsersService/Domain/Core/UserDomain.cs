﻿using SharedKernel.Common.Responses;
using UsersService.Application.DTO;
using UsersService.Domain.Entity;
using UsersService.Domain.Interface;
using UsersService.Infrastructure.Interface;

namespace UsersService.Domain.Core
{
    public class UserDomain : IUserDomain
    {
        #region Properties
        public readonly IUserRepository _userRepository;
        #endregion

        #region Methods
        public UserDomain(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<RetrieveDatabaseResult<UserRetrieveDTO>> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        public async Task<RetrieveDatabaseResult<List<UserRetrieveDTO>>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<RetrieveDatabaseResult<List<SearchUsersDTO>>> SearchUsersAsync(string firstName, string lastName, string email)
        {
            return await _userRepository.SearchUsersAsync(firstName, lastName, email);
        }

        public async Task<DatabaseResult> UpdateUserAsync(UserRetrieveDTO userDTO)
        {
            return await _userRepository.UpdateUserAsync(userDTO);
        }

        public async Task<DatabaseResult> UpdateUserProfileAsync(UserProfileDTO userDTO)
        {
            return await _userRepository.UpdateUserProfileAsync(userDTO);
        }

        public async Task<DatabaseResult> ChangeUserPasswordAsync(int userId, string password, string email)
        {
            return await _userRepository.ChangeUserPasswordAsync(userId, password, email);
        }

        public async Task<DatabaseResult> DeleteUserAsync(int userId)
        {
            return await _userRepository.DeleteUserAsync(userId);
        }

        public async Task<RetrieveDatabaseResult<UserByEmailEntity>> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }


        #endregion
    }
}
