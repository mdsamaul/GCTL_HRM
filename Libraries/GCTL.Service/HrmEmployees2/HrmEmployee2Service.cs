using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HrmEmployees2;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading.Tasks;

namespace GCTL.Service.HrmEmployees2
{
    public class HrmEmployee2Service : AppService<HrmEmployee>, IHrmEmployee2Service
    {
        #region Service & Repository

        private readonly IRepository<HrmEmployee> empRepository;
        private readonly IRepository<HrmDefSex> sexRepository;
        private readonly IRepository<HrmDefReligion> religionRepository;
        private readonly IRepository<HrmDefMaritalStatus> maritalStatusRepository;
        private readonly IRepository<HrmDefBloodGroup> bloodGrouypRepository;
        private readonly IRepository<CoreBranch> coreBranchRepository;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IRepository<HrmDefNationality> nationalityRepository;
        private readonly IRepository<HrmEmployeePhoto> photoRepository;
        private readonly IRepository<HrmEmpDigitalSignature> signatureRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly GCTL_ERP_DB_DatapathContext contexService;
        private readonly IRepository<HrmEmployeeQualification> qualificationRepository;
        private readonly IDeleteHistoryService deleteHistoryRepository;

        public HrmEmployee2Service(
            IRepository<HrmEmployee> empRepository,
            GCTL_ERP_DB_DatapathContext contexService,
            IRepository<HrmEmpDigitalSignature> signatureRepository,
            IRepository<HrmEmployeePhoto> photoRepository,
            IRepository<HrmDefSex> sexRepository,
            IRepository<HrmDefReligion> religionRepository,
            IRepository<HrmDefMaritalStatus> maritalStatusRepository,
            IRepository<HrmDefBloodGroup> bloodGrouypRepository,
            IRepository<CoreBranch> coreBranchRepository,
            IRepository<CoreCompany> coreCompanyRepository,
            IRepository<HrmDefNationality> nationalityRepository,
            IRepository<CoreAccessCode> accessCodeRepository,
            IRepository<HrmEmployeeQualification> qualificationRepository,
            IDeleteHistoryService deleteHistoryRepository
            
            )
    : base(empRepository)
        {
            this.empRepository = empRepository;
            this.sexRepository = sexRepository;
            this.religionRepository = religionRepository;
            this.maritalStatusRepository = maritalStatusRepository;
            this.bloodGrouypRepository = bloodGrouypRepository;
            this.coreBranchRepository = coreBranchRepository;
            this.coreCompanyRepository = coreCompanyRepository;
            this.nationalityRepository = nationalityRepository;
            this.accessCodeRepository = accessCodeRepository;
            this.photoRepository = photoRepository;
            this.signatureRepository = signatureRepository;
            this.contexService = contexService;
            this.qualificationRepository = qualificationRepository;
            this.deleteHistoryRepository = deleteHistoryRepository;
        }

        #endregion

