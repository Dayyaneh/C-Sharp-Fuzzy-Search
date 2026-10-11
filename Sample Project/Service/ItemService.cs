using FuzzySearchProject.Entities;

namespace FuzzySearchProject.Service
{
    public class ItemService
    {
        public List<ItemEntity> GenerateDummyItems()
        {
            List<(string Manufacturer, string Category, string[] Names)> itemGroups = new()
            {
                ("Acme Safety", "Hand Protection", new[]
                {
                    "Safety Gloves",
                    "Safty Gloves",
                    "Safety Glvoes",
                    "Safety Work Gloves",
                    "Cut Resistant Safety Gloves"
                }),
                ("Acme Safety", "Hand Protection", new[]
                {
                    "Nitrile Gloves",
                    "Nitril Gloves",
                    "Nitrile Golves",
                    "Nitrile Disposable Gloves",
                    "Nitrile Examination Gloves"
                }),
                ("Acme Safety", "Hand Protection", new[]
                {
                    "Latex Gloves",
                    "Ltaex Gloves",
                    "Latex Glvoes",
                    "Latex Disposable Gloves",
                    "Powder Free Latex Gloves"
                }),
                ("Acme Safety", "Hand Protection", new[]
                {
                    "Leather Gloves",
                    "Leater Gloves",
                    "Leather Golves",
                    "Leather Work Gloves",
                    "Leather Welding Gloves"
                }),
                ("Acme Safety", "Eye Protection", new[]
                {
                    "Safety Glasses",
                    "Safty Glasses",
                    "Safety Glases",
                    "Clear Safety Glasses",
                    "Anti Fog Safety Glasses"
                }),
                ("Acme Safety", "Eye Protection", new[]
                {
                    "Safety Goggles",
                    "Saftey Goggles",
                    "Safety Gogles",
                    "Protective Safety Goggles",
                    "Anti Fog Safety Goggles"
                }),
                ("Acme Safety", "Face Protection", new[]
                {
                    "Face Shield",
                    "Face Sheild",
                    "Fae Shield",
                    "Full Face Shield",
                    "Clear Face Shield"
                }),
                ("Acme Safety", "Respiratory Protection", new[]
                {
                    "Dust Mask",
                    "Dust Maks",
                    "Dust Msk",
                    "Disposable Dust Mask",
                    "Reusable Dust Mask"
                }),
                ("Acme Safety", "Hearing Protection", new[]
                {
                    "Ear Plugs",
                    "Ear Plgus",
                    "Ear Plgs",
                    "Foam Ear Plugs",
                    "Reusable Ear Plugs"
                }),
                ("Acme Safety", "Hearing Protection", new[]
                {
                    "Ear Muffs",
                    "Ear Mufs",
                    "Ear Mufffs",
                    "Noise Reduction Ear Muffs",
                    "Adjustable Ear Muffs"
                }),
                ("Northwind Tools", "Hand Tools", new[]
                {
                    "Phillips Screwdriver",
                    "Philips Screwdriver",
                    "Phillips Screwdrier",
                    "Phillips Screwdriver Set",
                    "Long Phillips Screwdriver"
                }),
                ("Northwind Tools", "Hand Tools", new[]
                {
                    "Flathead Screwdriver",
                    "Flathed Screwdriver",
                    "Flathead Screwdiver",
                    "Flathead Screwdriver Set",
                    "Insulated Flathead Screwdriver"
                }),
                ("Northwind Tools", "Hand Tools", new[]
                {
                    "Torx Screwdriver",
                    "Trox Screwdriver",
                    "Torx Screwdrvier",
                    "Torx Screwdriver Set",
                    "Precision Torx Screwdriver"
                }),
                ("Northwind Tools", "Hand Tools", new[]
                {
                    "Precision Screwdriver",
                    "Precison Screwdriver",
                    "Precision Screwdrier",
                    "Precision Screwdriver Set",
                    "Precision Phillips Screwdriver"
                }),
                ("Northwind Tools", "Hand Tools", new[]
                {
                    "Adjustable Wrench",
                    "Adjustble Wrench",
                    "Adjustable Wernch",
                    "Small Adjustable Wrench",
                    "Large Adjustable Wrench"
                }),
                ("Northwind Tools", "Hand Tools", new[]
                {
                    "Combination Wrench",
                    "Combinaton Wrench",
                    "Combination Wrenhc",
                    "Metric Combination Wrench",
                    "Combination Wrench Set"
                }),
                ("Northwind Tools", "Hand Tools", new[]
                {
                    "Socket Wrench",
                    "Soket Wrench",
                    "Socket Wrenhc",
                    "Socket Wrench Set",
                    "Ratcheting Socket Wrench"
                }),
                ("Northwind Tools", "Hand Tools", new[]
                {
                    "Torque Wrench",
                    "Torqe Wrench",
                    "Torque Wernch",
                    "Digital Torque Wrench",
                    "Adjustable Torque Wrench"
                }),
                ("Northwind Tools", "Hand Tools", new[]
                {
                    "Needle Nose Pliers",
                    "Needel Nose Pliers",
                    "Needle Nose Plires",
                    "Long Needle Nose Pliers",
                    "Bent Needle Nose Pliers"
                }),
                ("Northwind Tools", "Hand Tools", new[]
                {
                    "Cutting Pliers",
                    "Cuting Pliers",
                    "Cutting Plieres",
                    "Diagonal Cutting Pliers",
                    "Heavy Duty Cutting Pliers"
                }),
                ("Northwind Tools", "Hand Tools", new[]
                {
                    "Locking Pliers",
                    "Lockng Pliers",
                    "Locking Plires",
                    "Curved Jaw Locking Pliers",
                    "Long Nose Locking Pliers"
                }),
                ("Northwind Tools", "Hand Tools", new[]
                {
                    "Claw Hammer",
                    "Calw Hammer",
                    "Claw Hamer",
                    "Steel Claw Hammer",
                    "Fiberglass Claw Hammer"
                }),
                ("Northwind Tools", "Hand Tools", new[]
                {
                    "Rubber Mallet",
                    "Ruber Mallet",
                    "Rubber Malelt",
                    "Soft Rubber Mallet",
                    "Heavy Duty Rubber Mallet"
                }),
                ("Contoso Tools", "Measuring Tools", new[]
                {
                    "Measuring Tape",
                    "Measurng Tape",
                    "Measuring Tpae",
                    "Measuring Tape 5m",
                    "Measuring Tape 10m"
                }),
                ("Contoso Tools", "Measuring Tools", new[]
                {
                    "Digital Caliper",
                    "Digtial Caliper",
                    "Digital Calipr",
                    "Digital Caliper 150mm",
                    "Digital Caliper 200mm"
                }),
                ("Fabrikam Tools", "Power Tool Accessories", new[]
                {
                    "Drill Bit",
                    "Dril Bit",
                    "Drill Bti",
                    "Steel Drill Bit",
                    "Masonry Drill Bit"
                }),
                ("Fabrikam Tools", "Power Tool Accessories", new[]
                {
                    "Drill Bit Set",
                    "Dril Bit Set",
                    "Drill Bti Set",
                    "Metric Drill Bit Set",
                    "Titanium Drill Bit Set"
                }),
                ("Fabrikam Tools", "Power Tools", new[]
                {
                    "Cordless Drill",
                    "Cordles Drill",
                    "Cordless Dril",
                    "Cordless Hammer Drill",
                    "Cordless Drill 18V"
                }),
                ("Fabrikam Tools", "Power Tools", new[]
                {
                    "Cordless Impact Driver",
                    "Cordles Impact Driver",
                    "Cordless Impact Drvier",
                    "Compact Cordless Impact Driver",
                    "Cordless Impact Driver 18V"
                }),
                ("Northwind Tools", "Cutting Tools", new[]
                {
                    "Utility Knife",
                    "Utilty Knife",
                    "Utility Knfie",
                    "Retractable Utility Knife",
                    "Folding Utility Knife"
                }),
                ("Northwind Tools", "Cutting Tool Accessories", new[]
                {
                    "Utility Knife Blade",
                    "Utility Kife Blade",
                    "Utility Knife Balde",
                    "Utility Knife Blade Pack",
                    "Heavy Duty Utility Knife Blade"
                }),
                ("Contoso Supplies", "Cable Management", new[]
                {
                    "Cable Tie",
                    "Calbe Tie",
                    "Cable Tye",
                    "Black Cable Tie",
                    "Reusable Cable Tie"
                }),
                ("Contoso Supplies", "Cable Management", new[]
                {
                    "Cable Tie Mount",
                    "Cable Tie Mout",
                    "Cable Tye Mount",
                    "Adhesive Cable Tie Mount",
                    "Screw Mount Cable Tie Mount"
                }),
                ("Contoso Supplies", "Electrical Supplies", new[]
                {
                    "Electrical Tape",
                    "Electical Tape",
                    "Electrical Tpae",
                    "Black Electrical Tape",
                    "Red Electrical Tape"
                }),
                ("Contoso Supplies", "Adhesives", new[]
                {
                    "Duct Tape",
                    "Dcut Tape",
                    "Duct Tpae",
                    "Silver Duct Tape",
                    "Heavy Duty Duct Tape"
                }),
                ("Contoso Supplies", "Packaging Supplies", new[]
                {
                    "Packing Tape",
                    "Paking Tape",
                    "Packing Tpae",
                    "Clear Packing Tape",
                    "Brown Packing Tape"
                }),
                ("Fabrikam Fasteners", "Fasteners", new[]
                {
                    "Hex Bolt",
                    "Hex Blot",
                    "Hex Boltt",
                    "Hex Bolt M6",
                    "Hex Bolt M8"
                }),
                ("Fabrikam Fasteners", "Fasteners", new[]
                {
                    "Hex Nut",
                    "Hex Ntu",
                    "Hex Nutt",
                    "Hex Nut M6",
                    "Hex Nut M8"
                }),
                ("Fabrikam Fasteners", "Fasteners", new[]
                {
                    "Flat Washer",
                    "Flat Waser",
                    "Flat Wahser",
                    "Flat Washer M6",
                    "Flat Washer M8"
                }),
                ("Fabrikam Fasteners", "Fasteners", new[]
                {
                    "Lock Washer",
                    "Lock Waser",
                    "Lock Wahser",
                    "Split Lock Washer",
                    "Toothed Lock Washer"
                })
            };

            int[] quantities = { 17, 34, 0 };
            List<ItemEntity> items = new List<ItemEntity>(200);
            long itemId = 1;

            foreach ((string manufacturer, string category, string[] names) in itemGroups)
            {
                foreach (string itemName in names)
                {
                    items.Add(new ItemEntity
                    {
                        ItemId = itemId,
                        ItemName = itemName,
                        Manufacturer = manufacturer,
                        Category = category,
                        Description = $"Dummy inventory item in the {category} category.",
                        PartNumber = $"TEST-{itemId:D4}",
                        Quantity = quantities[(int)((itemId - 1) % quantities.Length)]
                    });

                    itemId++;
                }
            }

            return items;
        }
    }
}
