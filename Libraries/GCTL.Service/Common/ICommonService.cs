namespace GCTL.Service.Common
{
    public interface ICommonService
    {
        void FindAccFiveDigit(ref string strMaxNo, string strFldName, string strTableName, int intLenWithPadding, string WhereColumn, string WhereValue);
        void FindAccFourDigit(ref string strMaxNo, string strFldName, string strTableName, int intLenWithPadding, string WhereColumn, string WhereValue);
        void FindAccThreeDigit(ref string strMaxNo, string strFldName, string strTableName, int intLenWithPadding, string WhereColumn, string WhereValue);
        void FindAccTwoDigit(ref string strMaxNo, string strFldName, string strTableName, int intLenWithPadding, string WhereColumn, string WhereValue);
        void FindMaxGCTL(ref string strMaxNo, string strFldName, string strTableName, int intLenWithPadding, string WhereColumn, string WhereValue);
        void FindMaxNo(ref string strMaxNo, string strFldName, string strTableName, int intLenWithPadding);
        void FindMaxNoAuto(ref string strMaxNo, string strFldName, string strTableName);
        string NextCode(string fieldName, string table, int length);
        string GenerateCode(string columnName, string tableName, string prefix = "", int length = 8);
        string GenerateNextCode(string field, string table, int length = 3, string prefix = "");
        public string GenerateNextNumber(string field, string table, int length = 2, string prefix = "");

        void FindVoucherNo(ref string strMaxNo, string VoucherType_Code);
        void MaxNoWithYearAndTwoDight(ref string strMaxNo, string strFldName, string strTableName, int intLenWithPadding,string StartTwoDight );

        //Check references for a record
        //List<string> CheckReferenceBeforeDelete(string tableName, string keyField, object keyValue);

        //Task SaveDeleteHistoryAsync(string tableName, object deleteedRow);
        Task<bool> LogDeletedRecordsAsync<T>(List<T> entities, string tableName) where T : class;

        //List<(string TableName, string ControllerName)> CheckReferenceBeforeDelete(string tableName, string keyField, object keyValue);
        List<(string TableName, string ControllerName, string Title)> CheckReferenceBeforeDelete(string tableName, string keyField, object keyValue);
    }
}