        #region GetAllAsync
        public async Task<List<HrmEmployee2SetUpViewModel>> GetAllAsync()
        {
            var data = (from emp in empRepository.All().AsNoTracking()

                        join sex in sexRepository.All().AsNoTracking()
                        on emp.SexCode equals sex.SexCode into empSex
                        from sex in empSex.DefaultIfEmpty()

                        join nationality in nationalityRepository.All().AsNoTracking()
                        on emp.NationalityCode equals nationality.NationalityCode into empNationality
                        from nationality in empNationality.DefaultIfEmpty()

                        //join religion in religionRepository.All().AsNoTracking()
                        //on emp.ReligionCode equals religion.ReligionCode into empReligion
                        //from religion in empReligion.DefaultIfEmpty()

                        //join maritalStatus in maritalStatusRepository.All().AsNoTracking()
                        //on emp.MaritalStatusCode equals maritalStatus.MaritalStatusCode into empMaritalstatus
                        //from maritalStatus in empMaritalstatus.DefaultIfEmpty()

                        //join blood in bloodGrouypRepository.All().AsNoTracking()
                        //on emp.BloodGroupCode equals blood.BloodGroup into empBlood
                        //from blood in empBlood.DefaultIfEmpty()

                        //join company in coreCompanyRepository.All().AsNoTracking()
                        //on emp.CompanyCode equals company.CompanyCode into empComapny
                        //from company in empComapny.DefaultIfEmpty()

                        //join branch in coreBranchRepository.All().AsNoTracking()
                        //on emp.BranchCode equals branch.BranchCode into empBranch
                        //from branch in empBranch.DefaultIfEmpty()

                        join photo in photoRepository.All().AsNoTracking()
                        on emp.EmployeeId equals photo.EmployeeId into empPhoto
                        from photo in empPhoto.DefaultIfEmpty()

                        join signature in signatureRepository.All().AsNoTracking()
                        on emp.EmployeeId equals signature.EmployeeId into empSignature
                        from signature in empSignature.DefaultIfEmpty()

                        select new HrmEmployee2SetUpViewModel
                        {
                            AutoId = emp.AutoId,
                            EmployeeId = emp.EmployeeId,
                            FirstName = emp.FirstName ?? "",
                            LastName = emp.LastName ?? "",
                            FatherName = emp.FatherName ?? "",

                            MotherName = emp.MotherName ?? "",
                            FirstNameBangla = emp.FirstNameBangla ?? "",
                            LastNameBangla = emp.LastNameBangla ?? "",
                            DateOfBirthCertificate = emp.DateOfBirthCertificate,
                            DateOfBirthOrginal = emp.DateOfBirthOrginal,
                            BirthCertificateNo = emp.BirthCertificateNo ?? "",
                            PlaceOfBirth = emp.PlaceOfBirth ?? "",
                            SexCode = sex.Sex ?? "",
                            
                            NationalityCode = nationality.Nationality ?? "",
                            //ReligionCode = religion.Religion ?? "",
                            //MaritalStatusCode = maritalStatus.MaritalStatus ?? "",
                            //BloodGroupCode = blood.BloodGroup ?? "",
                            //CompanyCode = company.CompanyName ?? "",
                            //BranchCode = branch.BranchName ?? "",

                            //
                            NationalIdno = emp.NationalIdno ?? "",
                            //MarriageDate = emp.MarriageDate,
                            //NoOfSon = emp.NoOfSon ?? "",
                            //NoOfDaughters = emp.NoOfDaughters,
                            //CardNo = emp.CardNo ?? "",
                            PersonalEmail = emp.PersonalEmail ?? "",
                            Telephone = emp.Telephone ?? "",
                            //TinNo = emp.TinNo ?? "",
                           // ExtraCurriActivities = emp.ExtraCurriActivities,
                            //Remarks = emp.Remarks,
                            //Ldate = emp.Ldate,
                            //ModifyDate = emp.ModifyDate,
                            PhotoUrl = photo != null ? $"data:{photo.ImgType};base64,{Convert.ToBase64String(photo.Photo)}" : " ",
                            DigitalSignatureUrl = signature != null ? $"data:{signature.ImgType};base64,{Convert.ToBase64String(signature.DigitalSignature)}" : " "

                        }).ToList();

            return await Task.FromResult(data);
        }
        #endregion

        #region GetByIdAsync

