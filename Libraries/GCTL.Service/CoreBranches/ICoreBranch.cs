using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Data.Models;

namespace GCTL.Service.Branches
{
    public interface ICoreBranch
    {
        List<CoreBranch> GetBranchesByCompCode(string compCode);
    }
}
