namespace FinalProject.BL
{
    public class CalculatorItemCandidate
    {
        public int CandidateID { get; set; }
        public string CustomItemName { get; set; }
        public decimal SuggestedPrice { get; set; }
        public string SuggestedDescription { get; set; }
        public int Occurrences { get; set; }
        public CalculatorItemCandidate(){}
        
        
                    public static List<CalculatorItemCandidate> GetCandidates()
        {
            DBservices db = new DBservices();
            return db.GetUncreatedPopularCandidates(2); // מציג פריטים עם 2+ שימושים
        }
        public static int AddCalcItem(string item)
        {
            DBservices db = new DBservices();
            return db.CreatePriceCalculatorItemFromCandidate(item);
        }
        public static int RejectCandidate(string item)
        {
            DBservices db = new DBservices();
            return db.RejectCalculatorItemCandidate(item);
        }



    }

}
