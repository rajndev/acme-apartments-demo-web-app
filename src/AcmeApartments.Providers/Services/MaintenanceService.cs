﻿using AcmeApartments.Providers.DTOs;
using AcmeApartments.Providers.Interfaces;
using AcmeApartments.Providers.HelperClasses;
using AcmeApartments.Data.Provider.Identity;
using AcmeApartments.Data.Provider.Interfaces;
using AcmeApartments.Data.Provider.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcmeApartments.Providers.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MaintenanceService(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MaintenanceRequest> GetMaintenanceRequest(int maintenanceId)
        {
            var maintenanceRecord = await _unitOfWork.MaintenanceRequestRepository.Get(filter: maintenanceRecord => maintenanceRecord.Id == maintenanceId).FirstOrDefaultAsync();
            return maintenanceRecord;
        }

        public async Task<bool> SubmitMaintenanceRequest(NewMaintenanceRequestDto newMaintenanceRequestDTO, AptUser user)
        {
            try
            {
                var maintenanceRequest = new MaintenanceRequest
                {
                    User = user,
                    DateRequested = DateTime.Now,
                    isAllowedToEnter = newMaintenanceRequestDTO.isAllowedToEnter,
                    ProblemDescription = newMaintenanceRequestDTO.ProblemDescription,
                    Status = MaintenanceRequestStatus.PENDINGAPPROVAL
                };

                await _unitOfWork.MaintenanceRequestRepository.Insert(maintenanceRequest);
                await _unitOfWork.Save();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<List<MaintenanceRequest>> GetMaintenanceRequests(string userId)
        {
            var requests = await _unitOfWork.MaintenanceRequestRepository.Get(filter: u => u.AptUserId == userId).ToListAsync();
            return requests;
        }

        public async Task<List<AptUser>> GetMaintenanceRequestsUsers()
        {
            var users = await (from userRecord in _unitOfWork.AptUserRepository.Get()
                               join mRecord in _unitOfWork.MaintenanceRequestRepository.Get() on userRecord.Id equals mRecord.AptUserId
                               select userRecord).Distinct().ToListAsync();
            return users;
        }

        public async Task<List<MaintenanceRequest>> GetMaintenanceUserRequests(string aptUserId)
        {
            var requests = await (from userRecord in _unitOfWork.AptUserRepository.Get()
                                  join mRecord in _unitOfWork.MaintenanceRequestRepository.Get(includeProperties: "User") on userRecord.Id equals mRecord.AptUserId
                                  select mRecord).Where(s => s.AptUserId == aptUserId).ToListAsync();

            return requests;
        }

        public async Task<MaintenanceRequest> EditMaintenanceRequest(MaintenanceRequestEditDto maintenanceRequestEditDTO)
        {
            var maintenanceRecord = await _unitOfWork.MaintenanceRequestRepository.GetByID(maintenanceRequestEditDTO.Id);

            maintenanceRecord.ProblemDescription = maintenanceRequestEditDTO.ProblemDescription;
            maintenanceRecord.isAllowedToEnter = maintenanceRequestEditDTO.isAllowedToEnter;

            _unitOfWork.MaintenanceRequestRepository.Update(maintenanceRecord);
            await _unitOfWork.Save();

            return maintenanceRecord;
        }

        public async Task<bool> ApproveMaintenanceRequest(string userId, int maintenanceId)
        {
            try
            {
                var maintenanceRecord = await _unitOfWork.MaintenanceRequestRepository.GetByID(maintenanceId);

                maintenanceRecord.AptUserId = userId;
                maintenanceRecord.Status = MaintenanceRequestStatus.APPROVED;

                _unitOfWork.MaintenanceRequestRepository.Update(maintenanceRecord);
                await _unitOfWork.Save();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DenyMaintenanceRequest(string userId, int maintenanceId)
        {
            try
            {
                var maintenanceRecord = await _unitOfWork.MaintenanceRequestRepository.GetByID(maintenanceId);
                maintenanceRecord.AptUserId = userId;
                maintenanceRecord.Status = MaintenanceRequestStatus.UNAPPROVED;

                _unitOfWork.MaintenanceRequestRepository.Update(maintenanceRecord);
                await _unitOfWork.Save();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
