using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.Data;
using GCTL.Data.Models;
using GCTL.Service.Branches;

namespace GCTL.Service.CoreBranches
{
    //TODO: Sf
    public class CoreBranchService :  ICoreBranch
    {

        private readonly IRepository<CoreBranch> _branchRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        public CoreBranchService(IRepository<CoreBranch> branchRepository, IRepository<CoreAccessCode> accessCodeRepository)
        {
            _branchRepository = branchRepository;
            this.accessCodeRepository = accessCodeRepository;
        }

        public List<CoreBranch> GetBranchesByCompCode(string compCode)
        {
            try
            {
                var result = _branchRepository.GetAll().Where(e => e.CompanyCode == compCode).ToList();
                return result;
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
