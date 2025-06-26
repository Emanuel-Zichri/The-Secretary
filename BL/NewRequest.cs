namespace FinalProject.BL

{ 

	public class NewRequest
	{
			public Customer customerDetails {  get; set; }
			public SpaceDetails [] spaceDetails { get; set; }
        public DateTime PreferredDate { get; set; }
        public int PreferredSlot { get; set; }
        public string selectedParquet { get; set; }

    }
}