        public async Task<HrmEmployee2SetUpViewModel> GetByIdAsync(string id)
        {
            var data = await (from emp in empRepository.All().AsNoTracking()

              join sex in sexRepository.All().AsNoTracking()
              on emp.SexCode equals sex.SexCode into empSex
              from sex in empSex.DefaultIfEmpty() 

              join nationality in nationalityRepository.All().AsNoTracking()
              on emp.NationalityCode equals nationality.NationalityCode into empNationality
              from nationality in empNationality.DefaultIfEmpty() 

              join religion in religionRepository.All().AsNoTracking()
              on emp.ReligionCode equals religion.ReligionCode into empReligion
              from religion in empReligion.DefaultIfEmpty()

              join maritalStatus in maritalStatusRepository.All().AsNoTracking()
              on emp.MaritalStatusCode equals maritalStatus.MaritalStatusCode into empMaritalstatus
              from maritalStatus in empMaritalstatus.DefaultIfEmpty()

              join blood in bloodGrouypRepository.All().AsNoTracking()
              on emp.BloodGroupCode equals blood.BloodGroupCode into empBlood
              from blood in empBlood.DefaultIfEmpty()

              join company in coreCompanyRepository.All().AsNoTracking()
              on emp.CompanyCode equals company.CompanyCode into empCompany
              from company in empCompany.DefaultIfEmpty()

              join branch in coreBranchRepository.All().AsNoTracking()
              on emp.BranchCode equals branch.BranchCode into empBranch
              from branch in empBranch.DefaultIfEmpty()

              join photo in photoRepository.All().AsNoTracking()
              on emp.EmployeeId equals photo.EmployeeId into empPhoto
              from photo in empPhoto.DefaultIfEmpty()

              join signature in signatureRepository.All().AsNoTracking()
              on emp.EmployeeId equals signature.EmployeeId into empSignature
              from signature in empSignature.DefaultIfEmpty()

              where emp.EmployeeId == id

              select new HrmEmployee2SetUpViewModel
              {
                  AutoId = emp.AutoId,
                  EmployeeId = emp.EmployeeId,
                  FirstName = emp.FirstName,
                  SexCode = sex.SexCode,
                  ReligionCode = religion.ReligionCode,
                  BranchCode = branch.BranchCode,
                  CompanyCode = company.CompanyCode,
                  BloodGroupCode = blood.BloodGroupCode,
                  NationalityCode = nationality.NationalityCode,
                  MaritalStatusCode = maritalStatus.MaritalStatusCode,
                  LastName = emp.LastName,
                  FatherName = emp.FatherName,
                  MotherName = emp.MotherName,
                  FirstNameBangla = emp.FirstNameBangla,
                  LastNameBangla = emp.LastNameBangla,
                  DateOfBirthCertificate = emp.DateOfBirthCertificate,
                  DateOfBirthOrginal = emp.DateOfBirthOrginal,
                  BirthCertificateNo = emp.BirthCertificateNo,
                  FatherOccupation = emp.FathersOccupation,
                  MotherOccupation = emp.MothersOccupation,
                  PlaceOfBirth = emp.PlaceOfBirth,
                  SexName = sex.Sex,
                  Nationality = nationality.Nationality,
                  Religion = religion.Religion,
                  MaritalStatus = maritalStatus.MaritalStatus,
                  BloodGroup = blood.BloodGroup,
                  Company = company.CompanyName,
                  Branch = branch.BranchName,
                  NationalIdno = emp.NationalIdno,
                  MarriageDate = emp.MarriageDate,
                  NoOfSon = emp.NoOfSon,
                  NoOfDaughters = emp.NoOfDaughters,
                  CardNo = emp.CardNo,
                  PersonalEmail = emp.PersonalEmail,
                  Telephone = emp.Telephone,
                  TinNo = emp.TinNo,
                  ExtraCurriActivities = emp.ExtraCurriActivities,
                  Remarks = emp.Remarks,
                  Ldate = emp.Ldate,
                  ModifyDate = emp.ModifyDate,
                  PhotoUrl = photo != null ? $"data:{photo.ImgType};base64,{Convert.ToBase64String(photo.Photo)}" : "/images/noImage.png",
                  DigitalSignatureUrl = signature != null ? $"data:{signature.ImgType};base64,{Convert.ToBase64String(signature.DigitalSignature)}" : "/images/signature.jpg"

              }).FirstOrDefaultAsync();

            return data;
        }

        #endregion

        #region GetEmployeeDropSelections

        public async Task<IEnumerable<CommonSelectModel>> GetEmployeeDropSelections()
        {
            return await empRepository.All()
                .Select(u => new CommonSelectModel
                {
                    Code = u.EmployeeId,
                    Name = string.Format("{0} {1} ({2})", u.FirstName, u.LastName, u.EmployeeId)
                })
                .ToListAsync();
        }


        #endregion

        #region SaveAsync

