namespace FinalProject.BL
{
    public class ParquetType
    {
        public int ParquetTypeID { get; set; }
        public string? TypeName { get; set; }
        public decimal? PricePerUnit { get; set; }
        public string? ImageURL { get; set; }
        public bool? IsActive { get; set; }

        public static int AddParquetType(ParquetType parquetType)
        {
            DBservices dbs = new DBservices();
            return dbs.AddParquetType(parquetType);
        }
        public static int updateParquetType(ParquetType parquetType)
        {
            DBservices dbs = new DBservices();
            return dbs.updateParquetType(parquetType);
        }
    }

}
