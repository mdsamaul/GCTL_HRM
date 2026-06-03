using AutoMapper;
using GCTL.Core.ViewModels.Departments;
using GCTL.Core.ViewModels.Employees;
using GCTL.Core.ViewModels.LeaveTypes;
using GCTL.Data.Models;
using GCTL.UI.Core.Helpers.Mappers.Employees;

namespace GCTL.UI.Core.Helpers.Mappers.LeaveTypes
{
    public class LeaveTypeProfile:Profile
    {
        public LeaveTypeProfile()
        {
            CreateMap<HrmAtdLeaveType, LeaveTypeSetupViewModel>().ReverseMap();
           

        }


    }
}