        public async Task<bool> SaveAsync(HrmEmployee2SetUpViewModel entityVM, string CompanyCode)
        {
            await empRepository.BeginTransactionAsync();
            try
            {
                HrmEmployee entity = new HrmEmployee();
                entity.EmployeeId = entityVM.EmployeeId;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.BranchCode = entityVM.BranchCode;
                entity.ReligionCode = entityVM.ReligionCode ?? string.Empty;
                entity.SexCode = entityVM.SexCode ?? string.Empty;
                entity.NationalityCode = entityVM.NationalityCode ?? string.Empty;
                entity.MaritalStatusCode = entityVM.MaritalStatusCode ?? string.Empty;
                entity.BloodGroupCode = entityVM.BloodGroupCode ?? string.Empty;
                entity.FirstName = entityVM.FirstName ?? string.Empty;
                entity.LastName = entityVM.LastName ?? string.Empty;
                entity.FirstNameBangla = entityVM.FirstNameBangla;
                entity.LastNameBangla = entityVM.LastNameBangla;
                entity.FatherName = entityVM.FatherName ?? string.Empty;
                entity.MotherName = entityVM.MotherName ?? string.Empty;
                entity.FathersOccupation = entityVM.FatherOccupation;
                entity.MothersOccupation = entityVM.MotherOccupation;
                entity.DateOfBirthOrginal = entityVM.DateOfBirthOrginal;
                entity.DateOfBirthCertificate = entityVM.DateOfBirthCertificate;
                entity.BirthCertificateNo = entityVM.BirthCertificateNo ?? string.Empty;
                entity.PlaceOfBirth = entityVM.PlaceOfBirth ?? string.Empty;
                entity.NationalIdno = entityVM.NationalIdno ?? string.Empty;
                entity.ExtraCurriActivities = entityVM.ExtraCurriActivities;
                entity.MarriageDate = entityVM.MarriageDate;
                entity.Spouse = entityVM.Spouse;
                entity.NoOfSon = entityVM.NoOfSon ?? string.Empty;
                entity.NoOfDaughters = entityVM.NoOfDaughters ?? string.Empty;
                entity.PersonalEmail = entityVM.PersonalEmail ?? string.Empty;
                entity.Telephone = entityVM.Telephone ?? string.Empty;
                entity.CardNo = entityVM.CardNo ?? string.Empty;
                entity.TinNo = entityVM.TinNo;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.Ldate = DateTime.Now;
                entity.CompanyCode = CompanyCode;
                entity.UserInfoEmployeeId = entityVM.UserInfoEmployeeId ?? string.Empty;
                await empRepository.AddAsync(entity);
                //

                if (entityVM.Photo != null && entityVM.Photo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {

                        await entityVM.Photo.CopyToAsync(memoryStream);


                        HrmEmployeePhoto photo = new HrmEmployeePhoto
                        {
                            EmployeeId = entity.EmployeeId,
                            Photo = memoryStream.ToArray(),
                            ImgType = entityVM.Photo.ContentType,
                            ImgSize = entityVM.Photo.Length
                        };

                        // Add photo to the database
                        await photoRepository.AddAsync(photo);
                    }
                }

                if (entityVM.Signature != null && entityVM.Signature.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {

                        await entityVM.Signature.CopyToAsync(memoryStream);
                        HrmEmpDigitalSignature signature = new HrmEmpDigitalSignature
                        {
                            EmployeeId = entity.EmployeeId,
                            DigitalSignature = memoryStream.ToArray(),
                            ImgType = entityVM.Signature.ContentType,
                            ImgSize = entityVM.Signature.Length
                        };


                        await signatureRepository.AddAsync(signature);
                    }
                }
                //
                await empRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex.Message);
                await empRepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region UpdateAsync

        public async Task<bool> UpdateAsync(HrmEmployee2SetUpViewModel entityVM)
        {
            await empRepository.BeginTransactionAsync();
            try
            {
                var entity = await empRepository.GetByIdAsync(entityVM.EmployeeId);
                if (entity == null)
                {
                    await empRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.EmployeeId = entityVM.EmployeeId;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.BranchCode = entityVM.BranchCode;
                entity.ReligionCode = entityVM.ReligionCode ?? string.Empty;
                entity.SexCode = entityVM.SexCode ?? string.Empty;
                entity.NationalityCode = entityVM.NationalityCode ?? string.Empty;
                entity.MaritalStatusCode = entityVM.MaritalStatusCode ?? string.Empty;
                entity.BloodGroupCode = entityVM.BloodGroupCode ?? string.Empty;
                entity.FirstName = entityVM.FirstName ?? string.Empty;
                entity.LastName = entityVM.LastName ?? string.Empty;
                entity.FirstNameBangla = entityVM.FirstNameBangla;
                entity.LastNameBangla = entityVM.LastNameBangla;
                entity.FatherName = entityVM.FatherName ?? string.Empty;
                entity.MotherName = entityVM.MotherName ?? string.Empty;
                entity.FathersOccupation = entityVM.FatherOccupation;
                entity.MothersOccupation = entityVM.MotherOccupation;
                entity.DateOfBirthOrginal = entityVM.DateOfBirthOrginal;
                entity.DateOfBirthCertificate = entityVM.DateOfBirthCertificate;
                entity.BirthCertificateNo = entityVM.BirthCertificateNo ?? string.Empty;
                entity.PlaceOfBirth = entityVM.PlaceOfBirth ?? string.Empty;
                entity.NationalIdno = entityVM.NationalIdno ?? string.Empty;
                entity.ExtraCurriActivities = entityVM.ExtraCurriActivities;
                entity.MarriageDate = entityVM.MarriageDate;
                entity.Spouse = entityVM.Spouse;
                entity.NoOfSon = entityVM.NoOfSon ?? string.Empty;
                entity.NoOfDaughters = entityVM.NoOfDaughters ?? string.Empty;
                entity.PersonalEmail = entityVM.PersonalEmail ?? string.Empty;
                entity.Telephone = entityVM.Telephone ?? string.Empty;
                entity.CardNo = entityVM.CardNo ?? string.Empty;
                entity.TinNo = entityVM.TinNo;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await empRepository.UpdateAsync(entity);
               
                if (entityVM.IsClearPhoto)
                {
                    var existingPhoto = await photoRepository.All().Where(x => x.EmployeeId == entity.EmployeeId).ToListAsync();
                    await photoRepository.DeleteRangeAsync(existingPhoto);
                }

                if (entityVM.Photo != null && entityVM.Photo.Length > 0)
                {
                    var existingPhotos = await photoRepository.All().Where(x => x.EmployeeId == entity.EmployeeId).ToListAsync();
                    if (existingPhotos.Any())
                    {
                        await photoRepository.DeleteRangeAsync(existingPhotos);
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await entityVM.Photo.CopyToAsync(memoryStream);

                        HrmEmployeePhoto photo = new HrmEmployeePhoto
                        {
                            EmployeeId = entity.EmployeeId,
                            Photo = memoryStream.ToArray(),
                            ImgType = entityVM.Photo.ContentType,
                            ImgSize = entityVM.Photo.Length
                        };

                        await photoRepository.UpdateAsync(photo);
                    }
                }
                else
                {
                    var existingPhoto = await photoRepository.All().Where(x => x.EmployeeId == entity.EmployeeId).FirstOrDefaultAsync();
                    if (existingPhoto != null)
                    {
                        existingPhoto.ImgType = existingPhoto.ImgType;
                        existingPhoto.ImgSize = existingPhoto.ImgSize;
                        await photoRepository.UpdateAsync(existingPhoto);
                    }
                }

                //Signature Update

                if (entityVM.IsClearSignature)
                {
                    var existingSignature = await signatureRepository.All().Where(x => x.EmployeeId == entity.EmployeeId).ToListAsync();
                    await signatureRepository.DeleteRangeAsync(existingSignature);

                }
                if (entityVM.Signature != null && entityVM.Signature.Length > 0)
                {
                    var existingSignature = signatureRepository.All().Where(x => x.EmployeeId == entity.EmployeeId).ToList();
                    if (existingSignature.Any())
                    {
                        await signatureRepository.DeleteRangeAsync(existingSignature);
                    }
                    using (var memoryStream = new MemoryStream())
                    {
                        await entityVM.Signature.CopyToAsync(memoryStream);

                        HrmEmpDigitalSignature signature = new HrmEmpDigitalSignature
                        {
                            EmployeeId = entity.EmployeeId,
                            DigitalSignature = memoryStream.ToArray(),
                            ImgType = entityVM.Signature.ContentType,
                            ImgSize = entityVM.Signature.Length
                        };

                        await signatureRepository.UpdateAsync(signature);
                    }
                }
                else
                {

                    var existingSignature = await signatureRepository.All().Where(x => x.EmployeeId == entity.EmployeeId).FirstOrDefaultAsync();
                    if (existingSignature != null)
                    {
                        existingSignature.ImgType = existingSignature.ImgType;
                        existingSignature.ImgSize = existingSignature.ImgSize;
                        await signatureRepository.UpdateAsync(existingSignature);
                    }
                }

                //End Update Signatue 
                await empRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await empRepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region GenearateNextCode

        public async Task<string> GenerateNextCode()
        {
            var code = await empRepository.GetAllAsync();
            var lastCode = code.Max(x => x.EmployeeId);

            long nextCode = 1;

            if (!string.IsNullOrEmpty(lastCode))
            {
                string numericPart = new string(lastCode.Where(char.IsDigit).ToArray());

                if (long.TryParse(numericPart, out long lastNumber))
                {
                    lastNumber++;
                    nextCode = lastNumber;
                }
                else
                {
                    nextCode = 1;
                }
            }

            return nextCode.ToString();
        }

        #endregion

        #region Delelete

        public async Task<(bool isSuccess, string message, bool refError)> DeleteLeaveType(List<string> ids, DeleteHistoryViewModel model)
        {
            string coloum = "EmployeeId";
            if(ids == null && !ids.Any())
            {
                return (false,"Error Deleting Employee",false);
            }

            try
            {
                var entity = await empRepository.All().Where(e => ids.Contains(e.EmployeeId)).AsNoTracking().ToListAsync();

                if (!entity.Any())
                {
                    return(false, "Employee not found", false);
                }

                var dependencyCheck = await deleteHistoryRepository.CheckDependenciesAsync(model.tableName, coloum, ids);
                if(dependencyCheck!=null && !dependencyCheck.CanDelete)
                    return (false, dependencyCheck.Message, true);

                await empRepository.BeginTransactionAsync();

                var photos = await photoRepository.All().Where(p => ids.Contains(p.EmployeeId)).ToListAsync();
                await photoRepository.DeleteRangeAsync(photos);

                var sinature = await signatureRepository.All().Where(s => ids.Contains(s.EmployeeId)).ToListAsync();
                await signatureRepository.DeleteRangeAsync(sinature);

                await empRepository.DeleteRangeAsync(entity);
                return (true, "Employee deleted successfully", false);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex.Message);
                await empRepository.RollbackTransactionAsync();
                return (false, "Delete Failed", false);
            }

        }

        public async Task<HrmEmployee> GetLeaveType(string code)
        {
            return await empRepository.GetByIdAsync(code);
        }

        #endregion

        #region DuplicateCheck

        // NationalIdno BirthCertificateNo
        public async Task<bool> IsExistByAsync(string code, string firstName, string dateOfBirthOriginal, string fathersName)
        {
            DateTime? dOB = null;
            if (DateTime.TryParseExact(dateOfBirthOriginal, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                dOB = result.Date;
            }

            return await empRepository.All().AnyAsync(x =>
            x.EmployeeId != code &&
            x.FirstName == firstName &&
            x.DateOfBirthOrginal == dOB
            //(x.NationalIdno==nationalIdno || x.BirthCertificateNo==birthCertificateNo)

            );
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Information System" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Information System" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Information System" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Information System" && x.CheckDelete);
        }

        #endregion

    }
}